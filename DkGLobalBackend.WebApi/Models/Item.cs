using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DkGLobalBackend.WebApi.Models
{
    public class Item
    {
        [Key]
        public int ItemId { get; set; }
        public string Name { get; set; }
        public string ModelNumber { get; set; }
        public string SerialNumber { get; set; }
        public string BrandName { get; set; }
        public int Price { get; set; }
        public DateOnly PurchaseDate { get; set; }
        public DateOnly WarrantyEnd { get; set; }

        public string SourceName { get; set; }
        public string SourcePhoneNumber { get; set; }

        public DateOnly LastServicingDate { get; set; }
        public DateOnly NextServicingDate { get; set; }

        public string ServiceProviderName { get; set; }
        public string ServiceProviderPhoneNumber { get; set; }

        public string Status { get; set; }
        public string ItemCondition { get; set; }
        public string Quantity { get; set; }


        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }

        //[JsonIgnore]
        //public ICollection<ItemUser> ItemUsers { get; set; } = new List<ItemUser>();
        public ICollection<AssignItemUser> AssignItemUsers { get; set; }
    }
}
