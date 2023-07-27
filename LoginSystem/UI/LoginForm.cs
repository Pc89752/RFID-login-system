using System;
using System.Net.Http;

namespace LoginUI
{
    public class LoginForm : TableLayoutPanel
    {
        private Label _lblUsername = new Label();
        private TextBox _txtUsername = new TextBox();
        private Label _lblPassword = new Label();
        private TextBox _txtPassword = new TextBox();
        private Button _btnLogin = new Button();
        private Label _errorLabel = new Label();
        private ServerHandler _sh;
        public LoginForm(ServerHandler sh)
        {
            _sh = sh;
            AutoSize = true;

            _lblUsername.Text = "Username:";
            _lblPassword.Text = "Password:";
            _txtPassword.PasswordChar = '*';
            _btnLogin.Text = "Submit";

            Controls.Add(_lblUsername, 0, 0);
            Controls.Add(_lblPassword, 0, 1);
            Controls.Add(_txtUsername, 1, 0);
            Controls.Add(_txtPassword, 1, 1);
            Controls.Add(_btnLogin, 1, 2);
            Controls.Add(_errorLabel, 0, 3);
            SetColumnSpan(_errorLabel, 2);

            // onsubmit
            _btnLogin.Click += onSubmitAsync;

            // adding errorLabel
            _errorLabel.Font = new Font("Arial", 24,FontStyle.Bold);
            _errorLabel.Margin = new Padding(0, 30, 0, 0);
            _errorLabel.AutoSize = true;
            _errorLabel.Anchor = AnchorStyles.None;
            _errorLabel.TextAlign = ContentAlignment.MiddleCenter;
        }

        private async void onSubmitAsync(object? sender, EventArgs e)
        {
            Dictionary<string, object> payload = new Dictionary<string, object>()
            {
                {"account", _txtUsername.Text},
                {"password", _txtPassword.Text}
            };
            (_errorLabel.ForeColor, _errorLabel.Text) = await _sh.submitAsync(payload, Settings.LoginForm_endpoint);
            await LoginUI.usageRecordID_loginAsync();
        }

        // [STAThread]
        // static void Main()
        // {
        //     Application.EnableVisualStyles();
        //     Application.SetCompatibleTextRenderingDefault(false);
        //     Form mainForm = new Form();
        //     mainForm.FormBorderStyle = FormBorderStyle.FixedDialog;
        //     mainForm.Controls.Add(new LoginForm());
        //     Application.Run(mainForm);
        // }
    }
}