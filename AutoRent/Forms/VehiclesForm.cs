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
        private Button _btnAdd;
        private Button _btnEdit;
        private Button _btnDelete;

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

            _btnAdd = new Button { Text = "Novo vozilo", Left = 660, Top = 11, Width = 100 };
            _btnAdd.Click += BtnAdd_Click;

            _btnEdit = new Button { Text = "Uredi", Left = 770, Top = 11, Width = 100 };
            _btnEdit.Click += BtnEdit_Click;

            _btnDelete = new Button { Text = "Obriši vozilo", Left = 550, Top = 11, Width = 100 };
            _btnDelete.Click += BtnDelete_Click;

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
            Controls.Add(_btnAdd);
            Controls.Add(_btnEdit);
            Controls.Add(_btnDelete);
            Controls.Add(_grid);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (var dialog = new VehicleEditForm())
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    _vehicleRepository.Insert(dialog.Vehicle);
                    LoadVehicles();
                }
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            var selected = _grid.CurrentRow?.DataBoundItem as Vehicle;
            if (selected == null)
            {
                MessageBox.Show("Odaberite vozilo koje želite urediti.", "Nije odabrano vozilo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var dialog = new VehicleEditForm(selected))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    _vehicleRepository.Update(dialog.Vehicle);
                    LoadVehicles();
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            var selected = _grid.CurrentRow?.DataBoundItem as Vehicle;
            if (selected == null)
            {
                MessageBox.Show("Odaberite vozilo koje želite obrisati.", "Nije odabrano vozilo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirm = MessageBox.Show(
                $"Jeste li sigurni da želite obrisati vozilo \"{selected}\"?\n" +
                "Napomena: vozilo se ne može obrisati ako ima povezane rezervacije ili servise.",
                "Potvrda brisanja", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            try
            {
                _vehicleRepository.Delete(selected.Id);
                LoadVehicles();
            }
            catch (System.Data.SqlClient.SqlException)
            {
                MessageBox.Show(
                    "Vozilo nije moguće obrisati jer postoje povezane rezervacije ili servisi.",
                    "Brisanje nije uspjelo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
