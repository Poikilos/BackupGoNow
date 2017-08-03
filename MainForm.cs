/*
 * Created by SharpDevelop.
 * User: Owner
 * Date: 10/5/2008
 * Time: 12:21 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Reflection;

namespace OrangejuiceElectronica
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		//TODO: Option to remove files from the backup drive that aren't in the backup script
		public static bool bDebug=false;
		public static ArrayList alInvalidDrives=new ArrayList();
		public static ArrayList alExtraDestinations=new ArrayList();
		public static string sFileScript="script.txt";
		public static string sFileMain="main.ini";
		public static MainForm mainformNow=null;
		public static ListBox lbOutNow=null;
		public static int iValidDrivesFound=0;
		public static int iDestinations=0;
		public static int iLBRightMargin=0;
		public static int iLBBottomMargin=0;
		public static bool bCloseErrorRedirect=false;
		public static int iTickLastRefresh=Environment.TickCount;
		public static int iTicksRefreshInterval=500;
		private static FolderLister flisterNow=null;
		private static bool bBusyCopying=true;
		private static bool bCancel=false;
		private static bool bExitIfNoUsableDrivesFound=false;
		private static bool bUserCancelledLastRun=false;
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
		}//end MainForm constructor

		public static bool ToBool(string sNow) {
			return sNow.ToLower()=="yes"||sNow=="1"||sNow.ToLower()=="true";
		}

		bool RunScript(string sFileX) {
			bool bGood=false;
			StreamReader streamIn=null;
			try {
				streamIn=new StreamReader(sFileX);
				string sLine;
				flisterNow=new FolderLister();
				flisterNow.bShowFolders=true;
				int iLine=0;
				int iListedLines=0;
				while ( (sLine=streamIn.ReadLine()) != null ) {
					int iMarker=sLine.IndexOf(":");
					if (iMarker>0 && sLine.Length>(iMarker+1)) {
						string sCommand=sLine.Substring(0,iMarker).ToLower();
						string sValue=sLine.Substring(iMarker+1);
						if (sCommand.StartsWith("#")) {
						    	//ignore
					    }
						else if (sCommand=="excludedest") {
							alInvalidDrives.Add(sValue);
							if (bDebug) Output("Not using "+sValue+" for backup");
						}
						else if (sCommand=="includedest") {
							alExtraDestinations.Add(sValue);
						}
					    else if (sCommand=="exclude") {
					    	FolderLister.alExclusions.Add(sValue);
					    	string sTemp="";
					    	foreach (string sExclusion in FolderLister.alExclusions) {
					    		sTemp+=(sTemp==""?"":", ")+sExclusion;
					    	}
					    	if (bDebug) Output("#Exclusions changed: "+sTemp);
					    }
					    else if (sCommand=="include") {
					    	FolderLister.alExclusions.Remove(sValue);
					    	string sTemp="";
					    	foreach (string sExclusion in FolderLister.alExclusions) {
					    		sTemp+=(sTemp==""?"":", ")+sExclusion;
					    	}
					    	if (bDebug) Output("#Exclusions changed: "+sTemp);
					    }
						else if (sCommand=="addfile") {
							BackupFile(sValue,false);
						}
						else if (sCommand=="addfolder") {
							flisterNow.sSearchRoot=sValue;
							string sDirSep=char.ToString(Path.DirectorySeparatorChar);
							Output("Loading \""+flisterNow.sSearchRoot+"\"...");
							string sTempFile=Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)+"FolderList.tmp";
							//FolderLister.SetOutputFile(sTempFile);
							flisterNow.StartRecordingLines();
							flisterNow.Start();
							btnCancel.Enabled=true;
							while (flisterNow.IsBusy) {
								Thread.Sleep(500);
								mainformNow.Refresh();
								Application.DoEvents();
							}
							bBusyCopying=true;
							Thread.Sleep(1000);
							string[] sarrListed=flisterNow.GetLines();
							//iListedLines=0;
							if (sarrListed!=null&&sarrListed.Length>0) {
								//if (File.Exists(sTempFile)) {
								//	StreamReader streamTemp=new StreamReader(sTempFile);
								//	string sLineNow;
								//	while ( (sLineNow=streamTemp.ReadLine()) != null ) {
								foreach (string sLineNow in sarrListed) {
									iListedLines++;
									FileAttributes fileattribNow = File.GetAttributes(sLineNow);
									FileInfo fiNow=new FileInfo(sLineNow);
									//if (fiNow.Attributes&FileAttributes.Directory
									if ((fileattribNow & FileAttributes.Directory) == FileAttributes.Directory) {
										ReconstructPathOnBackup(sLineNow);
									}
									else BackupFile(sLineNow,true);
									if (bDebug) Output(sLineNow,true);
									if (bCancel) {
										bCancel=false;
										break;
									}
								}
								bBusyCopying=false;
								btnCancel.Enabled=false;
								//	}
								//	streamTemp.Close();
								//	File.Delete(sTempFile);
								//	Thread.Sleep(500);
							}
							else Output("Could not find any files in the added folder.");
						}
						else if (sCommand=="exitifnousabledrivesfound") {
							bExitIfNoUsableDrivesFound=ToBool(sValue);
						}
					}//end if has ":" in right place
					iLine++;
				}
				//if (bDebug) {
					Output("Finished reading "+sFileX+" (listed: "+iListedLines+").",true);
				//}
				bGood=true;
			}
			catch (Exception exn) {
				string sMsg=sFileX+" could not be read.\n"+exn.ToString();
				if (bDebug) MessageBox.Show(sMsg,"Backup GoNow");
				Console.Error.WriteLine(sMsg);
				bGood=false;
			}
			try {
				if (streamIn!=null) streamIn.Close();
			}
			catch {}
			return bGood;
		}//end RunScript
		public string GetBackupDest() {
			string sReturn=this.cbDest.Text;
			string sDirSep=char.ToString(Path.DirectorySeparatorChar);
			if (!sReturn.EndsWith(sDirSep)) sReturn+=sDirSep;
			return sReturn;
		}
		private static bool bShowReconstructedBackupPathError=true;
		public string ReconstructedBackupPath(string sSrcPath) {
			if (bDebug) Output("Reconstruction(as received): "+sSrcPath);
			string sReturn=GetBackupDest();
			string sSubPath=sSrcPath;
			int iStart=0;
			if (sSubPath[iStart]=='/') {
				while (iStart<sSubPath.Length&&sSubPath[iStart]=='/') {
					iStart++;
				}
			}
			//else iStart=Chunker.IndexOfAnyDirectorySeparatorChar(sSubPath); //uncommenting this removes the "C" folder if using this program in windows to backup local files
			string sDirSep=char.ToString(Path.DirectorySeparatorChar);
			if (iStart>-1&&iStart<sSubPath.Length) {
				sSubPath=sSubPath.Substring(iStart);
				if (bDebug) Output("Reconstruction(before normalize): "+sSubPath);
				sSubPath=Chunker.ConvertDirectorySeparatorsToNormal(sSubPath);
				if (bDebug) Output("Reconstruction(before removedouble): "+sSubPath);
				sSubPath=Chunker.RemoveDoubleDirectorySeparators(sSubPath);
				if (sSubPath!=null&&sSubPath!=""&&sSubPath.StartsWith(sDirSep)) {
					if (sSubPath.Length>1) sSubPath=sSubPath.Substring(1);
					else sSubPath="";
				}
			}
			else sSubPath="";

			if (sSubPath=="") {
				if (bShowReconstructedBackupPathError) {
					MessageBox.Show("The backup source cannot be parsed so these files will be placed in the root of \""+GetBackupDest()+"\".");
					bShowReconstructedBackupPathError=false;
				}
				sReturn=GetBackupDest();
			}
			else sReturn+=sSubPath;
			if ( !sReturn.EndsWith(sDirSep) )
				sReturn+=sDirSep;
			return sReturn;
		}//end ReconstructedBackupPath
		public bool ReconstructPathOnBackup(string sSrcPath) {
			string sBackupFolder=ReconstructedBackupPath(sSrcPath);
			Chunker.CreateFolderRecursively(sBackupFolder);
			if (bDebug) Output("Created \""+sBackupFolder+"\"");
			return false;
		}
		public void BackupFile(string sSrcFilePath, bool bUseReconstructedPath) {
			try {
				FileInfo fiSrc=new FileInfo(sSrcFilePath);
				string sDirSep=char.ToString(Path.DirectorySeparatorChar);
				if (fiSrc.Exists) {
					string sBackupFolder=bUseReconstructedPath?ReconstructedBackupPath(fiSrc.Directory.FullName):this.cbDest.Text;
					if (!sBackupFolder.EndsWith(sDirSep)) sBackupFolder+=sDirSep;
					string sDestFile=sBackupFolder+fiSrc.Name;
					FileInfo fiDest=new FileInfo(sDestFile);
					if (fiDest.Exists) {
						if (fiDest.LastWriteTime<fiSrc.LastWriteTime) {
							if (!bDebug) File.Copy(sSrcFilePath,sDestFile,true);
							Output("Updating: \""+sDestFile+"\"");
						}
						else {
							//already newer or same timestamp so ignore
							Output("Was Up to Date: \""+sDestFile+"\"");
						}
					}
					else {
						if (!bDebug) File.Copy(sSrcFilePath,sDestFile);
						Output("Copied New: \""+sDestFile+"\"");
					}
				}
				else Output("Could not find \""+sSrcFilePath+"\"");
			}
			catch (Exception exn) {
				Console.Error.WriteLine("Error in BackupFile: ");
				Console.Error.WriteLine(exn.ToString());
				Console.Error.WriteLine();
			}
		}//end BackupFile
		public static bool bShowOutputException=true;
		public void Output(string sLineX) {
			Output(sLineX,false);
		}
		private static ArrayList alDisplayQueue=new ArrayList();
		public void Flush() {
			if (alDisplayQueue!=null&&alDisplayQueue.Count>0) {
				lbOutNow.BeginUpdate();
				foreach (string sNow in alDisplayQueue) {
					lbOutNow.Items.Add(sNow);
				}
				lbOutNow.EndUpdate();
				MainForm.lbOutNow.SelectedIndex=MainForm.lbOutNow.Items.Count-1;
				lbOutNow.Refresh();
				Application.DoEvents();
				mainformNow.Refresh();
				alDisplayQueue.Clear();
				iTickLastRefresh=Environment.TickCount;
			}
		}
		public void Output(string sLineX, bool bForceRefresh) {
			try {
				alDisplayQueue.Add(sLineX);
				if ( bForceRefresh || (Environment.TickCount-iTickLastRefresh>iTicksRefreshInterval) ) Flush();
			}
			catch (Exception exn) {
				if (bShowOutputException) {
					MessageBox.Show("Exception in Output: \n\n"+exn.ToString());
					bShowOutputException=false;
				}
			}
		}
		
		bool ValidDest(string sDrivePath) {
			bool bValid=true;
			foreach (string sInvalid in alInvalidDrives) {
				if (sDrivePath.ToLower()==sInvalid.ToLower()) {
					bValid=false;
					break;
				}
			}
			return bValid;
		}
		
		void CbDestSelectedIndexChanged(object sender, EventArgs e)
		{
			
		}
		
		void BtnGoClick(object sender, EventArgs e)
		{
			if (this.cbDest.Text!="") {
				btnGo.Enabled=false;
				bUserCancelledLastRun=false;
				if (!RunScript(sFileScript)) MessageBox.Show("Backup was not complete.");
				else {
					if (!bUserCancelledLastRun) MessageBox.Show("Finished Backup");
					else MessageBox.Show("Cancelled Backup");
					if (!bDebug) Application.Exit();
				}
			}
			else {
				MessageBox.Show("No destination drive is present.");
			}
		}
		
		void MainFormLoad(object sender, EventArgs e)
		{
			DateTime datetimeNow = DateTime.Now;
			string sFileErrLog = "1.ErrLog.txt";//"1.Errors "+datetimeNow.ToString("yyyyMMddHHmm") + ".log";
			TextWriter errStream = new StreamWriter(sFileErrLog);
			string sMyProcess = Assembly.GetExecutingAssembly().Location;
			//sMyProcess = sMyProcess.Substring(sMyProcess.LastIndexOf('\\') + 1);
			Console.SetError(errStream);
			Console.Error.Write("{0}", sMyProcess);
			Console.Error.WriteLine(": started at {0}.", datetimeNow);
			Console.Error.WriteLine();
			bCloseErrorRedirect=true;
      
			FolderLister.bDebug=bDebug;
			Chunker.bDebug=bDebug;
			lbOutNow=this.lbOut;
			RunScript(sFileMain);
			cbDest.BeginUpdate();
			cbDest.Items.Clear();
			string[] sarrDrive=Environment.GetLogicalDrives();
			foreach (string sDrivePathNow in sarrDrive) {
				if (ValidDest(sDrivePathNow)) {
					cbDest.Items.Add(sDrivePathNow);
					iValidDrivesFound++;
					iDestinations++;
				}
			}
			foreach (string sExtraDest in alExtraDestinations) {
				cbDest.Items.Add(sExtraDest);
				iDestinations++;
			}
			cbDest.EndUpdate();
			
			
			//FolderLister.Echo("Test");
			string sMsg="No backup drive can be found.  Try connecting the drive and then try again.";
			if (iValidDrivesFound+iDestinations>0) {
				if (bExitIfNoUsableDrivesFound&&iValidDrivesFound==0) {
					MessageBox.Show(sMsg);
					Application.Exit();
				}
				cbDest.SelectedIndex=0;
			}
			else {
				MessageBox.Show(sMsg);
				if (!bDebug) Application.Exit();
			}
			CalculateMargins();
			FixSize();
		}
		
		void CalculateMargins() {
			iLBRightMargin=this.Width-(lbOut.Left+lbOut.Width);
			iLBBottomMargin=this.Height-(lbOut.Top+lbOut.Height);
		}
		
		void FixSize() {
			lbOut.Width=this.Width-(iLBRightMargin+lbOut.Left);
			lbOut.Height=this.Height-(iLBBottomMargin+lbOut.Top);
			btnGo.Left=(this.Width-btnGo.Width)/2;
		}

		void CbDestTextChanged(object sender, EventArgs e)
		{
			bool bFound=false;
			foreach (string sNow in cbDest.Items) {
				if (cbDest.Text==sNow) bFound=true;
			}
			if (!bFound) cbDest.SelectedIndex=0;
			
		}
		
		void MainFormResize(object sender, EventArgs e)
		{
			FixSize();
		}
		
		void MainFormFormClosed(object sender, FormClosedEventArgs e)
		{
	      if (bCloseErrorRedirect) Console.Error.Close();
		}
		
		void BtnCancelClick(object sender, EventArgs e)
		{
			if (bBusyCopying) {
				bCancel=true;
				btnCancel.Enabled=false;
				bUserCancelledLastRun=true;
			}
			else {
				if (flisterNow!=null&&flisterNow.IsBusy) flisterNow.Stop();
				btnCancel.Enabled=false;
				bUserCancelledLastRun=true;
			}
		}
	}//end MainForm
}//end namespace
