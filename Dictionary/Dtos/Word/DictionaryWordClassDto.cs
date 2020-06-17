using System.Collections.Generic;

namespace Dictionary.Dtos.Word
{
    public class DictionaryWordClassDto
    {
        public string Name { get; set; }

        public IEnumerable<DefinitionDto> Definitions { get; set; }
    }
}
