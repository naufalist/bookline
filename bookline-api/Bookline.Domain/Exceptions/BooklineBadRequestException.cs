using System;

namespace Bookline.Domain.Exceptions
{
    public class BooklineBadRequestException : Exception
    {
        public BooklineBadRequestException() { }

        public BooklineBadRequestException(string message)
            : base(message) { }

        public BooklineBadRequestException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}