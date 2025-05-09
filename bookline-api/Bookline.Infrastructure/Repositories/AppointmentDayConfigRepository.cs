using Bookline.Domain.Entities;
using Bookline.Domain.Exceptions;
using Bookline.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Bookline.Infrastructure.Repositories
{
    public class AppointmentDayConfigRepository : IAppointmentDayConfigRepository
    {
        private readonly AppDbContext _appDbContext;

        public AppointmentDayConfigRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<List<AppointmentDayConfig>> GetAsync()
        {
            return await _appDbContext.AppointmentDayConfigs
                .Where(config => config.DeletedAt == null)
                .ToListAsync();
        }

        public async Task<AppointmentDayConfig> GetByIdAsync(Guid id)
        {
            return await _appDbContext.AppointmentDayConfigs
                .FirstOrDefaultAsync(config => config.Id == id && config.DeletedAt == null);
        }

        public async Task<AppointmentDayConfig> GetByDateAsync(DateTime date)
        {
            return await _appDbContext.AppointmentDayConfigs
                .FirstOrDefaultAsync(config => config.Date == date && config.DeletedAt == null);
        }

        public async Task<AppointmentDayConfig> AddAsync(AppointmentDayConfig appointmentDayConfigRequest)
        {
            var transaction = await _appDbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            try
            {
                DateTime targetDate = appointmentDayConfigRequest.Date;

                AppointmentDayConfig existingConfig = await _appDbContext.AppointmentDayConfigs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(config => config.Date == targetDate);

                if (existingConfig != null)
                {
                    throw new BooklineBadRequestException($"Config for {targetDate:yyyy-MM-dd} already exists.");
                }

                int existingAppointmentsCount = await _appDbContext.Appointments
                    .Where(appointment => appointment.Date == targetDate)
                    .CountAsync();

                if (existingAppointmentsCount > 0 && appointmentDayConfigRequest.MaxAppointments < existingAppointmentsCount)
                {
                    throw new BooklineBadRequestException($"Cannot set max appointments to {appointmentDayConfigRequest.MaxAppointments} because there are already {existingAppointmentsCount} appointments on {targetDate:yyyy-MM-dd}.");
                }

                appointmentDayConfigRequest.Date = targetDate;
                appointmentDayConfigRequest.CreatedAt = DateTime.UtcNow;

                _appDbContext.AppointmentDayConfigs.Add(appointmentDayConfigRequest);

                await _appDbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return appointmentDayConfigRequest;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                throw;
            }
        }

        public async Task<AppointmentDayConfig> FullUpdateAsync(AppointmentDayConfig appointmentDayConfigRequest)
        {
            AppointmentDayConfig existingConfig = await _appDbContext.AppointmentDayConfigs
                .FirstOrDefaultAsync(config => config.Id == appointmentDayConfigRequest.Id);

            if (existingConfig == null)
            {
                throw new BooklineBadRequestException("Appointment day config doesn't exist.");
            }

            existingConfig.Date = appointmentDayConfigRequest.Date;
            existingConfig.IsHoliday = appointmentDayConfigRequest.IsHoliday;
            existingConfig.MaxAppointments = appointmentDayConfigRequest.MaxAppointments;

            await _appDbContext.SaveChangesAsync();

            return existingConfig;
        }

        public async Task<AppointmentDayConfig> PartialUpdateAsync(AppointmentDayConfig appointmentDayConfigRequest)
        {
            var transaction = await _appDbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            try
            {
                AppointmentDayConfig existingConfig = await _appDbContext.AppointmentDayConfigs
                    .Where(config => config.Id == appointmentDayConfigRequest.Id)
                    .TagWith("FOR UPDATE")
                    .FirstOrDefaultAsync();

                if (existingConfig == null)
                {
                    throw new BooklineBadRequestException("Appointment day config doesn't exist.");
                }

                int existingAppointments = await _appDbContext.Appointments
                    .Where(appointment => appointment.Date == existingConfig.Date)
                    .CountAsync();

                if (existingAppointments > 0 && appointmentDayConfigRequest.IsHoliday != existingConfig.IsHoliday)
                {
                    throw new BooklineBadRequestException($"Cannot update holiday status for {existingConfig.Date:yyyy-MM-dd} because there are already {existingAppointments} appointments booked.");
                }

                if (existingAppointments > 0 &&
                    appointmentDayConfigRequest.MaxAppointments != 0 &&
                    appointmentDayConfigRequest.MaxAppointments < existingAppointments)
                {
                    throw new BooklineBadRequestException($"Cannot reduce max appointments below existing booked appointments ({existingAppointments}).");
                }

                if (appointmentDayConfigRequest.Date != default && appointmentDayConfigRequest.Date != existingConfig.Date)
                {
                    existingConfig.Date = appointmentDayConfigRequest.Date;
                }

                if (appointmentDayConfigRequest.IsHoliday != existingConfig.IsHoliday)
                {
                    existingConfig.IsHoliday = appointmentDayConfigRequest.IsHoliday;
                }

                if (appointmentDayConfigRequest.MaxAppointments != 0 &&
                    appointmentDayConfigRequest.MaxAppointments != existingConfig.MaxAppointments)
                {
                    existingConfig.MaxAppointments = appointmentDayConfigRequest.MaxAppointments;
                }

                existingConfig.UpdatedAt = DateTime.UtcNow;

                await _appDbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return existingConfig;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                throw;
            }
        }

        public async Task HardDeleteAsync(AppointmentDayConfig appointmentDayConfigRequest)
        {
            _appDbContext.AppointmentDayConfigs.Remove(appointmentDayConfigRequest);

            await _appDbContext.SaveChangesAsync();
        }

        public async Task<bool> IsDateUsedByOtherConfigAsync(Guid currentId, DateTime date)
        {
            return await _appDbContext.AppointmentDayConfigs
                .AnyAsync(config => config.Date == date && config.Id != currentId && config.DeletedAt == null);
        }
    }
}