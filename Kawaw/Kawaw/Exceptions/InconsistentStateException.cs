using System;

namespace Kawaw.Exceptions
{
    public class InconsistentStateException : Exception
    {
        public InconsistentStateException()
            : base("app data is inconsistent with the website")
        {
        }
    }
}