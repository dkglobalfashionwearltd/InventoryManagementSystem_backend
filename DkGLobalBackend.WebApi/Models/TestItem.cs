using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DkGLobalBackend.WebApi.Models
{
    public class TestItem
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        [JsonIgnore]
        public ICollection<TestUser> TestUsers { get; set; } = new List<TestUser>();
    }
}
