namespace DkGLobalBackend.WebApi.Models.RequestDto
{
    public class ApplicationUserReq
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
    }
}
