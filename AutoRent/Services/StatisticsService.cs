using System.Data;

namespace AutoRent.Services
{
    public class StatisticsService
    {
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
    }
}
