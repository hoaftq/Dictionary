using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class Definition
    {
        public int Id { get; set; }

        public string Content { get; set; }


        public int SubDictionaryId { get; set; }

        public SubDictionary SubDictionary { get; set; }

        public int WordClassId { get; set; }

        public WordClass WordClass { get; set; }

        public int WordId { get; set; }

        public Word Word { get; set; }

        public ICollection<Usage> Usages { get; set; }
    }
}
