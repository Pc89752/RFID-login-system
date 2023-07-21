using System;
using System.Threading;
using System.Management;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace screen_lock
{
    public class RFIDReader : TableLayoutPanel
    {
        private static Label _stateLabel = new Label();
        private static Thread? _thread;
        private static string _innerCode = "";
        private string _exePath = @"..\NfcCode\NfcCode.exe";
        private Process? _process;
        private static string? _serverUrl;
        private const string route = "/submit/innerCode_login";
        private bool _processRunning = false;
        public RFIDReader(string serverUri)
        {
            _serverUrl = serverUri + route;

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
            try
            {
                _process.CloseMainWindow(); // Close the main window (e.g., send close signal to a regular application)
                if (!_process.WaitForExit(5000)) _process.Kill();
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
            // check if the process successfully started
            if(_process.HasExited)
            {
                _process.WaitForInputIdle();
                _processRunning = true;
            }
            _thread = new Thread(readInput);
            _thread.Start();
            Console.WriteLine("Started Reading");
        }

        private async static void readInput()
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
                    onSubmit();
                }
                Thread.Sleep(2000);
            }
        }

        // private async void onSubmit(object? sender, EventArgs e)
        private static async void onSubmit()
        {
            if(_serverUrl==null)
            {
                _stateLabel.ForeColor = Color.Orange;
                _stateLabel.Text = "Invalid Uri!";
                return;
            }

            using(var client = new HttpClient())
            {
                // Creating payload Json
                string formattedDateTime = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss");
                JObject payloadJson =
                    new JObject(
                        new JProperty("innerCode", _innerCode),
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
                    _stateLabel.ForeColor = Color.Orange;
                    _stateLabel.Text = "Connect failed!";
                    return;
                }

                // handle response
                if(result != null)
                {
                    int status = Int32.Parse(result);
                    switch (status)
                    {
                        // all normal
                        case 0:
                            _stateLabel.ForeColor = Color.Blue;
                            _stateLabel.Text = "Success!";
                            break;
                        // Invalid username
                        case 1:
                            _stateLabel.ForeColor = Color.Red;
                            _stateLabel.Text = "Invalid username!";
                            break;
                        // Invalid password
                        case 2:
                            _stateLabel.ForeColor = Color.Red;
                            _stateLabel.Text = "Invalid password!";
                            break;
                    }
                }
                else
                {
                    _stateLabel.ForeColor = Color.Orange;
                    _stateLabel.Text = "Connect failed!";
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
        //     mainForm.Controls.Add(new RFIDReader())1106173973

        //     Application.Run(mainForm);
        // }
    }
}