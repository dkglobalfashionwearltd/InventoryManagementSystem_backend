namespace DkGLobalBackend.WebApi.Models.RequestDto
{
    public class AssignItemUserDto
    {
        public List<string> ItemIds { get; set; }
        public List<string> ItemUserIds { get; set; }
        public DateOnly AssignedDate { get; set; }
        public string AssignTimeCondition { get; set; }
        public string? AssignAgainstTo { get; set; }
        public string Status { get; set; }
    }
}
