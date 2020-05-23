using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class Phase
    {
        public int Id { get; set; }

        public string Content { get; set; }


        public int DictionaryId { get; set; }

        public Dictionary Dictionary { get; set; }

        public int WordId { get; set; }

        public Word Word { get; set; }

        public IList<PhaseDefinition> Definitions { get; set; }
    }
}
