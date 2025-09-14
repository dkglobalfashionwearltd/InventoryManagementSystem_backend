using DkGLobalBackend.WebApi.Models;
using DkGLobalBackend.WebApi.Models.RequestDto;
using DkGLobalBackend.WebApi.Services.IServices;
using DkGLobalBackend.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.RegularExpressions;

namespace DkGLobalBackend.WebApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public AuthController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpPost]
        [Route("user/login")]
        public async Task<ApiResponse> LoginReq(string username, string password)
        {
            var response = new ApiResponse();
            if (username != null && username != "" && password != null && password != "")
            {
               
                    response = await _serviceManager.Auth.Login(username, password);
                    return response;

            }
            else
            {
                response.Success = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.Message = "Username or password is incorrect";
                return response;
            }
        }

        [HttpPost]
        [Route("user/registration")]
        //[Authorize(Roles = "admin")]
        public async Task<ApiResponse> Registration(ApplicationUserReq request)
        {
            var response = new ApiResponse();
            string[] userRoles = { Roles.IT, Roles.STORE, Roles.USER };
            var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            var isEmailValid =  Regex.IsMatch(request.Email, pattern, RegexOptions.IgnoreCase);
            if (request == null)
            {
                response.Success = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.Message = "Request can not be empty or null.";
                return response;
            }
            if (isEmailValid == false)
            {
                response.Success = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.Message = "Email validation error";
                return response;
            }
            if (!userRoles.Contains(request.Role, StringComparer.OrdinalIgnoreCase))
            {
                response.Success = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.Message = $"Role {request.Role} does not exist.";
                return response;
            }

            var isUniqueUser = _serviceManager.Auth.IsUniqueUser(request.PhoneNumber);
            if (isUniqueUser == false)
            {
                response.Success = false;
                response.StatusCode = HttpStatusCode.Conflict;
                response.Message = "User already exists with this phone number";
                return response;
            }
            if (request.UserName == null || request.UserName == "" || request.Password == null || request.Password == "")
            {
                response.Success = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.Message = "User name and password can not be empty or null.";
                return response;
            }
            var PasswordRegex = @"^(?=(.*[A-Z]))(?=(.*\d))(?=(.*\W))(?=.{6,})[A-Za-z\d\W]*$";
            var regex = new Regex(PasswordRegex);
            var validPassword = regex.IsMatch(request.Password);
            if (!validPassword)
            {
                response.Success = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.Message = "Password must be at least 6 characters long, include at least one uppercase letter, one digit, and one non-alphanumeric character.";
                return response;
            }
            response = await _serviceManager.Auth.Registration(request);
            await _serviceManager.Save();

            return response;
        }


        [HttpGet]
        [Route("user/getall")]
        public async Task<ApiResponse> GetAllUserInfo(CancellationToken cancellationToken)
        {
            var response = new ApiResponse();
            try
            {
                var genericReq = new GenericServiceRequest<ApplicationUser>
                {
                    Expression = null,
                    IncludeProperties = null,
                    Tracked = true,
                    CancellationToken = cancellationToken
                };
                var resultRes = await _serviceManager.Auth.GetAllAsync(genericReq);
                if(resultRes == null)
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Success = false;
                    response.Message = "Not Found";
                    return response;
                }

                response.StatusCode = HttpStatusCode.OK;
                response.Success = true;
                response.Message = "Successful";
                response.Result = resultRes;
                return response;

            }
            catch (Exception ex) 
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }

        }

        [HttpGet]
        [Route("user/get")]
        public async Task<ApiResponse> GetUserInfo(string UserId, CancellationToken cancellationToken)
        {
            var response = new ApiResponse();
            try
            {
                var genericReq = new GenericServiceRequest<ApplicationUser>
                {
                    Expression = x=>x.Id == UserId,
                    IncludeProperties = null,
                    Tracked = true,
                    CancellationToken = cancellationToken
                };
                var resultRes = await _serviceManager.Auth.GetAsync(genericReq);
                if (resultRes == null)
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Success = false;
                    response.Message = "Not Found";
                    return response;
                }

                response.StatusCode = HttpStatusCode.OK;
                response.Success = true;
                response.Message = "Successful";
                response.Result = resultRes;
                return response;

            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }

        }

        [HttpPost]
        [Route("update-user-info")]
        public async Task<ApiResponse> UpdateUserInfo(ApplicationUserUpdateReq req,CancellationToken cancellationToken)
        {
            var response = new ApiResponse();
            try
            {
                if(req.UserId == null)
                {
                    response.StatusCode=HttpStatusCode.BadRequest;
                    response.Success=false;
                    response.Message = "Id Not Found";
                    return response;
                }
                var userData = await _serviceManager.Auth.GetAsync(new GenericServiceRequest<ApplicationUser>
                {
                    Expression = x=>x.Id == req.UserId,
                    IncludeProperties = null,
                    Tracked = false,
                    CancellationToken = cancellationToken
                });
                if(userData == null)
                {
                    response.StatusCode=HttpStatusCode.NotFound;
                    response.Success=false;
                    response.Message = "Data Not Found";
                    return response;
                }
                userData.UserName = req.UserName ?? userData.UserName;
                userData.PhoneNumber = req.PhoneNumber ?? userData.PhoneNumber;
                userData.Email = req.Email ?? userData.Email;
                await _serviceManager.Save();

                response.StatusCode = HttpStatusCode.NotFound;
                response.Success = false;
                response.Message = "Data Not Found";
                return response;

            }
            catch (Exception ex) {

                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = ex.InnerException?.Message != null ? ex.InnerException.Message :  ex.Message;
                return response;
            }
           
        }
    }
}
