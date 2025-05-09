using Bookline.Shared.Validations;
using System.ComponentModel.DataAnnotations;

namespace Bookline.Domain.DTOs
{
    public class AppointmentDayConfigRequestDto
    {
        [Required(ErrorMessage = "Date is required.")]
        [ValidDateFormat("yyyy-MM-dd")]
        public string Date { get; set; }

        [System.ComponentModel.DefaultValue(false)]
        public bool IsHoliday { get; set; }

        [Required(ErrorMessage = "Max Appointments is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Minimum value of max appointments must be at least 1 (one).")]
        [System.ComponentModel.DefaultValue(1)]
        public int MaxAppointments { get; set; }
    }
}