namespace Bookline.Domain.DTOs
{
    public class GlobalResponse
    {
        public GlobalResponse(string message = null, object data = null)
        {
            Message = message;
            Data = data;
        }

        public string Message { get; set; }
        public object Data { get; set; }
    }
}