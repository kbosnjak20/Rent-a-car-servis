using System;
using System.Linq;
using System.Windows.Forms;
using AutoRent.Models;
using AutoRent.Repositories;
using AutoRent.Services;

namespace AutoRent.Forms
{
    public partial class ReservationEditForm : Form
    {
        public Reservation Reservation { get; private set; }

        private readonly VehicleRepository _vehicleRepository = new VehicleRepository();
        private readonly ClientRepository _clientRepository = new ClientRepository();
        private readonly AvailabilityService _availabilityService = new AvailabilityService();

        private readonly ComboBox _cmbVehicle = new ComboBox();
        private readonly ComboBox _cmbClient = new ComboBox();
        private readonly Button _btnNewClient = new Button();
        private readonly DateTimePicker _dtStart = new DateTimePicker();
        private readonly DateTimePicker _dtEnd = new DateTimePicker();
        private readonly ComboBox _cmbRentalType = new ComboBox();
        private readonly NumericUpDown _numMileage = new NumericUpDown();
        private readonly Label _lblAvailability = new Label();

        public ReservationEditForm()
        {
            InitializeLayout();
            LoadVehicles();
            LoadClients();
        }

        private void InitializeLayout()
        {
            Text = "Nova rezervacija";
            Width = 420;
            Height = 400;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;

            _cmbVehicle.DropDownStyle = ComboBoxStyle.DropDownList;
            _cmbVehicle.SelectedIndexChanged += (s, e) => UpdateAvailabilityLabel();

            _cmbClient.DropDownStyle = ComboBoxStyle.DropDownList;

            _btnNewClient.Text = "Novi klijent...";
            _btnNewClient.Click += BtnNewClient_Click;

            _dtStart.Format = DateTimePickerFormat.Custom;
            _dtStart.CustomFormat = "dd.MM.yyyy. HH:mm";
            _dtStart.Value = DateTime.Now;
            _dtStart.ValueChanged += (s, e) => UpdateAvailabilityLabel();

            _dtEnd.Format = DateTimePickerFormat.Custom;
            _dtEnd.CustomFormat = "dd.MM.yyyy. HH:mm";
            _dtEnd.Value = DateTime.Now.AddDays(1);
            _dtEnd.ValueChanged += (s, e) => UpdateAvailabilityLabel();

            _cmbRentalType.DropDownStyle = ComboBoxStyle.DropDownList;
            foreach (RentalType type in Enum.GetValues(typeof(RentalType))) _cmbRentalType.Items.Add(type);
            _cmbRentalType.SelectedIndex = 1;

            _numMileage.Maximum = 1000000;

            _lblAvailability.ForeColor = System.Drawing.Color.Firebrick;
            _lblAvailability.AutoSize = true;

            AddRow("Vozilo:", _cmbVehicle, 15);
            AddRow("Klijent:", _cmbClient, 50);
            _btnNewClient.Left = 210;
            _btnNewClient.Top = 80;
            _btnNewClient.Width = 170;
            Controls.Add(_btnNewClient);

            AddRow("Početak najma:", _dtStart, 115);
            AddRow("Kraj najma:", _dtEnd, 150);
            AddRow("Tip najma:", _cmbRentalType, 185);
            AddRow("Stanje km pri preuzimanju:", _numMileage, 220);

            _lblAvailability.Left = 15;
            _lblAvailability.Top = 255;
            _lblAvailability.Width = 370;
            Controls.Add(_lblAvailability);

            var btnOk = new Button { Text = "Rezerviraj", Left = 120, Top = 310, Width = 100, DialogResult = DialogResult.OK };
            var btnCancel = new Button { Text = "Odustani", Left = 230, Top = 310, Width = 100, DialogResult = DialogResult.Cancel };
            btnOk.Click += BtnOk_Click;

            Controls.Add(btnOk);
            Controls.Add(btnCancel);
            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }

        private void AddRow(string labelText, Control input, int top)
        {
            var label = new Label { Text = labelText, Left = 15, Top = top + 3, Width = 190 };
            input.Left = 210;
            input.Top = top;
            input.Width = 170;
            Controls.Add(label);
            Controls.Add(input);
        }

        private void LoadVehicles()
        {
            _cmbVehicle.Items.Clear();
            foreach (var vehicle in _vehicleRepository.GetAll())
            {
                _cmbVehicle.Items.Add(vehicle);
            }
            if (_cmbVehicle.Items.Count > 0)
            {
                _cmbVehicle.SelectedIndex = 0;
                _numMileage.Value = ((Vehicle)_cmbVehicle.SelectedItem).CurrentMileage;
            }
        }

        private void LoadClients()
        {
            _cmbClient.Items.Clear();
            foreach (var client in _clientRepository.GetAll())
            {
                _cmbClient.Items.Add(client);
            }
            if (_cmbClient.Items.Count > 0) _cmbClient.SelectedIndex = 0;
        }

        private void BtnNewClient_Click(object sender, EventArgs e)
        {
            using (var dialog = new ClientEditForm())
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    var newId = _clientRepository.Insert(dialog.Client);
                    dialog.Client.Id = newId;
                    LoadClients();
                    _cmbClient.SelectedItem = _cmbClient.Items
                        .Cast<Client>()
                        .FirstOrDefault(c => c.Id == newId);
                }
            }
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

            var available = _availabilityService.IsVehicleAvailable(vehicle.Id, _dtStart.Value, _dtEnd.Value);
            _lblAvailability.Text = available
                ? "Vozilo je dostupno u odabranom terminu."
                : "Vozilo NIJE dostupno u odabranom terminu (rezervirano ili na servisu).";
            _lblAvailability.ForeColor = available
                ? System.Drawing.Color.DarkGreen
                : System.Drawing.Color.Firebrick;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (_cmbVehicle.SelectedItem == null || _cmbClient.SelectedItem == null)
            {
                MessageBox.Show("Odaberite vozilo i klijenta.", "Nepotpuni podaci",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            if (!_availabilityService.IsVehicleAvailable(vehicle.Id, _dtStart.Value, _dtEnd.Value))
            {
                MessageBox.Show(
                    "Odabrano vozilo nije dostupno u traženom terminu (već je rezervirano ili je na servisu).\n" +
                    "Odaberite drugo vozilo ili drugi termin.",
                    "Vozilo nije dostupno", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            Reservation = new Reservation
            {
                VehicleId = vehicle.Id,
                ClientId = ((Client)_cmbClient.SelectedItem).Id,
                EmployeeId = Session.CurrentEmployee.Id,
                StartDate = _dtStart.Value,
                EndDate = _dtEnd.Value,
                RentalType = (RentalType)_cmbRentalType.SelectedItem,
                MileageAtPickup = (int)_numMileage.Value
            };
        }
    }
}
