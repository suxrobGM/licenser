using System;
using System.Runtime.Serialization;

namespace Sms.Licensing.Shared.Exceptions
{
    [Serializable]
    public class ServerErrorException : Exception
    {
        public ServerErrorException()
        {
        }

        public ServerErrorException(string message) : base(message)
        {
        }

        public ServerErrorException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ServerErrorException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}