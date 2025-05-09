namespace Bookline.Domain.DTOs
{
    public class AppointmentDayConfigResponseDto
    {
        public string Id { get; set; }
        public string Date { get; set; }
        public bool IsHoliday { get; set; }
        public int MaxAppointments { get; set; }
    }
}