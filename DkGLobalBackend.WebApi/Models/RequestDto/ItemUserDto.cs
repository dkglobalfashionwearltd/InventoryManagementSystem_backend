using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DkGLobalBackend.WebApi.Models.RequestDto
{
    public class ItemUserDto
    {
        public int OfficeId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Designation { get; set; }

        public int DepartmentId { get; set; }

        public string Status { get; set; }
    }
}
