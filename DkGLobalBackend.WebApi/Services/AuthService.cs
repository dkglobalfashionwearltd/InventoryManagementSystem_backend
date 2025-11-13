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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _secretKey;
        //private readonly IWebHostEnvironment _webHostEnvironment;
        public AuthService(
            InventoryDbContext db, 
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, 
            string secretKey,
            IHttpContextAccessor httpContextAccessor
            ) : base(db)
        {
            _dbContext = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _secretKey = secretKey;
            _httpContextAccessor = httpContextAccessor;
            
        }

        public bool IsUniqueUser(string phoneNumber)
        {
            var user = _dbContext.ApplicationUsers?.FirstOrDefault(u => u.PhoneNumber == phoneNumber);
            if (user == null) { 
                return true;
            }
            return false;

        }

        public async Task<ApiResponse> Login(LoginReq req)
        {
            var response = new ApiResponse();
            var loginResponse = new LoginResponse();

            try
            {
                if(req == null)
                {
                    response.Success = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = "Username or password is incorrect";
                    return response;
                }
                var user = _dbContext.ApplicationUsers?.FirstOrDefault(u => u.UserName.ToLower() == req.Username.ToLower());
                if (user == null)
                {
                    response.Success = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = "Username or password is incorrect";
                    return response;
                }
                bool isValid = await _userManager.CheckPasswordAsync(user, req.Password);
                if(isValid == false)
                {
                    response.Success = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = "Username or password is incorrect";
                    return response;
                }

                var roles = await _userManager.GetRolesAsync(user);
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_secretKey);
                var tokenExpire = req.RememberMe ? DateTime.UtcNow.AddDays(10) : DateTime.UtcNow.AddMinutes(30);

                var tokenDescription = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity([
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.UserName.ToString()),
                        new Claim(ClaimTypes.Role, roles.FirstOrDefault())

                        ]),
                    Expires = tokenExpire,
                    SigningCredentials = new(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature),
                };

                var token = tokenHandler.CreateToken(tokenDescription);

                loginResponse.UserId = user.Id;
                loginResponse.Role = roles.FirstOrDefault();
                loginResponse.Token = tokenHandler.WriteToken(token);
                loginResponse.TokenExpire = tokenExpire;

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
       
        public async Task<ApiResponse> LoginNew(LoginReq req)
        {
            var response = new ApiResponse();
            var loginResponse = new LoginResponse();

            try
            {
                var user = _dbContext.ApplicationUsers
                    ?.FirstOrDefault(u => u.UserName.ToLower() == req.Username.ToLower());

                if (user == null)
                {
                    response.Success = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = "Username or password is incorrect";
                    return response;
                }
                bool isValid = await _userManager.CheckPasswordAsync(user, req.Password);
                if (!isValid)
                {
                    response.Success = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = "Username or password is incorrect";
                    return response;
                }

                var roles = await _userManager.GetRolesAsync(user);
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_secretKey);

                // short-lived access token (15–30 mins)
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, roles.FirstOrDefault() ?? "User")
            }),
                    Expires = DateTime.UtcNow.AddMinutes(30),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var accessToken = tokenHandler.WriteToken(token);

                // Long-lived refresh token (valid for 7–30 days)
                var refreshToken = Guid.NewGuid().ToString();
                var refreshExpiry = req.RememberMe ? DateTime.Now.AddDays(30) : DateTime.Now.AddDays(1);

                // Optionally save refresh token in DB for invalidation
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiry = refreshExpiry;
                await _dbContext.SaveChangesAsync();

                // Set cookies securely
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // only for HTTPS
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddMinutes(30)
                };
                _httpContextAccessor.HttpContext.Response.Cookies.Append("access_token", accessToken, cookieOptions);


                var refreshOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = refreshExpiry
                };
                _httpContextAccessor.HttpContext.Response.Cookies.Append("refresh_token", refreshToken, refreshOptions);

                var sessionToken = Guid.NewGuid().ToString();
                var sessionExpiry = DateTime.Now.AddMinutes(30);

                var sessionTokenOptions = new CookieOptions
                {
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = sessionExpiry
                };
                _httpContextAccessor.HttpContext.Response.Cookies.Append("session_token", sessionToken, sessionTokenOptions);
                _httpContextAccessor.HttpContext.Response.Cookies.Append("user_id", user.Id, new CookieOptions
                {
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = sessionExpiry
                });
                _httpContextAccessor.HttpContext.Response.Cookies.Append("user_role", roles.FirstOrDefault(), new CookieOptions
                {
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = sessionExpiry
                });

                // Return only non-sensitive data
                //loginResponse.UserId = user.Id;
                //loginResponse.Role = roles.FirstOrDefault();
                //loginResponse.Token = Guid.NewGuid().ToString();

                response.Success = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Message = "Login Successful";
                //response.Result = loginResponse;

                return response;
            }
            catch(Exception ex)
            {
                response.Success = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
                return response;
            }
        }

        public async Task<ApiResponse> LogoutNew()
        {
            var response = new ApiResponse();

            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    response.Success = false;
                    response.StatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "HTTP context not available";
                    return response;
                }

                // Read refresh token from cookies
                var refreshToken = httpContext.Request.Cookies["refresh_token"];

                if (!string.IsNullOrEmpty(refreshToken))
                {
                    // Find the user who has this refresh token
                    var user = _dbContext.ApplicationUsers.FirstOrDefault(u => u.RefreshToken == refreshToken);
                    if (user != null)
                    {
                        // Invalidate refresh token
                        user.RefreshToken = null;
                        user.RefreshTokenExpiry = null;
                        await _dbContext.SaveChangesAsync();
                    }
                }

                // Delete cookies from browser
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.Now.AddDays(-1) // force expiry
                };
                var cookieSessionOptions = new CookieOptions
                {
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.Now.AddDays(-1) // force expiry
                };

                httpContext.Response.Cookies.Delete("access_token", cookieOptions);
                httpContext.Response.Cookies.Delete("refresh_token", cookieOptions);
                httpContext.Response.Cookies.Delete("session_token", cookieSessionOptions);
                httpContext.Response.Cookies.Delete("user_id", cookieSessionOptions);
                httpContext.Response.Cookies.Delete("user_role", cookieSessionOptions);

                response.Success = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Message = "Logout successful";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = $"Logout failed: {ex.Message}";
                return response;
            }
        }

        public async Task<ApiResponse> RefreshNew()
        {
            var response = new ApiResponse();
            var loginResponse = new LoginResponse();

            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    response.Success = false;
                    response.StatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "HTTP context not available";
                    return response;
                }

                var refreshToken = httpContext.Request.Cookies["refresh_token"];
                if (string.IsNullOrEmpty(refreshToken))
                {
                    response.Success = false;
                    response.StatusCode = HttpStatusCode.Unauthorized;
                    response.Message = "No refresh token found";
                    return response;
                }

                var user = _dbContext.ApplicationUsers.FirstOrDefault(u => u.RefreshToken == refreshToken);
                if (user == null || user.RefreshTokenExpiry < DateTime.Now)
                {
                    response.Success = false;
                    response.StatusCode = HttpStatusCode.Unauthorized;
                    response.Message = "Invalid or expired refresh token";
                    return response;
                }

                var roles = await _userManager.GetRolesAsync(user);
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_secretKey);

                // Create a new short-lived access token
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, roles.FirstOrDefault() ?? "User")
            }),
                    Expires = DateTime.Now.AddMinutes(30),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var newAccessToken = tokenHandler.WriteToken(token);

                // Update the cookie
                httpContext.Response.Cookies.Append("access_token", newAccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.Now.AddMinutes(30)
                });

                // Update the session token
                var sessionToken = Guid.NewGuid().ToString();
                var sessionExpiry = DateTime.Now.AddMinutes(30);
                
                httpContext.Response.Cookies.Append("session_token", sessionToken, new CookieOptions
                {
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = sessionExpiry
                });
                httpContext.Response.Cookies.Append("user_id", user.Id, new CookieOptions
                {
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = sessionExpiry
                });
                httpContext.Response.Cookies.Append("user_role", roles.FirstOrDefault(), new CookieOptions
                {
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = sessionExpiry
                });

                // Return only non-sensitive data
                //loginResponse.UserId = user.Id;
                //loginResponse.Role = roles.FirstOrDefault();
                //loginResponse.Token = Guid.NewGuid().ToString();

                response.Success = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Message = "Token refreshed successfully";
                //response.Result = loginResponse;
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = $"Error refreshing token: {ex.Message}";
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
