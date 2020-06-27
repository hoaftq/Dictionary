using System.Collections.Generic;

namespace DataAccess.Models
{
    public class Phase
    {
        public int Id { get; set; }

        public string Content { get; set; }


        public int SubDictionaryId { get; set; }

        public SubDictionary SubDictionary { get; set; }

        public int WordId { get; set; }

        public Word Word { get; set; }

        public IList<PhaseDefinition> Definitions { get; set; }
    }
}
