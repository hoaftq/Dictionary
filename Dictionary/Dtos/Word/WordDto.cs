using System.Collections.Generic;

namespace Dictionary.Dtos.Word
{
    public class WordDto
    {
        public string Content { get; set; }

        public string Spelling { get; set; }

        public string SpellingAudioUrl { get; set; }

        public IEnumerable<WordSubDictionaryDto> SubDictionaries { get; set; }

        public IEnumerable<WordFormDto> WordForms { get; set; }

        public IEnumerable<RelativeWordDto> RelativeWords { get; set; }
    }
}
