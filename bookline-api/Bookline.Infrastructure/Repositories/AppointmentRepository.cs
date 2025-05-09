using Bookline.Domain.Entities;
using Bookline.Domain.Exceptions;
using Bookline.Domain.ValueObjects;
using Bookline.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Bookline.Infrastructure.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly AppDbContext _appDbContext;
        private const int MaxAvailableDaysToCheck = 30;
        private const int MaxAppointmentsPerDay = 100;

        public AppointmentRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<int> CountByDateAsync(DateTime requestDate)
        {
            return await _appDbContext.Appointments
                .CountAsync(appointment => appointment.Date.Date == requestDate.Date);
        }

        public async Task<List<Appointment>> GetAsync()
        {
            return await _appDbContext.Appointments
                .Include(appointment => appointment.Customer)
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetByDateAsync(DateTime requestDate)
        {
            return await _appDbContext.Appointments
                .Include(appointment => appointment.Customer)
                .Where(appointment => appointment.Date.Date == requestDate.Date)
                .ToListAsync();
        }

        public async Task<Appointment> AddAsync(Appointment appointmentRequest)
        {
            var transaction = await _appDbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            try
            {
                await IsCustomerAlreadyHasAppointmentInDay(appointmentRequest.Customer.Id, appointmentRequest.Date);

                DateTime? selectedAvailableDate = null;
                int appointmentCount = default;

                if (appointmentRequest.Date.DayOfWeek == DayOfWeek.Saturday || appointmentRequest.Date.DayOfWeek == DayOfWeek.Sunday)
                {
                    AppointmentAvailabilityDay availableDay = await EnsureNextAvailableAppointmentDay(appointmentRequest.Date);
                    await IsCustomerAlreadyHasAppointmentInDay(appointmentRequest.Customer.Id, availableDay.Date);
                    selectedAvailableDate = availableDay.Date;
                    appointmentCount = availableDay.Count;
                } else
                {
                    AppointmentDayConfig dayConfig = await _appDbContext.AppointmentDayConfigs
                    .FirstOrDefaultAsync(config => config.Date == appointmentRequest.Date);

                    if (dayConfig != null)
                    {
                        if (dayConfig.IsHoliday)
                        {
                            AppointmentAvailabilityDay availableDay = await EnsureNextAvailableAppointmentDay(appointmentRequest.Date);
                            await IsCustomerAlreadyHasAppointmentInDay(appointmentRequest.Customer.Id, availableDay.Date);
                            selectedAvailableDate = availableDay.Date;
                            appointmentCount = availableDay.Count;
                        }
                        else
                        {
                            appointmentCount = await _appDbContext.Appointments
                                .CountAsync(appointment => appointment.Date == appointmentRequest.Date);

                            if (appointmentCount < dayConfig.MaxAppointments)
                            {
                                selectedAvailableDate = appointmentRequest.Date;
                            }
                            else
                            {
                                AppointmentAvailabilityDay availableDay = await EnsureNextAvailableAppointmentDay(appointmentRequest.Date);
                                await IsCustomerAlreadyHasAppointmentInDay(appointmentRequest.Customer.Id, availableDay.Date);
                                selectedAvailableDate = availableDay.Date;
                                appointmentCount = availableDay.Count;
                            }
                        }
                    }
                    else
                    {
                        appointmentCount = await _appDbContext.Appointments
                            .Where(appointment => appointment.Date == appointmentRequest.Date)
                            .CountAsync();

                        if (appointmentCount < MaxAppointmentsPerDay)
                        {
                            selectedAvailableDate = appointmentRequest.Date;
                        }
                        else
                        {
                            AppointmentAvailabilityDay availableDay = await EnsureNextAvailableAppointmentDay(appointmentRequest.Date);
                            await IsCustomerAlreadyHasAppointmentInDay(appointmentRequest.Customer.Id, availableDay.Date);
                            selectedAvailableDate = availableDay.Date;
                            appointmentCount = availableDay.Count;
                        }
                    }
                }

                if (selectedAvailableDate == null || !selectedAvailableDate.HasValue)
                {
                    throw new BooklineBadRequestException("No appointment date is available within the next 30-day period.");
                }

                appointmentRequest.Date = selectedAvailableDate.Value;
                appointmentRequest.Token = Token.Generate(appointmentCount + 1);
                appointmentRequest.CreatedAt = DateTime.UtcNow;

                _appDbContext.Appointments.Add(appointmentRequest);

                await _appDbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return appointmentRequest;
            }
            catch (BooklineBadRequestException ex)
            {
                Console.WriteLine(ex.Message);

                await transaction.RollbackAsync();

                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                await transaction.RollbackAsync();

                throw;
            }
        }

        private async Task<AppointmentAvailabilityDay> FindNextAvailableAppointmentDay(DateTime nextPotentialDate)
        {
            int daysChecked = 0;

            nextPotentialDate = nextPotentialDate.AddDays(1); // move from current to next date

            while (daysChecked < MaxAvailableDaysToCheck)
            {
                if (nextPotentialDate.DayOfWeek == DayOfWeek.Saturday || nextPotentialDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    nextPotentialDate = nextPotentialDate.AddDays(1);
                    daysChecked++;
                    continue;
                }

                AppointmentDayConfig dayConfig = await _appDbContext.AppointmentDayConfigs
                    .FirstOrDefaultAsync(config => config.Date == nextPotentialDate);

                int appointmentCount = await _appDbContext.Appointments
                    .CountAsync(appointment => appointment.Date == nextPotentialDate);

                if (dayConfig != null)
                {
                    if (dayConfig.IsHoliday)
                    {
                        nextPotentialDate = nextPotentialDate.AddDays(1);
                        daysChecked++;
                        continue;
                    }

                    if (appointmentCount < dayConfig.MaxAppointments)
                    {
                        return new AppointmentAvailabilityDay(nextPotentialDate, appointmentCount);
                    }
                }
                else
                {
                    if (appointmentCount < MaxAppointmentsPerDay)
                    {
                        return new AppointmentAvailabilityDay(nextPotentialDate, appointmentCount);
                    }
                }

                nextPotentialDate = nextPotentialDate.AddDays(1);
                daysChecked++;
            }

            return null;
        }

        private async Task<AppointmentAvailabilityDay> EnsureNextAvailableAppointmentDay(DateTime nextPotentialDate)
        {
            AppointmentAvailabilityDay availableDay = await FindNextAvailableAppointmentDay(nextPotentialDate);

            if (availableDay == null)
            {
                throw new BooklineBadRequestException("No appointment date is available within the next 30 day period.");
            }

            return availableDay;
        }

        private async Task IsCustomerAlreadyHasAppointmentInDay(Guid customerId, DateTime appointmentDate)
        {
            int customerAppointmentCount = await _appDbContext.Appointments
                .Where(appointment => appointment.CustomerId == customerId && appointment.Date == appointmentDate)
                .CountAsync();

            if (customerAppointmentCount > 0)
            {
                DateTime nextAvailableDate = appointmentDate.AddDays(1);

                while (nextAvailableDate.DayOfWeek == DayOfWeek.Saturday || nextAvailableDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    nextAvailableDate = nextAvailableDate.AddDays(1);
                }

                throw new BooklineBadRequestException($"You already have an appointment on this date: {appointmentDate:yyyy-MM-dd}. Try to book next available weekday: {nextAvailableDate:yyyy-MM-dd}");
            }
        }
    }
}