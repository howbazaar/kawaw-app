using System.Runtime.Serialization;

namespace Kawaw.JSON
{
    [DataContract]
    public class NotificationResponse
    {
        [DataMember(Name = "id")]
        public uint Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "choice")]
        public bool? Choice { get; set; }
    }
}