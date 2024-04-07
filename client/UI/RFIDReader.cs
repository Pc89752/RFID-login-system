using System.Diagnostics;

namespace LoginUI
{

    public class RFIDReader : TableLayoutPanel
    {
        private Label _errorLabel = new Label();
        private string _innerCode = "";
        private string exePath = Settings.NFcCode_path;
        private Button _btnLogin = new Button();
        private readonly bool processExisted;
        private Label _key = new Label();
        private TextBox _txtKey = new TextBox();
        private Process? process;
        private ServerHandler _sh;
        private CancellationTokenSource? cancellationTokenSource;
        private Task? readingTask;

        private ScreenCloseEvent screenCloseEvent;
        
        
        public RFIDReader(ServerHandler sh,ScreenCloseEvent screenCloseEvent)
        {
            this.screenCloseEvent = screenCloseEvent;
            _sh = sh;
            processExisted =
                Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(exePath)).Length>0;
            if(processExisted) LoginUI.logger.Information("Process existed");

            AutoSize = true;
            FlowLayoutPanel panel = new FlowLayoutPanel();

            // AutoSize = true;
            // _stateLabel.Font = new Font("Arial", 24,FontStyle.Bold);
            // _stateLabel.Margin = new Padding(0, 30, 0, 0);
            // _stateLabel.AutoSize = true;
            // _stateLabel.Anchor = AnchorStyles.None;
            // _stateLabel.TextAlign = ContentAlignment.MiddleCenter;
            // _stateLabel.Text = "Waiting";
            // Controls.Add(_stateLabel, 0, 0);

            Controls.Add(panel);
            Controls.Add(_btnLogin);
            _key.Margin = new Padding(0,0,0,0);
            _key.Text = "內碼:";
            _txtKey.PasswordChar = '*';
            _btnLogin.Text = "Submit";
            _btnLogin.Click += onSubmitAsync;
            _txtKey.Width = 800;
            Controls.Add(_key, 0, 0);
            Controls.Add(_txtKey, 0, 1);
            Controls.Add(_btnLogin, 0, 2);

            _errorLabel.AutoSize = true;
            Controls.Add(_errorLabel, 0, 3);
        }

        // TODO: check if the process is killed when current tab is not RFID
        public void stopReading()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
            }

            if(!processExisted && process != null)
            {
                LoginUI.logger.Information("Killing the process");
                // process.Dispose();
                process.Kill();
                process = null;
                LoginUI.logger.Information("Killed the process");
            }
            LoginUI.logger.Information("Stopped Reading");
        }
        public void startReading()
        {
            if(!processExisted && process == null)
            {
                LoginUI.logger.Information("Process started");
                process = new Process();
                process.StartInfo.FileName = exePath;
                process.Start();
            }
            else LoginUI.logger.Information("Process is running");

            cancellationTokenSource = new CancellationTokenSource();
            // readingTask = readInputAsync(cancellationTokenSource.Token);
            LoginUI.logger.Information("Started Reading");
        }

        private async void onSubmitAsync(object? sender, EventArgs e)
        {
            Dictionary<string, object> payload = new Dictionary<string, object>()
            {
                {"innerCode", _txtKey.Text},
            };
            bool isSuccess;
            (isSuccess, _errorLabel.ForeColor, _errorLabel.Text) = await _sh.submitAsync(payload, Settings.RFIDReader_endpoint);
            if(isSuccess) {
                await LoginUI.usageRecordID_ReportAsync();
                screenCloseEvent.OnEvent();
            }
            
        }


        // private void clearInput()
        // {
        //     while(!string.IsNullOrEmpty(Console.ReadLine()));
        // }

        // private async Task readInputAsync(CancellationToken cancellationToken)
        // {            
        //     while (!cancellationToken.IsCancellationRequested)
        //     {
        //         string? inp = await Task.Run(() => Console.ReadLine());
        //         if (inp != null)
        //         {
        //             _innerCode = inp;
        //             _stateLabel.Text = "logging in...";
        //             _stateLabel.ForeColor = Color.Black;
        //             await submitAsync();
        //         }
        //         await Task.Delay(2000, cancellationToken);
        //     }
        // }

        // private async Task submitAsync()
        // {
        //     Dictionary<string, object> payload = new Dictionary<string, object>()
        //     {
        //         {"innerCode", _innerCode}
        //     };
        //     bool isSuccess;
        //     (isSuccess, _stateLabel.ForeColor, _stateLabel.Text) = await _sh.submitAsync(payload, Settings.RFIDReader_endpoint);
        //     if(isSuccess) await LoginUI.usageRecordID_ReportAsync();
        // }
    }
}