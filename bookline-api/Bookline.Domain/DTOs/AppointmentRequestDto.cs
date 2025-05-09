using Bookline.Shared.Validations;
using System.ComponentModel.DataAnnotations;

namespace Bookline.Domain.DTOs
{
    public class AppointmentRequestDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        [ValidDateFormat("yyyy-MM-dd")]
        public string Date { get; set; }
    }
}