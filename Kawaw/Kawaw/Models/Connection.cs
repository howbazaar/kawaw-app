namespace Kawaw.Models
{
    public class Connection
    {
        private readonly JSON.Connection _connection;

        public Connection(JSON.Connection connection)
        {
            _connection = connection;
        }

        public uint Id { get { return _connection.Id; }}
        public string Email { get { return _connection.Email; } }
        public string Type { get { return _connection.Type; } }
        public string TypeValue { get { return _connection.TypeValue; } }

        // break up the optional bool into two bools
        public bool Pending { get { return _connection.Accepted == null; } }
        // pending are considered not accepted
        public bool Accepted { get { return _connection.Accepted == true; } }
        public string Status { get
        {
            if (Pending) return "Pending";
            return Accepted ? "Accepted" : "Rejected";
        } }
        public string Name { get { return _connection.Name; } }
        public string Organisation { get { return _connection.Organisation; } }
    }
}