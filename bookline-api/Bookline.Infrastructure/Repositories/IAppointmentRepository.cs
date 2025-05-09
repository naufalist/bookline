using Bookline.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bookline.Infrastructure.Repositories
{
    public interface IAppointmentRepository
    {
        Task<int> CountByDateAsync(DateTime date);
        Task<List<Appointment>> GetAsync();
        Task<List<Appointment>> GetByDateAsync(DateTime date);
        Task<Appointment> AddAsync(Appointment appointment);
    }
}