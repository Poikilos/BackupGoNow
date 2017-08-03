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
using System.Threading;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
//using System.Management;//for getting free disk space (ManagementObject)
//using System.Text;//StringBuilder etc

//TODO:Fix margins correctly

namespace OrangejuiceElectronica {
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form {
		public static string sMyNameAndVersion="Backup GoNow 2009-09-09";
		public static string sMyName="Backup GoNow";
		//TODO: Option to remove files from the backup drive that aren't in the backup script
		public static int iLine=0;
		public static int iListedLines=0;
		public static int iCouldNotFinish=0;
		public static string DefaultProfile_Name="BackupGoNowDefault";
		public static bool bLoadedProfile=false;
		public static int iMaxCopyErrorsToShow=10;
		public static bool bUserSaysStayOpen=false;
		public static char[] carrBreakAfter=new char[] {'-',':','/','\\'};
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
		public static bool bRealTime=true;
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
		public static MainForm mainformNow=null;
		public static ListBox lbOutNow=null;
		public static ulong ulByteCountTotalProcessed_LastRun=0;
		public static int iLBRightMargin=0;
		public static int iLBBottomMargin=0;
		public static int iTickLastRefresh=Environment.TickCount;
		public static int iTicksRefreshInterval=2000;
		private static FolderLister flisterNow=null;
		private static bool bBusyCopying=false;
		private static bool bExitIfNoUsableDrivesFound=false;
		private static bool bAlwaysStayOpen=false;
		private static bool bUserCancelledLastRun=false;
		private static bool bCopyErrorLastRun=false;
		private static bool bDiskFullLastRun=false;
		private static int iSkipped=0;
		private static ArrayList alSkipped=new ArrayList();
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
		private static string sParticipleNow="";
		private static bool bMegaDebug=false;
		private static string sParticiple {
			set { sParticipleNow=value; if (bMegaDebug) Console.Error.WriteLine(value!=null?value:"(null debug info)"); }
			get { return sParticipleNow; }
		}
		
		public static ArrayList alInvalidDrives=new ArrayList();
		public static ArrayList alExtraDestinations=new ArrayList();
		public static ArrayList alFilesBackedUpManually=new ArrayList();
		public static int iValidDrivesFound=0;
		public static int iDestinations=0;
		private static long ulByteCountDestTotalSize=0;
		private static long ulByteCountDestAvailableFreeSpace=0;
		private static string DestFolder_FullName="";
		private static string DestSuffixOnlyThenSlash="";
		
		private const int MaxLocations=256;
		private static int iLocations=0;
		private static LocInfo[] locinfoarr=null;//see MainForm constructor
		
		private static ArrayList alFolderFullName=new ArrayList();
		private static ArrayList alFolderLabel=new ArrayList();
		private static ArrayList alFolder=new ArrayList();
		
		//private static string GetDestDriveRoot() { //="";//drive or folder
		//	try {
		//		return (driveinfoarrNow!=null&&iDriveDest>-1&&iDriveDest<driveinfoarrNow.Length) ? locinfoarr[DestFolderIndexNow].RootDirectory.FullName : "";
		//	}
		//	catch (Exception exn) {
		//		string sMsg="Error getting drive root folder: "+ToOneLine(exn);
		//		Output(sMsg);
		//		Console.Error.WriteLine(sMsg);
		//	}
		//	return "";
		//}
		public static int FolderDriveIndex(string FolderNow) {
			int iReturn=-1;
			if (FolderNow!=null&&FolderNow!=""&&driveinfoarrNow!=null) {
				FolderNow=LocalFolderThenSlash(FolderNow);
				for (int i=driveinfoarrNow.Length-1; i>=0; i--) {//start at end in case subfolder is a mounted volume
					if (driveinfoarrNow[i]!=null) {
						if (FolderNow.StartsWith(LocalFolderThenSlash(driveinfoarrNow[i].RootDirectory.FullName))) {
							iReturn=i;
							break;
						}
					}
				}
			}
			return iReturn;
		}//end FolderToDriveIndex
		public static int FolderLocationIndex(string FolderNow) {
			int iReturn=-1;
			if (FolderNow!=null&&FolderNow!=""&&locinfoarr!=null) {
				FolderNow=LocalFolderThenSlash(FolderNow);
				for (int i=locinfoarr.Length-1; i>=0; i--) {//start at end in case subfolder is a mounted volume
					if (locinfoarr[i]!=null) {
						if (FolderNow.StartsWith(LocalFolderThenSlash(locinfoarr[i].DriveRoot_FullNameThenSlash))) {//TODO: change this?
							iReturn=i;
							break;
						}
					}
				}
			}
			return iReturn;
		}//end FolderLocationIndex
		public static bool AddLocation(string FolderNow) {
			bool bGood=false;
			try {
				if (iLocations<locinfoarr.Length) {
					locinfoarr[iLocations]=new LocInfo();
					locinfoarr[iLocations].DriveRoot_FullNameThenSlash=LocalFolderThenSlash(FolderNow);
					int iDrive=FolderDriveIndex(FolderNow);
					if (iDrive>-1) {
						locinfoarr[iLocations].TotalSize=driveinfoarrNow[iDrive].TotalSize;
						locinfoarr[iLocations].AvailableFreeSpace=driveinfoarrNow[iDrive].AvailableFreeSpace;
					}
					iLocations++;
				}
			}
			catch (Exception exn) {
				ShowExn(exn,"adding location","AddLocation");
			}
			return bGood;
		}//AddLocation
		public static string sWasUpToDate="Was Up to Date";
		public static Brush brushItemOther = Brushes.Black;
		public static SolidBrush brushItemWasUpToDate = new SolidBrush(Color.FromArgb(192, 192, 192)); //Brushes.Gray;
		public static DriveInfo[] driveinfoarrNow=null;
		//public static int iDriveDest=0;
		//public static string sDestRootThenSlash="";
		public static bool bStartedCopyingAnyFiles=false;
		//private static string sDestPathSlash {
		//	get { return sDestRootThenSlash+DestSuffixOnlyThenSlash; }
		//}
		private static string sDirSep=char.ToString(Path.DirectorySeparatorChar);
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
					MainForm.sParticiple="generating retry batch \".old\" backup filename";
					string sOldBat=RetryBatchFile_Name+".old";
					MainForm.sParticiple="deleting old retry batch";
					if (File.Exists(sOldBat)) {
						try {
							File.Delete(sOldBat);
						}
						catch (Exception exn) {
							ShowExn(exn,sParticiple);
						}
					}
					MainForm.sParticiple="moving previous retry batch to \""+sOldBat+"\"";
					File.Move(RetryBatchFile_Name,sOldBat);
				}
				catch (Exception exn) {
					ShowExn(exn,sParticiple);
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
				ShowExn(exn,"changing to directory of executable","MainForm constructor");
			}
			tbStatusNow=this.tbStatus;
			//string sMyPath = Assembly.GetExecutingAssembly().Location;
			//int iLastSlash=sMyPath.LastIndexOf(char.ToString(Path.DirectorySeparatorChar));
			//if (iLastSlash>-1) {
			//	sMyPath=sMyPath.Substring(0,iLastSlash);
			//	Directory.SetCurrentDirectory(sMyPath);
			//}
			locinfoarr=new LocInfo[MaxLocations];
			for (int i=0; i<MaxLocations; i++) {
				locinfoarr[i]=null;
			}
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
					ShowExn(exn,"adding batch retry line");
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
					ReconstructPathOnBackup(diNow.FullName);
					if (!bUserCancelledLastRun&&!bDiskFullLastRun
						&&flisterNow.UseFolder(diNow))
						BackupFolder(diNow);
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
					if (flisterNow.UseFile(diBase,fiNow)) {
						//barrUsedSrcFile[iSrcNow]=true;
						BackupFile(fiNow.FullName,true);
						if (bTestOnly) Output("  ("+fiNow.FullName+")",true);
						//if (bDeleteFilesNotOnSource_AfterCopyingEachFolder) alActuallyUsedSrcFiles.Add(fiNow.Name);
						//iSrcNow++;
					}
				}//end foreach file
				if (bDeleteFilesNotOnSource_AfterCopyingEachFolder&&!bSourceListingWasCancelled) {
					DirectoryInfo diTarget=new DirectoryInfo(ReconstructedBackupPath(diBase.FullName));
					bool bFoundOnSource=false;
					foreach (DirectoryInfo diDest in diTarget.GetDirectories()) {
						bFoundOnSource=false;
						foreach (DirectoryInfo diSource in diarrSrc) {
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
						foreach (FileInfo fiSource in fiarrSrc) {
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
					}
				}
			}
			catch {} //no files
			iDepth--;
		}//end BackupFolder recursively
		public static readonly string SlashWildSlash=sDirSep+"*"+sDirSep;
		bool RunScript(string sFileX) {
			if (alSkipped!=null||alCopyError!=null) { //TODO: recheck logic.  This used to be done below (see identical commented lines)
				if (alSkipped.Count!=0||alCopyError.Count>0) {
					Output("Clearing error cache...",true);
				}
				alSkipped.Clear();
				alCopyError.Clear();
			}
			int iFilesProcessedPrev=iFilesProcessed;
			int iFilesProcessed_ThisScript=0;
			bool bGood=false;
			StreamReader streamIn=null;
			iCouldNotFinish=0;
			try {
				if (alSkipped!=null) alSkipped.Clear();
				else alSkipped=new ArrayList();
				if (!File.Exists(sFileX)) {
					Console.Error.WriteLine("File does not exist: \"" + sFileX + "\"!");
				}
				else {
					Console.Error.WriteLine("Reading \"" + sFileX + "\":");
				}
				streamIn=new StreamReader(sFileX);
				string sLine;
				flisterNow=new FolderLister();
				//flisterNow.MinimumFileSize=1;//1byte (trying to avoid bad symlinks here)
				flisterNow.bShowFolders=true;
				iLine=0;
				iListedLines=0;
				while ( (sLine=streamIn.ReadLine()) != null ) {
					if (sLine.StartsWith("#")) Console.Error.WriteLine("\t"+sLine);
					RunScriptLine(sLine);
					iLine++;
					if (bDiskFullLastRun||bUserCancelledLastRun) break; //do NOT stop if Copy Error only
				}//end while lines in script
				//if (bTestOnly) {
				if (alSkipped.Count>0) {
					Output("");
					Output("Could not list "+alSkipped.Count.ToString()+":",true);
					foreach (string sSkippedNow in alSkipped) {
						Output("(could not list) "+sSkippedNow);
					}
				}
				if (alCopyError.Count>0) {
					Output("");
					Output("Could not copy "+alCopyError.Count.ToString(),true);
					foreach (string sCopyErrorNow in alCopyError) {
						Output("(could not copy) "+sCopyErrorNow);
					}
					Output("");
				}
				iFilesProcessed_ThisScript=iFilesProcessed-iFilesProcessedPrev;
				if (iListedLines+alSkipped.Count+alCopyError.Count+iFilesProcessed_ThisScript>0) Output("Finished reading "+sFileX+" (listed: "+iListedLines+"; could not list: "+alSkipped.Count.ToString()+"; copy errors: "+alCopyError.Count.ToString()+"; files listed:"+iFilesProcessed_ThisScript.ToString()+").",true);
				else Output("Finished reading "+sFileX+" (commands processed)");
				//if (alSkipped!=null||alCopyError!=null) {
				//	if (alSkipped.Count!=0||alCopyError.Count>0) {
				//		Output("Clearing error cache...",true);
				//	}
				//	alSkipped.Clear();
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
				string sParticiple = "running " + SafeString(sFileX,true) + ":";
				if (bTestOnly) MessageBox.Show("Error "+sParticiple+"\n"+exn.ToString(),"Backup GoNow");
				ShowExn(exn,sParticiple,"RunScript");
				bGood=false;
			}
			try {
				if (streamIn!=null) streamIn.Close();
			}
			catch {}
			if (iFilesProcessed_ThisScript==0) iFilesProcessed_ThisScript=iFilesProcessed-iFilesProcessedPrev;
			if (bRemoveTrivialMessagesAfterScript&&bOutputTrivial&&(iFilesProcessed_ThisScript+alSkipped.Count+alCopyError.Count>0)) {
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
				ShowExn(exn,"simplifying output",String.Format("RemoveTrivialMessages(){{iLine:{0}}}:",iLine));
			}
			lbOutNow.EndUpdate();
		}
		//public static string WithoutEndDirSep(string sPathX) {
		//	if (sPathX.Length>1&&sPathX.EndsWith(sDirSep)) {
		//		sPathX=sPathX.Substring(0,sPathX.Length-1);
		//	}
		//	return sPathX;
		//}//end WithoutEndDirSep

		public static string FolderThenSlash(string sPath, string DirectoryDelimiter) {
			return (sPath!=null&&sPath!="")?(sPath.EndsWith(DirectoryDelimiter)?sPath:sPath+DirectoryDelimiter):DirectoryDelimiter;
		}
		public static string SlashThenFolder(string sPath, string DirectoryDelimiter) {
			return (sPath!=null&&sPath!="")?(sPath.StartsWith(DirectoryDelimiter)?sPath:DirectoryDelimiter+sPath):DirectoryDelimiter;
		}
		public static string FolderThenNoSlash(string sPath, string DirectoryDelimiter) {
			return (sPath!=null&&sPath!="")?(sPath.EndsWith(DirectoryDelimiter)?sPath.Substring(0,sPath.Length-1):sPath):"";
		}
		public static string NoSlashThenFolder(string sPath, string DirectoryDelimiter) {
			return (sPath!=null&&sPath!="")?(sPath.StartsWith(DirectoryDelimiter)?sPath.Substring(1):sPath):"";
		}
		public static string RemoteFolderThenSlash(string sPath) {
			return FolderThenSlash(sPath,"/");
		}
		public static string RemoteFolderThenNoSlash(string sPath) {
			return FolderThenNoSlash(sPath,"/");
		}
		public static string LocalFolderThenSlash(string sPath) {
			return FolderThenSlash(sPath,sDirSep);
		}
		public static string LocalFolderThenNoSlash(string sPath) {
			return FolderThenNoSlash(sPath,sDirSep);
		}
		
		public bool SetDestFolder(string FolderNow) {
			bool bGood=false;
			int DestFolderIndexNow=-1;
			sParticiple="initializing";
			//locinfoarr[DestFolderIndexNow]
			try {
				if (FolderNow!=null&&FolderNow!="") {
					sParticiple="removing trailing '"+sDirSep+"'";
					FolderNow=LocalFolderThenNoSlash(FolderNow);
					sParticiple="checking driveinfoarrNow";
					DestFolderIndexNow=MainForm.FolderLocationIndex(FolderNow);
					if (DestFolderIndexNow<0) {
						AddLocation(FolderNow);
						Output("Adding location \""+FolderNow+"\"");
						DestFolderIndexNow=MainForm.FolderLocationIndex(FolderNow);
						Output("  at index "+DestFolderIndexNow);
					}
					else {
						Output("Found location \""+FolderNow+"\"");
						Output("  at index "+DestFolderIndexNow+" ("+locinfoarr[DestFolderIndexNow].DriveRoot_FullNameThenSlash+")");
					}
					if (DestFolderIndexNow>-1) {
						bGood=true;
						DestFolder_FullName=locinfoarr[DestFolderIndexNow].DriveRoot_FullNameThenSlash;
						if (DestFolder_FullName!=sDirSep&&DestFolder_FullName.EndsWith(sDirSep)) DestFolder_FullName=DestFolder_FullName.Substring(0,DestFolder_FullName.Length-sDirSep.Length);
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
				ShowExn(exn,sParticiple+String.Format(" {{driveinfoarrNow{0}; DestFolderIndexNow:{1}}}",(driveinfoarrNow!=null?".Length:"+driveinfoarrNow.Length:":null"),DestFolderIndexNow ),"SetDestFolder");
			}
			try {
				if (DestFolderIndexNow>-1) {
					ulByteCountDestTotalSize=(long)locinfoarr[DestFolderIndexNow].TotalSize;
					ulByteCountDestAvailableFreeSpace=(long)locinfoarr[DestFolderIndexNow].AvailableFreeSpace; //TotalFreeSpace doesn't count user quotas
					//Console.WriteLine( "{0}MB free {1}MB total ({2}bytes/{3}bytes) on {4} ({5})",
					//		(ulByteCountDestTotalSize/1024/1024), (ulByteCountDestAvailableFreeSpace/1024/1024), ulByteCountDestTotalSize, ulByteCountDestAvailableFreeSpace, locinfoarr[DestFolderIndexNow].VolumeLabel, locinfoarr[DestFolderIndexNow].DriveFormat );
				}
				else {
					ulByteCountDestTotalSize=Int64.MaxValue;
					ulByteCountDestAvailableFreeSpace=Int64.MaxValue;
					Console.WriteLine( "{0}MB free {1}MB total ({2}bytes/{3}bytes)",//debug only
								(ulByteCountDestTotalSize/1024/1024), (ulByteCountDestAvailableFreeSpace/1024/1024), ulByteCountDestTotalSize, ulByteCountDestAvailableFreeSpace );
					
				}
			}
			catch (Exception exn) {
				ShowExn(exn,"accessing locinfoarr["+DestFolderIndexNow.ToString()+"]:");
				ulByteCountDestTotalSize=0;
				ulByteCountDestAvailableFreeSpace=0;
			}
			Console.WriteLine("SetDestFolder:"+(FolderNow!=null?"\""+FolderNow+"\"":"null")+"(DestFolderIndexNow:"+DestFolderIndexNow.ToString()+")");//debug only
			return bGood;
		}//end SetDestFolder
		/// <summary>
		/// Makes sure that:
		/// -sDestRootThenSlash ends with slash (i.e. sDestRootThenSlash=LocalFolderThenSlash(sDestRootThenSlash) )
		/// -DestSuffixOnlyThenSlash does NOT start with slash, and DOES end with slash
		/// </summary>
		/*
		public bool FixSlashes() {
			bool bGood=false;
			SetDestFolder(this.comboDest.Text);
			//sDestRootThenSlash=DestFolder_FullName;//GetDestDriveRoot();
			if (sDestRootThenSlash!="") {
				bGood=true;
				if (!sDestRootThenSlash.EndsWith(sDirSep)) sDestRootThenSlash+=sDirSep;
			}
			else bGood=false;
			if (DestSuffixOnlyThenSlash!="") {
				while (DestSuffixOnlyThenSlash.StartsWith(sDirSep)) DestSuffixOnlyThenSlash=(DestSuffixOnlyThenSlash.Length>1)?DestSuffixOnlyThenSlash.Substring(1):"";
				if (!DestSuffixOnlyThenSlash.EndsWith(sDirSep)) DestSuffixOnlyThenSlash+=sDirSep;
			}
			return bGood;
		}//end FixSlashes
		*/
		public string ReconstructedSourcePath(string sBackupPath) {
			string sReturn=sBackupPath;
			//sBackupPath is constructed using: comboDest.Items.Add(LocalFolderThenSlash(locinfoarr[iNow].FullName) + DestSuffixOnlyThenSlash);
			string sDestPrefix=LocalFolderThenSlash(DestFolder_FullName)+DestSuffixOnlyThenSlash;
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
		}
		private static bool bShowReconstructedBackupPathError=true;
		public string ReconstructedBackupPath(string sSrcPath) {
			//Output("Reconstruction sSrcPath(as received): "+sSrcPath);//debug only
			//Output("Reconstruction DestFolder_FullName(as received): "+DestFolder_FullName);//debug only
			//NOTE: LocalFolderThenSlash just makes sure it ends with a slash and uses sDirSep
			string sReturn=LocalFolderThenSlash(DestFolder_FullName)+DestSuffixOnlyThenSlash;
			string sDestAppend=sSrcPath;
			int iStart=0;
			if (sDestAppend[iStart]=='/') {
				while (iStart<sDestAppend.Length&&sDestAppend[iStart]=='/') {
					iStart++;
				}
			}
			//else iStart=Chunker.IndexOfAnyDirectorySeparatorChar(sDestAppend); //uncommenting this removes the "C" folder if using this program in windows to backup local files
			if (iStart>-1&&iStart<sDestAppend.Length) {
				sDestAppend=sDestAppend.Substring(iStart);
				//if (bTestOnly) Output("Reconstruction(before normalize): "+sDestAppend);
				sDestAppend=Chunker.ConvertDirectorySeparatorsToNormal(sDestAppend);
				//if (bTestOnly) Output("Reconstruction(before removedouble): "+sDestAppend);
				sDestAppend=Chunker.RemoveDoubleDirectorySeparators(sDestAppend);
				if (sDestAppend!=null&&sDestAppend!=""&&sDestAppend.StartsWith(sDirSep)) {
					if (sDestAppend.Length>1) sDestAppend=sDestAppend.Substring(1);
					else sDestAppend="";
				}
			}
			else sDestAppend="";

			if (sDestAppend=="") {
				sReturn=LocalFolderThenSlash(DestFolder_FullName);
				if (bShowReconstructedBackupPathError) {
					MessageBox.Show("The backup source cannot be parsed so these files will be placed in \""+sReturn+"\".");
					bShowReconstructedBackupPathError=false;
				}
			}
			else sReturn+=sDestAppend;
			if ( !sReturn.EndsWith(sDirSep) )
				sReturn+=sDirSep;
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
				ArrayList alFoldersNotPreviouslyExisting=new ArrayList();
				if (!Chunker.CreateFolderRecursively(ref alFoldersNotPreviouslyExisting, BackupFolder_FullName)) bGood=false;
				foreach (string FolderNow_FullName in alFoldersNotPreviouslyExisting) {
					//if (!AlreadyMkdir(FolderNow_FullName)) {
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
				if (!bGood&&Chunker.sLastExn.ToLower().IndexOf("system.io.ioexception: disk full")>-1) bDiskFullLastRun=true;
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
				string sDirSep=char.ToString(Path.DirectorySeparatorChar);
				if (fiNow.Exists) {
					ulByteCountFolderNowDone+=(ulong)fiNow.Length;
					ulByteCountTotalProcessed+=(ulong)fiNow.Length;
					string BackupFolder_ThenSlash=bUseReconstructedPath?ReconstructedBackupPath(fiNow.Directory.FullName):LocalFolderThenSlash(DestFolder_FullName);
					if (bUseReconstructedPath) {
						if (!Directory.Exists(ReconstructedBackupPath(fiNow.Directory.FullName))) ReconstructPathOnBackup(fiNow.Directory.FullName);
					}
					if (!BackupFolder_ThenSlash.EndsWith(sDirSep)) BackupFolder_ThenSlash+=sDirSep;
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
				if (exn.ToString().ToLower().IndexOf("system.io.ioexception: disk full")>-1) bDiskFullLastRun=true;
				else if (exn.ToString().ToLower().IndexOf("system.io.directorynotfoundexception")>-1) {
					alCopyError.Add("Recreate source folder failed"+sCopyErrorFileFullNameOpener+SrcFile_FullName+sCopyErrorFileFullNameCloser+ToOneLine(exn.ToString()));
				}
				else {
					alCopyError.Add("Could not read"+sCopyErrorFileFullNameOpener+SrcFile_FullName+sCopyErrorFileFullNameCloser+ToOneLine(exn.ToString()));
					bCopyErrorLastRun=true;
				}
				ShowExn(exn,"backing up file","BackupFile");
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
				ShowExn(exn,"","MainForm.Flush");
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
					ShowExn(exn,"","Output("+((sLineX!=null)?("non-null"):("null"))+","+(bForceRefresh?"true":"false")+")");
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
				if (bRealTime) {
					//int iDot=sPercentFree.IndexOf(".");
					//if (iDot>-1) {
					//	if (iDot
					//}
					tbStatusNow.Text=String.Format( "{0}MB ({1} files) processed, {2}MB added      {3} " + ((ulByteCountDestAvailableFreeSpace==Int64.MaxValue)?"disk space unknown (not implemented in this version of your computer's framework)({4}/{5}MB)":"space remaining ({4}/{5}MB)"),
						(ulByteCountTotalProcessed/1024/1024), iFilesProcessed, lByteCountTotalActuallyAdded/1024/1024, sPercentFree, (lFree/1024/1024), ulByteCountDestTotalSize/1024/1024 )
						+(bAutoScroll?"":"...");
				}
				else tbStatusNow.Text=String.Format( "{0}MB / {1}MB counted so far",
						((ulByteCountTotalProcessed)/1024/1024),(ulByteCountTotal/1024/1024) )
						+  (bAutoScroll?"":"...");
		
			}
			catch (Exception exn) {
				ShowExn(exn,"updating progress bar");
			}
		}//end UpdateProgressBar
		/// <summary>
		/// Checks string against ExcludeDest strings (all, literal)
		/// </summary>
		/// <param name="DestName"></param>
		/// <returns>True if DestName was not specified by an ExcludeDest statement</returns>
		bool ValidDest(string DestName) {
			bool bValid=true;
			foreach (string sInvalid in alInvalidDrives) {
				if (DestName.ToLower()==sInvalid.ToLower()) {
					bValid=false;
					break;
				}
			}
			return bValid;
		}
		//bool ValidDestLabel(string DestName) {
		//	bool bValid=true;
		//	if (DestName!=null&&DestName.Length>0) {
		//		foreach (string sInvalid in alInvalidDrives) {
		//			if (DestName.ToLower()==sInvalid.ToLower()) {
		//				bValid=false;
		//				break;
		//			}
		//		}
		//	}
		//	return bValid;
		//}
		
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
				Directory_FullName=diBase.FullName;
				string FileNow_FullName="";
				long FileNow_Length=0;
				foreach (FileInfo fiNow in diBase.GetFiles()) {
					try {
						FileNow_FullName=fiNow.FullName;
						FileNow_Length=fiNow.Length;
						fiNow.Attributes=FileAttributes.Normal;//fiNow.Attributes^= FileAttributes.ReadOnly;
						//try {
						//	FileSecurity fisec=new FileSecurity();
						//	IdentityReference idref=new SecurityIdentifier(WellKnownSidType.SelfSid);
							
						//	fisec.SetOwner(idref);
						//}
						fiNow.Delete();
						if (bDecrementBytesAdded) MainForm.lByteCountTotalActuallyAdded-=FileNow_Length;
					}
					catch (Exception exn) {
						ShowExn(exn,"deleting file "+SafeString(FileNow_FullName,true));
					}
				}
				foreach (DirectoryInfo diSub in diBase.GetDirectories()) {
					DeleteFolderRecursively(diSub,bDecrementBytesAdded);
				}
				diBase.Attributes=FileAttributes.Normal;
				diBase.Delete();
			}
			catch (Exception exn) {
				ShowExn(exn,"deleting folder "+SafeString(Directory_FullName,true));
			}
		}
		/*
		void DeleteFolderRecursively(string Directory_FullName) {
			lByteCountTotalActuallyAdded-=
		}
		
		void DeleteIfNotOnSource_Recursively(string FolderNoSlash_FullName) {
			if ((FolderNoSlash_FullName+sDirSep)!=(sDestPrefix) {
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
		void BtnGoClick(object sender, EventArgs e)
		{
			if (this.comboDest.Text!="") {
				btnGo.Enabled=false;
				btnEditMain.Enabled=false;
				btnEditScript.Enabled=false;
				bUserCancelledLastRun=false;
				bCopyErrorLastRun=false;
				bDiskFullLastRun=false;
				bool bGood=true;
				
				//if (!FixSlashes()) bGood=false;
				if (!SetDestFolder(comboDest.Text)) bGood=false;
				if (bDeleteFilesNotOnSource_BeforeBackup) {
					string sDestPrefix=LocalFolderThenSlash(DestFolder_FullName)+DestSuffixOnlyThenSlash;
					if (sDestPrefix.Length>1&&sDestPrefix.EndsWith(sDirSep)) sDestPrefix=sDestPrefix.Substring(0,sDestPrefix.Length-1);
					//TODO: DeleteIfNotOnSource_Recursively(sDestPrefix);
					//TODO: MessageBox.Show("Reconstructed source \""+sReturn+"\" "+((Directory.Exists(sReturn))?"exists":"does not exist"));
				}
				
				if (ulByteCountTotalProcessed_LastRun>0) {
					//mainformNow.progressbarMain.Style=ProgressBarStyle.Continuous; //should be already set
				}
				else {
					mainformNow.progressbarMain.Style=ProgressBarStyle.Marquee;
				}
				if (!RunScript(MainForm.BackupProfileFolder_FullName+sDirSep+ScriptFile_Name)) bGood=false;
				if (mainformNow.progressbarMain.Style==ProgressBarStyle.Marquee) {
					mainformNow.progressbarMain.Style=ProgressBarStyle.Continuous;
					mainformNow.progressbarMain.Value=bGood?mainformNow.progressbarMain.Maximum:(mainformNow.progressbarMain.Maximum/2);
				}
				
				Output((ulByteCountTotalProcessed/1024/1024/1024).ToString()+"GB = "+(ulByteCountTotalProcessed/1024/1024).ToString()+"MB = "+(ulByteCountTotalProcessed/1024).ToString()+"KB = "+ulByteCountTotalProcessed.ToString()+"bytes of "+(ulByteCountTotal/1024/1024).ToString()+"MB source data finished, "+(ulByteCountTotalActuallyCopied/1024/1024).ToString()+"MB difference copied).",true);
				try {
					if (bGood&&!MainForm.bUserCancelledLastRun) {
						Output("Opening log \""+BackupProfileFolder_FullName + sDirSep + LogFile_Name+"\"",true);
						StreamWriter streamOut=new StreamWriter(BackupProfileFolder_FullName + sDirSep + LogFile_Name);
						Output("Writing log...",true);
						streamOut.WriteLine("ulByteCountTotalProcessed:"+ulByteCountTotalProcessed.ToString());
						streamOut.Close();
						Output("Writing log...OK",true);
					}
				}
				catch (Exception exn) {
					Console.Error.WriteLine();
				}
				if (!bGood) {
					MessageBox.Show("Backup was not complete.  Close all other programs and try again--some skipped files may be system files and are not required to be backed up.");
					this.btnGo.Enabled = true;
				}
				else {
					if (bUserCancelledLastRun) MessageBox.Show("Cancelled Backup");
					else if (bDiskFullLastRun) MessageBox.Show("Destination drive is full - Could not finish");
					else if (bCopyErrorLastRun) {
						int iActualNamesFound=0;
						string sFileList="";
						string FileNow_FullName;
						string FileNow_Name;
						string FileNow_Directory_FullName;
						if (alCopyError!=null&&alCopyError.Count>0) {
							foreach (string sCopyErrorNow in alCopyError) {
								if (sCopyErrorNow!=null&&sCopyErrorNow!="") {
									if (iActualNamesFound>=iMaxCopyErrorsToShow) {
										sFileList+="\n(there are more errors but this list has been limited to "+iMaxCopyErrorsToShow.ToString()+")";
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
									while (FileNow_FullName.EndsWith(sDirSep)) {
										if (FileNow_FullName==sDirSep) break;
										else FileNow_FullName=FileNow_FullName.Substring(0,FileNow_FullName.Length-1);
									}
									int iLastSlash=FileNow_FullName.LastIndexOf(sDirSep);
									if (iLastSlash>=0) {
										if (FileNow_FullName==sDirSep) {
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
										sFileList+=" \n"+FileNow_Name+" in "+FileNow_Directory_FullName;
										if (iCopyErrorEnder>0) sFileList+=" *"+sCopyErrorNow.Substring(0,iCopyErrorEnder);//intentionally iCopyErrorEnder>0 so that it is ignored if ender is at location zero
									}
									else {
										FileNow_Name="";
										FileNow_Directory_FullName="";
										if (FileNow_FullName!="") sFileList+=" \n"+FileNow_FullName;
										else sFileList+=" \n"+"(?)";
									}
									
									if (sFileList=="") sFileList="Copy Error while attempting to backup the following files: ";
									iActualNamesFound++;
								}
							}
						}
						if (sFileList=="") sFileList="Copy error: could not copy "+alCopyError.Count.ToString()+" file"+((alCopyError.Count!=1)?"s":"")+".";
						//MessageBox.Show(sFileList);
						DialogResult dlg=MessageBox.Show(sFileList+"\n\n  Do you wish to to review the list (press cancel to exit)?","Result", MessageBoxButtons.OKCancel);
						if (dlg==DialogResult.OK) bUserSaysStayOpen=true;
						else bUserSaysStayOpen=false;
					}
					else MessageBox.Show("Finished Backup");
					if ((bCopyErrorLastRun&&!bUserSaysStayOpen) || !bTestOnly && !bAlwaysStayOpen) Application.Exit(); // && !bCopyErrorLastRun
				}
				btnEditMain.Enabled=true;
				btnEditScript.Enabled=true;
			}
			else {
				MessageBox.Show(LimitedWidth("No destination drive is present.  Connect a flash drive, external hard drive, or other backup media and open the "+sMyName+" icon again.",40,"\n",true));
			}
		}//end BtnGoClick
		
		void MainFormLoad(object sender, EventArgs e)
		{
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

			FolderLister.bDebug=bTestOnly;
			Chunker.bDebug=bTestOnly;
			lbOutNow=this.lbOut;
			Console.Error.WriteLine("About to load " + SafeString(StartupFile_Name,true));
			RunScript(StartupFile_Name);
			this.lblProfile.Visible=true;
			Console.Error.WriteLine("Finished " + SafeString(StartupFile_Name,true)+" in MainFormLoad");
			bool bFoundLoadProfile=false;
			bool bSuccessFullyResetStartup=false;
			if (!bLoadedProfile) {
				Console.Error.WriteLine(SafeString(StartupFile_Name,true)+" did not load a profile so loading default (\""+DefaultProfile_Name+"\")");
				bool bTest=RunScriptLine("LoadProfile:"+DefaultProfile_Name);
				Console.Error.WriteLine("Loaded Profile \""+DefaultProfile_Name+"\"..."+(bTest?"OK":"FAILED!"));
				string sAllData="";
				try {
					string sMsg="Attempting to edit "+SafeString(StartupFile_Name,true)+" and add \"LoadProFile:"+DefaultProfile_Name+"\" if no valid LoadProfile statement is found...";
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
						ShowExn(exn,"reading "+SafeString(StartupFile_Name,true));
					}
					Console.Error.Write("writing data...");
					Console.Error.Flush();
					System.Threading.Thread.Sleep(100);//wait for file to be ready (is this ever needed???)
					StreamWriter streamOut=new StreamWriter(StartupFile_Name);
					streamOut.WriteLine(sAllData);
					streamOut.WriteLine("LoadProfile:"+DefaultProfile_Name);
					streamOut.Close();
					Console.Error.WriteLine("OK.");
					bSuccessFullyResetStartup=true;
				}
				catch (Exception exn) {
					ShowExn(exn,"creating "+SafeString(StartupFile_Name,true),"MainFormLoad");
				}
				System.Threading.Thread.Sleep(500);
				tbStatus.Text="Done checking startup {bLoadedProfile:"+(bLoadedProfile?"yes":"no")+"; bFoundLoadProfile:"+(bFoundLoadProfile?"yes":"no")+"; bSuccessFullyResetStartup:"+(bSuccessFullyResetStartup?"yes":"no")+"}.";
			}//end if !bLoadedProfile
			
			if (iLocations>0) {
				if (bExitIfNoUsableDrivesFound&&iValidDrivesFound==0 && !bAlwaysStayOpen) Application.Exit();
			}
			else if (!bTestOnly&&!bAlwaysStayOpen) Application.Exit();
			
			CalculateMargins();
			UpdateSize();
		}//end MainFormLoad
		public static string SafeString(string val, bool bShowValueAndQuoteIfNonNull) {
			return ( (val!=null) ? (bShowValueAndQuoteIfNonNull?"\""+val+"\"":"non-null") : "null" );
		}
		void CalculateMargins() {
			iLBRightMargin=lbOut.Left;//this.Width-(lbOut.Left+lbOut.Width);
			iLBBottomMargin=this.Height-(lbOut.Top+lbOut.Height);
		}
		
		void UpdateSize() {
			lbOut.Width=this.ClientSize.Width-lbOut.Left*2;//(iLBRightMargin+lbOut.Left);
			lbOut.Height=this.ClientSize.Height-lbOut.Top-tbStatus.Height-lbOut.Left;//(iLBBottomMargin+lbOut.Top);
			//btnGo.Left=(this.Width-btnGo.Width)/2;
			this.progressbarMain.Left=this.ClientRectangle.Left;
			this.progressbarMain.Width=this.ClientRectangle.Width;
			this.progressbarMain.Top=this.tbStatus.Top-this.progressbarMain.Height;
			labelTrivialStatus.Width=this.ClientSize.Width-labelTrivialStatus.Left;
			//this.lblDest.Width=this.ClientSize.Width;
		}

		void ComboDestTextChanged(object sender, EventArgs e)
		{
			//bool bFound=false;
			//foreach (string sNow in comboDest.Items) {
			//	if (comboDest.Text==sNow) bFound=true;
			//}
			//if (!bFound) comboDest.SelectedIndex=0;
			int FolderIndexNow=MainForm.FolderLocationIndex(comboDest.Text);
			bool bGB=locinfoarr[FolderIndexNow].AvailableFreeSpace/1024/1024/1024 > 0;
			if (FolderIndexNow>=0) {
				this.lblDestInfo.Text=locinfoarr[FolderIndexNow].VolumeLabel+" ("
					+ ((locinfoarr[FolderIndexNow].AvailableFreeSpace!=Int64.MaxValue)  ?  ( bGB ? (((decimal)locinfoarr[FolderIndexNow].AvailableFreeSpace/1024m/1024m/1024m).ToString("#")+"GB free"):(((decimal)locinfoarr[FolderIndexNow].AvailableFreeSpace/1024m/1024m).ToString("0.###")+"MB free") )  :  "unknown free"  );
			}
			else this.lblDestInfo.Text="";
		}
		
		void MainFormResize(object sender, EventArgs e)
		{
			UpdateSize();
		}
		
		void MainFormFormClosed(object sender, FormClosedEventArgs e)
		{
		}
		
		void BtnCancelClick(object sender, EventArgs e)
		{
			if (bBusyCopying) {
				btnCancel.Enabled=false;
				bUserCancelledLastRun=true;
			}
			else {
				if (flisterNow!=null&&flisterNow.IsBusy) flisterNow.Stop();
				btnCancel.Enabled=false;
				bUserCancelledLastRun=true;
			}
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
				int iMarker=sLine.IndexOf(":");
				if (iMarker>0 && sLine.Length>(iMarker+1)) {
					string sCommandLower=sLine.Substring(0,iMarker).ToLower();
					string sValue=sLine.Substring(iMarker+1);
					string sValueOrig=sValue;
					try {
						sValue=sValue.Replace("%MYDOCS%",Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)); //same as Personal
						sValue=sValue.Replace("%DESKTOP%",Environment.GetFolderPath(Environment.SpecialFolder.Desktop)); //The logical Desktop rather than the physical file system location DesktopDirectory
						sValue=sValue.Replace("%APPDATA%",Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
						sValue=sValue.Replace("%LOCALAPPDATA%",Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
					}
					catch (Exception exn) {
						ShowExn(exn,"parsing environment variables in command parameter","RunScriptLine");
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
						alInvalidDrives.Add(sValue);
						if (bTestOnly) Output("Not using "+sValue+" for backup");
					}
					else if (sCommandLower=="includedest") {
						alExtraDestinations.Add(sValue);
					}
					else if (sCommandLower=="addmask") {
						FolderLister.alMasks.Add(sValue);
						string sTemp="";
						foreach (string sMask in FolderLister.alMasks) {
							sTemp+=(sTemp==""?"":", ")+sMask;
						}
						if (bTestOnly) Output("#Masks changed: "+sTemp);
					}
					else if (sCommandLower=="removemask") {
						if (sValue=="*") FolderLister.alMasks.Clear();
						else FolderLister.alMasks.Remove(sValue);
						string sTemp="";
						foreach (string sMask in FolderLister.alMasks) {
							sTemp+=(sTemp==""?"":", ")+sMask;
						}
						if (bTestOnly) Output("#Masks changed: "+sTemp);
					}
					else if (sCommandLower=="exclude") {
						FolderLister.alExclusions.Add(sValue);
						string sTemp="";
						foreach (string sExclusion in FolderLister.alExclusions) {
							sTemp+=(sTemp==""?"":", ")+sExclusion;
						}
						if (bTestOnly) Output("#Exclusions changed: "+sTemp);
					}
					else if (sCommandLower=="include") {
						if (sValue=="*") FolderLister.alExclusions.Clear();
						else FolderLister.alExclusions.Remove(sValue);
						string sTemp="";
						foreach (string sExclusion in FolderLister.alExclusions) {
							sTemp+=(sTemp==""?"":", ")+sExclusion;
						}
						if (bTestOnly) Output("#Exclusions changed: "+sTemp);
					}
					else if (sCommandLower=="addfile") {
						if (sValue!=null&&sValue!="") {
							ArrayList alFiles=new ArrayList();
							int iWild=sValue.IndexOf(SlashWildSlash);
							if ((iWild>-1)&&(sValue.Length>iWild+3)) {
								Output("Listing folders for wildcard \""+sValue+"\":");
								DirectoryInfo diBranch=new DirectoryInfo(sValue.Substring(0,iWild+1));//+1 in case /*/ so that / will be used
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
									alFilesBackedUpManually.Add(ReconstructedBackupPath(fiSrc.DirectoryName)+sDirSep+fiSrc.Name);
									BackupFile(sFileTheoretical,true);
								}
								else {
									bCopyErrorLastRun=true;
									alCopyError.Add("File specified in configuration does not exist"+sCopyErrorFileFullNameOpener+sFileTheoretical+sCopyErrorFileFullNameCloser);
								}
							}//end foreach file (single file unless path has wildcard)
						}//end if sValue is not blank
					}//end if sCommandLower=="addfile"
					else if (sCommandLower=="addfolder") {
						int iSlashWildSlash=sValue.IndexOf(SlashWildSlash);
						if (iSlashWildSlash>-1) {
							ArrayList alFoldersTheoretical=new ArrayList();
							string BaseFolder_FullName=sValue.Substring(0,iSlashWildSlash);
							DirectoryInfo diBase=new DirectoryInfo(BaseFolder_FullName);
							string SpecifiedFolder_Name=sValue.Substring(iSlashWildSlash+SlashWildSlash.Length);
							if (diBase.Exists) {
								foreach (DirectoryInfo diNow in diBase.GetDirectories()) {
									string FolderTheoretical_FullName=diBase.FullName+sDirSep+diNow.Name+sDirSep+SpecifiedFolder_Name;
									if (FolderTheoretical_FullName.Contains(SlashWildSlash) //allow SlashWildSlash to allow recursive usage of SlashWildSlash
									    ||Directory.Exists(FolderTheoretical_FullName)) {
										alFoldersTheoretical.Add(FolderTheoretical_FullName);
									}
								}
								this.lbOut.Items.Add("Adding ("+alFoldersTheoretical.Count.ToString()+") folder(s) via wildcard "+SafeString(sValue,true)+"...");
								Application.DoEvents();
								foreach (string sFolderTheoretical in alFoldersTheoretical) {
									RunScriptLine("AddFolder:"+sFolderTheoretical);
								}
								this.lbOut.Items.Add("Done adding ("+alFoldersTheoretical.Count.ToString()+") folder(s) via wildcard.");
							}
							else {
								this.lbOut.Items.Add("ERROR: Folder does not exist ("+SafeString(BaseFolder_FullName,true)+")--cannot add specified subfolder(s) via wildcard.");
							}
						}
						else {//alFoldersTheoretical.Add(sValue);
						//foreach (string sFolderTheoretical in alFoldersTheoretical) {
							flisterNow.sSearchRoot=sValue;
							//string sDirSep=char.ToString(Path.DirectorySeparatorChar);
							Output("Loading \""+flisterNow.sSearchRoot+"\""+(flisterNow.MaskCount>0?(" (only "+flisterNow.MasksToCSV()+")"):"")+"...");
							//string sTempFile=Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)+"FolderList.tmp";
							//FolderLister.SetOutputFile(sTempFile);
							
							if (bRealTime) {
								btnCancel.Enabled=true;
								//sVerb="getting directory info";
								DirectoryInfo diRoot=new DirectoryInfo(flisterNow.sSearchRoot);
								iDepth=-1;
								bBusyCopying=true;
								if (diRoot.Exists) BackupFolder(diRoot);
								else {
									Output("Folder cannot be read: "+diRoot.FullName);
								}
							}
							else {
								flisterNow.StartRecordingLines();
								flisterNow.Start();
								btnCancel.Enabled=true;
								while (flisterNow.IsBusy) {
									Thread.Sleep(500);
									mainformNow.Refresh();
									Application.DoEvents();
								}
								bBusyCopying=true;
								Thread.Sleep(250);
								if (FolderLister.alSkipped!=null&&FolderLister.alSkipped.Count>0) {
									foreach (string sSkippedNow in FolderLister.alSkipped) {
										alSkipped.Add(sSkippedNow);
									}
								}
								string[] sarrListed=flisterNow.GetLines();
								ulByteCountFolderNow=flisterNow.ByteCount;
								ulByteCountTotal+=ulByteCountFolderNow;
								ulByteCountFolderNowDone=0;
								if (bTestOnly) Output("Getting ready to copy "+(ulByteCountFolderNow/1024/1024).ToString()+"MB...");
								//iListedLines=0;
								if (sarrListed!=null&&sarrListed.Length>0) {
									//if (File.Exists(sTempFile)) {
									//	StreamReader streamTemp=new StreamReader(sTempFile);
									//	string sListedItem;
									//	while ( (sListedItem=streamTemp.ReadLine()) != null ) {
									foreach (string sListedItem in sarrListed) {
										iListedLines++;
										FileAttributes fileattribNow = File.GetAttributes(sListedItem);
										//FileInfo fiNow=new FileInfo(sListedItem);
										//if (fiNow.Attributes&FileAttributes.Directory
										if ((fileattribNow & FileAttributes.Directory) == FileAttributes.Directory) {
											ReconstructPathOnBackup(sListedItem);
										}
										else {
											FileInfo fiX=new FileInfo(sListedItem);
											if (fiX.Exists) ReconstructPathOnBackup(fiX.DirectoryName);
											BackupFile(sListedItem,true);
										}
										if (bTestOnly) Output(sListedItem,true);
										if (bUserCancelledLastRun||bDiskFullLastRun) break; //do NOT stop if Copy Error only
									}
									//	}
									//	streamTemp.Close();
									//	File.Delete(sTempFile);
									//	Thread.Sleep(500);
								}
								else Output("Could not find any files in the added folder.");
							}//else !bRealTime
							bBusyCopying=false;
							btnCancel.Enabled=false;
						}
					}//end if sCommandLower==addfolder
					else if (sCommandLower=="loadprofile") {
						this.btnEditScript.Enabled=false;
						this.btnEditMain.Enabled=false;
						Console.Error.Write("LoadProfile...");
						Console.Error.Flush();
						BackupProfileFolder_FullName=".";
						string BackupProfileFolder_FullName_TEMP="."+sDirSep+"profiles"+sDirSep+sValue;
						Console.Error.Write("checking \"" + BackupProfileFolder_FullName_TEMP + "\"...");
						Console.Error.Flush();
						if (Directory.Exists(BackupProfileFolder_FullName_TEMP)) {
							Console.Error.Write("found...");
							Console.Error.Flush();
							DirectoryInfo diProfileX = new DirectoryInfo(BackupProfileFolder_FullName_TEMP);
							BackupProfileFolder_FullName = diProfileX.FullName;
							this.btnEditScript.Enabled=true;
							this.btnEditMain.Enabled=true;
							alInvalidDrives.Clear();
							alExtraDestinations.Clear();
							RunScript(BackupProfileFolder_FullName + sDirSep + MainFile_Name); //excludes and adds destinations
							if (File.Exists(BackupProfileFolder_FullName + sDirSep + LogFile_Name)) RunScript(BackupProfileFolder_FullName + sDirSep + LogFile_Name); //excludes and adds destinations
							bLoadedProfile=true;
	
							comboDest.BeginUpdate();
							comboDest.Items.Clear();
							//string[] sarrDrive=Environment.GetLogicalDrives();
							//foreach (string sDrivePathNow in sarrDrive) {
							//	if (ValidDest(sDrivePathNow)) {
							//		comboDest.Items.Add(sDrivePathNow);
							//		iValidDrivesFound++;
							//		iDestinations++;
							//	}
							//}
							iDestinations = 0;
							iLocations = 0;
							iValidDrivesFound = 0;
	
							DriveInfo[] driveinfoarrAbsolute = null;
							driveinfoarrAbsolute = Chunker.GetLogicalDrivesInfo();//DriveInfo.GetDrives();
							if (driveinfoarrAbsolute != null && driveinfoarrAbsolute.Length > 0) {
								driveinfoarrNow = new DriveInfo[driveinfoarrAbsolute.Length];
								for (int iAbsolute = 0; iAbsolute < driveinfoarrAbsolute.Length; iAbsolute++) {//foreach (DriveInfo driveinfoNow in DriveInfo.GetDrives()) {
									if (driveinfoarrAbsolute[iAbsolute] != null && driveinfoarrAbsolute[iAbsolute].IsReady) {
										if (ValidDest(driveinfoarrAbsolute[iAbsolute].RootDirectory.FullName)
											 && ((driveinfoarrAbsolute[iAbsolute].VolumeLabel == "") || (ValidDest(driveinfoarrAbsolute[iAbsolute].VolumeLabel))))
										{
											//comboDest.Items.Add(driveinfoarrAbsolute[iAbsolute].RootDirectory.FullName);
											//TODO: use AddLocation (?)
											driveinfoarrNow[iValidDrivesFound] = driveinfoarrAbsolute[iAbsolute];
											locinfoarr[iLocations] = new LocInfo();
											locinfoarr[iLocations].VolumeLabel = driveinfoarrAbsolute[iAbsolute].VolumeLabel;
											locinfoarr[iLocations].TotalSize = driveinfoarrAbsolute[iAbsolute].TotalSize;
											locinfoarr[iLocations].AvailableFreeSpace = driveinfoarrAbsolute[iAbsolute].AvailableFreeSpace;
											locinfoarr[iLocations].DriveRoot_FullNameThenSlash = LocalFolderThenSlash(driveinfoarrAbsolute[iAbsolute].RootDirectory.FullName);
											iLocations++;
											iValidDrivesFound++;
											iDestinations++;
										}
									}
								}
								if (iDestinations > 0) {
									DriveInfo[] driveinfoarrOld = driveinfoarrNow;
									driveinfoarrNow = new DriveInfo[iDestinations];
									int iOld = 0;
									for (int iNow = 0; iNow < iDestinations; iNow++)
									{
										driveinfoarrNow[iNow] = driveinfoarrOld[iOld];
										iOld++;
									}
								}
								else driveinfoarrNow = null;
							}//end if found any volumes on computer
	
							/* <http://codeidol.com/csharp/csharpckbk2/Filesystem-I-O/Querying-Information-for-All-Drives-on-a-System/> 2008-10-27
							foreach (DriveInfo drive in DriveInfo.GetDrives()) {
								if (drive.IsReady) {
									Console.WriteLine("Drive " + drive.Name + " is ready.");
									Console.WriteLine("AvailableFreeSpace: " + drive.AvailableFreeSpace);
									Console.WriteLine("DriveFormat: " + drive.DriveFormat);
									Console.WriteLine("DriveType: " + drive.DriveType);
									Console.WriteLine("Name: " + drive.Name);
									Console.WriteLine("RootDirectory.FullName: " +
											drive.RootDirectory.FullName);
									Console.WriteLine("TotalFreeSpace: " + drive.TotalFreeSpace);
									Console.WriteLine("TotalSize: " + drive.TotalSize);
									Console.WriteLine("VolumeLabel: " + drive.VolumeLabel);
								}
								else {
										Console.WriteLine("Drive " + drive.Name + " is not ready.");
								}
							}
							*/
							foreach (string sExtraDest in alExtraDestinations) {
								//comboDest.Items.Add(LocalFolderThenSlash(sExtraDest));
								AddLocation(sExtraDest);
								iDestinations++;
							}
							if (Directory.Exists("/Volumes")) {
								DirectoryInfo diMedia = new DirectoryInfo("/Volumes");
								foreach (DirectoryInfo diNow in diMedia.GetDirectories()) {
									if ((diNow.Attributes & FileAttributes.ReadOnly) != FileAttributes.ReadOnly) {
										if (ValidDest(diNow.FullName)) {
											AddLocation(diNow.FullName);
											iValidDrivesFound++;
										}
									}
								}
							}
							//comboDest.Items.Clear();//already done above
							for (int iNow = 0; iNow < iLocations; iNow++) {
								comboDest.Items.Add(LocalFolderThenSlash(locinfoarr[iNow].DriveRoot_FullNameThenSlash) + DestSuffixOnlyThenSlash);
							}
	
							comboDest.EndUpdate();
	
	
							//FolderLister.Echo("Test");
							string sMsg = "No backup drive can be found.  Try connecting the drive and then try again.";
							if (iLocations > 0) {//iValidDrivesFound+iDestinations>0) {
								if (bExitIfNoUsableDrivesFound && iValidDrivesFound == 0) {
									MessageBox.Show(sMsg);
								}
								comboDest.SelectedIndex = 0;
							}
							else {
								MessageBox.Show(sMsg);
							}
							this.lblProfile.Text="Profile: "+sValue;
						}//end if profile folder exists
						else {
							MessageBox.Show("Unable to open profile \"" + sValue + "\"!");
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
						DestSuffixOnlyThenSlash=LocalFolderThenSlash(sValue);
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
				ShowExn(exn,"parsing line "+SafeString(sLine,true)+" {iLine+1:"+(iLine+1).ToString()+"; aka--offset:"+iLine+"}");
				iCouldNotFinish++;
			}
			return bGood;
		}//end RunScriptLine
		
		void BtnEditScriptClick(object sender, EventArgs e)
		{
			try {
				DialogResult dlgresult=MessageBox.Show(LimitedWidth(ScriptFile_Name +" must be saved before returning to "+sMyName+" and pressing \"Go\" in order for any changes you make to take effect.",40,"\n",true), sMyName, MessageBoxButtons.OKCancel);
				if (dlgresult==DialogResult.OK) System.Diagnostics.Process.Start(BackupProfileFolder_FullName+sDirSep+ScriptFile_Name);
			}
			catch (Exception exn) {
				ShowExn(exn,"opening \""+ScriptFile_Name+"\" in profile","BtnEditScriptClick");
			}
		}
		public static bool IsNotBlank(string sNow) {
			return sNow!=null&&sNow.Length>0;
		}
		public static void ShowExn(Exception exn) {
			ShowExn(exn,"");
		}
		public static void ShowExn(Exception exn, string sParticiple) {
			ShowExn(exn,sParticiple,"");
		}
		public static void ShowExn(Exception exn, string sParticiple, string sMethodName) {
			Console.Error.WriteLine();
			string sMsg="Could not finish";
			if (IsNotBlank(sParticiple)) sMsg+=" "+sParticiple;
			if (IsNotBlank(sMethodName)) sMsg+=" in "+sMethodName;
			Output(sMsg,true);
			Console.Error.WriteLine(sMsg);
			if (exn!=null) Console.Error.WriteLine(exn.ToString());
			else Console.Error.WriteLine(" exn is null so no further information can be displayed");
		}
		public static bool Contains(char[] Haystack, char Needle) {
			bool bReturn=false;
			if (Haystack!=null) {
				for (int iTest=0; iTest<Haystack.Length; iTest++) {
					if (Needle==Haystack[iTest]) {
						bReturn=true;
						break;
					}
				}
			}
			return bReturn;
		}
		
		public static int MoveBackToOrStayAt(string Haystack, char Needle, int FindBeforeAndIncludingIndex) {
			int iFound=-1;
			if (Haystack!=null&&FindBeforeAndIncludingIndex>=0&&FindBeforeAndIncludingIndex<Haystack.Length) {
				for (int iChar=FindBeforeAndIncludingIndex; iChar>-1; iChar--) {
					if (Haystack[iChar]==Needle) {
						iFound=iChar;
						break;
					}
				}
			}
			return iFound;
		}//end MoveBackToOrStayAt
		public static int MoveBackToOrStayAtAny(string Haystack, char[] Needles, int FindBeforeAndIncludingIndex) {
			int iFound=-1;
			if (Haystack!=null&&FindBeforeAndIncludingIndex>=0&&FindBeforeAndIncludingIndex<Haystack.Length) {
				for (int iChar=FindBeforeAndIncludingIndex; iChar>-1; iChar--) {
					if (Contains(Needles,Haystack[iChar])) {//inentionally find haystack char in needles
						iFound=iChar;
						break;
					}
				}
			}
			return iFound;
		}//end MoveBackToOrStayAtAny
		public static string SafeSubstring(string AllText, int start) {
			string sReturn="";
			int length=0;
			int startOrig=start;
			try {
				if (AllText!=null) {
					if (start<0) {
						Console.Error.WriteLine("Warning: start is negative in SafeSubstring(AllText.Length="+AllText.Length.ToString()+",start="+startOrig.ToString()+")");
						start=0;
					}
					else if (start>=AllText.Length) {
						Console.Error.WriteLine("Warning: start is beyond range in SafeSubstring(AllText.Length="+AllText.Length.ToString()+",start="+startOrig.ToString()+")");
						start=AllText.Length; //ok since checked by SafeString primary overload
					}
					length=AllText.Length-start;
					if (start+length>AllText.Length) {
						Console.Error.WriteLine("Warning: SafeSubstring(AllText.Length="+AllText.Length.ToString()+",start="+startOrig.ToString()+")");
						length=AllText.Length-start;
					}
					sReturn=SafeSubstring(AllText,start,length);
				}
				else Console.Error.WriteLine("Warning: SafeSubstring(AllText=null,start="+startOrig.ToString()+")");
			}
			catch (Exception exn) {
				ShowExn(exn,"getting substring","SafeSubstring(AllText=null,start="+startOrig.ToString()+")");
			}
			return sReturn;
		}//end SafeSubstring
		public static string SafeSubstring(string AllText, int start, int length) {
			string sReturn="";
			int startOrig=start;
			int lengthOrig=length;
			try {
				if (AllText!=null) {
					if (start<0) {
						Console.Error.WriteLine("Warning: start is negative in SafeSubstring(AllText.Length="+AllText.Length.ToString()+",start="+startOrig.ToString()+",length="+lengthOrig.ToString()+")");
						start=0;
					}
					else if (start>=AllText.Length) {
						Console.Error.WriteLine("Warning: start is beyond range in SafeSubstring(AllText.Length="+AllText.Length.ToString()+",start="+startOrig.ToString()+",length="+lengthOrig.ToString()+")");
						start=AllText.Length; //ok since checked by final if clause
					}
					if (start+length>AllText.Length) {
						Console.Error.WriteLine("Warning: SafeSubstring(AllText.Length="+AllText.Length.ToString()+",start="+startOrig.ToString()+",length="+lengthOrig.ToString()+")");
						length=AllText.Length-start;
					}
					if (length>0) {
						sReturn=AllText.Substring(start,length);
					}
					else Console.Error.WriteLine("Warning: returning blank in SafeSubstring(AllText.Length="+AllText.Length.ToString()+",start="+startOrig.ToString()+",length="+lengthOrig.ToString()+")");
				}
				else Console.Error.WriteLine("Warning: null string sent to SafeSubstring(AllText=null,start="+startOrig.ToString()+",length="+lengthOrig.ToString()+")");
			}
			catch (Exception exn) {
				ShowExn(exn,"getting substring","SafeSubstring(AllText=null,start="+startOrig.ToString()+",length="+lengthOrig.ToString()+")");
			}
			return sReturn;
		}//end SafeSubstring
		/// <summary>
		/// Keeps width limited to LineWidth characters and returns the result with LineDelimiter added where needed
		/// </summary>
		/// <param name="AllText"></param>
		/// <param name="LineWidth"></param>
		/// <param name="LineDelimiter"></param>
		/// <returns></returns>
		public static string LimitedWidth(string AllText, int LineWidth, string LineDelimiter,bool KeepTrailingSpaceOnNextLine_FalseRemovesIt) { //Aka WrapText aka AutoWrap aka StringWrap
			string sReturn="";
			try {
				if (LineWidth<1) LineWidth=1;
				int iStartNow=0;
				string AllTextSparse=AllText;//debug only
				int iSparsenAt=AllText.Length;//debug only
				while (iSparsenAt>=0) {
					AllTextSparse=AllTextSparse.Insert(iSparsenAt," ["+iSparsenAt.ToString()+"]");
					if (iSparsenAt==AllText.Length) iSparsenAt=(((int)(iSparsenAt/10))*10)-10;
					else iSparsenAt-=10;
				}
				Console.Error.WriteLine();
				Console.Error.WriteLine();
				Console.Error.WriteLine(AllText);//debug only
				Console.Error.WriteLine(AllTextSparse);//debug only
				while (iStartNow<AllText.Length) {
					if (iStartNow+LineWidth>=AllText.Length) {
						Console.Error.WriteLine();
						sReturn+=((sReturn!="")?LineDelimiter:"") + SafeSubstring(AllText,iStartNow);
						Console.Error.WriteLine("SafeSubstring(AllText,"+iStartNow.ToString()+")");//debug only
						Console.Error.WriteLine(SafeSubstring(AllText,iStartNow));
						iStartNow=iStartNow+LineWidth;
						break;
					}
					else {
						int iPrevBreakBeforeAndDiscard=MoveBackToOrStayAt(AllText,' ',iStartNow+LineWidth);//intentionally uses index past end (endbfore) for start
						int iPrevBreakAfter=MoveBackToOrStayAtAny(AllText,carrBreakAfter,iStartNow+LineWidth-1);//intentionally uses last character (endbefore-1) for start
						if (iPrevBreakBeforeAndDiscard>-1||iPrevBreakAfter>-1) {
							if (iPrevBreakBeforeAndDiscard>iPrevBreakAfter) {
								Console.Error.WriteLine();
								sReturn+=((sReturn!="")?LineDelimiter:"") + SafeSubstring(AllText,iStartNow,iPrevBreakBeforeAndDiscard-iStartNow);
								Console.Error.WriteLine("SafeSubstring(AllText,"+iStartNow.ToString()+","+(iPrevBreakBeforeAndDiscard-iStartNow).ToString()+")",true);//debug only
								Console.Error.WriteLine(SafeSubstring(AllText,iStartNow,iPrevBreakBeforeAndDiscard-iStartNow));
								if (KeepTrailingSpaceOnNextLine_FalseRemovesIt) iStartNow=iPrevBreakBeforeAndDiscard;
								else iStartNow=iPrevBreakBeforeAndDiscard+1;//+1 discards the character e.g. ' '
							}
							else {
								Console.Error.WriteLine();
								sReturn+=((sReturn!="")?LineDelimiter:"") + SafeSubstring(AllText,iStartNow,iPrevBreakAfter+1-iStartNow);
								Console.Error.WriteLine("SafeSubstring(AllText,"+iStartNow.ToString()+","+(iPrevBreakAfter+1-iStartNow).ToString()+")",true);//debug only
								Console.Error.WriteLine(SafeSubstring(AllText,iStartNow,iPrevBreakAfter+1-iStartNow));
								iStartNow=iPrevBreakAfter+1;//+1 discards the character e.g. ' '
							}
						}
						else {//else no breaks, so force break at LineWidth
							Console.Error.WriteLine();
							sReturn+=((sReturn!="")?LineDelimiter:"") + SafeSubstring(AllText,iStartNow,iStartNow+LineWidth-iStartNow);
							Output("SafeSubstring(AllText,"+iStartNow.ToString()+","+(iStartNow+LineWidth-iStartNow).ToString()+")",true);//debug only
							Output(SafeSubstring(AllText,iStartNow,iStartNow+LineWidth-iStartNow),true);
							iStartNow=iStartNow+LineWidth;
						}
					}//end else more than one line of text remains
				}//end while iStartNow<AllText.Length
			}
			catch (Exception exn) {
				ShowExn(exn,"limiting width");
			}
			return sReturn;
		}
		void BtnEditMainClick(object sender, EventArgs e)
		{
			try {
				DialogResult dlgresult=MessageBox.Show(LimitedWidth("In order for any changes you make to take effect, you must close "+MainFile_Name+", close "+sMyName+" then open the "+sMyName+" icon again.",40,"\n",true), sMyName, MessageBoxButtons.OKCancel);
				if (dlgresult==DialogResult.OK) System.Diagnostics.Process.Start(BackupProfileFolder_FullName+sDirSep+MainFile_Name);
			}
			catch (Exception exn) {
				ShowExn(exn,"opening "+SafeString(MainFile_Name,true)+" for profile","BtnEditMainClick");
			}
		}
	}//end MainForm
}//end namespace
