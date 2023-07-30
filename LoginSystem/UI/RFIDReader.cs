using System.Diagnostics;

namespace LoginUI
{
    public class RFIDReader : TableLayoutPanel
    {
        private Label _stateLabel = new Label();
        private string _innerCode = "";
        private string exePath = Settings.NFcCode_path;
        private readonly bool processExisted;
        private Process? process;
        private ServerHandler _sh;
        private CancellationTokenSource? cancellationTokenSource;
        private Task? readingTask;

        public RFIDReader(ServerHandler sh)
        {
            _sh = sh;
            processExisted =
                Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(exePath)).Length>0;
            if(processExisted) LoginUI.logger.Information("Process existed");

            AutoSize = true;
            _stateLabel.Font = new Font("Arial", 24,FontStyle.Bold);
            _stateLabel.Margin = new Padding(0, 30, 0, 0);
            _stateLabel.AutoSize = true;
            _stateLabel.Anchor = AnchorStyles.None;
            _stateLabel.TextAlign = ContentAlignment.MiddleCenter;
            _stateLabel.Text = "Waiting";
            Controls.Add(_stateLabel, 0, 0);
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
            readingTask = readInputAsync(cancellationTokenSource.Token);
            LoginUI.logger.Information("Started Reading");
        }

        private async Task readInputAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                string? inp = await Task.Run(() => Console.ReadLine());
                if (inp != null)
                {
                    _innerCode = inp;
                    _stateLabel.Text = "logging in...";
                    _stateLabel.ForeColor = Color.Black;
                    await submitAsync();
                }
                await Task.Delay(2000, cancellationToken);
            }
        }

        private async Task submitAsync()
        {
            Dictionary<string, object> payload = new Dictionary<string, object>()
            {
                {"innerCode", _innerCode}
            };
            bool isSuccess;
            (isSuccess, _stateLabel.ForeColor, _stateLabel.Text) = await _sh.submitAsync(payload, Settings.RFIDReader_endpoint);
            if(isSuccess) await LoginUI.usageRecordID_ReportAsync();
        }
    }
}