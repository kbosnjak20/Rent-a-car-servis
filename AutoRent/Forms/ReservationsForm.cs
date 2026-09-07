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
        private ComboBox _cmbVehicleFilter;
        private ComboBox _cmbClientFilter;
        private ComboBox _cmbStatusFilter;
        private Button _btnSearch;
        private Button _btnNew;

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
            Height = 600;
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

            _grid = new DataGridView
            {
                Left = 12,
                Top = 50,
                Width = 960,
                Height = 500,
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
