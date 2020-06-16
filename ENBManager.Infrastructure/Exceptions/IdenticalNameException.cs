using System;

namespace ENBManager.Infrastructure.Exceptions
{
    public class IdenticalNameException : Exception
    {
        public IdenticalNameException(string message)
            : base(message) { }
    }
}
