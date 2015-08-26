using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Kawaw.Database;
using Xamarin;
using Xamarin.Forms;

namespace Kawaw.Models
{
    public class User
    {
        public User()
        {
            _db = DependencyService.Get<IDatabase>();
            AsyncInit();
        }

        private async void AsyncInit()
        {
            var values = await _db.GetRemote();
            Remote = CreateRemoteSite(values);
            _user = await _db.GetUserDetails();
        }

        private RemoteSite CreateRemoteSite(Remote values)
        {
            Debug.WriteLine("BaseUrl = '{0}', CSRFToken = '{1}', SessionID = '{2}'", values.BaseUrl, values.CSRFToken, values.SessionId);
            var remote = new RemoteSite(values.BaseUrl, values.CSRFToken, values.SessionId);

            MessagingCenter.Subscribe(this, "remote-session-change",
                (object sender, Remote newValues) => _db.SetRemoteSession(newValues.SessionId, newValues.CSRFToken));
            MessagingCenter.Subscribe(this, "remote-baseurl-change",
                (object sender, string baseurl) => _db.SetRemoteBaseUrl(baseurl));

            // If there is a non-empty session id then the user is authenticated.
            Authenticated = !string.IsNullOrEmpty(values.SessionId);

            return remote;
        }

        public async Task Login(string email, string password)
        {
            await Remote.Login(email, password);
            await PostAuthenticate();
        }

        public async Task Register(string email, string password)
        {
            await Remote.Register(email, password);
            await PostAuthenticate();
        }

        private async Task PostAuthenticate()
        {
            Authenticated = true;
            await RegisterDevice();
            await Refresh();
        }

        public async Task Logout()
        {
            Debug.WriteLine("unregister device");
            await UnregisterDevice();
            await Remote.Logout();
            Authenticated = false;
            // Clear out details.
            await _db.SaveUserDetails(null);
            await _db.SaveEvents(null);
            await _db.SaveConnections(null);
            await _db.SaveNotifications(null);
        }

        public async Task AddEmail(string email)
        {
            var jsonUser = await Remote.AddEmail(email);
            await UpdateUser(jsonUser);
        }

        public async Task UpdateUserDetails(string firstName, string lastName, string address,
            DateTime dateOfBirth)
        {
            var jsonUser = await Remote.UpdateUserDetails(FirstName, LastName, Address, DateOfBirth);
            await UpdateUser(jsonUser);
        }

        public async Task EmailAction(string action, string address)
        {
            var jsonUser = await Remote.EmailAction(action, address);
            await UpdateUser(jsonUser);
        }

        static public readonly DateTime MinDateOfBirthValue = new DateTime(1900, 1, 1);

        private JSON.User _user;

        private readonly IDatabase _db;

        public RemoteSite Remote { get; private set; }

        private async Task UpdateUser(JSON.User user)
        {
            _user = user;
            await _db.SaveUserDetails(user);
            MessagingCenter.Send<object>(this, "user-updated");
        }

        private async Task UpdateConnections()
        {
            var connections = await Remote.GetConnections();
            await _db.SaveConnections(connections);
            MessagingCenter.Send<object>(this, "connections-updated");
        }

        private async Task UpdateEvents()
        {
            var events = await Remote.GetEvents();
            await _db.SaveEvents(events);
            MessagingCenter.Send<object>(this, "events-updated");
        }

        private async Task UpdateNotifications()
        {
            var notifications = await Remote.GetNotifications();
            await _db.SaveNotifications(notifications);
            MessagingCenter.Send<object>(this, "notifications-updated");
        }

        public async Task ConnectionAction(Connection connection, bool accept)
        {
            // NOTE: quite brutal at this stage, the entire list is serialized out
            // updated and serialized back into the database.
            var result = await Remote.ConnectionAction(connection.Id, accept);
            // Update our connections.
            var connections = await _db.GetConnections();
            foreach (var conn in connections.Where(conn => conn.Id == result.Id))
            {
                conn.Accepted = result.Accepted;
            }
            await _db.SaveConnections(connections);
            MessagingCenter.Send<object>(this, "connections-updated");
        }

        public async Task NotificationAction(uint notificationId, uint memberId, bool accepted)
        {
            // TODO: consider having the notification action return the notifications for the user
            // at the remote view level so there is just one call, possibly if an post value is set.
            await Remote.NotificationAction(notificationId, memberId, accepted);
            await UpdateNotifications();
        }

        public bool Authenticated { get; private set; }

        public bool HasVerifiedEmail { get { return _user.Emails.Any(email => email.Verified); }}

        public string FullName { get { return _user == null ? "<null user>" : _user.FullName; }}
        public string FirstName { get { return _user == null ? "<null user>" : _user.FirstName; } }
        public string LastName { get { return _user == null ? "<null user>" : _user.LastName; } }
        public string Address { get { return _user == null ? "<null user>" : _user.Address; } }
        public DateTime DateOfBirth
        {
            get
            {
                if (_user == null) return new DateTime(0);
                return string.IsNullOrEmpty(_user.DateOfBirth) ? new DateTime(0) : DateTime.Parse(_user.DateOfBirth);
            }
        }
        public string PrimaryEmail { get { return _user == null ? "" : _user.PrimaryEmail; } }

        public IEnumerable<Email> Emails
        {
            get
            {
                if (_user == null) return null;

                var list = from e in _user.Emails select new Email(e);
                return list.AsEnumerable();
            }
        }

        public async Task<IEnumerable<Connection>> Connections()
        {
            var connections = await _db.GetConnections();
            return connections == null
                ? Enumerable.Empty<Connection>()
                : connections.Select(c => new Connection(c)).AsEnumerable();
        }

        public async Task<IEnumerable<Notification>> Notifications()
        {
            var notifications = await _db.GetNotifications();
            return notifications == null
                ? Enumerable.Empty<Notification>()
                : notifications.Select(n => new Notification(n)).AsEnumerable();
        }

        public async Task<IEnumerable<Event>> Events()
        {
            var events = await _db.GetEvents();
            return events == null
                ? Enumerable.Empty<Event>()
                : events.Select(e => new Event(e)).AsEnumerable();
        }

        public static string OptionalDateTime(DateTime value, string unsetText = "")
        {
            if (value == new DateTime(0) || value == MinDateOfBirthValue)
                return unsetText;
            return value.ToString("dd MMM yyyy");
        }

        public async Task<bool> RegisterDevice()
        {
            var token = DependencyService.Get<INotificationRegisration>().Token;
            if (string.IsNullOrEmpty(token))
            {
                Debug.WriteLine("No token to register.");
                return false;
            }
            var registered = await Remote.RegisterDevice(token);
            if (registered)
            {
                await _db.SetNotificationToken(token);
            }
            return registered;
        }

        public async Task<bool> UnregisterDevice()
        {
            await _db.SetNotificationToken(null);
            var old = await _db.OldNotificationTokens();
            var result = true;
            foreach (var token in old)
            {
                var unregistered = await Remote.UnregisterDevice(token);
                if (unregistered)
                {
                    await _db.RemoveOldNotificationToken(token);
                }
                else
                {
                    result = false;
                }
            }
            return result;
        }

        public async Task Refresh()
        {
            try
            {
                Debug.WriteLine("remote site: {0}", Remote);
                MessagingCenter.Send((object)this, "action-started");
                // TODO: see if we can work out how to do the for calls and updates in parallel.
                var response = await Remote.GetUserDetails();
                await UpdateUser(response);
                await UpdateConnections();
                await UpdateEvents();
                await UpdateNotifications();
                // Let's tell Xamarin about this user.
                var traits = new Dictionary<string, string>
                    {
                        {Insights.Traits.Email, PrimaryEmail},
                        {Insights.Traits.Name, FullName}
                    };
                Insights.Identify("user-" + response.Id, traits);
                MessagingCenter.Send((object)this, "user-refreshed");
            }
            finally
            {
                MessagingCenter.Send((object)this, "action-stopped");
            }
        }
    }
}