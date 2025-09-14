using System.ComponentModel.DataAnnotations;

namespace DkGLobalBackend.WebApi.Models
{
    public class TestUser
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;

        public ICollection<TestItem> TestItems { get; set; } = new List<TestItem>();
    }
}
