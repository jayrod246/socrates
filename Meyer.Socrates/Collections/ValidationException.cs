using System;
using System.Runtime.Serialization;

namespace Meyer.Socrates.Collections
{
    public class ValidationException: Exception
    {
        private static string ERROR_MSG = "Validation on an item failed.";
        public ValidationException() : base(ERROR_MSG)
        {
        }

        public ValidationException(Exception innerException) : base(ERROR_MSG, innerException)
        {

        }

        public ValidationException(string message) : base(message ?? ERROR_MSG)
        {
        }

        public ValidationException(string message, Exception innerException) : base(message ?? ERROR_MSG, innerException)
        {
        }

        protected ValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
