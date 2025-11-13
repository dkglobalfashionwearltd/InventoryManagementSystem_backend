using DkGLobalBackend.WebApi.Models;
using DkGLobalBackend.WebApi.Models.RequestDto;
using DkGLobalBackend.WebApi.Models.ResponseDto;

namespace DkGLobalBackend.WebApi.Services.IServices
{
    public interface IAuth : IServices<ApplicationUser>
    {
        bool IsUniqueUser(string phoneNumber);
        Task<ApiResponse> Login(LoginReq req);

        //.........
        Task<ApiResponse> LoginNew(LoginReq req);
        Task<ApiResponse> RefreshNew();
        Task<ApiResponse> LogoutNew();
        Task<ApiResponse> Registration(ApplicationUserReq req);
        void Update(ApplicationUser user);
    }
}
