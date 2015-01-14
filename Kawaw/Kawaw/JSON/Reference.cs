using System.Runtime.Serialization;

namespace Kawaw.JSON
{
    [DataContract]
    public class Reference
    {
        [DataMember(Name = "id")]
        public uint Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}