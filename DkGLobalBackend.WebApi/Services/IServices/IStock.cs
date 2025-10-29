using DkGLobalBackend.WebApi.Models;

namespace DkGLobalBackend.WebApi.Services.IServices
{
    public interface IStock : IServices<Stock>
    {
        void Update(Stock stock);
    }
}
