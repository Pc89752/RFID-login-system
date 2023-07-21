using System;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace screen_lock
{
    public class LoginForm : TableLayoutPanel
    {
        private Label _lblUsername = new Label();
        private TextBox _txtUsername = new TextBox();
        private Label _lblPassword = new Label();
        private TextBox _txtPassword = new TextBox();
        private Button _btnLogin = new Button();
        private Label _errorLabel = new Label();
        private string _serverUrl;
        private const string route = "/submit/account_login";
        public LoginForm(string serverUri)
        // public LoginForm()
        {
            _serverUrl = serverUri + route;
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

        public void errorMannualClosing()
        {
            _errorLabel.ForeColor = Color.Red;
            _errorLabel.Text = "Form cannot be closed manually!";
        }

        private async void onSubmit(object? sender, EventArgs e)
        {
            if(_serverUrl==null)
            {
                _errorLabel.ForeColor = Color.Orange;
                _errorLabel.Text = "Invalid Uri!";
                return;
            }

            using(var client = new HttpClient())
            {
                // Creating payload Json
                string formattedDateTime = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss");
                JObject payloadJson =
                    new JObject(
                        new JProperty("account", _txtUsername.Text),
                        new JProperty("password", _txtPassword.Text),
                        new JProperty("computerID", 1),
                        new JProperty("loginTime", formattedDateTime)
                    );

                // post login request
                string? result = null;
                try
                {
                    var response = await client.PostAsync(_serverUrl, new StringContent(payloadJson.ToString()));
                    if(response!=null) result = await response.Content.ReadAsStringAsync();
                }
                catch (System.Exception)
                {
                    _errorLabel.ForeColor = Color.Orange;
                    _errorLabel.Text = "Connect failed!";
                    return;
                }

                // handle response
                // JObject returnJson = new JObject();
                if(result != null)
                {
                    // JObject returnJson = JObject.Parse(result);
                    // int? status = (int?)returnJson["status"];
                    int status = Int32.Parse(result);
                    switch (status)
                    {
                        // all normal
                        case 0:
                            _errorLabel.ForeColor = Color.Blue;
                            _errorLabel.Text = "Success!";
                            break;
                        // Invalid username
                        case 1:
                            _errorLabel.ForeColor = Color.Red;
                            _errorLabel.Text = "Invalid username!";
                            break;
                        // Invalid password
                        case 2:
                            _errorLabel.ForeColor = Color.Red;
                            _errorLabel.Text = "Invalid password!";
                            break;
                    }
                }
                else
                {
                    _errorLabel.ForeColor = Color.Orange;
                    _errorLabel.Text = "Connect failed!";
                }
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