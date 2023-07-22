using System;
using System.ServiceProcess;

namespace LoginSystem
{
    public class BGService: ServiceBase
    {
        private const string _closeReportRoute = "/closeReport/";
        private LoginScreen _loginScreen;
        private ServerHandler _sh;
        public BGService()
        {
            // TODO: Get the Uri of server
            _sh = new ServerHandler("http://127.0.0.1:5000/");
            _loginScreen = new LoginScreen(_sh);
        }

        protected override void OnStart(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(_loginScreen);
        }

        protected override void OnStop()
        {
            // Code to be executed when the service stops.
            returnClosing();
            cleaning();
        }

        private void cleaning()
        {
            _loginScreen.cleaning();
        }

        private async void returnClosing()
        {
            string? usageRecordID = ServerHandler.usageRecordID;
            if(usageRecordID == null) return;
            Dictionary<string, object> payload = new Dictionary<string, object>()
            {
                {"usageRecordID", usageRecordID}
            };

            await _sh.submit(payload, _closeReportRoute);
        }
    }

    // public static class LoginSystem
    // {
    //     [STAThread]
    //     static void Main()
    //     {
    //         Application.EnableVisualStyles();
    //         Application.SetCompatibleTextRenderingDefault(false);

    //         // TODO: need an Url of server
    //         LoginScreen loginScreen = new LoginScreen("http://127.0.0.1:5000/");
    //         Application.Run(loginScreen);
    //     }
    // }
}