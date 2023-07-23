using System;
using System.Net.Http;

namespace LoginSystem
{
    public class DevPass : TableLayoutPanel
    {
        private Label _key = new Label();
        private Label _errorLabel = new Label();
        private TextBox _txtKey = new TextBox();
        private Button _btnLogin = new Button();
        private ServerHandler _sh;
        // TODO: change endPoint
        private const string _endPoint = "/submit/devPass";
        public DevPass(ServerHandler sh)
        {
            _sh = sh;

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

            _errorLabel.AutoSize = true;
            Controls.Add(_errorLabel, 0, 3);
        }

        private async void onSubmit(object? sender, EventArgs e)
        {
            Dictionary<string, object> payload = new Dictionary<string, object>()
            {
                {"DEV_TOKEN", _txtKey.Text}
            };
            int status_code = await _sh.submit(payload, _endPoint);

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
                case 4:
                    _errorLabel.ForeColor = Color.Red;
                    _errorLabel.Text = "Invalid token!";
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
        //     mainForm.Controls.Add(new walkway());
        //     Application.Run(mainForm);
        // }
    }
}