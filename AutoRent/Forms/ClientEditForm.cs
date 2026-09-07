using System;
using System.Windows.Forms;
using AutoRent.Models;

namespace AutoRent.Forms
{
    public partial class ClientEditForm : Form
    {
        public Client Client { get; private set; }

        private readonly TextBox _txtFirstName = new TextBox();
        private readonly TextBox _txtLastName = new TextBox();
        private readonly TextBox _txtContact = new TextBox();

        public ClientEditForm()
        {
            Text = "Novi klijent";
            Width = 340;
            Height = 230;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;

            AddRow("Ime:", _txtFirstName, 20);
            AddRow("Prezime:", _txtLastName, 55);
            AddRow("Kontakt (telefon/e-mail):", _txtContact, 90);

            var btnOk = new Button { Text = "Spremi", Left = 60, Top = 140, Width = 90, DialogResult = DialogResult.OK };
            var btnCancel = new Button { Text = "Odustani", Left = 160, Top = 140, Width = 90, DialogResult = DialogResult.Cancel };
            btnOk.Click += BtnOk_Click;

            Controls.Add(btnOk);
            Controls.Add(btnCancel);
            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }

        private void AddRow(string labelText, Control input, int top)
        {
            var label = new Label { Text = labelText, Left = 15, Top = top + 3, Width = 150 };
            input.Left = 150;
            input.Top = top;
            input.Width = 150;
            Controls.Add(label);
            Controls.Add(input);
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_txtFirstName.Text) || string.IsNullOrWhiteSpace(_txtLastName.Text))
            {
                MessageBox.Show("Ime i prezime su obavezni podaci.", "Nepotpuni podaci",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            Client = new Client
            {
                FirstName = _txtFirstName.Text.Trim(),
                LastName = _txtLastName.Text.Trim(),
                Contact = string.IsNullOrWhiteSpace(_txtContact.Text) ? null : _txtContact.Text.Trim()
            };
        }
    }
}
