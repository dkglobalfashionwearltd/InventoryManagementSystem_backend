using DkGLobalBackend.WebApi.Models;
using DkGLobalBackend.WebApi.Models.RequestDto;
using DkGLobalBackend.WebApi.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;


namespace DkGLobalBackend.WebApi.Controllers
{
    [Route("api/stocks")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly IStockService _stockService;
        private ApiResponse response;
        public StockController(IStockService stockService)
        {
            response = new ApiResponse();
            _stockService = stockService;
        }

        [HttpGet]
        [Route("getall")]
        [Authorize(Roles = "admin")]
        public async Task<ApiResponse> GetAllStock( CancellationToken cancellationToken)
        {
            response = await _stockService.GetAllStockAsync(cancellationToken);
            return response;
        }


        [HttpGet]
        [Route("get-by-model")]
        [Authorize(Roles = "admin")]
        public async Task<ApiResponse> GetByModel(string model, CancellationToken cancellationToken)
        {
            response = await _stockService.GetStockByModelAsync(model,cancellationToken);
            return response;
        }

        [HttpPost]
        [Route("manage")]
        [Authorize(Roles = "admin")]
        public async Task<ApiResponse> ManageStock(CreateAndUpdateStockDto req, CancellationToken cancellationToken)
        {
            response = await _stockService.ManageStockAsync(req, cancellationToken);
            return response;
        }


    }
}
