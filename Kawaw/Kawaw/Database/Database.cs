using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;

[assembly: Xamarin.Forms.Dependency(typeof(Kawaw.Database.Implementation))]
namespace Kawaw.Database
{
    public class Implementation : IDatabase
    {
        private const string RemoteKey = "remote";

        private const string TokenKey = "token";
        private const string OldTokensKey = "old-tokens";

        private const string UserDetailsKey = "user";
        private const string EventsKey = "events";
        private const string ConnectionsKey = "connections";
        private const string NotificationsKey = "notifications";

        public Implementation()
        {
            BlobCache.ApplicationName = "kawaw";
        }

        public async Task<Remote> GetRemote()
        {
            return await BlobCache.UserAccount.GetObject<Remote>(RemoteKey)
                .Catch(Observable.Return(
                    new Remote
                    {
                        BaseUrl = Constants.BaseUrl
                    }
                ));
        }

        public async Task SetRemoteBaseUrl(string baseUrl)
        {
            var remote = await GetRemote();
            remote.BaseUrl = baseUrl;
            await BlobCache.UserAccount.InsertObject(RemoteKey, remote);
        }

        public async Task SetRemoteSession(string sessionId, string csrfToken)
        {
            Debug.WriteLine("SetRemoteSession = ({0}, {1})", sessionId, csrfToken);

            var remote = await GetRemote();
            remote.SessionId = sessionId;
            remote.CSRFToken = csrfToken;
            await BlobCache.UserAccount.InsertObject(RemoteKey, remote);
        }

        public async Task<string> NotificationToken()
        {
            return await BlobCache.UserAccount.GetObject<string>(TokenKey).Catch(Observable.Return<string>(null));
        }

        async void IDatabase.SetNotificationToken(string token)
        {
            var oldToken = await NotificationToken();
            if (!string.IsNullOrEmpty(oldToken))
            {
                var old = await OldNotificationTokens();
                old.Add(oldToken);
                await BlobCache.UserAccount.InsertObject(OldTokensKey, old);
            }
            await BlobCache.UserAccount.InsertObject(TokenKey, token);
        }

        public async Task<List<string>> OldNotificationTokens()
        {
            return await BlobCache.UserAccount.GetObject<List<string>>(OldTokensKey).Catch(Observable.Return(new List<string>()));
        }

        public async void RemoveOldNotificationToken(string token)
        {
            Debug.WriteLine("RemoveOldNotificationToken {0}", token ?? "<null>");
            var old = await OldNotificationTokens();
            old.Remove(token);
            await BlobCache.UserAccount.InsertObject(OldTokensKey, old);
        }

        async Task<JSON.User> IDatabase.GetUserDetails()
        {
            return await BlobCache.UserAccount.GetObject<JSON.User>(UserDetailsKey).Catch(Observable.Return(new JSON.User()));
        }

        async void IDatabase.SaveUserDetails(JSON.User value)
        {
            if (value == null)
            {
                await BlobCache.UserAccount.InvalidateObject<JSON.User>(UserDetailsKey);
            }
            else
            {
                await BlobCache.UserAccount.InsertObject(UserDetailsKey, value);
            }
        }

        async Task<JSON.Event[]> IDatabase.GetEvents()
        {
            return await BlobCache.UserAccount.GetObject<JSON.Event[]>(EventsKey).Catch(Observable.Return<JSON.Event[]>(null));
        }

        async void IDatabase.SaveEvents(JSON.Event[] events)
        {
            if (events == null)
            {
                await BlobCache.UserAccount.InvalidateObject<JSON.Event[]>(EventsKey);
            }
            else
            {
                await BlobCache.UserAccount.InsertObject(EventsKey, events);
            }
        }

        async Task<JSON.Connection[]> IDatabase.GetConnections()
        {
            return await BlobCache.UserAccount.GetObject<JSON.Connection[]>(ConnectionsKey).Catch(Observable.Return<JSON.Connection[]>(null));
        }

        async void IDatabase.SaveConnections(JSON.Connection[] connections)
        {
            if (connections == null)
            {
                await BlobCache.UserAccount.InvalidateObject<JSON.Connection[]>(ConnectionsKey);
            }
            else
            {
                await BlobCache.UserAccount.InsertObject(ConnectionsKey, connections);
            }
        }

        async Task<JSON.Notification[]> IDatabase.GetNotification()
        {
            return await BlobCache.UserAccount.GetObject<JSON.Notification[]>(NotificationsKey).Catch(Observable.Return<JSON.Notification[]>(null));
        }

        async void IDatabase.SaveNotifications(JSON.Notification[] notifications)
        {
            if (notifications == null)
            {
                await BlobCache.UserAccount.InvalidateObject<JSON.Notification[]>(NotificationsKey);
            }
            else
            {
                await BlobCache.UserAccount.InsertObject(NotificationsKey, notifications);
            }
        }

    }
}