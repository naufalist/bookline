using System;

namespace Bookline.Domain.Entities
{
    public class Customer : AuditableEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }
}