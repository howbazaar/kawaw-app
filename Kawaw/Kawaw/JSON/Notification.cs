using System.Runtime.Serialization;

namespace Kawaw.JSON
{
    [DataContract]
    public class Notification
    {
        [DataMember(Name = "id")]
        public uint Id { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "closing_date")]
        public string ClosingDate { get; set; }

        [DataMember(Name = "note")]
        public string Note { get; set; }

        [DataMember(Name = "pending")]
        public bool Pending { get; set; }

        [DataMember(Name = "organisation")]
        public string Organisation { get; set; }

        [DataMember(Name = "responses")]
        public NotificationResponse[] Responses { get; set; }

        // activity, session and description are optional.
        [DataMember(Name = "activity")]
        public string Activity { get; set; }

        [DataMember(Name = "session")]
        public string Session { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }
    }
}