namespace ATG.CodeTest.Models
{
   public class Lot
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }
        public bool IsArchived { get; set; }
    }
}
