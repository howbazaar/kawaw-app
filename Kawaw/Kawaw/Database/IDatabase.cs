using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;

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
        JSON.Connection[] Connections { get; set; }
        JSON.Notification[] Notifications { get; set; }

        ObservableCollection<JSON.Event> Events { get; }
        void SaveEvents(IEnumerable<JSON.Event> events);
    }
}