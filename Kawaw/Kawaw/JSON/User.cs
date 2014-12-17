using System;
using System.Runtime.Serialization;

namespace Kawaw.JSON
{
    [DataContract]
    public class Connection
    {
        [DataMember(Name = "id")]
        public uint Id { get; set; }

        [DataMember(Name = "email")]
        public string Email{ get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "_type")]
        public string TypeValue { get; set; }

        [DataMember(Name = "accepted")]
        public bool? Accepted { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "org")]
        public string Organisation { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }
    }

    [DataContract]
    public class User
    {
        [DataMember(Name = "date_of_birth")]
        public string DateOfBirth { get; set; }

        [DataMember(Name = "first_name")]
        public string FirstName { get; set; }

        [DataMember(Name = "last_name")]
        public string LastName { get; set; }

        [DataMember(Name = "token")]
        public string CSRFToken { get; set; }

        [DataMember(Name = "full_name")]
        public string FullName { get; set; }

        [DataMember(Name = "address")]
        public string Address { get; set; }

        // There is a social thing, but need to remember what it does.
        [DataMember(Name = "email")]
        public string PrimaryEmail { get; set; }

        [DataMember(Name = "emails")]
        public Email[] Emails { get; set; }
    }

    [DataContract]
    public class Email
    {
        [DataMember(Name = "verified")]
        public bool Verified { get; set; }

        [DataMember(Name = "email")]
        public string Address { get; set; }

        [DataMember(Name = "primary")]
        public bool Primary { get; set; }
    }

}