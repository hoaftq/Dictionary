using System.Collections.Generic;

namespace DataAccess.Models
{
    public class PhaseDefinition
    {
        public int Id { get; set; }

        public string Content { get; set; }


        public IList<PhaseUsage> Usages { get; set; }
    }
}
