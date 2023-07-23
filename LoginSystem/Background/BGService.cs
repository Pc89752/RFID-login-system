using System;
using System.ServiceProcess;
using System.Diagnostics;
using System.Threading;
using System.Management;

namespace LoginSystem
{
    public class BGService: ServiceBase
    {
        // TODO: Get the Uri of server
        private const string _serverUrl = "http://127.0.0.1:5000/closeReport/";
        // TODO: Get the computer ID
        private const string _computerID = "MyComputer";
        // set .exe path of UI
        private const string exe_path = @"C:\Users\91a04\OneDrive\文件\GitHub\RFID-login-system\LoginSystem\UI\bin\Debug\net7.0-windows\LoginUI.exe";
        private static int? usageRecordID;
        public BGService()
        {
        }

        protected override void OnStart(string[] args)
        {
            // start the UI
            Process process = new Process();
            process.StartInfo.FileName = exe_path;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();

            // TODO: get the usageRecordID from UI
            process.WaitForExit();
            usageRecordID = process.ExitCode;
        }

        protected override void OnStop()
        {
            // TODO: get usageRecordID
            if(usageRecordID != null) returnClosing(Convert.ToInt32(usageRecordID));
            cleaning();
        }

        private void cleaning()
        {
        }

        private async void returnClosing(int usageRecordID)
        {
            using(var client = new HttpClient())
            {
                try
                {
                    await client.PostAsync(_serverUrl, new StringContent(Convert.ToString(usageRecordID)));
                }
                catch (System.Exception ex)
                {
                    Log.log("ERROR", "Post closing message", ex, null);
                }
            }
        }

        [STAThread]
        static void Main()
        {
            // start the UI
            Process process = new Process();
            process.StartInfo.FileName = exe_path;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();

            // TODO: get the usageRecordID from UI
            process.WaitForExit();
            usageRecordID = process.ExitCode;
            // XXX: Testing
            Console.WriteLine(usageRecordID);
        }
    }
}