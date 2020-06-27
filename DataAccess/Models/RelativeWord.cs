namespace DataAccess.Models
{
    public class RelativeWord
    {
        public int Id { get; set; }

        public bool IsSynomym { get; set; }

        public string WordClass { get; set; }

        public string RelWord { get; set; }


        public int WordId { get; set; }

        public Word Word { get; set; }
    }
}
