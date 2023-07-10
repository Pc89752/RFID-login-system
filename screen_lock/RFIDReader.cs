using System;
using System.Threading;
using System.Management;
using System.Diagnostics;

namespace screen_lock
{
    public class RFIDReader : TableLayoutPanel
    {
        private Label _loadingChar = new Label();
        private int _loadingState = 0;
        private string[] _animationChars = {"-", "/", "|", "\\"};
        private static Thread? _thread;
        public bool _isRunning = false;
        private static string _password = "";
        private string _exePath = @"..\NfcCode\NfcCode.exe";
        private Process? _process;
        public RFIDReader()
        {
            AutoSize = true;
            _loadingChar.Text = _animationChars[_loadingState];
            _loadingChar.Font = new Font("Arial", 72);
            _loadingChar.TextAlign = ContentAlignment.MiddleCenter;
            _loadingChar.AutoSize = true;
            // _loadingChar.Dock = DockStyle.Fill;
            _loadingChar.Margin = new Padding(40);
            Controls.Add(_loadingChar, 0, 0);
        }

        private void spinLoadingChar()
        {
            if(_loadingState==3) _loadingState=0;
            else _loadingState++;

            _loadingChar.Text = _animationChars[_loadingState];
        }

        public void stopReading()
        {
            if(_thread!=null) _thread.Join();
            if(!_isRunning) return;
            if(_process == null) return;
            // if(_process.HasExited) return;
            // Wait for the thread to finish before exiting
            try
            {
                using (Process P = new Process())
                {
                    P.StartInfo = new ProcessStartInfo()
                    {
                        FileName = "taskkill",
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        Arguments = "/F /PID " + _process.Id
                    };
                    P.Start();
                    P.WaitForExit(60000);
                }
            }
            catch
            {
                using (Process P = new Process())
                {
                    P.StartInfo = new ProcessStartInfo()
                    {
                        FileName = "tskill",
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        Arguments = "/PID " + _process.Id + " /A /V"
                    };
                    P.Start();
                    P.WaitForExit(60000);
                }
            }
            _isRunning = false;
            Console.WriteLine("Stopped");
        }

        public void startReading()
        {
            if(_isRunning) return;
            _isRunning = true;
            
            _process = Process.Start(_exePath);
            _process.WaitForInputIdle();
            // _thread = new Thread(readInput);
            // _thread.Start();
            Console.WriteLine("Started");
        }

        private async static void readInput()
        {
            while(true)
            {
                string? inp = await Task.Run(() => Console.ReadLine());
                if(inp!=null)
                {
                    _password = inp;
                    Console.WriteLine(inp);
                }
                Thread.Sleep(1000);
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