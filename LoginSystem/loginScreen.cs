using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Microsoft.Win32;

namespace screen_lock
{
    public class LoginScreen : Form
    {
        private string? _serverUri;
        private LoginForm? _loginForm;
        private static RFIDReader? _RFID_reader;
        TabControl tc = new TabControl();
        private int tc_index = 0;
        private const string startTimeRoute = "/startTime/";
        private const string shutdownReportRoute = "/closeReport/";
        public LoginScreen(string serverUri)
        {
            InitializeComponent(serverUri);
        }

        private void InitializeComponent(string serverUri)
        {
            _serverUri = serverUri;

            // returnStartTime();
            this.SuspendLayout();

            // XXX: testing
            // this.ControlBox=false;
            this.WindowState = FormWindowState.Maximized;
            // this.FormBorderStyle = FormBorderStyle.None;
            // this.TopMost = true;

            // Adding tabs
            tc.Name = "DynamicTabControl";
            tc.Dock = DockStyle.Fill;
            tc.SelectedIndexChanged += tab_indexChanged;

            // Adding RFID to the tab
            TableLayoutPanel RFIDPanel = new TableLayoutPanel();
            RFIDPanel.Dock = DockStyle.Top;
            RFIDPanel.AutoSize = true;
            RFIDPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            RFIDPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            TabPage RFIDPage = new TabPage();
            RFIDPage.Name = "RFID";
            RFIDPage.Text = "RFID";
            _RFID_reader = new RFIDReader(serverUri);
            _RFID_reader.Anchor = AnchorStyles.None;
            RFIDPanel.Controls.Add(_RFID_reader);
            RFIDPanel.Dock = DockStyle.Fill;
            RFIDPanel.AutoSize = true;
            RFIDPage.Controls.Add(RFIDPanel);
            RFIDPage.Font = new Font("Verdana", 12);
            tc.TabPages.Add(RFIDPage);

            // adding loginForm
            _loginForm = new LoginForm(_serverUri);
            _loginForm.Anchor = AnchorStyles.None;
            if(Screen.PrimaryScreen!=null)
                _loginForm.Margin = new Padding(0, Screen.PrimaryScreen.Bounds.Height/4, 0, 0);

            // Adding login page to the tab
            TabPage loginPage = new TabPage();
            loginPage.Name = "loginPage";
            loginPage.Text = "user login";
            loginPage.Controls.Add(_loginForm);
            loginPage.Font = new Font("Verdana", 12);
            tc.TabPages.Add(loginPage);

            // Adding walkway to the tab
            TableLayoutPanel walkwayPanel = new TableLayoutPanel();
            walkwayPanel.Dock = DockStyle.Top;
            walkwayPanel.AutoSize = true;
            walkwayPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            walkwayPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            TabPage walkwayPage = new TabPage();
            walkwayPage.Name = "dev pass";
            walkwayPage.Text = "dev pass";
            DevPass devPass = new DevPass(serverUri);
            devPass.Anchor = AnchorStyles.None;
            walkwayPanel.Controls.Add(devPass);
            walkwayPanel.Dock = DockStyle.Fill;
            walkwayPanel.AutoSize = true;
            walkwayPage.Controls.Add(walkwayPanel);
            walkwayPage.Font = new Font("Verdana", 12);
            tc.TabPages.Add(walkwayPage);

            Controls.Add(tc);


            // TODO: uncomment this line before officially run
            // this.FormClosing += preventUserClosing;

            // TODO: handle program closing: cleaning, returnShutdownTime
            // // SystemEvents.SessionEnding += cleaning;
            // // SystemEvents.SessionEnding += returnShutdownTime;

            // // TODO: delete this 2 line of code before officially run
            // this.FormClosing += cleaning;
            // this.FormClosing += returnShutdownTime;

            this.ResumeLayout(false);
            this.PerformLayout();
            _RFID_reader.startReading();
        }

        private void preventUserClosing(object? sender, FormClosingEventArgs e)
        {
            // Check the CloseReason
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                switch (tc_index)
                {
                    case 1:
                        if(_loginForm!=null) _loginForm.errorMannualClosing();
                        break;
                }
            }
        }

        private async void returnShutdownTime(object? sender, SessionEndingEventArgs e)
        // private async void returnShutdownTime(object? sender, FormClosingEventHandler e)
        {
            bool isShutdown = e.Reason == SessionEndReasons.SystemShutdown;
            string formattedDateTime = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss");
            string serverUrl = _serverUri + shutdownReportRoute;
            using(var client = new HttpClient())
            {
                // Creating payload Json
                JObject payloadJson =
                    new JObject(
                        new JProperty("computerID", 1),
                        new JProperty("shutdownTime", formattedDateTime),
                        new JProperty("isShutdown", isShutdown)
                    );
                try
                {
                    await client.PostAsync(serverUrl, new StringContent(payloadJson.ToString()));
                }
                catch (System.Exception) {}
            }
        }

        // private async void returnStartTime()
        // {
        //     string formattedDateTime = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss");
        //     string serverUrl = _serverUri + startTimeRoute;
        //     using(var client = new HttpClient())
        //     {
        //         // Creating payload Json\
        //         JObject payloadJson =
        //             new JObject(
        //                 new JProperty("computerID", 1),
        //                 new JProperty("startTime", formattedDateTime)
        //             );
        //         try
        //         {
        //             await client.PostAsync(serverUrl, new StringContent(payloadJson.ToString()));
        //         }
        //         catch (System.Exception) {}
        //     }
        // }

        private void cleaning(object? sender, SessionEndingEventArgs e)
        {
            if(_RFID_reader != null) _RFID_reader.CloseProcess();
        }

        private void tab_indexChanged(object? sender, EventArgs e)
        {
            if(tc_index == 0 && _RFID_reader!=null) _RFID_reader.stopReading();
            tc_index = tc.SelectedIndex;
            if(tc_index == 0 && _RFID_reader!=null) _RFID_reader.startReading();
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // TODO: need an Url of server
            LoginScreen loginScreen = new LoginScreen("http://127.0.0.1:5000/");

            loginScreen.Padding = new Padding(50);
            Application.Run(loginScreen);
        }
    }
}