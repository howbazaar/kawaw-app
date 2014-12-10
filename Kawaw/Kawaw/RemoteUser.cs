using System;
using System.Runtime.Serialization;

namespace Kawaw
{
    [DataContract]
    public class RemoteUser 
    {
        // NOTE: probably want to store the cookies with the user so it gets persisted correctly.
        [DataMember(Name = "user")]
        private JSON.User _user;

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
        }

        public string FullName { get { return _user.FullName; }}
        public string FirstName { get { return _user.FirstName; } }
        public string LastName { get { return _user.LastName; } }
        public string CSRFToken { get { return _user.CSRFToken; } }
        public string Address { get { return _user.Address; } }
        public DateTime DateOfBirth
        {
            get
            {
                return string.IsNullOrEmpty(_user.DateOfBirth) ? new DateTime(0) : DateTime.Parse(_user.DateOfBirth);
            }
        }
        public string PrimaryEmail { get { return _user.PrimaryEmail; } }

    }
}