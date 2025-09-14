using DkGLobalBackend.WebApi.Models;

namespace DkGLobalBackend.WebApi.Services.IServices
{
    public interface IItemUser : IServices<ItemUser>
    {
        void Update(ItemUser itemUser);
    }
}
