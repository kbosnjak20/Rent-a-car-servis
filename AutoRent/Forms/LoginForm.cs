using System;
using System.Windows.Forms;
using AutoRent.Repositories;

namespace AutoRent.Forms
{
    public partial class LoginForm : Form
    {
        private readonly EmployeeRepository _employeeRepository = new EmployeeRepository();

        private readonly TextBox _txtUsername = new TextBox();
        private readonly TextBox _txtPassword = new TextBox();
        private readonly Label _lblError = new Label();

        public LoginForm()
        {
            InitializeLayout();
        }

        private void InitializeLayout()
        {
            Text = "AutoRent - prijava";
            Width = 360;
            Height = 240;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = false;
            MinimizeBox = false;

            var lblUsername = new Label { Text = "Korisničko ime:", Left = 20, Top = 25, Width = 150 };
            _txtUsername.Left = 20;
            _txtUsername.Top = 50;
            _txtUsername.Width = 300;

            var lblPassword = new Label { Text = "Lozinka:", Left = 20, Top = 85, Width = 150 };
            _txtPassword.Left = 20;
            _txtPassword.Top = 110;
            _txtPassword.Width = 300;
            _txtPassword.PasswordChar = '•';

            _lblError.Left = 20;
            _lblError.Top = 140;
            _lblError.Width = 300;
            _lblError.ForeColor = System.Drawing.Color.Firebrick;

            var btnLogin = new Button { Text = "Prijava", Left = 20, Top = 165, Width = 300, DialogResult = DialogResult.OK };
            btnLogin.Click += BtnLogin_Click;

            Controls.Add(lblUsername);
            Controls.Add(_txtUsername);
            Controls.Add(lblPassword);
            Controls.Add(_txtPassword);
            Controls.Add(_lblError);
            Controls.Add(btnLogin);

            AcceptButton = btnLogin;
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            var employee = _employeeRepository.Authenticate(_txtUsername.Text.Trim(), _txtPassword.Text);

            if (employee == null)
            {
                _lblError.Text = "Neispravno korisničko ime ili lozinka.";
                DialogResult = DialogResult.None;
                return;
            }

            Session.CurrentEmployee = employee;
            DialogResult = DialogResult.OK;
        }
    }
}
