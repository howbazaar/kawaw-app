using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Microsoft.VisualBasic.CompilerServices;
using Xamarin.Forms;

namespace Kawaw
{
    [DataContract]
    public class RemoteUser
    {
        static public readonly DateTime MinDateOfBirthValue = new DateTime(1900, 1, 1);

        // NOTE: probably want to store the cookies with the user so it gets persisted correctly.
        [DataMember(Name = "user")]
        private JSON.User _user;

        [DataMember(Name = "csrftoken")]
        public string CSRFToken { get; set; }

        [DataMember(Name = "sessionid")]
        public string SessionId { get; set; }

        [DataMember(Name = "connections")]
        private JSON.Connection[] _connections;

        public RemoteUser()
        {
        }

        public RemoteUser(JSON.User user)
        {
            _user = user;
        }

        public void UpdateUser(JSON.User user)
        {
            _user = user;
            MessagingCenter.Send<object>(this, "user-updated");
        }

        public void UpdateConnections(JSON.Connection[] connections)
        {
            _connections = connections;
            if (connections == null)
                Debug.WriteLine("connections is null");
            else
                Debug.WriteLine("connections has {0} items", connections.Length);
            MessagingCenter.Send<object>(this, "connections-updated");
        }

        public string FullName { get { return _user.FullName; }}
        public string FirstName { get { return _user.FirstName; } }
        public string LastName { get { return _user.LastName; } }
        public string Address { get { return _user.Address; } }
        public DateTime DateOfBirth
        {
            get
            {
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

        public static string OptionalDateTime(DateTime value)
        {
            if (value == new DateTime(0) || value == MinDateOfBirthValue)
                return "not set";
            return value.ToString("dd MMM yyyy");
        }

        public async void Refresh(IRemoteSite remote)
        {
            Debug.WriteLine("Refreshing user {0}", FullName);
            try
            {
                var response = await remote.GetUserDetails();
                UpdateUser(response);
                var connections = await remote.GetConnections();
                UpdateConnections(connections);
            }
            catch (Exception e)
            {
                Debug.WriteLine("TODO: handle stale session, site down.");
            }   
        }
    }

    public class Email
    {
        private JSON.Email _email;
        public Email(JSON.Email email)
        {
            _email = email;
        }
        public bool Verified { get { return _email.Verified; }}
        public string Address { get { return _email.Address; } }
        public bool Primary { get { return _email.Primary; } }

        public string Description
        {
            get
            {
                var result = Verified ? "Verified" : "Unverified";
                if (Primary)
                {
                    result = "Primary, " + result;
                }
                return result;
            }
        }
    }

    public class Connection
    {
        private JSON.Connection _connection;

        public Connection(JSON.Connection connection)
        {
            _connection = connection;
        }

        public uint Id { get { return _connection.Id; }}
        public string Email { get { return _connection.Email; } }
        public string Type { get { return _connection.Type; } }
        public string TypeValue { get { return _connection.TypeValue; } }

        // break up the optional bool into two bools
        public bool Pending { get { return _connection.Accepted == null; } }
        // pending are considered not accepted
        public bool Accepted { get { return _connection.Accepted == true; } }
        public string Status { get
        {
            if (Pending) return "pending";
            return Accepted ? "accepted" : "rejected";
        } }
        public string Name { get { return _connection.Name; } }
        public string Organisation { get { return _connection.Organisation; } }
    }

    class OptionalDateConverter : IValueConverter
    {
        // from the view-model to the view
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is DateTime ? RemoteUser.OptionalDateTime((DateTime)value) : "unexpected type";
        }

        // from the view to the view-model
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}