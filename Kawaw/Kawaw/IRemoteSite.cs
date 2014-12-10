using System.Threading.Tasks;

namespace Kawaw
{
    public interface IRemoteSite
    {
        Task<bool> Login(string username, string password);
        Task<JSON.User> GetUserDetails();
    }
}