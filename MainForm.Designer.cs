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

namespace JakeGustafson
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
			this.comboDest = new System.Windows.Forms.ComboBox();
			this.lbOut = new System.Windows.Forms.ListBox();
			this.lblDest = new System.Windows.Forms.Label();
			this.tbStatus = new System.Windows.Forms.TextBox();
			this.lblDestInfo = new System.Windows.Forms.Label();
			this.progressbarMain = new System.Windows.Forms.ProgressBar();
			this.lblProfile = new System.Windows.Forms.Label();
			this.labelTrivialStatus = new System.Windows.Forms.Label();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.menuitemGo = new System.Windows.Forms.ToolStripMenuItem();
			this.menuitemCancel = new System.Windows.Forms.ToolStripMenuItem();
			this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuitemEditMain = new System.Windows.Forms.ToolStripMenuItem();
			this.menuitemEditScript = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveOutputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuitemHelp_ViewOutputOfLastRun = new System.Windows.Forms.ToolStripMenuItem();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.tableLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// comboDest
			// 
			this.comboDest.Dock = System.Windows.Forms.DockStyle.Fill;
			this.comboDest.FormattingEnabled = true;
			this.comboDest.Location = new System.Drawing.Point(157, 3);
			this.comboDest.Name = "comboDest";
			this.comboDest.Size = new System.Drawing.Size(153, 21);
			this.comboDest.TabIndex = 1;
			this.comboDest.SelectedIndexChanged += new System.EventHandler(this.ComboDestSelectedIndexChanged);
			this.comboDest.TextChanged += new System.EventHandler(this.ComboDestTextChanged);
			// 
			// lbOut
			// 
			this.lbOut.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbOut.FormattingEnabled = true;
			this.lbOut.HorizontalScrollbar = true;
			this.lbOut.Location = new System.Drawing.Point(3, 55);
			this.lbOut.Name = "lbOut";
			this.lbOut.Size = new System.Drawing.Size(467, 251);
			this.lbOut.TabIndex = 2;
			this.lbOut.MouseUp += new System.Windows.Forms.MouseEventHandler(this.LbOutMouseUp);
			this.lbOut.MouseEnter += new System.EventHandler(this.LbOutMouseEnter);
			this.lbOut.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LbOutMouseDown);
			this.lbOut.MouseLeave += new System.EventHandler(this.LbOutMouseLeave);
			// 
			// lblDest
			// 
			this.lblDest.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblDest.Location = new System.Drawing.Point(3, 0);
			this.lblDest.Name = "lblDest";
			this.lblDest.Size = new System.Drawing.Size(148, 22);
			this.lblDest.TabIndex = 3;
			this.lblDest.Text = "Destination:";
			this.lblDest.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tbStatus
			// 
			this.tbStatus.BackColor = System.Drawing.SystemColors.Control;
			this.tbStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.tbStatus.Location = new System.Drawing.Point(0, 370);
			this.tbStatus.Name = "tbStatus";
			this.tbStatus.ReadOnly = true;
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
			this.progressbarMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.progressbarMain.Location = new System.Drawing.Point(2, 318);
			this.progressbarMain.Margin = new System.Windows.Forms.Padding(2);
			this.progressbarMain.Name = "progressbarMain";
			this.progressbarMain.Size = new System.Drawing.Size(469, 26);
			this.progressbarMain.Step = 1;
			this.progressbarMain.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.progressbarMain.TabIndex = 8;
			// 
			// lblProfile
			// 
			this.lblProfile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblProfile.Location = new System.Drawing.Point(3, 26);
			this.lblProfile.Name = "lblProfile";
			this.lblProfile.Size = new System.Drawing.Size(467, 20);
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
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.lbOut, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.lblProfile, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.progressbarMain, 0, 3);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 24);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 4;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 90F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(473, 346);
			this.tableLayoutPanel1.TabIndex = 14;
			this.tableLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.TableLayoutPanel1Paint);
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.ColumnCount = 3;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
			this.tableLayoutPanel2.Controls.Add(this.comboDest, 1, 0);
			this.tableLayoutPanel2.Controls.Add(this.lblDest, 0, 0);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(2, 2);
			this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(2);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 1;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(469, 22);
			this.tableLayoutPanel2.TabIndex = 14;
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.menuitemGo,
									this.menuitemCancel,
									this.settingsToolStripMenuItem,
									this.helpToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
			this.menuStrip1.Size = new System.Drawing.Size(473, 24);
			this.menuStrip1.TabIndex = 15;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// menuitemGo
			// 
			this.menuitemGo.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.menuitemGo.Name = "menuitemGo";
			this.menuitemGo.Size = new System.Drawing.Size(36, 20);
			this.menuitemGo.Text = "Go";
			this.menuitemGo.Click += new System.EventHandler(this.MenuitemGoClick);
			// 
			// menuitemCancel
			// 
			this.menuitemCancel.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.menuitemCancel.Name = "menuitemCancel";
			this.menuitemCancel.Size = new System.Drawing.Size(62, 20);
			this.menuitemCancel.Text = "Cancel";
			this.menuitemCancel.Click += new System.EventHandler(this.MenuitemCancelClick);
			// 
			// settingsToolStripMenuItem
			// 
			this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.menuitemEditMain,
									this.menuitemEditScript});
			this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
			this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
			this.settingsToolStripMenuItem.Text = "Settings";
			// 
			// menuitemEditMain
			// 
			this.menuitemEditMain.Name = "menuitemEditMain";
			this.menuitemEditMain.Size = new System.Drawing.Size(147, 22);
			this.menuitemEditMain.Text = "Main";
			this.menuitemEditMain.Click += new System.EventHandler(this.MenuitemEditMainClick);
			// 
			// menuitemEditScript
			// 
			this.menuitemEditScript.Name = "menuitemEditScript";
			this.menuitemEditScript.Size = new System.Drawing.Size(147, 22);
			this.menuitemEditScript.Text = "Current Script";
			this.menuitemEditScript.Click += new System.EventHandler(this.MenuitemEditScriptClick);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.saveOutputToolStripMenuItem,
									this.menuitemHelp_ViewOutputOfLastRun});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.helpToolStripMenuItem.Text = "Help";
			// 
			// saveOutputToolStripMenuItem
			// 
			this.saveOutputToolStripMenuItem.Name = "saveOutputToolStripMenuItem";
			this.saveOutputToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.saveOutputToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
			this.saveOutputToolStripMenuItem.Text = "Save Output";
			this.saveOutputToolStripMenuItem.Click += new System.EventHandler(this.SaveOutputToolStripMenuItemClick);
			// 
			// menuitemHelp_ViewOutputOfLastRun
			// 
			this.menuitemHelp_ViewOutputOfLastRun.Name = "menuitemHelp_ViewOutputOfLastRun";
			this.menuitemHelp_ViewOutputOfLastRun.Size = new System.Drawing.Size(202, 22);
			this.menuitemHelp_ViewOutputOfLastRun.Text = "View Output of Last Run";
			this.menuitemHelp_ViewOutputOfLastRun.Click += new System.EventHandler(this.MenuitemHelp_ViewOutputOfLastRunClick);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(473, 390);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.labelTrivialStatus);
			this.Controls.Add(this.lblDestInfo);
			this.Controls.Add(this.tbStatus);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "MainForm";
			this.Text = "Backup GoNow";
			this.Load += new System.EventHandler(this.MainFormLoad);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainFormFormClosed);
			this.Resize += new System.EventHandler(this.MainFormResize);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.ToolStripMenuItem menuitemHelp_ViewOutputOfLastRun;
		private System.Windows.Forms.ToolStripMenuItem menuitemCancel;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.Windows.Forms.ToolStripMenuItem saveOutputToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem menuitemEditScript;
		private System.Windows.Forms.ToolStripMenuItem menuitemEditMain;
		private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem menuitemGo;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label labelTrivialStatus;
		private System.Windows.Forms.Label lblProfile;
		private System.Windows.Forms.ProgressBar progressbarMain;
		private System.Windows.Forms.Label lblDestInfo;
		private System.Windows.Forms.Label lblDest;
		private System.Windows.Forms.ListBox lbOut;
		private System.Windows.Forms.ComboBox comboDest;
		private System.Windows.Forms.TextBox tbStatus;
	}
}
