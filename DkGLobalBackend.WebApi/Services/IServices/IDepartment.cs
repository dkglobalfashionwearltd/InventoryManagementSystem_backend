using DkGLobalBackend.WebApi.Models;

namespace DkGLobalBackend.WebApi.Services.IServices
{
    public interface IDepartment : IServices<Department>
    {
        void Update(Department department);
    }
}
