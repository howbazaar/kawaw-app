using System;
using System.Collections.Generic;
using System.Linq;

namespace Kawaw.Models
{
    public class Event
    {
        private JSON.Event _event;
        private readonly DateTime _eventLocalStart;
        private readonly DateTime _eventLocalFinish;

        public Event(JSON.Event evt)
        {
            _event = evt;
            _eventLocalStart = DateTime.ParseExact(_event.StartDate + _event.StartTime, "yyyy-MM-ddHH:mm", null);
            _eventLocalFinish = DateTime.ParseExact(_event.FinishDate + _event.FinishTime, "yyyy-MM-ddHH:mm", null);
        }

        public string Duration { get { return _event.Duration; } }
        public uint Id { get { return _event.Id; } }
        public bool? Confirmed { get { return _event.Confirmed; } }
        public string Organisation { get { return _event.Organisation; } }
        public string Location { get { return _event.Location; } }
        public string Type { get { return _event.Type; } }
        public DateTime Start { get { return _eventLocalStart; } }
        public DateTime Finish { get { return _eventLocalFinish; } }

        public bool SameDay { get { return _event.StartDate == _event.FinishDate;  } }

        public Venue Venue { get { return new Venue(_event.Venue); } }

        public string Modified { get { return _event.Modified; } }
        public List<Link> Links
        {
            get
            {
                if (_event == null || _event.Links == null)
                {
                    return new List<Link>();
                }

                return new List<Link>(
                    from li in _event.Links select new Link(li)
                    );
            }
        }
    }
}