namespace DkGLobalBackend.WebApi.Services.IServices
{
    public interface IServiceManager
    {
        Task<int> Save();
        public IItem Items { get; }
        public ICategory Categories { get; }
        public IDepartment Departments { get; }
        public IItemUser ItemUsers { get; }
        public IAuth Auth { get; }
        public IAssignItem AssignItemUsers { get; }
    }
}
