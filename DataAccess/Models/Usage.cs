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
