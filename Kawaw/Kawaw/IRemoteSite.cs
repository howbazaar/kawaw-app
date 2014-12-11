using System;
using System.Threading.Tasks;

namespace Kawaw
{
    public interface IRemoteSite
    {

        Task<bool> Login(string username, string password);
        Task<JSON.User> GetUserDetails();
        void Logout();

        Task<JSON.User> UpdateUserDetails(string firstName, string lastName, string address, DateTime dateOfBirth);

        string CSRFToken { get; }
        string SessionId { get; }
    }
}