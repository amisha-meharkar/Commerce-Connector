using System.Xml.Serialization;

namespace FileProcessingApi.Models
{
    [XmlRoot("Inventories")]
    public class Inventories
    {
        [XmlElement("Inventory")]
        public List<InventoryModel> InventoryList { get; set; }
    }

    public class InventoryModel 
    {
        //public int ProductId { get; set; }
        //public int StockLevel { get; set; }

        public int Quantity { get; set; }
        public int ReOrderLevel { get; set; }
        public string SKU { get; set; }       
        public string WarehouseCode { get; set; }
    }
}
