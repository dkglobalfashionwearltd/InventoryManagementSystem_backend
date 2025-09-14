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
    [Route("api/item")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public ItemController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet]
        [Route("getall")]
        public async Task<ApiResponse> GetAllItem(CancellationToken cancellationToken)
        {
            var respose = new ApiResponse();
            try
            {
                var genericReq = new GenericServiceRequest<Item>
                {
                    Expression = null,
                    IncludeProperties = "Category",
                    Tracked = true,
                    CancellationToken = cancellationToken
                };
                var itemsResult = await _serviceManager.Items.GetAllAsync(genericReq);
                if (itemsResult == null)
                {
                    respose.StatusCode = HttpStatusCode.NotFound;
                    respose.Success = false;
                    respose.Message = "Item List Not Found";
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

        [HttpGet]
        [Route("get")]
        public async Task<ApiResponse> GetItem(int ItemId,CancellationToken cancellationToken)
        {
            var respose = new ApiResponse();
            try
            {
                if (ItemId <= 0)
                {
                    respose.StatusCode = HttpStatusCode.BadRequest;
                    respose.Success = false;
                    respose.Message = "Id Not Found";
                    return respose;
                }
                var genericReq = new GenericServiceRequest<Item>
                {
                    Expression = x => x.ItemId == ItemId,
                    IncludeProperties = "Category",
                    Tracked = true,
                    CancellationToken = cancellationToken
                };
                var itemsResult = await _serviceManager.Items.GetAsync(genericReq);
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
        public async Task<ApiResponse> CreateItem(ItemDto itemDto)
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
                Item itemToCreate = new()
                {
                    Name = itemDto.Name,
                    ModelNumber = itemDto.ModelNumber,
                    SerialNumber = itemDto.SerialNumber,
                    BrandName = itemDto.BrandName,
                    Price = itemDto.Price,
                    PurchaseDate = itemDto.PurchaseDate,
                    SourceName = itemDto.SourceName,
                    SourcePhoneNumber = itemDto.SourcePhoneNumber,
                    LastServicingDate = itemDto.LastServicingDate,
                    NextServicingDate = itemDto.NextServicingDate,
                    ServiceProviderName = itemDto.ServiceProviderName,
                    ServiceProviderPhoneNumber = itemDto.ServiceProviderPhoneNumber,
                    Status = itemDto.Status,
                    CategoryId = itemDto.CategoryId,
                    ItemCondition = itemDto.ItemCondition,
                    WarrantyEnd = itemDto.WarrantyEnd,
                    Quantity = itemDto.Quantity,
                };
                await _serviceManager.Items.AddAsync(itemToCreate);
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
        public async Task<ApiResponse> UpdateItem(ItemUpdateDto itemDto,CancellationToken cancellationToken)
        {
            var respose = new ApiResponse();
            try
            {
                if (itemDto.ItemId <=0)
                {
                    respose.StatusCode = HttpStatusCode.BadRequest;
                    respose.Success = false;
                    respose.Message = "Id Not Found";
                    return respose;
                }
                var genericReq = new GenericServiceRequest<Item>
                {
                    Expression = x => x.ItemId == itemDto.ItemId,
                    IncludeProperties = null,
                    Tracked = true,
                    CancellationToken = cancellationToken
                };
                var result = await _serviceManager.Items.GetAsync(genericReq);

                if (result == null)
                {
                    respose.StatusCode = HttpStatusCode.NotFound;
                    respose.Success = false;
                    respose.Message = "Not Found";
                    return respose;
                }

                result.Name = (itemDto.Name == "" || itemDto.Name == null) ?  result.Name : itemDto.Name;
                result.ModelNumber = (itemDto.ModelNumber == "" || itemDto.ModelNumber == null) ? result.ModelNumber : itemDto.ModelNumber;
                result.SerialNumber = itemDto.SerialNumber ?? result.SerialNumber;
                result.BrandName = itemDto.BrandName ?? result.BrandName;
                result.ItemCondition = itemDto.ItemCondition ?? result.ItemCondition;
                result.Quantity = itemDto.Quantity ?? result.Quantity;
                result.Price = itemDto.Price == 0 ? result.Price : itemDto.Price;
                result.PurchaseDate = itemDto.PurchaseDate.ToString() == null ? result.PurchaseDate : itemDto.PurchaseDate;
                result.WarrantyEnd = itemDto.WarrantyEnd.ToString() == null ? result.WarrantyEnd : itemDto.WarrantyEnd;
                result.SourceName = itemDto.SourceName ?? result.SourceName;
                result.SourcePhoneNumber = itemDto.SourcePhoneNumber ?? result.SourcePhoneNumber;
                result.LastServicingDate = itemDto.LastServicingDate.ToString() == null ? result.LastServicingDate : result.LastServicingDate;
                result.NextServicingDate = itemDto.NextServicingDate.ToString() == null ? result.NextServicingDate : result.NextServicingDate;
                result.ServiceProviderName = itemDto.ServiceProviderName ?? result.ServiceProviderName;
                result.ServiceProviderPhoneNumber = itemDto.ServiceProviderPhoneNumber ?? result.ServiceProviderPhoneNumber;
                result.Status = itemDto.Status ?? result.Status;
                result.CategoryId = itemDto.CategoryId;
                
                _serviceManager.Items.Update(result);
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
        public async Task<ApiResponse> UpdateStatusItem(UpdateStatusDto itemDto, CancellationToken cancellationToken)
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
                var genericReq = new GenericServiceRequest<Item>
                {
                    Expression = x => x.ItemId == itemDto.Id,
                    IncludeProperties = null,
                    Tracked = true,
                    CancellationToken = cancellationToken
                };
                var result = await _serviceManager.Items.GetAsync(genericReq);

                if (result == null)
                {
                    respose.StatusCode = HttpStatusCode.NotFound;
                    respose.Success = false;
                    respose.Message = "Not Found";
                    return respose;
                }
              
                result.Status = itemDto.Status ?? result.Status;
               

                _serviceManager.Items.Update(result);
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
        public async Task<ApiResponse> DeleteItem(int ItemId, CancellationToken cancellationToken)
        {
            var respose = new ApiResponse();
            if (ItemId <= 0)
            {
                respose.StatusCode = HttpStatusCode.BadRequest;
                respose.Success = false;
                respose.Message = "Id Not Found";
                return respose;
            }
            try
            {
                var genericReq = new GenericServiceRequest<Item>
                {
                    Expression = x => x.ItemId == ItemId,
                    IncludeProperties = null,
                    Tracked = true,
                    CancellationToken = cancellationToken
                };
                var result = await _serviceManager.Items.GetAsync(genericReq);
                if (result == null)
                {
                    respose.StatusCode = HttpStatusCode.NotFound;
                    respose.Success = false;
                    respose.Message = "Not Found";
                    return respose;
                }
                _serviceManager.Items.Remove(result);
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
