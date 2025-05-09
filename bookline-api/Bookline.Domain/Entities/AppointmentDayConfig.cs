using System;

namespace Bookline.Domain.Entities
{
    public class AppointmentDayConfig : AuditableEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Date { get; set; }
        public bool IsHoliday { get; set; }
        public int MaxAppointments { get; set; }
    }
}