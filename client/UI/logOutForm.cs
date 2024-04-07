using System;
using System.Drawing;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Text.Json;
namespace LoginUI{
    public class LogOutForm : Form
    {
        private Button button1 = new Button() ;

        private ServerHandler _sh;
        private LoginScreen screen;

        private int usageRecordID;
        
        private readonly static string _serverUrl = Settings.URI + Settings.CloseReport_endpoint;
    // TODO: Get the computer ID
        public LogOutForm(ServerHandler sh,int usageRecordID)
        {
            this.usageRecordID = usageRecordID;
            _sh = sh;
            screen = new LoginScreen(_sh);
           
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "登出";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += button1_Click;
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.button1);
            this.FormClosing += preventUserClosing;
            this.Name = "登出介面";
            this.Text = "登出介面";
            this.ResumeLayout(false);
        }

        private void button1_Click(object? sender, EventArgs e)
        {
            // 登出功能
            try
            {
                using(var client = new HttpClient())
                {
                    Dictionary<string, string> dict = new Dictionary<string, string>()
                    {
                        {"usageRecordID", Convert.ToString(usageRecordID)}
                    };
                    string payload = JsonConvert.SerializeObject(dict);

                    client.PostAsync(_serverUrl, new StringContent(payload)).Wait();
                }
            }
            catch (Exception)
            {
            }
            this.Close();
            
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

        
    }
}
