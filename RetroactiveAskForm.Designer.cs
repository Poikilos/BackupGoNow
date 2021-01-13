/*
 * Created by SharpDevelop.
 * User: ramesh.jain
 * Date: 1/12/2021
 * Time: 6:51 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace ExpertMultimedia
{
	partial class RetroactiveAskForm
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
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.rememberCB = new System.Windows.Forms.CheckBox();
			this.bodyLabel = new System.Windows.Forms.Label();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.noBtn = new System.Windows.Forms.Button();
			this.yesBtn = new System.Windows.Forms.Button();
			this.tableLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.rememberCB, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.bodyLabel, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 2);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(408, 261);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// rememberCB
			// 
			this.rememberCB.AutoSize = true;
			this.rememberCB.Checked = true;
			this.rememberCB.CheckState = System.Windows.Forms.CheckState.Checked;
			this.rememberCB.Location = new System.Drawing.Point(3, 206);
			this.rememberCB.Name = "rememberCB";
			this.rememberCB.Size = new System.Drawing.Size(133, 17);
			this.rememberCB.TabIndex = 0;
			this.rememberCB.Text = "Remember my answer.";
			this.rememberCB.UseVisualStyleBackColor = true;
			// 
			// bodyLabel
			// 
			this.bodyLabel.Location = new System.Drawing.Point(3, 0);
			this.bodyLabel.Name = "bodyLabel";
			this.bodyLabel.Size = new System.Drawing.Size(402, 189);
			this.bodyLabel.TabIndex = 2;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.AutoSize = true;
			this.tableLayoutPanel2.ColumnCount = 2;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel2.Controls.Add(this.noBtn, 1, 0);
			this.tableLayoutPanel2.Controls.Add(this.yesBtn, 0, 0);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 229);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 1;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.Size = new System.Drawing.Size(402, 29);
			this.tableLayoutPanel2.TabIndex = 3;
			// 
			// noBtn
			// 
			this.noBtn.Location = new System.Drawing.Point(204, 3);
			this.noBtn.Name = "noBtn";
			this.noBtn.Size = new System.Drawing.Size(75, 23);
			this.noBtn.TabIndex = 1;
			this.noBtn.Text = "No";
			this.noBtn.UseVisualStyleBackColor = true;
			this.noBtn.Click += new System.EventHandler(this.NoBtnClick);
			// 
			// yesBtn
			// 
			this.yesBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.yesBtn.Location = new System.Drawing.Point(123, 3);
			this.yesBtn.Name = "yesBtn";
			this.yesBtn.Size = new System.Drawing.Size(75, 23);
			this.yesBtn.TabIndex = 0;
			this.yesBtn.Text = "Yes";
			this.yesBtn.UseVisualStyleBackColor = true;
			this.yesBtn.Click += new System.EventHandler(this.YesBtnClick);
			// 
			// RetroactiveAskForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(408, 261);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "RetroactiveAskForm";
			this.Text = "Backup GoNow";
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.tableLayoutPanel2.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Button yesBtn;
		private System.Windows.Forms.Button noBtn;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.Label bodyLabel;
		private System.Windows.Forms.CheckBox rememberCB;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
	}
}
