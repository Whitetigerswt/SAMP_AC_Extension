/*
 * Created by SharpDevelop.
 * User: My real Account
 * Date: 7/9/2012
 * Time: 12:26 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace SAMP_AC_Extension
{
	partial class RunAsAdmin
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(79, 40);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "Please run Wire as Administrator.";
			// 
			// RunAsAdmin
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 103);
			this.Controls.Add(this.label1);
			this.Name = "RunAsAdmin";
			this.Text = "RunAsAdmin";
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Label label1;
	}
}
