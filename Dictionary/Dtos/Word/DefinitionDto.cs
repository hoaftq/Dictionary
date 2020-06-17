using System.Collections.Generic;

namespace Dictionary.Dtos.Word
{
    public class DefinitionDto
    {
        public string Content { get; set; }

        public IEnumerable<UsageDto> Usages { get; set; }
    }
}
