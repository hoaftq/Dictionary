using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class PhaseDefinition
    {
        public int Id { get; set; }

        public string Content { get; set; }


        public IList<PhaseUsage> Usages { get; set; }
    }
}
