using DkGLobalBackend.WebApi.Models;

namespace DkGLobalBackend.WebApi.Services.IServices
{
    public interface IAssignItem : IServices<AssignItemUser>
    {
        void Update(AssignItemUser user);
    }
}
