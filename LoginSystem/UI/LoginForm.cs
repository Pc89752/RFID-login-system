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
            _btnLogin.Click += onSubmit;

            // adding errorLabel
            _errorLabel.Font = new Font("Arial", 24,FontStyle.Bold);
            _errorLabel.Margin = new Padding(0, 30, 0, 0);
            _errorLabel.AutoSize = true;
            _errorLabel.Anchor = AnchorStyles.None;
            _errorLabel.TextAlign = ContentAlignment.MiddleCenter;
        }

        private async void onSubmit(object? sender, EventArgs e)
        {
            Dictionary<string, object> payload = new Dictionary<string, object>()
            {
                {"account", _txtUsername.Text},
                {"password", _txtPassword.Text}
            };
            int status_code = await _sh.submit(payload, Settings.LoginForm_endpoint);

            switch(status_code)
            {
                case -1:
                    _errorLabel.ForeColor = Color.Orange;
                    _errorLabel.Text = "Connect failed!";
                    break;
                case 0:
                    _errorLabel.ForeColor = Color.Blue;
                    _errorLabel.Text = "Success!";
                    break;
                case 1:
                    _errorLabel.ForeColor = Color.Red;
                    _errorLabel.Text = "Invalid username!";
                    break;
                case 2:
                    _errorLabel.ForeColor = Color.Red;
                    _errorLabel.Text = "Invalid password!";
                    break;
                default:
                    Log.log("ERROR", $"status_code: {status_code}", new Exception("status_code out of range"), null);
                    break;
            }
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