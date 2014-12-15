using System;
using System.Threading.Tasks;

namespace Kawaw
{
    public interface IRemoteSite
    {

        Task<bool> Login(string username, string password);
        Task<JSON.User> GetUserDetails();
        Task<JSON.User> AddEmail(string address);
        Task<JSON.User> EmailAction(string action, string address);
        void Logout();

        Task<JSON.User> UpdateUserDetails(string firstName, string lastName, string address, DateTime dateOfBirth);

        Task<JSON.Connection[]> GetConnections();

        string CSRFToken { get; }
        string SessionId { get; }
    }
}