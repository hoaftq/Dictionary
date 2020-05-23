using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class Usage
    {
        public int Id { get; set; }

        public string Sample { get; set; }

        public string Translation { get; set; }


        public int DefinitionId { get; set; }

        public Definition Definition { get; set; }
    }
}
