using System.Windows.Forms;
using AutoRent.Repositories;

namespace AutoRent.Forms
{
    public partial class ClientsForm : Form
    {
        private readonly ClientRepository _clientRepository = new ClientRepository();

        public ClientsForm()
        {
            Text = "Klijenti";
            Width = 500;
            Height = 400;
            StartPosition = FormStartPosition.CenterScreen;

            var grid = new DataGridView
            {
                Left = 12,
                Top = 12,
                Width = 460,
                Height = 340,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                DataSource = _clientRepository.GetAll()
            };

            Controls.Add(grid);
        }
    }
}