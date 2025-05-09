namespace Bookline.Domain.DTOs
{
    public class AppointmentResponseDto
    {
        public string Date { get; set; }
        public string Token { get; set; }
        public CustomerResponseDto Customer { get; set; }
    }
}
