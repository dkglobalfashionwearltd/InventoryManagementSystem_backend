using DkGLobalBackend.WebApi.Database;
using DkGLobalBackend.WebApi.Models;
using DkGLobalBackend.WebApi.Models.RequestDto;
using DkGLobalBackend.WebApi.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;


namespace DkGLobalBackend.WebApi.Controllers
{
    [Route("api/assign")]
    [ApiController]
    public class AssignController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        private InventoryDbContext _context;
        public AssignController(IServiceManager db, InventoryDbContext context)
        {
            _serviceManager = db;
            _context = context;
        }

        [HttpGet]
        [Route("getall")]
        [Authorize(Roles = "admin")]
        public async Task<ApiResponse> GetAllAssignItemUser(CancellationToken cancellationToken)
        {
            var response = new ApiResponse();
            try
            {
                var result = await _context.ItemUsers
                        .Include(u => u.Department)
                        .Include(u => u.AssignItemUsers)
                            .ThenInclude(au => au.Item)
                                .ThenInclude(i => i.Category)
                        .AsNoTracking()
                        .ToListAsync(cancellationToken);

                var projected = result.Select(u => new
                {
                    u.ItemUserId,
                    u.Name,
                    DepartmentName = u.Department?.Name,
                    u.Designation,
                    u.OfficeId,
                    Items = u.AssignItemUsers.Select(au => new
                    {
                        au.Item.ItemId,
                        au.Item.Name,
                        CategoryName = au.Item.Category.Name,
                        au.AssignedDate,
                        au.Remarks,
                        au.AssignAgainstTo,
                        au.Status
                    }).ToList()
                }).ToList();

                if (!projected.Any())
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Success = false;
                    response.Message = "No assignments found.";
                    return response;
                }

                response.StatusCode = HttpStatusCode.OK;
                response.Success = true;
                response.Message = "Success";
                response.Result = projected;
                return response;
            }
            catch (OperationCanceledException ex)
            {
                response.StatusCode = HttpStatusCode.RequestTimeout;
                response.Success = false;
                response.Message = ex.Message;
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
        [Route("item-user")]
        [Authorize(Roles = "admin")]
        public async Task<ApiResponse> AssignItemUser(AssignItemUserDto dto, CancellationToken cancellationToken)
        {
            var response = new ApiResponse();
            if (!dto.ItemIds.Any() || !dto.ItemUserIds.Any())
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.Success = false;
                response.Message = "At least one ItemId and one ItemUserId are required.";
                return response;
            }

            try
            {
                // create 

                foreach (var itemId in dto.ItemIds)
                {
                    foreach (var userId in dto.ItemUserIds)
                    {
                        var exists = await _serviceManager.AssignItemUsers.AnyAsync(new GenericServiceRequest<AssignItemUser>
                        {
                            Expression = u => u.ItemId == int.Parse(itemId) && u.ItemUserId == int.Parse(userId),
                            IncludeProperties = null,
                            Tracked = false,
                            CancellationToken = cancellationToken
                        });
                        if (!exists)
                        {
                            await _serviceManager.AssignItemUsers.AddAsync(new AssignItemUser
                            {
                                ItemId = int.Parse(itemId),
                                ItemUserId = int.Parse(userId),
                                AssignedDate = dto.AssignedDate,
                                Remarks = dto.Remarks,
                                AssignAgainstTo = int.Parse(dto.AssignAgainstTo ?? "0"),
                                Status = "active",
                            });
                        }
                    }
                }

                await _serviceManager.Save();

                response.StatusCode = HttpStatusCode.Created;
                response.Success = true;
                response.Message = "Assignment(s) successful.";
                return response;


            }
            catch (TaskCanceledException ex)
            {
                response.StatusCode = HttpStatusCode.RequestTimeout;
                response.Success = false;
                response.Message = ex.Message;
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
    }
}
