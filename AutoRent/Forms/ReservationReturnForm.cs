using System;
using System.Windows.Forms;
using AutoRent.Models;
using AutoRent.Repositories;
using AutoRent.Services;

namespace AutoRent.Forms
{
    public partial class ReservationReturnForm : Form
    {
        private readonly Reservation _reservation;
        private readonly Vehicle _vehicle;
        private readonly PricingService _pricingService = new PricingService();

        private readonly NumericUpDown _numMileage = new NumericUpDown();
        private readonly TextBox _txtDamage = new TextBox();
        private readonly Label _lblPrice = new Label();

        public int MileageAtReturn { get; private set; }
        public string DamageDescription { get; private set; }
        public decimal Price { get; private set; }

        public ReservationReturnForm(Reservation reservation, Vehicle vehicle)
        {
            _reservation = reservation;
            _vehicle = vehicle;
            InitializeLayout();
        }

        private void InitializeLayout()
        {
            Text = $"Evidencija povrata - rezervacija #{_reservation.Id}";
            Width = 400;
            Height = 300;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;

            _numMileage.Minimum = _reservation.MileageAtPickup ?? 0;
            _numMileage.Maximum = 2000000;
            _numMileage.Value = _numMileage.Minimum;
            _numMileage.Width = 200;

            _txtDamage.Multiline = true;
            _txtDamage.Height = 60;
            _txtDamage.Width = 340;

            var lblMileage = new Label { Text = "Stanje km pri povratu:", Left = 15, Top = 18, Width = 170 };
            _numMileage.Left = 190;
            _numMileage.Top = 15;

            var lblDamage = new Label { Text = "Opis oštećenja (ako postoji):", Left = 15, Top = 55, Width = 250 };
            _txtDamage.Left = 15;
            _txtDamage.Top = 78;

            var btnCalculate = new Button { Text = "Izračunaj cijenu najma", Left = 15, Top = 150, Width = 200 };
            btnCalculate.Click += (s, e) => CalculatePrice();

            _lblPrice.Left = 15;
            _lblPrice.Top = 185;
            _lblPrice.Width = 340;
            _lblPrice.Font = new System.Drawing.Font(Font, System.Drawing.FontStyle.Bold);

            var btnOk = new Button { Text = "Potvrdi povrat", Left = 90, Top = 220, Width = 110, DialogResult = DialogResult.OK };
            var btnCancel = new Button { Text = "Odustani", Left = 210, Top = 220, Width = 100, DialogResult = DialogResult.Cancel };
            btnOk.Click += BtnOk_Click;

            Controls.Add(lblMileage);
            Controls.Add(_numMileage);
            Controls.Add(lblDamage);
            Controls.Add(_txtDamage);
            Controls.Add(btnCalculate);
            Controls.Add(_lblPrice);
            Controls.Add(btnOk);
            Controls.Add(btnCancel);

            CalculatePrice();
        }

        private void CalculatePrice()
        {
            Price = _pricingService.CalculatePrice(_vehicle, _reservation.StartDate, _reservation.EndDate, _reservation.RentalType);
            _lblPrice.Text = $"Iznos najma: {Price:0.00} €";
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (_numMileage.Value < (_reservation.MileageAtPickup ?? 0))
            {
                MessageBox.Show("Stanje kilometara pri povratu ne može biti manje od stanja pri preuzimanju.",
                    "Neispravan unos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            MileageAtReturn = (int)_numMileage.Value;
            DamageDescription = _txtDamage.Text.Trim();
        }
    }
}
