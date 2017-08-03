/*
 * Created by SharpDevelop.
 * User: Jake Gustafson, all rights reserved (Owner)
 * Date: 2/5/2006
 * Time: 1:44 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Threading;

namespace GoNowBackup
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.Timer timerStart;
		private System.Windows.Forms.Label lblFinished;
		private System.Windows.Forms.TextBox tbFolder;
		private System.Windows.Forms.StatusBar sbMain;
		private System.Windows.Forms.RichTextBox rtbError;
		private bool bBusy=false;
		private int iCount=0;
		public static StatusBar sbNow;
		public static RichTextBox rtbNow;
		//public static string sErrNow;
		public static string sLog {
			set {
				try {
					rtbNow.Visible=true;
					rtbNow.Text=value;
				}
				catch (Exception exn) {
					
				}
			}
		}
		public static string sStatus {
			set {
				try{
					sbNow.Text=value;
				}
				catch (Exception exn) {
					
				}
			}
		}
		private DirectoryInfo dirinfoStart;
		private int iDepth=1;
		private int iMaxDepth=0;//0=no limit
		private bool bListFiles=false;
		private ThreadStart deltsDoList;
		private Thread tDoList;
		public string sMyDocs;
		public string sFolder;
		string sFolderPreText;
		string sFilePreText;
		string sFolderSymbol="|-[-] ";
		string sFileSymbol="|- ";

		public MainForm() {
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			MainForm.sbNow=this.sbMain;
			//btnGo.Enabled=false;
			deltsDoList = new ThreadStart(DoList);
	 		tDoList = new Thread(deltsDoList);
	 		sMyDocs=Environment.GetFolderPath(Environment.SpecialFolder.Personal);
	 		tbFolder.Text=sMyDocs;
	 		rtbNow=rtbError;
		}
		
		[STAThread]
		public static void Main(string[] args)
		{
			Application.Run(new MainForm());
		}
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.rtbError = new System.Windows.Forms.RichTextBox();
			this.sbMain = new System.Windows.Forms.StatusBar();
			this.tbFolder = new System.Windows.Forms.TextBox();
			this.lblFinished = new System.Windows.Forms.Label();
			this.timerStart = new System.Windows.Forms.Timer(this.components);
			this.btnGo = new System.Windows.Forms.Button();
			this.nudDepth = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.nudDepth)).BeginInit();
			this.SuspendLayout();
			// 
			// rtbError
			// 
			this.rtbError.Dock = System.Windows.Forms.DockStyle.Right;
			this.rtbError.Location = new System.Drawing.Point(443, 32);
			this.rtbError.Name = "rtbError";
			this.rtbError.Size = new System.Drawing.Size(163, 276);
			this.rtbError.TabIndex = 3;
			this.rtbError.Text = "";
			this.rtbError.Visible = false;
			// 
			// sbMain
			// 
			this.sbMain.Location = new System.Drawing.Point(0, 308);
			this.sbMain.Name = "sbMain";
			this.sbMain.Size = new System.Drawing.Size(606, 8);
			this.sbMain.SizingGrip = false;
			this.sbMain.TabIndex = 2;
			this.sbMain.Text = "Loading...";
			// 
			// tbFolder
			// 
			this.tbFolder.Dock = System.Windows.Forms.DockStyle.Top;
			this.tbFolder.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tbFolder.Location = new System.Drawing.Point(0, 0);
			this.tbFolder.Name = "tbFolder";
			this.tbFolder.Size = new System.Drawing.Size(606, 32);
			this.tbFolder.TabIndex = 1;
			this.tbFolder.Text = "C:\\Documents and Settings";
			// 
			// lblFinished
			// 
			this.lblFinished.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblFinished.Location = new System.Drawing.Point(1, 1);
			this.lblFinished.Name = "lblFinished";
			this.lblFinished.Size = new System.Drawing.Size(0, 0);
			this.lblFinished.TabIndex = 4;
			this.lblFinished.Text = "Finished";
			this.lblFinished.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.lblFinished.Visible = false;
			// 
			// timerStart
			// 
			this.timerStart.Interval = 5000;
			this.timerStart.Tick += new System.EventHandler(this.TimerStartTick);
			// 
			// btnGo
			// 
			this.btnGo.Dock = System.Windows.Forms.DockStyle.Top;
			this.btnGo.Location = new System.Drawing.Point(0, 32);
			this.btnGo.Name = "btnGo";
			this.btnGo.Size = new System.Drawing.Size(443, 35);
			this.btnGo.TabIndex = 5;
			this.btnGo.Text = "Go";
			this.btnGo.UseVisualStyleBackColor = true;
			this.btnGo.Click += new System.EventHandler(this.Button1Click);
			// 
			// nudDepth
			// 
			this.nudDepth.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.nudDepth.Location = new System.Drawing.Point(0, 284);
			this.nudDepth.Name = "nudDepth";
			this.nudDepth.Size = new System.Drawing.Size(443, 24);
			this.nudDepth.TabIndex = 6;
			this.nudDepth.Value = new decimal(new int[] {
									4,
									0,
									0,
									0});
			this.nudDepth.ValueChanged += new System.EventHandler(this.NudDepthValueChanged);
			// 
			// label1
			// 
			this.label1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.label1.Location = new System.Drawing.Point(0, 280);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(443, 4);
			this.label1.TabIndex = 7;
			this.label1.Text = "Depth (0 for no limit):";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(7, 17);
			this.ClientSize = new System.Drawing.Size(606, 316);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.nudDepth);
			this.Controls.Add(this.btnGo);
			this.Controls.Add(this.rtbError);
			this.Controls.Add(this.sbMain);
			this.Controls.Add(this.tbFolder);
			this.Controls.Add(this.lblFinished);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "GoNowBackup";
			this.Closed += new System.EventHandler(this.MainFormClosed);
			this.Load += new System.EventHandler(this.MainFormLoad);
			((System.ComponentModel.ISupportInitialize)(this.nudDepth)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.Button btnGo;
		private System.Windows.Forms.NumericUpDown nudDepth;
		private System.Windows.Forms.Label label1;
		#endregion
		void Label1Click(object sender, System.EventArgs e)
		{
			
		}
		
		public void SubList(DirectoryInfo dirinfoNow, ref StreamWriter swList) {
			int iDepthPrev=iDepth;
			iDepth++;
			string sFolderPreTextPrev=sFolderPreText;
			string sFilePreTextPrev=sFilePreText;
			string sIndent="";
			for (int iDent=0; iDent<iDepthPrev; iDent++) {
				if (sIndent=="") sIndent="   ";
				else sIndent="   "+sIndent;
			}
			sFolderPreText=(sIndent=="")?sFolderSymbol:sIndent+sFolderSymbol;
			sFilePreText=(sIndent=="")?sFileSymbol:sIndent+sFileSymbol;
			foreach (DirectoryInfo dirinfoX in dirinfoNow.GetDirectories()) {
				swList.WriteLine(sFolderPreText+dirinfoX.Name);
				this.tbFolder.Text=dirinfoX.FullName;
				if (iDepth<iMaxDepth||iMaxDepth==0) SubList(dirinfoX, ref swList);
			}//end foreach folder
			if (bListFiles) FileList(dirinfoNow, ref swList);
			sFolderPreText=sFolderPreTextPrev;
			sFilePreText=sFilePreTextPrev;
			iDepth=iDepthPrev;
		}//end SubList
		public void FileList(DirectoryInfo dirinfoNow, ref StreamWriter swList) {
			iDepth++;
			int iDepthCurrent=iDepth;
			if (bListFiles) {
				foreach (FileInfo fiNow in dirinfoNow.GetFiles()) {
					swList.WriteLine(sFilePreText+fiNow.Name);
				}
			}
		}//end FileList
		public void DoList() {
			/*
			 * Assumes that these vars are set:
			 * iMaxDepth=(int)nudDepth.Value;
			 * bListFiles=true;
			 * sFolder=this.tbFolder.Text;
			 * tDoList.Start();
			*/

			//(values of global vars are set by the calling function to the values of form fields)
			try {
				dirinfoStart=new DirectoryInfo(sFolder);
				if (bBusy==false) {
					bBusy=true;
					iCount=0;
					//(adds no trailing slash to sMyDocs)
					string sMyFile=sMyDocs+"\\"+"Folders-Listed.txt";
					StreamWriter swList=new StreamWriter(sMyFile);
					swList.AutoFlush=false;
					//string sLine;
					this.sbMain.Text="Listing...";
					swList.WriteLine(dirinfoStart.FullName);
					foreach (DirectoryInfo dirinfoX in dirinfoStart.GetDirectories()) {
						iDepth=1;
						sFolderPreText=sFolderSymbol;
						sFilePreText=sFileSymbol;
						swList.WriteLine(sFolderPreText+dirinfoX.FullName);
						this.sbMain.Text=dirinfoX.FullName;
						SubList(dirinfoX, ref swList);
						//if (bListFiles) FileList(dirinfoX, ref swList);
					}
					this.sbMain.Text="Listing...done.  Writing file...";
					swList.Flush();
					swList.Close();
					this.sbMain.Text="Finished.";
					//this.lblFinished.Visible=true;
					//this.lblFinished.Show();
					bBusy=false;
				}
				/* 
				 * TODO: on close:
				 * try {
				 * 	tDoList.Abort();
				 * }
				 * catch (Exception exn) {
				 * }
				 * 
				 */

			}
			catch (Exception exn) {
				DoError("count="+iCount.ToString()+"  "+exn.ToString());
			}
			
		}
		public void DoError(string sErr) {
			sStatus="ERROR! "+sErr;
			sLog=sErr;
		}
		
		void MainFormLoad(object sender, System.EventArgs e) {
			nudDepth.Value=iMaxDepth;
			this.bListFiles=true;
			this.sbMain.Text = "";
		}
		
		void TimerStartTick(object sender, System.EventArgs e) {
			timerStart.Enabled=false;
		}
		
		void MainFormClosed(object sender, System.EventArgs e) {
			try {
				tDoList.Abort();
			}
			catch (Exception exn) {
				
			}
		}
		
		
		void Button1Click(object sender, System.EventArgs e) {
			iMaxDepth=(int)nudDepth.Value;
			bListFiles=true;
			sFolder=this.tbFolder.Text;
			tDoList.Start();
		}
		
		void NudDepthValueChanged(object sender, System.EventArgs e) {
			
		}
	}
}
