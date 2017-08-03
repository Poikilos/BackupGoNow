/*
 * Created by SharpDevelop.
 * User: Owner
 * Date: 10/5/2008
 * Time: 12:21 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
			this.cbDest = new System.Windows.Forms.ComboBox();
			this.lbOut = new System.Windows.Forms.ListBox();
			this.lblDest = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnGo
			// 
			this.btnGo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnGo.Location = new System.Drawing.Point(29, 12);
			this.btnGo.Name = "btnGo";
			this.btnGo.Size = new System.Drawing.Size(285, 23);
			this.btnGo.TabIndex = 0;
			this.btnGo.Text = "Go";
			this.btnGo.UseVisualStyleBackColor = true;
			this.btnGo.Click += new System.EventHandler(this.BtnGoClick);
			// 
			// cbDest
			// 
			this.cbDest.FormattingEnabled = true;
			this.cbDest.Location = new System.Drawing.Point(112, 41);
			this.cbDest.Name = "cbDest";
			this.cbDest.Size = new System.Drawing.Size(121, 21);
			this.cbDest.TabIndex = 1;
			this.cbDest.SelectedIndexChanged += new System.EventHandler(this.CbDestSelectedIndexChanged);
			this.cbDest.TextChanged += new System.EventHandler(this.CbDestTextChanged);
			// 
			// lbOut
			// 
			this.lbOut.FormattingEnabled = true;
			this.lbOut.Location = new System.Drawing.Point(29, 79);
			this.lbOut.Name = "lbOut";
			this.lbOut.Size = new System.Drawing.Size(238, 147);
			this.lbOut.TabIndex = 2;
			// 
			// lblDest
			// 
			this.lblDest.Location = new System.Drawing.Point(6, 44);
			this.lblDest.Name = "lblDest";
			this.lblDest.Size = new System.Drawing.Size(100, 22);
			this.lblDest.TabIndex = 3;
			this.lblDest.Text = "Destination:";
			this.lblDest.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// btnCancel
			// 
			this.btnCancel.Enabled = false;
			this.btnCancel.Location = new System.Drawing.Point(239, 41);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 4;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.BtnCancelClick);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(336, 257);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.lblDest);
			this.Controls.Add(this.lbOut);
			this.Controls.Add(this.cbDest);
			this.Controls.Add(this.btnGo);
			this.Name = "MainForm";
			this.Text = "Backup GoNow";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainFormFormClosed);
			this.Resize += new System.EventHandler(this.MainFormResize);
			this.Load += new System.EventHandler(this.MainFormLoad);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label lblDest;
		private System.Windows.Forms.ListBox lbOut;
		private System.Windows.Forms.Button btnGo;
		private System.Windows.Forms.ComboBox cbDest;
	}
}
