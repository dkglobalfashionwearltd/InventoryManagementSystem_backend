namespace DkGLobalBackend.WebApi.Models.ResponseDto
{
    public class LoginResponse
    {
        public string UserId { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
    }
}
