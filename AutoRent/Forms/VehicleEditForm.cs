using System;
using System.Windows.Forms;
using AutoRent.Models;

namespace AutoRent.Forms
{
    public partial class VehicleEditForm : Form
    {
        public Vehicle Vehicle { get; private set; }

        private readonly TextBox _txtName = new TextBox();
        private readonly TextBox _txtBrand = new TextBox();
        private readonly NumericUpDown _numYear = new NumericUpDown();
        private readonly TextBox _txtRegistration = new TextBox();
        private readonly DateTimePicker _dtRegistrationDate = new DateTimePicker();
        private readonly ComboBox _cmbFuelType = new ComboBox();
        private readonly NumericUpDown _numConsumption = new NumericUpDown();
        private readonly ComboBox _cmbCategory = new ComboBox();
        private readonly NumericUpDown _numPricePerHour = new NumericUpDown();
        private readonly NumericUpDown _numPricePerDay = new NumericUpDown();
        private readonly NumericUpDown _numInitialMileage = new NumericUpDown();

        public VehicleEditForm(Vehicle existingVehicle = null)
        {
            Vehicle = existingVehicle;
            InitializeLayout();

            if (existingVehicle != null)
            {
                Text = "Uredi vozilo";
                FillFieldsFrom(existingVehicle);
            }
            else
            {
                Text = "Novo vozilo";
                _dtRegistrationDate.Value = DateTime.Today;
            }
        }

        private void InitializeLayout()
        {
            Width = 420;
            Height = 480;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;

            _numYear.Minimum = 1990;
            _numYear.Maximum = DateTime.Now.Year + 1;
            _numYear.Value = DateTime.Now.Year;

            _cmbFuelType.DropDownStyle = ComboBoxStyle.DropDownList;
            foreach (FuelType fuel in Enum.GetValues(typeof(FuelType))) _cmbFuelType.Items.Add(fuel);
            _cmbFuelType.SelectedIndex = 0;

            _numConsumption.DecimalPlaces = 1;
            _numConsumption.Increment = 0.1M;
            _numConsumption.Maximum = 50;

            _cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            foreach (VehicleCategory category in Enum.GetValues(typeof(VehicleCategory))) _cmbCategory.Items.Add(category);
            _cmbCategory.SelectedIndex = 0;

            _numPricePerHour.DecimalPlaces = 2;
            _numPricePerHour.Maximum = 1000;
            _numPricePerDay.DecimalPlaces = 2;
            _numPricePerDay.Maximum = 5000;
            _numInitialMileage.Maximum = 1000000;

            AddRow("Naziv:", _txtName, 15);
            AddRow("Marka:", _txtBrand, 50);
            AddRow("Godište:", _numYear, 85);
            AddRow("Broj registracije:", _txtRegistration, 120);
            AddRow("Datum registracije:", _dtRegistrationDate, 155);
            AddRow("Tip goriva:", _cmbFuelType, 190);
            AddRow("Prosj. potrošnja (l/100km):", _numConsumption, 225);
            AddRow("Kategorija:", _cmbCategory, 260);
            AddRow("Cijena po satu (€):", _numPricePerHour, 295);
            AddRow("Cijena po danu (€):", _numPricePerDay, 330);
            AddRow("Početno stanje km:", _numInitialMileage, 365);

            var btnOk = new Button { Text = "Spremi", Left = 120, Top = 405, Width = 90, DialogResult = DialogResult.OK };
            var btnCancel = new Button { Text = "Odustani", Left = 220, Top = 405, Width = 90, DialogResult = DialogResult.Cancel };
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

        private void FillFieldsFrom(Vehicle vehicle)
        {
            _txtName.Text = vehicle.Name;
            _txtBrand.Text = vehicle.Brand;
            _numYear.Value = vehicle.Year;
            _txtRegistration.Text = vehicle.RegistrationNumber;
            _dtRegistrationDate.Value = vehicle.RegistrationDate;
            _cmbFuelType.SelectedItem = vehicle.FuelType;
            _numConsumption.Value = (decimal)vehicle.AverageConsumption;
            _cmbCategory.SelectedItem = vehicle.Category;
            _numPricePerHour.Value = vehicle.PricePerHour;
            _numPricePerDay.Value = vehicle.PricePerDay;
            _numInitialMileage.Value = vehicle.InitialMileage;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_txtName.Text) ||
                string.IsNullOrWhiteSpace(_txtBrand.Text) ||
                string.IsNullOrWhiteSpace(_txtRegistration.Text))
            {
                MessageBox.Show("Naziv, marka i broj registracije su obavezni podaci.",
                    "Nepotpuni podaci", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            Vehicle = new Vehicle
            {
                Id = Vehicle?.Id ?? 0,
                Name = _txtName.Text.Trim(),
                Brand = _txtBrand.Text.Trim(),
                Year = (int)_numYear.Value,
                RegistrationNumber = _txtRegistration.Text.Trim(),
                RegistrationDate = _dtRegistrationDate.Value.Date,
                FuelType = (FuelType)_cmbFuelType.SelectedItem,
                AverageConsumption = (double)_numConsumption.Value,
                Category = (VehicleCategory)_cmbCategory.SelectedItem,
                PricePerHour = _numPricePerHour.Value,
                PricePerDay = _numPricePerDay.Value,
                InitialMileage = (int)_numInitialMileage.Value,
                CurrentMileage = Vehicle?.CurrentMileage ?? (int)_numInitialMileage.Value
            };
        }
    }
}
