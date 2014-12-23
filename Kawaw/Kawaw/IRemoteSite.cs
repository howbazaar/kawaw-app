using System;
using System.Threading.Tasks;

namespace Kawaw
{
    public interface IRemoteSite
    {
        string BaseUrl { get; set; }

        Task<RemoteUser> Login(string username, string password);
        Task<bool> Register(string email, string name, string password, string password2);
        Task<JSON.User> GetUserDetails();
        Task<JSON.User> AddEmail(string address);
        Task<JSON.User> EmailAction(string action, string address);
        void Logout();

        Task<JSON.User> UpdateUserDetails(string firstName, string lastName, string address, DateTime dateOfBirth);

        Task<JSON.Connection[]> GetConnections();
        Task<JSON.Connection> ConnectionAction(uint id, bool accept);
    }
}