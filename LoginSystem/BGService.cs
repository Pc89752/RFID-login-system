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
            // TODO: Get the computer ID
            _sh = new ServerHandler("http://127.0.0.1:5000/", "MyComputer");
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
            // TODO: check if usageRecordID normal
            string? usageRecordID = _sh.usageRecordID;
            if(usageRecordID == null) return;
            Dictionary<string, object> payload = new Dictionary<string, object>()
            {
                {"usageRecordID", usageRecordID}
            };

            await _sh.submit(payload, _closeReportRoute);
        }
    }

    // XXX: Testing
    public class LoginSystem
    {
        [STAThread]
        static void Main()
        {
            ServerHandler sh = new ServerHandler("http://127.0.0.1:5000/", "MyComputer");
            LoginScreen loginScreen = new LoginScreen(sh);
            Application.EnableVisualStyles();
            // Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(loginScreen);
        }
    }
}