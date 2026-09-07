using System;
using System.Collections.Generic;
using System.Data;
using AutoRent.Models;

namespace AutoRent.Repositories
{
    public class ClientRepository
    {
        public List<Client> GetAll()
        {
            var table = Data.Db.ExecuteQuery("SELECT * FROM Klijent ORDER BY Prezime, Ime");
            var result = new List<Client>();
            foreach (DataRow row in table.Rows) result.Add(Map(row));
            return result;
        }

        public Client GetById(int id)
        {
            var table = Data.Db.ExecuteQuery(
                "SELECT * FROM Klijent WHERE ID_Klijent = @Id",
                new Dictionary<string, object> { ["@Id"] = id });
            return table.Rows.Count == 0 ? null : Map(table.Rows[0]);
        }

        public int Insert(Client client)
        {
            const string sql = @"
                INSERT INTO Klijent (Ime, Prezime, Kontakt)
                VALUES (@Ime, @Prezime, @Kontakt)";

            return Data.Db.ExecuteInsertReturnId(sql, new Dictionary<string, object>
            {
                ["@Ime"] = client.FirstName,
                ["@Prezime"] = client.LastName,
                ["@Kontakt"] = client.Contact
            });
        }

        private static Client Map(DataRow row)
        {
            var contact = row["Kontakt"] == DBNull.Value ? null : row["Kontakt"].ToString();

            var isVoditelj = Session.CurrentEmployee != null
                && Session.CurrentEmployee.Role == EmployeeRole.VoditeljServisa;

            return new Client
            {
                Id = Convert.ToInt32(row["ID_Klijent"]),
                FirstName = row["Ime"].ToString(),
                LastName = row["Prezime"].ToString(),
                Contact = isVoditelj ? contact : Services.PrivacyHelper.MaskContact(contact)
            };
        }
    }
}
