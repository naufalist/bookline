using Bookline.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bookline.Infrastructure.Repositories
{
    public interface IAppointmentDayConfigRepository
    {
        Task<List<AppointmentDayConfig>> GetAsync();
        Task<AppointmentDayConfig> GetByIdAsync(Guid id);
        Task<AppointmentDayConfig> GetByDateAsync(DateTime date);
        Task<AppointmentDayConfig> AddAsync(AppointmentDayConfig appointmentDayConfig);
        Task<AppointmentDayConfig> FullUpdateAsync(AppointmentDayConfig appointmentDayConfig);
        Task<AppointmentDayConfig> PartialUpdateAsync(AppointmentDayConfig appointmentDayConfig);
        Task HardDeleteAsync(AppointmentDayConfig appointmentDayConfig);
        Task<bool> IsDateUsedByOtherConfigAsync(Guid currentId, DateTime date);
    }
}