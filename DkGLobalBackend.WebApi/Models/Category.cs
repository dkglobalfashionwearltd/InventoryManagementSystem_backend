using System.ComponentModel.DataAnnotations;

namespace DkGLobalBackend.WebApi.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
    }
}
