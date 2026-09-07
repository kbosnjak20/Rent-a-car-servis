using System;
using System.Linq;
using System.Windows.Forms;
using AutoRent.Models;
using AutoRent.Repositories;

namespace AutoRent.Forms
{
    public partial class ReservationsForm : Form
    {
        private readonly ReservationRepository _reservationRepository = new ReservationRepository();
        private readonly VehicleRepository _vehicleRepository = new VehicleRepository();
        private readonly ClientRepository _clientRepository = new ClientRepository();

        private DataGridView _grid;
        private System.Collections.Generic.List<Reservation> _currentReservations = new System.Collections.Generic.List<Reservation>();
        private ComboBox _cmbVehicleFilter;
        private ComboBox _cmbClientFilter;
        private ComboBox _cmbStatusFilter;
        private Button _btnSearch;
        private Button _btnNew;
        private Button _btnReschedule;
        private Button _btnReturn;
        private Button _btnCancelOrDelete;

        public ReservationsForm()
        {
            InitializeLayout();
            LoadFilterData();
            Search();
        }

        private void InitializeLayout()
        {
            Text = "Rezervacije vozila";
            Width = 1000;
            Height = 680;
            StartPosition = FormStartPosition.CenterScreen;

            var lblVehicle = new Label { Text = "Vozilo:", Left = 12, Top = 15, Width = 50 };
            _cmbVehicleFilter = new ComboBox { Left = 65, Top = 12, Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };

            var lblClient = new Label { Text = "Klijent:", Left = 255, Top = 15, Width = 50 };
            _cmbClientFilter = new ComboBox { Left = 305, Top = 12, Width = 160, DropDownStyle = ComboBoxStyle.DropDownList };

            var lblStatus = new Label { Text = "Status:", Left = 475, Top = 15, Width = 50 };
            _cmbStatusFilter = new ComboBox { Left = 525, Top = 12, Width = 140, DropDownStyle = ComboBoxStyle.DropDownList };
            _cmbStatusFilter.Items.Add("Svi statusi");
            foreach (ReservationStatus status in Enum.GetValues(typeof(ReservationStatus))) _cmbStatusFilter.Items.Add(status);
            _cmbStatusFilter.SelectedIndex = 0;

            _btnSearch = new Button { Text = "Pretraži", Left = 675, Top = 11, Width = 90 };
            _btnSearch.Click += (s, e) => Search();

            _btnNew = new Button { Text = "Nova rezervacija", Left = 860, Top = 11, Width = 120 };
            _btnNew.Click += BtnNew_Click;

            _btnReschedule = new Button
            {
                Text = "Uredi termin", Left = 12, Top = 585, Width = 130, Height = 32,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            _btnReschedule.Click += BtnReschedule_Click;

            _btnReturn = new Button
            {
                Text = "Evidentiraj povrat", Left = 150, Top = 585, Width = 140, Height = 32,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            _btnReturn.Click += BtnReturn_Click;

            _btnCancelOrDelete = new Button
            {
                Text = "Otkaži/Obriši", Left = 300, Top = 585, Width = 130, Height = 32,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            _btnCancelOrDelete.Click += BtnCancelOrDelete_Click;

            _grid = new DataGridView
            {
                Left = 12,
                Top = 50,
                Width = 960,
                Height = 520,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            Controls.Add(lblVehicle);
            Controls.Add(_cmbVehicleFilter);
            Controls.Add(lblClient);
            Controls.Add(_cmbClientFilter);
            Controls.Add(lblStatus);
            Controls.Add(_cmbStatusFilter);
            Controls.Add(_btnSearch);
            Controls.Add(_btnNew);
            Controls.Add(_btnReschedule);
            Controls.Add(_btnReturn);
            Controls.Add(_btnCancelOrDelete);
            Controls.Add(_grid);
        }

        private void LoadFilterData()
        {
            _cmbVehicleFilter.Items.Add("Sva vozila");
            foreach (var vehicle in _vehicleRepository.GetAll()) _cmbVehicleFilter.Items.Add(vehicle);
            _cmbVehicleFilter.SelectedIndex = 0;

            _cmbClientFilter.Items.Add("Svi klijenti");
            foreach (var client in _clientRepository.GetAll()) _cmbClientFilter.Items.Add(client);
            _cmbClientFilter.SelectedIndex = 0;
        }

        private void Search()
        {
            int? vehicleId = _cmbVehicleFilter.SelectedIndex > 0
                ? ((Vehicle)_cmbVehicleFilter.SelectedItem).Id
                : (int?)null;

            int? clientId = _cmbClientFilter.SelectedIndex > 0
                ? ((Client)_cmbClientFilter.SelectedItem).Id
                : (int?)null;

            ReservationStatus? status = _cmbStatusFilter.SelectedIndex > 0
                ? (ReservationStatus)_cmbStatusFilter.SelectedItem
                : (ReservationStatus?)null;

            var reservations = _reservationRepository.Search(vehicleId, clientId, status);
            _currentReservations = reservations;

            _grid.DataSource = reservations.Select(r => new
            {
                r.Id,
                Vozilo = r.VehicleDisplay,
                Klijent = r.ClientDisplay,
                Pocetak = r.StartDate,
                Zavrsetak = r.EndDate,
                r.RentalType,
                r.Status,
                Cijena = r.RentalPrice
            }).ToList();
        }

        private Reservation GetSelectedReservation()
        {
            var index = _grid.CurrentRow?.Index;
            if (index == null || index < 0 || index >= _currentReservations.Count) return null;
            return _currentReservations[index.Value];
        }

        private void BtnCancelOrDelete_Click(object sender, EventArgs e)
        {
            var reservation = GetSelectedReservation();
            if (reservation == null)
            {
                MessageBox.Show("Odaberite rezervaciju koju želite otkazati/obrisati.", "Nije odabrana rezervacija",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (reservation.Status != ReservationStatus.Aktivna)
            {
                MessageBox.Show("Otkazati/obrisati je moguće samo aktivnu rezervaciju.", "Nije moguće",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var alreadyStarted = reservation.StartDate <= DateTime.Now;

            if (!alreadyStarted)
            {
                var confirm = MessageBox.Show(
                    "Rezervacija još nije započela - može se trajno obrisati. Obrisati odabranu rezervaciju?",
                    "Potvrda brisanja", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm != DialogResult.Yes) return;

                _reservationRepository.Delete(reservation.Id);
            }
            else
            {
                var confirm = MessageBox.Show(
                    "Vozilo je po ovoj rezervaciji već preuzeto (najam je u tijeku) - rezervacija se ne može obrisati, " +
                    "nego samo otkazati. Otkazati odabranu rezervaciju?",
                    "Potvrda otkazivanja", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm != DialogResult.Yes) return;

                _reservationRepository.Cancel(reservation.Id);
            }

            Search();
        }

        private void BtnReschedule_Click(object sender, EventArgs e)
        {
            var reservation = GetSelectedReservation();
            if (reservation == null)
            {
                MessageBox.Show("Odaberite rezervaciju koju želite urediti.", "Nije odabrana rezervacija",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (reservation.Status != ReservationStatus.Aktivna)
            {
                MessageBox.Show("Termin je moguće mijenjati samo za aktivne rezervacije.", "Nije moguće urediti",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var dialog = new ReservationRescheduleForm(reservation))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    _reservationRepository.UpdateSchedule(reservation.Id, dialog.NewStart, dialog.NewEnd, dialog.NewRentalType);
                    Search();
                }
            }
        }

        private void BtnReturn_Click(object sender, EventArgs e)
        {
            var reservation = GetSelectedReservation();
            if (reservation == null)
            {
                MessageBox.Show("Odaberite rezervaciju za koju evidentirate povrat.", "Nije odabrana rezervacija",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (reservation.Status != ReservationStatus.Aktivna)
            {
                MessageBox.Show("Povrat je moguće evidentirati samo za aktivne rezervacije.", "Nije moguće evidentirati",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var vehicle = _vehicleRepository.GetById(reservation.VehicleId);

            using (var dialog = new ReservationReturnForm(reservation, vehicle))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    _reservationRepository.RecordReturn(reservation.Id, dialog.MileageAtReturn, dialog.DamageDescription, dialog.Price);
                    _vehicleRepository.UpdateMileage(vehicle.Id, dialog.MileageAtReturn);
                    Search();
                }
            }
        }

        private void BtnNew_Click(object sender, EventArgs e)
        {
            using (var dialog = new ReservationEditForm())
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    _reservationRepository.Insert(dialog.Reservation);
                    LoadFilterData();
                    Search();
                }
            }
        }
    }
}
