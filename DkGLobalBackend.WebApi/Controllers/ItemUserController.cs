using DkGLobalBackend.WebApi.Models;
using DkGLobalBackend.WebApi.Models.RequestDto;
using DkGLobalBackend.WebApi.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DkGLobalBackend.WebApi.Controllers
{
    [Route("api/item-user")]
    [ApiController]
    public class ItemUserController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public ItemUserController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet]
        [Route("getall")]
        public async Task<ApiResponse> GetAllItemUser(CancellationToken cancellationToken)
        {
            var respose = new ApiResponse();
            try
            {
                var genericReq = new GenericServiceRequest<ItemUser>
                {
                    Expression = null,
                    IncludeProperties = "Department",
                    Tracked = true,
                    CancellationToken = cancellationToken
                };
                var result = await _serviceManager.ItemUsers.GetAllAsync(genericReq);
                if (result == null)
                {
                    respose.StatusCode = HttpStatusCode.NotFound;
                    respose.Success = false;
                    respose.Message = "Item User List Not Found";
                    return respose;
                }
                respose.StatusCode = HttpStatusCode.OK;
                respose.Success = true;
                respose.Message = "Successful";
                respose.Result = result;

                return respose;
            }
            catch (OperationCanceledException ex)
            {
                respose.StatusCode = HttpStatusCode.RequestTimeout;
                respose.Success = false;
                respose.Message = ex.Message;
                return respose;
            }
            catch (Exception ex)
            {
                respose.StatusCode = HttpStatusCode.InternalServerError;
                respose.Success = false;
                respose.Message = ex.Message;
                return respose;
            }

        }

        [HttpGet]
        [Route("get")]
        public async Task<ApiResponse> GetItemUser(int ItemUserId, CancellationToken cancellationToken)
        {
            var respose = new ApiResponse();
            try
            {
                if (ItemUserId <= 0)
                {
                    respose.StatusCode = HttpStatusCode.BadRequest;
                    respose.Success = false;
                    respose.Message = "Id Not Found";
                    return respose;
                }
                var genericReq = new GenericServiceRequest<ItemUser>
                {
                    Expression = x => x.ItemUserId == ItemUserId,
                    IncludeProperties = "Department",
                    Tracked = true,
                    CancellationToken = cancellationToken
                };
                var itemsResult = await _serviceManager.ItemUsers.GetAsync(genericReq);
                if (itemsResult == null)
                {
                    respose.StatusCode = HttpStatusCode.NotFound;
                    respose.Success = false;
                    respose.Message = "Not Found";
                    return respose;
                }
                respose.StatusCode = HttpStatusCode.OK;
                respose.Success = true;
                respose.Message = "Successful";
                respose.Result = itemsResult;

                return respose;
            }
            catch (OperationCanceledException ex)
            {
                respose.StatusCode = HttpStatusCode.RequestTimeout;
                respose.Success = false;
                respose.Message = ex.Message;
                return respose;
            }
            catch (Exception ex)
            {
                respose.StatusCode = HttpStatusCode.InternalServerError;
                respose.Success = false;
                respose.Message = ex.Message;
                return respose;
            }
            
        }

        [HttpPost]
        [Route("create")]
        public async Task<ApiResponse> CreateItemUser(ItemUserDto itemDto)
        {
            var respose = new ApiResponse();
            try
            {
                if (itemDto == null)
                {
                    respose.StatusCode = HttpStatusCode.BadRequest;
                    respose.Success = false;
                    respose.Message = "Empty Request";
                    return respose;
                }
                ItemUser itemToCreate = new()
                {
                    Name = itemDto.Name,
                    OfficeId = itemDto.OfficeId,
                    PhoneNumber = itemDto.PhoneNumber,
                    Designation = itemDto.Designation,
                    DepartmentId = itemDto.DepartmentId,
                    Status = itemDto.Status,
                   
                };
                await _serviceManager.ItemUsers.AddAsync(itemToCreate);
                var res = await _serviceManager.Save();
                if(res < 1)
                {
                    respose.StatusCode = HttpStatusCode.InternalServerError;
                    respose.Success = false;
                    respose.Message = "Creation Failed";
                    return respose;
                }
                else
                {
                    respose.StatusCode = HttpStatusCode.Created;
                    respose.Success = true;
                    respose.Message = "Created Successfully";
                    return respose;
                }
            }
            catch (Exception ex)
            {
                respose.StatusCode = HttpStatusCode.InternalServerError;
                respose.Success = false;
                respose.Message = ex.Message;
                return respose;
            }
            
        }

        [HttpPost]
        [Route("update")]
        public async Task<ApiResponse> UpdateItemUser(ItemUserUpdateDto itemDto,CancellationToken cancellationToken)
        {
            var respose = new ApiResponse();
            try
            {
                if (itemDto.ItemUserId <=0)
                {
                    respose.StatusCode = HttpStatusCode.BadRequest;
                    respose.Success = false;
                    respose.Message = "Id Not Found";
                    return respose;
                }
                var genericReq = new GenericServiceRequest<ItemUser>
                {
                    Expression = x => x.ItemUserId == itemDto.ItemUserId,
                    IncludeProperties = null,
                    Tracked = true,
                    CancellationToken = cancellationToken
                };
                var result = await _serviceManager.ItemUsers.GetAsync(genericReq);

                if (result == null)
                {
                    respose.StatusCode = HttpStatusCode.NotFound;
                    respose.Success = false;
                    respose.Message = "Not Found";
                    return respose;
                }

                result.Name = (itemDto.Name == "" || itemDto.Name == null) ?  result.Name : itemDto.Name;  
                result.OfficeId = itemDto.OfficeId <= 0 ? result.OfficeId : itemDto.OfficeId;
                result.PhoneNumber = itemDto.PhoneNumber ?? result.PhoneNumber;
                result.Designation = itemDto.PhoneNumber ?? result.Designation;
                result.Status = itemDto.Status ?? result.Status;
                result.DepartmentId = itemDto.DepartmentId;
               
                
                _serviceManager.ItemUsers.Update(result);
                var res = await _serviceManager.Save();
                if (res < 1)
                {
                    respose.StatusCode = HttpStatusCode.InternalServerError;
                    respose.Success = false;
                    respose.Message = "Update Failed";
                    return respose;
                }
                else
                {
                    respose.StatusCode = HttpStatusCode.OK;
                    respose.Success = true;
                    respose.Message = "Updated Successfully";
                    return respose;
                }
            }
            catch (OperationCanceledException ex)
            {
                respose.StatusCode = HttpStatusCode.RequestTimeout;
                respose.Success = false;
                respose.Message = ex.Message;
                return respose;
            }
            catch (Exception ex)
            {
                respose.StatusCode = HttpStatusCode.InternalServerError;
                respose.Success = false;
                respose.Message = ex.Message;
                return respose;
            }
        }

        [HttpPost]
        [Route("update-status")]
        public async Task<ApiResponse> UpdateStatusItemUser(UpdateStatusDto itemDto, CancellationToken cancellationToken)
        {
            var respose = new ApiResponse();
            try
            {
                if (itemDto.Id <= 0)
                {
                    respose.StatusCode = HttpStatusCode.BadRequest;
                    respose.Success = false;
                    respose.Message = "Id Not Found";
                    return respose;
                }
                var genericReq = new GenericServiceRequest<ItemUser>
                {
                    Expression = x => x.ItemUserId == itemDto.Id,
                    IncludeProperties = null,
                    Tracked = true,
                    CancellationToken = cancellationToken
                };
                var result = await _serviceManager.ItemUsers.GetAsync(genericReq);

                if (result == null)
                {
                    respose.StatusCode = HttpStatusCode.NotFound;
                    respose.Success = false;
                    respose.Message = "Not Found";
                    return respose;
                }
              
                result.Status = itemDto.Status ?? result.Status;
               

                _serviceManager.ItemUsers.Update(result);
                var res = await _serviceManager.Save();
                if (res < 1)
                {
                    respose.StatusCode = HttpStatusCode.InternalServerError;
                    respose.Success = false;
                    respose.Message = "Failed to update status";
                    return respose;
                }
                else
                {
                    respose.StatusCode = HttpStatusCode.OK;
                    respose.Success = true;
                    respose.Message = "Status Updated Successfully";
                    return respose;
                }
            }
            catch (OperationCanceledException ex)
            {
                respose.StatusCode = HttpStatusCode.RequestTimeout;
                respose.Success = false;
                respose.Message = ex.Message;
                return respose;
            }
            catch (Exception ex)
            {
                respose.StatusCode = HttpStatusCode.InternalServerError;
                respose.Success = false;
                respose.Message = ex.Message;
                return respose;
            }
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<ApiResponse> DeleteItemUser(int ItemUserId, CancellationToken cancellationToken)
        {
            var respose = new ApiResponse();
            if (ItemUserId <= 0)
            {
                respose.StatusCode = HttpStatusCode.BadRequest;
                respose.Success = false;
                respose.Message = "Id Not Found";
                return respose;
            }
            try
            {
                var genericReq = new GenericServiceRequest<ItemUser>
                {
                    Expression = x => x.ItemUserId == ItemUserId,
                    IncludeProperties = null,
                    Tracked = true,
                    CancellationToken = cancellationToken
                };
                var result = await _serviceManager.ItemUsers.GetAsync(genericReq);
                if (result == null)
                {
                    respose.StatusCode = HttpStatusCode.NotFound;
                    respose.Success = false;
                    respose.Message = "Not Found";
                    return respose;
                }
                _serviceManager.ItemUsers.Remove(result);
                await _serviceManager.Save();
                respose.StatusCode = HttpStatusCode.OK;
                respose.Success = true;
                respose.Message = "Delete Successful";

                return respose;
            }
            catch (OperationCanceledException ex)
            {
                respose.StatusCode = HttpStatusCode.RequestTimeout;
                respose.Success = false;
                respose.Message = ex.Message;
                return respose;
            }
            catch (Exception ex)
            {
                respose.StatusCode = HttpStatusCode.InternalServerError;
                respose.Success = false;
                respose.Message = ex.Message;
                return respose;
            }

        }

    }
}
