using System;
using System.Collections.Generic;
using System.Data;
using AutoRent.Models;

namespace AutoRent.Repositories
{
    public class ReservationRepository
    {
        private const string BaseSelect = @"
            SELECT r.*, 
                   v.Naziv AS VoziloNaziv, v.Marka AS VoziloMarka, v.BrojRegistracije,
                   k.Ime AS KlijentIme, k.Prezime AS KlijentPrezime
            FROM Rezervacija r
            INNER JOIN Vozilo  v ON v.ID_Vozilo  = r.ID_Vozilo
            INNER JOIN Klijent k ON k.ID_Klijent = r.ID_Klijent";

        public List<Reservation> Search(int? vehicleId = null, int? clientId = null,
            ReservationStatus? status = null, DateTime? from = null, DateTime? to = null)
        {
            var sql = BaseSelect + " WHERE 1 = 1";
            var parameters = new Dictionary<string, object>();

            if (vehicleId.HasValue)
            {
                sql += " AND r.ID_Vozilo = @VehicleId";
                parameters["@VehicleId"] = vehicleId.Value;
            }
            if (clientId.HasValue)
            {
                sql += " AND r.ID_Klijent = @ClientId";
                parameters["@ClientId"] = clientId.Value;
            }
            if (status.HasValue)
            {
                sql += " AND r.Status = @Status";
                parameters["@Status"] = status.Value.ToString();
            }
            if (from.HasValue)
            {
                sql += " AND r.DatumZavrsetka >= @From";
                parameters["@From"] = from.Value;
            }
            if (to.HasValue)
            {
                sql += " AND r.DatumPocetka <= @To";
                parameters["@To"] = to.Value;
            }

            sql += " ORDER BY r.DatumPocetka DESC";

            var table = Data.Db.ExecuteQuery(sql, parameters);
            var result = new List<Reservation>();
            foreach (DataRow row in table.Rows) result.Add(Map(row));
            return result;
        }

        public Reservation GetById(int id)
        {
            var table = Data.Db.ExecuteQuery(
                BaseSelect + " WHERE r.ID_Rezervacija = @Id",
                new Dictionary<string, object> { ["@Id"] = id });

            return table.Rows.Count == 0 ? null : Map(table.Rows[0]);
        }

        private static Reservation Map(DataRow row)
        {
            return new Reservation
            {
                Id = Convert.ToInt32(row["ID_Rezervacija"]),
                VehicleId = Convert.ToInt32(row["ID_Vozilo"]),
                ClientId = Convert.ToInt32(row["ID_Klijent"]),
                EmployeeId = Convert.ToInt32(row["ID_Zaposlenik"]),
                StartDate = Convert.ToDateTime(row["DatumPocetka"]),
                EndDate = Convert.ToDateTime(row["DatumZavrsetka"]),
                RentalType = (RentalType)Enum.Parse(typeof(RentalType), row["TipNajma"].ToString()),
                Status = (ReservationStatus)Enum.Parse(typeof(ReservationStatus), row["Status"].ToString()),
                MileageAtPickup = row["StanjeKmPreuzimanje"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["StanjeKmPreuzimanje"]),
                MileageAtReturn = row["StanjeKmPovrat"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["StanjeKmPovrat"]),
                DamageDescription = row["OpisOstecenja"] == DBNull.Value ? null : row["OpisOstecenja"].ToString(),
                RentalPrice = row["IznosNajma"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(row["IznosNajma"]),
                VehicleDisplay = $"{row["VoziloMarka"]} {row["VoziloNaziv"]} ({row["BrojRegistracije"]})",
                ClientDisplay = $"{row["KlijentIme"]} {row["KlijentPrezime"]}"
            };
        }
    }
}
