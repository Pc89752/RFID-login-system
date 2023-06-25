using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

public class Form1 : Form
{
    private TextBox accountTxtBox = new System.Windows.Forms.TextBox();
    private TextBox passwordTxtBox = new System.Windows.Forms.TextBox();

    public Form1()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();

        this.ClientSize = new System.Drawing.Size(284, 264);
        // this.Controls.Add(this.passwordTxtBox);
        this.ControlBox=false;
        this.WindowState = FormWindowState.Maximized;
        this.FormBorderStyle = FormBorderStyle.None;
        this.TopMost = true;

        (var cw, var ch) = (Screen.PrimaryScreen.Bounds.Width/2, Screen.PrimaryScreen.Bounds.Height/2);
        this.accountTxtBox.Location = new Point(cw, ch);
        this.accountTxtBox.Width = cw/2;
        this.Controls.Add(this.accountTxtBox);
        // this.FormClosing += preventUserClosing;
        // this.Text = "TextBox Example";
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private void preventUserClosing(object? sender, FormClosingEventArgs e)
    {
        // Check the CloseReason to determine if it's a manual close
        if (e.CloseReason == CloseReason.UserClosing)
        {
            // Cancel the close operation
            e.Cancel = true;
            
            // Optionally, display a message to inform the user
            MessageBox.Show("Form cannot be closed manually.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new Form1());
    }
}