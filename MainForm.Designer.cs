/*
 *  Created by SharpDevelop (To change this template use Tools | Options | Coding | Edit Standard Headers).
 * User: Jake Gustafson (Owner)
 * Date: 1/25/2007
 * Time: 10:08 AM
 * 
 */
namespace GoNowBackup
{
	partial class MainForm : System.Windows.Forms.Form
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
			this.components = new System.ComponentModel.Container();
			this.tbStatus = new System.Windows.Forms.TextBox();
			this.timerStart = new System.Windows.Forms.Timer(this.components);
			this.rtbOutput = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// tbStatus
			// 
			this.tbStatus.BackColor = System.Drawing.SystemColors.ActiveBorder;
			this.tbStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.tbStatus.Location = new System.Drawing.Point(0, 329);
			this.tbStatus.Name = "tbStatus";
			this.tbStatus.Size = new System.Drawing.Size(464, 24);
			this.tbStatus.TabIndex = 0;
			// 
			// timerStart
			// 
			this.timerStart.Interval = 5000;
			this.timerStart.Tick += new System.EventHandler(this.TimerStartTick);
			// 
			// rtbOutput
			// 
			this.rtbOutput.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtbOutput.Location = new System.Drawing.Point(0, 0);
			this.rtbOutput.Name = "rtbOutput";
			this.rtbOutput.ReadOnly = true;
			this.rtbOutput.Size = new System.Drawing.Size(464, 329);
			this.rtbOutput.TabIndex = 1;
			this.rtbOutput.Text = "";
			this.rtbOutput.TextChanged += new System.EventHandler(this.RtbOutputTextChanged);
			// 
			// MainForm
			// 
			//this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
			//this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(464, 353);
			this.Controls.Add(this.rtbOutput);
			this.Controls.Add(this.tbStatus);
			this.Name = "MainForm";
			this.Text = "GoNowBackup";
			//this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainFormFormClosed);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFormFormClosing);
			this.Load += new System.EventHandler(this.MainFormLoad);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.RichTextBox rtbOutput;
		private System.Windows.Forms.Timer timerStart;
		private System.Windows.Forms.TextBox tbStatus;
	}
}
