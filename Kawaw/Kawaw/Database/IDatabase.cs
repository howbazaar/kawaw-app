using System.Collections.Generic;

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
        Remote GetRemote();
        void SetRemoteBaseUrl(string baseUrl);
        void SetRemoteSession(string sessionId, string csrfToken);

        string NotificationToken();
        void SetNotificationToken(string token);
        List<string> OldNotificationTokens();
        void RemoveOldNotificationToken(string token);

        JSON.User User { get; set; }
        JSON.Event Events { get; set; }
        JSON.Connection[] Connections { get; set; }
        JSON.Notification[] Notifications { get; set; }
    }
}