using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace LoginSystem
{
    public class LoginScreen : Form
    {
        // private string? _serverUri;
        private ServerHandler _sh;
        private LoginForm _loginForm;
        private RFIDReader _RFID_reader;
        private DevPass _devPass;
        private TabControl tc = new TabControl();
        private int tc_index = 0;
        // private const string startTimeRoute = "/startTime/";
        public LoginScreen(ServerHandler sh)
        {
            _sh = sh;
            _RFID_reader = new RFIDReader(_sh);
            _loginForm = new LoginForm(_sh);
            _devPass = new DevPass(_sh);
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // returnStartTime();
            Padding = new Padding(50);
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
            _RFID_reader.Anchor = AnchorStyles.None;
            RFIDPanel.Controls.Add(_RFID_reader);
            RFIDPanel.Dock = DockStyle.Fill;
            RFIDPanel.AutoSize = true;
            RFIDPage.Controls.Add(RFIDPanel);
            RFIDPage.Font = new Font("Verdana", 12);
            tc.TabPages.Add(RFIDPage);

            // adding loginForm
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
            TableLayoutPanel devPassPanel = new TableLayoutPanel();
            devPassPanel.Dock = DockStyle.Top;
            devPassPanel.AutoSize = true;
            devPassPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            devPassPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            TabPage devPassPage = new TabPage();
            devPassPage.Name = "dev pass";
            devPassPage.Text = "dev pass";
            _devPass.Anchor = AnchorStyles.None;
            devPassPanel.Controls.Add(_devPass);
            devPassPanel.Dock = DockStyle.Fill;
            devPassPanel.AutoSize = true;
            devPassPage.Controls.Add(devPassPanel);
            devPassPage.Font = new Font("Verdana", 12);
            tc.TabPages.Add(devPassPage);

            Controls.Add(tc);


            // TODO: uncomment this line before officially run
            this.FormClosing += cleaning;
            // this.FormClosing += preventUserClosing;

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
                // switch (tc_index)
                // {
                //     case 1:
                //         if(_loginForm!=null) _loginForm.errorMannualClosing();
                //         break;
                // }
            }
        }

        private void tab_indexChanged(object? sender, EventArgs e)
        {
            if(tc_index == 0 && _RFID_reader!=null) _RFID_reader.stopReading();
            tc_index = tc.SelectedIndex;
            if(tc_index == 0 && _RFID_reader!=null) _RFID_reader.startReading();
        }

        private void cleaning(object? sender, FormClosingEventArgs e)
        {
            if(_RFID_reader != null) _RFID_reader.CloseProcess();
        }

        // XXX: Testing
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