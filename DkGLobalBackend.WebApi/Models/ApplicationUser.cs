using Microsoft.AspNetCore.Identity;

namespace DkGLobalBackend.WebApi.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Password { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
    }
}
