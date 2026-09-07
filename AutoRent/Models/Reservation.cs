using System;

namespace AutoRent.Models
{
    public enum RentalType
    {
        Sat,
        Dan
    }

    public enum ReservationStatus
    {
        Aktivna,
        Zavrsena,
        Otkazana
    }

    public class Reservation
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public int ClientId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public RentalType RentalType { get; set; }
        public ReservationStatus Status { get; set; }
        public int? MileageAtPickup { get; set; }
        public int? MileageAtReturn { get; set; }
        public string DamageDescription { get; set; }
        public decimal? RentalPrice { get; set; }

        public string VehicleDisplay { get; set; }
        public string ClientDisplay { get; set; }
    }
}
