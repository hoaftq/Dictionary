
using DataAccess.Data;
using DataAccess.Models;
using HtmlAgilityPack;
using log4net.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CollectData
{
    class TratuParser
    {
        private const int NumberOfConcurrentProcessingWords = 20;

        private const int NumberOfConcurrentSavingWords = 1000;

        private static readonly WordClass UnknownWordClass = new WordClass() { Name = "Unknown" };

        private static readonly ILogger logger = LoggerManager.GetLogger(Assembly.GetEntryAssembly(), typeof(TratuParser));

        private readonly DictionaryContext context;

        private readonly ConcurrentQueue<Word> wordQueue = new ConcurrentQueue<Word>();

        private int totalPageCount = 0;

        private int totalWordCount = 0;

        private int successWordCount = 0;

        private CancellationState cancellationState;

        private CancellationState resumeState;

        private bool isReachedCancellationPage = false;

        private bool isReachedCancellationWord = false;

        public TratuParser(DictionaryContext context)
        {
            this.context = context;
        }

        public void Parse(string href = null, CancellationToken? parserToken = null)
        {
            var tokenSource = new CancellationTokenSource();

            // A thread to save ready words to database
            // We need a foreground thread here (not a task), if not when the main thread finishes it will terminate the thread before saving data completely
            var thread = new Thread(() =>
            {
                while (!tokenSource.Token.IsCancellationRequested)
                {
                    var words = new List<Word>();
                    for (int i = 0; i < NumberOfConcurrentSavingWords; i++)
                    {
                        if (!wordQueue.TryDequeue(out Word word))
                        {
                            break;
                        }

                        words.Add(word);
                    }

                    // Save many words at the same time to improve performance
                    if (words.Count > 0)
                    {
                        SaveWords(words);
                    }
                }
            });
            thread.Start();


            if (href == null || href.EndsWith("/special:allpages", StringComparison.OrdinalIgnoreCase))
            {
                // If there is a saved file, the process will start over from the state in this file
                // So if we want to process from the begining, the saved file has to be deleted
                resumeState = CancellationUtil.Restore();

                var url = "http://tratu.soha.vn/dict/en_vn/special:allpages";
                logger.Log(GetType(), Level.Info, $"Processing all pages at {url}", null);

                var htmlWeb = new HtmlWeb();
                var htmlDoc = htmlWeb.Load(url);

                foreach (var link in htmlDoc.DocumentNode.SelectNodes("//table[@class='allpageslist']/tr/td[1]/a[@href]"))
                {
                    var pageUrl = link.Attributes["href"].Value;
                    if (resumeState != null && !isReachedCancellationPage)
                    {
                        // Reach the stopped page, turn the flag to on so that all the following pages will be proccessed
                        if (pageUrl == resumeState.PageUrl)
                        {
                            isReachedCancellationPage = true;
                        }
                        else
                        {
                            // Ignore pages before saved page
                            logger.Log(GetType(), Level.Info, $"Ignored page {pageUrl}", null);
                            continue;
                        }
                    }

                    // Parser is requested to stop
                    if (parserToken?.IsCancellationRequested ?? false)
                    {
                        // Just need to save the state if it's canceled at a later point
                        if (resumeState == null || isReachedCancellationPage)
                        {
                            cancellationState = new CancellationState()
                            {
                                PageUrl = pageUrl
                            };
                            logger.Log(GetType(), Level.Info, $"Process is being cancelled at {cancellationState.PageUrl}", null);
                        }
                        break;
                    }

                    ParsePage(pageUrl, parserToken);

                    // Parser is requested to stop
                    if (parserToken?.IsCancellationRequested ?? false)
                    {
                        break;
                    }
                }
            }
            else if (href.Contains("%C4%90%E1%BA%B7c_bi%E1%BB%87t:Allpages/", StringComparison.OrdinalIgnoreCase))
            {
                ParsePage(href);
            }
            else
            {
                ParseWord(href);
            }

            // All words has been processed
            while (wordQueue.Count > 0)
            {
            }

            // Signal to end the register thread
            tokenSource.Cancel();

            // Wait for register thread to end
            thread.Join();

            // Only save the state if there is a cancellation
            if (cancellationState != null)
            {
                CancellationUtil.Save(cancellationState);
            }

            logger.Log(GetType(), Level.Info, $"{totalWordCount} words were read from {totalPageCount} pages, {successWordCount} words were registered successfuly to database", null);
        }

        public void ParsePage(string href, CancellationToken? parserToken = null)
        {
            var url = CreateFullUrl(href);
            logger.Log(GetType(), Level.Info, $"Processing a page at {url}", null);

            var htmlDoc = LoadPageWithTimeout(url, 1);
            if (htmlDoc == null)
            {
                htmlDoc = LoadPageWithTimeout(url, 1);
            }
            if (htmlDoc == null)
            {
                logger.Log(GetType(), Level.Error, $"Loading page {url} timed out", null);
            }

            IEnumerable<HtmlNode> wordNodes = htmlDoc.DocumentNode.SelectNodes("//div[@id='bodyContent']/table[2]//td/a");

            do
            {
                var firstWordUrl = wordNodes.First().Attributes["href"].Value;
                if (resumeState?.WordUrl != null && !isReachedCancellationWord)
                {
                    if (firstWordUrl == resumeState.WordUrl)
                    {
                        isReachedCancellationWord = true;
                    }
                    else
                    {
                        logger.Log(GetType(), Level.Info, $"Ignored a block of {NumberOfConcurrentProcessingWords} words starting at {firstWordUrl}", null);
                        wordNodes = wordNodes.Skip(NumberOfConcurrentProcessingWords);
                        continue;
                    }
                }

                // Parser is requested to stop
                if (parserToken?.IsCancellationRequested ?? false)
                {
                    if (resumeState?.WordUrl == null || isReachedCancellationWord)
                    {
                        cancellationState = new CancellationState()
                        {
                            PageUrl = href,
                            WordUrl = wordNodes.First().Attributes["href"].Value
                        };
                        logger.Log(GetType(), Level.Info, $"Process is being cancelled at {cancellationState.PageUrl}, {cancellationState.WordUrl}", null);
                    }

                    break;
                }

                var consecutiveNodes = wordNodes.Take(NumberOfConcurrentProcessingWords);

                // Load a block of words at a time and wait for that to complete before moving to another block
                var parserTasks = new List<Task>();
                foreach (var w in consecutiveNodes)
                {
                    parserTasks.Add(Task.Run(() =>
                    {
                        var word = ReadWord(w.Attributes["href"].Value);
                        if (word != null)
                        {
                            wordQueue.Enqueue(word);
                        }
                    }));
                }
                Task.WaitAll(parserTasks.ToArray());

                wordNodes = wordNodes.Skip(NumberOfConcurrentProcessingWords);
            } while (wordNodes.Count() > 0);
        }

        public void ParseWord(string href)
        {
            var word = ReadWord(href);
            if (word != null)
            {
                wordQueue.Enqueue(word);
            }
        }

        private void SaveWords(List<Word> words)
        {
            var subDictionaries = new List<SubDictionary>();
            var wordClasses = new List<WordClass>();

            SubDictionary GetExistedSubDictionary(SubDictionary subDict)
            {
                // Comparation is case insensitive in database!
                var reusedSubDict = subDictionaries.SingleOrDefault(s => s.Name.Equals(subDict.Name, StringComparison.OrdinalIgnoreCase));
                if (reusedSubDict != null)
                {
                    return reusedSubDict;
                }

                reusedSubDict = context.SubDictionaries.SingleOrDefault(s => s.Name == subDict.Name);
                if (reusedSubDict != null)
                {
                    subDictionaries.Add(reusedSubDict);
                    return reusedSubDict;
                }

                subDictionaries.Add(subDict);
                return subDict;
            }

            WordClass GetExistedWordClass(WordClass wordClass)
            {
                var reusedWordClass = wordClasses.SingleOrDefault(w => w.Name.Equals(wordClass.Name, StringComparison.OrdinalIgnoreCase));
                if (reusedWordClass != null)
                {
                    return reusedWordClass;
                }

                reusedWordClass = context.WordClasses.SingleOrDefault(w => w.Name == wordClass.Name);
                if (reusedWordClass != null)
                {
                    wordClasses.Add(reusedWordClass);
                    return reusedWordClass;
                }

                wordClasses.Add(wordClass);
                return wordClass;
            }

            foreach (var word in words)
            {
                // If a dictionary or a word class already exists then use it, do not create a new one
                if (word.Definitions != null)
                {
                    foreach (var def in word.Definitions)
                    {
                        def.SubDictionary = GetExistedSubDictionary(def.SubDictionary);
                        def.WordClass = GetExistedWordClass(def.WordClass);
                    }
                }

                if (word.Phases != null)
                {
                    foreach (var p in word.Phases)
                    {
                        p.SubDictionary = GetExistedSubDictionary(p.SubDictionary);
                    }
                }
            }

            try
            {
                totalWordCount += words.Count;

                context.Words.AddRange(words);
                context.SaveChanges();

                successWordCount += words.Count;
                logger.Log(GetType(), Level.Info, $"{words.Count} word(s) '{string.Join(", ", words.Select(w => w.Content))}' were registered successfully", null);
            }
            catch (DbUpdateException)
            {
                logger.Log(GetType(), Level.Info, $"Could not save {words.Count} word(s) '{string.Join(", ", words.Select(w => w.Content))}', trying to register each word individually", null);

                // The words could not be saved, remove them from the context
                // TODO is this enough?
                //context.Words.RemoveRange(words);
                foreach (var word in words)
                {
                    context.Entry(word).State = EntityState.Detached;
                }

                // Try to register each word separately
                foreach (var word in words)
                {
                    try
                    {
                        //context.Words.Add(word);
                        context.Entry(word).State = EntityState.Added;
                        context.SaveChanges();

                        successWordCount++;
                        logger.Log(GetType(), Level.Info, $"The word '{word.Content}' was registered successfully", null);
                    }
                    catch (DbUpdateException ex)
                    {
                        //context.Words.Remove(word);
                        context.Entry(word).State = EntityState.Detached;
                        logger.Log(GetType(), Level.Error, $"Could not save the word '{word.Content}' to database", ex);
                    }
                }
            }
        }

        private Word ReadWord(string href)
        {
            var url = CreateFullUrl(href);
            logger.Log(GetType(), Level.Info, $"Geting a new word at {url}", null);

            Interlocked.Increment(ref totalPageCount);

            try
            {
                var htmlDoc = LoadPageWithTimeout(url, 1);

                // Reload the page if it timed out or was not fully loaded
                if (htmlDoc == null || htmlDoc.DocumentNode.SelectSingleNode("//div[@id='bodyContent']") == null)
                {
                    htmlDoc = LoadPageWithTimeout(url, 1);
                }

                if (htmlDoc == null)
                {
                    logger.Log(GetType(), Level.Error, $"Loading page {url} timed out", null);
                    return null;
                }

                if (htmlDoc.DocumentNode.SelectSingleNode("//div[@id='bodyContent']") == null)
                {
                    logger.Log(GetType(), Level.Error, $"Page {url} was not fully loaded", null);
                    return null;
                }

                var word = new Word();
                var keywords = htmlDoc.DocumentNode.SelectNodes("//head/meta")
                    .FirstOrDefault(n => n.GetAttributeValue("name", null) == "keywords")
                    ?.GetAttributeValue("content", string.Empty);
                word.Content = keywords.Split(',')[0];

                var contentNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='bodyContent']");
                var children = contentNode.SelectNodes("./div");
                var spellingNode = children.Select(c => c.SelectSingleNode(".//font[@color='red']/text()"))
                    .Where(n => n != null)
                    .FirstOrDefault();
                var spelling = spellingNode?.InnerText.TrimAllSpecialCharacters();
                word.Spelling = spelling != "Phiên âm này đang chờ bạn hoàn thiện" ? spelling : null;
                word.SpellingAudioUrl = null; // The audio link does not work anymore!

                var subDictionaryNodes = contentNode.SelectNodes("./div[@class='section-h2']");
                if (subDictionaryNodes == null)
                {
                    logger.Log(GetType(), Level.Error, $"Could not find any dictionary node at {url}", null);
                    return null;
                }

                var relativeWordsNode = subDictionaryNodes.SingleOrDefault(d => d.SelectSingleNode("./h2/span")?.InnerHtml.Contains("Các từ liên quan") == true);
                if (relativeWordsNode != null)
                {
                    subDictionaryNodes.Remove(relativeWordsNode);
                    word.RelativeWords = ReadRelativeWords(relativeWordsNode);
                }

                word.Definitions = new List<Definition>();
                word.WordForms = new List<WordForm>();
                foreach (var d in subDictionaryNodes)
                {
                    ReadDictionary(d, word, url);
                }

                return word;
            }
            catch (Exception ex)
            {
                logger.Log(GetType(), Level.Error, $"An error occured while getting a word at {url}", ex);
                return null;
            }
        }

        private void ReadDictionary(HtmlNode dictionaryNode, Word word, string url)
        {
            var dictionaryName = dictionaryNode.SelectSingleNode("./h2/span|./h3/span").InnerText?.TrimAllSpecialCharacters();

            SubDictionary subDictionary = null; // context.Dictionaries.Where(d => d.Name == dictionaryName).SingleOrDefault();
            if (subDictionary == null)
            {
                subDictionary = new SubDictionary()
                {
                    Name = dictionaryName,
                    IsPrimary = dictionaryName == "Thông dụng" ? true : (bool?)null
                };
            }

            var wordClassNodes = dictionaryNode.SelectNodes("./div[@class='section-h3']");
            if (wordClassNodes != null)
            {
                var wordFormsNode = wordClassNodes.SingleOrDefault(c => c.SelectSingleNode("./h3[1]/span")?.InnerHtml.IndexOf("Hình thái từ", StringComparison.OrdinalIgnoreCase) >= 0);
                if (wordFormsNode != null)
                {
                    wordClassNodes.Remove(wordFormsNode);
                    word.WordForms.AddRange(ReadWordForms(wordFormsNode));
                }

                var phasesNode = wordClassNodes.SingleOrDefault(c => c.SelectSingleNode("./h3[1]/span")?.InnerHtml.IndexOf("Cấu trúc từ", StringComparison.OrdinalIgnoreCase) >= 0);
                if (phasesNode != null)
                {
                    wordClassNodes.Remove(phasesNode);
                    word.Phases = ReadPhases(phasesNode, subDictionary);
                }

                var defs = wordClassNodes.SelectMany(wc => ReadWordClass(wc, word, subDictionary));
                word.Definitions.AddRange(defs);
            }
            else
            {
                // This is for pages like http://tratu.soha.vn/dict/en_vn/Ablative_method, a lot of pages have this form
                var defNodes = dictionaryNode.SelectNodes("./div[@class='section-h5']");
                if (defNodes != null)
                {
                    var defs = defNodes.Select(d => new Definition()
                    {
                        Content = d.InnerText?.TrimAllSpecialCharacters(),
                        Word = word,
                        SubDictionary = subDictionary,
                        WordClass = UnknownWordClass,
                    });
                    word.Definitions.AddRange(defs);
                }
                else
                {
                    logger.Log(GetType(), Level.Warn, $"Could not get any word class node at {url}", null);
                }
            }
        }

        private IEnumerable<Definition> ReadWordClass(HtmlNode wordClassNode, Word word, SubDictionary subDictionary)
        {
            string wordClassText = wordClassNode.SelectSingleNode("./h3[1]/span/text()").InnerText?.TrimAllSpecialCharacters();
            WordClass wordClass = null; // context.WordClasses.SingleOrDefault(wc => wc.Name == wordClassText);
            if (wordClass == null)
            {
                wordClass = new WordClass()
                {
                    Name = wordClassText
                };
            }

            var definitionNodes = wordClassNode.SelectNodes("./div");
            if (definitionNodes == null)
            {
                // Treat wordClassNode as a definiton node (http://tratu.soha.vn/dict/en_vn/Allegedly)
                return new List<Definition>() { ReadDefinition(wordClassNode, word, subDictionary, wordClass) };
            }

            // Add a where here because in some cases a definition content is null and this will cause the world not to be registed
            // http://tratu.soha.vn/dict/en_vn/Absorbency
            return definitionNodes.Select(n => ReadDefinition(n, word, subDictionary, wordClass)).Where(d => !string.IsNullOrEmpty(d.Content));
        }

        private Definition ReadDefinition(HtmlNode definitionNode, Word word, SubDictionary subDictionary, WordClass wordClass)
        {
            var definitionTextNode = definitionNode.SelectSingleNode("./h5[1]/span");

            // This is in case wordClassNode treated as a definition node (http://tratu.soha.vn/dict/en_vn/Allegedly)
            if (definitionTextNode == null)
            {
                definitionTextNode = definitionNode.SelectSingleNode("./h3[1]/span");
            }
            var usageNodes = definitionNode.SelectNodes("./dl/dd/dl/dd")?.Select(u => u.InnerText?.TrimAllSpecialCharacters()).ToList();

            var definition = new Definition()
            {
                Content = definitionTextNode?.InnerText?.TrimAllSpecialCharacters(),
                Word = word,
                SubDictionary = subDictionary,
                WordClass = wordClass,
                Usages = new List<Usage>()
            };

            if (usageNodes != null)
            {
                if (usageNodes.Count % 2 != 0)
                {
                    // This is for pages like http://tratu.soha.vn/dict/en_vn/According, http://tratu.soha.vn/dict/en_vn/Acclimatization
                    usageNodes.RemoveAt(0);
                    logger.Log(GetType(), Level.Warn, "Number of usages is an old number, remove the first one", null);
                }

                for (int i = 0; i < usageNodes.Count / 2; i++)
                {
                    definition.Usages.Add(new Usage()
                    {
                        Sample = usageNodes[i * 2],
                        Translation = usageNodes[i * 2 + 1]
                    });
                }
            }

            return definition;
        }

        private List<Phase> ReadPhases(HtmlNode phasesNode, SubDictionary subDictionary)
        {
            return phasesNode.SelectNodes("./div").Select(p =>
            {
                var phase = new Phase()
                {
                    SubDictionary = subDictionary
                };

                var phaseContentNode = p.SelectSingleNode("./h5");
                phase.Content = phaseContentNode.InnerText?.TrimAllSpecialCharacters();

                var defNodes = p.SelectNodes("./dl/dd/dl/dd");
                if (defNodes != null)
                {
                    string prevDefContent = null;
                    var defs = new List<PhaseDefinition>();
                    var usageStrs = new List<string>();

                    defNodes.Append(HtmlNode.CreateNode("dummy"));
                    foreach (HtmlNode d in defNodes)
                    {
                        string defContent = d.InnerText?.TrimAllSpecialCharacters();
                        var usageNodes = d.SelectNodes("./dl/dd");

                        if (!string.IsNullOrEmpty(defContent) || d.InnerHtml == "dummy")
                        {
                            if (!string.IsNullOrEmpty(prevDefContent))
                            {
                                var usages = new List<PhaseUsage>();
                                for (int i = 0; i < usageStrs.Count / 2; i++)
                                {
                                    usages.Add(new PhaseUsage()
                                    {
                                        Sample = usageStrs[2 * i],
                                        Translation = usageStrs[2 * i + 1]
                                    });
                                }
                                var def = new PhaseDefinition()
                                {
                                    Content = prevDefContent,
                                    Usages = usages
                                };

                                defs.Add(def);
                            }

                            usageStrs = usageNodes?.Select(u => u.InnerText?.TrimAllSpecialCharacters()).ToList() ?? new List<string>();
                            prevDefContent = defContent;
                        }
                        else
                        {
                            if (usageNodes != null)
                            {
                                usageStrs.AddRange(usageNodes.Select(u => u.InnerText?.TrimAllSpecialCharacters()));
                            }
                        }
                    }

                    phase.Definitions = defs;
                }
                else
                {
                    var defContentNode = phaseContentNode.NextSibling.NextSibling;
                    var defContent = defContentNode.InnerText?.TrimAllSpecialCharacters();
                    phase.Definitions = new List<PhaseDefinition>() {
                        new PhaseDefinition() { Content = defContent }
                    };
                }

                return phase;
            }).ToList();
        }

        private List<WordForm> ReadWordForms(HtmlNode wordFormsNode)
        {
            return wordFormsNode.SelectNodes("./ul/li").Select(wf =>
            {
                return new WordForm()
                {
                    FormType = wf.SelectSingleNode("./text()").InnerHtml?.TrimAllSpecialCharacters(),
                    Content = wf.SelectSingleNode("./a").InnerHtml
                };
            }).ToList();
        }

        private List<RelativeWord> ReadRelativeWords(HtmlNode relativeWordsNode)
        {
            return relativeWordsNode.SelectNodes("./div").SelectMany(n =>
            {
                string isSynonym = n.SelectSingleNode("./h3").InnerText?.TrimAllSpecialCharacters();
                // We need a where here because of words like http://tratu.soha.vn/dict/en_vn/Accident
                // (No related words even though a related word entry exists)
                return n.SelectNodes("./div").Where(r => r.SelectNodes("./dl/dd/a") != null).SelectMany(r =>
                {
                    string wordClass = r.SelectSingleNode("./h5").InnerText?.TrimAllSpecialCharacters();
                    var relativeWords = r.SelectNodes("./dl/dd/a").Select(a => a.InnerText?.TrimAllSpecialCharacters());
                    return relativeWords.Select(rw => new RelativeWord()
                    {
                        IsSynomym = isSynonym?.Contains("Từ đồng nghĩa") ?? false,
                        WordClass = wordClass,
                        RelWord = rw
                    });
                });
            }).ToList();
        }

        private string CreateFullUrl(string url)
        {
            if (url == null)
            {
                return null;
            }

            if (url.StartsWith("http://") || url.StartsWith("https://"))
            {
                return url;
            }
            return "http://tratu.soha.vn" + url;
        }

        private HtmlDocument LoadPageWithTimeout(string url, int timeoutInMinute)
        {
            var task = Task.Run(() =>
            {
                var htmlWeb = new HtmlWeb();
                try
                {
                    return htmlWeb.Load(url);
                }
                catch (Exception ex)
                {
                    logger.Log(GetType(), Level.Error, $"An error occured while loading {url}", ex);
                    return null;
                }
            });

            if (task.Wait(TimeSpan.FromMinutes(timeoutInMinute)))
            {
                return task.Result;
            }
            else
            {
                return null;
            }
        }
    }
}
