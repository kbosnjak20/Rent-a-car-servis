using System;
using System.Data;
using System.Linq;
using AutoRent.Models;
using AutoRent.Repositories;

namespace AutoRent.Services
{
    public class StatisticsService
    {
        private readonly VehicleRepository _vehicleRepository = new VehicleRepository();
        private readonly ReservationRepository _reservationRepository = new ReservationRepository();

        public DataTable GetReservationCountsByVehicle()
        {
            return Data.Db.ExecuteQuery(@"
                SELECT v.Naziv, v.Marka, v.BrojRegistracije,
                       COUNT(r.ID_Rezervacija) AS BrojRezervacija
                FROM Vozilo v
                LEFT JOIN Rezervacija r ON r.ID_Vozilo = v.ID_Vozilo AND r.Status <> 'Otkazana'
                GROUP BY v.ID_Vozilo, v.Naziv, v.Marka, v.BrojRegistracije
                ORDER BY BrojRezervacija DESC");
        }

        public DataTable GetUtilizationByCategory()
        {
            return Data.Db.ExecuteQuery(@"
                SELECT v.Kategorija,
                       COUNT(DISTINCT v.ID_Vozilo) AS BrojVozila,
                       COUNT(r.ID_Rezervacija) AS BrojRezervacija
                FROM Vozilo v
                LEFT JOIN Rezervacija r ON r.ID_Vozilo = v.ID_Vozilo AND r.Status <> 'Otkazana'
                GROUP BY v.Kategorija
                ORDER BY BrojRezervacija DESC");
        }

        public VehicleUsageSummary GetVehicleUsage(int vehicleId, DateTime from, DateTime to)
        {
            var vehicle = _vehicleRepository.GetById(vehicleId);
            var reservations = _reservationRepository
                .Search(vehicleId: vehicleId, status: null, from: from, to: to)
                .Where(r => r.Status != ReservationStatus.Otkazana)
                .ToList();

            var totalHours = reservations.Sum(r => (r.EndDate - r.StartDate).TotalHours);
            var totalRevenue = reservations.Sum(r => r.RentalPrice ?? 0M);

            return new VehicleUsageSummary
            {
                VehicleDisplay = vehicle?.ToString() ?? $"#{vehicleId}",
                From = from,
                To = to,
                ReservationCount = reservations.Count,
                TotalRentedHours = totalHours,
                TotalRevenue = totalRevenue
            };
        }
    }
}
