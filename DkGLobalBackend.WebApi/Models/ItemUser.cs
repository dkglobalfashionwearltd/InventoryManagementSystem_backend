using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DkGLobalBackend.WebApi.Models
{
    public class ItemUser
    {
        [Key]
        public int ItemUserId { get; set; }
        public int OfficeId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Designation { get; set; }

        public int DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department Department { get; set; }

        public string Status { get; set; }

        //[JsonIgnore]
        //public ICollection<Item> Items { get; set; } = new List<Item>();
        public ICollection<AssignItemUser> AssignItemUsers { get; set; }

    }
}
