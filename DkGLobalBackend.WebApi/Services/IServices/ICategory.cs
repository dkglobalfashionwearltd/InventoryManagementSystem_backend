using DkGLobalBackend.WebApi.Models;

namespace DkGLobalBackend.WebApi.Services.IServices
{
    public interface ICategory : IServices<Category>
    {
        void Update(Category category);
    }
}
