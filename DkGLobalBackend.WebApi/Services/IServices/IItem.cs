using DkGLobalBackend.WebApi.Models;

namespace DkGLobalBackend.WebApi.Services.IServices
{
    public interface IItem : IServices<Item>
    {
        void Update(Item item);
    }
}
