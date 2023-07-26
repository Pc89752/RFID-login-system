// using System;
using System.ServiceProcess;
using System.Diagnostics;
using Serilog;
using System.IO.Pipes;

namespace BGService
{
    public class BGService: ServiceBase
    {
        // TODO: Get the Uri of server
        private const string _serverUrl = @"http://127.0.0.1:5000/closeReport/";
        public static ILogger logger;
        // TODO: Get the computer ID
        private const string _computerID = @"MyComputer";
        const string LOG_FOLDER = @"/LoginSystem/log";
        private const string PIPE_NAME = "LoginSystem_UI";
        // private const string exe_path = @"C:\Program Files\LoginSystem\LoginUI.exe";
        private const string exe_path = @"C:\Users\91a04\OneDrive\文件\GitHub\RFID-login-system\LoginSystem\UI\bin\Debug\net7.0-windows\LoginUI.exe";
        private static int usageRecordID = -1;
        private static Thread thread_reciveRecordID = new Thread(reciveReordID);
        static BGService()
        {
            string logFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + LOG_FOLDER;
            logger = new LoggerConfiguration()
                .WriteTo.File($"{logFolder}/{{Date}}.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        // TODO: test pipeline
        protected override void OnStart(string[] args)
        {
            logger.Information("Background starting");
            startUI();
            logger.Information("Background started");
        }

        private static void startUI()
        {
            logger.Information("UI Starting");
            ProcessStartInfo psi = new ProcessStartInfo(exe_path);
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.WorkingDirectory = Path.GetDirectoryName(exe_path);
            psi.EnvironmentVariables["pipe_name"] = PIPE_NAME;

            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo = psi;
                    process.Start();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An exception occurred during starting UI.");
            }
            logger.Information("UI Started");
        }

        private static void reciveReordID()
        {
            Console.WriteLine("Recieving");
            usageRecordID = PipeHandler.ReceiveDataAsync(PIPE_NAME);
            Console.WriteLine($"Recieved: {usageRecordID}");
        }

        protected override void OnStop()
        {
            logger.Information("Background closing");
            if(usageRecordID != -1) returnClosing(usageRecordID);
            cleaning();
            logger.Information("Background closed");
        }

        private static void cleaning()
        {
        }


        private async void returnClosing(int usageRecordID)
        {
            try
            {
                using(var client = new HttpClient())
                {
                    try
                    {
                        await client.PostAsync(_serverUrl, new StringContent(Convert.ToString(usageRecordID)));
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, "An exception occurred during BGService.returnClosing");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An exception occurred during BGService.OnStop");
            }

        }
        
        [STAThread]
        static void Main()
        {
            // startUI();
            thread_reciveRecordID.Start();
            thread_reciveRecordID.Join();
            // XXX: Testing
            // Console.WriteLine(usageRecordID);
        }

    }
}