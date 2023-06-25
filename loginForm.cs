using System;
using System.Net.Http;

namespace screen_lock
{
    public class LoginForm : TableLayoutPanel
    {
        private Label _lblUsername = new Label();
        private TextBox _txtUsername = new TextBox();
        private Label _lblPassword = new Label();
        private TextBox _txtPassword = new TextBox();
        private Button _btnLogin = new Button();
        public LoginForm()
        {
            AutoSize = true;

            _lblUsername.Text = "Username:";
            _lblPassword.Text = "Password:";
            _txtPassword.PasswordChar = '*';
            _btnLogin.Text = "Submit";

            Controls.Add(_lblUsername, 0, 0);
            Controls.Add(_lblPassword, 0, 1);
            Controls.Add(_txtUsername, 1, 0);
            Controls.Add(_txtPassword, 1, 1);
            Controls.Add(_btnLogin, 1, 2);
        }

        public Button BtnLogin
        {
            get {return _btnLogin;}
        }

        public String Username
        {
            get {return _txtUsername.Text;}
        }

        public String Password
        {
            get {return _txtPassword.Text;}
        }
        

        // [STAThread]
        // static void Main()
        // {
        //     Application.EnableVisualStyles();
        //     Application.SetCompatibleTextRenderingDefault(false);
        //     Form mainForm = new Form();
        //     mainForm.FormBorderStyle = FormBorderStyle.FixedDialog;
        //     mainForm.Controls.Add(new LoginForm());
        //     Application.Run(mainForm);
        // }
    }
}