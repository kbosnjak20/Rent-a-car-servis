using System;
using System.Windows.Forms;
using AutoRent.Models;
using AutoRent.Services;

namespace AutoRent.Forms
{
    public partial class ReservationRescheduleForm : Form
    {
        private readonly Reservation _reservation;
        private readonly AvailabilityService _availabilityService = new AvailabilityService();

        private readonly DateTimePicker _dtStart = new DateTimePicker();
        private readonly DateTimePicker _dtEnd = new DateTimePicker();
        private readonly ComboBox _cmbRentalType = new ComboBox();
        private readonly Label _lblAvailability = new Label();

        public DateTime NewStart { get; private set; }
        public DateTime NewEnd { get; private set; }
        public RentalType NewRentalType { get; private set; }

        public ReservationRescheduleForm(Reservation reservation)
        {
            _reservation = reservation;
            InitializeLayout();
        }

        private void InitializeLayout()
        {
            Text = $"Izmjena termina - rezervacija #{_reservation.Id}";
            Width = 380;
            Height = 260;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;

            _dtStart.Format = DateTimePickerFormat.Custom;
            _dtStart.CustomFormat = "dd.MM.yyyy. HH:mm";
            _dtStart.Value = _reservation.StartDate;

            _dtEnd.Format = DateTimePickerFormat.Custom;
            _dtEnd.CustomFormat = "dd.MM.yyyy. HH:mm";
            _dtEnd.Value = _reservation.EndDate;

            _cmbRentalType.DropDownStyle = ComboBoxStyle.DropDownList;
            foreach (RentalType type in Enum.GetValues(typeof(RentalType))) _cmbRentalType.Items.Add(type);
            _cmbRentalType.SelectedItem = _reservation.RentalType;

            _lblAvailability.ForeColor = System.Drawing.Color.Firebrick;
            _lblAvailability.AutoSize = true;

            AddRow("Novi početak:", _dtStart, 15);
            AddRow("Novi kraj:", _dtEnd, 50);
            AddRow("Tip najma:", _cmbRentalType, 85);

            _lblAvailability.Left = 15;
            _lblAvailability.Top = 120;
            _lblAvailability.Width = 340;
            Controls.Add(_lblAvailability);

            var btnOk = new Button { Text = "Spremi", Left = 90, Top = 170, Width = 90, DialogResult = DialogResult.OK };
            var btnCancel = new Button { Text = "Odustani", Left = 190, Top = 170, Width = 90, DialogResult = DialogResult.Cancel };
            btnOk.Click += BtnOk_Click;

            Controls.Add(btnOk);
            Controls.Add(btnCancel);
            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }

        private void AddRow(string labelText, Control input, int top)
        {
            var label = new Label { Text = labelText, Left = 15, Top = top + 3, Width = 140 };
            input.Left = 160;
            input.Top = top;
            input.Width = 190;
            Controls.Add(label);
            Controls.Add(input);
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (_dtStart.Value >= _dtEnd.Value)
            {
                MessageBox.Show("Datum početka mora biti prije datuma završetka.", "Neispravan termin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            var available = _availabilityService.IsVehicleAvailable(
                _reservation.VehicleId, _dtStart.Value, _dtEnd.Value, excludeReservationId: _reservation.Id);

            if (!available)
            {
                MessageBox.Show(
                    "Vozilo nije dostupno u novom terminu (preklapa se s drugom rezervacijom ili servisom).",
                    "Termin nije dostupan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            NewStart = _dtStart.Value;
            NewEnd = _dtEnd.Value;
            NewRentalType = (RentalType)_cmbRentalType.SelectedItem;
        }
    }
}
