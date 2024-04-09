using System.IO.Pipes;
using Serilog;
using System;

namespace LoginUI
{
    class LoginUI
    {
        private static readonly ServerHandler sh;
        private static readonly LoginScreen loginScreen;
        // private static readonly LogOutForm logOutForm;
        public static int usageRecordID = -1;
        // Set Global to communicate within sessions
        private const string PIPE_NAME = @"\\.\pipe\Global\LoginSystem_UI";
        // public const int TIMEOUT_MILISEC = 30000;  // half min
        public static ILogger logger;
        private static readonly string LOG_FOLDER = @"/LoginSystem/log/LoginUI";
        static LoginUI()
        {
            string logFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + LOG_FOLDER;
            logger = new LoggerConfiguration()
                .WriteTo.File($"{logFolder}/{{Date}}.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            sh = new ServerHandler(Settings.URI, Settings.ComputerName);
            loginScreen = new LoginScreen(sh);
            // logOutForm = new LogOutForm(sh, screenCloseEvent);
        }

        public static async Task usageRecordID_ReportAsync()
        {
            await sendDataAsync(usageRecordID);
            // logger.Information("Process exited gracefully");
            // Application.Exit();
        }

        private static async Task sendDataAsync(int data)
        {
            logger.Information($"Sending ID: {usageRecordID}");
            NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", PIPE_NAME, PipeDirection.Out, PipeOptions.Asynchronous);
            try
            {
                await pipeClient.ConnectAsync(5000);
                byte[] dataArr = BitConverter.GetBytes(data);
                await pipeClient.WriteAsync(dataArr.AsMemory(0, 4));
                logger.Information($"Data transfered");
                pipeClient.Dispose();
                logger.Information($"ID sent");
            }
            catch
            {
                logger.Error("Pipe not responding!");
            }

        }
        
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            // Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(loginScreen);
            // Application.Run(logOutForm);
        
            
            
            // // XXX: Testing
            // if(usageRecordID == -1)
            // {
            //     logger.Information($"Sending ID: -1");
            //     usageRecordID_ReportAsync().Wait();
            //     logger.Information($"ID sent");
            // }
        }

    }
}
