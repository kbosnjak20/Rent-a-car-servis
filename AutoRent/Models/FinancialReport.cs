namespace AutoRent.Models
{
    public class FinancialReport
    {
        public int VehicleId { get; set; }
        public string VehicleDisplay { get; set; }
        public int Year { get; set; }
        public decimal TotalRevenue { get; set; }
        public double TotalKilometersDriven { get; set; }
        public decimal FuelCost { get; set; }
        public bool WasServiced { get; set; }
        public decimal ServiceCost { get; set; }
        public decimal Profit => TotalRevenue - FuelCost - ServiceCost;
    }
}
