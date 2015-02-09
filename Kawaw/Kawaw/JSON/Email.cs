using System.Runtime.Serialization;

namespace Kawaw.JSON
{
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