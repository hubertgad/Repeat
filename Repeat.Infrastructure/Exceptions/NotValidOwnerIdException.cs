using System;

namespace Repeat.Infrastucture.Exceptions
{
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
    }
}