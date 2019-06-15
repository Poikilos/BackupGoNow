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

namespace ExpertMultimedia
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.destinationComboBox = new System.Windows.Forms.ComboBox();
			this.lbOut = new System.Windows.Forms.ListBox();
			this.lblDest = new System.Windows.Forms.Label();
			this.tbStatus = new System.Windows.Forms.TextBox();
			this.progressbarMain = new System.Windows.Forms.ProgressBar();
			this.profileLabel = new System.Windows.Forms.Label();
			this.labelTrivialStatus = new System.Windows.Forms.Label();
			this.mainTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.mainTabControl = new System.Windows.Forms.TabControl();
			this.optionsTabPage = new System.Windows.Forms.TabPage();
			this.optionsOuterTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.optionsHelpLabel = new System.Windows.Forms.Label();
			this.optionsTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.logTabPage = new System.Windows.Forms.TabPage();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.addFolderButton = new System.Windows.Forms.Button();
			this.destinationTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.driveLabel = new System.Windows.Forms.Label();
			this.goFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.goButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuitemEditMain = new System.Windows.Forms.ToolStripMenuItem();
			this.menuitemEditScript = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveOutputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuitemHelp_ViewOutputOfLastRun = new System.Windows.Forms.ToolStripMenuItem();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.startupTimer = new System.Windows.Forms.Timer(this.components);
			this.mainFolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.mainTableLayoutPanel.SuspendLayout();
			this.mainTabControl.SuspendLayout();
			this.optionsTabPage.SuspendLayout();
			this.optionsOuterTableLayoutPanel.SuspendLayout();
			this.logTabPage.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.destinationTableLayoutPanel.SuspendLayout();
			this.goFlowLayoutPanel.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// destinationComboBox
			// 
			this.destinationComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.destinationComboBox.FormattingEnabled = true;
			this.destinationComboBox.Location = new System.Drawing.Point(294, 4);
			this.destinationComboBox.Margin = new System.Windows.Forms.Padding(4);
			this.destinationComboBox.Name = "destinationComboBox";
			this.destinationComboBox.Size = new System.Drawing.Size(291, 27);
			this.destinationComboBox.TabIndex = 1;
			this.destinationComboBox.SelectedIndexChanged += new System.EventHandler(this.DestinationComboBoxSelectedIndexChanged);
			// 
			// lbOut
			// 
			this.lbOut.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbOut.FormattingEnabled = true;
			this.lbOut.HorizontalScrollbar = true;
			this.lbOut.ItemHeight = 19;
			this.lbOut.Location = new System.Drawing.Point(4, 4);
			this.lbOut.Margin = new System.Windows.Forms.Padding(4);
			this.lbOut.Name = "lbOut";
			this.lbOut.Size = new System.Drawing.Size(862, 213);
			this.lbOut.TabIndex = 2;
			this.lbOut.MouseUp += new System.Windows.Forms.MouseEventHandler(this.LbOutMouseUp);
			this.lbOut.MouseEnter += new System.EventHandler(this.LbOutMouseEnter);
			this.lbOut.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LbOutMouseDown);
			this.lbOut.MouseLeave += new System.EventHandler(this.LbOutMouseLeave);
			// 
			// lblDest
			// 
			this.lblDest.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.lblDest.AutoSize = true;
			this.lblDest.Location = new System.Drawing.Point(198, 6);
			this.lblDest.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lblDest.Name = "lblDest";
			this.lblDest.Size = new System.Drawing.Size(88, 19);
			this.lblDest.TabIndex = 3;
			this.lblDest.Text = "Destination:";
			this.lblDest.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tbStatus
			// 
			this.tbStatus.BackColor = System.Drawing.SystemColors.Control;
			this.tbStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.tbStatus.Location = new System.Drawing.Point(0, 559);
			this.tbStatus.Margin = new System.Windows.Forms.Padding(4);
			this.tbStatus.Name = "tbStatus";
			this.tbStatus.ReadOnly = true;
			this.tbStatus.Size = new System.Drawing.Size(886, 27);
			this.tbStatus.TabIndex = 6;
			// 
			// progressbarMain
			// 
			this.progressbarMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.progressbarMain.Location = new System.Drawing.Point(3, 455);
			this.progressbarMain.Name = "progressbarMain";
			this.progressbarMain.Size = new System.Drawing.Size(880, 38);
			this.progressbarMain.Step = 1;
			this.progressbarMain.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.progressbarMain.TabIndex = 8;
			// 
			// profileLabel
			// 
			this.profileLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.profileLabel.AutoSize = true;
			this.profileLabel.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.profileLabel.Location = new System.Drawing.Point(353, 0);
			this.profileLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.profileLabel.Name = "profileLabel";
			this.profileLabel.Size = new System.Drawing.Size(179, 19);
			this.profileLabel.TabIndex = 9;
			this.profileLabel.Text = "(Error: no profile found!)";
			this.profileLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.profileLabel.Visible = false;
			// 
			// labelTrivialStatus
			// 
			this.labelTrivialStatus.AutoEllipsis = true;
			this.labelTrivialStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.labelTrivialStatus.Location = new System.Drawing.Point(0, 525);
			this.labelTrivialStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labelTrivialStatus.Name = "labelTrivialStatus";
			this.labelTrivialStatus.Size = new System.Drawing.Size(886, 34);
			this.labelTrivialStatus.TabIndex = 10;
			// 
			// mainTableLayoutPanel
			// 
			this.mainTableLayoutPanel.ColumnCount = 1;
			this.mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.mainTableLayoutPanel.Controls.Add(this.profileLabel, 0, 0);
			this.mainTableLayoutPanel.Controls.Add(this.progressbarMain, 0, 6);
			this.mainTableLayoutPanel.Controls.Add(this.mainTabControl, 0, 1);
			this.mainTableLayoutPanel.Controls.Add(this.flowLayoutPanel1, 0, 2);
			this.mainTableLayoutPanel.Controls.Add(this.destinationTableLayoutPanel, 0, 3);
			this.mainTableLayoutPanel.Controls.Add(this.goFlowLayoutPanel, 0, 5);
			this.mainTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mainTableLayoutPanel.Location = new System.Drawing.Point(0, 29);
			this.mainTableLayoutPanel.Name = "mainTableLayoutPanel";
			this.mainTableLayoutPanel.RowCount = 7;
			this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.mainTableLayoutPanel.Size = new System.Drawing.Size(886, 496);
			this.mainTableLayoutPanel.TabIndex = 14;
			this.mainTableLayoutPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.TableLayoutPanel1Paint);
			// 
			// mainTabControl
			// 
			this.mainTabControl.Controls.Add(this.optionsTabPage);
			this.mainTabControl.Controls.Add(this.logTabPage);
			this.mainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mainTabControl.Location = new System.Drawing.Point(4, 23);
			this.mainTabControl.Margin = new System.Windows.Forms.Padding(4);
			this.mainTabControl.Name = "mainTabControl";
			this.mainTabControl.SelectedIndex = 0;
			this.mainTabControl.Size = new System.Drawing.Size(878, 267);
			this.mainTabControl.TabIndex = 15;
			// 
			// optionsTabPage
			// 
			this.optionsTabPage.Controls.Add(this.optionsOuterTableLayoutPanel);
			this.optionsTabPage.Location = new System.Drawing.Point(4, 28);
			this.optionsTabPage.Margin = new System.Windows.Forms.Padding(4);
			this.optionsTabPage.Name = "optionsTabPage";
			this.optionsTabPage.Padding = new System.Windows.Forms.Padding(4);
			this.optionsTabPage.Size = new System.Drawing.Size(870, 235);
			this.optionsTabPage.TabIndex = 0;
			this.optionsTabPage.Text = "Options";
			this.optionsTabPage.UseVisualStyleBackColor = true;
			// 
			// optionsOuterTableLayoutPanel
			// 
			this.optionsOuterTableLayoutPanel.ColumnCount = 1;
			this.optionsOuterTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.optionsOuterTableLayoutPanel.Controls.Add(this.optionsHelpLabel, 0, 0);
			this.optionsOuterTableLayoutPanel.Controls.Add(this.optionsTableLayoutPanel, 0, 1);
			this.optionsOuterTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.optionsOuterTableLayoutPanel.Location = new System.Drawing.Point(4, 4);
			this.optionsOuterTableLayoutPanel.Name = "optionsOuterTableLayoutPanel";
			this.optionsOuterTableLayoutPanel.RowCount = 2;
			this.optionsOuterTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.optionsOuterTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.optionsOuterTableLayoutPanel.Size = new System.Drawing.Size(862, 227);
			this.optionsOuterTableLayoutPanel.TabIndex = 1;
			// 
			// optionsHelpLabel
			// 
			this.optionsHelpLabel.AutoSize = true;
			this.optionsHelpLabel.Location = new System.Drawing.Point(3, 0);
			this.optionsHelpLabel.Name = "optionsHelpLabel";
			this.optionsHelpLabel.Size = new System.Drawing.Size(322, 19);
			this.optionsHelpLabel.TabIndex = 1;
			this.optionsHelpLabel.Text = "Press \"Go\" to backup with the following options:";
			// 
			// optionsTableLayoutPanel
			// 
			this.optionsTableLayoutPanel.AutoScroll = true;
			this.optionsTableLayoutPanel.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
			this.optionsTableLayoutPanel.ColumnCount = 3;
			this.optionsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.optionsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
			this.optionsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.optionsTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.optionsTableLayoutPanel.Location = new System.Drawing.Point(4, 23);
			this.optionsTableLayoutPanel.Margin = new System.Windows.Forms.Padding(4);
			this.optionsTableLayoutPanel.Name = "optionsTableLayoutPanel";
			this.optionsTableLayoutPanel.RowCount = 2;
			this.optionsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
			this.optionsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.optionsTableLayoutPanel.Size = new System.Drawing.Size(854, 200);
			this.optionsTableLayoutPanel.TabIndex = 0;
			// 
			// logTabPage
			// 
			this.logTabPage.Controls.Add(this.lbOut);
			this.logTabPage.Location = new System.Drawing.Point(4, 28);
			this.logTabPage.Margin = new System.Windows.Forms.Padding(4);
			this.logTabPage.Name = "logTabPage";
			this.logTabPage.Padding = new System.Windows.Forms.Padding(4);
			this.logTabPage.Size = new System.Drawing.Size(870, 235);
			this.logTabPage.TabIndex = 1;
			this.logTabPage.Text = "Log";
			this.logTabPage.UseVisualStyleBackColor = true;
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.AutoSize = true;
			this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanel1.Controls.Add(this.addFolderButton);
			this.flowLayoutPanel1.Location = new System.Drawing.Point(4, 298);
			this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(108, 42);
			this.flowLayoutPanel1.TabIndex = 16;
			// 
			// addFolderButton
			// 
			this.addFolderButton.Location = new System.Drawing.Point(4, 4);
			this.addFolderButton.Margin = new System.Windows.Forms.Padding(4);
			this.addFolderButton.Name = "addFolderButton";
			this.addFolderButton.Size = new System.Drawing.Size(100, 34);
			this.addFolderButton.TabIndex = 0;
			this.addFolderButton.Text = "Add Folder";
			this.addFolderButton.UseVisualStyleBackColor = true;
			this.addFolderButton.Click += new System.EventHandler(this.AddFolderButtonClick);
			// 
			// destinationTableLayoutPanel
			// 
			this.destinationTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.destinationTableLayoutPanel.ColumnCount = 3;
			this.destinationTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
			this.destinationTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34F));
			this.destinationTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
			this.destinationTableLayoutPanel.Controls.Add(this.destinationComboBox, 1, 0);
			this.destinationTableLayoutPanel.Controls.Add(this.lblDest, 0, 0);
			this.destinationTableLayoutPanel.Controls.Add(this.driveLabel, 2, 0);
			this.destinationTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.destinationTableLayoutPanel.Location = new System.Drawing.Point(3, 347);
			this.destinationTableLayoutPanel.Name = "destinationTableLayoutPanel";
			this.destinationTableLayoutPanel.RowCount = 1;
			this.destinationTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.destinationTableLayoutPanel.Size = new System.Drawing.Size(880, 32);
			this.destinationTableLayoutPanel.TabIndex = 14;
			// 
			// driveLabel
			// 
			this.driveLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.driveLabel.AutoSize = true;
			this.driveLabel.Location = new System.Drawing.Point(592, 6);
			this.driveLabel.Name = "driveLabel";
			this.driveLabel.Size = new System.Drawing.Size(0, 19);
			this.driveLabel.TabIndex = 4;
			// 
			// goFlowLayoutPanel
			// 
			this.goFlowLayoutPanel.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.goFlowLayoutPanel.AutoSize = true;
			this.goFlowLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.goFlowLayoutPanel.Controls.Add(this.goButton);
			this.goFlowLayoutPanel.Controls.Add(this.cancelButton);
			this.goFlowLayoutPanel.Location = new System.Drawing.Point(373, 405);
			this.goFlowLayoutPanel.Name = "goFlowLayoutPanel";
			this.goFlowLayoutPanel.Size = new System.Drawing.Size(139, 44);
			this.goFlowLayoutPanel.TabIndex = 17;
			// 
			// goButton
			// 
			this.goButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.goButton.AutoSize = true;
			this.goButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.goButton.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.goButton.Location = new System.Drawing.Point(4, 4);
			this.goButton.Margin = new System.Windows.Forms.Padding(4);
			this.goButton.Name = "goButton";
			this.goButton.Size = new System.Drawing.Size(46, 36);
			this.goButton.TabIndex = 17;
			this.goButton.Text = "Go";
			this.goButton.UseVisualStyleBackColor = true;
			this.goButton.Click += new System.EventHandler(this.GoButtonClick);
			// 
			// cancelButton
			// 
			this.cancelButton.AutoSize = true;
			this.cancelButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.cancelButton.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Bold);
			this.cancelButton.Location = new System.Drawing.Point(57, 3);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(79, 36);
			this.cancelButton.TabIndex = 1;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.CancelButtonClick);
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.settingsToolStripMenuItem,
									this.helpToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Padding = new System.Windows.Forms.Padding(5, 3, 0, 3);
			this.menuStrip1.Size = new System.Drawing.Size(886, 29);
			this.menuStrip1.TabIndex = 15;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// settingsToolStripMenuItem
			// 
			this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.menuitemEditMain,
									this.menuitemEditScript});
			this.settingsToolStripMenuItem.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
			this.settingsToolStripMenuItem.Size = new System.Drawing.Size(73, 23);
			this.settingsToolStripMenuItem.Text = "Settings";
			// 
			// menuitemEditMain
			// 
			this.menuitemEditMain.Name = "menuitemEditMain";
			this.menuitemEditMain.Size = new System.Drawing.Size(166, 24);
			this.menuitemEditMain.Text = "Main";
			this.menuitemEditMain.Click += new System.EventHandler(this.MenuitemEditMainClick);
			// 
			// menuitemEditScript
			// 
			this.menuitemEditScript.Name = "menuitemEditScript";
			this.menuitemEditScript.Size = new System.Drawing.Size(166, 24);
			this.menuitemEditScript.Text = "Current Script";
			this.menuitemEditScript.Click += new System.EventHandler(this.MenuitemEditScriptClick);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.saveOutputToolStripMenuItem,
									this.menuitemHelp_ViewOutputOfLastRun});
			this.helpToolStripMenuItem.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(51, 23);
			this.helpToolStripMenuItem.Text = "Help";
			// 
			// saveOutputToolStripMenuItem
			// 
			this.saveOutputToolStripMenuItem.Name = "saveOutputToolStripMenuItem";
			this.saveOutputToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.saveOutputToolStripMenuItem.Size = new System.Drawing.Size(231, 24);
			this.saveOutputToolStripMenuItem.Text = "Save Log";
			this.saveOutputToolStripMenuItem.Click += new System.EventHandler(this.SaveOutputToolStripMenuItemClick);
			// 
			// menuitemHelp_ViewOutputOfLastRun
			// 
			this.menuitemHelp_ViewOutputOfLastRun.Name = "menuitemHelp_ViewOutputOfLastRun";
			this.menuitemHelp_ViewOutputOfLastRun.Size = new System.Drawing.Size(231, 24);
			this.menuitemHelp_ViewOutputOfLastRun.Text = "View Log from Last Run";
			this.menuitemHelp_ViewOutputOfLastRun.Click += new System.EventHandler(this.MenuitemHelp_ViewOutputOfLastRunClick);
			// 
			// startupTimer
			// 
			this.startupTimer.Tick += new System.EventHandler(this.StartupTimerTick);
			// 
			// mainFolderBrowserDialog
			// 
			this.mainFolderBrowserDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
			this.mainFolderBrowserDialog.HelpRequest += new System.EventHandler(this.MainFolderBrowserDialogHelpRequest);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(886, 586);
			this.Controls.Add(this.mainTableLayoutPanel);
			this.Controls.Add(this.labelTrivialStatus);
			this.Controls.Add(this.tbStatus);
			this.Controls.Add(this.menuStrip1);
			this.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip1;
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "MainForm";
			this.Text = "Backup GoNow";
			this.Load += new System.EventHandler(this.MainFormLoad);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainFormFormClosed);
			this.Resize += new System.EventHandler(this.MainFormResize);
			this.mainTableLayoutPanel.ResumeLayout(false);
			this.mainTableLayoutPanel.PerformLayout();
			this.mainTabControl.ResumeLayout(false);
			this.optionsTabPage.ResumeLayout(false);
			this.optionsOuterTableLayoutPanel.ResumeLayout(false);
			this.optionsOuterTableLayoutPanel.PerformLayout();
			this.logTabPage.ResumeLayout(false);
			this.flowLayoutPanel1.ResumeLayout(false);
			this.destinationTableLayoutPanel.ResumeLayout(false);
			this.destinationTableLayoutPanel.PerformLayout();
			this.goFlowLayoutPanel.ResumeLayout(false);
			this.goFlowLayoutPanel.PerformLayout();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.ComboBox destinationComboBox;
		private System.Windows.Forms.FolderBrowserDialog mainFolderBrowserDialog;
		private System.Windows.Forms.Timer startupTimer;
		private System.Windows.Forms.Label profileLabel;
		private System.Windows.Forms.Label optionsHelpLabel;
		private System.Windows.Forms.TableLayoutPanel optionsOuterTableLayoutPanel;
		private System.Windows.Forms.Label driveLabel;
		private System.Windows.Forms.FlowLayoutPanel goFlowLayoutPanel;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.TableLayoutPanel mainTableLayoutPanel;
		private System.Windows.Forms.TableLayoutPanel destinationTableLayoutPanel;
		private System.Windows.Forms.Button goButton;
		private System.Windows.Forms.Button addFolderButton;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.TabPage logTabPage;
		private System.Windows.Forms.TableLayoutPanel optionsTableLayoutPanel;
		private System.Windows.Forms.TabPage optionsTabPage;
		private System.Windows.Forms.TabControl mainTabControl;
		private System.Windows.Forms.ToolStripMenuItem menuitemHelp_ViewOutputOfLastRun;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.Windows.Forms.ToolStripMenuItem saveOutputToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem menuitemEditScript;
		private System.Windows.Forms.ToolStripMenuItem menuitemEditMain;
		private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.Label labelTrivialStatus;
		private System.Windows.Forms.ProgressBar progressbarMain;
		private System.Windows.Forms.Label lblDest;
		private System.Windows.Forms.ListBox lbOut;
		private System.Windows.Forms.TextBox tbStatus;
	}
}
