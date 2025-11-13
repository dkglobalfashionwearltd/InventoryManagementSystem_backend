namespace DkGLobalBackend.WebApi.Models.RequestDto
{
    public class LoginReq
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; } = false;
    }
}
