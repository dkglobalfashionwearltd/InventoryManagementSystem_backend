namespace DkGLobalBackend.WebApi.Models.RequestDto
{
    public class ItemDto
    {
        public string Name { get; set; }
        public string ModelNumber { get; set; }
        public string SerialNumber { get; set; }
        public string BrandName { get; set; }
        public int Price { get; set; }
        public DateOnly PurchaseDate { get; set; }

        public string SourceName { get; set; }
        public string SourcePhoneNumber { get; set; }

        public DateOnly LastServicingDate { get; set; }
        public DateOnly NextServicingDate { get; set; }

        public string ServiceProviderName { get; set; }
        public string ServiceProviderPhoneNumber { get; set; }

        public string Status { get; set; }
        public string ItemCondition { get; set; }
        public string Quantity { get; set; }
        public DateOnly WarrantyEnd { get; set; }


        public int CategoryId { get; set; }
    }
}
