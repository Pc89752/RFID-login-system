using System;
using System.Drawing;
using System.Windows.Forms;

namespace LoginUI{
    public class LogOutForm : Form
    {
        private Button button1 = new Button() ;

        public LogOutForm()
        {
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
            this.Name = "登出介面";
            this.Text = "登出介面";
            this.ResumeLayout(false);
        }

        private void button1_Click(object? sender, EventArgs e)
        {
            // 登出功能
            this.Close();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // 設定
        }

        
    }
}
