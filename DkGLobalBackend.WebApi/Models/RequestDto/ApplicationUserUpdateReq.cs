namespace DkGLobalBackend.WebApi.Models.RequestDto
{
    public class ApplicationUserUpdateReq
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
