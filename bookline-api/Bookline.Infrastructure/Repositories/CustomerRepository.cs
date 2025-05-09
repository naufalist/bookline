using Bookline.Domain.Entities;
using Bookline.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Bookline.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Customer> GetByEmailAsync(string email)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(customer => customer.Email.ToLower() == email.ToLower());
        }

        public async Task AddAsync(Customer customer)
        {
            _context.Customers.Add(customer);

            await _context.SaveChangesAsync();
        }
    }
}