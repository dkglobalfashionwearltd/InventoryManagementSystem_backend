using DkGLobalBackend.WebApi.Models;
using DkGLobalBackend.WebApi.Models.RequestDto;
using DkGLobalBackend.WebApi.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DkGLobalBackend.WebApi.Controllers
{
    [Route("api/department")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public DepartmentController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet]
        [Route("getall")]
        public async Task<ApiResponse> GetAllDepartment(CancellationToken cancellationToken)
        {
            var respose = new ApiResponse();
            try
            {
                var genericReq = new GenericServiceRequest<Department>
                {
                    Expression = null,
                    IncludeProperties = null,
                    Tracked = true,
                    CancellationToken = cancellationToken
                };
                var result = await _serviceManager.Departments.GetAllAsync(genericReq);
                if (result == null)
                {
                    respose.StatusCode = HttpStatusCode.NotFound;
                    respose.Success = false;
                    respose.Message = "Data List Not Found";
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
        public async Task<ApiResponse> GetDepartment(int DepartmentId, CancellationToken cancellationToken)
        {
            var respose = new ApiResponse();
            try
            {
                if (DepartmentId <= 0)
                {
                    respose.StatusCode = HttpStatusCode.BadRequest;
                    respose.Success = false;
                    respose.Message = "Id Not Found";
                    return respose;
                }
                var genericReq = new GenericServiceRequest<Department>
                {
                    Expression = x => x.DepartmentId == DepartmentId,
                    IncludeProperties = null,
                    Tracked = true,
                    CancellationToken = cancellationToken
                };
                var result = await _serviceManager.Departments.GetAsync(genericReq);
                if (result == null)
                {
                    respose.StatusCode = HttpStatusCode.NotFound;
                    respose.Success = false;
                    respose.Message = "Not Found";
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

        [HttpPost]
        [Route("create")]
        public async Task<ApiResponse> CreateDepartment(DepartmentDto itemDto)
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
                Department itemToCreate = new()
                {
                    Name = itemDto.Name,                    
                    Status = itemDto.Status,                   
                };
                await _serviceManager.Departments.AddAsync(itemToCreate);
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
        public async Task<ApiResponse> UpdateDepartment(DepartmentUpdateDto itemDto, CancellationToken cancellationToken)
        {
            var respose = new ApiResponse();
            try
            {
                if (itemDto.DepartmentId <= 0)
                {
                    respose.StatusCode = HttpStatusCode.BadRequest;
                    respose.Success = false;
                    respose.Message = "Id Not Found";
                    return respose;
                }
                var genericReq = new GenericServiceRequest<Department>
                {
                    Expression = x => x.DepartmentId == itemDto.DepartmentId,
                    IncludeProperties = null,
                    Tracked = true,
                    CancellationToken = cancellationToken
                };
                var result = await _serviceManager.Departments.GetAsync(genericReq);
                if (result == null)
                {
                    respose.StatusCode = HttpStatusCode.NotFound;
                    respose.Success = false;
                    respose.Message = "Not Found";
                    return respose;
                }
                result.Name = itemDto.Name ?? result.Name;
                result.Status = itemDto.Status ?? result.Status;

                _serviceManager.Departments.Update(result);
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

        [HttpDelete]
        [Route("delete")]
        public async Task<ApiResponse> DeleteDepartment(int DepartmentId, CancellationToken cancellationToken)
        {
            var respose = new ApiResponse();
            if (DepartmentId <= 0)
            {
                respose.StatusCode = HttpStatusCode.BadRequest;
                respose.Success = false;
                respose.Message = "Id Not Found";
                return respose;
            }
            try
            {
                var genericReq = new GenericServiceRequest<Department>
                {
                    Expression = x => x.DepartmentId == DepartmentId,
                    IncludeProperties = null,
                    Tracked = true,
                    CancellationToken = cancellationToken
                };
                var result = await _serviceManager.Departments.GetAsync(genericReq);
                if (result == null)
                {
                    respose.StatusCode = HttpStatusCode.NotFound;
                    respose.Success = false;
                    respose.Message = "Not Found";
                    return respose;
                }
                _serviceManager.Departments.Remove(result);
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
