using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dictionary.Dtos.Word
{
    public class WordSubDictionaryDto
    {
        public string Name { get; set; }

        public IEnumerable<DictionaryWordClassDto> WordClasses { get; set; }

        public IEnumerable<PhaseDto> Phases { get; set; }
    }
}
