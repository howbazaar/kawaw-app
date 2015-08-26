using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Channels;
using Kawaw.Database;
using Couchbase.Lite;
using Kawaw.JSON;

[assembly: Xamarin.Forms.Dependency(typeof(Kawaw.Shared.Implementation))]
// ReSharper disable once CheckNamespace
namespace Kawaw.Shared
{
    public class Implementation : IDatabase
    {
        private const string RemoteKey = "remote";
        private const string TokenKey = "token";
        private const string UserDetailsKey = "user";
        private const string EventsView = "events";
        private const string EventType = "event";
        private const string ConnectionsKey = "connections";
        private const string NotificationsKey = "notifications";

        private readonly Couchbase.Lite.Database _db;
        private ObservableCollection<Event> _events;

        public Implementation()
        {
            _db = Manager.SharedInstance.GetDatabase("kawaw");
            SetupEvents();
        }

        private void SetupEvents()
        {
            var view = _db.GetView(EventsView);
            view.SetMap((doc, emit) =>
            {
                if (!doc.ContainsKey("type")) return;
                if (doc["type"] as string == EventType)
                {
                    emit(doc["key"], doc["key"]);
                }
            }, "1");
            _events = new ObservableCollection<Event>(GetEvents());
            var query = view.CreateQuery().ToLiveQuery();
            query.Changed += (sender, e) =>
            {
                foreach (var row in e.Rows)
                {
                    Debug.WriteLine("row changed: {0}", row.DocumentId);
                    
                }
            };

        }

        public Remote GetRemote()
        {
            var doc = _db.GetExistingDocument(RemoteKey);
            if (doc == null)
            {
                return new Remote
                {
                    BaseUrl = Constants.BaseUrl
                };
            }
            return new Remote
            {
                BaseUrl = doc.GetProperty("base-url") as string,
                CSRFToken = doc.GetProperty("csrf-token") as string,
                SessionId = doc.GetProperty("session-id") as string,
            };
        }

        public void SetRemoteBaseUrl(string baseUrl)
        {
            Debug.WriteLine("SetRemoteBaseUrl = {0}", baseUrl);
            var doc = _db.GetDocument(RemoteKey);
            doc.Update(rev =>
            {
                var properties = rev.Properties;
                properties["base-url"] = baseUrl;
                return true;
            });
        }

        public void SetRemoteSession(string sessionId, string csrfToken)
        {
            Debug.WriteLine("SetRemoteSession = ({0}, {1})", sessionId, csrfToken);
            var doc = _db.GetDocument(RemoteKey);
            doc.Update(rev =>
            {
                var properties = rev.Properties;
                properties["csrf-token"] = csrfToken;
                properties["session-id"] = sessionId;
                return true;
            });
        }

        public string NotificationToken()
        {
            var doc = _db.GetExistingDocument(TokenKey);
            return doc == null ? null : doc.GetProperty<string>("current");
        }

        public void SetNotificationToken(string token)
        {
            Debug.Print("SetNotificationToken {0}", token ?? "<null>");
            var doc = _db.GetDocument(TokenKey);
            doc.Update(rev =>
            {
                var properties = rev.Properties;
                object currentObj;
                if (properties.TryGetValue("current", out currentObj))
                {
                    var current = currentObj as string;
                    if (!string.IsNullOrEmpty(current))
                    {
                        object oldObj;
                        properties.TryGetValue("old", out oldObj);
                        // Add it to the list of tokens to unregister.
                        var old = oldObj as List<string> ?? new List<string>();
                        old.Add(current);
                        properties["old"] = old;
                    }
                }
                properties["current"] = token;
                return true;
            });
        }

        public List<string> OldNotificationTokens()
        {
            var doc = _db.GetDocument(TokenKey);
            return doc.GetProperty<List<string>>("old");
        }

        public void RemoveOldNotificationToken(string token)
        {
            Debug.Print("RemoveOldNotificationToken {0}", token ?? "<null>");
            var doc = _db.GetDocument(TokenKey);
            doc.Update(rev =>
            {
                var properties = rev.Properties;
                // Remove the token from the old token list.
                object oldObj;
                if (properties.TryGetValue("old", out oldObj))
                {
                    var old = oldObj as List<string>;
                    if (old != null)
                    {
                        old.Remove(token);
                        properties["old"] = old;
                    }
                }
                return true;
            });
        }

        User IDatabase.User { get { return GetUser(); } set { SetUser(value);} }
        Connection[] IDatabase.Connections { get { return GetConnections(); } set { SetConnections(value); } }

        ObservableCollection<Event> IDatabase.Events => _events;

        Notification[] IDatabase.Notifications { get { return GetNotifications(); } set { SetNotifications(value); } }

        private User GetUser()
        {
            var doc = _db.GetExistingDocument(UserDetailsKey);
            return doc?.GetProperty<User>("details");
        }

        private void SetUser(User user)
        {
            var doc = _db.GetDocument(UserDetailsKey);
            doc.Update(rev =>
            {
                var properties = rev.Properties;
                properties["details"] = user;
                return true;
            });
        }

        private Connection[] GetConnections()
        {
            var doc = _db.GetExistingDocument(ConnectionsKey);
            return doc?.GetProperty<Connection[]>("details");
        }

        private void SetConnections(Connection[] connections)
        {
            var doc = _db.GetDocument(ConnectionsKey);
            doc.Update(rev =>
            {
                var properties = rev.Properties;
                properties["details"] = connections;
                return true;
            });
        }

        private IEnumerable<Event> GetEvents()
        {
            var query = _db.GetView(EventsView).CreateQuery();
            var rows = query.Run();
            return rows.Select(row => row.DocumentProperties).Select(properties => properties["details"] as Event);
        }

        public void SaveEvents(IEnumerable<Event> events)
        {
            var ids = new HashSet<uint>(GetEvents().Select(e => e.Id));
            foreach (var e in events)
            {
                SaveEvent(e);
                ids.Remove(e.Id);
            }
            foreach (var id in ids)
            {
                Debug.WriteLine("Removing deleted event id {0}", id);
                var doc = _db.GetDocument(EventKey(id));
                doc.Delete();
            }
        }

        private static string EventKey(uint id)
        {
            return "event-" + id;
        }

        private Event Event(uint id)
        {
            var doc = _db.GetExistingDocument(EventKey(id));
            return doc?.GetProperty<Event>("details");
        }

        private void SaveEvent(Event jsonEvent)
        {
            Debug.WriteLine("Saving event id {0}", jsonEvent.Id);
            var key = EventKey(jsonEvent.Id);
            var doc = _db.GetDocument(key);
            doc.Update(rev =>
            {
                var properties = rev.Properties;
                properties["key"] = key;
                properties["type"] = EventType;
                properties["details"] = jsonEvent;
                return true;
            });
        }


        private Notification[] GetNotifications()
        {
            var doc = _db.GetExistingDocument(NotificationsKey);
            return doc?.GetProperty<Notification[]>("details");
        }

        private void SetNotifications(Notification[] notifications)
        {
            var doc = _db.GetDocument(NotificationsKey);
            doc.Update(rev =>
            {
                var properties = rev.Properties;
                properties["details"] = notifications;
                return true;
            });
        }

    }
}