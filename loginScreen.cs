using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace screen_lock
{
    public class LoginScreen : Form
    {
        private LoginForm? _loginForm;
        private Label _errorLabel = new Label();
        public LoginScreen()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.ControlBox=false;
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;

            TableLayoutPanel loginPanel = new TableLayoutPanel();
            loginPanel.Dock = DockStyle.Top;
            loginPanel.AutoSize = true;
            loginPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            loginPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // adding loginForm
            _loginForm = new LoginForm();
            _loginForm.Anchor = AnchorStyles.None;
            if(Screen.PrimaryScreen!=null)
                _loginForm.Margin = new Padding(0, Screen.PrimaryScreen.Bounds.Height/4, 0, 0);

            // adding errorLabel
            // _errorLabel.Text = "Form cannot be closed manually!";
            _errorLabel.ForeColor = Color.Red;
            _errorLabel.Font = new Font("Arial", 24,FontStyle.Bold);
            _errorLabel.Margin = new Padding(0, 30, 0, 0);
            _errorLabel.AutoSize = true;
            _errorLabel.Anchor = AnchorStyles.None;
            _errorLabel.TextAlign = ContentAlignment.MiddleCenter;

            loginPanel.Controls.Add(_loginForm, 0, 0);
            loginPanel.Controls.Add(_errorLabel, 0, 1);
            loginPanel.SetColumnSpan(_errorLabel, 1);

            Controls.Add(loginPanel);

            // TODO: uncomment this line before officially run
            // this.FormClosing += preventUserClosing;
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void preventUserClosing(object? sender, FormClosingEventArgs e)
        {
            // Check the CloseReason
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                _errorLabel.Text = "Form cannot be closed manually!";
            }
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            LoginScreen loginScreen = new LoginScreen();
            loginScreen.Padding = new Padding(50);
            Application.Run(loginScreen);
        }
    }
}