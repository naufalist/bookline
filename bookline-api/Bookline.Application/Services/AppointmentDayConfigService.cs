using Bookline.Application.Interfaces;
using Bookline.Domain.DTOs;
using Bookline.Domain.Entities;
using Bookline.Domain.Exceptions;
using Bookline.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Bookline.Application.Services
{
    public class AppointmentDayConfigService : IAppointmentDayConfigService
    {
        private readonly IAppointmentDayConfigRepository _appointmentDayConfigRepository;

        public AppointmentDayConfigService(IAppointmentDayConfigRepository appointmentDayConfigRepository)
        {
            _appointmentDayConfigRepository = appointmentDayConfigRepository;
        }

        public async Task<List<AppointmentDayConfigResponseDto>> GetAppointmentDayConfigsAsync()
        {
            List<AppointmentDayConfig> configs = await _appointmentDayConfigRepository.GetAsync();

            return configs
                .Select(config => new AppointmentDayConfigResponseDto
                {
                    Id = config.Id.ToString(),
                    Date = config.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    IsHoliday = config.IsHoliday,
                    MaxAppointments = config.MaxAppointments
                })
                .ToList();
        }

        public async Task<AppointmentDayConfigResponseDto> GetAppointmentDayConfigByIdAsync(string id)
        {
            if (!Guid.TryParse(id, out Guid parsedGuid))
            {
                throw new BooklineBadRequestException("Invalid ID format. Expected a valid GUID.");
            }

            AppointmentDayConfig config = await _appointmentDayConfigRepository.GetByIdAsync(parsedGuid);

            return new AppointmentDayConfigResponseDto()
            {
                Date = config.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                IsHoliday = config.IsHoliday,
                MaxAppointments = config.MaxAppointments
            };
        }

        public async Task<AppointmentDayConfigResponseDto> GetAppointmentDayConfigByDateAsync(string date)
        {
            if (!DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                throw new BooklineBadRequestException("Invalid date format. Expected format is yyyy-MM-dd.");
            }

            AppointmentDayConfig existingConfig = await _appointmentDayConfigRepository.GetByDateAsync(parsedDate);

            if (existingConfig == null || existingConfig.DeletedAt != null)
            {
                throw new BooklineNotFoundException("Appointment day config not found or has been deleted.");
            }

            return new AppointmentDayConfigResponseDto()
            {
                Id = existingConfig.Id.ToString(),
                Date = existingConfig.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                IsHoliday = existingConfig.IsHoliday,
                MaxAppointments = existingConfig.MaxAppointments
            };
        }

        public async Task<AppointmentDayConfigResponseDto> PostAppointmentDayConfigAsync(AppointmentDayConfigRequestDto request)
        {
            DateTime parsedConfigDate = DateTime.ParseExact(request.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            AppointmentDayConfig existingConfig = await _appointmentDayConfigRepository.GetByDateAsync(parsedConfigDate);

            if (existingConfig != null)
            {
                throw new BooklineBadRequestException($"Appointment config for {request.Date} has already been added.");
            }

            AppointmentDayConfig config = new AppointmentDayConfig
            {
                Date = parsedConfigDate,
                IsHoliday = request.IsHoliday,
                MaxAppointments = request.MaxAppointments,
            };

            config = await _appointmentDayConfigRepository.AddAsync(config);

            return new AppointmentDayConfigResponseDto
            {
                Id = config.Id.ToString(),
                Date = config.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                IsHoliday = config.IsHoliday,
                MaxAppointments = config.MaxAppointments
            };
        }

        public async Task<AppointmentDayConfigResponseDto> PartialUpdateAppointmentDayConfigAsync(string id, AppointmentDayConfigRequestDto request)
        {
            if (!Guid.TryParse(id, out Guid parsedGuid))
            {
                throw new BooklineBadRequestException("Invalid ID format. Expected a valid GUID.");
            }

            AppointmentDayConfig existingConfig = await _appointmentDayConfigRepository.GetByIdAsync(parsedGuid);

            if (existingConfig == null || existingConfig.DeletedAt != null)
            {
                throw new BooklineNotFoundException("Appointment day config not found or has been deleted.");
            }

            DateTime parsedConfigDate = DateTime.ParseExact(request.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            if (await _appointmentDayConfigRepository.IsDateUsedByOtherConfigAsync(parsedGuid, parsedConfigDate))
            {
                throw new BooklineBadRequestException("Another configuration already exists for this date.");
            }

            existingConfig.Date = parsedConfigDate;
            existingConfig.IsHoliday = request.IsHoliday;
            existingConfig.MaxAppointments = request.MaxAppointments;

            existingConfig = await _appointmentDayConfigRepository.PartialUpdateAsync(existingConfig);

            return new AppointmentDayConfigResponseDto
            {
                Id = existingConfig.Id.ToString(),
                Date = existingConfig.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                IsHoliday = existingConfig.IsHoliday,
                MaxAppointments = existingConfig.MaxAppointments
            };
        }

        public async Task HardDeleteAppointmentDayConfigAsync(string id)
        {
            if (!Guid.TryParse(id, out Guid parsedGuid))
            {
                throw new BooklineBadRequestException("Invalid ID format. Expected a valid GUID.");
            }

            AppointmentDayConfig existingConfig = await _appointmentDayConfigRepository.GetByIdAsync(parsedGuid);

            if (existingConfig == null)
            {
                throw new BooklineBadRequestException("Appointment day config not found or has already been deleted.");
            }

            await _appointmentDayConfigRepository.HardDeleteAsync(existingConfig);
        }
    }
}