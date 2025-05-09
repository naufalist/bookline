using Bookline.Application.Interfaces;
using Bookline.Domain.DTOs;
using Bookline.Domain.Entities;
using Bookline.Domain.Exceptions;
using Bookline.Domain.Helpers;
using Bookline.Infrastructure.Repositories;
using System;
using System.Threading.Tasks;

namespace Bookline.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task PostCustomerAsync(CustomerRequestDto request)
        {
            Customer existingCustomer = await _customerRepository.GetByEmailAsync(request.Email);

            if (existingCustomer != null)
            {
                throw new BooklineBadRequestException("Email already registered.");
            }

            Customer customer = new Customer
            {
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = PasswordHelper.HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow
            };

            await _customerRepository.AddAsync(customer);
        }
    }
}