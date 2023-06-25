namespace screen_lock
{
    public class LoginForm : TableLayoutPanel
    {
        public LoginForm()
        {
            // Set explicit size for the TableLayoutPanel
            Width = 250;

            Label lblUsername = new Label();
            lblUsername.Text = "Username:";
            Controls.Add(lblUsername, 0, 0);

            TextBox txtUsername = new TextBox();
            Controls.Add(txtUsername, 1, 0);

            Label lblPassword = new Label();
            lblPassword.Text = "Password:";
            Controls.Add(lblPassword, 0, 1);

            TextBox txtPassword = new TextBox();
            txtPassword.PasswordChar = '*';
            Controls.Add(txtPassword, 1, 1);

            Button btnLogin = new Button();
            btnLogin.Text = "Login";
            Controls.Add(btnLogin, 1, 2);
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