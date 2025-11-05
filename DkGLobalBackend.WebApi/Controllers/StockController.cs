using DkGLobalBackend.WebApi.Models;
using DkGLobalBackend.WebApi.Models.RequestDto;
using DkGLobalBackend.WebApi.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Net;


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
        public async Task<ApiResponse> GetAllStock( CancellationToken cancellationToken)
        {

            try
            {
                var data = await _serviceManager.Stocks.GetAllAsync(new GenericServiceRequest<Stock>
                {
                    IncludeProperties = "Item",
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

                var dataToDisplay = data.Select(x => new
                {
                    StockId = x.Id,
                    x.ItemId,
                    ItemName = x.Item.Name,
                    ItemModel = x.Item.ModelNumber,
                    x.TotalGivenQuantity,
                    x.LastQuantity,
                    x.CurrentQuantity,
                    StockedAt = x.CreatedAt,
                    LastStockedAt = x.UpdatedAt,
                    x.StockOutAt,
                    x.StockCount
                });

                response.Success = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Message = "Successful";
                response.Result = dataToDisplay;
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
                    Expression = x => x.Item.ModelNumber == model,
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

                var dataToDisplay = new
                {
                    StockId = data.Id,
                    data.ItemId,
                    ItemName = data.Item.Name,
                    ItemModel = data.Item.ModelNumber,
                    data.TotalGivenQuantity,
                    data.LastQuantity,
                    data.CurrentQuantity,
                    StockedAt = data.CreatedAt,
                    LastStockedAt = data.UpdatedAt,
                    data.StockOutAt,
                    data.StockCount
                };

                response.Success = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Message = "Successful";
                response.Result = dataToDisplay;
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
                Item itemData = new();
                Stock existingStock = new();
                // Validate the stock exists
                if (req.ModelNumber != null)
                {
                    existingStock = await _serviceManager.Stocks.GetAsync(new GenericServiceRequest<Stock>
                    {
                        Expression = x => x.Item.ModelNumber == req.ModelNumber,
                        CancellationToken = cancellationToken
                    });
                }
                // Validate the item&stock exists
                if (req.ItemId > 0)
                {
                    itemData = await _serviceManager.Items.GetAsync(new GenericServiceRequest<Item>
                    {
                        Expression = x => x.ItemId == req.ItemId,
                        CancellationToken = cancellationToken
                    });
                    existingStock = await _serviceManager.Stocks.GetAsync(new GenericServiceRequest<Stock>
                    {
                        Expression = x => x.Item.ItemId == req.ItemId,
                        CancellationToken = cancellationToken
                    });
                }

                // Perform stock action
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
                        if (itemData == null)
                        {
                            response.Success = false;
                            response.StatusCode = HttpStatusCode.NotFound;
                            response.Message = "Item not found.";
                            return response;
                        }
                        
                        var newStock = new Stock
                        {
                            ItemId = req.ItemId,
                            TotalGivenQuantity = req.Quantity,
                            LastQuantity = req.Quantity,
                            CurrentQuantity = req.Quantity,
                            StockCount = 1,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                            IsDeleted = false
                        };
                        await _serviceManager.Stocks.AddAsync(newStock);
                        await AddHistory(req.ActionBy, $"Stock {req.ActionType}", cancellationToken);
                        response.Message = "Item stocked successfully.";
                        break;

                    case "plus":
                        if (existingStock == null)
                        {
                            response.Success = false;
                            response.StatusCode = HttpStatusCode.NotFound;
                            response.Message = "No existing stock found to add.";
                            return response;
                        }

                        existingStock.TotalGivenQuantity += req.Quantity;
                        existingStock.LastQuantity = req.Quantity;
                        existingStock.CurrentQuantity += req.Quantity;
                        existingStock.StockCount++;
                        existingStock.UpdatedAt = DateTime.Now;
                        _serviceManager.Stocks.Update(existingStock);
                        await AddHistory(req.ActionBy, $"Stock {req.ActionType}", cancellationToken);
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

                        existingStock.CurrentQuantity -= req.Quantity;
                        if (existingStock.CurrentQuantity <= 0)
                            existingStock.CurrentQuantity = 0;
                            existingStock.StockOutAt = DateTime.Now;

                        _serviceManager.Stocks.Update(existingStock);
                        await AddHistory(req.ActionBy, $"Stock {req.ActionType}", cancellationToken);
                        response.Message = "Stock decreased successfully.";
                        break;
                    case "deactivate":
                        if (existingStock == null)
                        {
                            response.Success = false;
                            response.StatusCode = HttpStatusCode.NotFound;
                            response.Message = "No existing stock found to deactivate.";
                            return response;
                        }

                        existingStock.IsDeleted = true;
                        existingStock.DeletedAt = DateTime.Now;
                        _serviceManager.Stocks.Update(existingStock);
                        await AddHistory(req.ActionBy, $"Stock {req.ActionType}", cancellationToken);
                        response.Message = "Stock deactivated successfully.";
                        break;
                    case "delete":
                        if (existingStock == null)
                        {
                            response.Success = false;
                            response.StatusCode = HttpStatusCode.NotFound;
                            response.Message = "No existing stock found to deactivate.";
                            return response;
                        }
                        _serviceManager.Stocks.Remove(existingStock);
                        await AddHistory(req.ActionBy, $"Stock {req.ActionType}", cancellationToken);
                        response.Message = "Stock deleted successfully.";
                        break;

                    default:
                        response.Success = false;
                        response.StatusCode = HttpStatusCode.BadRequest;
                        response.Message = "Invalid ActionType. Use 'create', 'deactivate', 'delete', 'plus', or 'minus'.";
                        return response;
                }

                // Save changes
                int result = await _serviceManager.Save();
                response.Success = result > 0;
                response.StatusCode = HttpStatusCode.OK;
                return response;
            }
            catch(TaskCanceledException ex)
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

        private async Task<bool> AddHistory(string id,string actionName, CancellationToken cancellationToken)
        {
            try
            {
                var userData = await _serviceManager.Auth.GetAsync(new GenericServiceRequest<ApplicationUser>
                {
                    Expression = x=>x.Id == id,
                    Tracked = true,
                    CancellationToken = cancellationToken
                });
                var dataToAddHistory = new History
                {
                    ActionTitle = actionName,
                    ActionBysId = id,
                    ActionBysName = userData.UserName ?? "",
                    ActionAt = DateTime.Now,
                };
                await _serviceManager.Histories.AddAsync(dataToAddHistory);
                int i = await _serviceManager.Save();
                return i > 0;
            }catch (Exception ex)
            {
                return false;
            }
        }

    }
}
