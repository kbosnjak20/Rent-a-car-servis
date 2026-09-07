using System;
using AutoRent.Models;

namespace AutoRent.Services
{
    public class PricingService
    {
        public decimal CalculatePrice(Vehicle vehicle, DateTime start, DateTime end, RentalType rentalType)
        {
            if (end <= start)
            {
                throw new ArgumentException("Datum završetka mora biti nakon datuma početka.");
            }

            var duration = end - start;

            if (rentalType == RentalType.Sat)
            {
                var hours = (int)Math.Ceiling(duration.TotalHours);
                return hours * vehicle.PricePerHour;
            }

            var days = (int)Math.Ceiling(duration.TotalDays);
            return days * vehicle.PricePerDay;
        }
    }
}
