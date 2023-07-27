using System;
using System.Threading;
using System.Management;
using System.Diagnostics;

namespace LoginUI
{
    public class RFIDReader : TableLayoutPanel
    {
        private Label _stateLabel = new Label();
        private Thread? _thread;
        private string _innerCode = "";
        private string _exePath = Settings.NFcCode_path;
        private Process? _process;
        private ServerHandler _sh;
        private CancellationTokenSource? cancellationTokenSource;
        private Task? readingTask;
        private bool _processRunning = false;
        public RFIDReader(ServerHandler sh)
        {
            _sh = sh;

            AutoSize = true;
            _stateLabel.Font = new Font("Arial", 24,FontStyle.Bold);
            _stateLabel.Margin = new Padding(0, 30, 0, 0);
            _stateLabel.AutoSize = true;
            _stateLabel.Anchor = AnchorStyles.None;
            _stateLabel.TextAlign = ContentAlignment.MiddleCenter;
            _stateLabel.Text = "Waiting";
            Controls.Add(_stateLabel, 0, 0);
        }
        public void stopReading()
        {
            if(_thread!=null) _thread.Join();
            Console.WriteLine(_processRunning);
            if(_processRunning && _process != null)
            {
                // Wait for the thread to finish before exiting
                CloseProcess();
                _processRunning = false;
            }
            Console.WriteLine("Stopped Reading");
        }

        public void CloseProcess()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
                _processRunning = false;
                Console.WriteLine("Stopped Reading");
            }
        }
        public void startReading()
        {
            if (_processRunning) return;

            _process = Process.Start(_exePath);
            _processRunning = true;
            cancellationTokenSource = new CancellationTokenSource();
            readingTask = readInputAsync(cancellationTokenSource.Token);
            Console.WriteLine("Started Reading");
        }

        private async Task readInputAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                string? inp = await Task.Run(() => Console.ReadLine());
                if (inp != null)
                {
                    _innerCode = inp;
                    Console.WriteLine(inp);
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
            (_stateLabel.ForeColor, _stateLabel.Text) = await _sh.submitAsync(payload, Settings.RFIDReader_endpoint);
            await LoginUI.usageRecordID_loginAsync();
        }
    }
}