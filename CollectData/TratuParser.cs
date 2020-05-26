
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
using System.Threading.Tasks;

namespace CollectData
{
    class TratuParser
    {
        private static ILogger logger = LoggerManager.GetLogger(Assembly.GetEntryAssembly(), typeof(TratuParser));

        private const int NUMBER_OF_CONCURRENT_PROCESSING_WORDS = 10;

        private DictionaryContext context;

        private ConcurrentBag<Word> parsedWords = new ConcurrentBag<Word>();

        public TratuParser(DictionaryContext context)
        {
            this.context = context;
        }

        public void Parse()
        {
            var url = "http://tratu.soha.vn/dict/en_vn/special:allpages";
            logger.Log(GetType(), Level.Debug, $"Processing all pages at {url}", null);

            HtmlWeb htmlWeb = new HtmlWeb();
            var htmlDoc = htmlWeb.Load(url);

            foreach (var link in htmlDoc.DocumentNode.SelectNodes("//table[@class='allpageslist']/tr/td[1]/a[@href]"))
            {
                ReadPage(link.Attributes["href"].Value);
            }
        }

        private void ReadPage(string href)
        {
            var url = CreateFullUrl(href);
            logger.Log(GetType(), Level.Debug, $"Processing a page at {url}", null);

            HtmlWeb htmlWeb = new HtmlWeb();
            var htmlDoc = htmlWeb.Load(url);

            var wordNodes = htmlDoc.DocumentNode.SelectNodes("//div[@id='bodyContent']/table[2]//td/a");

            List<Task> parserTasks = new List<Task>();

            int currentIndex = 0;

            while (currentIndex < wordNodes.Count)
            {
                int x = Math.Min(NUMBER_OF_CONCURRENT_PROCESSING_WORDS, wordNodes.Count - currentIndex);
                for (int i = 0; i < x; i++)
                {
                    parserTasks.Add(Task.Run(() =>
                    {
                        var word = ReadWord(wordNodes[currentIndex].Attributes["href"].Value);
                        if (word != null)
                        {
                            parsedWords.Add(word);
                        }
                    }));
                    currentIndex++;
                }

                Task.WaitAll(parserTasks.ToArray());
                parserTasks.Clear();

                try
                {
                    context.Words.AddRange(parsedWords);
                    context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    logger.Log(GetType(), Level.Error, $"Could not save {parsedWords.Count} words to database", ex);
                }

                parsedWords.Clear();
            }
        }

        private Word ReadWord(string href)
        {
            var url = CreateFullUrl(href);
            logger.Log(GetType(), Level.Debug, $"Geting a new word at {url}", null);

            try
            {
                var word = new Word();

                HtmlWeb htmlWeb = new HtmlWeb();
                var htmlDoc = htmlWeb.Load(url);

                var keywords = htmlDoc.DocumentNode.SelectNodes("//head/meta")
                    .FirstOrDefault(n => n.GetAttributeValue("name", null) == "keywords")
                    ?.GetAttributeValue("content", string.Empty);
                word.Content = keywords.Split(',')[0];

                var contentNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='bodyContent']");
                var children = contentNode.SelectNodes("./div");
                var spellingNode = children.Select(c => c.SelectSingleNode(".//font[@color='red']/text()"))
                    .Where(n => n != null)
                    .FirstOrDefault();
                word.Spelling = spellingNode?.InnerText;
                word.SpellingAudioUrl = null; // The audio link does not work anymore!

                var dictionaryNodes = contentNode.SelectNodes("./div[@class='section-h2']");
                if (dictionaryNodes == null)
                {
                    logger.Log(GetType(), Level.Error, $"Could not find any dictionary node at {url}", null);
                    return null;
                }

                var relativeWordsNode = dictionaryNodes.SingleOrDefault(d => d.SelectSingleNode("./h2/span")?.InnerHtml.Contains("Các từ liên quan") == true);
                if (relativeWordsNode != null)
                {
                    dictionaryNodes.Remove(relativeWordsNode);
                    word.RelativeWords = ReadRelativeWords(relativeWordsNode);
                }

                word.Definitions = new List<Definition>();
                word.WordForms = new List<WordForm>();
                foreach (var d in dictionaryNodes)
                {
                    ReadDictionary(d, word);
                }

                return word;
            }
            catch (Exception ex)
            {
                logger.Log(GetType(), Level.Error, $"An error occured while getting a word at {url}", ex);
                return null;
            }
        }

        private void ReadDictionary(HtmlNode dictionaryNode, Word word)
        {
            var dictionaryName = dictionaryNode.SelectSingleNode("./h2/span|./h3/span").InnerText;

            Dictionary dictionary = context.Dictionaries.Where(d => d.Name == dictionaryName).SingleOrDefault();
            if (dictionary == null)
            {
                dictionary = new Dictionary()
                {
                    Name = dictionaryName
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
                    word.Phases = ReadPhases(phasesNode, dictionary);
                }

                word.Definitions.AddRange(wordClassNodes.SelectMany(wc => ReadWordClass(wc, word, dictionary)));
            }
            else
            {
                // TODO need to process here
                logger.Log(GetType(), Level.Warn, "Could not get any word class node", null);
            }
        }

        private IEnumerable<Definition> ReadWordClass(HtmlNode wordClassNode, Word word, Dictionary dictionary)
        {
            string wordClassText = wordClassNode.SelectSingleNode("./h3[1]/span/text()").InnerText.TrimAllSpecialCharacters(); // TODO really need this
            WordClass wordClass = context.WordClasses.SingleOrDefault(wc => wc.Name == wordClassText);
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
                return new List<Definition>() { ReadDefinition(wordClassNode, word, dictionary, wordClass) };
            }

            return definitionNodes.Select(n => ReadDefinition(n, word, dictionary, wordClass));
        }

        private Definition ReadDefinition(HtmlNode definitionNode, Word word, Dictionary dictionary, WordClass wordClass)
        {
            var definitionTextNode = definitionNode.SelectSingleNode("./h5[1]/span");

            // This is in case wordClassNode treated as a definition node (http://tratu.soha.vn/dict/en_vn/Allegedly)
            if (definitionTextNode == null)
            {
                definitionTextNode = definitionNode.SelectSingleNode("./h3[1]/span");
            }
            var usageNodes = definitionNode.SelectNodes("./dl/dd/dl/dd")?.Select(u => u.InnerText).ToList();

            Definition definition = new Definition()
            {
                Content = definitionTextNode?.InnerText,
                Word = word,
                Dictionary = dictionary,
                WordClass = wordClass,
                Usages = new List<Usage>()
            };

            if (usageNodes != null)
            {
                if (usageNodes.Count % 2 != 0)
                {
                    logger.Log(GetType(), Level.Error, "Number of usages is an old number", null);
                }
                else
                {
                    for (int i = 0; i < usageNodes.Count / 2; i++)
                    {
                        definition.Usages.Add(new Usage()
                        {
                            Sample = usageNodes[i * 2],
                            Translation = usageNodes[i * 2 + 1]
                        });
                    }
                }
            }

            return definition;
        }

        private List<Phase> ReadPhases(HtmlNode phasesNode, Dictionary dictionary)
        {
            return phasesNode.SelectNodes("./div").Select(p =>
            {
                var phase = new Phase()
                {
                    Dictionary = dictionary
                };

                var phaseContentNode = p.SelectSingleNode("./h5");
                phase.Content = phaseContentNode.InnerText?.Trim();

                var defNodes = p.SelectNodes("./dl/dd/dl/dd");
                if (defNodes != null)
                {
                    string prevDefContent = null;
                    var defs = new List<PhaseDefinition>();
                    var usageStrs = new List<string>();

                    defNodes.Append(HtmlNode.CreateNode("dummy"));
                    foreach (HtmlNode d in defNodes)
                    {
                        string defContent = d.SelectSingleNode("./text()")?.InnerText.TrimAllSpecialCharacters();
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

                            usageStrs = usageNodes?.Select(u => u.InnerText).ToList() ?? new List<string>();
                            prevDefContent = defContent;
                        }
                        else
                        {
                            if (usageNodes != null)
                            {
                                usageStrs.AddRange(usageNodes.Select(u => u.InnerText));
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
                    FormType = wf.SelectSingleNode("./text()").InnerHtml.TrimAllSpecialCharacters(),
                    Content = wf.SelectSingleNode("./a").InnerHtml
                };
            }).ToList();
        }

        private List<RelativeWord> ReadRelativeWords(HtmlNode relativeWordsNode)
        {
            return relativeWordsNode.SelectNodes("./div").SelectMany(n =>
            {
                string isSynonym = n.SelectSingleNode("./h3").InnerText;
                return n.SelectNodes("./div").SelectMany(r =>
                {
                    string wordClass = r.SelectSingleNode("./h5").InnerText;
                    var relativeWords = r.SelectNodes("./dl/dd/a").Select(a => a.InnerText);
                    return relativeWords.Select(rw => new RelativeWord()
                    {
                        IsSynomym = isSynonym == "Từ đồng nghĩa",
                        WordClass = wordClass,
                        RelWord = rw
                    });
                });
            }).ToList();
        }

        private string CreateFullUrl(string url)
        {
            if (url.StartsWith("http://") || url.StartsWith("https://"))
            {
                return url;
            }
            return "http://tratu.soha.vn" + url;
        }
    }
}
