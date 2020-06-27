using System.Collections.Generic;

namespace DataAccess.Models
{
    public class Word
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public string Spelling { get; set; }

        public string SpellingAudioUrl { get; set; }


        public List<Definition> Definitions { get; set; } // TODO

        public ICollection<Phase> Phases { get; set; }

        public List<WordForm> WordForms { get; set; }

        public ICollection<RelativeWord> RelativeWords { get; set; }
    }
}
