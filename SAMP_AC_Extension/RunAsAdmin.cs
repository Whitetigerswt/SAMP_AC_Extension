/*
 * Created by SharpDevelop.
 * User: My real Account
 * Date: 7/9/2012
 * Time: 12:26 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SAMP_AC_Extension
{
	/// <summary>
	/// Description of RunAsAdmin.
	/// </summary>
	public partial class RunAsAdmin : Form
	{
		public RunAsAdmin()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		public static void showWindow() {
			RunAsAdmin test = new RunAsAdmin();
			test.Show();
			test.Activate();
			test.Refresh();
			
			
		}
	}
}
