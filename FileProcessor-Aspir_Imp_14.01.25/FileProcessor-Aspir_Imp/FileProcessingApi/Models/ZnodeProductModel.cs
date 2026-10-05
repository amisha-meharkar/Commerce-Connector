namespace FileProcessingApi.Models
{
    public class ZnodeProductModel
    {
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public string SKU { get; set; }
        public bool IsActive { get; set; }
        public string LongDescription { get; set; }
        public string ShortDescription { get; set; }
        public string Brand { get; set; }
        public string Assortment { get; set; }
        public string Tags { get; set; }
        public string ProductImage { get; set; }
        public bool FreeShipping { get; set; }
        public string ShippingCostRules { get; set; }
        public string UOM { get; set; } // Unit of Measurement
        public decimal Weight { get; set; }
        public int? MaximumQuantity { get; set; }
        public int? MinimumQuantity { get; set; }
        public decimal? Height { get; set; }
        public string OutOfStockOptions { get; set; }
        public decimal? Width { get; set; }
        public decimal? Length { get; set; }
        public string ShippingInformation { get; set; }
        public string TestImage { get; set; }
        public bool IsObsolete { get; set; }
        public bool IsDownloadable { get; set; }
        public string JBCalenderData { get; set; }
        public string WashNew { get; set; }
        public string AddInfo { get; set; }
        public string ColorJeans { get; set; }
        public string Custom { get; set; }
        public string Container { get; set; }
        public string FacetAttribute { get; set; }
        public string PackageSizeFromRequest { get; set; }
        public string PackagingType { get; set; }
        public string TypicalLeadTime { get; set; }
        public bool DisplayVariantsOnGrid { get; set; }
        public string UPC { get; set; }
        public string Video1 { get; set; }
        public string Video2 { get; set; }
        public bool HideFromSearch { get; set; }
        public string QAType { get; set; }
        public string QAColor { get; set; }
        public string QASize { get; set; }
        public string GalleryImages { get; set; }
        public string ProductSpecification { get; set; }
        public string ProductCode { get; set; }
        public string FeatureDescription { get; set; }
        public bool CallForPricing { get; set; }
        public string Vendor { get; set; }
        public string Highlights { get; set; }
    }
}
