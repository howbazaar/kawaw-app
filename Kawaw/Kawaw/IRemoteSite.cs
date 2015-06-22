using System;
using System.Threading.Tasks;
using Kawaw.Models;

namespace Kawaw
{
    public interface IRemoteSite
    {
        string BaseUrl { get; set; }

        Task<User> Login(string username, string password);
        Task<User> Register(string email, string password);
        void Logout();

        Task<JSON.User> GetUserDetails();
        Task<JSON.User> AddEmail(string address);
        Task<JSON.User> EmailAction(string action, string address);

        Task<JSON.User> UpdateUserDetails(string firstName, string lastName, string address, DateTime dateOfBirth);

        Task<JSON.Connection[]> GetConnections();
        Task<JSON.Connection> ConnectionAction(uint id, bool accept);

        Task<JSON.Notification[]> GetNotifications();
        Task NotificationAction(uint notificationId, uint memberId, bool accepted);

        Task<JSON.Event[]> GetEvents();

        Task<bool> RegisterDevice(string token);
        Task<bool> UnregisterDevice(string token);
    }
}