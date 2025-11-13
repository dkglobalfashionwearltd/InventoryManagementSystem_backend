using DkGLobalBackend.WebApi.Models;
using DkGLobalBackend.WebApi.Models.RequestDto;

namespace DkGLobalBackend.WebApi.Services.IServices
{
    public interface IStockService
    {
        Task<ApiResponse> GetAllStockAsync(CancellationToken cancellationToken);
        Task<ApiResponse> GetStockByModelAsync(string model,CancellationToken cancellationToken);
        Task<ApiResponse> ManageStockAsync(CreateAndUpdateStockDto req, CancellationToken cancellationToken);

        Task<bool> AddHistoryAsync(string id, string actionName, CancellationToken cancellationToken);
    }
}
