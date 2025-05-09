using System;

namespace Bookline.Domain.Entities
{
    public class AppointmentAvailabilityDay
    {
        public AppointmentAvailabilityDay(DateTime date, int count)
        {
            Date = date;
            Count = count;
        }

        public DateTime Date { get; set; }
        public int Count { get; set; }
    }
}