using System;
using System.Linq;
using System.Windows.Forms;
using AutoRent.Repositories;

namespace AutoRent.Forms
{
    public partial class ServicesForm : Form
    {
        private readonly ServiceRepository _serviceRepository = new ServiceRepository();
        private readonly VehicleRepository _vehicleRepository = new VehicleRepository();

        private DataGridView _grid;

        public ServicesForm()
        {
            InitializeLayout();
            LoadServices();
        }

        private void InitializeLayout()
        {
            Text = "Servisi vozila";
            Width = 750;
            Height = 550;
            StartPosition = FormStartPosition.CenterScreen;

            var btnNew = new Button { Text = "Zakaži servis", Left = 12, Top = 12, Width = 130 };
            btnNew.Click += BtnNew_Click;

            _grid = new DataGridView
            {
                Left = 12,
                Top = 50,
                Width = 710,
                Height = 440,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            Controls.Add(btnNew);
            Controls.Add(_grid);
        }

        private void LoadServices()
        {
            var vehicles = _vehicleRepository.GetAll().ToDictionary(v => v.Id, v => v.ToString());

            var services = _serviceRepository.GetAll().Select(s => new
            {
                s.Id,
                Vozilo = vehicles.ContainsKey(s.VehicleId) ? vehicles[s.VehicleId] : $"#{s.VehicleId}",
                s.StartDate,
                s.EndDate,
                Cijena = s.Price
            }).ToList();

            _grid.DataSource = services;
        }

        private void BtnNew_Click(object sender, EventArgs e)
        {
            var vehicles = _vehicleRepository.GetAll();
            using (var dialog = new ServiceEditForm(vehicles))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    _serviceRepository.Insert(dialog.Service);
                    LoadServices();
                }
            }
        }
    }
}
