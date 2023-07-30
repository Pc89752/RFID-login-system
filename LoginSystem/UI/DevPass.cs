using System;
using System.Net.Http;

namespace LoginUI
{
    public class DevPass : TableLayoutPanel
    {
        private Label _key = new Label();
        private Label _errorLabel = new Label();
        private TextBox _txtKey = new TextBox();
        private Button _btnLogin = new Button();
        private ServerHandler _sh;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
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
            _btnLogin.Click += onSubmitAsync;
            _txtKey.Width = 800;
            Controls.Add(_key, 0, 0);
            Controls.Add(_txtKey, 0, 1);
            // SetColumnSpan(_txtKey, 2);
            Controls.Add(_btnLogin, 0, 2);

            _errorLabel.AutoSize = true;
            Controls.Add(_errorLabel, 0, 3);
        }

        private async void onSubmitAsync(object? sender, EventArgs e)
        {
            Dictionary<string, object> payload = new Dictionary<string, object>()
            {
                {"DEV_TOKEN", _txtKey.Text}
            };
            bool isSuccess;
            (isSuccess, _errorLabel.ForeColor, _errorLabel.Text) = await _sh.submitAsync(payload, Settings.DevPass_endpoint);
            if(isSuccess) await LoginUI.usageRecordID_ReportAsync();
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