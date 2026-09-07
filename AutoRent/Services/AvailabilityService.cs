using System;
using System.Collections.Generic;
using AutoRent.Repositories;

namespace AutoRent.Services
{
    public class AvailabilityService
    {
        private readonly ServiceRepository _serviceRepository = new ServiceRepository();

        public bool IsVehicleAvailable(int vehicleId, DateTime from, DateTime to, int? excludeReservationId = null)
        {
            if (from >= to)
            {
                throw new ArgumentException("Datum početka mora biti prije datuma završetka.");
            }

            if (HasReservationOverlap(vehicleId, from, to, excludeReservationId))
            {
                return false;
            }

            if (_serviceRepository.HasServiceOverlap(vehicleId, from, to))
            {
                return false;
            }

            return true;
        }

        public bool HasActiveReservationInPeriod(int vehicleId, DateTime from, DateTime to)
        {
            return HasReservationOverlap(vehicleId, from, to, excludeReservationId: null);
        }

        private bool HasReservationOverlap(int vehicleId, DateTime from, DateTime to, int? excludeReservationId)
        {
            var sql = @"
                SELECT COUNT(*) FROM Rezervacija
                WHERE ID_Vozilo = @VehicleId
                  AND Status = 'Aktivna'
                  AND DatumPocetka < @To AND DatumZavrsetka > @From";

            var parameters = new Dictionary<string, object>
            {
                ["@VehicleId"] = vehicleId,
                ["@From"] = from,
                ["@To"] = to
            };

            if (excludeReservationId.HasValue)
            {
                sql += " AND ID_Rezervacija <> @ExcludeId";
                parameters["@ExcludeId"] = excludeReservationId.Value;
            }

            var result = Data.Db.ExecuteScalar(sql, parameters);
            return Convert.ToInt32(result) > 0;
        }
    }
}
