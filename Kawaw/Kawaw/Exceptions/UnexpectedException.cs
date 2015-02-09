using System;

namespace Kawaw.Exceptions
{
    public class UnexpectedException : Exception
    {
        private readonly string _context;
        public UnexpectedException(string context = "")
            : base("an unexpected error occurred")
        {
            _context = context;
        }

        public string Context
        {
            get { return _context; }
        }
    }
}