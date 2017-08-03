/*
 * Created by SharpDevelop.
 * User: Owner
 * Date: 10/5/2008
 * Time: 12:21 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace OrangejuiceElectronica
{
	partial class MainForm
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
			this.btnGo = new System.Windows.Forms.Button();
			this.comboDest = new System.Windows.Forms.ComboBox();
			this.lbOut = new System.Windows.Forms.ListBox();
			this.lblDest = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.tbStatus = new System.Windows.Forms.TextBox();
			this.lblDestInfo = new System.Windows.Forms.Label();
			this.progressbarMain = new System.Windows.Forms.ProgressBar();
			this.lblProfile = new System.Windows.Forms.Label();
			this.labelTrivialStatus = new System.Windows.Forms.Label();
			this.btnEditScript = new System.Windows.Forms.Button();
			this.btnEditMain = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnGo
			// 
			this.btnGo.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnGo.Location = new System.Drawing.Point(0, 0);
			this.btnGo.Name = "btnGo";
			this.btnGo.Size = new System.Drawing.Size(64, 64);
			this.btnGo.TabIndex = 0;
			this.btnGo.Text = "Go";
			this.btnGo.UseVisualStyleBackColor = true;
			this.btnGo.Click += new System.EventHandler(this.BtnGoClick);
			// 
			// comboDest
			// 
			this.comboDest.FormattingEnabled = true;
			this.comboDest.Location = new System.Drawing.Point(83, 70);
			this.comboDest.Name = "comboDest";
			this.comboDest.Size = new System.Drawing.Size(121, 21);
			this.comboDest.TabIndex = 1;
			this.comboDest.SelectedIndexChanged += new System.EventHandler(this.ComboDestSelectedIndexChanged);
			this.comboDest.TextChanged += new System.EventHandler(this.ComboDestTextChanged);
			// 
			// lbOut
			// 
			this.lbOut.FormattingEnabled = true;
			this.lbOut.HorizontalScrollbar = true;
			this.lbOut.Location = new System.Drawing.Point(0, 148);
			this.lbOut.Name = "lbOut";
			this.lbOut.Size = new System.Drawing.Size(238, 147);
			this.lbOut.TabIndex = 2;
			this.lbOut.MouseUp += new System.Windows.Forms.MouseEventHandler(this.LbOutMouseUp);
			this.lbOut.MouseEnter += new System.EventHandler(this.LbOutMouseEnter);
			this.lbOut.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LbOutMouseDown);
			this.lbOut.MouseLeave += new System.EventHandler(this.LbOutMouseLeave);
			// 
			// lblDest
			// 
			this.lblDest.Location = new System.Drawing.Point(0, 67);
			this.lblDest.Name = "lblDest";
			this.lblDest.Size = new System.Drawing.Size(77, 22);
			this.lblDest.TabIndex = 3;
			this.lblDest.Text = "Destination:";
			this.lblDest.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// btnCancel
			// 
			this.btnCancel.Enabled = false;
			this.btnCancel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnCancel.Location = new System.Drawing.Point(64, 0);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(64, 64);
			this.btnCancel.TabIndex = 4;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.BtnCancelClick);
			// 
			// tbStatus
			// 
			this.tbStatus.BackColor = System.Drawing.SystemColors.Control;
			this.tbStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.tbStatus.Location = new System.Drawing.Point(0, 370);
			this.tbStatus.Name = "tbStatus";
			this.tbStatus.Size = new System.Drawing.Size(473, 20);
			this.tbStatus.TabIndex = 6;
			// 
			// lblDestInfo
			// 
			this.lblDestInfo.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblDestInfo.Location = new System.Drawing.Point(210, 70);
			this.lblDestInfo.Name = "lblDestInfo";
			this.lblDestInfo.Size = new System.Drawing.Size(246, 21);
			this.lblDestInfo.TabIndex = 7;
			this.lblDestInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// progressbarMain
			// 
			this.progressbarMain.Location = new System.Drawing.Point(0, 346);
			this.progressbarMain.Margin = new System.Windows.Forms.Padding(2);
			this.progressbarMain.Name = "progressbarMain";
			this.progressbarMain.Size = new System.Drawing.Size(462, 19);
			this.progressbarMain.Step = 1;
			this.progressbarMain.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.progressbarMain.TabIndex = 8;
			// 
			// lblProfile
			// 
			this.lblProfile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblProfile.Location = new System.Drawing.Point(0, 102);
			this.lblProfile.Name = "lblProfile";
			this.lblProfile.Size = new System.Drawing.Size(473, 20);
			this.lblProfile.TabIndex = 9;
			this.lblProfile.Text = "(Error: no profile found!)";
			this.lblProfile.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lblProfile.Visible = false;
			// 
			// labelTrivialStatus
			// 
			this.labelTrivialStatus.AutoEllipsis = true;
			this.labelTrivialStatus.Location = new System.Drawing.Point(0, 122);
			this.labelTrivialStatus.Name = "labelTrivialStatus";
			this.labelTrivialStatus.Size = new System.Drawing.Size(659, 23);
			this.labelTrivialStatus.TabIndex = 10;
			// 
			// btnEditScript
			// 
			this.btnEditScript.Enabled = false;
			this.btnEditScript.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnEditScript.Location = new System.Drawing.Point(192, 0);
			this.btnEditScript.Name = "btnEditScript";
			this.btnEditScript.Size = new System.Drawing.Size(64, 64);
			this.btnEditScript.TabIndex = 11;
			this.btnEditScript.Text = "Show Script";
			this.btnEditScript.UseVisualStyleBackColor = true;
			this.btnEditScript.Click += new System.EventHandler(this.BtnEditScriptClick);
			// 
			// btnEditMain
			// 
			this.btnEditMain.Enabled = false;
			this.btnEditMain.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnEditMain.Location = new System.Drawing.Point(128, 0);
			this.btnEditMain.Name = "btnEditMain";
			this.btnEditMain.Size = new System.Drawing.Size(64, 64);
			this.btnEditMain.TabIndex = 12;
			this.btnEditMain.Text = "Show Profile Main Settings";
			this.btnEditMain.UseVisualStyleBackColor = true;
			this.btnEditMain.Click += new System.EventHandler(this.BtnEditMainClick);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(473, 390);
			this.Controls.Add(this.lbOut);
			this.Controls.Add(this.btnEditMain);
			this.Controls.Add(this.btnEditScript);
			this.Controls.Add(this.labelTrivialStatus);
			this.Controls.Add(this.progressbarMain);
			this.Controls.Add(this.lblDestInfo);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.lblDest);
			this.Controls.Add(this.comboDest);
			this.Controls.Add(this.btnGo);
			this.Controls.Add(this.tbStatus);
			this.Controls.Add(this.lblProfile);
			this.Name = "MainForm";
			this.Text = "Backup GoNow";
			this.Load += new System.EventHandler(this.MainFormLoad);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainFormFormClosed);
			this.Resize += new System.EventHandler(this.MainFormResize);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.Button btnEditScript;
		private System.Windows.Forms.Button btnEditMain;
		private System.Windows.Forms.Label labelTrivialStatus;
		private System.Windows.Forms.Label lblProfile;
		private System.Windows.Forms.ProgressBar progressbarMain;
		private System.Windows.Forms.Label lblDestInfo;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label lblDest;
		private System.Windows.Forms.ListBox lbOut;
		private System.Windows.Forms.Button btnGo;
		private System.Windows.Forms.ComboBox comboDest;
		private System.Windows.Forms.TextBox tbStatus;
	}
}
