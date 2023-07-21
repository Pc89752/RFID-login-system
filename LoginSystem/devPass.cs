using System;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace screen_lock
{
    public class DevPass : TableLayoutPanel
    {
        private Label _key = new Label();
        private Label _errorLabel = new Label();
        private TextBox _txtKey = new TextBox();
        private Button _btnLogin = new Button();
        private string _serverUrl;
        private const string route = "/submit/devPass";
        public DevPass(string serverUri)
        {
            _serverUrl = serverUri + route;

            AutoSize = true;
            FlowLayoutPanel panel = new FlowLayoutPanel();
            panel.FlowDirection = FlowDirection.LeftToRight;
            panel.AutoSize = true;

            panel.Controls.Add(_key);
            panel.Controls.Add(_txtKey);

            Controls.Add(panel);
            Controls.Add(_btnLogin);
            _key.Margin = new Padding(0,0,0,0);
            _key.Text = "金鑰:";
            _txtKey.PasswordChar = '*';
            _btnLogin.Text = "Submit";
            _btnLogin.Click += onSubmit;
            _txtKey.Width = 800;
            Controls.Add(_key, 0, 0);
            Controls.Add(_txtKey, 0, 1);
            // SetColumnSpan(_txtKey, 2);
            Controls.Add(_btnLogin, 0, 2);
            Controls.Add(_errorLabel, 0, 3);
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
                JObject payloadJson =
                    new JObject(
                        new JProperty("DEV_TOKEN", _txtKey.Text)
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
                    int status = Int32.Parse(result);
                    switch (status)
                    {
                        // all normal
                        case 0:
                            _errorLabel.ForeColor = Color.Blue;
                            _errorLabel.Text = "Success!";
                            break;
                        // Invalid token
                        case 1:
                            _errorLabel.ForeColor = Color.Red;
                            _errorLabel.Text = "Invalid token!";
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
        //     mainForm.Controls.Add(new walkway());
        //     Application.Run(mainForm);
        // }
    }
}