using System;

namespace Kawaw.Exceptions
{
    public class SessionExpiredException : Exception
    {
        public SessionExpiredException()
            : base("session expired")
        {
        }
    }
}