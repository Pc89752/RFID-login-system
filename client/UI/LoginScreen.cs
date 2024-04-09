using System;
using System.Collections.Generic;
using System.ComponentModel;
// using System.Data;
// using System.Drawing;
// using System.Linq;
// using System.Text;
// using System.Windows.Forms;
// using Microsoft.Win32;

namespace LoginUI
{
    public class LoginScreen : Form
    {
        private LoginForm _loginForm;
        private RFIDReader _RFID_reader;
        private DevPass _devPass;
        private TabControl tc = new TabControl();
        private int tc_index = 0;
        private ScreenCloseEvent screenCloseEvent = new ScreenCloseEvent();
        public LoginScreen(ServerHandler sh)
        {
            _RFID_reader = new RFIDReader(sh, screenCloseEvent);
            _loginForm = new LoginForm(sh);
            _devPass = new DevPass(sh, screenCloseEvent);

            screenCloseEvent.Handler += (sender, e) =>
            {
                if (screenCloseEvent.ToLogin) Show();
                else
                {
                    Hide();
                    Task.Run(() => Application.Run(new LogOutForm(sh, screenCloseEvent)));
                }
            };

            InitializeComponent();

        }

        private void InitializeComponent()
        {
            // returnStartTime();
            Padding = new Padding(50);
            this.SuspendLayout();

            // XXX: Uncomment this line during formal operation
            // this.TopMost = true;
            this.ControlBox = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

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
            if (Screen.PrimaryScreen != null)
                _loginForm.Margin = new Padding(0, Screen.PrimaryScreen.Bounds.Height / 4, 0, 0);

            // Adding login page to the tab
            TabPage loginPage = new TabPage();
            loginPage.Name = "loginPage";
            loginPage.Text = "user login";
            loginPage.Controls.Add(_loginForm);
            loginPage.Font = new Font("Verdana", 12);
            tc.TabPages.Add(loginPage);

            // Adding devPass to the tab
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
            this.FormClosing += preventUserClosing;

            this.ResumeLayout(false);
            this.PerformLayout();
            _RFID_reader.startReading();
        }

        private void preventUserClosing(object? sender, FormClosingEventArgs e)
        {
            // Check the CloseReason
            if (e.CloseReason == CloseReason.UserClosing)
            {
                // XXX: Testing
                e.Cancel = true;
            }
        }

        private void tab_indexChanged(object? sender, EventArgs e)
        {
            // Before change
            if (_RFID_reader != null)
            {
                if (tc_index == 0) _RFID_reader.stopReading();
            }
            tc_index = tc.SelectedIndex;

            // After change
            if (_RFID_reader != null)
            {
                if (tc_index == 0) _RFID_reader.startReading();
            }
        }

        private void cleaning(object? sender, FormClosingEventArgs e)
        {
            if (tc.SelectedIndex == 0) _RFID_reader.stopReading();
        }
    }

    public class ScreenCloseEvent
    {
        private bool toLogin = true;
        public bool ToLogin
        {
            get { return toLogin; }
        }
        public event EventHandler? Handler;
        public void ShowLoginForm()
        {
            toLogin = true;
            OnEvent();
        }
        public void HideLoginForm()
        {
            toLogin = false;
            OnEvent();
        }
        public virtual void OnEvent()
        {
            Handler?.Invoke(this, new EventArgs());
        }
    }
}