using System.Runtime.Serialization;

namespace Kawaw.JSON
{
    [DataContract]
    public class Event
    {
 
        [DataMember(Name = "duration")]
        public string Duration { get; set; }

        [DataMember(Name = "id")]
        public uint Id { get; set; }

        [DataMember(Name = "confirmed")]
        public bool? Confirmed { get; set; }

        [DataMember(Name = "organisation")]
        public string Organisation { get; set; }

        [DataMember(Name = "location")]
        public string Location { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "start")]
        public string Start { get; set; }

        [DataMember(Name = "start_date")]
        public string StartDate { get; set; }

        [DataMember(Name = "start_time")]
        public string StartTime { get; set; }

        [DataMember(Name = "finish")]
        public string Finish { get; set; }

        [DataMember(Name = "finish_date")]
        public string FinishDate { get; set; }

        [DataMember(Name = "finish_time")]
        public string FinishTime { get; set; }

        [DataMember(Name = "venue")]
        public Venue Venue { get; set; }

        [DataMember(Name = "modified")]
        public string Modified { get; set; }

        [DataMember(Name = "links")]
        public Link[] Links { get; set; }



    }
}