using System;
using System.Windows.Forms;

namespace AutoRent.Forms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeLayout();
        }

        private void InitializeLayout()
        {
            var employee = Session.CurrentEmployee;
            Text = employee != null ? $"AutoRent - prijavljen/a: {employee} ({employee.Role})" : "AutoRent";
            Width = 420;
            Height = 480;
            StartPosition = FormStartPosition.CenterScreen;

            var top = 20;
            var btnVehicles = AddMenuButton("Vozni park", ref top);
            btnVehicles.Click += (s, e) => new VehiclesForm().ShowDialog(this);

            var btnReservations = AddMenuButton("Rezervacije vozila", ref top);
            btnReservations.Click += (s, e) => new ReservationsForm().ShowDialog(this);

            var btnServices = AddMenuButton("Servisi vozila", ref top);
            btnServices.Click += (s, e) => new ServicesForm().ShowDialog(this);

            var btnClients = AddMenuButton("Klijenti", ref top);
            btnClients.Click += (s, e) => new ClientsForm().ShowDialog(this);

            var btnStatistics = AddMenuButton("Statistika korištenja vozila", ref top);
            btnStatistics.Click += (s, e) => new StatisticsForm().ShowDialog(this);

            if (employee != null && employee.Role == Models.EmployeeRole.VoditeljServisa)
            {
                var btnUsage = AddMenuButton("Korištenje vozila u periodu", ref top);
                btnUsage.Click += (s, e) => new VehicleUsageForm().ShowDialog(this);

                var btnReport = AddMenuButton("Godišnji financijski izvještaj", ref top);
                btnReport.Click += (s, e) => new ReportForm().ShowDialog(this);
            }

            var btnLogout = new Button { Text = "Odjava", Left = 20, Top = 380, Width = 340, Height = 35 };
            btnLogout.Click += (s, e) =>
            {
                Session.CurrentEmployee = null;
                Application.Restart();
            };
            Controls.Add(btnLogout);
        }

        private Button AddMenuButton(string text, ref int top)
        {
            var button = new Button { Text = text, Left = 20, Top = top, Width = 340, Height = 40 };
            button.BackColor = Theme.Primary;
            button.ForeColor = Theme.PrimaryText;
            Controls.Add(button);
            top += 50;
            return button;
        }
    }
}
