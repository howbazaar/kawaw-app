using System;
using System.Net;

namespace Kawaw.Exceptions
{
    public class UnexpectedStatusException : Exception
    {
        public UnexpectedStatusException(HttpStatusCode status)
            : base("unexpected status code: " + status)
        {
        }
    }
}