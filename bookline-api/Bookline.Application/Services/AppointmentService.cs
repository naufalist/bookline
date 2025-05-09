using Bookline.Application.Interfaces;
using Bookline.Domain.DTOs;
using Bookline.Domain.Entities;
using Bookline.Domain.Exceptions;
using Bookline.Domain.Helpers;
using Bookline.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Bookline.Application.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ICustomerRepository _customerRepository;

        public AppointmentService(IAppointmentRepository appointmentRepository, ICustomerRepository customerRepository)
        {
            _appointmentRepository = appointmentRepository;
            _customerRepository = customerRepository;
        }

        public async Task<List<AppointmentResponseDto>> GetAppointments()
        {
            List<Appointment> appointments = await _appointmentRepository.GetAsync();

            return appointments
                .Select(appointment => new AppointmentResponseDto
                {
                    Date = appointment.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Token = appointment.Token
                })
                .ToList();
        }

        public async Task<List<AppointmentResponseDto>> GetAppointmentsByDateAsync(string date)
        {
            if (!DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                throw new BooklineBadRequestException("Invalid date format. Expected format is yyyy-MM-dd.");
            }

            List<Appointment> appointments = await _appointmentRepository.GetByDateAsync(parsedDate);

            return appointments
                .Select(appointment => new AppointmentResponseDto
                {
                    Date = appointment.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Token = appointment.Token,
                    Customer = new CustomerResponseDto()
                    {
                        FullName = appointment.Customer.FullName,
                        Email = appointment.Customer.Email
                    }
                })
                .ToList();
        }

        public async Task<AppointmentResponseDto> PostAppointmentAsync(AppointmentRequestDto request)
        {
            Customer customer = await _customerRepository.GetByEmailAsync(request.Email);

            if (customer == null)
            {
                throw new BooklineBadRequestException("Invalid email address or password.");
            }

            if (!PasswordHelper.VerifyPassword(request.Password, customer.PasswordHash))
            {
                throw new BooklineBadRequestException("Invalid email address or password.");
            }

            DateTime parsedAppointmentDate = DateTime.ParseExact(request.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            if (parsedAppointmentDate.Date < DateTime.Today)
            {
                throw new BooklineBadRequestException("Appointment date value cannot be in the past.");
            }

            Appointment appointment = new Appointment
            {
                Customer = customer,
                Date = parsedAppointmentDate
            };

            appointment = await _appointmentRepository.AddAsync(appointment);

            return new AppointmentResponseDto
            {
                Date = appointment.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                Token = appointment.Token
            };
        }
    }
}