using System.Windows.Forms;
using AutoRent.Services;

namespace AutoRent.Forms
{
    public partial class StatisticsForm : Form
    {
        private readonly StatisticsService _statisticsService = new StatisticsService();

        public StatisticsForm()
        {
            InitializeLayout();
        }

        private void InitializeLayout()
        {
            Text = "Statistika korištenja vozila";
            Width = 700;
            Height = 550;
            StartPosition = FormStartPosition.CenterScreen;

            var lblByVehicle = new Label { Text = "Broj rezervacija po vozilu:", Left = 12, Top = 10, Width = 300 };
            var gridByVehicle = new DataGridView
            {
                Left = 12,
                Top = 32,
                Width = 660,
                Height = 220,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                DataSource = _statisticsService.GetReservationCountsByVehicle()
            };

            var lblByCategory = new Label { Text = "Iskorištenost po kategoriji vozila:", Left = 12, Top = 265, Width = 300 };
            var gridByCategory = new DataGridView
            {
                Left = 12,
                Top = 287,
                Width = 660,
                Height = 220,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                DataSource = _statisticsService.GetUtilizationByCategory()
            };

            Controls.Add(lblByVehicle);
            Controls.Add(gridByVehicle);
            Controls.Add(lblByCategory);
            Controls.Add(gridByCategory);
        }
    }
}
