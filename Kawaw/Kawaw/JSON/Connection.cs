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
}