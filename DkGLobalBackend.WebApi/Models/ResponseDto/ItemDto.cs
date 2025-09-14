namespace DkGLobalBackend.WebApi.Models.ResponseDto
{
    public class ItemDto
    {
        public int ItemId { get; set; }
        public string Name { get; set; }
        public string ModelNumber { get; set; }
        public string SerialNumber { get; set; }
        public int Price { get; set; }
        public DateOnly PurchaseDate { get; set; }

        public string SourceName { get; set; }
        public string SourcePhoneNumber { get; set; }

        public DateOnly LastServicingDate { get; set; }
        public DateOnly NextServicingDate { get; set; }

        public string ServiceProviderName { get; set; }
        public string ServiceProviderPhoneNumber { get; set; }

        public string Status { get; set; }

        public ItemUser ItemUser { get; set; }
        public Category Category { get; set; }
    }
}
