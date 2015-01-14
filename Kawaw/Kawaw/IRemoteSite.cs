using System;
using System.Threading.Tasks;

namespace Kawaw
{
    public interface IRemoteSite
    {
        string BaseUrl { get; set; }

        Task<RemoteUser> Login(string username, string password);
        Task<RemoteUser> Register(string email, string password);
        void Logout();

        Task<JSON.User> GetUserDetails();
        Task<JSON.User> AddEmail(string address);
        Task<JSON.User> EmailAction(string action, string address);

        Task<JSON.User> UpdateUserDetails(string firstName, string lastName, string address, DateTime dateOfBirth);

        Task<JSON.Connection[]> GetConnections();
        Task<JSON.Connection> ConnectionAction(uint id, bool accept);

        Task<JSON.Event[]> GetEvents();
    }
}