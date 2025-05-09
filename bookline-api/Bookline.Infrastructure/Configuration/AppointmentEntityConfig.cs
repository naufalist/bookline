using Bookline.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookline.Infrastructure.Configuration
{
    public class AppointmentEntityConfig : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.HasKey(appointment => appointment.Id);
            builder.HasOne(appointment => appointment.Customer).WithMany().HasForeignKey(a => a.CustomerId);
            builder.Property(appointment => appointment.Token).IsRequired();
        }
    }
}