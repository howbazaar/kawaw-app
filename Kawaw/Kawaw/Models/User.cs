using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using PushNotification.Plugin;
using PushNotification.Plugin.Abstractions;
using Xamarin;
using Xamarin.Forms;

namespace Kawaw.Models
{
    [DataContract]
    public class User
    {
        static public readonly DateTime MinDateOfBirthValue = new DateTime(1900, 1, 1);

        [DataMember(Name = "user")]
        private JSON.User _user;

        [DataMember(Name = "connections")]
        private JSON.Connection[] _connections;

        [DataMember(Name = "events")]
        private JSON.Event[] _events;

        [DataMember(Name = "notifications")]
        private JSON.Notification[] _notifications;

        [DataMember(Name = "device-token")]
        private string _token;

        public IRemoteSite Remote { get; set; }

        public void UpdateUser(JSON.User user)
        {
            _user = user;
            MessagingCenter.Send<object>(this, "user-updated");
        }

        public void UpdateConnections(JSON.Connection[] connections)
        {
            _connections = connections;
            MessagingCenter.Send<object>(this, "connections-updated");
        }

        public void UpdateEvents(JSON.Event[] events)
        {
            _events = events;
            MessagingCenter.Send<object>(this, "events-updated");
        }

        public void UpdateNotifications(JSON.Notification[] notifications)
        {
            _notifications = notifications;
            MessagingCenter.Send<object>(this, "notifications-updated");
        }

        public async Task ConnectionAction(Connection connection, bool accept)
        {
            var result = await Remote.ConnectionAction(connection.Id, accept);
            // Update our connections.
            foreach (var conn in _connections.Where(conn => conn.Id == result.Id))
            {
                conn.Accepted = result.Accepted;
            }
            MessagingCenter.Send<object>(this, "connections-updated");
        }

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
        public string PrimaryEmail { get { return _user.PrimaryEmail; } }

        public IEnumerable<Email> Emails
        {
            get
            {
                var list = from e in _user.Emails select new Email(e);
                return list.AsEnumerable();
            }
        }

        public IEnumerable<Connection> Connections
        {
            get
            {
                if (_connections == null)
                {
                    return Enumerable.Empty<Connection>();
                }
                //var otherlist = _connections.Select(c => new Connection(c));
                var list = from conn in _connections select new Connection(conn); 
                return list.AsEnumerable();
            }
        }

        public IEnumerable<Notification> Notifications
        {
            get
            {
                return _notifications == null
                    ? Enumerable.Empty<Notification>()
                    : _notifications.Select(n => new Notification(n)).AsEnumerable();
            }
        }

        public IEnumerable<Event> Events
        {
            get
            {
                if (_events == null)
                {
                    return Enumerable.Empty<Event>();
                }
                //var otherlist = _connections.Select(c => new Connection(c));
                var list = from ev in _events select new Event(ev);
                return list.AsEnumerable();
            }
        }

        public static string OptionalDateTime(DateTime value, string unsetText = "")
        {
            if (value == new DateTime(0) || value == MinDateOfBirthValue)
                return unsetText;
            return value.ToString("dd MMM yyyy");
        }

        public Task<bool> RegisterDevice()
        {
            var token = CrossPushNotification.Current.Token;
            if (string.IsNullOrEmpty(token))
            {
                Debug.WriteLine("No token to register.");
                return Task.FromResult(false);
            }
            _token = token;
            return Remote.RegisterDevice(token);
        }

        public Task<bool> UnregisterDevice()
        {
            if (string.IsNullOrEmpty(_token))
            {
                Debug.WriteLine("No token to unregister.");
                return Task.FromResult(false);
            }
            return Remote.UnregisterDevice(_token);
        }

        public async Task Refresh()
        {
            try
            {
                Debug.WriteLine("remote site: {0}", Remote);
                MessagingCenter.Send((object)this, "action-started");
                var response = await Remote.GetUserDetails();
                UpdateUser(response);
                var connections = await Remote.GetConnections();
                UpdateConnections(connections);
                var events = await Remote.GetEvents();
                UpdateEvents(events);
                var notifications = await Remote.GetNotifications();
                UpdateNotifications(notifications);
                // Let's tell Xamarin about this user.
                var traits = new Dictionary<string, string>
                    {
                        {Insights.Traits.Email, PrimaryEmail},
                        {Insights.Traits.Name, FullName}
                    };
                Insights.Identify(PrimaryEmail, traits);
                MessagingCenter.Send((object)this, "user-refreshed");
            }
            finally
            {
                MessagingCenter.Send((object)this, "action-stopped");
            }
        }
    }
}