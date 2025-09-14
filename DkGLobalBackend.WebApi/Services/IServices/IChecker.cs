namespace DkGLobalBackend.WebApi.Services.IServices
{
    public interface IChecker
    {
        Task<bool> IsDatabaseConnectedAsync(string conStr);

    }
}
