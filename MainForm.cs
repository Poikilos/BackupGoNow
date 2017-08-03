/*
 * Created by SharpDevelop.
 * User: Owner
 * Date: 10/5/2008
 * Time: 12:21 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
//using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
//using System.Threading;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
//using System.Management;//for getting free disk space (ManagementObject)
//using System.Text;//StringBuilder etc

namespace JakeGustafson {
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form {
		public static string sMyNameAndVersion="Backup GoNow 2011-02-21";
		public static string sMyName="Backup GoNow";
		ArrayList alPseudoRootsNow=null;
		ArrayList alSelectableDrives=null;
		//TODO: Option to remove files from the backup drive that aren't in the backup script
		public static int iLine=0;
		public static int iListedLines=0;
		public static int iCouldNotFinish=0;
		public static string DefaultProfile_Name="BackupGoNowDefault";
		public static bool bLoadedProfile=false;
		public static int iMaxCopyErrorsToShow=10;
		public static bool bUserSaysStayOpen=false;
		public static string sCopyErrorFileFullNameCloser=" -- ";
		public static string sCopyErrorFileFullNameOpener=": ";
		public static bool bRemoveTrivialMessagesAfterScript=true;
		public static bool bOutputTrivial=false;
		public static bool bWriteBatchForFailedFiles=true;
		public static string RetryBatchFile_Name="retry-last.bat";
		public static StreamWriter streamBatchRetry=null;
		//public static string sFileErrLog = "1.ErrLog.txt";//"1.Errors "+datetimeNow.ToString("yyyyMMddHHmm") + ".log";
		public static TextWriter errStream = null;
		public static string BackupProfileFolder_FullName="";
		public static bool bDeleteFilesNotOnSource_AfterCopyingEachFolder=true;
		public static bool bDeleteFilesNotOnSource_BeforeBackup=false;
		//public static bool bRealTime=true;
		public static System.Windows.Forms.TextBox tbStatusNow=null;
		public static int iDepth=0;
		public static int iFilesProcessed=0;
		public static bool bTestOnly=false;
		public static bool bAutoScroll=true;
		public static string sAppName="Backup GoNow";
		public static string ScriptFile_Name="script.txt";
		public static string StartupFile_Name="startup.ini";
		public static string MainFile_Name="main.ini";
		public static string LogFile_Name="summary.log";
		public static readonly string OutputFile_Name="Backup GoNow output.txt";
		public static readonly string sLastRunLog="1.LastRun Output.txt";
		public static string OutputFile_FullName="";//fixed in MainFormLoad
		public static MainForm mainformNow=null;
		public static ListBox lbOutNow=null;
		public static ulong ulByteCountTotalProcessed_LastRun=0;
		public static int iLBRightMargin=0;
		public static int iLBBottomMargin=0;
		public static int iTickLastRefresh=Environment.TickCount;
		public static int iTicksRefreshInterval=2000;
		//private static FolderLister flisterNow=null;
		//private static bool bBusyCopying=false;
		private static bool bExitIfNoUsableDrivesFound=false;
		private static bool bAlwaysStayOpen=false;
		private static bool bUserCancelledLastRun=false;
		private static bool bCopyErrorLastRun=false;
		private static bool bDiskFullLastRun=false;
		//private static bool iFilesTooBigToFit=0;
		private static int iSkipped=0;
		private static ArrayList alSkippedDueToException=new ArrayList(); //formerly alSkipped
		//private static ArrayList CurrentFolder_alSkippedDueToException=new ArrayList();
		private static ArrayList alCopyError=new ArrayList();
		private static string sCP="";//fixed later
		private static string sMkdir="";//fixed later
		//private const int StatusOK=0;
		//private const int StatusUserCancel=1;
		//private const int StatusCopyError=2;//copy error is NOT a reason to stop
		//private const int StatusDiskFull=3;
		//private static int iStatus=StatusOK;
		private static ulong ulByteCountFolderNow=0;
		private static ulong ulByteCountFolderNowDone=0;
		private static ulong ulByteCountTotal=0;
		private static ulong ulByteCountTotalProcessed=0;
		private static ulong ulByteCountTotalActuallyCopied=0;
		private static long lByteCountTotalActuallyAdded=0;
		
		public static ArrayList alFilesBackedUpManually=new ArrayList();
		private static long ulByteCountDestTotalSize=0;
		private static long ulByteCountDestAvailableFreeSpace=0;
		private static string DestFolder_FullName="";
		private static string DestSubfolderRelNameThenSlash="";
		
		private static ArrayList alFolderFullName=new ArrayList();
		private static ArrayList alFolderLabel=new ArrayList();
		private static ArrayList alFolder=new ArrayList();
		
		//private static string GetDestDriveRoot() { //="";//drive or folder
		//	try {
		//		return (driveinfoarrSelectableDrive!=null&&iDriveDest>-1&&iDriveDest<driveinfoarrSelectableDrive.Length) ? locinfoarrPseudoRoot[DestFolderIndexNow].RootDirectory.FullName : "";
		//	}
		//	catch (Exception exn) {
		//		string sMsg="Error getting drive root folder: "+ToOneLine(exn);
		//		Output(sMsg);
		//		Console.Error.WriteLine(sMsg);
		//	}
		//	return "";
		//}
		bool SaveOutputToTextFile() {
			bool bGood=false;
			StreamWriter streamOut=null;
			try {
				streamOut=new StreamWriter(OutputFile_FullName);
				DateTime dtNow=DateTime.Now;
				streamOut.WriteLine("# "+dtNow.Year.ToString()+"-"+dtNow.Month+"-"+dtNow.Day+" "+dtNow.Hour+":"+dtNow.Minute+":"+dtNow.Second);
				for (int i=0; i<this.lbOut.Items.Count; i++) {
					streamOut.WriteLine(this.lbOut.Items[i].ToString());
				}
				bGood=true;
				streamOut.Close();
			}
			catch (Exception exn) {
				bGood=false;
			}
			return bGood;
		}//end SaveOutputToTextFile

		public static string sWasUpToDate="Was Up to Date";
		public static Brush brushItemOther = Brushes.Black;
		public static SolidBrush brushItemWasUpToDate = new SolidBrush(Color.FromArgb(192, 192, 192)); //Brushes.Gray;
		//public static int iDriveDest=0;
		//public static string sDestRootThenSlash="";
		public static bool bStartedCopyingAnyFiles=false;
		//private static string sDestPathSlash {
		//	get { return sDestRootThenSlash+DestSubfolderRelNameThenSlash; }
		//}
		public MainForm() {
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			if (Path.DirectorySeparatorChar!='\\') {
				sCP="cp -f";
				sMkdir="mkdir";
			}
			else {
				sCP="copy /y";
				sMkdir="md";
			}
			
			if (File.Exists(RetryBatchFile_Name)) {
				try {
					Common.sParticiple="generating retry batch \".old\" backup filename";
					string sOldBat=RetryBatchFile_Name+".old";
					Common.sParticiple="deleting old retry batch";
					if (File.Exists(sOldBat)) {
						try {
							File.Delete(sOldBat);
						}
						catch (Exception exn) {
							Common.ShowExn(exn,Common.sParticiple);
						}
					}
					Common.sParticiple="moving previous retry batch to \""+sOldBat+"\"";
					File.Move(RetryBatchFile_Name,sOldBat);
				}
				catch (Exception exn) {
					Common.ShowExn(exn,Common.sParticiple);
				}
			}
			//The file will be appended for each write for safety (see WriteRetryLineIfCreatingRetryBatch)
			//if (bWriteBatchForFailedFiles) {
			//	try {
			//		streamBatchRetry=new StreamWriter(RetryBatchFile_Name);
			//	}
			//	catch (Exception exn) {
			//		streamBatchRetry=null;
			//	}
			//}

			InitializeComponent();
			string Executable_FullName=System.Reflection.Assembly.GetExecutingAssembly().Location;
			try {
				FileInfo fiNow=new FileInfo(Executable_FullName);
				string ExecutableFolder_FullName=fiNow.DirectoryName;
				//Directory.SetCurrentDirectory(ExecutableFolder_FullName);
			}
			catch (Exception exn) {
				Common.ShowExn(exn,"changing to directory of executable","MainForm constructor");
			}
			tbStatusNow=this.tbStatus;
			//string sMyPath = Assembly.GetExecutingAssembly().Location;
			//int iLastSlash=sMyPath.LastIndexOf(char.ToString(Path.DirectorySeparatorChar));
			//if (iLastSlash>-1) {
			//	sMyPath=sMyPath.Substring(0,iLastSlash);
			//	Directory.SetCurrentDirectory(sMyPath);
			//}
		}//end MainForm constructor
		private void WriteRetryLineIfCreatingRetryBatch(string sLine) {
			if (bWriteBatchForFailedFiles) {
				try {
					streamBatchRetry=File.AppendText(MainForm.RetryBatchFile_Name);
					if (streamBatchRetry!=null) {
						streamBatchRetry.WriteLine(sLine);
						streamBatchRetry.Close();
					}
				}
				catch (Exception exn) {
					Common.ShowExn(exn,"adding batch retry line");
				}
			}
		}
		~MainForm() {
			if (errStream!=null) errStream.Close();
		}
		public static bool ToBool(string sNow) {
			return sNow.ToLower()=="yes"||sNow=="1"||sNow.ToLower()=="true";
		}
		void BackupFolder(DirectoryInfo diBase) {
			iDepth++;
			try {
				//if (bTestOnly) Output("Getting ready to copy "+(diBase.Size/1024/1024).ToString()+"MB...");			
				foreach (DirectoryInfo diNow in diBase.GetDirectories()) {
					if (bUserCancelledLastRun) break;
					if (!Common.IsExcludedFolder(diNow)) {
						ReconstructPathOnBackup(diNow.FullName);
						if (!bUserCancelledLastRun&&!bDiskFullLastRun
							&&!Common.IsExcludedFolder(diNow))//&&flisterNow.UseFolder(diNow))
							BackupFolder(diNow);
					}
				}
			}
			catch {} //no subfolders
			
			FileInfo[] fiarrSrc=null;
			DirectoryInfo[] diarrSrc=null;
			//bool[] barrUsedSrcFile=null;
			//ArrayList alActuallyUsedSrcFiles=null;//new ArrayList();
			int iSrcNow=0;
			try {
				bool bSourceListingWasCancelled=false;
				fiarrSrc=diBase.GetFiles();
				diarrSrc=diBase.GetDirectories();
				//if (bDeleteFilesNotOnSource_AfterCopyingEachFolder) alActuallyUsedSrcFiles=new ArrayList();
				//if (fiarrSrc!=null&&fiarrSrc.Count>0) barrUsedSrcFile=new bool[fiarrSrc.Count];
				//if (fiarrSrc==null) bSourceListingWasCancelled=true; this is ok
				foreach (FileInfo fiNow in fiarrSrc) {
					if (bUserCancelledLastRun||bDiskFullLastRun) {
						bSourceListingWasCancelled=true;
						break;
					}
					if (!Common.IsExcludedFile(diBase,fiNow)) {//if (flisterNow.UseFile(diBase,fiNow)) {
						//barrUsedSrcFile[iSrcNow]=true;
						//lbOut.Items.Add(fiNow.FullName+" not excluded by "+Common.MasksToCSV());//debug only
						BackupFile(fiNow.FullName,true);
						if (bTestOnly) Output("  ("+fiNow.FullName+")",true);
						//if (bDeleteFilesNotOnSource_AfterCopyingEachFolder) alActuallyUsedSrcFiles.Add(fiNow.Name);
						//iSrcNow++;
					}
					if (bUserCancelledLastRun) break;
				}//end foreach file
				if (bDeleteFilesNotOnSource_AfterCopyingEachFolder&&!bSourceListingWasCancelled) {
					DirectoryInfo diTarget=new DirectoryInfo(ReconstructedBackupPath(diBase.FullName));
					bool bFoundOnSource=false;
					foreach (DirectoryInfo diDest in diTarget.GetDirectories()) {
						bFoundOnSource=false;
						if (bUserCancelledLastRun) break;
						foreach (DirectoryInfo diSource in diarrSrc) {
							if (bUserCancelledLastRun) break;
							if (diDest.Name==diSource.Name) {
								bFoundOnSource=true;
								break;
							}
						}
						if (!bFoundOnSource) {
							string DeletedFolder_FullName=diDest.FullName;
							MainForm.Output("Removing deleted/moved folder from backup: "+DeletedFolder_FullName,true);
							DeleteFolderRecursively(diDest,true);//use my method so that lByteCountTotalActuallyAdded is decremented//diDest.Delete(true);
						}
					}
					foreach (FileInfo fiDest in diTarget.GetFiles()) {
						bFoundOnSource=false;
						//iSrcNow=0;
						if (bUserCancelledLastRun) break;
						foreach (FileInfo fiSource in fiarrSrc) {
							if (bUserCancelledLastRun) break;
							if (fiDest.Name==fiSource.Name) {
								bFoundOnSource=true;
								break;
							}
						}//checking against fiarrSrc without filtering prevents successive mutually-exclusive backups from causing files to be deleted that shouldn't be like commented code below does
						
						/*
						foreach (string sKeep in alActuallyUsedSrcFiles) {//FileInfo fiKeep in fiarrSrc) {
							if (fiDest.Name==sKeep) {//fiDest.Name==fiKeep.Name) {//TODO: MUST NOT delete if ARCHIVE is present if compression is implemented in BackupFile feature!
								bFoundOnSource=true;
								break;
							}
						}
						if (alFilesBackedUpManually!=null&&alFilesBackedUpManually.Count>0) {
							foreach (string sKeep in alFilesBackedUpManually) {
								if (fiDest.FullName==sKeep) {//fiDest.Name==fiKeep.Name) {//TODO: MUST NOT delete if ARCHIVE is present if compression is implemented in BackupFile feature!
									bFoundOnSource=true;
									break;
								}
							}
						}
						*/
						if (!bFoundOnSource) {
							Output("Removing deleted/moved file from backup: \""+fiDest.FullName+"\"");
							fiDest.Attributes=FileAttributes.Normal;//fiDest.Attributes^= FileAttributes.ReadOnly;
							fiDest.Delete();
							lByteCountTotalActuallyAdded-=fiDest.Length;
						}
						if (bUserCancelledLastRun||bDiskFullLastRun) break; //note: do NOT stop if Copy Error only
					}//end for each file to destination (to check if should be deleted)
				}//end if delete files not on source
			}
			catch {} //no files
			iDepth--;
		}//end BackupFolder recursively
		bool RunScript(string sFileX) {
			if (alSkippedDueToException!=null||alCopyError!=null) { //TODO: recheck logic.  This used to be done below (see identical commented lines)
				if (alSkippedDueToException.Count!=0||alCopyError.Count>0) {
					Output("Clearing error cache...",true);
				}
				alSkippedDueToException.Clear();
				alCopyError.Clear();
			}
			int iFilesProcessedPrev=iFilesProcessed;
			int iFilesProcessed_ThisScript=0;
			bool bGood=false;
			StreamReader streamIn=null;
			iCouldNotFinish=0;
			try {
				if (alSkippedDueToException!=null) alSkippedDueToException.Clear();
				else alSkippedDueToException=new ArrayList();
				if (!File.Exists(sFileX)) {
					Console.Error.WriteLine("File does not exist: \"" + sFileX + "\"!");
				}
				else {
					Console.Error.WriteLine("Reading \"" + sFileX + "\":");
				}
				streamIn=new StreamReader(sFileX);
				string sLine;
				//flisterNow=new FolderLister();
				//flisterNow.MinimumFileSize=1;//1byte (trying to avoid bad symlinks here)
				//flisterNow.bShowFolders=true;
				iLine=0;
				iListedLines=0;
				while ( (sLine=streamIn.ReadLine()) != null ) {
					if (bUserCancelledLastRun) {
						bUserCancelledLastRun=true;
						break;
					}
					if (sLine.StartsWith("#")) Console.Error.WriteLine("\t"+sLine);
					RunScriptLine(sLine);
					iLine++;
					if (bDiskFullLastRun||bUserCancelledLastRun) break; //do NOT stop if Copy Error only
				}//end while lines in script
				//if (bTestOnly) {
				if (alSkippedDueToException.Count>0) {
					Output("");
					Output("Could not list "+alSkippedDueToException.Count.ToString()+":",true);
					foreach (string sSkippedNow in alSkippedDueToException) {
						if (bUserCancelledLastRun) break;
						Output("(could not list) "+sSkippedNow);
					}
				}
				if (alCopyError.Count>0) {
					Output("");
					Output("Could not copy "+alCopyError.Count.ToString(),true);
					foreach (string sCopyErrorNow in alCopyError) {
						if (bUserCancelledLastRun) break;
						Output("(could not copy) "+sCopyErrorNow);
					}
					Output("");
				}
				iFilesProcessed_ThisScript=iFilesProcessed-iFilesProcessedPrev;
				if (!bUserCancelledLastRun) {
					if (iListedLines+alSkippedDueToException.Count+alCopyError.Count+iFilesProcessed_ThisScript>0) {
						Output("Finished reading "+sFileX+" (listed: "+iListedLines+"; could not list: "+alSkippedDueToException.Count.ToString()+"; copy errors: "+alCopyError.Count.ToString()+"; files listed:"+iFilesProcessed_ThisScript.ToString()+").",true);
					}
					else Output("Finished reading "+sFileX+" (commands processed)");
				}
				else Output("Script was cancelled by user.");
				//if (alSkippedDueToException!=null||alCopyError!=null) {
				//	if (alSkippedDueToException.Count!=0||alCopyError.Count>0) {
				//		Output("Clearing error cache...",true);
				//	}
				//	alSkippedDueToException.Clear();
				//	alCopyError.Clear();
				//}
				bGood=true;
				Console.Error.WriteLine();//in case RunScriptLine failed after a Write
				if (File.Exists(sFileX)) { 
					Console.Error.WriteLine("Reading \"" + sFileX + "\"..."  +  ( (iCouldNotFinish>0) ? (iCouldNotFinish.ToString()+" lines FAILED!") : ("OK") )  );
				}
			}
			catch (Exception exn) {
				Console.Error.WriteLine();
				if (File.Exists(sFileX)) {
					Console.Error.WriteLine("Reading \"" + sFileX + "\"..." + ((iCouldNotFinish > 0) ? (iCouldNotFinish.ToString() + " lines FAILED!") : ("FAILED!")));
				}
				Common.sParticiple = "running " + Common.SafeString(sFileX,true) + ":";
				if (bTestOnly) MessageBox.Show("Error "+Common.sParticiple+"\n"+exn.ToString(),"Backup GoNow");
				Common.ShowExn(exn,Common.sParticiple,"RunScript");
				bGood=false;
			}
			try {
				if (streamIn!=null) streamIn.Close();
			}
			catch {}
			if (iFilesProcessed_ThisScript==0) iFilesProcessed_ThisScript=iFilesProcessed-iFilesProcessedPrev;
			if (bRemoveTrivialMessagesAfterScript&&bOutputTrivial&&(iFilesProcessed_ThisScript+alSkippedDueToException.Count+alCopyError.Count>0)) {
				Output(sRemovingTrivialMessages_TheMetaTrivialMessage,true);
				RemoveTrivialMessages();
			}
			return bGood;
		}//end RunScript
		public static string sRemovingTrivialMessages_TheMetaTrivialMessage="Removing trivial messages...";
		public void RemoveTrivialMessages() {
			//Console.Error.Write("RemoveTrivialMessages");//debug only
			//Console.Error.Flush();//debug only
			int iLine=0;
			lbOutNow.BeginUpdate();
			try {
				Flush();
				//Console.Error.Write("...");//debug only
				//Console.Error.Flush();//debug only
				string sLineTemp="";
				while (this.lbOut.Items.Count>0 && iLine<lbOut.Items.Count) {
					sLineTemp=lbOut.Items[iLine].ToString();
					if (bUserCancelledLastRun) {
						break;
					}
					if (sLineTemp.Contains(sWasUpToDate)||sLineTemp==sRemovingTrivialMessages_TheMetaTrivialMessage) {
						//Console.Error.WriteLine();//debug only
						//Console.Error.Write("Removing["+iLine.ToString()+"]:\""+lbOut.Items[iLine].ToString()+"\"");//debug only
						//Console.Error.Flush();
						lbOut.Items.RemoveAt(iLine);
						//Console.Error.Write(".");
						//Console.Error.Flush();
						//lbOut.Refresh();
						//Console.Error.Write(".");
						//Console.Error.Flush();
						//Application.DoEvents();
						//Console.Error.WriteLine(".");
					}
					else {
						//Console.Error.Write("+");//debug only
						//Console.Error.Flush();//debug only
						iLine++;
					}
				}
				//Console.Error.WriteLine("RemoveTrivialMessages done.");//debug only
			}
			catch (Exception exn) {
				Common.ShowExn(exn,"simplifying output",String.Format("RemoveTrivialMessages(){{iLine:{0}}}:",iLine));
			}
			lbOutNow.EndUpdate();
		}
		//public static string WithoutEndDirSep(string sPathX) {
		//	if (sPathX.Length>1&&sPathX.EndsWith(Common.sDirSep)) {
		//		sPathX=sPathX.Substring(0,sPathX.Length-1);
		//	}
		//	return sPathX;
		//}//end WithoutEndDirSep
		
		public bool SetDestFolder(string FolderNow) {
			bool bGood=false;
			int DestFolderIndexNow=-1;
			Common.sParticiple="initializing";
			//locinfoarrPseudoRoot[DestFolderIndexNow]
			LocInfo locinfoPseudoRootNow=null;
			try {
				if (FolderNow!=null&&FolderNow!="") {
					Common.sParticiple="removing trailing '"+Common.sDirSep+"'";
					FolderNow=Common.LocalFolderThenNoSlash(FolderNow);
					Common.sParticiple="checking driveinfoarrSelectableDrive";
					DestFolderIndexNow=Common.InternalIndexOfPseudoRootWhereFolderStartsWithItsRoot(FolderNow,false);
					locinfoPseudoRootNow=(DestFolderIndexNow>=0)?Common.GetPseudoRoot(DestFolderIndexNow):null;
					if (DestFolderIndexNow<0) {
						Common.AddFolderToPseudoRoots(FolderNow);
						Output("Adding location \""+FolderNow+"\"");
						DestFolderIndexNow=Common.InternalIndexOfPseudoRootWhereFolderStartsWithItsRoot(FolderNow,false);
						Output("  at index "+DestFolderIndexNow);
					}
					else {
						Output("Found location \""+FolderNow+"\"");
						Output("  at index "+DestFolderIndexNow+" ("+locinfoPseudoRootNow.DriveRoot_FullNameThenSlash+")");
					}
					if (DestFolderIndexNow>-1) {
						bGood=true;
						DestFolder_FullName=locinfoPseudoRootNow.DriveRoot_FullNameThenSlash;
						if (DestFolder_FullName!=Common.sDirSep&&DestFolder_FullName.EndsWith(Common.sDirSep)) DestFolder_FullName=DestFolder_FullName.Substring(0,DestFolder_FullName.Length-Common.sDirSep.Length);
					}
					else {
						Console.Error.WriteLine("SetDestFolder: could not set folder to \""+FolderNow+"\"");
						Console.Error.WriteLine();
					}
				}
			}
			catch (Exception exn) {
				ulByteCountDestTotalSize=0;
				ulByteCountDestAvailableFreeSpace=0;
				Common.ShowExn(exn,Common.sParticiple+String.Format(" {{driveinfoarrSelectableDrive{0}; DestFolderIndexNow:{1}}}",Common.GetSelectableDriveArrayMsg_LengthColonCount_else_ColonNull(),DestFolderIndexNow ),"SetDestFolder");
			}
			try {
				if (DestFolderIndexNow>-1) {
					ulByteCountDestTotalSize=(long)locinfoPseudoRootNow.TotalSize;
					ulByteCountDestAvailableFreeSpace=(long)locinfoPseudoRootNow.AvailableFreeSpace; //TotalFreeSpace doesn't count user quotas
					//Console.WriteLine( "{0}MB free {1}MB total ({2}bytes/{3}bytes) on {4} ({5})",
					//		(ulByteCountDestTotalSize/1024/1024), (ulByteCountDestAvailableFreeSpace/1024/1024), ulByteCountDestTotalSize, ulByteCountDestAvailableFreeSpace, locinfoarrPseudoRoot[DestFolderIndexNow].VolumeLabel, locinfoarrPseudoRoot[DestFolderIndexNow].DriveFormat );
				}
				else {
					ulByteCountDestTotalSize=Int64.MaxValue;
					ulByteCountDestAvailableFreeSpace=Int64.MaxValue;
					Console.WriteLine( "{0}MB free {1}MB total ({2}bytes/{3}bytes)",//debug only
								(ulByteCountDestTotalSize/1024/1024), (ulByteCountDestAvailableFreeSpace/1024/1024), ulByteCountDestTotalSize, ulByteCountDestAvailableFreeSpace );
					
				}
			}
			catch (Exception exn) {
				Common.ShowExn(exn,"accessing locinfoarrPseudoRoot["+DestFolderIndexNow.ToString()+"]:");
				ulByteCountDestTotalSize=0;
				ulByteCountDestAvailableFreeSpace=0;
			}
			Console.WriteLine("SetDestFolder:"+(FolderNow!=null?"\""+FolderNow+"\"":"null")+"(DestFolderIndexNow:"+DestFolderIndexNow.ToString()+")");//debug only
			return bGood;
		}//end SetDestFolder
		/// <summary>
		/// Makes sure that:
		/// -sDestRootThenSlash ends with slash (i.e. sDestRootThenSlash=Common.LocalFolderThenSlash(sDestRootThenSlash) )
		/// -DestSubfolderRelNameThenSlash does NOT start with slash, and DOES end with slash
		/// </summary>
		/*
		public bool FixSlashes() {
			bool bGood=false;
			SetDestFolder(this.comboDest.Text);
			//sDestRootThenSlash=DestFolder_FullName;//GetDestDriveRoot();
			if (sDestRootThenSlash!="") {
				bGood=true;
				if (!sDestRootThenSlash.EndsWith(Common.sDirSep)) sDestRootThenSlash+=Common.sDirSep;
			}
			else bGood=false;
			if (DestSubfolderRelNameThenSlash!="") {
				while (DestSubfolderRelNameThenSlash.StartsWith(Common.sDirSep)) DestSubfolderRelNameThenSlash=(DestSubfolderRelNameThenSlash.Length>1)?DestSubfolderRelNameThenSlash.Substring(1):"";
				if (!DestSubfolderRelNameThenSlash.EndsWith(Common.sDirSep)) DestSubfolderRelNameThenSlash+=Common.sDirSep;
			}
			return bGood;
		}//end FixSlashes
		*/
		public string ReconstructedSourcePath(string sBackupPath) {
			string sReturn=sBackupPath;
			//sBackupPath is constructed using: comboDest.Items.Add(Common.LocalFolderThenSlash(locinfoarrPseudoRoot[iNow].FullName) + DestSubfolderRelNameThenSlash);
			string sDestPrefix=Common.LocalFolderThenSlash(DestFolder_FullName)+DestSubfolderRelNameThenSlash;
			if (sReturn.StartsWith(sDestPrefix)) {
				if (sReturn.Length>sDestPrefix.Length) {
					sReturn=sReturn.Substring(sDestPrefix.Length);
					if (sDestPrefix.StartsWith("/")) {
						sReturn="/"+sReturn;
					}
					else if (sDestPrefix.Length>1&&sDestPrefix[1]==':') {
						sReturn=sReturn.Substring(0,1)+":"+sReturn.Substring(1);
					}
				}
				else {
					sReturn="";
				}
			}
			else {
				sReturn="";
				Console.Error.WriteLine("Cannot reconstruct source path for \""+sBackupPath+"\" so it will not be marked for deletion even if it does not exist on the source.");
			}
			return sReturn;
		}//end ReconstructedSourcePath
		private static bool bShowReconstructedBackupPathError=true;
		public string ReconstructedBackupPath(string sSrcPath) {
			//Output("Reconstruction sSrcPath(as received): "+sSrcPath);//debug only
			//Output("Reconstruction DestFolder_FullName(as received): "+DestFolder_FullName);//debug only
			//NOTE: Common.LocalFolderThenSlash just makes sure it ends with a slash and uses Common.sDirSep
			string sReturn=Common.LocalFolderThenSlash(DestFolder_FullName)+DestSubfolderRelNameThenSlash;
			string sDestAppend=sSrcPath;
			int iStart=0;
			if (sDestAppend[iStart]=='/') {
				while (iStart<sDestAppend.Length&&sDestAppend[iStart]=='/') {
					iStart++;
				}
			}
			//else iStart=IndexOfAnyDirectorySeparatorChar(sDestAppend); //uncommenting this removes the "C" folder if using this program in windows to backup local files
			if (iStart>-1&&iStart<sDestAppend.Length) {
				sDestAppend=sDestAppend.Substring(iStart);
				//if (bTestOnly) Output("Reconstruction(before normalize): "+sDestAppend);
				sDestAppend=Common.ConvertDirectorySeparatorsToNormal(sDestAppend);
				//if (bTestOnly) Output("Reconstruction(before removedouble): "+sDestAppend);
				sDestAppend=Common.RemoveDoubleDirectorySeparators(sDestAppend);
				if (sDestAppend!=null&&sDestAppend!=""&&sDestAppend.StartsWith(Common.sDirSep)) {
					if (sDestAppend.Length>1) sDestAppend=sDestAppend.Substring(1);
					else sDestAppend="";
				}
			}
			else sDestAppend="";

			if (sDestAppend=="") {
				sReturn=Common.LocalFolderThenSlash(DestFolder_FullName);
				if (bShowReconstructedBackupPathError) {
					MessageBox.Show("The backup source cannot be parsed so these files will be placed in \""+sReturn+"\".");
					bShowReconstructedBackupPathError=false;
				}
			}
			else sReturn+=sDestAppend;
			if ( !sReturn.EndsWith(Common.sDirSep) )
				sReturn+=Common.sDirSep;
			return sReturn;
		}//end ReconstructedBackupPath
		/*
		public static ArrayList alAlreadyMD=new ArrayList();
		public bool AlreadyMkdir(string Folder_FullName) {
			bool bFound=false;
			foreach (string FolderNow_FullName in alAlreadyMD) {
				if (FolderNow_FullName==Folder_FullName) {
					bFound=true;
					break;
				}
			}
			return bFound;
		}
		*/
		public bool ReconstructPathOnBackup(string sSrcPath) {
			bool bAlreadyExisted=false;
			string BackupFolder_FullName=ReconstructedBackupPath(sSrcPath);
			bool bGood=true;
			try {
				bAlreadyExisted=Directory.Exists(BackupFolder_FullName);
			}
			catch {
			}
			if (!bAlreadyExisted) {
				string sGetExn="";
				ArrayList alFoldersNotPreviouslyExisting=new ArrayList();
				if (!Common.CreateFolderRecursively(out sGetExn, ref alFoldersNotPreviouslyExisting, BackupFolder_FullName)) bGood=false;
				foreach (string FolderNow_FullName in alFoldersNotPreviouslyExisting) {
					//if (!AlreadyMkdir(FolderNow_FullName)) {
					if (bUserCancelledLastRun) {
						bUserCancelledLastRun=true;
					}
					if (!Directory.Exists(FolderNow_FullName)) {
						WriteRetryLineIfCreatingRetryBatch(sMkdir+" \""+FolderNow_FullName+"\"");
					}
					//	alAlreadyMD.Add(FolderNow_FullName);
					//}
				}
				
				//DirectorySecurity dirsec=new DirectorySecurity(
				Directory.CreateDirectory(BackupFolder_FullName);
				if (!Directory.Exists(BackupFolder_FullName)) Output("FAILED TO CREATE \""+BackupFolder_FullName+"\"",true);
				else //if (bTestOnly)
				Output("Created \""+BackupFolder_FullName+"\" to contain backup folder \""+BackupFolder_FullName+"\" (source path \""+sSrcPath+"\")");
				if (!bGood&&sGetExn.ToLower().IndexOf("system.io.ioexception: disk full")>-1) bDiskFullLastRun=true;
			}
			else bGood=true;
			return bGood;
		}
		public void BackupFile(string SrcFile_FullName, bool bUseReconstructedPath) {
			BackupFile(SrcFile_FullName,bUseReconstructedPath,null);
		}
		public void BackupFile(string SrcFile_FullName, bool bUseReconstructedPath, FileInfo fiNow) {
			string sLastAttemptedCommand="";
			bStartedCopyingAnyFiles=true;
			decimal dDone=-1.0m;
			int iDone=0;//whole-number percentage
			string sDone="";
			string sDestFile="";
			if (ulByteCountFolderNow>0) {
				try {
					dDone=(decimal)ulByteCountFolderNowDone/(decimal)ulByteCountFolderNow;
					iDone=(int)(dDone*100m);
					sDone="("+iDone.ToString()+"%) ";
				}
				catch {}
			}
			try {
				if (fiNow==null) fiNow=new FileInfo(SrcFile_FullName);
				if (fiNow.Exists) {
					ulByteCountFolderNowDone+=(ulong)fiNow.Length;
					ulByteCountTotalProcessed+=(ulong)fiNow.Length;
					string BackupFolder_ThenSlash=bUseReconstructedPath?ReconstructedBackupPath(fiNow.Directory.FullName):Common.LocalFolderThenSlash(DestFolder_FullName);
					if (bUseReconstructedPath) {
						if (!Directory.Exists(ReconstructedBackupPath(fiNow.Directory.FullName))) ReconstructPathOnBackup(fiNow.Directory.FullName);
					}
					if (!BackupFolder_ThenSlash.EndsWith(Common.sDirSep)) BackupFolder_ThenSlash+=Common.sDirSep;
					sDestFile=BackupFolder_ThenSlash+fiNow.Name;
					FileInfo fiDest=new FileInfo(sDestFile);
					if (fiDest.Exists) {
						if (fiDest.LastWriteTime<fiNow.LastWriteTime||fiDest.Length!=fiNow.Length) {
							if (	fiDest.LastWriteTime==fiNow.LastWriteTime && fiDest.Length!=fiNow.Length )
								Output(sDone+"Resaving: \""+sDestFile+"\"");
							else if ( fiDest.LastWriteTime>fiNow.LastWriteTime ) {
								Console.Error.WriteLine(sDone+"Dest is newer: \""+sDestFile+"\"");
							}
							else
								Output(sDone+"Updating: \""+sDestFile+"\"");
							if (!bTestOnly) {
								if ( fiDest.LastWriteTime<fiNow.LastWriteTime
								    || (fiDest.LastWriteTime==fiNow.LastWriteTime&&fiDest.Length!=fiNow.Length) )
								lByteCountTotalActuallyAdded+=(long)fiNow.Length-(long)fiDest.Length;
								sLastAttemptedCommand=sCP+" \""+SrcFile_FullName+"\" \""+sDestFile+"\"";
								File.Copy(SrcFile_FullName,sDestFile,true);
								ulByteCountTotalActuallyCopied+=(ulong)fiNow.Length;
							}
						}
						else {
							//already newer or same timestamp so ignore
							if (bOutputTrivial) Output(sDone+sWasUpToDate+": \""+sDestFile+"\"",false);
							else {
								this.labelTrivialStatus.Text=sDone+sWasUpToDate+": \""+sDestFile+"\"  (limited messages for faster performance)";
								if ((Environment.TickCount-iTickLastRefresh>iTicksRefreshInterval)) {
									UpdateProgressBar();
									tbStatus.Invalidate();
									Application.DoEvents();
									Flush();
									iTickLastRefresh=Environment.TickCount;
								}
							}
						}
					}
					else {
						if (!bTestOnly) {
							lByteCountTotalActuallyAdded+=(long)fiNow.Length;
							sLastAttemptedCommand=sCP+" \""+SrcFile_FullName+"\" \""+sDestFile+"\"";
							File.Copy(SrcFile_FullName,sDestFile);
							ulByteCountTotalActuallyCopied+=(ulong)fiNow.Length;
						}
						Output(sDone+"Copied New: \""+sDestFile+"\"");
					}
				}
				else Output(sDone+"Could not find \""+SrcFile_FullName+"\"");
			}
			catch (Exception exn) {
				if (sLastAttemptedCommand!=null&&sLastAttemptedCommand!="") {
					WriteRetryLineIfCreatingRetryBatch(sLastAttemptedCommand);
				}
				if (exn.ToString().ToLower().IndexOf("system.io.ioexception: disk full")>-1
				   || exn.ToString().ToLower().IndexOf("system.io.ioexception: there is not enough space on the disk")>-1) {
					bDiskFullLastRun=true;
				}
				else if (exn.ToString().ToLower().IndexOf("too long")>-2) {
					alCopyError.Add("Filename is too long for "+sCopyErrorFileFullNameOpener+SrcFile_FullName+sCopyErrorFileFullNameCloser+ToOneLine(exn.ToString()));
				}
				else if (exn.ToString().ToLower().IndexOf("system.io.directorynotfoundexception")>-1) {
					alCopyError.Add("Recreate source folder failed"+sCopyErrorFileFullNameOpener+SrcFile_FullName+sCopyErrorFileFullNameCloser+ToOneLine(exn.ToString()));
				}
				else if (exn.ToString().ToLower().IndexOf("not enough space")>-2) {
					//NOTE: can be unable to copy file even if disk not technically full
					bDiskFullLastRun=true;//bFileTooBigToFitLastRun=true;
					alCopyError.Add("Not enough space for"+sCopyErrorFileFullNameOpener+SrcFile_FullName+sCopyErrorFileFullNameCloser+ToOneLine(exn.ToString()));
				}
				else {
					alCopyError.Add("Could not read"+sCopyErrorFileFullNameOpener+SrcFile_FullName+sCopyErrorFileFullNameCloser+ToOneLine(exn.ToString()));
					bCopyErrorLastRun=true;
				}
				string sMsg="";
				if (alCopyError.Count>0) sMsg=(string)alCopyError[alCopyError.Count-1];
				Common.ShowExn(exn,"backing up file ("+sMsg+")","BackupFile");
			}
			iFilesProcessed++;
		}//end BackupFile
		public static bool bShowOutputException=true;
		public static void Output(string sLineX) {
			Output(sLineX,false);
		}
		private static ArrayList alDisplayQueue=new ArrayList();
		public static void Flush() {
			try {
				if (alDisplayQueue!=null&&alDisplayQueue.Count>0) {
					bool bUpdateListBox=false;
					bool bUpdateTrivialMessageLabel=false;
					//bRefreshListBox=true; //commented for debug only
					if (bUpdateListBox) lbOutNow.BeginUpdate();
					string sLastMsg="";
					if (alDisplayQueue!=null&&alDisplayQueue.Count>0) {
						foreach (string sNow in alDisplayQueue) {
							sLastMsg=alDisplayQueue[alDisplayQueue.Count-1].ToString();
							mainformNow.labelTrivialStatus.Text=sLastMsg;
							bUpdateTrivialMessageLabel=true;
							lbOutNow.Items.Add(sNow);
						}
					}
					if (bUpdateListBox) lbOutNow.EndUpdate();
					if (bAutoScroll && MainForm.lbOutNow.Items.Count>0) MainForm.lbOutNow.SelectedIndex=MainForm.lbOutNow.Items.Count-1;
					//lbOutNow.Refresh();
					//mainformNow.Refresh();
					lbOutNow.Invalidate();
					if (bUpdateListBox||bUpdateTrivialMessageLabel) Application.DoEvents();
					alDisplayQueue.Clear();
					iTickLastRefresh=Environment.TickCount;
				}
			}
			catch (Exception exn) {
				Common.ShowExn(exn,"","MainForm.Flush");
			}
		}
		public static bool bShowNoDiskSpaceAtStartWarning=true;
		public static void Output(string sLineX, bool bForceRefresh) {
			try {
				Console.WriteLine(sLineX);
				alDisplayQueue.Add(sLineX);
				if ( bForceRefresh || (Environment.TickCount-iTickLastRefresh>iTicksRefreshInterval) ) {
					//decimal dDone=-1.0m;
					//if (ulByteCountTotal>0) dDone=(decimal)ulByteCountTotalProcessed/(decimal)ulByteCountTotal;
					if (bStartedCopyingAnyFiles) {
						UpdateProgressBar();
					}//end if bStartedCopyingAnyFiles
					Flush();
				}
			}
			catch (Exception exn) {
				if (bShowOutputException) {
					Common.ShowExn(exn,"","Output("+((sLineX!=null)?("non-null"):("null"))+","+(bForceRefresh?"true":"false")+")");
					MessageBox.Show("Exception in Output: \n\n"+exn.ToString());
					bShowOutputException=false;
				}
			}
		}
		
		public static void UpdateProgressBar() {
			string sPercentFree="";
			try {
				decimal mFree=0;
				long lFree=(long)ulByteCountDestAvailableFreeSpace-lByteCountTotalActuallyAdded;
				try {
					if (ulByteCountDestTotalSize>0) {
						mFree=(decimal)((decimal)ulByteCountDestAvailableFreeSpace-(decimal)lByteCountTotalActuallyAdded) / (decimal)ulByteCountDestTotalSize;
					}
					else if (bShowNoDiskSpaceAtStartWarning) {
						Console.Error.WriteLine("ERROR: No disk space on destination while copying first file");
						bShowNoDiskSpaceAtStartWarning=false;
					}
				}
				catch (Exception exn) {
					mFree=0.0m;
				}
				//mFree*=100.0m;
				if (mFree>=0) sPercentFree=String.Format("{0:P}",mFree);
				if (ulByteCountDestAvailableFreeSpace==Int64.MaxValue) sPercentFree="?%:";
				if (ulByteCountTotalProcessed_LastRun>0) {
					if (ulByteCountTotalProcessed>ulByteCountTotalProcessed_LastRun) {
						mainformNow.progressbarMain.Value=mainformNow.progressbarMain.Maximum;
					}
					else {
						mainformNow.progressbarMain.Maximum=(int)(ulByteCountTotalProcessed_LastRun/1024/1024);
						mainformNow.progressbarMain.Value=(int)(ulByteCountTotalProcessed/1024/1024);
					}
				}
				else {
					//if (ulByteCountTotalProcessed>=ulByteCountTotalProcessed_LastRun) {
						//should already be ProgressBarStyle.Continuous until before condition becomes true
						
						if (ulByteCountDestAvailableFreeSpace!=Int64.MaxValue) {
							mainformNow.progressbarMain.Style=ProgressBarStyle.Continuous;
							mainformNow.progressbarMain.Value=(int)( (decimal)mainformNow.progressbarMain.Maximum*(1.0m-mFree) );
						}
						//else {
							
						//}
					//}
				}
				//if (bRealTime) {
					//int iDot=sPercentFree.IndexOf(".");
					//if (iDot>-1) {
					//	if (iDot
					//}
					ulong ulMBProcessed=ulByteCountTotalProcessed/1024/1024;
					ulong ulKBProcessed=ulByteCountTotalProcessed/1024;
					string Processed_Size=ulMBProcessed.ToString()+"MB";
					if (ulMBProcessed==0) Processed_Size=ulKBProcessed.ToString()+"KB";
					if (ulKBProcessed==0) Processed_Size=ulByteCountTotalProcessed+"bytes";

					long ulMBAdded=lByteCountTotalActuallyAdded/1024/1024;
					long ulKBAdded=lByteCountTotalActuallyAdded/1024;
					string Added_Size=ulMBAdded.ToString()+"MB";
					if (ulMBAdded==0) Added_Size=ulKBAdded.ToString()+"KB";
					if (ulKBAdded==0) Added_Size=lByteCountTotalActuallyAdded+"bytes";
					
					
					tbStatusNow.Text=String.Format( "{0} ({1} files) processed, {2} added      {3} " + ((ulByteCountDestAvailableFreeSpace==Int64.MaxValue)?"disk space unknown (not implemented in this version of your computer's framework)({4}/{5}MB)":"space remaining ({4}/{5}MB)"),
						Processed_Size, iFilesProcessed, Added_Size, sPercentFree, (lFree/1024/1024), ulByteCountDestTotalSize/1024/1024 )
						+(bAutoScroll?"":"...");
				//}
				//else tbStatusNow.Text=String.Format( "{0}MB / {1}MB counted so far",
				//		((ulByteCountTotalProcessed)/1024/1024),(ulByteCountTotal/1024/1024) )
				//		+  (bAutoScroll?"":"...");
		
			}
			catch (Exception exn) {
				Common.ShowExn(exn,"updating progress bar");
			}
		}//end UpdateProgressBar
		
		void ComboDestSelectedIndexChanged(object sender, EventArgs e)
		{
			
		}
		void LbOutMouseEnter(object sender, EventArgs e) {
			//bAutoScroll=false;
		}
		void LbOutMouseLeave(object sender, EventArgs e) {
			//bAutoScroll=true;
		}
		void LbOutMouseUp(object sender, MouseEventArgs e) {
			bAutoScroll=true;
		}
		void LbOutMouseDown(object sender, MouseEventArgs e) {
			bAutoScroll=false;
		}
		void DeleteFolderRecursively(DirectoryInfo diBase, bool bDecrementBytesAdded) {
			string Directory_FullName="";
			try {
				if ( (diBase.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly ) {
					try {
						//faPrev=fiNow.Attributes;
						diBase.Attributes=FileAttributes.Normal;
						//bChangeAttrib=true;
					}
					catch {}
				}
				Directory_FullName=diBase.FullName;
				string FileNow_FullName="";
				long FileNow_Length=0;
				foreach (FileInfo fiNow in diBase.GetFiles()) {
					if (bUserCancelledLastRun) break;
					try {
						FileNow_FullName=fiNow.FullName;
						FileNow_Length=fiNow.Length;
						fiNow.Attributes=FileAttributes.Normal;//fiNow.Attributes^= FileAttributes.ReadOnly;
						//try {
						//	FileSecurity fisec=new FileSecurity();
						//	IdentityReference idref=new SecurityIdentifier(WellKnownSidType.SelfSid);
							
						//	fisec.SetOwner(idref);
						//}
						try {
							if ( (fiNow.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly ) 
								fiNow.Attributes=FileAttributes.Normal;
						}
						catch (Exception exn) {
							Output("CouldntRemoveReadonlyStatus:"+fiNow.FullName);
						}
						fiNow.Delete();
						if (bDecrementBytesAdded) MainForm.lByteCountTotalActuallyAdded-=FileNow_Length;
					}
					catch (Exception exn) {
						Common.ShowExn(exn,"deleting file "+Common.SafeString(FileNow_FullName,true));
					}
				}
				foreach (DirectoryInfo diSub in diBase.GetDirectories()) {
					if (bUserCancelledLastRun) break;
					DeleteFolderRecursively(diSub,bDecrementBytesAdded);
				}
				diBase.Attributes=FileAttributes.Normal;
				diBase.Delete();
			}
			catch (Exception exn) {
				Common.ShowExn(exn,"deleting folder "+Common.SafeString(Directory_FullName,true));
			}
		}//end DeleteFolderRecursively
		/*
		void DeleteFolderRecursively(string Directory_FullName) {
			lByteCountTotalActuallyAdded-=
		}
		
		void DeleteIfNotOnSource_Recursively(string FolderNoSlash_FullName) {
			if ((FolderNoSlash_FullName+Common.sDirSep)!=(sDestPrefix) {
				if () {
				}//end if delete whole folder
				else {
					foreach (DirectoryInfo diNow in diBranch.GetDirectories()) {
					}
				}
			}
			else {
				MessageBox.Show("Ignored deletion of destination base folder \""+diBase.FullName+"\"");//debug only
				foreach (DirectoryInfo diBranch in Di
			}
		}//end DeleteIfNotOnSource_Recursively
		*/
		
		void MainFormLoad(object sender, EventArgs e) {
			Common.mcbNow=new MyCallBack();
			menuitemCancel.Enabled=false;
			this.Text=sMyNameAndVersion;
			this.tbStatus.Text = "Welcome to "+sAppName+"";
			DateTime datetimeNow = DateTime.Now;
			string sMyProcess = Assembly.GetExecutingAssembly().Location;
			sMyProcess = sMyProcess.Substring(sMyProcess.LastIndexOf('\\') + 1);
			//errStream=new StreamWriter(sFileErrLog);
			//Console.SetError(errStream);
			Console.Error.Write("{0}", sMyProcess);
			Console.Error.WriteLine(": started at {0}.", datetimeNow);
			Console.Error.WriteLine();
			if (bTestOnly) Common.iDebugLevel=Common.DebugLevel_On;
			//FolderLister.bDebug=bTestOnly;
			//bDebug=bTestOnly;
			lbOutNow=this.lbOut;
			Console.Error.WriteLine("About to load " + Common.SafeString(StartupFile_Name,true));
			DateTime dtNow=DateTime.Now;
			lbOutNow.Items.Add(dtNow.Year+"-"+dtNow.Month+"-"+dtNow.Day+" "+dtNow.Hour+":"+dtNow.Minute);
			RunScript(StartupFile_Name);
			this.lblProfile.Visible=true;
			Console.Error.WriteLine("Finished " + Common.SafeString(StartupFile_Name,true)+" in MainFormLoad");
			bool bFoundLoadProfile=false;
			bool bSuccessFullyResetStartup=false;
			string sMsg="Attempting to get path of current user's Documents...";
			tbStatus.Text=sMsg;
			OutputFile_FullName=Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)+Common.sDirSep+OutputFile_Name;
			sMsg="Welcome to "+sMyName+"!";
			tbStatus.Text=sMsg;
			if (!bLoadedProfile) {
				Console.Error.WriteLine(Common.SafeString(StartupFile_Name,true)+" did not load a profile so loading default (\""+DefaultProfile_Name+"\")");
				bool bTest=RunScriptLine("LoadProfile:"+DefaultProfile_Name);
				Console.Error.WriteLine("Loaded Profile \""+DefaultProfile_Name+"\"..."+(bTest?"OK":"FAILED!"));
				string sAllData="";
				try {
					sMsg="Attempting to edit "+Common.SafeString(StartupFile_Name,true)+" and add \"LoadProFile:"+DefaultProfile_Name+"\" if no valid LoadProfile statement is found...";
					tbStatus.Text=sMsg;
					Application.DoEvents();
					Console.Error.Write(sMsg);
					Console.Error.Flush();
					Console.Error.Write("reading data...");
					Console.Error.Flush();
					try {
						if (File.Exists(StartupFile_Name)) {
							StreamReader streamIn=new StreamReader(StartupFile_Name);
							string sLine;
							while ( (sLine=streamIn.ReadLine()) != null ) {
								while (sLine.EndsWith("\n")||sLine.EndsWith("\r")) {
									if (sLine=="\n"||sLine=="\r") {sLine=""; break;}
									else sLine=sLine.Substring(0,sLine.Length-1);
								}
								while (sLine.StartsWith("\n")||sLine.StartsWith("\r")) {
									if (sLine=="\n"||sLine=="\r") {sLine=""; break;}
									else sLine=sLine.Substring(1);
								}
								while (sLine.StartsWith(" ")) {
									if (sLine==" ") {sLine=""; break;}
									else sLine=sLine.Substring(1);
								}
								sAllData+=sLine+Environment.NewLine;
								if (sLine.ToLower().StartsWith("loadprofile:")&&sLine.ToLower()!="loadprofile") bFoundLoadProfile=true;
							}
							streamIn.Close();
						}
					}
					catch (Exception exn) {
						Common.ShowExn(exn,"reading "+Common.SafeString(StartupFile_Name,true));
					}
					Console.Error.Write("writing data...");
					Console.Error.Flush();
					//System.Threading.Thread.Sleep(100);//wait for file to be ready (is this ever needed???)
					StreamWriter streamOut=new StreamWriter(StartupFile_Name);
					streamOut.WriteLine(sAllData);
					streamOut.WriteLine("LoadProfile:"+DefaultProfile_Name);
					streamOut.Close();
					Console.Error.WriteLine("OK.");
					bSuccessFullyResetStartup=true;
				}
				catch (Exception exn) {
					Common.ShowExn(exn,"creating "+Common.SafeString(StartupFile_Name,true),"MainFormLoad");
				}
				//System.Threading.Thread.Sleep(500);
				tbStatus.Text="Done checking startup {bLoadedProfile:"+(bLoadedProfile?"yes":"no")+"; bFoundLoadProfile:"+(bFoundLoadProfile?"yes":"no")+"; bSuccessFullyResetStartup:"+(bSuccessFullyResetStartup?"yes":"no")+"}.";
			}//end if !bLoadedProfile
			alPseudoRootsNow=Common.PseudoRoots_DriveRootFullNameThenSlash_ToArrayList();
			alSelectableDrives=Common.SelectableDrives_DriveRootFullNameThenSlash_ToArrayList();
			if (alPseudoRootsNow!=null && alPseudoRootsNow.Count>0) {
				if (bExitIfNoUsableDrivesFound&&(alSelectableDrives==null||alSelectableDrives.Count==0) && !bAlwaysStayOpen)
					Application.Exit();
			}
			else if (!bTestOnly&&!bAlwaysStayOpen)
				Application.Exit();
			
			CalculateMargins();
			UpdateSize();
		}//end MainFormLoad
		void CalculateMargins() {
			iLBRightMargin=lbOut.Left;//this.Width-(lbOut.Left+lbOut.Width);
			iLBBottomMargin=this.Height-(lbOut.Top+lbOut.Height);
		}
		
		void UpdateSize() {
			lbOut.Width=this.ClientSize.Width-lbOut.Left*2;//(iLBRightMargin+lbOut.Left);
			lbOut.Height=this.ClientSize.Height-lbOut.Top-tbStatus.Height-lbOut.Left;//(iLBBottomMargin+lbOut.Top);
			this.progressbarMain.Left=this.ClientRectangle.Left;
			this.progressbarMain.Width=this.ClientRectangle.Width;
			this.progressbarMain.Top=this.tbStatus.Top-this.progressbarMain.Height;
			labelTrivialStatus.Width=this.ClientSize.Width-labelTrivialStatus.Left;
			//this.lblDest.Width=this.ClientSize.Width;
		}

		void ComboDestTextChanged(object sender, EventArgs e) {
			//bool bFound=false;
			//foreach (string sNow in comboDest.Items) {
			//	if (comboDest.Text==sNow) bFound=true;
			//}
			//if (!bFound) comboDest.SelectedIndex=0;
			int FolderIndexNow=Common.InternalIndexOfPseudoRootWhereFolderStartsWithItsRoot(comboDest.Text,false);
			LocInfo locinfoNow=Common.GetPseudoRoot(FolderIndexNow);
			bool bGB=locinfoNow.AvailableFreeSpace/1024/1024/1024 > 0;
			if (FolderIndexNow>=0) {
				this.lblDestInfo.Text=locinfoNow.VolumeLabel+" ("
					+ ((locinfoNow.AvailableFreeSpace!=Int64.MaxValue)  ?  ( bGB ? (((decimal)locinfoNow.AvailableFreeSpace/1024m/1024m/1024m).ToString("#")+"GB free"):(((decimal)locinfoNow.AvailableFreeSpace/1024m/1024m).ToString("0.###")+"MB free") )  :  "unknown free"  );
			}
			else this.lblDestInfo.Text="";
		}//end ComboDestTextChanged
		
		void MainFormResize(object sender, EventArgs e) {
			UpdateSize();
		}
		
		void MainFormFormClosed(object sender, FormClosedEventArgs e) {
		}
		
//lines in MainForm.Designer.cs
/*		private void LbOutDrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e) {
			e.DrawBackground();
			
			//TODO: find out why no gray lines are being drawn (supposed to use brushItemWasUpToDate)
			//if (((ListBox)sender).Items[e.Index].Y>0) {
				if (((ListBox)sender).Items[e.Index].ToString().IndexOf(sWasUpToDate)>-1) 
					e.Graphics.DrawString( ((ListBox)sender).Items[e.Index].ToString(), e.Font, brushItemWasUpToDate,e.Bounds,StringFormat.GenericDefault);
				else {
					e.Graphics.DrawString( ((ListBox)sender).Items[e.Index].ToString(), e.Font, brushItemOther,e.Bounds,StringFormat.GenericDefault);
				}
				e.DrawFocusRectangle();//only draws if focused
			//}
		}//end paint item override*/
		public static string ToOneLine(string sNow) {
			sNow=sNow.Replace("\n"," ");
			sNow=sNow.Replace("\r"," ");
			while (sNow.Contains("  ")) sNow=sNow.Replace("  "," ");
			return sNow;
		}
		public static string ToOneLine(Exception exn) {
			return ToOneLine(exn.ToString());
		}
		public bool RunScriptLine(string sLine) {
			bool bForceBad=false;
			bool bGood=false;
			try {
				Common.RemoveEndsWhiteSpaceByRef(ref sLine);
				Common.sParticiple="showing line";
				if (sLine==null||!sLine.StartsWith("#")) this.lbOut.Items.Add("   RunScriptLine("+Common.SafeString(sLine,true)+")");
				Common.sParticiple="parsing line";
				int iMarker=sLine.IndexOf(":");
				if (iMarker>0 && sLine.Length>(iMarker+1)) {
					string sCommandLower=sLine.Substring(0,iMarker).ToLower();
					string sValue=sLine.Substring(iMarker+1);
					string sValueOrig=sValue;
					try {
						DirectoryInfo diMyDocs=new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
						DirectoryInfo diUserProfile=null;
						if (diMyDocs!=null&&diMyDocs.Exists) diUserProfile=diMyDocs.Parent;
						if (diUserProfile!=null&&diUserProfile.Exists) {
							sValue=sValue.Replace("%USERPROFILE%",diUserProfile.FullName);
						}
						sValue=sValue.Replace("%MYDOCS%",Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)); //same as Personal
						sValue=sValue.Replace("%DESKTOP%",Environment.GetFolderPath(Environment.SpecialFolder.Desktop)); //The logical Desktop rather than the physical file system location DesktopDirectory
						sValue=sValue.Replace("%APPDATA%",Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
						sValue=sValue.Replace("%LOCALAPPDATA%",Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
					}
					catch (Exception exn) {
						Common.ShowExn(exn,"parsing environment variables in command parameter","RunScriptLine");
						sValue=sValueOrig;
					}
					/*
	Application Data 	Per-user application-specific files 	%USERPROFILE%\Application Data 	Win98
	Cookies 	Internet Explorer browser cookies 	%USERPROFILE%\Cookies 	Win98
	Desktop Directory 	Files stored on the user's desktop 	%USERPROFILE%\Desktop 	Win95
	Favorites 	User's Favorites 	%USERPROFILE%\Favorites 	Win9898
	Fonts 	Container folder for installed fonts 	%windir%\Fonts 	Win98XP
	History 	User-specific browser history 	%USERPROFILE%\Local Settings\History 	Win98
	Internet Cache 	User-specific Temporary Internet Files 	%USERPROFILE%\Local Settings\Temporary Internet Files 	Win98
	LocalApplicationData 	User-specific and computer-specific application settings 	%USERPROFILE%\Local Settings\Application Data 	Win2000/ME
	My Documents 	%USERPROFILE%\My Documents (WinNT line) [User's documents] C:\My Documents (Win98-ME) 	Win98 []
	My Music 	User's music 	%USERPROFILE%\My Documents\My Music 	WinXP []
	My Pictures 	User's pictures 	%USERPROFILE%\My Documents\My Pictures 	WinXP []
	My Videos 	User's video files 	%USERPROFILE%\My Documents\My Videos 	WinXP []
	Programs 	User-specific "(All) Programs" groups and icons 	%USERPROFILE%\Start Menu\Programs 	Win95 []
	Recent 	User-specific "My Recent Documents" 	%USERPROFILE%\Recent 	Win98 []
	Send To 	User-specific "Send To" menu items 	%USERPROFILE%\SendTo 	Win98 []
	Start Menu 	User-specific "Start Menu" items 	%USERPROFILE%\Start Menu 	Win98 []
	System 	The Windows system directory 	%windir%\system32 	Win2000 []
	Saved Games 	User's Saved Games 	%USERPROFILE%\saved games 	WinVista []
	Templates 	User-specific document templates 	%USERPROFILE%\Templates 	Win98 []
	
	Desktop                C:\Users\{username}\Desktop
	Programs               C:\Users\{username}\AppData\Roaming\Microsoft\Windows\Start Menu\Programs
	Personal               C:\Users\{username}\Documents
	Personal               C:\Users\{username}\Documents
	Favorites              C:\Users\{username}\NetHood\Favorites
	Startup                C:\Users\{username}\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup
	Recent                 C:\Users\{username}\AppData\Roaming\Microsoft\Windows\Recent
	SendTo                 C:\Users\{username}\AppData\Roaming\Microsoft\Windows\SendTo
	StartMenu              C:\Users\{username}\AppData\Roaming\Microsoft\Windows\Start Menu
	MyMusic                C:\Users\{username}\Music
	DesktopDirectory       C:\Users\{username}\Desktop
	MyComputer
	Templates              C:\Users\{username}\AppData\Roaming\Microsoft\Windows\Templates
	ApplicationData        C:\Users\{username}\AppData\Roaming
	LocalApplicationData   C:\Users\{username}\AppData\Local
	InternetCache          C:\Users\{username}\AppData\Local\Microsoft\Windows\Temporary Internet Files
	Cookies                C:\Users\{username}\AppData\Roaming\Microsoft\Windows\Cookies
	History                C:\Users\{username}\AppData\Local\Microsoft\Windows\History
	CommonApplicationData  C:\ProgramData
	System                 C:\Windows\system32
	ProgramFiles           C:\Program Files (x86)
	MyPictures             C:\Users\{username}\Pictures
	CommonProgramFiles     C:\Program Files (x86)\Common Files
	
	%ALLUSERSPROFILE%  	C:\Documents and Settings\All Users  OR  C:\Users\{username}
	%APPDATA% 	C:\Documents and Settings\{username}\Application Data  OR  C:\Users\{username}\AppData\Roaming
	%HOMEPATH% 	\Documents and Settings\{username}  OR  \Users\{username}
	%USERPROFILE% 	C:\Documents and Settings\{username}  OR  C:\Users\{username}
	
	%PROGRAMFILES% 	Directory containing program files, usually C:\Program Files
	%WINDIR% 	C:\Windows
	%SYSTEMROOT% 	The Windows XP root directory, usually C:\Windows
	%TEMP% and %TMP% 	C:\DOCUME~1\{username}\LOCALS~1\Temp
	%USERNAME% 	{username}
	%SYSTEMDRIVE% 	The drive containing the Windows XP root directory, usually C:
	%PROMPT% 	Code for current command prompt format. Code is usually $P$G
	%HOMEDRIVE% 	C:
	%COMPUTERNAME% 	{computername}
	%COMSPEC% 	C:\Windows\System32\cmd.exe
	%PATH% 	C:\Windows\System32\;C:\Windows\;C:\Windows\System32\Wbem
	%PATHEXT% 	.COM; .EXE; .BAT; .CMD; .VBS; .VBE; .JS ; .WSF; .WSH
					 */
					//%USERPROFILE%
					if (sCommandLower.StartsWith("#")) {
						//ignore
					}
					else if (sCommandLower=="excludedest") {
						Common.AddDriveToInvalidDrives(sValue);
						if (bTestOnly) Output("Not using "+sValue+" for backup");
					}
					else if (sCommandLower=="includedest") {
						Common.AddPathToExtraPseudoRootsToManuallyAdd(sValue);
					}
					else if (sCommandLower=="addmask") {
						Common.alMasks.Add(sValue);
						string sTemp="";
						foreach (string sMask in Common.alMasks) {
							sTemp+=(sTemp==""?"":", ")+sMask;
						}
						if (bTestOnly) Output("#Masks changed: "+sTemp);
					}
					else if (sCommandLower=="removemask") {
						if (sValue=="*") Common.alMasks.Clear();
						else Common.alMasks.Remove(sValue);
						string sTemp="";
						foreach (string sMask in Common.alMasks) {
							sTemp+=(sTemp==""?"":", ")+sMask;
						}
						if (bTestOnly) Output("#Masks changed: "+sTemp);
					}
					else if (sCommandLower=="exclude") {
						Common.alExclusions.Add(sValue);
						string sTemp="";
						foreach (string sExclusion in Common.alExclusions) {
							sTemp+=(sTemp==""?"":", ")+sExclusion;
						}
						if (bTestOnly) Output("#Exclusions changed: "+sTemp);
					}
					else if (sCommandLower=="include") {
						if (sValue=="*") Common.alExclusions.Clear();
						else Common.alExclusions.Remove(sValue);
						string sTemp="";
						foreach (string sExclusion in Common.alExclusions) {
							sTemp+=(sTemp==""?"":", ")+sExclusion;
						}
						if (bTestOnly) Output("#Exclusions changed: "+sTemp);
					}
					else if (sCommandLower=="addfile") {
						if (sValue!=null&&sValue!="") {
							ArrayList alFiles=new ArrayList();
							int iWild=sValue.IndexOf(Common.SlashWildSlash);
							DirectoryInfo diBranch=null;
							if ((iWild>-1)&&(sValue.Length>iWild+3)) {
								Output("Listing folders for wildcard \""+sValue+"\":");
								diBranch=new DirectoryInfo(sValue.Substring(0,iWild+1));//+1 in case /*/ so that / will be used
								foreach (DirectoryInfo diNow in diBranch.GetDirectories()) {
									alFiles.Add( sValue.Substring(0,iWild+1)+diNow.Name+sValue.Substring(iWild+2) );
									Output("  "+sValue.Substring(0,iWild+1)+diNow.Name+sValue.Substring(iWild+2));
								}
								if (alFiles.Count<1) Output("  No matching folders.");
							}
							else alFiles.Add(sValue);
							foreach (string sFileTheoretical in alFiles) {
								try {ulByteCountTotal+=(ulong)(new FileInfo(sFileTheoretical)).Length;}
								catch {}
								FileInfo fiSrc=new FileInfo(sFileTheoretical);
								if (fiSrc.Exists) {
									ulByteCountTotal+=(ulong)fiSrc.Length;
									//if (fiX.Exists())
									ReconstructPathOnBackup(fiSrc.DirectoryName);
									alFilesBackedUpManually.Add(ReconstructedBackupPath(fiSrc.DirectoryName)+Common.sDirSep+fiSrc.Name);
									if (!Common.IsExcludedFile(fiSrc.Directory,fiSrc)) BackupFile(sFileTheoretical,true);
								}
								else {
									bCopyErrorLastRun=true;
									alCopyError.Add("File specified in configuration does not exist"+sCopyErrorFileFullNameOpener+sFileTheoretical+sCopyErrorFileFullNameCloser);
								}
							}//end foreach file (single file unless path has wildcard)
						}//end if sValue is not blank
					}//end if sCommandLower=="addfile"
					else if (sCommandLower=="addfolder") {
						int iSlashWildSlash=sValue.IndexOf(Common.SlashWildSlash);
						if (iSlashWildSlash>-1) {
							ArrayList alFoldersTheoretical=new ArrayList();
							string BaseFolder_FullName=sValue.Substring(0,iSlashWildSlash);
							DirectoryInfo diBase=new DirectoryInfo(BaseFolder_FullName);
							string SpecifiedFolder_Name=sValue.Substring(iSlashWildSlash+Common.SlashWildSlash.Length);
							if (diBase.Exists) {
								foreach (DirectoryInfo diNow in diBase.GetDirectories()) {
									string FolderTheoretical_FullName=diBase.FullName+Common.sDirSep+diNow.Name+Common.sDirSep+SpecifiedFolder_Name;
									if (FolderTheoretical_FullName.Contains(Common.SlashWildSlash) //allow Common.SlashWildSlash to allow recursive usage of Common.SlashWildSlash
									    ||Directory.Exists(FolderTheoretical_FullName)) {
										alFoldersTheoretical.Add(FolderTheoretical_FullName);
									}
								}
								this.lbOut.Items.Add("Adding ("+alFoldersTheoretical.Count.ToString()+") folder(s) via wildcard "+Common.SafeString(sValue,true)+"...");
								Application.DoEvents();
								int iNonExcludable=0;
								int iWildcardsAdded=0;
								foreach (string sFolderTheoretical in alFoldersTheoretical) {
									if (!sFolderTheoretical.Contains(Common.SlashWildSlash)) {
										if (!Common.IsExcludedFolder(new DirectoryInfo(sFolderTheoretical))) {
											RunScriptLine("AddFolder:"+sFolderTheoretical);
											iNonExcludable++;
										}
									}
									else {
										RunScriptLine("AddFolder:"+sFolderTheoretical);
										iWildcardsAdded++;
									}
								}
								if (iNonExcludable>0) this.lbOut.Items.Add("Done adding ("+iNonExcludable.ToString()+") folder(s) via wildcard and "+iWildcardsAdded.ToString()+" recursive wildcard folders.");
							}
							else {
								this.lbOut.Items.Add("ERROR: Folder does not exist ("+Common.SafeString(BaseFolder_FullName,true)+")--cannot add specified subfolder(s) via wildcard.");
							}
						}
						else {//alFoldersTheoretical.Add(sValue);
						//foreach (string sFolderTheoretical in alFoldersTheoretical) {
							string sSearchRoot=sValue;
							//string Common.sDirSep=char.ToString(Path.DirectorySeparatorChar);
							Output("Loading \""+sSearchRoot+"\""+(Common.MaskCount>0?(" (only "+Common.MasksToCSV()+")"):"")+"...");
							//string sTempFile=Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)+"FolderList.tmp";
							//FolderLister.SetOutputFile(sTempFile);
							
							//if (bRealTime) {
							menuitemCancel.Enabled=true;
							//Common.sParticiple="getting directory info";
							DirectoryInfo diRoot=new DirectoryInfo(sSearchRoot);
							iDepth=-1;
							//bBusyCopying=true;
							if (diRoot.Exists) BackupFolder(diRoot);
							//else {
							//	Output("Folder cannot be read: "+diRoot.FullName);
							//}
							//}
							//bBusyCopying=false;
							menuitemCancel.Enabled=false;
							//else {
							//	menuitemCancel.Enabled=true;
								
							//	string[] sarrListed=flisterNow.GetLines();
							//	ulByteCountFolderNow=flisterNow.ByteCount;
							//	ulByteCountTotal+=ulByteCountFolderNow;
							//	ulByteCountFolderNowDone=0;
							//	if (bTestOnly) Output("Getting ready to copy "+(ulByteCountFolderNow/1024/1024).ToString()+"MB...");
							//	//iListedLines=0;
							//	//if (sarrListed!=null&&sarrListed.Length>0) {
							//		//if (File.Exists(sTempFile)) {
							//		//	StreamReader streamTemp=new StreamReader(sTempFile);
							//		//	string sListedItem;
							//		//	while ( (sListedItem=streamTemp.ReadLine()) != null ) {
							//		sLastFileUsed=diRoot.FullName;
							//		bContinue=true;
							//		BackupTree(diRoot);
							//		bContinue=false;
							//		//	}
							//		//	streamTemp.Close();
							//		//	File.Delete(sTempFile);
							//		//	Thread.Sleep(500);
							//	//}
							//	//else Output("Could not find any files in the added folder.");
							//	if (iFileCount<=0) Output("Could not find any files in the added folder.");
							//	//if (CurrentFolder_alSkippedDueToException!=null&&CurrentFolder_alSkippedDueToException.Count>0) {
							//	//	foreach (string sSkippedNow in CurrentFolder_alSkippedDueToException) {
							//	//		alSkippedDueToException.Add(sSkippedNow);
							//	//	}
							//	//	CurrentFolder_alSkippedDueToException.Clear();
							//	//}
							//}//else !bRealTime
						}
					}//end if sCommandLower==addfolder
					else if (sCommandLower=="loadprofile") {
						Common.iDebugLevel=Common.DebugLevel_Mega;//debug only
						this.menuitemEditScript.Enabled=false;
						this.menuitemEditMain.Enabled=false;
						Console.Error.Write("LoadProfile...");
						Console.Error.Flush();
						BackupProfileFolder_FullName=".";
						string BackupProfileFolder_FullName_TEMP="."+Common.sDirSep+"profiles"+Common.sDirSep+sValue;
						Console.Error.Write("checking \"" + BackupProfileFolder_FullName_TEMP + "\"...");
						Console.Error.Flush();
						if (Directory.Exists(BackupProfileFolder_FullName_TEMP)) {
							Console.Error.Write("found...");
							Console.Error.Flush();
							DirectoryInfo diProfileX = new DirectoryInfo(BackupProfileFolder_FullName_TEMP);
							BackupProfileFolder_FullName = diProfileX.FullName;
							this.menuitemEditScript.Enabled=true;
							this.menuitemEditMain.Enabled=true;
							Common.ClearInvalidDrives();
							Common.ClearExtraDestinations();
							RunScript(BackupProfileFolder_FullName + Common.sDirSep + MainFile_Name); //excludes and adds destinations
							if (File.Exists(BackupProfileFolder_FullName + Common.sDirSep + LogFile_Name)) RunScript(BackupProfileFolder_FullName + Common.sDirSep + LogFile_Name); //excludes and adds destinations
							bLoadedProfile=true;
							Common.UpdateSelectableDrivesAndPseudoRoots(true);
							Common.sParticiple="finished updating Drives and PseudoRoots";
							if (Common.bMegaDebug) {
								Console_Error_WriteLine_AllDebugInfo();
							}
							alPseudoRootsNow=Common.PseudoRoots_DriveRootFullNameThenSlash_ToArrayList();
							
							comboDest.BeginUpdate();
							comboDest.Items.Clear();
							//comboDest.Items.Clear();//already done above
							foreach (string sNow in alPseudoRootsNow) {
								comboDest.Items.Add(Common.LocalFolderThenSlash(sNow) 
								                 + DestSubfolderRelNameThenSlash);
							}
							comboDest.EndUpdate();
							
							//FolderLister.Echo("Test");
							alSelectableDrives=Common.SelectableDrives_DriveRootFullNameThenSlash_ToArrayList();
							string sMsg="(unknown error while listing usable destinations in RunScriptLine)";
							if (alPseudoRootsNow!=null && alPseudoRootsNow.Count > 0) {//iSelectableDrives+iDestinations>0) {
								if (bExitIfNoUsableDrivesFound && alSelectableDrives.Count == 0) {
									sMsg = "No usable backup drives can be found.  Try connecting the drive and then try again.";
									Console.Error.WriteLine(sMsg);
									if (Common.bDebug) Console_Error_WriteLine_AllDebugInfo();
									MessageBox.Show(sMsg);
								}
								comboDest.SelectedIndex = 0;
							}
							else {
								sMsg= "No backup drive can be found.  Try connecting the drive and then try again.";
								Console.Error.WriteLine(sMsg);
								MessageBox.Show(sMsg);
								if (Common.bDebug) Console_Error_WriteLine_AllDebugInfo();
							}
							this.lblProfile.Text="Profile: "+sValue;
						}//end if profile folder exists
						else {
							string sMsg="Unable to open profile \"" + sValue + "\"!";
							Console.Error.WriteLine(sMsg);
							MessageBox.Show(sMsg);
						}
					}//end if (sCommandLower=="LoadProfile")
					else if (sCommandLower=="ulbytecounttotalprocessed") {
						ulByteCountTotalProcessed_LastRun=ulong.Parse(sValue);
					}
					else if (sCommandLower=="exitifnousabledrivesfound") {
						bExitIfNoUsableDrivesFound=ToBool(sValue);
					}
					else if (sCommandLower=="alwaysstayopen") {
						bAlwaysStayOpen=ToBool(sValue);
					}
					else if (sCommandLower=="testonly") {
						bTestOnly=ToBool(sValue);
						Output("Test mode turned "+(bTestOnly?"on":"off")+"."+(bTestOnly?"  No files will be copied.":""));
					}
					else if (sCommandLower=="destsubfolder") {
						DestSubfolderRelNameThenSlash=Common.LocalFolderThenSlash(sValue);
					}
					else {
						Console.Error.WriteLine("Unknown Command!: "+sCommandLower+":"+sValue);
						bForceBad=true;
					}
					bGood=true;
					if (bForceBad) bGood=false;
				}//end if has ":" in right place
			}
			catch (Exception exn) {
				Common.ShowExn(exn,"parsing line "+Common.SafeString(sLine,true)+" {iLine+1:"+(iLine+1).ToString()+"; aka--offset:"+iLine+"}");
				iCouldNotFinish++;
			}
			return bGood;
		}//end RunScriptLine
		
		void Console_Error_WriteLine_AllDebugInfo() {
			Console.Error.WriteLine("{\n PseudoRoot entry count:"+Common.GetPseudoRoots_EntriesCount().ToString()
									+";\n PseudoRoot non-null entries:"+Common.GetPseudoRoots_CountNonNull(false).ToString()
									+";\n PseudoRoot non-null entries including entries past end:"+Common.GetPseudoRoots_CountNonNull(true).ToString()
									+";\n PseudoRoot Array"+Common.GetPseudoRootArrayMsg_LengthColonCount_else_ColonNull()
									+";\n SafeCount(alSelectableDrives):"+Common.SafeCount(alSelectableDrives).ToString()
									+";\n SafeCount(alPseudoRootsNow):"+Common.SafeCount(alPseudoRootsNow).ToString()
									+";\n Selectable Drive Array"+Common.GetSelectableDriveArrayMsg_LengthColonCount_else_ColonNull()
									+";\n Selectable Drive entries:"+Common.GetSelectableDriveMsg_EntriesCount().ToString()
									+";\n Selectable Drive non-null entries:"+Common.GetSelectableDrives_CountNonNull(false).ToString()
									+";\n Selectable Drive non-null entries including entries past end:"+Common.GetSelectableDrives_CountNonNull(true).ToString()
									+";\n Invalid Destinations ArrayList:"+Common.ToString(Common.GetInvalidDrivesList(),"   ")
								+"\n}");
	
		}
		
		void TableLayoutPanel1Paint(object sender, PaintEventArgs e) {
			
		}
		
		void MenuitemGoClick(object sender, EventArgs e) {
			if (this.comboDest.Text!="") {
				menuitemGo.Enabled=false;
				menuitemEditMain.Enabled=false;
				menuitemEditScript.Enabled=false;
				menuitemHelp_ViewOutputOfLastRun.Enabled=false;
				bUserCancelledLastRun=false;
				bCopyErrorLastRun=false;
				bDiskFullLastRun=false;
				//iFilesTooBigToFitLastRun//bFileTooBigToFitLastRun=false;
				bool bGood=true;
				
				//if (!FixSlashes()) bGood=false;
				if (!SetDestFolder(comboDest.Text)) bGood=false;
				if (bDeleteFilesNotOnSource_BeforeBackup) {
					string sDestPrefix=Common.LocalFolderThenSlash(DestFolder_FullName)+DestSubfolderRelNameThenSlash;
					if (sDestPrefix.Length>1&&sDestPrefix.EndsWith(Common.sDirSep)) sDestPrefix=sDestPrefix.Substring(0,sDestPrefix.Length-1);
					//TODO: DeleteIfNotOnSource_Recursively(sDestPrefix);
					//TODO: MessageBox.Show("Reconstructed source \""+sReturn+"\" "+((Directory.Exists(sReturn))?"exists":"does not exist"));
				}
				
				if (ulByteCountTotalProcessed_LastRun>0) {
					//mainformNow.progressbarMain.Style=ProgressBarStyle.Continuous; //should be already set
				}
				else {
					mainformNow.progressbarMain.Style=ProgressBarStyle.Marquee;
				}
				if (!RunScript(MainForm.BackupProfileFolder_FullName+Common.sDirSep+ScriptFile_Name)) bGood=false;
				if (mainformNow.progressbarMain.Style==ProgressBarStyle.Marquee) {
					mainformNow.progressbarMain.Style=ProgressBarStyle.Continuous;
					mainformNow.progressbarMain.Value=bGood?mainformNow.progressbarMain.Maximum:(mainformNow.progressbarMain.Maximum/2);
				}
				
				Output((ulByteCountTotalProcessed/1024/1024/1024).ToString()+"GB = "+(ulByteCountTotalProcessed/1024/1024).ToString()+"MB = "+(ulByteCountTotalProcessed/1024).ToString()+"KB = "+ulByteCountTotalProcessed.ToString()+"bytes of "+(ulByteCountTotal/1024/1024).ToString()+"MB source data finished, "+(ulByteCountTotalActuallyCopied/1024/1024).ToString()+"MB difference copied).",true);
				try {
					if (bGood&&!MainForm.bUserCancelledLastRun) {
						Output("Opening log \""+BackupProfileFolder_FullName + Common.sDirSep + LogFile_Name+"\"",true);
						StreamWriter streamOut=new StreamWriter(BackupProfileFolder_FullName + Common.sDirSep + LogFile_Name);
						Output("Writing log (statistics)...",true);
						streamOut.WriteLine("ulByteCountTotalProcessed:"+ulByteCountTotalProcessed.ToString());
						streamOut.Close();
						Output("Writing log (statistics)...OK ("+LogFile_Name+")",true);
					}
				}
				catch (Exception exn) {
					Console.Error.WriteLine();
				}
				int iMessages=0;
				if (!bGood) {
					lbOut.Items.Add("Some files may be system files and are not required to be backed up.  RunScript failed.");
					iMessages++;
					Application.DoEvents();
					WriteLastRunLog();
					menuitemHelp_ViewOutputOfLastRun.Enabled=true;
					FileInfo fiSaved=new FileInfo(sLastRunLog);
					//DialogResult dlgresultNow=MessageBox.Show("Finished.\n\nLog ("+iMessages.ToString()+" message(s)) saved to \""+fiSaved.FullName+"\"\n\n  Do you wish to to review the list?","Result", MessageBoxButtons.YesNo);
					MessageBox.Show("Finished.\n\nLog ("+iMessages.ToString()+" message(s)) saved to \""+fiSaved.FullName+"\"",sMyName);//DialogResult dlg=MessageBox.Show(sFileList+"\n\n  Do you wish to to review the list (press cancel to exit)?","Result", MessageBoxButtons.OKCancel);
					//if (dlgresultNow==DialogResult.Yes) {
					//	System.Diagnostics.Process.Start(sLastRunLog);
					//}
					
					this.menuitemGo.Enabled = true;
				}
				else {
					if (bUserCancelledLastRun) {
						MessageBox.Show("Cancelled Backup.");
					}
					else if (bDiskFullLastRun) {
						string sMsg="Destination drive could not fit all files - Could not finish";
						lbOut.Items.Add(sMsg);
						Application.DoEvents();
						iMessages++;
						MessageBox.Show(sMsg);
					}
					else if (bCopyErrorLastRun) {
						int iActualNamesFound=0;
						//string sFileList="";
						string FileNow_FullName;
						string FileNow_Name;
						string FileNow_Directory_FullName;
						if (alCopyError!=null&&alCopyError.Count>0) {
							iMessages=alCopyError.Count;
							foreach (string sCopyErrorNow in alCopyError) {
								if (sCopyErrorNow!=null&&sCopyErrorNow!="") {
									if (iActualNamesFound>=iMaxCopyErrorsToShow) {
										//sFileList+="\n(there are more errors but this list has been limited to "+iMaxCopyErrorsToShow.ToString()+")";
										break;
									}
									int iFileFullNameStart=sCopyErrorNow.IndexOf(sCopyErrorFileFullNameOpener);
									int iCopyErrorEnder=iFileFullNameStart;
									int iFileFullNameEnder=sCopyErrorNow.IndexOf(sCopyErrorFileFullNameCloser);
									if (iFileFullNameStart>-1) {
										iFileFullNameStart+=sCopyErrorFileFullNameOpener.Length;
										if (iFileFullNameEnder>-1) FileNow_FullName=sCopyErrorNow.Substring(iFileFullNameStart,(iFileFullNameEnder-iFileFullNameStart));
										else FileNow_FullName=sCopyErrorNow.Substring(iFileFullNameStart);
									}
									else FileNow_FullName="";
									while (FileNow_FullName.EndsWith(Common.sDirSep)) {
										if (FileNow_FullName==Common.sDirSep) break;
										else FileNow_FullName=FileNow_FullName.Substring(0,FileNow_FullName.Length-1);
									}
									int iLastSlash=FileNow_FullName.LastIndexOf(Common.sDirSep);
									if (iLastSlash>=0) {
										if (FileNow_FullName==Common.sDirSep) {
											FileNow_Name="(root filesystem)";
											FileNow_Directory_FullName="/";
										}
										else {
											FileNow_Name=FileNow_FullName.Substring(iLastSlash+1);
											if (iLastSlash==0) FileNow_Directory_FullName=FileNow_FullName.Substring(0,1);
											else FileNow_Directory_FullName=FileNow_FullName.Substring(0,iLastSlash);
										}
										if (FileNow_Name=="") FileNow_Name="(unknown file)";
										if (FileNow_Directory_FullName=="") FileNow_Directory_FullName="(unknown location)";
										//sFileList+=" \n"+FileNow_Name+" in "+FileNow_Directory_FullName;
										//if (iCopyErrorEnder>0) sFileList+=" *"+sCopyErrorNow.Substring(0,iCopyErrorEnder);//intentionally iCopyErrorEnder>0 so that it is ignored if ender is at location zero
									}
									else {
										FileNow_Name="";
										FileNow_Directory_FullName="";
										//if (FileNow_FullName!="") sFileList+=" \n"+FileNow_FullName;
										//else sFileList+=" \n"+"(?)";
									}
									
									//if (sFileList=="") sFileList="Copy Error while attempting to backup the following files: ";
									iActualNamesFound++;
								}//end if copy error string !=null
							}//end for each sCopyError in alCopyError
						}
						else {
							//if (sFileList=="") {
								lbOut.Items.Add("Unknown copy error.");
								iMessages++;
								Application.DoEvents();
								//sFileList="Copy error: could not copy "+alCopyError.Count.ToString()+" file"+((alCopyError.Count!=1)?"s":"")+".";
								
							//}
						}
						//MessageBox.Show(sFileList);
						
						WriteLastRunLog();
						FileInfo fiSaved=new FileInfo(sLastRunLog);
						MessageBox.Show("Finished Backup.\n\nLog ("+iMessages.ToString()+" message(s)) saved to \""+fiSaved.FullName+"\"",sMyName);//DialogResult dlg=MessageBox.Show(sFileList+"\n\n  Do you wish to to review the list (press cancel to exit)?","Result", MessageBoxButtons.OKCancel);
						//if (dlg==DialogResult.OK) bUserSaysStayOpen=true;
						//else
							bUserSaysStayOpen=false;
					}//end if bCopyErrorLastRun
					else {
						WriteLastRunLog();
						FileInfo fiSaved=new FileInfo(sLastRunLog);
						if (iMessages<=0) MessageBox.Show("Finished Backup.");//\n\nLog saved to \""+fiSaved.FullName+"\"",sMyName);
						else MessageBox.Show("Finished Backup.\n\nLog ("+iMessages.ToString()+" message(s)) saved to \""+fiSaved.FullName+"\"",sMyName);
					}
					if ((bCopyErrorLastRun&&!bUserSaysStayOpen) || !bTestOnly && !bAlwaysStayOpen)
						Application.Exit(); // && !bCopyErrorLastRun
				}//end else bGood
				menuitemEditMain.Enabled=true;
				menuitemEditScript.Enabled=true;
			}
			else {
				MessageBox.Show(Common.LimitedWidth("No destination drive is present.  Connect a flash drive, external hard drive, or other backup media and open the "+sMyName+" icon again.",40,"\n",true));
			}			
		}//end MenuitemGoClick
		
		void WriteLastRunLog(string sOnlyDateAndThisText) {
			try {
				StreamWriter streamOut=new StreamWriter(sLastRunLog);
				DateTime dtNow=DateTime.Now;
				streamOut.WriteLine("# "+sMyNameAndVersion+" (early-quit message only) "+dtNow.Year+"-"+dtNow.Month+"-"+dtNow.Day+" "+dtNow.Hour+":"+dtNow.Minute+":"+dtNow.Second);
				streamOut.WriteLine(sOnlyDateAndThisText);
				streamOut.Close();
			}
			catch (Exception exn) {
				Console.Error.WriteLine();
				Console.Error.WriteLine("Could not finish :"+exn.ToString());
			}
		}
		void WriteLastRunLog() {
			try {
				StreamWriter streamOut=new StreamWriter(sLastRunLog);
				DateTime dtNow=DateTime.Now;
				streamOut.WriteLine("# "+sMyNameAndVersion+"  "+dtNow.Year+"-"+dtNow.Month+"-"+dtNow.Day+" "+dtNow.Hour+":"+dtNow.Minute+":"+dtNow.Second);
				//streamOut.WriteLine("Could not copy:");
				//streamOut.WriteLine(sFileList); //NOTE: lbOut includes "(could not copy)" messages
				//streamOut.WriteLine();
				streamOut.WriteLine("Output:");
				for (int i=0; i<this.lbOut.Items.Count; i++) {
					streamOut.WriteLine(this.lbOut.Items[i].ToString());
				}
				streamOut.Close();
			}
			catch (Exception exn) {
				Console.Error.WriteLine();
				Console.Error.WriteLine("Could not finish WriteLastRunLog:"+exn.ToString());
			}
		}//end WriteLastRunLog
		
		void MenuitemCancelClick(object sender, EventArgs e) {
			//if (bBusyCopying) {
			//	menuitemCancel.Enabled=false;
			//	bUserCancelledLastRun=true;
			//}
			//else {
			if (!bUserCancelledLastRun) {
				//if (flisterNow!=null&&flisterNow.IsBusy) flisterNow.Stop();
				menuitemCancel.Enabled=false;
				bUserCancelledLastRun=true;
			}
			//}
		}//end MenuitemCancelClick
		
		void MenuitemEditMainClick(object sender, EventArgs e) {
			try {
				DialogResult dlgresult=MessageBox.Show(Common.LimitedWidth("In order for any changes you make to take effect, you must close "+MainFile_Name+", close "+sMyName+" then open the "+sMyName+" icon again.",40,"\n",true), sMyName, MessageBoxButtons.OKCancel);
				if (dlgresult==DialogResult.OK) System.Diagnostics.Process.Start(BackupProfileFolder_FullName+Common.sDirSep+MainFile_Name);
			}
			catch (Exception exn) {
				Common.ShowExn(exn,"opening "+Common.SafeString(MainFile_Name,true)+" for profile","menuitemEditMainClick");
			}			
		}//end MenuitemEditMainClick
		
		void MenuitemEditScriptClick(object sender, EventArgs e) {
			try {
				DialogResult dlgresult=MessageBox.Show(Common.LimitedWidth(ScriptFile_Name +" must be saved before returning to "+sMyName+" and pressing \"Go\" in order for any changes you make to take effect.",40,"\n",true), sMyName, MessageBoxButtons.OKCancel);
				if (dlgresult==DialogResult.OK) System.Diagnostics.Process.Start(BackupProfileFolder_FullName+Common.sDirSep+ScriptFile_Name);
			}
			catch (Exception exn) {
				Common.ShowExn(exn,"opening \""+ScriptFile_Name+"\" in profile","menuitemEditScriptClick");
			}			
		}//end MenuitemEditScriptClick

		void SaveOutputToolStripMenuItemClick(object sender, EventArgs e) {
			if (SaveOutputToTextFile()) {
				MessageBox.Show("SaveOutput...OK (Saved to "+OutputFile_FullName+")");
			}
			else {
				MessageBox.Show("SaveOutput...Failed (could not write to "+OutputFile_FullName+")");
			}
		}
		/* NOTE: no longer needed since flister was used for "non-realtime" backup
		void BackupTree(DirectoryInfo diBase) {
			try {
				foreach (DirectoryInfo diNow in diBase.GetDirectories()) {
					sLastFileUsed=diNow.FullName;
					if (!Common.IsExcludedFolder(diNow)) {
						iListedLines++;
						FileAttributes fileattribNow = File.GetAttributes(sListedItem);
						//FileInfo fiNow=new FileInfo(sListedItem);
						//if (fiNow.Attributes&FileAttributes.Directory
						if ((fileattribNow & FileAttributes.Directory) == FileAttributes.Directory) {
							ReconstructPathOnBackup(sListedItem);
						}
						Application.DoEvents();
						BackupTree(diNow);
					}//end if not excluded
					if (bUserCancelledLastRun) break;
				}//end foreach
				foreach (FileInfo fiNow in diBase.GetFiles()) {
					sLastFileUsed=fiNow.FullName;
					if (!Common.IsExcludedFile(diBase,fiNow)) {
							//FileInfo fiNow=new FileInfo(sListedItem);
							//if (fiNow.Exists)
							ReconstructPathOnBackup(fiNow.DirectoryName);
							BackupFile(fiNow,true);
						if (bTestOnly) Output(sListedItem,true);
						if (bUserCancelledLastRun||bDiskFullLastRun) break; //do NOT stop if Copy Error only
					}//end if not excluded
					if (bUserCancelledLastRun) break;
					BackupFile(fiNow.FullName,true,fiNow)
				}//end foreach
			}
			catch (Exception exn) {
				Common.HandleListException(diBase,exn,"MainForm.BackupTree");
			}
			Application.DoEvents();
		}//end BackupTree
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="diNow"></param>
		/// <param name="exn"></param>
		/// <param name="sMethod"></param>
		public static void HandleListException(DirectoryInfo diNow, Exception exn, string sMethod) {
			try {
				if (alSkippedDueToException==null) alSkippedDueToException=new ArrayList();
				string sMsg=exn.ToString();
				string sRecord="";
				sRecord=Common.sLastFileUsed;//if (diNow!=null) sRecord=diNow.FullName;
				
				if (sMsg.IndexOf("Unauthorized")>-1) {
					if (sRecord!="") sRecord="No file permissions:"+sRecord;
					sMsg=sMethod+" -- Access to "+diNow.FullName+" was forbidden:";
				}
				else if (sMsg.IndexOf("ERROR_NO_MORE_FILES")>-1) {
					//this is ok, it just means there are no files to list here
					sMsg="";
					//if (sRecord!="") sRecord="No files could be listed:"+sRecord;
				}
				else if (sMsg.IndexOf("FileNotFoundException")>-1) {
					if (sRecord!="") sRecord="Can't be found:"+sRecord;
					sMsg="Error in "+sMethod+" -- \""+diNow.FullName+"\" Cannot be found:";
				}
				else {
					if (sRecord!="") sRecord="Could not list:"+sRecord;
					sMsg="Error in "+sMethod+" -- listing \""+diNow.FullName+"\":";
				}
				if (sMsg!="") {
					if (sRecord!="") alSkippedDueToException.Add(sRecord);
					Console.Error.WriteLine();
					Console.Error.WriteLine(sMsg);
					Console.Error.WriteLine("(\""+Common.sLastFileUsed+"\")");
					Console.Error.WriteLine(exn.ToString());
				}
			}
			catch (Exception exn2) {
				Console.Error.WriteLine();
				Console.Error.WriteLine("HandleListException error:");
				Console.Error.WriteLine(exn2.ToString());
			}
		}//end HandleListException
		
		
		void MenuitemHelp_ViewOutputOfLastRunClick(object sender, EventArgs e) {
			try {
				System.Diagnostics.Process.Start(sLastRunLog);
			}
			catch {}
		}
	}//end MainForm
}//end namespace
