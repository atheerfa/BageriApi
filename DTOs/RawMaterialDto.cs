namespace BageriApi.DTOs
{
    public class RawMaterialDto
    {
        public string ArticleNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal PricePerKg { get; set; }
    }
}