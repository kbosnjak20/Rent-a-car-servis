using System;
using System.Windows.Forms;
using AutoRent.Models;
using AutoRent.Repositories;

namespace AutoRent.Forms
{
    public partial class VehiclesForm : Form
    {
        private readonly VehicleRepository _vehicleRepository = new VehicleRepository();

        private DataGridView _grid;
        private ComboBox _cmbCategory;
        private Button _btnRefresh;

        public VehiclesForm()
        {
            InitializeLayout();
            LoadVehicles();
        }

        private void InitializeLayout()
        {
            Text = "Vozni park";
            Width = 900;
            Height = 550;
            StartPosition = FormStartPosition.CenterScreen;

            var lblCategory = new Label { Text = "Kategorija:", Left = 12, Top = 15, Width = 70 };

            _cmbCategory = new ComboBox
            {
                Left = 90,
                Top = 12,
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cmbCategory.Items.Add("Sve kategorije");
            foreach (VehicleCategory category in Enum.GetValues(typeof(VehicleCategory)))
            {
                _cmbCategory.Items.Add(category);
            }
            _cmbCategory.SelectedIndex = 0;
            _cmbCategory.SelectedIndexChanged += (s, e) => LoadVehicles();

            _btnRefresh = new Button { Text = "Osvježi", Left = 300, Top = 11, Width = 90 };
            _btnRefresh.Click += (s, e) => LoadVehicles();

            _grid = new DataGridView
            {
                Left = 12,
                Top = 50,
                Width = 860,
                Height = 460,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            Controls.Add(lblCategory);
            Controls.Add(_cmbCategory);
            Controls.Add(_btnRefresh);
            Controls.Add(_grid);
        }

        private void LoadVehicles()
        {
            VehicleCategory? filter = null;
            if (_cmbCategory.SelectedIndex > 0)
            {
                filter = (VehicleCategory)_cmbCategory.SelectedItem;
            }

            var vehicles = _vehicleRepository.GetAll(filter);
            _grid.DataSource = vehicles;
        }
    }
}
