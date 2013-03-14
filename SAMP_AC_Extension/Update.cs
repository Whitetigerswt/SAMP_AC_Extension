/*
 * Created by SharpDevelop.
 * User: My PC
 * Date: 5/16/2012
 * Time: 7:28 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace SAMP_AC_Extension
{
	/// <summary>
	/// Description of Form1.
	/// </summary>
	public partial class Form1 : Form
	{
		public Form1()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			this.TopMost = true;
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		void Button1MouseClick(object sender, MouseEventArgs e)
		{
			endTheProcess();
		}
		
		void Form1FormClosed(object sender, FormClosedEventArgs e)
		{
			endTheProcess();
		}
		
		void Form1Leave(object sender, EventArgs e)
		{
			var form = this;
			form.Show();
			Application.OpenForms[form.Name].Activate();
		}
		
		void Form1Deactivate(object sender, EventArgs e)
		{	
			var form = this;
			form.Show();
			Application.OpenForms[form.Name].Activate();
		}
		
		public void Countdown(int from) {
			while(from > 0) {
				from--;
				this.label1.Text = "SAMP_AC_Extension ESL Wire Plugin has been automaticly updated, restarting in " + from + " seconds";

				this.label1.Show();
				this.label1.Refresh();
				
				Thread.Sleep(1000);
			}
			endTheProcess();
		}
		void endTheProcess() {
			Process[] prs = Process.GetProcessesByName("wire");

			foreach (Process pr in prs)
			{				
				pr.Kill();
			}
			
			prs = Process.GetProcessesByName("wire-plugin");

			foreach (Process pr in prs)
			{				
				pr.Kill();
			}
		}
		public static void ShowTheWindow() {
        	
        	Form1 test = new Form1();
			test.Show();
			test.Activate();
			test.Refresh();
			test.Countdown(5);
        }
	}
}
