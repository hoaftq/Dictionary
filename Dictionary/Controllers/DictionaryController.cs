using DataAccess.Data;
using DataAccess.Models;
using Dictionary.Dtos;
using Dictionary.Dtos.Word;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dictionary.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DictionaryController : ControllerBase
    {
        private const int NumberOfSuggestionWords = 6;

        private readonly DictionaryContext context;

        public DictionaryController(DictionaryContext context)
        {
            this.context = context;
        }

        [HttpGet("{word}")]
        public ActionResult<WordDto> Get(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return BadRequest();
            }

            word = HttpUtility.UrlDecode(word);

            var foundWord = context.Words.Include(w => w.Definitions)
                                    .ThenInclude(d => d.SubDictionary)
                                    .Include(w => w.Definitions)
                                        .ThenInclude(d => d.WordClass)
                                    .Include(w => w.Definitions)
                                        .ThenInclude(d => d.Usages)
                                    .Include(w => w.Phases)
                                        .ThenInclude(p => p.Definitions)
                                    .Include(w => w.WordForms)
                                    .Include(w => w.RelativeWords)
                                    .AsNoTracking()
                                    .FirstOrDefault(w => w.Content == word);

            if (foundWord == null)
            {
                return NotFound();
            }

            return ModelToDto(foundWord);
        }

        [HttpPost("{action}")]
        public IEnumerable<SuggestionDto> Search([FromForm]string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return null;
            }

            var foundWords = context.Words.Include(w => w.Definitions)
                                              .ThenInclude(d => d.WordClass)
                                          .Where(w => w.Content.StartsWith(word))
                                          .OrderBy(w => w.Content) // Ordering by Content improves performance significantly comparing to Content.Length
                                          .Take(NumberOfSuggestionWords)
                                          .AsNoTracking();

            var result = foundWords?.Select(w => new SuggestionDto()
            {
                Word = w.Content,
                Spelling = w.Spelling,
                WordClass = w.Definitions.Count > 0 ? w.Definitions[0].WordClass.Name : string.Empty,
                Definition = w.Definitions.Count > 0 ? w.Definitions[0].Content : string.Empty
            });

            return result;
        }

        [HttpGet("{action}")]
        public IEnumerable<SubDictionaryDto> SubDirectory()
        {
            return context.SubDictionaries.Select(sd => new SubDictionaryDto()
            {
                Id = sd.Id,
                Name = sd.Name,
                IsPrimary = sd.IsPrimary ?? false
            });
        }

        private static WordDto ModelToDto(Word word)
        {
            if (word == null)
            {
                return null;
            }

            var dictionaries = new List<WordSubDictionaryDto>();
            foreach (var dictionaryGroup in word.Definitions.GroupBy(def => def.SubDictionary.Name, def => def))
            {
                var wordClasses = new List<DictionaryWordClassDto>();
                foreach (var wordClassGroup in dictionaryGroup.GroupBy(def => def.WordClass.Name, def => def))
                {
                    wordClasses.Add(new DictionaryWordClassDto()
                    {
                        Name = wordClassGroup.Key,
                        Definitions = wordClassGroup.Select(d => new DefinitionDto()
                        {
                            Content = d.Content,
                            Usages = d.Usages?.Select(u => new UsageDto()
                            {
                                Sample = u.Sample,
                                Translation = u.Translation
                            })
                        })
                    });
                }

                var phases = word.Phases
                    ?.Where(p => p.SubDictionary?.Name == dictionaryGroup.Key)
                    .Select(p => new PhaseDto()
                    {
                        Content = p.Content,
                        Definitions = p.Definitions.Select(phaseDef => new DefinitionDto()
                        {
                            Content = phaseDef.Content,
                            Usages = phaseDef.Usages?.Select(pu => new UsageDto()
                            {
                                Sample = pu.Sample,
                                Translation = pu.Translation
                            })
                        })
                    });

                dictionaries.Add(new WordSubDictionaryDto()
                {
                    Name = dictionaryGroup.Key,
                    WordClasses = wordClasses,
                    Phases = phases
                });
            }

            var result = new WordDto()
            {
                Content = word.Content,
                Spelling = word.Spelling,
                SpellingAudioUrl = word.SpellingAudioUrl,
                SubDictionaries = dictionaries,

                WordForms = word.WordForms?.Select(wf => new WordFormDto()
                {
                    Content = wf.Content,
                    FormType = wf.FormType
                }),

                RelativeWords = word.RelativeWords?.Select(rw => new RelativeWordDto()
                {
                    Content = rw.RelWord,
                    IsSynonym = rw.IsSynomym,
                    WordClass = rw.WordClass
                })
            };

            return result;
        }
    }
}
