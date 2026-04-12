namespace BageriApi.Models
{
    public class RawMaterial
    {
        public int Id { get; set; }
        public string ArticleNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<SupplierRawMaterial> SupplierRawMaterials { get; set; } = new();
    }
}