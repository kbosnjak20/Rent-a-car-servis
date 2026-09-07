using System;
using System.Collections.Generic;
using System.Data;
using AutoRent.Models;

namespace AutoRent.Repositories
{
    public class ServiceRepository
    {
        public List<ServiceRecord> GetByVehicle(int vehicleId)
        {
            var table = Data.Db.ExecuteQuery(
                "SELECT * FROM Servis WHERE ID_Vozilo = @VehicleId ORDER BY DatumOd",
                new Dictionary<string, object> { ["@VehicleId"] = vehicleId });

            var result = new List<ServiceRecord>();
            foreach (DataRow row in table.Rows) result.Add(Map(row));
            return result;
        }

        public List<ServiceRecord> GetByVehicleAndYear(int vehicleId, int year)
        {
            var table = Data.Db.ExecuteQuery(
                @"SELECT * FROM Servis
                  WHERE ID_Vozilo = @VehicleId AND YEAR(DatumOd) = @Year",
                new Dictionary<string, object> { ["@VehicleId"] = vehicleId, ["@Year"] = year });

            var result = new List<ServiceRecord>();
            foreach (DataRow row in table.Rows) result.Add(Map(row));
            return result;
        }

        public int Insert(ServiceRecord service)
        {
            const string sql = @"
                INSERT INTO Servis (ID_Vozilo, DatumOd, DatumDo, Cijena)
                VALUES (@ID_Vozilo, @DatumOd, @DatumDo, @Cijena)";

            return Data.Db.ExecuteInsertReturnId(sql, new Dictionary<string, object>
            {
                ["@ID_Vozilo"] = service.VehicleId,
                ["@DatumOd"] = service.StartDate,
                ["@DatumDo"] = service.EndDate,
                ["@Cijena"] = service.Price
            });
        }

        public bool HasServiceOverlap(int vehicleId, DateTime from, DateTime to)
        {
            var result = Data.Db.ExecuteScalar(
                @"SELECT COUNT(*) FROM Servis
                  WHERE ID_Vozilo = @VehicleId
                    AND DatumOd < @To AND DatumDo > @From",
                new Dictionary<string, object>
                {
                    ["@VehicleId"] = vehicleId,
                    ["@From"] = from,
                    ["@To"] = to
                });

            return Convert.ToInt32(result) > 0;
        }

        public DataTable GetVehiclesWithExpiringRegistration(int daysThreshold = 30)
        {
            return Data.Db.ExecuteQuery(
                @"SELECT ID_Vozilo, Naziv, Marka, BrojRegistracije, DatumRegistracije,
                         DATEDIFF(DAY, GETDATE(), DATEADD(YEAR, 1, DatumRegistracije)) AS DanaDoIsteka
                  FROM Vozilo
                  WHERE DATEDIFF(DAY, GETDATE(), DATEADD(YEAR, 1, DatumRegistracije)) < @Days
                  ORDER BY DanaDoIsteka",
                new Dictionary<string, object> { ["@Days"] = daysThreshold });
        }

        private static ServiceRecord Map(DataRow row)
        {
            return new ServiceRecord
            {
                Id = Convert.ToInt32(row["ID_Servis"]),
                VehicleId = Convert.ToInt32(row["ID_Vozilo"]),
                StartDate = Convert.ToDateTime(row["DatumOd"]),
                EndDate = Convert.ToDateTime(row["DatumDo"]),
                Price = row["Cijena"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(row["Cijena"])
            };
        }
    }
}
