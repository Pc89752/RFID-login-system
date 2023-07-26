using System.IO.Pipes;

namespace LoginUI
{
    class LoginUI
    {
        private static ServerHandler sh;
        private static LoginScreen? loginScreen;
        private static string PIPE_NAME;
        static LoginUI()
        {
            sh = new ServerHandler("http://127.0.0.1:5000/", "MyComputer");
            // loginScreen = new LoginScreen(sh);
            // PIPE_NAME = Environment.GetEnvironmentVariable("pipe_name")!;
            PIPE_NAME = "LoginSystem_UI";
        }

        public static void recieveusageRecordID(int usageRecordID)
        {
            // loginScreen.Close();
            sendData(PIPE_NAME, usageRecordID);
        }

        public static void sendData(string pipe_name, int data)
        {
            NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", PIPE_NAME, PipeDirection.Out);
            pipeClient.Connect();
            pipeClient.WriteByte(Convert.ToByte(data));
            pipeClient.Dispose();
        }

        // XXX: Testing
        [STAThread]
        static void Main()
        {
            // Application.EnableVisualStyles();
            // // Application.SetCompatibleTextRenderingDefault(false);
            // Application.Run(loginScreen);
            Console.WriteLine("Sending");
            sendData(PIPE_NAME, 33);
            Console.WriteLine("Sent");
        }

    }
}
