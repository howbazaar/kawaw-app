namespace Kawaw.Models
{
    public class NotificationResponse
    {
        private readonly JSON.NotificationResponse _response;

        public NotificationResponse(Notification notification, JSON.NotificationResponse response)
        {
            Notification = notification;
            _response = response;
        }

        public uint Id { get { return _response.Id; }}
        public string Name { get { return _response.Name; } }
        public bool? Choice { get { return _response.Choice; } set { _response.Choice = value; } }
        public Notification Notification { get; private set; }
    }
}