using System;
using System.Collections.Generic;
using System.Data;
using AutoRent.Models;

namespace AutoRent.Repositories
{
    public class VehicleRepository
    {
        public List<Vehicle> GetAll(VehicleCategory? category = null, bool onlyActive = true)
        {
            var sql = "SELECT * FROM Vozilo WHERE 1 = 1";
            var parameters = new Dictionary<string, object>();

            if (category.HasValue)
            {
                sql += " AND Kategorija = @Kategorija";
                parameters["@Kategorija"] = category.Value.ToString();
            }

            sql += " ORDER BY Naziv";

            var table = Data.Db.ExecuteQuery(sql, parameters);
            return MapAll(table);
        }

        public Vehicle GetById(int id)
        {
            var table = Data.Db.ExecuteQuery(
                "SELECT * FROM Vozilo WHERE ID_Vozilo = @Id",
                new Dictionary<string, object> { ["@Id"] = id });

            return table.Rows.Count == 0 ? null : Map(table.Rows[0]);
        }

        public int Insert(Vehicle vehicle)
        {
            const string sql = @"
                INSERT INTO Vozilo
                    (Naziv, Marka, Godiste, BrojRegistracije, DatumRegistracije, TipGoriva,
                     ProsjecnaPotrosnja, Kategorija, CijenaPoSatu, CijenaPoDanu,
                     PocetnoStanjeKm, TrenutnoStanjeKm)
                VALUES
                    (@Naziv, @Marka, @Godiste, @BrojRegistracije, @DatumRegistracije, @TipGoriva,
                     @ProsjecnaPotrosnja, @Kategorija, @CijenaPoSatu, @CijenaPoDanu,
                     @PocetnoStanjeKm, @PocetnoStanjeKm)";

            return Data.Db.ExecuteInsertReturnId(sql, ToParameters(vehicle));
        }

        public void Update(Vehicle vehicle)
        {
            const string sql = @"
                UPDATE Vozilo SET
                    Naziv = @Naziv,
                    Marka = @Marka,
                    Godiste = @Godiste,
                    BrojRegistracije = @BrojRegistracije,
                    DatumRegistracije = @DatumRegistracije,
                    TipGoriva = @TipGoriva,
                    ProsjecnaPotrosnja = @ProsjecnaPotrosnja,
                    Kategorija = @Kategorija,
                    CijenaPoSatu = @CijenaPoSatu,
                    CijenaPoDanu = @CijenaPoDanu
                WHERE ID_Vozilo = @Id";

            var parameters = ToParameters(vehicle);
            parameters["@Id"] = vehicle.Id;
            Data.Db.ExecuteNonQuery(sql, parameters);
        }

        public void Delete(int id)
        {
            Data.Db.ExecuteNonQuery(
                "DELETE FROM Vozilo WHERE ID_Vozilo = @Id",
                new Dictionary<string, object> { ["@Id"] = id });
        }

        public void UpdateMileage(int vehicleId, int newMileage)
        {
            Data.Db.ExecuteNonQuery(
                "UPDATE Vozilo SET TrenutnoStanjeKm = @Km WHERE ID_Vozilo = @Id",
                new Dictionary<string, object> { ["@Km"] = newMileage, ["@Id"] = vehicleId });
        }

        private static Dictionary<string, object> ToParameters(Vehicle vehicle)
        {
            return new Dictionary<string, object>
            {
                ["@Naziv"] = vehicle.Name,
                ["@Marka"] = vehicle.Brand,
                ["@Godiste"] = vehicle.Year,
                ["@BrojRegistracije"] = vehicle.RegistrationNumber,
                ["@DatumRegistracije"] = vehicle.RegistrationDate,
                ["@TipGoriva"] = vehicle.FuelType.ToString(),
                ["@ProsjecnaPotrosnja"] = vehicle.AverageConsumption,
                ["@Kategorija"] = vehicle.Category.ToString(),
                ["@CijenaPoSatu"] = vehicle.PricePerHour,
                ["@CijenaPoDanu"] = vehicle.PricePerDay,
                ["@PocetnoStanjeKm"] = vehicle.InitialMileage
            };
        }

        private static List<Vehicle> MapAll(DataTable table)
        {
            var result = new List<Vehicle>();
            foreach (DataRow row in table.Rows)
            {
                result.Add(Map(row));
            }
            return result;
        }

        private static Vehicle Map(DataRow row)
        {
            return new Vehicle
            {
                Id = Convert.ToInt32(row["ID_Vozilo"]),
                Name = row["Naziv"].ToString(),
                Brand = row["Marka"].ToString(),
                Year = Convert.ToInt32(row["Godiste"]),
                RegistrationNumber = row["BrojRegistracije"].ToString(),
                RegistrationDate = Convert.ToDateTime(row["DatumRegistracije"]),
                FuelType = (FuelType)Enum.Parse(typeof(FuelType), row["TipGoriva"].ToString()),
                AverageConsumption = Convert.ToDouble(row["ProsjecnaPotrosnja"]),
                Category = (VehicleCategory)Enum.Parse(typeof(VehicleCategory), row["Kategorija"].ToString()),
                PricePerHour = Convert.ToDecimal(row["CijenaPoSatu"]),
                PricePerDay = Convert.ToDecimal(row["CijenaPoDanu"]),
                InitialMileage = Convert.ToInt32(row["PocetnoStanjeKm"]),
                CurrentMileage = Convert.ToInt32(row["TrenutnoStanjeKm"])
            };
        }
    }
}
