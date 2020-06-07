using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dictionary.Dtos.Word
{
    public class PhaseDto
    {
        public string Content { get; set; }

        public IEnumerable<DefinitionDto> Definitions { get; set; }
    }
}
