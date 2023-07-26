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

        public static async void recieve_usageRecordID(int usageRecordID)
        {
            // loginScreen.Close();
            await sendData(PIPE_NAME, usageRecordID);
        }

        public static async Task sendData(string pipe_name, int data)
        {
            NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", PIPE_NAME, PipeDirection.Out, PipeOptions.Asynchronous);
            // pipeClient.Connect();
            // pipeClient.WriteByte(Convert.ToByte(data));
            // pipeClient.Dispose();
            await pipeClient.ConnectAsync();
            pipeClient.WriteByte(Convert.ToByte(data));
            await pipeClient.DisposeAsync();
        }

        // XXX: Testing
        [STAThread]
        static async Task Main()
        {
            // Application.EnableVisualStyles();
            // // Application.SetCompatibleTextRenderingDefault(false);
            // Application.Run(loginScreen);
            Console.WriteLine("Sending");
            await sendData(PIPE_NAME, 33);
            Console.WriteLine("Sent");
        }

    }
}
