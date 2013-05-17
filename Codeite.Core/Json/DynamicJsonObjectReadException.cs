using System;

namespace Codeite.Core.Json
{
    public class DynamicJsonObjectReadException : Exception
    {
        public DynamicJsonObjectReadException()
        {
        }

        public DynamicJsonObjectReadException(string message) : base(message)
        {
        }

        public DynamicJsonObjectReadException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}