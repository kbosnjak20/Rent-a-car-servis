using System;
using System.Windows.Forms;
using AutoRent.Models;
using AutoRent.Repositories;
using AutoRent.Services;

namespace AutoRent.Forms
{
    public partial class VehicleUsageForm : Form
    {
        private readonly VehicleRepository _vehicleRepository = new VehicleRepository();
        private readonly StatisticsService _statisticsService = new StatisticsService();

        private readonly ComboBox _cmbVehicle = new ComboBox();
        private readonly DateTimePicker _dtFrom = new DateTimePicker();
        private readonly DateTimePicker _dtTo = new DateTimePicker();
        private readonly Label _lblResult = new Label();

        public VehicleUsageForm()
        {
            InitializeLayout();
            LoadVehicles();
        }

        private void InitializeLayout()
        {
            Text = "Pregled korištenja vozila u zadanom periodu";
            Width = 480;
            Height = 320;
            StartPosition = FormStartPosition.CenterScreen;

            _cmbVehicle.DropDownStyle = ComboBoxStyle.DropDownList;

            _dtFrom.Format = DateTimePickerFormat.Short;
            _dtFrom.Value = DateTime.Today.AddMonths(-1);

            _dtTo.Format = DateTimePickerFormat.Short;
            _dtTo.Value = DateTime.Today;

            var lblVehicle = new Label { Text = "Vozilo:", Left = 15, Top = 18, Width = 60 };
            _cmbVehicle.Left = 150; _cmbVehicle.Top = 15; _cmbVehicle.Width = 280;

            var lblFrom = new Label { Text = "Razdoblje od:", Left = 15, Top = 53, Width = 100 };
            _dtFrom.Left = 150; _dtFrom.Top = 50; _dtFrom.Width = 150;

            var lblTo = new Label { Text = "do:", Left = 15, Top = 88, Width = 100 };
            _dtTo.Left = 150; _dtTo.Top = 85; _dtTo.Width = 150;

            var btnShow = new Button { Text = "Prikaži korištenje", Left = 15, Top = 125, Width = 160 };
            btnShow.Click += (s, e) => ShowUsage();

            _lblResult.Left = 15;
            _lblResult.Top = 165;
            _lblResult.Width = 430;
            _lblResult.Height = 120;

            Controls.Add(lblVehicle);
            Controls.Add(_cmbVehicle);
            Controls.Add(lblFrom);
            Controls.Add(_dtFrom);
            Controls.Add(lblTo);
            Controls.Add(_dtTo);
            Controls.Add(btnShow);
            Controls.Add(_lblResult);
        }

        private void LoadVehicles()
        {
            foreach (var vehicle in _vehicleRepository.GetAll()) _cmbVehicle.Items.Add(vehicle);
            if (_cmbVehicle.Items.Count > 0) _cmbVehicle.SelectedIndex = 0;
        }

        private void ShowUsage()
        {
            if (_cmbVehicle.SelectedItem == null)
            {
                MessageBox.Show("Odaberite vozilo.", "Nije odabrano vozilo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (_dtFrom.Value > _dtTo.Value)
            {
                MessageBox.Show("Datum \"od\" mora biti prije ili jednak datumu \"do\".", "Neispravno razdoblje",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var vehicle = (Vehicle)_cmbVehicle.SelectedItem;
            var usage = _statisticsService.GetVehicleUsage(vehicle.Id, _dtFrom.Value.Date, _dtTo.Value.Date.AddDays(1).AddSeconds(-1));

            _lblResult.Text =
                $"Vozilo: {usage.VehicleDisplay}\r\n" +
                $"Razdoblje: {usage.From:dd.MM.yyyy.} - {usage.To:dd.MM.yyyy.}\r\n\r\n" +
                $"Broj rezervacija u razdoblju: {usage.ReservationCount}\r\n" +
                $"Ukupno sati najma: {usage.TotalRentedHours:0.0} h\r\n" +
                $"Ukupan ostvareni prihod: {usage.TotalRevenue:0.00} €";
        }
    }
}
