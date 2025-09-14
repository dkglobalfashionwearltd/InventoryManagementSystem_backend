using DkGLobalBackend.WebApi.Models;
using DkGLobalBackend.WebApi.Models.RequestDto;
using DkGLobalBackend.WebApi.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace DkGLobalBackend.WebApi.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public CategoryController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet]
        [Route("getall")]
        public async Task<ApiResponse> GetAllCategory(CancellationToken cancellationToken)
        {
            var respose = new ApiResponse();
            try
            {
                var genericReq = new GenericServiceRequest<Category>
                {
                    Expression = null,
                    IncludeProperties = null,
                    Tracked = true,
                    CancellationToken = cancellationToken
                };
                var categoryResult = await _serviceManager.Categories.GetAllAsync(genericReq);
                if (categoryResult.Any())
                {
                    respose.StatusCode = HttpStatusCode.OK;
                    respose.Success = true;
                    respose.Message = "Successful";
                    respose.Result = categoryResult;

                    return respose;
                }
                respose.StatusCode = HttpStatusCode.NotFound;
                respose.Success = false;
                respose.Message = "Data List Not Found";
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
        public async Task<ApiResponse> GetCategory(int CategoryId, CancellationToken cancellationToken)
        {
            var respose = new ApiResponse();
            try
            {
                if (CategoryId <= 0)
                {
                    respose.StatusCode = HttpStatusCode.BadRequest;
                    respose.Success = false;
                    respose.Message = "Id Not Found";
                    return respose;
                }
                var genericReq = new GenericServiceRequest<Category>
                {
                    Expression = x => x.CategoryId == CategoryId,
                    IncludeProperties = null,
                    Tracked = true,
                    CancellationToken = cancellationToken
                };
                var categoryResult = await _serviceManager.Categories.GetAsync(genericReq);
                
                if (categoryResult != null)
                { 
                    respose.StatusCode = HttpStatusCode.OK;
                    respose.Success = true;
                    respose.Message = "Successful";
                    respose.Result = categoryResult;

                    return respose;
                }
                respose.StatusCode = HttpStatusCode.NotFound;
                respose.Success = false;
                respose.Message = "Not Found";
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
        public async Task<ApiResponse> CreateCategory(CategoryDto itemDto)
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
                Category itemToCreate = new()
                {
                    Name = itemDto.Name,                    
                    Status = itemDto.Status,                   
                };
                await _serviceManager.Categories.AddAsync(itemToCreate);
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
        public async Task<ApiResponse> UpdateCategory(CategoryUpdateDto itemDto, CancellationToken cancellationToken)
        {
            var respose = new ApiResponse();
            try
            {
                if (itemDto.CategoryId <= 0)
                {
                    respose.StatusCode = HttpStatusCode.BadRequest;
                    respose.Success = false;
                    respose.Message = "Id Not Found";
                    return respose;
                }
                var genericReq = new GenericServiceRequest<Category>
                {
                    Expression = x => x.CategoryId == itemDto.CategoryId,
                    IncludeProperties = null,
                    Tracked = true,
                    CancellationToken = cancellationToken
                };
                var result = await _serviceManager.Categories.GetAsync(genericReq);
                if (result == null)
                {
                    respose.StatusCode = HttpStatusCode.NotFound;
                    respose.Success = false;
                    respose.Message = "Not Found";
                    return respose;
                }
                result.Name = itemDto.Name ?? result.Name;                
                result.Status = itemDto.Status ?? result.Status;

                _serviceManager.Categories.Update(result);
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
        public async Task<ApiResponse> DeleteCategory(int CategoryId, CancellationToken cancellationToken)
        {
            var respose = new ApiResponse();
            if (CategoryId <= 0)
            {
                respose.StatusCode = HttpStatusCode.BadRequest;
                respose.Success = false;
                respose.Message = "Id Not Found";
                return respose;
            }
            try
            {
                var genericReq = new GenericServiceRequest<Category>
                {
                    Expression = x => x.CategoryId == CategoryId,
                    IncludeProperties = null,
                    Tracked = true,
                    CancellationToken = cancellationToken
                };
                var categoryResult = await _serviceManager.Categories.GetAsync(genericReq);
                if (categoryResult != null)
                {
                    _serviceManager.Categories.Remove(categoryResult);
                    await _serviceManager.Save();
                    respose.StatusCode = HttpStatusCode.OK;
                    respose.Success = true;
                    respose.Message = "Delete Successful";

                    return respose;
                }
                    respose.StatusCode = HttpStatusCode.NotFound;
                    respose.Success = false;
                    respose.Message = "Not Found";
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
