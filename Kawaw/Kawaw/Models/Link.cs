namespace Kawaw.Models
{
    public class Link
    {
        private JSON.Link _link;
        public Link(JSON.Link link)
        {
            _link= link;
        }

        public string Type { get { return _link == null ? "" : _link.Type; } }

        public Reference Organisation
        {
            get
            {
                if (_link == null || _link.Organisation == null)
                    return null;
                return new Reference(_link.Organisation);
            }
        }
        public Reference Activity
        {
            get
            {
                if (_link == null || _link.Activity== null)
                    return null;
                return new Reference(_link.Activity);
            }
        }
        public Reference Team
        {
            get
            {
                if (_link == null || _link.Team == null)
                    return null;
                return new Reference(_link.Team);
            }
        }

        public string Members
        {
            get
            {
                if (_link.Members == null)
                {
                    return "";
                }
                return string.Join(", ",_link.Members);
            }
        }
    }
}