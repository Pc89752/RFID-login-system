using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace screen_lock
{
    public class LoginScreen : Form
    {
        private string? _serverUri;
        private LoginForm? _loginForm;
        private Label _errorLabel = new Label();
        public LoginScreen(string serverUri)
        {
            InitializeComponent(serverUri);
        }

        private void InitializeComponent(string serverUri)
        {
            _serverUri = serverUri;

            this.SuspendLayout();

            this.ControlBox=false;
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;

            TableLayoutPanel loginPanel = new TableLayoutPanel();
            loginPanel.Dock = DockStyle.Top;
            loginPanel.AutoSize = true;
            loginPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            loginPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // adding loginForm
            _loginForm = new LoginForm();
            _loginForm.BtnLogin.Click += onSubmit;
            _loginForm.Anchor = AnchorStyles.None;
            if(Screen.PrimaryScreen!=null)
                _loginForm.Margin = new Padding(0, Screen.PrimaryScreen.Bounds.Height/4, 0, 0);

            // adding errorLabel
            // _errorLabel.Text = "Form cannot be closed manually!";
            _errorLabel.Font = new Font("Arial", 24,FontStyle.Bold);
            _errorLabel.Margin = new Padding(0, 30, 0, 0);
            _errorLabel.AutoSize = true;
            _errorLabel.Anchor = AnchorStyles.None;
            _errorLabel.TextAlign = ContentAlignment.MiddleCenter;

            loginPanel.Controls.Add(_loginForm, 0, 0);
            loginPanel.Controls.Add(_errorLabel, 0, 1);
            loginPanel.SetColumnSpan(_errorLabel, 1);

            Controls.Add(loginPanel);

            // TODO: uncomment this line before officially run
            // this.FormClosing += preventUserClosing;
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void preventUserClosing(object? sender, FormClosingEventArgs e)
        {
            // Check the CloseReason
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                _errorLabel.ForeColor = Color.Red;
                _errorLabel.Text = "Form cannot be closed manually!";
            }
        }

        private void onSubmit(object? sender, EventArgs e)
        {
            if(sender==null || _loginForm == null)
            {
                _errorLabel.ForeColor = Color.Orange;
                _errorLabel.Text = "Major error!";
                return;
            }
            if(_serverUri==null)
            {
                _errorLabel.ForeColor = Color.Orange;
                _errorLabel.Text = "Invalid Uri!";
                return;
            }

            using(var client = new HttpClient())
            {
                // Creating payload Json
                JObject payloadJson =
                    new JObject(
                        new JProperty("account", _loginForm.Username),
                        new JProperty("password", _loginForm.Password)
                    );

                // post login request
                string? result = null;
                try
                {
                    var response = client.PostAsync(_serverUri, new StringContent(payloadJson.ToString()));
                    if(response!=null) result = response.ToString();
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
                    JObject returnJson = JObject.Parse(result);
                    int? status = (int?)returnJson["status"];
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

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // TODO: need an Uri of server
            LoginScreen loginScreen = new LoginScreen("aaa");
            loginScreen.Padding = new Padding(50);
            Application.Run(loginScreen);
        }
    }
}