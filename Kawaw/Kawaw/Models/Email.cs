namespace Kawaw.Models
{
    public class Email
    {
        private JSON.Email _email;
        public Email(JSON.Email email)
        {
            _email = email;
        }
        public bool Verified { get { return _email.Verified; }}
        public string Address { get { return _email.Address; } }
        public bool Primary { get { return _email.Primary; } }

        public string Description
        {
            get
            {
                var result = Verified ? "Verified" : "Unverified";
                if (Primary)
                {
                    result = "Primary, " + result;
                }
                return result;
            }
        }
    }
}