using DkGLobalBackend.WebApi.Models;
using DkGLobalBackend.WebApi.Models.RequestDto;
using DkGLobalBackend.WebApi.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DkGLobalBackend.WebApi.Controllers
{
    [Route("api/stocks")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        private ApiResponse response;
        public StockController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
            response = new ApiResponse();
        }

        [HttpGet]
        [Route("getall")]
        public async Task<ApiResponse> GetAllStock(string model, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(model))
            {
                response.Success = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.Message = "Model number required";
                return response;
            }

            try
            {
                var data = await _serviceManager.Stocks.GetAllAsync(new GenericServiceRequest<Stock>
                {
                    Tracked = true,
                    CancellationToken = cancellationToken
                });
                if (!data.Any())
                {
                    response.Success = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Message = "Data not found";
                    return response;
                }

                response.Success = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Message = "Successful";
                response.Result = data;
                return response;
            }
            catch (TaskCanceledException ex)
            {
                response.Success = false;
                response.StatusCode = HttpStatusCode.RequestTimeout;
                response.Message = ex.Message;
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
                return response;
            }
        }


        [HttpGet]
        [Route("get-by-model")]
        public async Task<ApiResponse> GetByModel(string model, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(model))
            {
                response.Success = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.Message = "Model number required";
                return response;
            }

            try
            {
                var data = await _serviceManager.Stocks.GetAsync(new GenericServiceRequest<Stock>
                {
                    Expression = x => x.ModelNumber == model,
                    Tracked = true,
                    CancellationToken = cancellationToken
                });
                if (data == null)
                {
                    response.Success = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Message = "Data not found";
                    return response;
                }

                response.Success = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Message = "Successful";
                response.Result = data;
                return response;
            }
            catch (TaskCanceledException ex)
            {
                response.Success = false;
                response.StatusCode = HttpStatusCode.RequestTimeout;
                response.Message = ex.Message;
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
                return response;
            }
        }

        [HttpPost]
        [Route("manage")]
        public async Task<ApiResponse> ManageStock(CreateAndUpdateStockDto req, CancellationToken cancellationToken)
        {
            if (req == null)
            {
                response.Success = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.Message = "Empty requested data.";
                return response;
            }

            try
            {
                // 1️⃣ Validate the item exists
                var item = await _serviceManager.Items.GetAsync(new GenericServiceRequest<Item>
                {
                    Expression = x => x.ModelNumber == req.ModelNumber,
                    CancellationToken = cancellationToken
                });

                if (item == null)
                {
                    response.Success = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Message = "Item not found.";
                    return response;
                }

                // 2️⃣ Get existing stock (if any)
                var existingStock = await _serviceManager.Stocks.GetAsync(new GenericServiceRequest<Stock>
                {
                    Expression = x => x.ModelNumber == req.ModelNumber,
                    CancellationToken = cancellationToken
                });

                // 3️⃣ Perform stock action
                switch (req.ActionType.ToLower())
                {
                    case "create":
                        if (existingStock != null)
                        {
                            response.Success = false;
                            response.StatusCode = HttpStatusCode.BadRequest;
                            response.Message = "Stock already exists for this item.";
                            return response;
                        }

                        var newStock = new Stock
                        {
                            ModelNumber = req.ModelNumber,
                            Quantity = req.Quantity,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                            IsDeleted = false
                        };
                        await _serviceManager.Stocks.AddAsync(newStock);
                        response.Message = "Stock created successfully.";
                        break;

                    case "plus":
                        if (existingStock == null)
                        {
                            response.Success = false;
                            response.StatusCode = HttpStatusCode.NotFound;
                            response.Message = "No existing stock found to add.";
                            return response;
                        }

                        existingStock.Quantity += req.Quantity;
                        existingStock.UpdatedAt = DateTime.Now;
                        _serviceManager.Stocks.Update(existingStock);
                        response.Message = "Stock increased successfully.";
                        break;

                    case "minus":
                        if (existingStock == null)
                        {
                            response.Success = false;
                            response.StatusCode = HttpStatusCode.NotFound;
                            response.Message = "No existing stock found to subtract.";
                            return response;
                        }

                        existingStock.Quantity -= req.Quantity;
                        if (existingStock.Quantity < 0)
                            existingStock.Quantity = 0;

                        existingStock.UpdatedAt = DateTime.Now;
                        _serviceManager.Stocks.Update(existingStock);
                        response.Message = "Stock decreased successfully.";
                        break;

                    default:
                        response.Success = false;
                        response.StatusCode = HttpStatusCode.BadRequest;
                        response.Message = "Invalid ActionType. Use 'create', 'plus', or 'minus'.";
                        return response;
                }

                // 4️⃣ Save changes
                int result = await _serviceManager.Save();
                response.Success = result > 0;
                response.StatusCode = HttpStatusCode.OK;

                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
                return response;
            }
        }




    }
}
