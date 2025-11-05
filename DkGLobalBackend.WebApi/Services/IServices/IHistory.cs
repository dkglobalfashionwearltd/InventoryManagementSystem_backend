using DkGLobalBackend.WebApi.Models;

namespace DkGLobalBackend.WebApi.Services.IServices
{
    public interface IHistory : IServices<History>
    {
        void Update(History history);
    }
}
