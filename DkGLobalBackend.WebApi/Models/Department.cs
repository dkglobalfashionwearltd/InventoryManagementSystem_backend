using System.ComponentModel.DataAnnotations;

namespace DkGLobalBackend.WebApi.Models
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }

    }
}
