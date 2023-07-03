using System;
using System.Threading;
using System.Net.Http;

namespace screen_lock
{
    public class RFIDReader : TableLayoutPanel
    {
        private Label _loadingChar = new Label();
        private int _loadingState = 0;
        private string[] _animationChars = {"-", "/", "|", "\\"};
        private Button _btnLogin = new Button();
        public RFIDReader()
        {
            AutoSize = true;
            _loadingChar.Text = _animationChars[_loadingState];
            _loadingChar.Font = new Font("Arial", 72);
            _loadingChar.TextAlign = ContentAlignment.MiddleCenter;
            _loadingChar.AutoSize = true;
            // _loadingChar.Dock = DockStyle.Fill;
            _loadingChar.Margin = new Padding(40);
            Controls.Add(_loadingChar, 0, 0);
            Controls.Add(_btnLogin, 0, 1);
        }

        private void spinLoadingChar()
        {
            if(_loadingState==3) _loadingState=0;
            else _loadingState++;

            _loadingChar.Text = _animationChars[_loadingState];
        }

        public Button BtnLogin
        {
            get {return _btnLogin;}
        }

        // [STAThread]
        // static void Main()
        // {
        //     Application.EnableVisualStyles();
        //     Application.SetCompatibleTextRenderingDefault(false);
        //     Form mainForm = new Form();
        //     mainForm.FormBorderStyle = FormBorderStyle.FixedDialog;
        //     mainForm.Controls.Add(new RFIDReader());
        //     Application.Run(mainForm);
        // }
    }
}