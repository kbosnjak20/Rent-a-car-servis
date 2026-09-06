using System;

namespace AutoRent.Models
{
    public enum VehicleCategory
    {
        OsobnoVozilo,
        PutnickiKombi,
        TeretniKombi,
        Limuzina
    }

    public enum FuelType
    {
        Dizel,
        Benzin
    }

    public class Vehicle
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public int Year { get; set; }
        public string RegistrationNumber { get; set; }
        public DateTime RegistrationDate { get; set; }
        public FuelType FuelType { get; set; }
        public double AverageConsumption { get; set; }
        public VehicleCategory Category { get; set; }
        public decimal PricePerHour { get; set; }
        public decimal PricePerDay { get; set; }
        public int InitialMileage { get; set; }
        public int CurrentMileage { get; set; }

        public override string ToString()
        {
            return $"{Brand} {Name} ({RegistrationNumber})";
        }
    }
}
