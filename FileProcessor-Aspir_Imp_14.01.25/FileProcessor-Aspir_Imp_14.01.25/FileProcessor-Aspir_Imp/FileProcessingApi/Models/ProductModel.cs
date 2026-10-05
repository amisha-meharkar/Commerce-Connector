using System.Xml.Serialization;

namespace FileProcessingApi.Models
{
    [XmlRoot("Products")]
    public class Products
    {
        [XmlElement("Product")]
        public List<ProductModel> ProductList { get; set; }
    }
    public class ProductModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
