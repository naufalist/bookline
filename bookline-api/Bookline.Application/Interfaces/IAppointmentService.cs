using Bookline.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bookline.Application.Interfaces
{
    public interface IAppointmentService
    {
        Task<List<AppointmentResponseDto>> GetAppointments();
        Task<List<AppointmentResponseDto>> GetAppointmentsByDateAsync(string date);
        Task<AppointmentResponseDto> PostAppointmentAsync(AppointmentRequestDto request);
    }
}