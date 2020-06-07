using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dictionary.Dtos.Word
{
    public class DictionaryWordClassDto
    {
        public string Name { get; set; }

        public IEnumerable<DefinitionDto> Definitions { get; set; }
    }
}
