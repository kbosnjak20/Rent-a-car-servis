using System;
using System.Windows.Forms;
using AutoRent.Models;
using AutoRent.Repositories;
using AutoRent.Services;

namespace AutoRent.Forms
{
    public partial class ReportForm : Form
    {
        private readonly VehicleRepository _vehicleRepository = new VehicleRepository();
        private readonly ServiceRepository _serviceRepository = new ServiceRepository();
        private readonly ReportService _reportService = new ReportService();

        private readonly ComboBox _cmbVehicle = new ComboBox();
        private readonly NumericUpDown _numYear = new NumericUpDown();
        private readonly NumericUpDown _numFuelPrice = new NumericUpDown();
        private readonly Label _lblResult = new Label();
        private readonly DataGridView _gridExpiring = new DataGridView();

        public ReportForm()
        {
            InitializeLayout();
            LoadVehicles();
            LoadExpiringRegistrations();
        }

        private void InitializeLayout()
        {
            Text = "Godišnji financijski izvještaj";
            Width = 720;
            Height = 620;
            StartPosition = FormStartPosition.CenterScreen;

            _cmbVehicle.DropDownStyle = ComboBoxStyle.DropDownList;

            _numYear.Minimum = 2000;
            _numYear.Maximum = DateTime.Now.Year;
            _numYear.Value = DateTime.Now.Year;

            _numFuelPrice.DecimalPlaces = 2;
            _numFuelPrice.Increment = 0.01M;
            _numFuelPrice.Maximum = 10;
            _numFuelPrice.Value = 1.45M;

            var lblVehicle = new Label { Text = "Vozilo:", Left = 12, Top = 15, Width = 60 };
            _cmbVehicle.Left = 80; _cmbVehicle.Top = 12; _cmbVehicle.Width = 220;

            var lblYear = new Label { Text = "Godina:", Left = 310, Top = 15, Width = 55 };
            _numYear.Left = 370; _numYear.Top = 12; _numYear.Width = 70;

            var lblFuel = new Label { Text = "Cijena goriva (€/l):", Left = 450, Top = 15, Width = 120 };
            _numFuelPrice.Left = 575; _numFuelPrice.Top = 12; _numFuelPrice.Width = 70;

            var btnGenerate = new Button { Text = "Generiraj izvještaj", Left = 12, Top = 45, Width = 150 };
            btnGenerate.Click += (s, e) => GenerateReport();

            _lblResult.Left = 12;
            _lblResult.Top = 85;
            _lblResult.Width = 660;
            _lblResult.Height = 130;
            _lblResult.Font = new System.Drawing.Font(Font.FontFamily, 10);

            var lblExpiring = new Label { Text = "Vozila kojima registracija ističe za manje od 30 dana:", Left = 12, Top = 230, Width = 400 };
            _gridExpiring.Left = 12;
            _gridExpiring.Top = 252;
            _gridExpiring.Width = 680;
            _gridExpiring.Height = 300;
            _gridExpiring.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            _gridExpiring.ReadOnly = true;
            _gridExpiring.AllowUserToAddRows = false;
            _gridExpiring.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            Controls.Add(lblVehicle);
            Controls.Add(_cmbVehicle);
            Controls.Add(lblYear);
            Controls.Add(_numYear);
            Controls.Add(lblFuel);
            Controls.Add(_numFuelPrice);
            Controls.Add(btnGenerate);
            Controls.Add(_lblResult);
            Controls.Add(lblExpiring);
            Controls.Add(_gridExpiring);
        }

        private void LoadVehicles()
        {
            _cmbVehicle.Items.Clear();
            foreach (var vehicle in _vehicleRepository.GetAll()) _cmbVehicle.Items.Add(vehicle);
            if (_cmbVehicle.Items.Count > 0) _cmbVehicle.SelectedIndex = 0;
        }

        private void LoadExpiringRegistrations()
        {
            _gridExpiring.DataSource = _serviceRepository.GetVehiclesWithExpiringRegistration(30);
        }

        private void GenerateReport()
        {
            if (_cmbVehicle.SelectedItem == null)
            {
                MessageBox.Show("Odaberite vozilo.", "Nije odabrano vozilo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var vehicle = (Vehicle)_cmbVehicle.SelectedItem;
            var report = _reportService.GenerateAnnualReport(vehicle.Id, (int)_numYear.Value, _numFuelPrice.Value);

            _lblResult.Text =
                $"Vozilo: {report.VehicleDisplay}\r\n" +
                $"Godina: {report.Year}\r\n" +
                $"Ukupan prihod od najma: {report.TotalRevenue:0.00} €\r\n" +
                $"Ukupno prijeđeno kilometara: {report.TotalKilometersDriven:0} km\r\n" +
                $"Trošak goriva: {report.FuelCost:0.00} €\r\n" +
                $"Vozilo je bilo na servisu ove godine: {(report.WasServiced ? "da" : "ne")}\r\n" +
                $"Trošak servisa: {report.ServiceCost:0.00} €\r\n" +
                $"DOBIT: {report.Profit:0.00} €";
        }
    }
}
