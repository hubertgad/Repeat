using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Repeat.Infrastucture.Exceptions
{
    [Serializable]
    public class NotValidOwnerIdException : Exception
    {
        public NotValidOwnerIdException()
        {
        }

        public NotValidOwnerIdException(string message) : base(message)
        {
        }

        public NotValidOwnerIdException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NotValidOwnerIdException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}