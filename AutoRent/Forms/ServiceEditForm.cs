using System;
using System.Windows.Forms;
using AutoRent.Models;
using AutoRent.Services;

namespace AutoRent.Forms
{
    public partial class ServiceEditForm : Form
    {
        public ServiceRecord Service { get; private set; }

        private readonly AvailabilityService _availabilityService = new AvailabilityService();

        private readonly ComboBox _cmbVehicle = new ComboBox();
        private readonly DateTimePicker _dtStart = new DateTimePicker();
        private readonly DateTimePicker _dtEnd = new DateTimePicker();
        private readonly Label _lblPrice = new Label();
        private readonly Label _lblAvailability = new Label();

        public ServiceEditForm(System.Collections.Generic.List<Vehicle> vehicles)
        {
            InitializeLayout(vehicles);
        }

        private void InitializeLayout(System.Collections.Generic.List<Vehicle> vehicles)
        {
            Text = "Zakazivanje servisa";
            Width = 380;
            Height = 320;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;

            _cmbVehicle.DropDownStyle = ComboBoxStyle.DropDownList;
            foreach (var vehicle in vehicles) _cmbVehicle.Items.Add(vehicle);
            if (_cmbVehicle.Items.Count > 0) _cmbVehicle.SelectedIndex = 0;
            _cmbVehicle.SelectedIndexChanged += (s, e) =>
            {
                UpdateAvailabilityLabel();
                UpdatePriceLabel();
            };

            _dtStart.Format = DateTimePickerFormat.Custom;
            _dtStart.CustomFormat = "dd.MM.yyyy. HH:mm";
            _dtStart.Value = DateTime.Today.AddHours(8);
            _dtStart.ValueChanged += (s, e) => UpdateAvailabilityLabel();

            _dtEnd.Format = DateTimePickerFormat.Custom;
            _dtEnd.CustomFormat = "dd.MM.yyyy. HH:mm";
            _dtEnd.Value = DateTime.Today.AddDays(1).AddHours(17);
            _dtEnd.ValueChanged += (s, e) => UpdateAvailabilityLabel();

            _lblPrice.AutoSize = true;
            _lblPrice.Font = new System.Drawing.Font(Font, System.Drawing.FontStyle.Bold);

            _lblAvailability.ForeColor = System.Drawing.Color.Firebrick;
            _lblAvailability.AutoSize = true;

            AddRow("Vozilo:", _cmbVehicle, 15);
            AddRow("Početak servisa:", _dtStart, 50);
            AddRow("Kraj servisa:", _dtEnd, 85);

            var lblPriceCaption = new Label { Text = "Cijena servisa (prema kategoriji vozila):", Left = 15, Top = 123, Width = 250 };
            _lblPrice.Left = 15;
            _lblPrice.Top = 145;
            Controls.Add(lblPriceCaption);
            Controls.Add(_lblPrice);

            _lblAvailability.Left = 15;
            _lblAvailability.Top = 175;
            _lblAvailability.Width = 340;
            Controls.Add(_lblAvailability);

            var btnOk = new Button { Text = "Zakaži", Left = 90, Top = 220, Width = 90, DialogResult = DialogResult.OK };
            var btnCancel = new Button { Text = "Odustani", Left = 190, Top = 220, Width = 90, DialogResult = DialogResult.Cancel };
            btnOk.Click += BtnOk_Click;

            Controls.Add(btnOk);
            Controls.Add(btnCancel);
            AcceptButton = btnOk;
            CancelButton = btnCancel;

            UpdateAvailabilityLabel();
            UpdatePriceLabel();
        }

        private void UpdatePriceLabel()
        {
            if (_cmbVehicle.SelectedItem == null) return;
            var vehicle = (Vehicle)_cmbVehicle.SelectedItem;
            var price = ServicePricing.GetFixedPrice(vehicle.Category);
            _lblPrice.Text = $"{price:0.00} € ({vehicle.Category})";
        }

        private void AddRow(string labelText, Control input, int top)
        {
            var label = new Label { Text = labelText, Left = 15, Top = top + 3, Width = 190 };
            input.Left = 210;
            input.Top = top;
            input.Width = 145;
            Controls.Add(label);
            Controls.Add(input);
        }

        private void UpdateAvailabilityLabel()
        {
            if (_cmbVehicle.SelectedItem == null) return;
            var vehicle = (Vehicle)_cmbVehicle.SelectedItem;

            if (_dtStart.Value >= _dtEnd.Value)
            {
                _lblAvailability.Text = "Datum početka mora biti prije datuma završetka.";
                return;
            }

            var hasReservation = _availabilityService.HasActiveReservationInPeriod(vehicle.Id, _dtStart.Value, _dtEnd.Value);
            _lblAvailability.Text = hasReservation
                ? "Vozilo NIJE moguće poslati na servis - u tom je terminu već rezervirano."
                : "Vozilo je slobodno za servis u odabranom terminu.";
            _lblAvailability.ForeColor = hasReservation
                ? System.Drawing.Color.Firebrick
                : System.Drawing.Color.DarkGreen;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (_cmbVehicle.SelectedItem == null)
            {
                MessageBox.Show("Odaberite vozilo.", "Nepotpuni podaci", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            var vehicle = (Vehicle)_cmbVehicle.SelectedItem;

            if (_dtStart.Value >= _dtEnd.Value)
            {
                MessageBox.Show("Datum početka mora biti prije datuma završetka.", "Neispravan termin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            if (_availabilityService.HasActiveReservationInPeriod(vehicle.Id, _dtStart.Value, _dtEnd.Value))
            {
                MessageBox.Show(
                    "Vozilo se ne može poslati na servis jer je u odabranom terminu već rezervirano.\n" +
                    "Odaberite drugi termin ili drugo vozilo.",
                    "Servis nije moguće zakazati", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            Service = new ServiceRecord
            {
                VehicleId = vehicle.Id,
                StartDate = _dtStart.Value,
                EndDate = _dtEnd.Value,
                Price = ServicePricing.GetFixedPrice(vehicle.Category)
            };
        }
    }
}
