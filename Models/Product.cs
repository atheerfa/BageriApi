namespace BageriApi.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal PricePerPiece { get; set; }
        public decimal Weight { get; set; }
        public int AmountPerPackage { get; set; }
        public DateTime BestBeforeDate { get; set; }
        public DateTime ManufacturingDate { get; set; }
    }
}