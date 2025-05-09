using Bookline.Domain.Entities;
using System.Threading.Tasks;

namespace Bookline.Infrastructure.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer> GetByEmailAsync(string email);
        Task AddAsync(Customer customer);
    }
}