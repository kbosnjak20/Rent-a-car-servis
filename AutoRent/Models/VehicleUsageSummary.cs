namespace AutoRent.Models
{
    public class VehicleUsageSummary
    {
        public string VehicleDisplay { get; set; }
        public System.DateTime From { get; set; }
        public System.DateTime To { get; set; }
        public int ReservationCount { get; set; }
        public double TotalRentedHours { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
