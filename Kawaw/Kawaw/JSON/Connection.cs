using System.Runtime.Serialization;

namespace Kawaw.JSON
{
    [DataContract]
    public class Connection
    {
        [DataMember(Name = "id")]
        public uint Id { get; set; }

        [DataMember(Name = "email")]
        public string Email{ get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "_type")]
        public string TypeValue { get; set; }

        [DataMember(Name = "accepted")]
        public bool? Accepted { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "org")]
        public string Organisation { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }


        protected bool Equals(Connection other)
        {
            return (
                Id == other.Id && 
                string.Equals(Email, other.Email) && 
                string.Equals(Type, other.Type) && 
                Accepted == other.Accepted && 
                string.Equals(Name, other.Name) && 
                string.Equals(Organisation, other.Organisation));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Connection)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)Id;
                hashCode = (hashCode * 397) ^ (Email != null ? Email.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Type != null ? Type.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Accepted.GetHashCode();
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Organisation != null ? Organisation.GetHashCode() : 0);
                return hashCode;
            }
        }

    }
}