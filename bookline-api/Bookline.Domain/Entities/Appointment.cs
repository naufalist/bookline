using System;

namespace Bookline.Domain.Entities
{
    public class Appointment : AuditableEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CustomerId { get; set; }
        public DateTime Date { get; set; }
        public string Token { get; set; }

        // Relation
        public Customer Customer { get; set; }
    }
}