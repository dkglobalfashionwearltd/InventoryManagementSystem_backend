using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DkGLobalBackend.WebApi.Models
{
    public class Stock
    {
        [Key]
        public int Id { get; set; }
        public int ItemId { get; set; }
        [ForeignKey("ItemId")]
        public Item Item { get; set; }

        public int TotalGivenQuantity { get; set; }
        public int LastQuantity { get; set; }
        public int CurrentQuantity { get; set; }
        public int StockCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime StockOutAt { get; set; }

        //soft delete
        public bool IsDeleted { get; set; } = false;
        public DateTime DeletedAt { get; set; }
    }
}
