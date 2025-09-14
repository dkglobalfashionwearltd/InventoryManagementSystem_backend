namespace DkGLobalBackend.WebApi.Models
{
    public class AssignItemUser
    {
        public int ItemId { get; set; }
        public Item Item { get; set; }

        public int ItemUserId { get; set; }
        public ItemUser ItemUser { get; set; }

        public DateOnly AssignedDate { get; set; }
        public string AssignTimeCondition { get; set; }
        public int? AssignAgainstTo { get; set; }
        public string Status { get; set; }
    }
}
