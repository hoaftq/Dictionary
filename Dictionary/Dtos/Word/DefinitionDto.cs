using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dictionary.Dtos.Word
{
    public class DefinitionDto
    {
        public string Content { get; set; }

        public IEnumerable<UsageDto> Usages { get; set; }
    }
}
