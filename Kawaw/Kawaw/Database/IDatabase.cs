using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace Kawaw.Database
{
    public struct Remote
    {
        public string BaseUrl;
        // ReSharper disable once InconsistentNaming
        public string CSRFToken;
        public string SessionId;
    }

    public interface IDatabase
    {
        Task<Remote> GetRemote();
        Task SetRemoteBaseUrl(string baseUrl);
        Task SetRemoteSession(string sessionId, string csrfToken);

        Task<string> NotificationToken();
        void SetNotificationToken(string token);

        Task<List<string>> OldNotificationTokens();
        void RemoveOldNotificationToken(string token);

        Task<JSON.User> GetUserDetails();
        void SaveUserDetails(JSON.User user);

        Task<JSON.Event[]> GetEvents();
        void SaveEvents(JSON.Event[] events);

        Task<JSON.Connection[]> GetConnections();
        void SaveConnections(JSON.Connection[] connections);

        Task<JSON.Notification[]> GetNotification();
        void SaveNotifications(JSON.Notification[] notifications);
    }
}