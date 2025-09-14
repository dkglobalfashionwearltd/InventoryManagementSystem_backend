using DkGLobalBackend.WebApi.Database;
using DkGLobalBackend.WebApi.Models;
using DkGLobalBackend.WebApi.Models.RequestDto;
using DkGLobalBackend.WebApi.Models.ResponseDto;
using DkGLobalBackend.WebApi.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace DkGLobalBackend.WebApi.Services
{
    public class AuthService : Services<ApplicationUser>, IAuth
    {
        private readonly InventoryDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly string _secretKey;
        //private readonly IWebHostEnvironment _webHostEnvironment;
        //private readonly IHttpContextAccessor _contextAccessor;
        public AuthService(
            InventoryDbContext db, 
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, 
            string secretKey
            ) : base(db)
        {
            _dbContext = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _secretKey = secretKey;
            
        }

        public bool IsUniqueUser(string phoneNumber)
        {
            var user = _dbContext.ApplicationUsers?.FirstOrDefault(u => u.PhoneNumber == phoneNumber);
            if (user == null) { 
                return true;
            }
            return false;

        }

        public async Task<ApiResponse> Login(string username, string password)
        {
            var response = new ApiResponse();
            var loginResponse = new LoginResponse();
            
            try
            {
                var user = _dbContext.ApplicationUsers?.FirstOrDefault(u => u.UserName.ToLower() == username.ToLower());
                bool isValid = await _userManager.CheckPasswordAsync(user, password);
                if(user == null || isValid == false)
                {
                    response.Success = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = "Username or password is incorrect";
                    return response;
                }

                var roles = await _userManager.GetRolesAsync(user);
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_secretKey);

                var tokenDescription = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity([
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.UserName.ToString()),
                        new Claim(ClaimTypes.Role, roles.FirstOrDefault())

                        ]),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature),
                };

                var token = tokenHandler.CreateToken(tokenDescription);

                loginResponse.UserId = user.Id;
                loginResponse.Role = roles.FirstOrDefault();
                loginResponse.Token = tokenHandler.WriteToken(token);

                response.Success = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Message = "Login Successful";
                response.Result = loginResponse;
                return response;


            }
            catch (Exception ex) {
                response.Success = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
                return response;
            }
        }

        public async Task<ApiResponse> Registration(ApplicationUserReq request)
        {
            var response = new ApiResponse();
            try
            {
                ApplicationUser user = new()
                {
                    UserName = request.UserName,
                    Password = request.Password,
                    PhoneNumber = request.PhoneNumber,
                    Email = request.Email,                    
                };
               
                    var resultRes = await _userManager.CreateAsync(user, request.Password);

                    if (resultRes.Succeeded)
                    {
                        var roleAssigned = await _userManager.AddToRoleAsync(user, request.Role);
                        

                        response.Success = true;
                        response.StatusCode = HttpStatusCode.Created;
                        response.Message = "User created successfully.";
                        //return response;
                    }
                    else
                    {
                        response.Success = false;
                        response.StatusCode = HttpStatusCode.InternalServerError;
                        response.Message = $"{string.Join("\n", resultRes.Errors.Select(s => s.Code))}\n{string.Join("\n", resultRes.Errors.Select(s => s.Description))}";
                    }
                

               
                return response;


            }
            catch (Exception ex) 
            {
                response.Success = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex?.Message + ex?.InnerException?.Message;
                return response;
            }
        }

        public void Update(ApplicationUser user)
        {
            _dbContext.Update(user);
        }
    }
}
