namespace DkGLobalBackend.WebApi.Models.RequestDto
{
    public class AssignItemUserSingleDto
    {
        public string ItemId { get; set; }
        public string ItemUserId { get; set; }
        public DateOnly AssignedDate { get; set; }
        public string AssignTimeCondition { get; set; }
        public string ItemSerialNumber { get; set; }
        public string? AssignAgainstTo { get; set; }
        public string Status { get; set; }
    }
}
