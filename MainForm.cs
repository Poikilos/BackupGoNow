/*
 *  Created by SharpDevelop (To change this template use Tools | Options | Coding | Edit Standard Headers).
 * User: Jake Gustafson (Owner)
 * Date: 1/25/2007
 * Time: 10:08 AM
 * 
 */

using System;
//using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Diagnostics;

namespace GoNowBackup {
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm
	{
		//TODO: optional:
		//-Make a bCompress option
		//	-Allow no dest file if not compressed,
		//	-Uncomment all the lines to allow this
		//	-Set the backup variables and run DoList() for each call to AddToBatch()
		public static string sSelfName="GoNowBackup";
		public static string sBatch="lastrun.bat";
		public static string sCompressionMethod="zip";//can be "zip" or "7z" (command options may not work with 7z--check this)
		public static int iErrors=0;
		public static int iMaxErrors=100;
		//private int iTickLastRefresh=Environment.TickCount();
		//private DirectoryInfo dirinfoStart;
		//private int iDepth=1;
		//private int iMaxDepth=0;//0=no limit
		//private bool bListFiles=false;
		//public string sFolderPreText;
		//public string sFilePreText;
		//public string sFolderSymbol="|-[-] ";
		//public string sFileSymbol="|- ";

		#region backup options (set only once)
		public static string sTargetDriveRootWithSlash="";
		public static string sTargetDriveFolderNameOrIsADot="";
		public static string sCommand="update";
		public static string sTargetFile="";
		#endregion
		public static string sDirSepString=char.ToString(Path.DirectorySeparatorChar);
		#region backup variables (set once per action)
		//public string sFolder;
		public static bool bContinue=true;//false if program close button is pressed.
		public static bool bBusy=false;
		public static StreamWriter swBatch=null;
		//private int iCountFiles=0;
		//private int iCountFolders=0;
		public static int iErrorBoxesShown=0;
		public static TextBox tbStatusStatic;
		public static RichTextBox rtbOutputStatic;
		public static ArrayList alRootLines; //what to backup (INCLUDES COMMAND <>)
		public static string sSettingsFileName="settings.txt";
		#endregion
		
		[STAThread]
		public static void Main(string[] args) {
			Application.EnableVisualStyles();
			//TODO:?fix? Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
		
		public MainForm() {
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// constructor code after the InitializeComponent() call.
			//

			tbStatusStatic=this.tbStatus;
			rtbOutputStatic=this.rtbOutput;
		}
		/*
		public void SubList(DirectoryInfo dirinfoNow) {
			int iDepthPrev=iDepth;
			iDepth++;
			iCountFolders++;
			string sFolderPreTextPrev=sFolderPreText;
			string sFilePreTextPrev=sFilePreText;
			string sIndent="";
			try {
				for (int iDent=0; iDent<iDepthPrev; iDent++) {
					if (sIndent=="") sIndent="   ";
					else sIndent="   "+sIndent;
				}
				sFolderPreText=(sIndent=="")?sFolderSymbol:sIndent+sFolderSymbol;
				sFilePreText=(sIndent=="")?sFileSymbol:sIndent+sFileSymbol;
				foreach (DirectoryInfo dirinfoX in dirinfoNow.GetDirectories()) {
					MainForm.ShowMessageLine(sFolderPreText+dirinfoX.Name);
					//MainForm.ShowMessageLine(dirinfoX.FullName);
					if (iDepth<iMaxDepth||iMaxDepth==0) SubList(dirinfoX);
					iCountFolders++;
				}//end foreach folder
				FileList(dirinfoNow);
				sFolderPreText=sFolderPreTextPrev;
				sFilePreText=sFilePreTextPrev;
			}
			catch (Exception exn) {
				MainForm.ShowError("Exception error traversing subfolders: "+exn.ToString());
			}
			iDepth=iDepthPrev;
		}//end SubList
		public void FileList(DirectoryInfo dirinfoNow) {
			iDepth++;
			int iDepthCurrent=iDepth;
			foreach (FileInfo fiNow in dirinfoNow.GetFiles()) {
				MainForm.ShowMessageLine(sFilePreText+fiNow.Name);
				iCountFiles++;
			}
		}//end FileList
		public void DoList() {
			//(values of global vars are set by the calling function to the values of form fields)
			try {
				dirinfoStart=new DirectoryInfo(sFolder);
				if (bBusy==false) {
					bBusy=true;
					iCountFiles=0;
					iCountFolders=0;
					//string sLine;
					MainForm.ShowMessageLine();
					MainForm.ShowMessageLine("Adding files from \""+sFolder+"\"...");
					//MainForm.ShowMessageLine(dirinfoStart.FullName);
					foreach (DirectoryInfo dirinfoX in dirinfoStart.GetDirectories()) {
						iDepth=1;
						sFolderPreText=sFolderSymbol;
						sFilePreText=sFileSymbol;
						MainForm.ShowMessageLine(sFolderPreText+dirinfoX.FullName);
						//MainForm.ShowMessageLine(dirinfoX.FullName);
						SubList(dirinfoX);
					}
					//MainForm.ShowMessageLine("Listing...done.  Writing file...");
					//MainForm.ShowMessageLine("Finished.");
					//this.lblFinished.Visible=true;
					//this.lblFinished.Show();
					bBusy=false;
				}
			}
			catch (Exception exn) {
				MainForm.ShowError("Exception error during list ("+iCountFiles.ToString()+") files and "+iCountFolders+" folders so far: "+exn.ToString());
			}
		}
		*/
		public static void ShowError(string sErr) {
			tbStatusStatic.Text=sErr;
			if (iErrors<iMaxErrors) rtbOutputStatic.Text+=sErr+"\n";
			else if (iErrors==iMaxErrors) {
				rtbOutputStatic.Text+="Too many errors, so this is the last message that will be shown: \n"
					+sErr+"\n";
			}
			Application.DoEvents();
			iErrors++;
		}
		public static void ShowMessageLine() {
			rtbOutputStatic.Text+="\n";
			Application.DoEvents();
		}
		public static void ShowMessageLine(string sLine) {
			ShowMessage(((sLine!=null)?sLine:"")+"\n");
		}
		public static void ShowMessage(string sMsg) {
			try {
				rtbOutputStatic.Text+=sMsg;
				tbStatusStatic.Text=sMsg;
				Application.DoEvents();
			}
			catch (Exception exn) {
				//do not report this
			}
		}
		public static bool SafeShow(string sNow) {
			bool bShown=false;
			string sTemp;
			if (iErrorBoxesShown<1) {
				if (sNow!=null) {
					sTemp=sNow.Replace(" ","");
					sTemp=sTemp.Replace("\n","");
					sTemp=sTemp.Replace("\r","");
					sTemp=sTemp.Replace("\t","");
					if (sTemp!=null&&sTemp!="") {
						sTemp=sNow;
					}
					else sTemp="Unknown Error";
					MessageBox.Show(sTemp);
					iErrorBoxesShown++;
				}
			}
			return bShown;
		}
		public static string SafeDriveStringFromLabelOrRealRootString(string sValue) {
			string sReturn="";
			if (sValue==null) {
				sReturn="";
			}
			else if (sValue.EndsWith(sDirSepString)) {
				sReturn=sValue;
			}
			else if (sValue.EndsWith("/")) {
				sReturn=sValue;
			}
			else if (sValue.EndsWith(":")) {
				sReturn=sValue+sDirSepString; //OK only since already made sure that DirectorySeparatorChar isn't ":"
			}
			else {//else look for drive by label
				int iLettersMaxUsed=0;
				int iLettersMax=26;
				int iFound=-1;
				DriveInfo[] diarrList=null;
				try {
					diarrList=DriveInfo.GetDrives();
					if (diarrList!=null) {
						for (int iNow=0; iNow<diarrList.Length; iNow++) {
							if ( diarrList[iNow].IsReady
							    && diarrList[iNow].VolumeLabel==sValue) {
								iFound=iNow;
								break;
							}
						}
						if (iFound>=0) {
							sReturn=diarrList[iFound].RootDirectory.ToString();
							if (!sReturn.EndsWith(sDirSepString)) {
								sReturn+=sDirSepString;//debug -- could this CAUSE a problem???
							}
							//System.Diagnostics.Process procNow=new System.Diagnostics.Process();
							//System.Diagnostics.Process.Start(diarrList[iFound].RootDirectory.ToString());
						}
						else {//not found
							SafeShow("Could not find drive labeled \""+sValue+"\" -- make sure that" +
							                " the drive is connected and that your drive has actually been set to "+sValue);
							sReturn="";
						}
					}
					else {
						MainForm.ShowError("Your computer couldn't show a drive list.  It is possible that is program is not compatible with your computer or that a drive is not working properly.  If you have a modern desktop or laptop computer, it is more likely that one of your drives is busy with another task.");
						sReturn="";
					}
				}
				catch (Exception exn) {
					MainForm.ShowError("Drive labeled not ready or not present.  \n\nError details:\n"+exn.ToString());
					sReturn="";
				}
			}
			return sReturn;
		}
		public static bool AssignVarByActionType(int iActionType, string sValue) {
			bool bFound=false;
			if (iActionType==Action.TypeSetTargetDrive) {
				sTargetDriveRootWithSlash=SafeDriveStringFromLabelOrRealRootString(sValue);
				//if (sTargetDriveRootWithSlash.Length<=0) { //do not do this yet.  Will be checked upon trying to run backup.
			}
			else if (iActionType==Action.TypeSetCommand) {
				sCommand=sValue;
			}
			else if (iActionType==Action.TypeSetTargetFile) {
				sTargetFile=sValue;
			}
			else if (iActionType==Action.TypeSetTargetFolder) {
				sTargetDriveFolderNameOrIsADot=sValue;
			}
			return bFound;
		}
		public static bool CheckVars() {
			bool bCheck= 
					sTargetDriveFolderNameOrIsADot!=null && sTargetDriveFolderNameOrIsADot.Length>0
				&&	sTargetFile!=null && sTargetFile.Length>0
				&&	sTargetDriveRootWithSlash!=null && sTargetDriveRootWithSlash.Length>0
				&&	sCommand!=null && sCommand.Length>0
				&& alRootLines!=null && alRootLines.Count>0;
			return bCheck;
		}
		public static int LoadSettingsFile() {
			alRootLines=new ArrayList();
			string sLine=" ";
			string sValueSubstringNow;
			int iSources=-1;//-1 is error state
			int iLines=0;//not really used yet as of 2007-01
			try {
				if (File.Exists(sSettingsFileName)) {
				    StreamReader srNow=new StreamReader(sSettingsFileName);
					while (sLine != null) {
				    	sLine = srNow.ReadLine();
				    	if (sLine != null) {
				    		if (sLine.Length>0 && !sLine.StartsWith("#")) { //ignores blank lines and comments
				    			int iActionType=Action.FromLine(out sValueSubstringNow, sLine);
				    			if (Action.IsUsable(iActionType)) {
					    			if (Action.IsAssignment(iActionType)) {
				    					AssignVarByActionType(iActionType,sValueSubstringNow);
					    			}
				    				else {
				    					alRootLines.Add(sLine);
					    				if (iSources==-1) iSources=1;
					    				else iSources++;
				    				}
				    			}
				    			else {
				    				MainForm.ShowError("Parser could not interpret line action.");
				    			}
				    			iLines++;
				    		}
				    	}
				    }
				    srNow.Close();
				}//end if settings file exists
			}
			catch (Exception exn) {
				SafeShow("The failed to load the settings file.\n\n"+exn.ToString());
				MainForm.ShowError("Exception during : "+exn.ToString());
				iSources=-1;
			}
			return iSources;
		}//end LoadSettingsFile()
		public static bool AnyExists(string sLocation) {
			FileInfo fiNow;
			bool bReturn;
			fiNow = new FileInfo(sLocation);
			bReturn=fiNow.Exists;
			if (!bReturn) {
				DirectoryInfo diNow=new DirectoryInfo(sLocation);
				bReturn=diNow.Exists;
			}
			return bReturn;
		}
		public static bool AddToBatch(string sLocation, bool IsFolder, bool DoSubfolders) {
			bool bGood=false;
			string sTargetNow;
			try {
				sTargetNow=sTargetDriveRootWithSlash
					+((sTargetDriveFolderNameOrIsADot==".")?"":sTargetDriveFolderNameOrIsADot+sDirSepString)
					+sTargetFile+"."+sCompressionMethod;
				if (swBatch==null) {
					if (sCommand=="replace" && AnyExists(sTargetNow)) {
						File.Delete(sTargetNow);
					}
					swBatch=new StreamWriter(sBatch);
				}
				string sTypeNote=((IsFolder)?"folder":"file");
				string sFolderNote=((DoSubfolders)?" with subfolders":(IsFolder?" without subfolders":" "));
				string sLocAppend="";//(DoSubfolders&&IsFolder)?( ((!sLocation.EndsWith(MainForm.sDirSepString))?MainForm.sDirSepString:"") + "*" ):"";//TO NOT DO (would put them in the root of the zip instead of subfolder
				MainForm.ShowMessage("Adding "+sTypeNote+sFolderNote+" \""+sLocation+sLocAppend+"\"...");
				if (AnyExists(sLocation)) {
					swBatch.WriteLine("REM copy "+sTypeNote+sFolderNote+":");
					swBatch.WriteLine("7za "+((sCommand=="replace")?"a":"u")
					                  +" -t"/*no space*/+sCompressionMethod+" \""+sTargetNow+"\" "
					                  +"\""+sLocation+sLocAppend+"\""+" -y "+((DoSubfolders&&IsFolder)?"-r ":"-r- ")+"-mx9");
					bGood=true;
					MainForm.ShowMessageLine("Success.");
				}
				else {
					bGood=false;//this will be noted by DoAllLists so no need to show error.
					MainForm.ShowMessageLine("Failed.");
					MainForm.ShowError("This file/folder does not exist, and will not be processed: \""+sLocation+"\".");
				}
			}
			catch (Exception exn) {
				MainForm.ShowError(exn.ToString());
			}
			return bGood;
		}//end AddToBatch
		public static bool RunBatch() {
			bBusy=true;
			if (bContinue==false)
				return false;
			
			bool bGood=false;
			try {
				MainForm.ShowMessage("Finalizing script...");
				try {swBatch.Close();}
				catch (Exception exn) {
					MainForm.ShowError("Couldn't close script: "+exn.ToString());
				}
				MainForm.ShowMessageLine("Success.");
				MainForm.ShowMessage("Running backup script...");

				Process proc = new Process();
				// Redirect the output stream of the child process.
				proc.StartInfo.UseShellExecute = false;
				//proc.StartInfo.RedirectStandardError = true;
				proc.StartInfo.FileName = sBatch;
				proc.StartInfo.CreateNoWindow=true;
				proc.StartInfo.WorkingDirectory=Application.StartupPath;
				proc.Start();
				proc.WaitForExit(); //TODO: make it async and stop if bContinue becomes false.
				string sErr = "";
				//sErr=proc.StandardOutput.ReadToEnd();
				//try {
				//	sErr=//was going to get it from file here.
				//}
				//catch (Exception exn) {//do not report this
				//	sErr="";
				//}
				ShowMessageLine("Success.");
				string sResult="Finished backup script.  Press OK to close.";
				bGood=true;
				if (sErr!=null && sErr.Length>0) sResult+=" with errors:\n"+sErr+"\n\nPress OK to close.";
				else sResult+=".";
				SafeShow(sResult);
				Application.Exit();
			}
			catch (Exception exn) {
				string sTemp="Exception error running backup script: \n\n"+exn.ToString();
				MainForm.ShowError(sTemp);
				SafeShow(sTemp);
			}
			return bGood;
		}
		public static void DoAllLists() {
			int iActionNow=-1;
			bool bGood=true;
			string sActionValueSubstringNow;
			try {
				foreach (string sLine in alRootLines) {
					iActionNow=Action.FromLine(out sActionValueSubstringNow, sLine);
					if (Action.IsUsable(iActionNow) && !Action.IsAssignment(iActionNow)) {
						if (iActionNow==Action.TypeFolderRecursive) {
							//iMaxDepth=0;
							//sFolder=sActionValueSubstringNow;
							bGood=AddToBatch(sActionValueSubstringNow,true,true);
						}
						else if (iActionNow==Action.TypeFolder) {
							//iMaxDepth=1;
							//sFolder=sActionValueSubstringNow;
							bGood=AddToBatch(sActionValueSubstringNow,true,false);
						}
						else if (iActionNow==Action.TypeFile) {
							bGood=AddToBatch(sActionValueSubstringNow,false,false);
						}
						else bGood=false;
					}
					else {//else is unusable or is an assignment
						bGood=false;
					}
					if (!bGood) MainForm.ShowError("One of the sources to backup (line: \""+sLine+"\") was not correctly specified in "+sSettingsFileName+" in "+Application.StartupPath);
				}
				RunBatch();
			}
			catch (Exception exn) {
				string sTemp="Exception Error processing list:\n\n"+exn.ToString();
				SafeShow(sTemp);
				MainForm.ShowError(sTemp);
			}
		}
		void Go() {
			bool bGood=true;
			int iTargetsNow=-1;
			try {
				if (AnyExists(sSettingsFileName)) {
					iTargetsNow=LoadSettingsFile();
					string sTemp;
					if (iTargetsNow<1) {
						sTemp="Error, no valid folders to backup were found.  See settings.txt";
						SafeShow(sTemp);
						bGood=false;
					}
					else if (CheckVars()) {
						DoAllLists();
					}
					else {
						SafeShow("Error: Settings file is not complete.  See "+sSettingsFileName+" in "+Application.StartupPath+".");
						bGood=false;
					}
				}
				else {
					SafeShow("Error: Settings file not found.");//TODO: show configuration info here
				}
			}
			catch (Exception exn) {
				SafeShow("Program failed to load.\n\n"+exn.ToString());
				MainForm.ShowError("Exception during program load: "+exn.ToString());
			}
			Application.Exit();
		}
		void MainFormLoad(object sender, System.EventArgs e) {
			timerStart.Enabled=true;
			timerStart.Interval=500;
			timerStart.Start();
			//Application.DoEvents();
			//Go();
		}
		
		void MainFormFormClosing(object sender, System.EventArgs e)//System.Windows.Forms.FormClosingEventArgs e)
		{
			bContinue=false;
		}
		
		void MainFormFormClosed(object sender, System.EventArgs e)//System.Windows.Forms.FormClosedEventArgs e)
		{
		}
		bool bDone=false;
		void TimerStartTick(object sender, System.EventArgs e) {
			if (!bDone) {
				bDone=true;
				timerStart.Enabled=false;//must happen BEFORE go
				Go();
			}
			timerStart.Enabled=false;//intentionally redundant
		}
		
		void RtbOutputTextChanged(object sender, System.EventArgs e)
		{
			
		}
	}
}
