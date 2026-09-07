using System;

namespace AutoRent.Models
{
    public class ServiceRecord
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? Price { get; set; }
    }
}
