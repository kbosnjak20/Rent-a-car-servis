using System;
using System.Collections.Generic;
using System.Data;
using AutoRent.Models;

namespace AutoRent.Repositories
{
    public class EmployeeRepository
    {
        public Employee Authenticate(string username, string password)
        {
            var table = Data.Db.ExecuteQuery(
                "SELECT * FROM Zaposlenik WHERE KorisnickoIme = @Username AND Lozinka = @Password",
                new Dictionary<string, object>
                {
                    ["@Username"] = username,
                    ["@Password"] = password
                });

            return table.Rows.Count == 0 ? null : Map(table.Rows[0]);
        }

        public Employee GetById(int id)
        {
            var table = Data.Db.ExecuteQuery(
                "SELECT * FROM Zaposlenik WHERE ID_Zaposlenik = @Id",
                new Dictionary<string, object> { ["@Id"] = id });
            return table.Rows.Count == 0 ? null : Map(table.Rows[0]);
        }

        private static Employee Map(DataRow row)
        {
            return new Employee
            {
                Id = Convert.ToInt32(row["ID_Zaposlenik"]),
                FirstName = row["Ime"].ToString(),
                LastName = row["Prezime"].ToString(),
                Username = row["KorisnickoIme"].ToString(),
                Role = (EmployeeRole)Enum.Parse(typeof(EmployeeRole), row["Uloga"].ToString())
            };
        }
    }
}
