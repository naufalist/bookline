using System;

namespace Bookline.Domain.Exceptions
{
    public class BooklineNotFoundException : Exception
    {
        public BooklineNotFoundException(string message) : base(message) { }
    }
}