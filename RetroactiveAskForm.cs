/*
 * Created by SharpDevelop.
 * User: ramesh.jain
 * Date: 1/12/2021
 * Time: 6:51 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ExpertMultimedia
{
	/// <summary>
	/// Description of RetroactiveAskForm.
	/// </summary>
	public partial class RetroactiveAskForm : Form
	{
		public bool remember = true;
		public string body = "(undefined message)";
		public RetroactiveAskForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			this.remember = this.rememberCB.Checked;
			this.bodyLabel.Text = this.body;
		}
		
		public DialogResult ShowDialog(IWin32Window owner, string msg) {
			this.body = msg;
			this.bodyLabel.Text = this.body;
			return this.ShowDialog(owner);
		}
		
		void YesBtnClick(object sender, EventArgs e)
		{
			this.remember = this.rememberCB.Checked;
			this.DialogResult = DialogResult.Yes;
		}
		
		void NoBtnClick(object sender, EventArgs e)
		{
			this.remember = this.rememberCB.Checked;
			this.DialogResult = DialogResult.No;
		}
	}
}
