using System;
using System.Collections.Generic;
using System.Linq;

namespace Kawaw.Models
{
    public class Notification
    {
        private readonly JSON.Notification _notification;
        private readonly DateTime _closingDate;

        public Notification(JSON.Notification note)
        {
            _notification = note;
            _closingDate = DateTime.ParseExact(_notification.ClosingDate, "yyyy-MM-dd", null);
        }

        public uint Id { get { return _notification.Id; } }
        public string Type { get { return _notification.Type; } }
        public DateTime ClosingDate { get { return _closingDate; }}
        public string Note { get { return _notification.Note; } }
        public bool Pending { get { return _notification.Pending; } set { _notification.Pending = value; } }
        public string Organisation { get { return _notification.Organisation; } }
        // activity, session and description are optional.
        public string Activity { get { return _notification.Activity; } }
        public string Session { get { return _notification.Session; } }
        public string Description { get { return _notification.Description; } }

        public List<NotificationResponse> Responses
        {
            get
            {
                if (_notification == null || _notification.Responses == null)
                {
                    return new List<NotificationResponse>();
                }

                return new List<NotificationResponse>(
                    from response in _notification.Responses select new NotificationResponse(this, response)
                    );
            }
        }
    }
}