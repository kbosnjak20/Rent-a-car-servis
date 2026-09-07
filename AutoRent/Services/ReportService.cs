using System.Linq;
using AutoRent.Models;
using AutoRent.Repositories;

namespace AutoRent.Services
{
    public class ReportService
    {
        private readonly VehicleRepository _vehicleRepository = new VehicleRepository();
        private readonly ReservationRepository _reservationRepository = new ReservationRepository();
        private readonly ServiceRepository _serviceRepository = new ServiceRepository();

        public FinancialReport GenerateAnnualReport(int vehicleId, int year, decimal fuelPricePerLiter)
        {
            var vehicle = _vehicleRepository.GetById(vehicleId);

            var reservations = _reservationRepository
                .Search(vehicleId: vehicleId, status: ReservationStatus.Zavrsena)
                .Where(r => r.StartDate.Year == year || r.EndDate.Year == year)
                .ToList();

            var totalRevenue = reservations.Sum(r => r.RentalPrice ?? 0M);

            var totalKm = reservations
                .Where(r => r.MileageAtPickup.HasValue && r.MileageAtReturn.HasValue)
                .Sum(r => r.MileageAtReturn.Value - r.MileageAtPickup.Value);

            var fuelCost = (decimal)(totalKm * vehicle.AverageConsumption / 100.0) * fuelPricePerLiter;

            var serviceRecords = _serviceRepository.GetByVehicleAndYear(vehicleId, year);
            var wasServiced = serviceRecords.Count > 0;
            var serviceCost = wasServiced ? ServicePricing.GetFixedPrice(vehicle.Category) : 0M;

            return new FinancialReport
            {
                VehicleId = vehicle.Id,
                VehicleDisplay = vehicle.ToString(),
                Year = year,
                TotalRevenue = totalRevenue,
                TotalKilometersDriven = totalKm,
                FuelCost = fuelCost,
                WasServiced = wasServiced,
                ServiceCost = serviceCost
            };
        }
    }
}
