namespace Kawaw.Models
{
    public class Reference
    {
        private readonly JSON.Reference _reference;

        public Reference(JSON.Reference reference)
        {
            _reference = reference;
        }
        public string Name { get { return _reference == null ? "" : _reference.Name; }}
    }
}