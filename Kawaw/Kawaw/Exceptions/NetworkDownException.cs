using System;

namespace Kawaw.Exceptions
{
    public class NetworkDownException : Exception
    {
        public NetworkDownException()
            : base("Kawaw is unreachable. No network?")
        {
        }
    }
}