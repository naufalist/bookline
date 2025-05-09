using Bookline.Domain.DTOs;
using System.Threading.Tasks;

namespace Bookline.Application.Interfaces
{
    public interface ICustomerService
    {
        Task PostCustomerAsync(CustomerRequestDto customer);
    }
}