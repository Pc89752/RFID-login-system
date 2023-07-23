using System;
using System.Threading;
using System.Management;
using System.Diagnostics;

namespace LoginSystem
{
    public class RFIDReader : TableLayoutPanel
    {
        private Label _stateLabel = new Label();
        private Thread? _thread;
        private string _innerCode = "";
        private string _exePath = @"C:\Program Files\NfcCode\NfcCode.exe";
        private Process? _process;
        private ServerHandler _sh;
        private const string _endPoint = "/submit/innerCode_login";
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
            if(_process == null) return;
            Console.WriteLine($"Closing Process {_process.ProcessName}.");
            try
            {
                _process.Kill();
                Console.WriteLine($"Process {_process.ProcessName} closed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while closing the process: {ex.Message}");
            }
        }
        public void startReading()
        {
            if(_processRunning) return;
            
            _process = Process.Start(_exePath);
            _processRunning = true;
            _thread = new Thread(() => readInput());
            _thread.Start();
            Console.WriteLine("Started Reading");
        }

        private async void readInput()
        {
            while(true)
            {
                string? inp = await Task.Run(() => Console.ReadLine());
                if(inp!=null)
                {
                    _innerCode = inp;
                    Console.WriteLine(inp);
                    _stateLabel.Text = "loging in...";
                    _stateLabel.ForeColor = Color.Black;
                    // await submit(sh);
                    submit();
                }
                Thread.Sleep(2000);
            }
        }

        private async void submit()
        {
            Dictionary<string, object> payload = new Dictionary<string, object>()
            {
                {"innerCode", _innerCode}
            };
            int status_code = await _sh.submit(payload, _endPoint);

            switch(status_code)
            {
                case -1:
                    _stateLabel.ForeColor = Color.Orange;
                    _stateLabel.Text = "Connect failed!";
                    break;
                case 0:
                    _stateLabel.ForeColor = Color.Blue;
                    _stateLabel.Text = "Success!";
                    break;
                case 3:
                    _stateLabel.ForeColor = Color.Red;
                    _stateLabel.Text = "Invalid innerCode!";
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
        //     mainForm.Controls.Add(new RFIDReader())1106173973

        //     Application.Run(mainForm);
        // }
    }
}