using DkGLobalBackend.WebApi.Services.IServices;
using MySqlConnector;

namespace DkGLobalBackend.WebApi.Services
{
    public class Checker : IChecker
    {
       
        public async Task<bool> IsDatabaseConnectedAsync(string conStr)
        {
            try
            {
                await using var connection = new MySqlConnection(conStr);
                await connection.OpenAsync();
                return true;

            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Database connection failed: {ex.Message}");
                return false;
            }
        }
    }
}
