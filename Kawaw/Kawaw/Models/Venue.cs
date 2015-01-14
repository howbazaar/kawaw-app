namespace Kawaw.Models
{
    public class Venue
    {
        private readonly JSON.Venue _venue;
        public Venue(JSON.Venue venue)
        {
            _venue = venue;
        }
        
        public string Address { get { return _venue == null ? "" : _venue.Address; }}
        public string Name { get { return _venue == null ? "" : _venue.Name; } }
    }
}