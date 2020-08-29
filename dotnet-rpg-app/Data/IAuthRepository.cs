using System.Threading.Tasks;
using dotnet_rpg_app.Models;

namespace dotnet_rpg_app.Data
{
    public interface IAuthRepository
    {
        Task<ServiceResponse<int>> Register(User user, string password);
        Task<ServiceResponse<string>> Login(string username, string password);
        Task<bool> UserExists(string username);
    }
}