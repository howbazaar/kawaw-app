using System.Runtime.Serialization;

namespace Kawaw.JSON
{
    [DataContract]
    public class Venue
    {
        [DataMember(Name = "organisation_id")]
        public uint? OrganisationId { get; set; }

        [DataMember(Name = "address")]
        public string Address { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "id")]
        public uint Id { get; set; }
    }
}