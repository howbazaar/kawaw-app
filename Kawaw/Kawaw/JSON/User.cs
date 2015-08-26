using System.Runtime.Serialization;

namespace Kawaw.JSON
{
    [DataContract]
    public class User
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "date_of_birth")]
        public string DateOfBirth { get; set; }

        [DataMember(Name = "first_name")]
        public string FirstName { get; set; }

        [DataMember(Name = "last_name")]
        public string LastName { get; set; }

        [DataMember(Name = "full_name")]
        public string FullName { get; set; }

        [DataMember(Name = "address")]
        public string Address { get; set; }

        // There is a social list as well that shows which google/facebook connections the user has set up.

        [DataMember(Name = "email")]
        public string PrimaryEmail { get; set; }

        [DataMember(Name = "emails")]
        public Email[] Emails { get; set; }
    }
}