using System.ComponentModel.DataAnnotations;

namespace DkGLobalBackend.WebApi.Models
{
    public class Stock
    {
        [Key]
        public int Id { get; set; }
        public string ModelNumber { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime DeletedAt { get; set; }
        public int DeletedBy { get; set; }
    }
}
