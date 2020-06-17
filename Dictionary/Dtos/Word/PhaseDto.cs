using System.Collections.Generic;

namespace Dictionary.Dtos.Word
{
    public class PhaseDto
    {
        public string Content { get; set; }

        public IEnumerable<DefinitionDto> Definitions { get; set; }
    }
}
