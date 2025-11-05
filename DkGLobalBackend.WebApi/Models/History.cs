using System.ComponentModel.DataAnnotations;

namespace DkGLobalBackend.WebApi.Models
{
    public class History
    {
        [Key]
        public int Id { get; set; }
        public string ActionTitle { get; set; }
        public string ActionBysId { get; set; }
        public string ActionBysName { get; set; }
        public DateTime ActionAt { get; set; }
    }
}
