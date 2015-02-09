using System.Runtime.Serialization;

namespace Kawaw.JSON
{
    [DataContract]
    public class Link
    {
        [DataMember(Name = "id")]
        public uint Id { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "organisation")]
        public Reference Organisation { get; set; }

        [DataMember(Name = "activity")]
        public Reference Activity { get; set; }

        [DataMember(Name = "team")]
        public Reference Team { get; set; }

        [DataMember(Name = "members")]
        public string[] Members { get; set; }
        //data blob to come later
        
    }
}