/*
 * Created by SharpDevelop.
 * User: My PC
 * Date: 5/16/2012
 * Time: 7:28 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace SAMP_AC_Extension
{
	partial class Form1
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
			this.button1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(35, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(213, 51);
			this.label1.TabIndex = 0;
			this.label1.Text = "SAMP_AC_Extension ESL Wire Plugin has been automaticly updated, Please restart ES" +
			"L Wire";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(79, 70);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(120, 29);
			this.button1.TabIndex = 1;
			this.button1.Text = "Close ESL Wire";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Button1MouseClick);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 111);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.label1);
			this.Name = "Form1";
			this.Text = "SAMP_AC_Extension updated.";
			this.Deactivate += new System.EventHandler(this.Form1Deactivate);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1FormClosed);
			this.Leave += new System.EventHandler(this.Form1Leave);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label label1;
	}
}
