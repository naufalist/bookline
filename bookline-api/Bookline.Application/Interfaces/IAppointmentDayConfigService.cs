using Bookline.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bookline.Application.Interfaces
{
    public interface IAppointmentDayConfigService
    {
        Task<List<AppointmentDayConfigResponseDto>> GetAppointmentDayConfigsAsync();
        Task<AppointmentDayConfigResponseDto> GetAppointmentDayConfigByIdAsync(string id);
        Task<AppointmentDayConfigResponseDto> GetAppointmentDayConfigByDateAsync(string date);
        Task<AppointmentDayConfigResponseDto> PostAppointmentDayConfigAsync(AppointmentDayConfigRequestDto request);
        Task<AppointmentDayConfigResponseDto> PartialUpdateAppointmentDayConfigAsync(string id, AppointmentDayConfigRequestDto request);
        Task HardDeleteAppointmentDayConfigAsync(string id);
    }
}