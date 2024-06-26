/*
 * Created by SharpDevelop.
 * Author: Jake Gustafson
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
using System.Diagnostics; //StackTrace etc
using System.Threading;

namespace ExpertMultimedia {
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form {
		public static string sMyNameAndVersion="Backup GoNow (git)";
		public static string sMyName="Backup GoNow";
		//ArrayList alPseudoRootsNow=null;
		//ArrayList alSelectableDrives=null;
		//TODO: Option to remove files from the backup drive that aren't in the backup script
		private static string overlimit_content_name="tl";
		private static string overlimit_yml_name="tl.ssv";
		private bool profileCBSuspendEvents=false;
		public static bool is_first_overlimit=true;
		public static int iLine=0;
		public static readonly ArrayList nonDatedDotExts = new ArrayList(new string[] {
			".tc", // TrueCrypt volume (deprecated)
			".hc" // VeraCrypt volume
		});
		public static bool bFoundLoadProfile=false;
		public static bool bSuccessFullyResetStartup=false;
		public static int iListedLines=0;
		public static long preferenceValueBest_Value=long.MinValue;
		public static long preferenceValueBest_DriveIndex=-1;
		public static long preferenceValueBest_InitiallyChosen_Index=-1;
		public static int iCouldNotFinish=0;
		public static readonly string DefaultProfile_Name="BackupGoNowDefault";
		public static bool bLoadedProfile=false;
		public static int iMaxCopyErrorsToShow=10;
		public static bool bUserSaysStayOpen=false;
		public static string sCopyErrorFileFullNameCloser=" -- ";
		public static string sCopyErrorFileFullNameOpener=": ";
		public static bool bRemoveTrivialMessagesAfterScript=true;
		public static bool bOutputTrivial=false;
		public static bool bWriteBatchForFailedFiles=true;
		public static bool bDeleteDestDirsIfEmptyAndSourceIsNot=true;
		public static string RetryBatchFile_Name_DontTouchMe="retry-last.bat";
		public static string RetryBatchFile_FullName=null;
		public static Stack scriptFileNameStack = new Stack(); // Files that are running shouldn't be saved nor deleted!
		public static StreamWriter streamBatchRetry=null;
		//public static string sFileErrLog = "1.ErrLog.txt";//"1.Errors "+datetimeNow.ToString("yyyyMMddHHmm") + ".log";
		public static TextWriter errStream = null;
		public static string ProfileName = "BackupGoNowDefault";
		// ^ ProfileName gets changed when StartupFile_FullName is loaded.
		public static string BackupProfileFolder_FullName {
			get {
				if (ProfileName == null) {
					throw new ApplicationException("ProfileName is null.");
				}
				if (string.IsNullOrWhiteSpace(ProfileName)) {
					throw new ApplicationException("ProfileName \""+ProfileName+"\" is blank.");
				}
				if (ProfileName.Contains(char.ToString(Path.DirectorySeparatorChar))) {
					throw new ApplicationException("ProfileName is a path: "+ProfileName);
				}
				if (ProfileName.ToLower() == profilesFolder_Name.ToLower()) {
					throw new ApplicationException("ProfileName is "+profilesFolder_Name);
				}
				return Path.Combine(profilesFolder_FullName, ProfileName);
			}
		}
		public static bool bDeleteFilesNotOnSource_AfterCopyingEachFolder=true;
		public static bool bDeleteFilesNotOnSource_BeforeBackup=false;
		//public static bool bRealTime=true;
		public static System.Windows.Forms.TextBox tbStatusNow=null;
		public static int iDepth=0;
		public static int iFilesProcessed=0;
		public static bool bTestOnly=false;
		public static bool bAutoScroll=true;
		public static bool clear_buttons_enabled=false;
		public bool retroactiveAnswerIsStored=false;
		public bool retroactiveAnswer=false;
		public bool retroactiveUnmovableAnswerIsStored=false;
		public bool retroactiveUnmovableAnswer=false;
		public static string sAppName="Backup GoNow";
		public static string MyAppDataFolder_FullName=null;
		// public static string thisProfileFolder_FullName=null;
		public static readonly string profilesFolder_Name = "profiles";
		public static string profilesFolder_FullName {
			get {
				return Path.Combine(MyAppDataFolder_FullName, profilesFolder_Name);
			}
		}
		public static readonly string StartupFile_Name = "startup.ini";
		public static string StartupFile_FullName {
			get {
				return Path.Combine(MyAppDataFolder_FullName, StartupFile_Name);
			}
		}
		public static readonly string MainScriptFile_Name = "main.ini";
		public static string MainScriptFile_FullName {
			get {
				return Path.Combine(BackupProfileFolder_FullName, MainScriptFile_Name);
			}
		}
		public static readonly string BackupScriptFile_Name="script.txt";
		public static string BackupScriptFile_FullName {
			get {
				return Path.Combine(BackupProfileFolder_FullName, BackupScriptFile_Name);
			}
		}
		public static readonly string LogFile_Name = "summary.log";
		public static readonly string OutputFile_Name = "Backup GoNow output.txt";
		public static readonly string LastRunLogFile_Name_DontTouchMe="1.LastRun Output.txt";
		public static string LastRunLog_FullName {
			get {
				return Path.Combine(MyAppDataFolder_FullName, LastRunLogFile_Name_DontTouchMe);
			}
		}
		public static string OutputFile_FullName {
			get {
				return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), OutputFile_Name);
			}
		}
		public static MainForm mainformNow=null;
		public static ListBox lbOutNow=null;
		public static ulong ulByteCountTotalProcessed_LastRun=0;
		public static int iLBRightMargin=0;
		public static int iLBBottomMargin=0;
		public static int iTickLastRefresh=Environment.TickCount;
		public static int iTicksRefreshInterval=2000;
		public static int optionColumnIndex_Command=0;
		public static int optionColumnIndex_Value=1;
		public static int optionColumnIndex_DeleteButton=2;
		public static bool useLastDirectoryDREnable=false;
		public static bool useLastFileDREnable=false;
		public static DialogResult lastDirectoryDR=DialogResult.None;
		public static DialogResult lastFileDR=DialogResult.None;
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
		private static string sShowError="";
		private static int iNonCommentLines=0;
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
		private static string DestinationDriveRootDirectory_FullName_OrSlashIfRootDir="";
		private static string DestSubfolderRelNameThenSlash=null;
		
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
		
		/// <summary>
		/// Format milliseconds as a seconds string.
		/// </summary>
		/// <param name="ms">Milliseconds</param>
		/// <returns>Seconds string, rounded to 3 places.</returns>
		public static string msss(long ms) {
			return (Math.Round((decimal)ms/1000, 3)).ToString();
		}
		void LogWriteLine(string line) {
			//NOTE: this.lbOut.Items.Add gets saved to last run log later.
			this.lbOut.Items.Add(line);
			Console.Error.WriteLine();
		}
		bool SaveOutputToTextFile() {
			bool bGood=false;
			StreamWriter outStream=null;
			try {
				outStream=new StreamWriter(OutputFile_FullName);
				DateTime dtNow=DateTime.Now;
				outStream.WriteLine("# "+dtNow.Year.ToString()+"-"+dtNow.Month+"-"+dtNow.Day+" "+dtNow.Hour+":"+dtNow.Minute+":"+dtNow.Second);
				for (int i=0; i<this.lbOut.Items.Count; i++) {
					outStream.WriteLine(this.lbOut.Items[i].ToString());
				}
				bGood=true;
				outStream.Close();
			}
			catch (Exception exn) {
				bGood=false;
			}
			return bGood;
		}//end SaveOutputToTextFile
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sourceFileName"></param>
		/// <param name="destFileName"></param>
		/// <param name="IsEmptyLineAllowed"></param>
		/// <returns>If ok, returns null. If error, returns error message.</returns>
		public static void CopyFileWithoutComments(string sourceFileName, string destFileName, bool IsEmptyLineAllowed) {
			StreamReader inStream=null;
			StreamWriter outStream=null;
			string msg=null;
			if (sourceFileName.ToLower()!=destFileName.ToLower()) {
				try {
					inStream=new StreamReader(sourceFileName);
					outStream=new StreamWriter(destFileName);
					string line=null;
					while ( (line=inStream.ReadLine()) != null ) {
						line=line.Trim();
						if ( (IsEmptyLineAllowed||!string.IsNullOrEmpty(line)) && !line.StartsWith("#") ) {
							outStream.WriteLine(line);
						}
					}
					outStream.Close();
					outStream=null;
					inStream.Close();
					inStream=null;
				}
				catch (Exception exn) {
					string sourceFileName_Quoted="";
					string destFileName_Quoted="";
					if (sourceFileName==null) sourceFileName_Quoted="null";
					else sourceFileName_Quoted="\""+sourceFileName+"\"";
					if (destFileName==null) destFileName_Quoted="null";
					else destFileName_Quoted="\""+destFileName+"\"";
					msg="Could not finish CopyFileWithoutComments("+sourceFileName_Quoted+","+destFileName_Quoted+"):"+exn.ToString();
					Console.Error.WriteLine(msg);
					if (outStream!=null) {
						try {outStream.Close(); outStream=null;}
						catch {} //don't care
					}
					if (inStream!=null) {
						try {inStream.Close(); inStream=null;}
						catch {} //don't care
					}
				}
			}
			else {
				string sourceFileName_Quoted="";
				string destFileName_Quoted="";
				if (sourceFileName==null) sourceFileName_Quoted="null";
				else sourceFileName_Quoted="\""+sourceFileName+"\"";
				if (destFileName==null) destFileName_Quoted="null";
				else destFileName_Quoted="\""+destFileName+"\"";
				msg="skipped CopyFileWithoutComments("+sourceFileName_Quoted+","+destFileName_Quoted+") since source and destination are the same";
				Console.Error.WriteLine(msg);
			}
		}//end CopyFileWithoutComments

		public static string sWasUpToDate="Was Up to Date";
		public static readonly string successfully_redirected_string="successfully redirected";
		public static Brush brushItemOther = Brushes.Black;
		public static SolidBrush brushItemWasUpToDate = new SolidBrush(Color.FromArgb(192, 192, 192)); //Brushes.Gray;
		//public static int iDriveDest=0;
		//public static string sDestRootThenSlash="";
		public static bool bStartedCopyingAnyFiles=false;
		//private static string sDestPathSlash {
		//	get { return sDestRootThenSlash+((DestSubfolderRelNameThenSlash!=null)?DestSubfolderRelNameThenSlash:null); }
		//}
		public MainForm() {
			MyAppDataFolder_FullName=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),sMyName);
			RetryBatchFile_FullName=Path.Combine(MyAppDataFolder_FullName,RetryBatchFile_Name_DontTouchMe);
			
			// thisProfileFolder_FullName=Path.Combine(profilesFolder_FullName,"BackupGoNowDefault");
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
			
			if (File.Exists(RetryBatchFile_FullName)) {
				try {
					Common.sParticiple="generating retry batch \".old\" backup filename";
					string sOldBat=RetryBatchFile_FullName+".old";
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
					File.Move(RetryBatchFile_FullName,sOldBat);
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
					streamBatchRetry=File.AppendText(RetryBatchFile_FullName);
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
			return sNow.ToLower()=="yes"||sNow=="1"||sNow.ToLower()=="true"||sNow.ToLower()=="on";
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="diBase"></param>
		/// <param name="enableRecreateFullPath"></param>
		/// <param name="baseSourcePath">If enableRecreateFullPath is false, baseSourcePath will be excluded from the path on the backup</param>
		/// <returns></returns>
		string BackupFolder(DirectoryInfo diBase, bool enableRecreateFullPath, string baseSourcePath) {
			string result = null;
			iDepth++;
			DirectoryInfo[] diarrSrc=null;
			try {
				diarrSrc=diBase.GetDirectories();
				// if (bTestOnly) Output("Getting ready to copy "+(diBase.Size/1024/1024).ToString()+"MB...");			
				foreach (DirectoryInfo diNow in diarrSrc) {
					if (bUserCancelledLastRun) break;
					if (!Common.IsExcludedFolder(diNow)) {  // TODO: if (!Common.IsExcludedFolder(diNow, true, true, false)) {
						ReconstructPathOnBackup(diNow.FullName, enableRecreateFullPath, baseSourcePath);
						if (!bUserCancelledLastRun&&!bDiskFullLastRun
						    	&&!Common.IsExcludedFolder(diNow)) { // TODO: &&!Common.IsExcludedFolder(diNow, true, true, false))  //&&flisterNow.UseFolder(diNow))
							string sNewPath = BackupFolder(diNow, enableRecreateFullPath, baseSourcePath);
							// ignore return--not needed for feedback (during recursion)
						}
					}
				}
			}
			catch {}  // no subfolders
			
			FileInfo[] fiarrSrc=null;
			
			// bool[] barrUsedSrcFile=null;
			// ArrayList alActuallyUsedSrcFiles=null;//new ArrayList();
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
					if (!Common.IsExcludedFile(fiNow)) {//if (!Common.IsExcludedFile(diBase,fiNow)) {//if (flisterNow.UseFile(diBase,fiNow)) {
						//barrUsedSrcFile[iSrcNow]=true;
						//lbOut.Items.Add(fiNow.FullName+" not excluded by "+Common.MasksToCSV());//debug only
						BackupFile(fiNow.FullName, enableRecreateFullPath, baseSourcePath, null);
						if (bTestOnly) Output("  ("+fiNow.FullName+")",true);
						//if (bDeleteFilesNotOnSource_AfterCopyingEachFolder) alActuallyUsedSrcFiles.Add(fiNow.Name);
						//iSrcNow++;
					}
					if (bUserCancelledLastRun) break;
				}//end foreach file
				DirectoryInfo diTarget=new DirectoryInfo(ReconstructedBackupPath(diBase.FullName,enableRecreateFullPath, baseSourcePath, null));
				result = diTarget.FullName;
				if (bDeleteFilesNotOnSource_AfterCopyingEachFolder&&!bSourceListingWasCancelled) {
					//DirectoryInfo diTarget=new DirectoryInfo(ReconstructedBackupPath(diBase.FullName));
					//bool bFoundOnSource=false;
					DirectoryInfo found_source_DI=null;
					foreach (DirectoryInfo diDest in diTarget.GetDirectories()) {
						//bFoundOnSource=false;
						found_source_DI=null;
						if (bUserCancelledLastRun) break;
						foreach (DirectoryInfo diSource in diarrSrc) {
							if (bUserCancelledLastRun) break;
							if (diDest.Name==diSource.Name) {
								//bFoundOnSource=true;
								found_source_DI=diSource;
								break;
							}
						}
						if (found_source_DI==null) {
							string DeletedFolder_FullName=diDest.FullName;
							//string retroactive_timed_name=diDest.LastWriteTimeUtc.ToString("yyyyMMddHHmmss", System.Globalization.CultureInfo.GetCultureInfo("en-US"));
							string diDest_Retroactive_Parent_FullName = ReconstructedBackupPath(ReconstructedSourcePath(diDest.Parent.FullName),enableRecreateFullPath, baseSourcePath, get_retroactive_timed_folder_partialpath_from_UTC(diDest.LastWriteTimeUtc));
							string diDest_Retroactive_FullName = Path.Combine(diDest_Retroactive_Parent_FullName, diDest.Name);
							MainForm.Output("Removing deleted/moved folder to retroactive folder: "+diDest_Retroactive_FullName,true);
							Application.DoEvents();
							DialogResult thisDR=DialogResult.None;
							if (!useLastDirectoryDREnable || (lastDirectoryDR==DialogResult.None)) {
								string this_msg="A folder was deleted after the last backup:\n "+diDest.FullName+"\n\n Do you want to move the backup to a retroactive backup\n "+diDest_Retroactive_FullName+"\n (press No to Delete; YES is recommended)";
								if (useLastDirectoryDREnable) this_msg+=" (your answer will be remembered for this session)";
								this_msg+="?";
								thisDR=MessageBox.Show(this_msg,"Backup GoNow",MessageBoxButtons.YesNoCancel);
								lastDirectoryDR=thisDR;
							}
							else thisDR=lastDirectoryDR;
							if (thisDR==DialogResult.Yes) {
								try {
									Directory.CreateDirectory(diDest_Retroactive_Parent_FullName);
									Directory.Move(diDest.FullName,diDest_Retroactive_FullName);
								}
								catch (Exception exn) {
									WriteLastRunLog("Could not finish making folder \""+diDest.FullName+"\" retroactive as \""+diDest_Retroactive_FullName+"\""+exn.ToString());
								}
								
							}
							else if (thisDR==DialogResult.No) {
								DeleteFolderRecursively(diDest,true);//use my method so that lByteCountTotalActuallyAdded is decremented//diDest.Delete(true);
							}
							else {
								if (!bUserCancelledLastRun) {
									//if (flisterNow!=null&&flisterNow.IsBusy) flisterNow.Stop();
									cancelButton.Enabled=false;
									bUserCancelledLastRun=true;
								}
							}
							//else leave it on the backup
						}
					}
					FileInfo found_source_FI=null;
					foreach (FileInfo fiDest in diTarget.GetFiles()) {
						found_source_DI=null;//bFoundOnSource=false;
						//iSrcNow=0;
						if (bUserCancelledLastRun) break;
						foreach (FileInfo fiSource in fiarrSrc) {
							if (bUserCancelledLastRun) break;
							if (fiDest.Name==fiSource.Name) {
								found_source_FI=fiSource;//bFoundOnSource=true;
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
						if (found_source_FI==null) {
							Output("Removing deleted/moved file from backup: \""+fiDest.FullName+"\"");
							fiDest.Attributes=FileAttributes.Normal;//fiDest.Attributes^= FileAttributes.ReadOnly;
							string DeletedFile_FullName=fiDest.FullName;
							//string retroactive_timed_name=fiDest.LastWriteTimeUtc.ToString("yyyy"+Common.sDirSep+"MM"+Common.sDirSep+"dd"+Common.sDirSep+"HHmmss", System.Globalization.CultureInfo.GetCultureInfo("en-US"));
							string fiDest_Parent_FullName=ReconstructedBackupPath(ReconstructedSourcePath(fiDest.Directory.FullName), enableRecreateFullPath, baseSourcePath, get_retroactive_timed_folder_partialpath_from_UTC(fiDest.LastWriteTimeUtc));
							string fiDest_Retroactive_FullName = Path.Combine( fiDest_Parent_FullName, fiDest.Name );
							try {
								Directory.CreateDirectory(fiDest_Parent_FullName);
								fiDest.MoveTo(fiDest_Retroactive_FullName);
							}
							catch (Exception exn) {
								string ask = "This file:\n "+fiDest.FullName+"\n could not be moved to retroactive file \n"+fiDest_Retroactive_FullName+"\n Do you want to keep it anyway (Yes is recommended)?";
								DialogResult thisDR = this.retroactiveUnmovableAnswer ? DialogResult.Yes : DialogResult.No;
								if (!this.retroactiveUnmovableAnswerIsStored) {
									// DialogResult thisDR=MessageBox.Show(ask, "Backup GoNow", MessageBoxButtons.YesNo);
									RetroactiveAskForm askForm = new RetroactiveAskForm();
									askForm.TopLevel = true;
									// askForm.Parent = this;
									// DialogResult thisDR = askForm.ShowDialog(this);
									thisDR = askForm.ShowDialog(this, ask);
									askForm.BringToFront();
									this.retroactiveUnmovableAnswerIsStored = askForm.remember;
								}
								if (thisDR == DialogResult.No) {
									this.retroactiveUnmovableAnswer = false;
									fiDest.Delete();
								}
								else if (thisDR == DialogResult.Yes) {
									this.retroactiveUnmovableAnswer = true;
								}
								WriteLastRunLog("Could not finish making file \""+fiDest.FullName+"\" retroactive as \""+fiDest_Retroactive_FullName+"\""+exn.ToString());
							}
							lByteCountTotalActuallyAdded-=fiDest.Length;
						}
						if (bUserCancelledLastRun||bDiskFullLastRun) break; //note: do NOT stop if Copy Error only
					}//end for each file to destination (to check if should be deleted)
				}//end if delete files not on source
				if (bDeleteDestDirsIfEmptyAndSourceIsNot) {
					Console.Error.Write("checking whether source \""+diBase.Name+"\" empty...");
					Console.Error.Flush();
					int iSrcFiles=(fiarrSrc!=null)?fiarrSrc.Length:0;
					int iSrcDirs=(diarrSrc!=null)?diarrSrc.Length:0;
					if (iSrcFiles+iSrcDirs>0) {
						Console.Error.Write("checking whether dest \""+diTarget.Name+"\" empty...");
						Console.Error.Flush();
						DirectoryInfo[] diarrTemp=diTarget.GetDirectories();
						FileInfo[] fiarrTemp=diTarget.GetFiles();
						int iFilesAfter=(fiarrTemp!=null)?fiarrTemp.Length:0;
						int iDirsAfter=(diarrTemp!=null)?diarrTemp.Length:0;
						if (iFilesAfter+iDirsAfter<=0) {
							Console.Error.Write("deleting empty...");
							Console.Error.Flush();
							diTarget.Delete();
							Console.Error.WriteLine("OK.");
						}
						else Console.Error.WriteLine("not empty.");
					}
					else {//keep dest even if empty since source exists though empty (empty folder would seem to be important in that case)
						Console.Error.WriteLine("not empty.");
					}
				}
			}
			catch {
				Console.Error.WriteLine("FAIL (BackupFolder recursively)");
			} //no files
			iDepth--;
			return result;
		}//end BackupFolder recursively
		string get_retroactive_timed_folder_partialpath_from_UTC(DateTime thisDT_MUST_BE_UTC) {
			//NOTE: must use DOUBLE dirsep, since escape sequences are allowed in datetime.ToString
			bool localize_enable = false;
			if (localize_enable)
				return Common.sDirSep + thisDT_MUST_BE_UTC.ToString("yyyy",System.Globalization.CultureInfo.GetCultureInfo("en-US"))
					+ Common.sDirSep + thisDT_MUST_BE_UTC.ToString("MM",System.Globalization.CultureInfo.GetCultureInfo("en-US"))
					+ Common.sDirSep + thisDT_MUST_BE_UTC.ToString("dd",System.Globalization.CultureInfo.GetCultureInfo("en-US"))
					+ Common.sDirSep + thisDT_MUST_BE_UTC.ToString("HH",System.Globalization.CultureInfo.GetCultureInfo("en-US"))
					+ thisDT_MUST_BE_UTC.ToString("mm",System.Globalization.CultureInfo.GetCultureInfo("en-US"))
					+ thisDT_MUST_BE_UTC.ToString("ss",System.Globalization.CultureInfo.GetCultureInfo("en-US"))
					+ Common.sDirSep;
			return Common.sDirSep + thisDT_MUST_BE_UTC.ToString("yyyy")
				+ Common.sDirSep + thisDT_MUST_BE_UTC.ToString("MM")
				+ Common.sDirSep + thisDT_MUST_BE_UTC.ToString("dd")
				+ Common.sDirSep + thisDT_MUST_BE_UTC.ToString("HH")
				+ thisDT_MUST_BE_UTC.ToString("mm")
				+ thisDT_MUST_BE_UTC.ToString("ss")
				+ Common.sDirSep;
			
		}
		bool RunScript(string sFileX, bool enableRecreateFullPath, long timeoutMS) {
			System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
			watch.Start();
			sShowError="";
			scriptFileNameStack.Push(sFileX);
			if (watch.ElapsedMilliseconds > timeoutMS) {
				Debug.WriteLine("Warning: " + msss(watch.ElapsedMilliseconds)
				                + "s passed before clearing errors " + sFileX);
			}
			
			if (alSkippedDueToException!=null||alCopyError!=null) { //TODO: recheck logic.  This used to be done below (see identical commented lines)
				if (alSkippedDueToException.Count!=0||alCopyError.Count>0) {
					Output("Clearing error cache...",true);
				}
				alSkippedDueToException.Clear();
				alCopyError.Clear();
			}
			
			iNonCommentLines=0;
			int iFilesProcessedPrev=iFilesProcessed;
			int iFilesProcessed_ThisScript=0;
			bool bGood=false;
			StreamReader streamIn=null;
			iCouldNotFinish=0;
			if (watch.ElapsedMilliseconds > timeoutMS) {
				Debug.WriteLine("Warning: " + msss(watch.ElapsedMilliseconds)
				                + "s passed before clearing skip list before " + sFileX);
			}
			try {
				if (alSkippedDueToException!=null) alSkippedDueToException.Clear();
				else alSkippedDueToException=new ArrayList();
				
				if (!File.Exists(sFileX)) {
					Console.Error.WriteLine("File does not exist: \"" + sFileX + "\"!");
				}
				else {
					Console.Error.WriteLine("Reading \"" + sFileX + "\":");
				}
				if (watch.ElapsedMilliseconds > timeoutMS) {
					Debug.WriteLine("Warning: " + msss(watch.ElapsedMilliseconds)
					                + "s passed before opening" + sFileX);
				}

				streamIn=new StreamReader(sFileX);
				string sLine;
				//flisterNow=new FolderLister();
				//flisterNow.MinimumFileSize=1;//1byte (trying to avoid bad symlinks here)
				//flisterNow.bShowFolders=true;
				iLine=0;
				iListedLines=0;

				if (watch.ElapsedMilliseconds > timeoutMS) {
					Debug.WriteLine("Warning: " + msss(watch.ElapsedMilliseconds)
					                + "s passed before reading" + sFileX);
				}
				
				while ( (sLine=streamIn.ReadLine()) != null ) {
					if (bUserCancelledLastRun) {
						bUserCancelledLastRun=true;
						break;
					}
					if (sLine.StartsWith("#")) Console.Error.WriteLine("\t"+sLine);
					else iNonCommentLines++;
					RunScriptLine(sLine, enableRecreateFullPath, sFileX, iLine+1, timeoutMS);
					// NOTE: timeoutMS is used as a *per-line* timeout above with its own Stopwatch.
					iLine++;
					Debug.WriteLine(sFileX+", line "+iLine.ToString()+": "+sLine);
					if (watch.ElapsedMilliseconds > timeoutMS) {
						Debug.WriteLine("Warning: " + msss(watch.ElapsedMilliseconds)
						                + "s for line " + iLine.ToString() + " of " + sFileX + ": "+sLine);
					}
					
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
				if (streamIn!=null) {
					streamIn.Close();
					streamIn=null;
				}
				string script_name="missing script";
				FileInfo fiScript=new FileInfo(sFileX);
				if (fiScript.Exists) {
					script_name=fiScript.Name;
				}
				string lastGoodScriptName=Path.GetFileNameWithoutExtension(sFileX)+"-LastGood"+Path.GetExtension(sFileX);
				string lastGoodScriptPath=Path.Combine(fiScript.Directory.FullName, lastGoodScriptName);
				if (iNonCommentLines<1) {
					Output("");
					string msg="ERROR: There were no commands in "+script_name+".";
					//NOTE: GetExtension's return DOES include dot if there is any
					
					string badScriptPath=Path.Combine(fiScript.Directory.FullName, Path.GetFileNameWithoutExtension(sFileX)+"-LastBad"+Path.GetExtension(sFileX));
					if (File.Exists(lastGoodScriptPath)) {
						if (File.Exists(badScriptPath)) File.Delete(badScriptPath);
						File.Move(sFileX, badScriptPath);
						File.Copy(lastGoodScriptPath, sFileX);
						msg+=" (last good script was loaded, try again)";
					}
					else {
						msg+=" and there was no \n\""+lastGoodScriptName+"\" file";
					}
					Output(msg,true);
					bCopyErrorLastRun=true;
					if (alCopyError==null) alCopyError=new ArrayList();
					alCopyError.Add(msg);
					sShowError=msg;
				}
				else {
					//backup even if there were any commands in script even if there were copy errors,
					//since script is apparently intact as user intended even if wrong
					//(copy errors will happen even in the case of permission issues or full dest anyway
					//so they do not normally indicate that the script is bad unless if case above is true)
					File.Copy(sFileX, lastGoodScriptPath, true);
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
				Console.Error.WriteLine();  // in case RunScriptLine failed after a Write
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
				throw exn;
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
			scriptFileNameStack.Pop();
			watch.Stop();
			if (watch.ElapsedMilliseconds > timeoutMS) {
				Debug.WriteLine("Warning: "+msss(watch.ElapsedMilliseconds)+"s for "+sFileX);
			}
			else {
				Debug.WriteLine("Info: "+msss(watch.ElapsedMilliseconds)+"s for "+sFileX);
			}
			return bGood;
		}//end RunScript
		public static string ToString(Stack thisStringStack, string delimiter) {
			string resultString="";
			//string[] thisStringArray = (string[])thisStringStack.ToArray();
			int index=0;
			foreach (string thisString in thisStringStack) {
				resultString+=((index==0)?(""):(delimiter))+"\""+thisString+"\"";
				index++;
			}
//			if (thisStringArray!=null) {
//				for (index=0; index<thisStringArray.Length; index++) {
//					string thisString=thisStringArray[index];
//					resultString+=((index==0)?(""):(delimiter))+"\""+thisString+"\"";
//				}
//			}
			return resultString;
		}
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
		
//		public bool SetDestFolder(string FolderNow) {
//			bool bGood=false;
//			int DestFolderIndexNow=-1;
//			Common.sParticiple="initializing";
//			//locinfoarrPseudoRoot[DestFolderIndexNow]
//			LocInfo locinfoPseudoRootNow=null;
//			try {
//				if (FolderNow!=null&&FolderNow!="") {
//					Common.sParticiple="removing trailing '"+Common.sDirSep+"'";
//					FolderNow=Common.LocalFolderThenNoSlash(FolderNow);
//					Common.sParticiple="checking driveinfoarrSelectableDrive";
//					DestFolderIndexNow=Common.InternalIndexOfPseudoRoot_WhereIsOrIsParentOf_FolderFullName(FolderNow,false);
//					locinfoPseudoRootNow=(DestFolderIndexNow>=0)?Common.GetPseudoRoot(DestFolderIndexNow):null;
//					if (DestFolderIndexNow<0) {
//						Common.AddFolderToPseudoRoots(FolderNow);
//						Output("Adding location \""+FolderNow+"\"");
//						DestFolderIndexNow=Common.InternalIndexOfPseudoRoot_WhereIsOrIsParentOf_FolderFullName(FolderNow,false);
//						Output("  at index "+DestFolderIndexNow);
//					}
//					else {
//						Output("Found location \""+FolderNow+"\"");
//						Output("  at index "+DestFolderIndexNow+" ("+locinfoPseudoRootNow.DriveRoot_FullNameThenSlash+")");
//					}
//					if (DestFolderIndexNow>-1) {
//						bGood=true;
//						DestinationDriveRootDirectory_FullName_OrSlashIfRootDir=locinfoPseudoRootNow.DriveRoot_FullNameThenSlash;
//						if (DestinationDriveRootDirectory_FullName_OrSlashIfRootDir!=Common.sDirSep&&DestinationDriveRootDirectory_FullName_OrSlashIfRootDir.EndsWith(Common.sDirSep)) DestinationDriveRootDirectory_FullName_OrSlashIfRootDir=DestinationDriveRootDirectory_FullName_OrSlashIfRootDir.Substring(0,DestinationDriveRootDirectory_FullName_OrSlashIfRootDir.Length-Common.sDirSep.Length);
//					}
//					else {
//						Console.Error.WriteLine("SetDestFolder: could not set folder to \""+FolderNow+"\"");
//						Console.Error.WriteLine();
//					}
//				}
//			}
//			catch (Exception exn) {
//				ulByteCountDestTotalSize=0;
//				ulByteCountDestAvailableFreeSpace=0;
//				Common.ShowExn(exn,Common.sParticiple+String.Format(" {{driveinfoarrSelectableDrive{0}; DestFolderIndexNow:{1}}}",Common.GetSelectableDriveArrayMsg_LengthColonCount_else_ColonNull(),DestFolderIndexNow ),"SetDestFolder");
//			}
//			try {
//				if (DestFolderIndexNow>-1) {
//					ulByteCountDestTotalSize=(long)locinfoPseudoRootNow.TotalSize;
//					ulByteCountDestAvailableFreeSpace=(long)locinfoPseudoRootNow.AvailableFreeSpace; //TotalFreeSpace doesn't count user quotas
//					//Console.WriteLine( "{0}MB free {1}MB total ({2}bytes/{3}bytes) on {4} ({5})",
//					//		(ulByteCountDestTotalSize/1024/1024), (ulByteCountDestAvailableFreeSpace/1024/1024), ulByteCountDestTotalSize, ulByteCountDestAvailableFreeSpace, locinfoarrPseudoRoot[DestFolderIndexNow].VolumeLabel, locinfoarrPseudoRoot[DestFolderIndexNow].DriveFormat );
//				}
//				else {
//					ulByteCountDestTotalSize=Int64.MaxValue;
//					ulByteCountDestAvailableFreeSpace=Int64.MaxValue;
//					Console.WriteLine( "{0}MB free {1}MB total ({2}bytes/{3}bytes)",//debug only
//								(ulByteCountDestTotalSize/1024/1024), (ulByteCountDestAvailableFreeSpace/1024/1024), ulByteCountDestTotalSize, ulByteCountDestAvailableFreeSpace );
//					
//				}
//			}
//			catch (Exception exn) {
//				Common.ShowExn(exn,"accessing locinfoarrPseudoRoot["+DestFolderIndexNow.ToString()+"]:");
//				ulByteCountDestTotalSize=0;
//				ulByteCountDestAvailableFreeSpace=0;
//			}
//			Console.WriteLine("SetDestFolder:"+(FolderNow!=null?"\""+FolderNow+"\"":"null")+"(DestFolderIndexNow:"+DestFolderIndexNow.ToString()+")");//debug only
//			return bGood;
//		}//end SetDestFolder
		/// <summary>
		/// Makes sure that:
		/// -sDestRootThenSlash ends with slash (i.e. sDestRootThenSlash=Common.LocalFolderThenSlash(sDestRootThenSlash) )
		/// -DestSubfolderRelNameThenSlash does NOT start with slash, and DOES end with slash
		/// </summary>
		/*
		public bool FixSlashes() {
			bool bGood=false;
			SetDestFolder(this.destinationComboBox.Text);
			//sDestRootThenSlash=DestinationDriveRootDirectory_FullName_OrSlashIfRootDir;//GetDestDriveRoot();
			if (sDestRootThenSlash!="") {
				bGood=true;
				if (!sDestRootThenSlash.EndsWith(Common.sDirSep)) sDestRootThenSlash+=Common.sDirSep;
			}
			else bGood=false;
			if (DestSubfolderRelNameThenSlash!="" && DestSubfolderRelNameThenSlash!=null) {
				while (DestSubfolderRelNameThenSlash.StartsWith(Common.sDirSep)) DestSubfolderRelNameThenSlash=(DestSubfolderRelNameThenSlash.Length>1)?DestSubfolderRelNameThenSlash.Substring(1):"";
				if (!DestSubfolderRelNameThenSlash.EndsWith(Common.sDirSep)) DestSubfolderRelNameThenSlash+=Common.sDirSep;
			}
			return bGood;
		}//end FixSlashes
		*/
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sBackupPath"></param>
		/// <returns>The path where the folder originally was on the source before it was deleted from the source</returns>
		public string ReconstructedSourcePath(string sBackupPath) {
			string sReturn=sBackupPath;
			//sBackupPath is constructed using: destinationComboBox.Items.Add(Common.LocalFolderThenSlash(locinfoarrPseudoRoot[iNow].FullName) + ((DestSubfolderRelNameThenSlash!=null)?DestSubfolderRelNameThenSlash:"");
			string sDestPrefix=Common.LocalFolderThenSlash(DestinationDriveRootDirectory_FullName_OrSlashIfRootDir)+((DestSubfolderRelNameThenSlash!=null)?DestSubfolderRelNameThenSlash:null);
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
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sSrcPath"></param>
		/// <param name="enableRecreateFullPath"></param>
		/// <param name="baseSourcePath">If enableRecreateFullPath is false, baseSourcePath will be excluded from the path on the backup</param>
		/// <param name="retroactive_string"></param>
		/// <returns>The directory for backup, including DestSubFolder, retroactive_string, and all parts of source path (including drive letter if any)</returns>
		public string ReconstructedBackupPath(string sSrcPath, bool enableRecreateFullPath, string baseSourcePath, string retroactive_string) {
			// Output("Reconstruction sSrcPath(as received): "+sSrcPath);  // debug only
			// Output("Reconstruction DestinationDriveRootDirectory_FullName_OrSlashIfRootDir(as received): "+DestinationDriveRootDirectory_FullName_OrSlashIfRootDir);  // debug only
			// NOTE: Common.LocalFolderThenSlash just makes sure it ends with a slash and uses Common.sDirSep
			string sReturn = null;  // Common.LocalFolderThenSlash(DestinationDriveRootDirectory_FullName_OrSlashIfRootDir)+((DestSubfolderRelNameThenSlash!=null)?DestSubfolderRelNameThenSlash:"");
			sReturn = DestinationDriveRootDirectory_FullName_OrSlashIfRootDir;
			if (DestSubfolderRelNameThenSlash!=null) {
				sReturn=Path.Combine(DestinationDriveRootDirectory_FullName_OrSlashIfRootDir, DestSubfolderRelNameThenSlash);
			}
			string removePartialPath = null;
			DirectoryInfo baseDI = new DirectoryInfo(baseSourcePath);
			if (baseDI.Exists) {
				removePartialPath = baseDI.Parent.FullName;  // only create the directory added (like rsync source without trailing slash)
			}
			else {
				FileInfo baseFI = new FileInfo(baseSourcePath);
				if (baseFI.Exists) removePartialPath = baseFI.Directory.FullName;  // create no extra directory, just use backup root + DestSubRelPathThenSlash if non-null
			}
			if (removePartialPath==null) throw new ApplicationException("removePartialPath is null (baseSourcePath must not exist)");
			if (retroactive_string!=null) {
				if (retroactive_string.StartsWith(char.ToString(Path.DirectorySeparatorChar))) {
					retroactive_string=retroactive_string.Substring(1);
				}
				sReturn=Path.Combine(sReturn,retroactive_string);
			}
			string sDestAppend=sSrcPath;
			int iStart=0;

			if (!enableRecreateFullPath) {
				if (sSrcPath.StartsWith(removePartialPath)) {  // NOTE: case sensitive
					iStart = removePartialPath.Length;
				}
			}

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
				sReturn=Common.LocalFolderThenSlash(DestinationDriveRootDirectory_FullName_OrSlashIfRootDir);
				if (bShowReconstructedBackupPathError) {
					MessageBox.Show("The backup source cannot be parsed so these files will be placed in \""+sReturn+"\".");
					bShowReconstructedBackupPathError=false;
				}
			}
			else sReturn+=sDestAppend;
			if ( !sReturn.EndsWith(Common.sDirSep) )
				sReturn+=Common.sDirSep;
			Console.Error.WriteLine("ReconstructedBackupPath: {sReturn:'"+sReturn+"', sSrcPath: '"+sSrcPath+"', retroactive_string: "+((retroactive_string!=null)?("'"+retroactive_string+"'"):"null")+"}");
			return sReturn;
		}//end ReconstructedBackupPath
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sSrcPath"></param>
		/// <param name="enableRecreateFullPath"></param>
		/// <param name="baseSourcePath">If enableRecreateFullPath is false, baseSourcePath will be excluded from the path on the backup</param>
		/// <returns></returns>
		public bool ReconstructPathOnBackup(string sSrcPath, bool enableRecreateFullPath, string baseSourcePath) {
			bool bAlreadyExisted=false;
			string BackupFolder_FullName=ReconstructedBackupPath(sSrcPath,enableRecreateFullPath, baseSourcePath, null);
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
		}  // end ReconstructPathOnBackup
	
	    public static bool FilesContentsAreEqual(FileInfo fileInfo1, FileInfo fileInfo2)
	    {
	    	// based on Lars' answer on https://stackoverflow.com/a/2637350/4541104
	        bool result;
	
	        if (fileInfo1.Length != fileInfo2.Length)
	        {
	            result = false;
	        }
	        else
	        {
	            using (var file1 = fileInfo1.OpenRead())
	            {
	                using (var file2 = fileInfo2.OpenRead())
	                {
	                	result = StreamsContentsAreEqual(file1, file2, fileInfo1.Name, fileInfo1.Length);
	                }
	            }
	        }
	
	        return result;
	    }
	
	    private static bool StreamsContentsAreEqual(Stream stream1, Stream stream2, string name, long total)
	    {
	    	// based on Lars' answer on https://stackoverflow.com/a/2637350/4541104
	        const int bufferSize = 1024 * sizeof(Int64);
	        var buffer1 = new byte[bufferSize];
	        var buffer2 = new byte[bufferSize];
	        long offset = 0;
	        long last_percent = 0;
	
	        while (true)
	        {
	            int count1 = stream1.Read(buffer1, 0, bufferSize);
	            int count2 = stream2.Read(buffer2, 0, bufferSize);
	            offset += bufferSize;
	
	            if (count1 != count2)
	            {
	                return false;
	            }
	
	            if (count1 == 0)
	            {
	                return true;
	            }
	
	            int iterations = (int)Math.Ceiling((double)count1 / sizeof(Int64));
	            for (int i = 0; i < iterations; i++)
	            {
	                if (BitConverter.ToInt64(buffer1, i * sizeof(Int64)) != BitConverter.ToInt64(buffer2, i * sizeof(Int64)))
	                {
	                    return false;
	                }
	            }
	            long percent = 100 * offset / total;
	            if (percent - last_percent >= 5) {  // reduce GUI updates
	            	if (tbStatusNow != null) {
		            	tbStatusNow.Text = "Checking \""+name+"\", please wait..."+percent+"%";
						Application.DoEvents();
	            	}
					last_percent = percent;
	            }
	        }
	    }
		
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
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="SrcFile_FullName"></param>
		/// <param name="enableRecreateFullPath"></param>
		/// <param name="baseSourcePath">If enableRecreateFullPath is false, baseSourcePath will be excluded from the path on the backup</param>
		//public string BackupFile(string SrcFile_FullName, bool enableRecreateFullPath, string baseSourcePath) {
		//	return BackupFile(SrcFile_FullName, enableRecreateFullPath, null);
		//}

		/*
		"A Windows file time is a 64-bit value that represents the
		number of 100-nanosecond intervals that have elapsed since
		12:00 midnight, January 1, 1601 A.D. (C.E.) Coordinated
		Universal Time (UTC)."
		-<https://learn.microsoft.com/en-us/dotnet/api/system.datetime.tofiletimeutc?view=net-8.0>
		Therefore:
		*/
		public const long winFileTimeUnitsPerSec = 1000000000 / 100;  // 1 billion / 100 == 10,000,000
		public const long variance = 20000000-1;  // 20 million increments is 2 seconds

		/// <summary>
		/// Fix an odd issue where time differs by one second after copied:
		/// `Dest is newer: "...Screenshot 2020-12-15 023148.png" 12/15/2020 7:31:52 AM>12/15/2020 7:31:51 AM`
		/// where dest is first and "..." is redacted.
		/// Therefore, *always* use SameWriteTime or GreaterWriteTime instead of LastWriteTimeUtc!
		/// See also:
		/// https://www.reddit.com/r/PowerShell/comments/2xanvb/comparing_lastwritetime_on_servers_with_time_off/
		/// </summary>
		/// <param name="fi1"></param>
		/// <param name="fi2"></param>
		/// <returns>True if same within variance</returns>
		public static bool SameWriteTime(FileInfo fi1, FileInfo fi2) {
			// long variance_seconds = 1;
			// long variance = variance_seconds * per_second;
			// or:
			// long variance = 1;
			// long timestamp1 = fi1.LastWriteTime.ToFileTimeUtc() / winFileTimeUnitsPerSec;
			// long timestamp2 = fi2.LastWriteTime.ToFileTimeUtc() / winFileTimeUnitsPerSec;
			// ^ fails with "script - Copy.txt" 133627719800000000>133627719782998069" so:
			long timestamp1 = fi1.LastWriteTime.ToFileTimeUtc();
			long timestamp2 = fi2.LastWriteTime.ToFileTimeUtc();
			if (timestamp1 == timestamp2) {
				return true;
			}
			if (Math.Abs(timestamp1 - timestamp2) <= variance) {
				return true;
			}
			return false;
		}

		/// <summary>
		/// Fix an odd issue where time differs by one second after copied:
		/// `Dest is newer: "...Screenshot 2020-12-12 065846.png" 132522479300000000>132522479290000000`
		/// where dest is first and "..." is redacted.
		/// Therefore, *always* use SameWriteTime or GreaterWriteTime instead of LastWriteTimeUtc!
		/// See also:
		/// https://www.reddit.com/r/PowerShell/comments/2xanvb/comparing_lastwritetime_on_servers_with_time_off/
		/// </summary>
		/// <param name="fi1"></param>
		/// <param name="fi2"></param>
		/// <returns>True if fi1 is greater within variance</returns>
		public static bool GreaterWriteTime(FileInfo fi1, FileInfo fi2) {
			if (SameWriteTime(fi1, fi2)) {
				return false;
			}
			// long variance_seconds = 1;
			// long variance = variance_seconds * per_second;
			// long timestamp1 = fi1.LastWriteTime.ToFileTimeUtc() / winFileTimeUnitsPerSec;
			// long timestamp2 = fi2.LastWriteTime.ToFileTimeUtc() / winFileTimeUnitsPerSec;
			// ^ fails with "script - Copy.txt" 133627719800000000>133627719782998069
			// ^ still fails with "desktop.ini" 133626953980000000>133626953960573144 so:
			long timestamp1 = fi1.LastWriteTime.ToFileTimeUtc();
			long timestamp2 = fi2.LastWriteTime.ToFileTimeUtc();
			if (timestamp1 > timestamp2) {
				return true;
			}
			if (timestamp1 > timestamp2 - variance) {
				return true;
			}
			return false;
		}
		
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="SrcFile_FullName"></param>
		/// <param name="enableRecreateFullPath"></param>
		/// <param name="baseSourcePath">If enableRecreateFullPath is false, baseSourcePath will be excluded from the path on the backup</param>
		/// <param name="fiNow"></param>
		public string BackupFile(string SrcFile_FullName, bool enableRecreateFullPath, string baseSourcePath, FileInfo fiNow) {
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
					//string BackupFolder_ThenSlash=enableRecreateFullPath?ReconstructedBackupPath(fiNow.Directory.FullName, enableRecreateFullPath, baseSourcePath, null):Common.LocalFolderThenSlash(DestinationDriveRootDirectory_FullName_OrSlashIfRootDir);
					string BackupFolder_ThenSlash=ReconstructedBackupPath(fiNow.Directory.FullName, enableRecreateFullPath, baseSourcePath, null);
					//if (enableRecreateFullPath) {

					if (!Directory.Exists(ReconstructedBackupPath(fiNow.Directory.FullName, enableRecreateFullPath, baseSourcePath, null))) {
						ReconstructPathOnBackup(fiNow.Directory.FullName, enableRecreateFullPath, baseSourcePath);
					}

					//} // else store it directly to root (+DesSubFolderRelNameThenSlash if non-null) and assume root exists (required for manual operation)

					if (!BackupFolder_ThenSlash.EndsWith(Common.sDirSep)) BackupFolder_ThenSlash+=Common.sDirSep;
					sDestFile=BackupFolder_ThenSlash+fiNow.Name;
					FileInfo fiDest=new FileInfo(sDestFile);
					string dotExt = Path.GetExtension(fiNow.Name); // may return null
					bool nonDated = nonDatedDotExts.Contains(dotExt);  // if null, still works (false)
					bool multi_save = false;
					bool update = false;
					if (fiDest.Exists) {
						if (SameWriteTime(fiDest, fiNow) && fiDest.Length!=fiNow.Length) {
							// *same* date, different length
							Output(sDone+"Resaving: \""+sDestFile+"\"");
							multi_save = true;
						}
						else if (SameWriteTime(fiDest, fiNow) && nonDated) {
							// *same* date, but non-dated (such as a mountable drive image)
							tbStatus.Text = "Checking \""+fiNow.Name+"\", please wait...";
							Application.DoEvents();
							if (!FilesContentsAreEqual(fiNow, fiDest)) {
								Output(sDone+"Resaving: \""+sDestFile+"\"");
								multi_save = true;
								tbStatus.Text = "Checking \""+fiNow.Name+"\"...resaving.";
							}
							else {
								// *Content* is equal, so do not save, but log it for these!
								string msg = "File contents match: source \""+fiNow.FullName+"\" and destination \""+fiDest.FullName+"\"";
								Debug.WriteLine(msg);
								Output(msg);
								tbStatus.Text = "Checking \""+fiNow.Name+"\"...already saved.";
							}
							Application.DoEvents();
						}
						else if (GreaterWriteTime(fiDest, fiNow)) {
							LogWriteLine("Dest is newer: \""+sDestFile+"\" "+fiDest.LastWriteTime.ToFileTimeUtc().ToString()+">"+fiNow.LastWriteTime.ToFileTimeUtc().ToString());
						}
						else if (GreaterWriteTime(fiNow, fiDest) || fiDest.Length!=fiNow.Length) {
							Output(sDone+"Updating: \""+sDestFile+"\"");
							update = true;
						}

						if (!SameWriteTime(fiDest, fiNow) && nonDated) {
							// If nonDated file was not logged yet, log it for clarity.
							if (GreaterWriteTime(fiDest, fiNow)) {
								LogWriteLine("\""+fiDest.FullName+"\" was newer: "+fiDest.LastWriteTime.ToFileTimeUtc().ToString()+">"+fiNow.LastWriteTime.ToFileTimeUtc().ToString());
								// multi_save = true;
							}
							else if (SameWriteTime(fiDest, fiNow)) {
								LogWriteLine("\""+fiDest.FullName+"\" was same: "+fiDest.LastWriteTime.ToFileTimeUtc().ToString()+"=="+fiNow.LastWriteTime.ToFileTimeUtc().ToString());
								// multi_save = true;
							}
							else {
								LogWriteLine("\""+fiDest.FullName+"\" was older: "+fiDest.LastWriteTime.ToFileTimeUtc().ToString()+"<"+fiNow.LastWriteTime.ToFileTimeUtc().ToString());
								// multi_save = true;
							}
						}

						if (update) {
							// TODO: Store retroactive copy with get_retroactive_timed_folder_partialpath_from_UTC
							if (!bTestOnly) {
								lByteCountTotalActuallyAdded+=(long)fiNow.Length-(long)fiDest.Length;
								sLastAttemptedCommand=sCP+" \""+SrcFile_FullName+"\" \""+sDestFile+"\"";
								File.Copy(SrcFile_FullName,sDestFile,true);
								ulByteCountTotalActuallyCopied+=(ulong)fiNow.Length;
							}
						}
						else if (multi_save) {
							// TODO: Store retroactive copy in special folder as per issue #21
							if (!bTestOnly) {
								lByteCountTotalActuallyAdded+=(long)fiNow.Length-(long)fiDest.Length;
								sLastAttemptedCommand=sCP+" \""+SrcFile_FullName+"\" \""+sDestFile+"\"";
								string firstDest = sDestFile + ".1st";
								string bakDest = sDestFile + ".bak";
								string badBakSrc = SrcFile_FullName + ".bak";
								string badFirstSrc = SrcFile_FullName + ".1st";
								if (File.Exists(badBakSrc)) {
									LogWriteLine(
										"Warning: there is an extra backup file \""+badBakSrc+"\""
										+"which (if not excluded)"
										+ " will interfere with multi-backup of non-dated file \""+bakDest+"\""
									);
								}
								if (File.Exists(badFirstSrc)) {
									LogWriteLine(
										"Warning: there is an extra backup file \""+badFirstSrc+"\""
										+"which (if not excluded)"
										+ " will interfere with multi-backup of non-dated file \""+firstDest+"\""
									);
								}
								
								if (!File.Exists(firstDest)) {
									File.Move(sDestFile, firstDest);
									lByteCountTotalActuallyAdded += fiDest.Length;
								}
								else if (File.Exists(bakDest)) {
									if (File.Exists(sDestFile)) {
										lByteCountTotalActuallyAdded -= (new FileInfo(bakDest)).Length;
										File.Delete(bakDest);
										lByteCountTotalActuallyAdded += fiDest.Length;
										File.Move(sDestFile, bakDest);
									}
								}
								else if (File.Exists(sDestFile)) {
									lByteCountTotalActuallyAdded += fiDest.Length;
									File.Move(sDestFile, bakDest);
								}
								File.Copy(SrcFile_FullName,sDestFile,true);
								ulByteCountTotalActuallyCopied+=(ulong)fiNow.Length;
							}
							tbStatus.Text = "Checking \""+fiNow.Name+"\"...resaved.";
							Application.DoEvents();
						}
						else {
							// The file already appears to be backed up and up to date.
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
				if (exn.ToString().ToLower().Contains("system.io.ioexception: disk full")
				   || exn.ToString().ToLower().Contains("system.io.ioexception: there is not enough space on the disk")) {
					bDiskFullLastRun=true;
				}
				else if (exn.ToString().ToLower().Contains("System.IO.PathTooLongException".ToLower())) {//.Contains("too long")) {
					string msg_suffix="";
					string overlimit_yml_path=DestinationDriveRootDirectory_FullName_OrSlashIfRootDir;
					if (DestSubfolderRelNameThenSlash!=null) {
						overlimit_yml_path=Path.Combine(overlimit_yml_path,DestSubfolderRelNameThenSlash);
					}
					overlimit_yml_path=Path.Combine(overlimit_yml_path, overlimit_yml_name);
					string overlimit_content_path=DestinationDriveRootDirectory_FullName_OrSlashIfRootDir;
					if (DestSubfolderRelNameThenSlash!=null) {
						overlimit_content_path=Path.Combine(overlimit_content_path,DestSubfolderRelNameThenSlash);
					}
					overlimit_content_path=Path.Combine(overlimit_content_path, overlimit_content_name);
					string original_dest_file_path=sDestFile;
					try {
						if (!Directory.Exists(overlimit_content_path)) {
							Directory.CreateDirectory(overlimit_content_path);
						}
						sDestFile = Path.Combine(overlimit_content_path, fiNow.Name);
						StreamWriter outs = null;
						if (is_first_overlimit) {
							outs = new StreamWriter(overlimit_yml_path);
							outs.WriteLine("sDestFile"+";"+"SrcFile_FullName"+";"+"original_dest_file_path");
							is_first_overlimit=false;
						}
						else outs = File.AppendText(overlimit_yml_path);
						outs.WriteLine(sDestFile+";"+SrcFile_FullName+";"+original_dest_file_path);
						outs.Close();
						sLastAttemptedCommand=sCP+" \""+fiNow.FullName+"\" \""+sDestFile+"\"";
						File.Copy(fiNow.FullName,sDestFile); //TODO: !! figure out why SrcFile_FullName is only .Name !!
						msg_suffix="("+successfully_redirected_string+" to \""+sDestFile+"\")";
					}
					catch (Exception exn2) {
						msg_suffix="(failed to save in \""+overlimit_content_path+"\": "+ToOneLine(exn2.ToString())+")";
					}
					if (!msg_suffix.Contains(successfully_redirected_string)) alCopyError.Add("Filename is too long for "+sCopyErrorFileFullNameOpener+SrcFile_FullName+sCopyErrorFileFullNameCloser+" "+msg_suffix+ToOneLine(exn.ToString()));
				}
				else if (exn.ToString().Contains("system.io.directorynotfoundexception")) {
					alCopyError.Add("Recreate source folder failed"+sCopyErrorFileFullNameOpener+SrcFile_FullName+sCopyErrorFileFullNameCloser+ToOneLine(exn.ToString()));
				}
				else if (exn.ToString().ToLower().Contains("not enough space")) {
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
				sMsg=ToOneLine(sMsg);
				if (sMsg.Trim().Length>0 && !sMsg.Contains(successfully_redirected_string)) Common.ShowExn(exn, "backing up file ("+sMsg+")", "BackupFile");
			}
			iFilesProcessed++;
			return sDestFile;
		}  // end BackupFile
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
					
					
					tbStatusNow.Text=String.Format( "{0} ({1} files) processed, {2} added      {3} " + ((ulByteCountDestAvailableFreeSpace==Int64.MaxValue)?"disk space unknown (not implemented in this version of your computer's framework)({4}/{5} MB)":"space remaining ({4}/{5} MB)"),
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
			cancelButton.Enabled=false;
			this.Text=sMyNameAndVersion;
			this.tbStatus.Text = "Welcome to "+sAppName+"";
			DateTime datetimeNow = DateTime.Now;
			string sMyProcess = Assembly.GetExecutingAssembly().Location;
			sMyProcess = sMyProcess.Substring(sMyProcess.LastIndexOf(Common.sDirSep) + 1);
			//errStream=new StreamWriter(sFileErrLog);
			//Console.SetError(errStream);
			Console.Error.Write("{0}", sMyProcess);
			Console.Error.WriteLine(": started at {0}.", datetimeNow);
			Console.Error.WriteLine();
			if (bTestOnly) Common.iDebugLevel=Common.DebugLevel_On;
			//FolderLister.bDebug=bTestOnly;
			//bDebug=bTestOnly;
			lbOutNow=this.lbOut;
			Console.Error.WriteLine("Running startupTimer");
			
			startupTimer.Start(); // runs StartupTimerTick after GUI is finished loading.
			
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

//		void DestinationComboBoxTextChanged(object sender, EventArgs e) {
//			//bool bFound=false;
//			//foreach (string sNow in destinationComboBox.Items) {
//			//	if (destinationComboBox.Text==sNow) bFound=true;
//			//}
//			//if (!bFound) destinationComboBox.SelectedIndex=0;
//			int FolderIndexNow=Common.InternalIndexOfPseudoRoot_WhereIsOrIsParentOf_FolderFullName(destinationComboBox.Text,false);
//			LocInfo locinfoNow=Common.GetPseudoRoot(FolderIndexNow);
//			bool bGB=locinfoNow.AvailableFreeSpace/1024/1024/1024 > 0;
//			if (FolderIndexNow>=0) {
//				this.driveLabel.Text=locinfoNow.VolumeLabel+" ("
//					+ ((locinfoNow.AvailableFreeSpace!=Int64.MaxValue)  ?  ( bGB ? (((decimal)locinfoNow.AvailableFreeSpace/1024m/1024m/1024m).ToString("#")+"GB free"):(((decimal)locinfoNow.AvailableFreeSpace/1024m/1024m).ToString("0.###")+"MB free") )  :  "unknown free"  )
//					+ ")";
//			}
//			else this.driveLabel.Text="";
//		}//end DestinationComboBoxTextChanged
		
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
		
		
		///as per priit on http://stackoverflow.com/questions/1410127/c-sharp-test-if-user-has-write-access-to-a-folder
		///edited Oct 17 '14 at 18:27
		public bool IsDirectoryWritable(string dirPath) //, bool throwIfFails = false)
		{
			try
			{
				using (FileStream fs = File.Create(
					Path.Combine(
						dirPath, 
						Path.GetRandomFileName()
					), 
					1,
					FileOptions.DeleteOnClose)
				)
				{ }
				return true;
			}
			catch (Exception exn)
			{
				//if (throwIfFails)
				//	throw;
				Console.WriteLine("Could not finish IsDirectoryWritable: "+exn.ToString());
				//else
					return false;
			}
		}


		
		//public static DirectoryInfo diMyDocs = null;
		//public static DirectoryInfo diUserProfile = null;
		public static string USERPROFILE_path = null;
		public static string MYDOCS_path = null;
		public static string DESKTOP_path = null;
		public static string APPDATA_path = null;
		public static string LOCALAPPDATA_path = null;
		public static bool is_CheckUserVars_done = false;
		public static void CheckUserVars() {
			try {
				if (MYDOCS_path==null || USERPROFILE_path==null) {
					DirectoryInfo diMyDocs=new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
					DirectoryInfo diUserProfile=null;
					if (diMyDocs!=null) {
						if (diMyDocs.Exists) {
						}
						else Output("WARNING: MYDOCS does not exist.");
						string parent_path = diMyDocs.Parent.FullName;
						if (parent_path!="/home" && parent_path!="C:\\Users" && parent_path!="Documents and Settings") {
							diUserProfile=diMyDocs.Parent;
						}
						else diUserProfile=diMyDocs;
					}
					else Output("WARNING: SpecialFolder.MyDocuments returned null.");
					if (diUserProfile!=null) {
						if (diUserProfile.Exists) {
						}
						else Output("WARNING: USERPROFILE does not exist.");
						USERPROFILE_path=diUserProfile.FullName;
					}
					else Output("WARNING: Could not determine USERPROFILE by checking parent of SpecialFolder.MyDocuments, so USERPROFILE will remain null.");
					MYDOCS_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
					DESKTOP_path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
					APPDATA_path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
					LOCALAPPDATA_path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
				}
			}
			catch (Exception exn) {
				//Common.ShowExn(exn,"checking user environment variables","CheckUserVars");
				//sValue=sValueOrig;
			}
			is_CheckUserVars_done=true;
		}
		
		// see also
		// Application Data 	Per-user application-specific files 	%USERPROFILE%\Application Data 	Win98
		// Cookies 	Internet Explorer browser cookies 	%USERPROFILE%\Cookies 	Win98
		// Desktop Directory 	Files stored on the user's desktop 	%USERPROFILE%\Desktop 	Win95
		// Favorites 	User's Favorites 	%USERPROFILE%\Favorites 	Win9898
		// Fonts 	Container folder for installed fonts 	%windir%\Fonts 	Win98XP
		// History 	User-specific browser history 	%USERPROFILE%\Local Settings\History 	Win98
		// Internet Cache 	User-specific Temporary Internet Files 	%USERPROFILE%\Local Settings\Temporary Internet Files 	Win98
		// LocalApplicationData 	User-specific and computer-specific application settings 	%USERPROFILE%\Local Settings\Application Data 	Win2000/ME
		// My Documents 	%USERPROFILE%\My Documents (WinNT line) [User's documents] C:\My Documents (Win98-ME) 	Win98 []
		// My Music 	User's music 	%USERPROFILE%\My Documents\My Music 	WinXP []
		// My Pictures 	User's pictures 	%USERPROFILE%\My Documents\My Pictures 	WinXP []
		// My Videos 	User's video files 	%USERPROFILE%\My Documents\My Videos 	WinXP []
		// Programs 	User-specific "(All) Programs" groups and icons 	%USERPROFILE%\Start Menu\Programs 	Win95 []
		// Recent 	User-specific "My Recent Documents" 	%USERPROFILE%\Recent 	Win98 []
		// Send To 	User-specific "Send To" menu items 	%USERPROFILE%\SendTo 	Win98 []
		// Start Menu 	User-specific "Start Menu" items 	%USERPROFILE%\Start Menu 	Win98 []
		// System 	The Windows system directory 	%windir%\system32 	Win2000 []
		// Saved Games 	User's Saved Games 	%USERPROFILE%\saved games 	WinVista []
		// Templates 	User-specific document templates 	%USERPROFILE%\Templates 	Win98 []
		
		// Desktop                C:\Users\{username}\Desktop
		// Programs               C:\Users\{username}\AppData\Roaming\Microsoft\Windows\Start Menu\Programs
		// Personal               C:\Users\{username}\Documents
		// Personal               C:\Users\{username}\Documents
		// Favorites              C:\Users\{username}\NetHood\Favorites
		// Startup                C:\Users\{username}\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup
		// Recent                 C:\Users\{username}\AppData\Roaming\Microsoft\Windows\Recent
		// SendTo                 C:\Users\{username}\AppData\Roaming\Microsoft\Windows\SendTo
		// StartMenu              C:\Users\{username}\AppData\Roaming\Microsoft\Windows\Start Menu
		// MyMusic                C:\Users\{username}\Music
		// DesktopDirectory       C:\Users\{username}\Desktop
		// MyComputer
		// Templates              C:\Users\{username}\AppData\Roaming\Microsoft\Windows\Templates
		// ApplicationData        C:\Users\{username}\AppData\Roaming
		// LocalApplicationData   C:\Users\{username}\AppData\Local
		// InternetCache          C:\Users\{username}\AppData\Local\Microsoft\Windows\Temporary Internet Files
		// Cookies                C:\Users\{username}\AppData\Roaming\Microsoft\Windows\Cookies
		// History                C:\Users\{username}\AppData\Local\Microsoft\Windows\History
		// CommonApplicationData  C:\ProgramData
		// System                 C:\Windows\system32
		// ProgramFiles           C:\Program Files (x86)
		// MyPictures             C:\Users\{username}\Pictures
		// CommonProgramFiles     C:\Program Files (x86)\Common Files
		
		// %ALLUSERSPROFILE%  	C:\Documents and Settings\All Users  OR  C:\Users\{username}
		// %APPDATA% 	C:\Documents and Settings\{username}\Application Data  OR  C:\Users\{username}\AppData\Roaming
		// %HOMEPATH% 	\Documents and Settings\{username}  OR  \Users\{username}
		// %USERPROFILE% 	C:\Documents and Settings\{username}  OR  C:\Users\{username}
		
		// %PROGRAMFILES% 	Directory containing program files, usually C:\Program Files
		// %WINDIR% 	C:\Windows
		// %SYSTEMROOT% 	The Windows XP root directory, usually C:\Windows
		// %TEMP% and %TMP% 	C:\DOCUME~1\{username}\LOCALS~1\Temp
		// %USERNAME% 	{username}
		// %SYSTEMDRIVE% 	The drive containing the Windows XP root directory, usually C:
		// %PROMPT% 	Code for current command prompt format. Code is usually $P$G
		// %HOMEDRIVE% 	C:
		// %COMPUTERNAME% 	{computername}
		// %COMSPEC% 	C:\Windows\System32\cmd.exe
		// %PATH% 	C:\Windows\System32\;C:\Windows\;C:\Windows\System32\Wbem
		// %PATHEXT% 	.COM; .EXE; .BAT; .CMD; .VBS; .VBE; .JS ; .WSF; .WSH
		public static string ReplacedUserVars(string sValue) {
			string sValueOrig=sValue;
			try {
				if (!is_CheckUserVars_done) {
					CheckUserVars();
				}
				if (USERPROFILE_path!=null) sValue=sValue.Replace("%USERPROFILE%",USERPROFILE_path);
				if (MYDOCS_path!=null) sValue=sValue.Replace("%MYDOCS%",MYDOCS_path); //same as SpecialFolder.Personal
				if (DESKTOP_path!=null) sValue=sValue.Replace("%DESKTOP%",DESKTOP_path); //The logical Desktop rather than the physical file system location DesktopDirectory
				if (APPDATA_path!=null) sValue=sValue.Replace("%APPDATA%",APPDATA_path);
				if (LOCALAPPDATA_path!=null) sValue=sValue.Replace("%LOCALAPPDATA%",LOCALAPPDATA_path);
			}
			catch (Exception exn) {
				Common.ShowExn(exn,"replacing environment variable references with values","ReplacedUserVars");
				sValue=sValueOrig;
			}
			return sValue;
		}
		private void DisplayLoadedProfileName() {
			DirectoryInfo diProfileX = new DirectoryInfo(BackupProfileFolder_FullName);
			this.profileCBSuspendEvents = true;
			if (!this.profileCB.Items.Contains(diProfileX.Name)) {
				this.profileCB.Items.Add(diProfileX.Name);
			}
			this.profileCB.SelectedIndex = this.profileCB.Items.IndexOf(diProfileX.Name);
			Application.DoEvents();
			this.profileCBSuspendEvents = false;
		}
		private bool _RunScriptLine(string sLine, bool enableRecreateFullPath, string sFile, int lineNumber, long timeoutMS) {
			System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
			watch.Start();
			bool bForceBad=false;
			bool bGood=false;
			Common.RemoveEndsWhiteSpaceByRef(ref sLine);
			Common.sParticiple="showing line";
			sLine=sLine.Trim();
			//if (sLine==null||(sLine.Length>0&&!sLine.StartsWith("#"))) LogWriteLine(Common.SafeString(sLine,true,false));
			Common.sParticiple="parsing line";
			int iMarker=sLine.IndexOf(":");
			if (iMarker>0) {  // && sLine.Length>(iMarker+1)) {
				string sCommandLower=sLine.Substring(0,iMarker).ToLower().Trim();
				string sValue=sLine.Substring(iMarker+1).Trim();
				LogWriteLine("RunScriptLine("+((sLine!=null)?("\""+sLine+"\""):"null")+","+(enableRecreateFullPath?"true":"false")+","+((sFile!=null)?("\""+sFile+"\""):"null")+","+lineNumber.ToString());
				sValue=ReplacedUserVars(sValue);
				//%USERPROFILE%
				if (sCommandLower.StartsWith("#")) {
					//ignore
				}
				else if (sCommandLower=="excludedest") {
					char thisSlash=Common.getSlash(sValue);
					//lbOut.Items.Add("Got slash: "+char.ToString(thisSlash));
					if (thisSlash==(char)0 || thisSlash==Path.DirectorySeparatorChar) {
						Common.AddDriveToInvalidDrives(sValue);
						//lbOut.Items.Add("Not using "+sValue+" for backup");
					}
					else {
//							string msg="Ignoring exclusion "+sValue+" since it is not a valid path on this OS.";
//							lbOut.Items.Add(msg);
//							Console.Error.WriteLine(msg);
					}
				}
				else if (sCommandLower=="includedest") {
					Common.AddPathToExtraPseudoRootsToManuallyAdd(sValue);
				}
				else if (sCommandLower=="addmask") {
					Common.allowed_names.Add(sValue);
					string sTemp="";
					foreach (string sMask in Common.allowed_names) {
						sTemp+=(sTemp==""?"":", ")+sMask;
					}
					if (bTestOnly) Output("#Masks changed: "+sTemp);
				}
				else if (sCommandLower=="removemask") {
					if (sValue=="*") Common.allowed_names.Clear();
					else Common.allowed_names.Remove(sValue);
					string sTemp="";
					foreach (string sMask in Common.allowed_names) {
						sTemp+=(sTemp==""?"":", ")+sMask;
					}
					if (bTestOnly) Output("#Masks changed: "+sTemp);
				}
				else if (sCommandLower=="exclude") {
					Common.excluded_names.Add(sValue);
					string sTemp="";
					foreach (string sExclusion in Common.excluded_names) {
						sTemp+=(sTemp==""?"":", ")+sExclusion;
					}
					if (bTestOnly) Output("#Exclusions changed: "+sTemp);
				}
				else if (sCommandLower=="excludefolderfullname") {
					Common.excluded_paths.Add(ReplacedUserVars(sValue));
					string sTemp="";
					foreach (string sExclusion in Common.excluded_paths) {
						sTemp+=(sTemp==""?"":", ")+sExclusion;
					}
					if (bTestOnly) Output("#Excluded paths changed: "+sTemp);
				}
				else if (sCommandLower=="uselastretroactivedirectoryanswer") {
					useLastDirectoryDREnable=ToBool(sValue);
				}
				else if (sCommandLower=="include") {
					if (sValue=="*") Common.excluded_names.Clear();
					else Common.excluded_names.Remove(sValue);
					string sTemp="";
					foreach (string sExclusion in Common.excluded_names) {
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
								ReconstructPathOnBackup(fiSrc.DirectoryName, enableRecreateFullPath, fiSrc.Directory.FullName);
								//alFilesBackedUpManually.Add(Path.Combine(ReconstructedBackupPath(fiSrc.DirectoryName,null),fiSrc.Name));
								if (!Common.IsExcludedFile(fiSrc)) {
									string resultPath = BackupFile(sFileTheoretical, enableRecreateFullPath, fiSrc.FullName, null);  // if (!Common.IsExcludedFile(fiSrc.Directory,fiSrc)) BackupFile(sFileTheoretical,true);
									alFilesBackedUpManually.Add(resultPath);
								}
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
								string FolderTheoretical_FullName=Path.Combine( Path.Combine(diBase.FullName,diNow.Name), SpecifiedFolder_Name );
								if (FolderTheoretical_FullName.Contains(Common.SlashWildSlash) //allow Common.SlashWildSlash to allow recursive usage of Common.SlashWildSlash
								    ||Directory.Exists(FolderTheoretical_FullName)) {
									alFoldersTheoretical.Add(FolderTheoretical_FullName);
								}
							}
							LogWriteLine("Adding ("+alFoldersTheoretical.Count.ToString()+") folder(s) via wildcard "+Common.SafeString(sValue,true)+"...");
							Application.DoEvents();
							int iNonExcludable=0;
							int iWildcardsAdded=0;
							foreach (string sFolderTheoretical in alFoldersTheoretical) {
								if (!sFolderTheoretical.Contains(Common.SlashWildSlash)) {
									if (!Common.IsExcludedFolder(new DirectoryInfo(sFolderTheoretical))) { //if (!Common.IsExcludedFolder(new DirectoryInfo(sFolderTheoretical),true,true,false)) {
										RunScriptLine("AddFolder:"+sFolderTheoretical, enableRecreateFullPath, "<wildcard in "+((sFile!=null)?sFile:"null")+">", lineNumber, -1);
										iNonExcludable++;
									}
								}
								else {
									RunScriptLine("AddFolder:"+sFolderTheoretical, enableRecreateFullPath, "<wildcard in "+((sFile!=null)?sFile:"null")+">", lineNumber, -1);
									iWildcardsAdded++;
								}
							}
							if (iNonExcludable>0) LogWriteLine("Done adding ("+iNonExcludable.ToString()+") folder(s) via wildcard and "+iWildcardsAdded.ToString()+" recursive wildcard folders.");
						}
						else {
							LogWriteLine("ERROR: Folder does not exist ("+Common.SafeString(BaseFolder_FullName,true)+")--cannot add specified subfolder(s) via wildcard.");
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
						cancelButton.Enabled=true;
						//Common.sParticiple="getting directory info";
						DirectoryInfo diRoot=new DirectoryInfo(sSearchRoot);
						iDepth=-1;
						//bBusyCopying=true;
						if (diRoot.Exists) BackupFolder(diRoot, enableRecreateFullPath, diRoot.FullName);
						//else {
						//	Output("Folder cannot be read: "+diRoot.FullName);
						//}
						//}
						//bBusyCopying=false;
						cancelButton.Enabled=false;
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
					Common.sParticiple="setting DestSubFolder";
					// FIXME: Maybe don't use Environment.MachineName here, since it can change after a migration or reinstall.
					RunScriptLine("DestSubFolder:Backup-"+Environment.MachineName, enableRecreateFullPath, "<loadprofile automation>", -1, 10);  // ok since happens before main.ini
					RunScriptLine("RecreateFullPathOnBackup:on", enableRecreateFullPath, "<loadprofile automation>", -1, 10);  // ok since happens before main.ini
					Common.iDebugLevel=Common.DebugLevel_Mega;//debug only
					this.menuitemEditScript.Enabled=false;
					this.menuitemEditMain.Enabled=false;
					Console.Error.Write("LoadProfile...");
					Console.Error.Flush();
					char foundSlash=Common.getSlash(sValue);
					//string BackupProfileFolder_FullName_TEMP=Path.Combine( Path.Combine(".","profiles"), sValue );
					string BackupProfileFolder_FullName_TEMP=null;
					if (sValue.Trim() == "") {
						BackupProfileFolder_FullName_TEMP = null;
					}
					else if (foundSlash!=(char)0) { //if found slash
						BackupProfileFolder_FullName_TEMP=sValue;
					}
					else { //else did not find slash
						BackupProfileFolder_FullName_TEMP=Path.Combine(profilesFolder_FullName,sValue);
					}
					if (BackupProfileFolder_FullName_TEMP != null) {
						Console.Error.Write("checking \"" + BackupProfileFolder_FullName_TEMP + "\"...");
					Console.Error.Flush();
					}
					if (BackupProfileFolder_FullName_TEMP == null) {
						Console.Error.Write("LoadProfile value is blank!");
						string sMsg="No profile name was set!";
						Console.Error.WriteLine(sMsg);
						MessageBox.Show(sMsg);
					}
					else if (Directory.Exists(BackupProfileFolder_FullName_TEMP)) {
						Console.Error.Write("found...");
						Console.Error.Flush();
						DirectoryInfo diProfileX = new DirectoryInfo(BackupProfileFolder_FullName_TEMP);
						if (diProfileX.Name == profilesFolder_Name) {
							throw new ApplicationException("value is blank or invalid \"profiles\"");
						}
						ProfileName = diProfileX.Name; // automatically corrects for full path
						// but don't SaveStartupFile: The value could be from somewhere else (maybe *not* startup.ini!)
						
						
						this.menuitemEditScript.Enabled=true;
						this.menuitemEditMain.Enabled=true;
						Common.ClearInvalidDrives();
						Common.ClearExtraDestinations();
						
						//string ProfileFolder_FullName;
						RunScript(MainScriptFile_FullName, recreateFullPathCheckBox.Checked, 1000); //excludes and adds destinations
						
						if (File.Exists( Path.Combine(BackupProfileFolder_FullName, LogFile_Name) )) {
							RunScript(Path.Combine(BackupProfileFolder_FullName, LogFile_Name), recreateFullPathCheckBox.Checked, 1000); //excludes and adds destinations
						}
						bLoadedProfile=true;
						Common.UpdateSelectableDrivesAndPseudoRoots(true);
						Common.sParticiple="finished updating Drives and PseudoRoots";
						if (Common.bMegaDebug) {
							Console_Error_WriteLine_AllDebugInfo();
						}
						//alPseudoRootsNow=Common.PseudoRoots_DriveRootFullNameThenSlash_ToArrayList();
						
						destinationComboBox.BeginUpdate();
						destinationComboBox.Items.Clear();
						//destinationComboBox.Items.Clear();//already done above
						Common.sParticiple="getting usable drives";
						//alSelectableDrives=Common.SelectableDrives_DriveRootFullNameThenSlash_ToArrayList();
						Common.sParticiple="showing usable drives";
						int comboIndex=-1;
//							foreach (string sNow in alSelectableDrives) {
//								Output("Found potential backup drive "+sNow,true);
//								destinationComboBox.Items.Add(Common.LocalFolderThenSlash(sNow) 
//								                 + ((DestSubfolderRelNameThenSlash!=null)?DestSubfolderRelNameThenSlash:""));
//								comboIndex=destinationComboBox.Items.Count-1;
//								
//							}
						int preferenceValueBest_ComboIndexNow=-1;
						if (timeoutMS > 0 && watch.ElapsedMilliseconds > timeoutMS) {
							Debug.WriteLine("Warning: \""+sLine+"\" before iterating drives took "+msss(watch.ElapsedMilliseconds)+"s");
						}
						
						for (int driveIndex=0; driveIndex<Common.GetPseudoRoots_EntriesCount(); driveIndex++) {
							LocInfo thisLocInfo=Common.GetPseudoRoot(driveIndex);
							if (thisLocInfo!=null) {
								//if (IsSelectableDrive(driveIndex)) {
								if (Common.IsValidDest(thisLocInfo.VolumeLabel)&&Common.IsValidDest(thisLocInfo.DriveRoot_FullNameThenSlash)) {
									long preferenceValue=getPreferenceLevelInPrefsFile(thisLocInfo);
									bool IsWritable=setPreferenceLevelInPrefsFile(thisLocInfo, preferenceValue);
									if (IsWritable) {
//											this was commented since it is illogical
//											if (preferenceValue==preferenceValueBest_Value
//											   		&&preferenceValue!=long.MinValue
//											  		&&driveIndex!=preferenceValueBest_DriveIndex) {
//												//prevent duplication of values:
//												preferenceValue+=1;
//												preferenceValueBest_Value=preferenceValue;
//												preferenceValueBest_DriveIndex=driveIndex;
//												setPreferenceLevelInPrefsFile(thisLocInfo,preferenceValue);
//											}
//											else 
										thisLocInfo.CustomLong=preferenceValue;
										Output("Found potential backup drive "+thisLocInfo.ToDisplayString(),true);
										destinationComboBox.Items.Add(thisLocInfo.ToDisplayString());
										comboIndex=destinationComboBox.Items.Count-1;
										if (preferenceValue>=preferenceValueBest_Value) {
											preferenceValueBest_DriveIndex=driveIndex;
											preferenceValueBest_Value=preferenceValue;
											preferenceValueBest_ComboIndexNow=comboIndex;
										}
										Common.setPseudoRootCustomInt(driveIndex,comboIndex);
									}
									else {
										thisLocInfo.CustomLong=long.MinValue;
										Output(thisLocInfo.ToDisplayString()+" is not writable");
										Common.setPseudoRootCustomInt(driveIndex,-1);
									}
								}
								else Common.setPseudoRootCustomInt(driveIndex,-1);
							}
						}//for driveIndex
						if (timeoutMS > 0 && watch.ElapsedMilliseconds > timeoutMS) {
							Debug.WriteLine("Warning: \""+sLine+"\" iterating drives took "+msss(watch.ElapsedMilliseconds)+"s");
						}
						
						if (preferenceValueBest_InitiallyChosen_Index<0) {
							preferenceValueBest_InitiallyChosen_Index=preferenceValueBest_DriveIndex;
						}
						destinationComboBox.EndUpdate();
						if (timeoutMS > 0 && watch.ElapsedMilliseconds > timeoutMS) {
							Debug.WriteLine("Warning: \""+sLine+"\" finalizing drive dropdown took "+msss(watch.ElapsedMilliseconds)+"s");
						}
						
						Application.DoEvents();
						//if (destinationComboBox.Items.Count>=1) destinationComboBox.Select(destinationComboBox.Items.Count-1,1);
						
						//FolderLister.Echo("Test");
						string sMsg="(unknown error while listing usable destinations in RunScriptLine)";
						if (Common.GetPseudoRoots_CountNonNull(false) > 0) {//iSelectableDrives+iDestinations>0) {
							//if (bExitIfNoUsableDrivesFound && Common.GetSelectableDrives_CountNonNull(false) == 0) {
							if (bExitIfNoUsableDrivesFound && destinationComboBox.Items.Count == 0) {
								sMsg = "No usable backup drives can be found.  Try connecting the drive and then try reopening this program.";
								Console.Error.WriteLine(sMsg);
								if (Common.bDebug) Console_Error_WriteLine_AllDebugInfo();
								MessageBox.Show(sMsg);
								//goButton.Enabled=false;
								Application.Exit();
							}
							if (preferenceValueBest_ComboIndexNow>=0) destinationComboBox.SelectedIndex = preferenceValueBest_ComboIndexNow;
							else destinationComboBox.SelectedIndex = destinationComboBox.Items.Count-1;
						}
						else {
							sMsg= "No backup drive can be found.  Try connecting the drive and then try again.";
							Console.Error.WriteLine(sMsg);
							MessageBox.Show(sMsg);
							/*
							Doing it asynchronously would prevent retry if drive inserted before clicking.
							new Thread(new ThreadStart(delegate
						    {
						      MessageBox.Show
						      (
						        sMsg, 
						        "",
						        MessageBoxButtons.OK,
						        MessageBoxIcon.Warning
						      );
						    })).Start();

							 */
							if (Common.bDebug) Console_Error_WriteLine_AllDebugInfo();
						}
						if (timeoutMS > 0 && watch.ElapsedMilliseconds > timeoutMS) {
							Debug.WriteLine("Warning: \""+sLine+"\" after GetPseudoRoots_CountNonNull took "+msss(watch.ElapsedMilliseconds)+"s");
						}
						
						this.profileLabel.Text="(User: "+Environment.UserName+") Configuration:";
						if (!this.profileCB.Items.Contains(diProfileX.Name)) {
							this.profileCB.Items.Add(diProfileX.Name);
						}
						
						ExpandProfileDropDown();
						// this.profileCB.Text = diProfileX.Name;
//						if (diProfileX.Name=="BackupGoNowDefault") {
//							if (this.profileLabel.Text.EndsWith("s"))
//								this.profileLabel.Text+="'";
//							else this.profileLabel.Text+="'s";
//						}
						DisplayLoadedProfileName();
						if (timeoutMS > 0 && watch.ElapsedMilliseconds > timeoutMS) {
							Debug.WriteLine("Warning: \""+sLine+"\" before ShowOptions took "+msss(watch.ElapsedMilliseconds)+"s");
						}

						ShowOptions(optionsTableLayoutPanel,BackupScriptFile_FullName);
						if (timeoutMS > 0 && watch.ElapsedMilliseconds > timeoutMS) {
							Debug.WriteLine("Warning: \""+sLine+"\" after ShowOptions took "+msss(watch.ElapsedMilliseconds)+"s");
						}
						
						//Common.sParticiple="before calling save options";
						tbStatus.Text="Saving options...";
						Application.DoEvents();
						if (timeoutMS > 0 && watch.ElapsedMilliseconds > timeoutMS) {
							Debug.WriteLine("Warning: \""+sLine+"\" after DoEvents took "+msss(watch.ElapsedMilliseconds)+"s");
						}
						
						SaveOptions();
						if (timeoutMS > 0 && watch.ElapsedMilliseconds > timeoutMS) {
							Debug.WriteLine("Warning: \""+sLine+"\" after SaveOptions took "+msss(watch.ElapsedMilliseconds)+"s");
						}
						
						Common.sParticiple="continuing after SaveOptions";
						tbStatus.Text="Saving options...OK";
						Application.DoEvents();
					}//end if profile folder exists
					else {
						Common.sParticiple="skipping profile folder since doesn't exist";
						tbStatus.Text="Loading Profile...FAIL (missing folder)";
						Application.DoEvents();
						string sMsg="Unable to open profile \"" + sValue + "\"!";
						Console.Error.WriteLine(sMsg);
						MessageBox.Show(sMsg);
					}
					Common.sParticiple="after LoadProfile";
				}//end if (sCommandLower=="LoadProfile")
				else if (sCommandLower=="ulbytecounttotalprocessed") {
					ulByteCountTotalProcessed_LastRun=ulong.Parse(sValue);
				}
				else if (sCommandLower=="exitifnousabledrivesfound") {
					bExitIfNoUsableDrivesFound=ToBool(sValue);
				}
				else if (sCommandLower=="recreatefullpathonbackup") {
					this.recreateFullPathCheckBox.Checked = ToBool(sValue);
				}
				else if (sCommandLower=="alwaysstayopen") {
					bAlwaysStayOpen=ToBool(sValue);
				}
				else if (sCommandLower=="testonly") {
					bTestOnly=ToBool(sValue);
					Output("Test mode turned "+(bTestOnly?"on":"off")+"."+(bTestOnly?"  No files will be copied.":""));
				}
				else if (sCommandLower=="destsubfolder") {
					if (sValue.Trim()=="") {
						DestSubfolderRelNameThenSlash=null;
						LogWriteLine("DestSubfolderRelNameThenSlash:null");
					}
					else {
						DestSubfolderRelNameThenSlash=Common.LocalFolderThenSlash(sValue.Trim());
						LogWriteLine("DestSubfolderRelNameThenSlash:"+DestSubfolderRelNameThenSlash);
					}
				}
				//TODO: else if (sCommandLower=="minimumdate") {
				//	Common.SetMinimumDateToCheckFolder(sValue);
				//}
				else {
					Console.Error.WriteLine("Unknown Command!: "+sCommandLower+":"+sValue);
					bForceBad=true;
				}
				bGood=true;
				if (bForceBad) bGood=false;
			}//end if has ":" in right place
			if (timeoutMS > 0 && watch.ElapsedMilliseconds > timeoutMS) {
				Debug.WriteLine("Warning: \""+sLine+"\" took "+msss(watch.ElapsedMilliseconds)+"s");
			}
			return bGood;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sLine"></param>
		/// <param name="sFile">For debugging purposes</param>
		/// <param name="iLine">For debugging purposes</param>
		/// <returns></returns>
		public bool RunScriptLine(string sLine, bool enableRecreateFullPath, string sFile, int lineNumber, long timeoutMS) {
			bool bForceBad=false;
			bool bGood=false;
			try {
				bGood = this._RunScriptLine(sLine, enableRecreateFullPath, sFile, lineNumber, timeoutMS);
			}
			catch (Exception exn) {
				Common.ShowExn(exn,"parsing line "+Common.SafeString(sLine,true)+" {iLine+1:"+(iLine+1).ToString()+"; status:"+tbStatus.Text+"}");
				try {
					LogWriteLine("RunScriptLine("+((sLine!=null)?("\""+sLine+"\""):"null")+","+(enableRecreateFullPath?"true":"false")+","+((sFile!=null)?("\""+sFile+"\""):"null")+","+lineNumber.ToString());
				}
				catch {}  // doesn't matter
				iCouldNotFinish++;
				throw exn;
			}
			return bGood;
		}//end RunScriptLine
		
		
		
		public void ShowOptions(TableLayoutPanel thisTableLayoutPanel, string thisScriptFile_RelOrFullName) {
			try {
//				for (int rowIndex=optionsTableLayoutPanel.RowCount-2; rowIndex>=0; rowIndex--) {
//					optionsTableLayoutPanel.Controls.RemoveAt(rowIndex);
//				}
				//Control lastRow = thisTableLayoutPanel.Controls[thisTableLayoutPanel.RowCount-1];
				thisTableLayoutPanel.GrowStyle = TableLayoutPanelGrowStyle.AddRows;
				thisTableLayoutPanel.Hide();
				thisTableLayoutPanel.SuspendLayout();  // Saves ~ 15 seconds!
				thisTableLayoutPanel.Controls.Clear();
				thisTableLayoutPanel.RowCount = 0;
				string line=null;
				StreamReader inStream=new StreamReader(thisScriptFile_RelOrFullName);
				int atRowIndex = 0;
				int colCount = thisTableLayoutPanel.ColumnCount;
				thisTableLayoutPanel.Hide();
				string prevousTipText = optionsHelpLabel.Text;
				optionsHelpLabel.Text = "Loading options...";
				bool previousTipVisible = optionsHelpLabel.Visible;
				optionsHelpLabel.Show();
				Application.DoEvents();
				while ( (line=inStream.ReadLine()) != null ) {
					line=line.Trim();
					if (line.Length>0) {
						string commandString=line;
						string valueString="";
						int colonIndex=-1;
						if (!line.StartsWith("#")) colonIndex=line.IndexOf(":");
						if (colonIndex>=0) {
							commandString = line.Substring(0,colonIndex);
							valueString = line.Substring(colonIndex+1);
						}
						thisTableLayoutPanel.RowStyles.Insert(atRowIndex, new RowStyle(SizeType.Absolute, 30f));
						thisTableLayoutPanel.RowCount += 1;
						
						System.Windows.Forms.Label newCommandLabel = new System.Windows.Forms.Label();
						newCommandLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
						newCommandLabel.AutoSize = true;
						newCommandLabel.Location = new System.Drawing.Point(0, 0);
						newCommandLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
						newCommandLabel.Name = "row"+atRowIndex.ToString()+"CommandLabel";
						newCommandLabel.Size = new System.Drawing.Size(88, 19);
						newCommandLabel.TabIndex = 100+atRowIndex*thisTableLayoutPanel.ColumnCount+optionColumnIndex_Command;
						newCommandLabel.Text = commandString;
						newCommandLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
						thisTableLayoutPanel.Controls.Add(newCommandLabel,optionColumnIndex_Command,atRowIndex);//thisCommandCell.Controls.Add(newCommandLabel);
						//Output("Added command cell "+newCommandLabel.Name,true);

						if (!line.StartsWith("#")) {
							System.Windows.Forms.Label newValueLabel = new System.Windows.Forms.Label();
							newValueLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
							newValueLabel.AutoSize = true;
							newValueLabel.Location = new System.Drawing.Point(0, 0);
							newValueLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
							newValueLabel.Name = "row"+atRowIndex.ToString()+"ValueLabel";
							newValueLabel.Size = new System.Drawing.Size(88, 19);
							newValueLabel.TabIndex = 100+atRowIndex*thisTableLayoutPanel.ColumnCount+optionColumnIndex_Value;
							newValueLabel.Text = valueString;
							newValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
							thisTableLayoutPanel.Controls.Add(newValueLabel,optionColumnIndex_Value,atRowIndex);//thisCommandCell.Controls.Add(newCommandLabel);
						
						
							System.Windows.Forms.Button newDeleteButton;
							newDeleteButton = new System.Windows.Forms.Button();
							newDeleteButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
							newDeleteButton.AutoSize = true;
							newDeleteButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
							newDeleteButton.Location = new System.Drawing.Point(0, 0);
							newDeleteButton.Name = "row"+atRowIndex.ToString()+"DeleteButton";
							newDeleteButton.Tag = "DeleteOptionIndex:"+atRowIndex.ToString();
							newDeleteButton.Size = new System.Drawing.Size(75, 21);
							newDeleteButton.TabIndex = 0;
							newDeleteButton.Enabled = clear_buttons_enabled;
							newDeleteButton.Text = "Clear";
							newDeleteButton.UseVisualStyleBackColor = true;
							newDeleteButton.Click += new System.EventHandler(this.AnyRemoveOptionIndexButtonClick);
							optionsTableLayoutPanel.Controls.Add(newDeleteButton, optionColumnIndex_DeleteButton, atRowIndex);
						}
						else {  // Comment in script--still add it, so indices line up
							optionsTableLayoutPanel.RowStyles[atRowIndex].Height=0;
						}
						atRowIndex++;
					}//if line length>0
				}
				optionsHelpLabel.Text = prevousTipText;
				if (!previousTipVisible) optionsHelpLabel.Hide();
				
				inStream.Close();
				//thisTableLayoutPanel.Controls[0] = lastRow;
			}
			catch (Exception exn) {
				string msg="Could not finish ShowOptions:"+Environment.NewLine+exn.ToString();
				Output(msg,true);
				throw exn;
			}
			finally {
				thisTableLayoutPanel.ResumeLayout();
				Application.DoEvents();  // try to prevent layout flicker after show
				thisTableLayoutPanel.Show();
			}
		}//end ShowOptions

		/// <summary>
		/// aka AnyClearButtonClick
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void AnyRemoveOptionIndexButtonClick(object sender, EventArgs e) {
			Button senderButton = sender as Button;
			string line = (string)senderButton.Tag;
			lbOut.Items.Add("Clicked "+line);
			string commandString=line;
			string commandString_ToLower=commandString.ToLower();
			string valueString="";
			int colonIndex=line.IndexOf(":");
			if (colonIndex>=0) {
				commandString=line.Substring(0,colonIndex);
				commandString_ToLower=commandString.ToLower();
				valueString=line.Substring(colonIndex+1);
				int rowIndex=int.Parse(valueString);
				if (commandString_ToLower=="deleteoptionindex") {
					Label commandLabel=(Label)optionsTableLayoutPanel.Controls["row"+valueString+"CommandLabel"];
					if (!commandLabel.Text.StartsWith("#\\0")) commandLabel.Text="#\\0"+commandLabel.Text;
					commandLabel.Visible=false;
					optionsTableLayoutPanel.Controls["row"+valueString+"ValueLabel"].Visible=false;
					optionsTableLayoutPanel.Controls["row"+valueString+"DeleteButton"].Enabled=false;
					optionsTableLayoutPanel.Controls["row"+valueString+"DeleteButton"].Visible=false;
					optionsTableLayoutPanel.RowStyles[rowIndex].Height=0;
				}
			}
			SaveOptions(new string[]{"#\\0"});
		}//end AnyDeleteOptionIndexButtonClick
		
		public static readonly string prefsVariableName="preferenceLevel";
		public static readonly string prefsFile_Name=".BackupGoNow-settings.txt";
		/// <summary>
		/// 
		/// </summary>
		/// <param name="thisLocInfo"></param>
		/// <returns>The preference level, or long.MinValue if can't read preferences</returns>
		public static long getPreferenceLevelInPrefsFile(LocInfo thisLocInfo) {
			long result=long.MinValue;
			string resultAsString=null;
			try {
				StreamReader prefsStreamReader=new StreamReader(thisLocInfo.DriveRoot_FullNameThenSlash+prefsFile_Name);
				string line=null;
				string prefsVariableName_ToLower=prefsVariableName.ToLower();
				while ( (line=prefsStreamReader.ReadLine()) != null ) {
					string line_ToLower=line.ToLower();
					
					if (line_ToLower.StartsWith(prefsVariableName_ToLower+":")) {
						resultAsString=line.Substring(prefsVariableName.Length+1);
						long tryValue=long.MinValue;
						bool IsParsed=long.TryParse(resultAsString, out tryValue);
						if (IsParsed) result=tryValue;
					}
				}
				prefsStreamReader.Close();
			}
			catch (Exception exn) {
				//Console.WriteLine("WARNING: ");
				//don't care
			}
			return result;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="thisLocInfo"></param>
		/// <returns>Destination is writable and prefs file was written</returns>
		public static bool setPreferenceLevelInPrefsFile(LocInfo thisLocInfo, long preferenceLevel) {
			bool IsWritten=false;
			try {
				StreamWriter prefStream=new StreamWriter(thisLocInfo.DriveRoot_FullNameThenSlash+prefsFile_Name);
				prefStream.WriteLine(prefsVariableName+":"+preferenceLevel);
				prefStream.Close();
				IsWritten=true;
			}
			catch {} //don't care
			return IsWritten;
		}

		void Console_Error_WriteLine_AllDebugInfo() {
			Console.Error.WriteLine("{\n PseudoRoot entry count:"+Common.GetPseudoRoots_EntriesCount().ToString()
									+";\n PseudoRoot non-null entries:"+Common.GetPseudoRoots_CountNonNull(false).ToString()
									+";\n PseudoRoot non-null entries including entries past end:"+Common.GetPseudoRoots_CountNonNull(true).ToString()
									+";\n PseudoRoot Array"+Common.GetPseudoRootArrayMsg_LengthColonCount_else_ColonNull()
									//+";\n SafeCount(alSelectableDrives):"+Common.SafeCount(alSelectableDrives).ToString()
									//+";\n SafeCount(alPseudoRootsNow):"+Common.SafeCount(alPseudoRootsNow).ToString()
									+";\n Selectable Drive Array"+Common.GetSelectableDriveArrayMsg_LengthColonCount_else_ColonNull()
									+";\n Selectable Drive entries:"+Common.GetSelectableDriveMsg_EntriesCount().ToString()
									+";\n Selectable Drive non-null entries:"+Common.GetSelectableDrives_CountNonNull(false).ToString()
									+";\n Selectable Drive non-null entries including entries past end:"+Common.GetSelectableDrives_CountNonNull(true).ToString()
									+";\n Invalid Destinations ArrayList:"+Common.ToString(Common.GetInvalidDrivesList(),"   ")
								+"\n}");
	
		}
		
		void TableLayoutPanel1Paint(object sender, PaintEventArgs e) {
			
		}
		

		
		void WriteLastRunLog(string sOnlyDateAndThisText) {
			try {
				StreamWriter outStream=new StreamWriter(LastRunLog_FullName);
				DateTime dtNow=DateTime.Now;
				outStream.WriteLine("# "+sMyNameAndVersion+" (early-quit message only) "+dtNow.Year+"-"+dtNow.Month+"-"+dtNow.Day+" "+dtNow.Hour+":"+dtNow.Minute+":"+dtNow.Second);
				outStream.WriteLine(sOnlyDateAndThisText);
				outStream.Close();
			}
			catch (Exception exn) {
				Console.Error.WriteLine();
				Console.Error.WriteLine("Could not finish :"+exn.ToString());
			}
		}
		void WriteLastRunLog() {
			try {
				StreamWriter outStream=new StreamWriter(LastRunLog_FullName);
				DateTime dtNow=DateTime.Now;
				outStream.WriteLine("# "+sMyNameAndVersion+"  "+dtNow.Year+"-"+dtNow.Month+"-"+dtNow.Day+" "+dtNow.Hour+":"+dtNow.Minute+":"+dtNow.Second);
				//outStream.WriteLine("Could not copy:");
				//outStream.WriteLine(sFileList); //NOTE: lbOut includes "(could not copy)" messages
				//outStream.WriteLine();
				outStream.WriteLine("Output:");
				for (int i=0; i<this.lbOut.Items.Count; i++) {
					outStream.WriteLine(this.lbOut.Items[i].ToString());
				}
				outStream.Close();
			}
			catch (Exception exn) {
				Console.Error.WriteLine();
				Console.Error.WriteLine("Could not finish WriteLastRunLog:"+exn.ToString());
			}
		}//end WriteLastRunLog
		
		
		
		void MenuitemEditMainClick(object sender, EventArgs e) {
			try {
				string file_FullName=Path.Combine(BackupProfileFolder_FullName,MainScriptFile_Name);
				DialogResult thisDR=MessageBox.Show(Common.LimitedWidth("In order for any changes you make to the file that is about to open (\""+file_FullName+"\") to take effect, you must save it, close "+sMyName+" then open the "+sMyName+" icon again.",40,"\n",true), sMyName, MessageBoxButtons.OKCancel);
				if (thisDR==DialogResult.OK) System.Diagnostics.Process.Start(file_FullName);
			}
			catch (Exception exn) {
				Common.ShowExn(exn,"opening "+Common.SafeString(MainScriptFile_Name,true)+" for profile","menuitemEditMainClick");
			}
		}//end MenuitemEditMainClick
		
		void MenuitemEditScriptClick(object sender, EventArgs e) {
			try {
				string file_FullName=Path.Combine(BackupProfileFolder_FullName,BackupScriptFile_Name);
				DialogResult thisDR=MessageBox.Show(Common.LimitedWidth("The file that is about to open (\""+file_FullName +"\") must be saved before pressing \"Go\" (in "+sMyName+") in order for any changes you make to be used.",40,"\n",true), sMyName, MessageBoxButtons.OKCancel);
				if (thisDR==DialogResult.OK) System.Diagnostics.Process.Start(file_FullName);
			}
			catch (Exception exn) {
				Common.ShowExn(exn,"opening \""+BackupScriptFile_Name+"\" in profile","menuitemEditScriptClick");
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
						if (bTestOnly) Output(sListedItem, true);
						if (bUserCancelledLastRun||bDiskFullLastRun) break; //do NOT stop if Copy Error only
					}//end if not excluded
					if (bUserCancelledLastRun) break;
					BackupFile(fiNow.FullName, true, fiNow)
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
				System.Diagnostics.Process.Start(LastRunLog_FullName);
			}
			catch {}
		}
		void writeDefault_StartupScript(string filename) {
			StreamWriter mainStream=null;
			try {
				mainStream = new StreamWriter(filename);
				mainStream.WriteLine(@"LoadProfile:BackupGoNowDefault");
				mainStream.Close();
			}
			catch (Exception exn) {
				throw exn;
			}
			finally {
				mainStream.Close();
			}
		}
		
		void writeDefault_MainScript(string filename) {
			StreamWriter mainStream=null;
			try {
				mainStream=new StreamWriter(filename);
				mainStream.WriteLine(@"ExcludeDest:Windows8_OS");
				mainStream.WriteLine(@"ExcludeDest:LENOVO");
				mainStream.WriteLine(@"ExcludeDest:HP_RECOVERY");
				mainStream.WriteLine(@"ExcludeDest:RECOVERY");
				mainStream.WriteLine(@"ExcludeDest:OS");
				mainStream.WriteLine(@"ExcludeDest:FACTORY_IMAGE");
				mainStream.WriteLine(@"ExcludeDest:Recovery");
				mainStream.WriteLine(@"ExcludeDest:DELLUTILITY");
                try {
					if (Directory.Exists ("C:\\")) mainStream.WriteLine ("ExcludeDest:C:\\");
					//some versions of .NET seem to crash if unix-like path is checked on Windows 
					if (Directory.Exists ("/")) mainStream.WriteLine ("ExcludeDest:/");
					if (Directory.Exists("/sys")) mainStream.WriteLine("ExcludeDest:/sys");
					if (Directory.Exists ("/home")) mainStream.WriteLine("ExcludeDest:/home");
				}
                catch {
                    //don't care
                }
				mainStream.WriteLine(@"ExitIfNoUsableDrivesFound:yes");
				mainStream.WriteLine(@"AlwaysStayOpen:no");
				mainStream.Close();
			}
			catch {}//don't care
		}
		void writeDefault_BackupScript(string filename) {
			StreamWriter backupStream=null;
			try {
				backupStream=new StreamWriter(filename);
				backupStream.WriteLine(@"# This file was GENERATED by "+sMyName);
				backupStream.WriteLine(@"AddFolder:%APPDATA%\"+sMyName);
				backupStream.WriteLine();
				backupStream.WriteLine(@"Exclude:$RECYCLE.BIN");
				backupStream.WriteLine(@"Exclude:*.tmp");
				backupStream.WriteLine(@"Exclude:Temp");
				backupStream.WriteLine(@"Exclude:Cache");
				backupStream.WriteLine(@"Exclude:Temporary Internet Files");
				backupStream.WriteLine(@"Exclude:Media Cache");
				backupStream.WriteLine(@"Exclude:blob_storage"); //%LOCALAPPDATA%\Google\Chrome\User Data\Default\blob_storage
				backupStream.WriteLine(@"Exclude:Thumbs.db");
				backupStream.WriteLine(@"Exclude:.git");
				backupStream.WriteLine(@"Exclude:__pycache__");
				backupStream.WriteLine(@"Exclude:*.pyc");
				backupStream.WriteLine(@"Exclude:NTUSER.DAT");
				backupStream.WriteLine(@"Exclude:UsrClass.dat");
				backupStream.WriteLine(@"Exclude:UsrClass.dat.LOG");
				backupStream.WriteLine(@"Exclude:ntuser.dat.LOG");
				backupStream.WriteLine(@"ExcludeFolderFullname:%APPDATA%\Mozilla\Firefox\Profiles\*\crashes");
				backupStream.WriteLine(@"ExcludeFolderFullname:%APPDATA%\Mozilla\Firefox\Profiles\*\datareporting");
				backupStream.WriteLine(@"ExcludeFolderFullname:%APPDATA%\Mozilla\Firefox\Profiles\*\extensions");
				backupStream.WriteLine(@"ExcludeFolderFullname:%APPDATA%\Mozilla\Firefox\Profiles\*\features");
				backupStream.WriteLine(@"ExcludeFolderFullname:%APPDATA%\Mozilla\Firefox\Profiles\*\gmp*");
				backupStream.WriteLine(@"ExcludeFolderFullname:%APPDATA%\Mozilla\Firefox\Profiles\*\healthreport");
				backupStream.WriteLine(@"ExcludeFolderFullname:%APPDATA%\Mozilla\Firefox\Profiles\*\minidumps");
				backupStream.WriteLine(@"ExcludeFolderFullname:%APPDATA%\Mozilla\Firefox\Profiles\*\saved-telemetry-pings");
				backupStream.WriteLine(@"ExcludeFolderFullname:%APPDATA%\Mozilla\Firefox\Profiles\*\searchplugins");
				backupStream.WriteLine(@"ExcludeFolderFullname:%APPDATA%\Mozilla\Firefox\Profiles\*\WOT");
				backupStream.WriteLine(@"ExcludeFolderFullname:%APPDATA%\Thunderbird\Profiles\*\extensions");
				backupStream.WriteLine();
				backupStream.WriteLine(@"#AddFolder:%USERPROFILE%");
				backupStream.WriteLine(@"AddFolder:%USERPROFILE%\.minetest");
				backupStream.WriteLine(@"AddFolder:%USERPROFILE%\.technic");
				backupStream.WriteLine(@"AddFolder:%USERPROFILE%\Favorites");
				backupStream.WriteLine(@"AddFolder:%USERPROFILE%\Links");
				backupStream.WriteLine(@"AddFolder:%USERPROFILE%\Music");
				backupStream.WriteLine(@"AddFolder:%USERPROFILE%\Pictures");
				backupStream.WriteLine(@"AddFolder:%USERPROFILE%\Videos");
				backupStream.WriteLine();
				backupStream.WriteLine(@"AddFolder:%MYDOCS%");
				backupStream.WriteLine(@"AddFolder:%DESKTOP%");
				backupStream.WriteLine();
				backupStream.WriteLine(@"Exclude:Crash Reports");
				backupStream.WriteLine();
				backupStream.WriteLine(@"AddFolder:%APPDATA%\ASUS");
				backupStream.WriteLine(@"AddFolder:%APPDATA%\ASUS WebStorage");
				backupStream.WriteLine(@"AddFolder:%APPDATA%\Corel");
				backupStream.WriteLine(@"AddFolder:%APPDATA%\IrfanView");
				backupStream.WriteLine(@"AddFolder:%APPDATA%\LeaderTech");
				backupStream.WriteLine(@"AddFolder:%APPDATA%\MAGIX");
				backupStream.WriteLine(@"AddFolder:%APPDATA%\Mozilla\Firefox");
				backupStream.WriteLine(@"AddFolder:%APPDATA%\NAPS2");
				backupStream.WriteLine(@"AddFolder:%APPDATA%\NuGet");
				backupStream.WriteLine(@"AddFolder:%APPDATA%\Skype");
				backupStream.WriteLine(@"AddFolder:%APPDATA%\Sony Corporation");
				backupStream.WriteLine(@"AddFolder:%APPDATA%\Thunderbird");
				backupStream.WriteLine();
				backupStream.WriteLine(@"AddFolder:%LOCALAPPDATA%\Asus");
				backupStream.WriteLine(@"AddFolder:%LOCALAPPDATA%\CEF");
				backupStream.WriteLine(@"ExcludeFolderFullname:%LOCALAPPDATA%\Google\Chrome\User Data\Default\Extensions");
				backupStream.WriteLine(@"AddFolder:%LOCALAPPDATA%\Google\Chrome\User Data\Default");
				backupStream.WriteLine(@"AddFolder:%LOCALAPPDATA%\IM");
				backupStream.WriteLine(@"AddFolder:%LOCALAPPDATA%\Skype");
				backupStream.WriteLine(@"AddFolder:%LOCALAPPDATA%\SkypePlugin");
				backupStream.WriteLine(@"AddFolder:%LOCALAPPDATA%\Thunderbird");
				backupStream.WriteLine(@"AddFolder:%LOCALAPPDATA%\Vosteran");
				backupStream.Close();
			}
			catch {}//don't care
		}
		public static FileInfo[] get_deepest_fis(DirectoryInfo di) {
			FileInfo[] fis=null;
			DirectoryInfo[] dis=di.GetDirectories();
			if (dis==null||dis.Length==0) {
				fis=di.GetFiles();
			}
			else {
				fis=get_deepest_fis(dis[0]);
			}
			return fis;
		}
		void GoButtonClick(object sender, EventArgs e)
		{
			if (this.destinationComboBox.Text!="") {
				
				//fix errant retroactive backup folders from old versions:
				string sDestPrefix=Common.LocalFolderThenSlash(DestinationDriveRootDirectory_FullName_OrSlashIfRootDir);
				if (DestSubfolderRelNameThenSlash!=null)
					sDestPrefix += DestSubfolderRelNameThenSlash;
				DirectoryInfo dest_root_di=new DirectoryInfo(sDestPrefix);
				DirectoryInfo[] dis=null;
				int check_bad_retro_count=0;
				if (dest_root_di.Exists) {
					dis=dest_root_di.GetDirectories();
					//NOTE: last letter in each bad_name_examples is drive letter!
					string[] bad_name_examples={"2017M4d9H251SSC","2017M12d12H251SSC","2017M12d9H251SSC","2017M9d12H251SSC",
												"2017M4d9H1251SSC","2017M12d12H1251SSC","2017M12d9H1251SSC","2017M9d12H1251SSC"};
					foreach (DirectoryInfo di in dis) {
						check_bad_retro_count+=1;
						// 2017M4d9H251SSC should be 2017/04/09/0251?? where ?? is second
						bool bad_enable=false;
						string source_drive_letter=null;
						for (int i=0; i<bad_name_examples.Length; i++) {
							if (di.Name.Length==bad_name_examples[i].Length) {
								int bad_M=bad_name_examples[i].IndexOf("M");
								int bad_d=bad_name_examples[i].IndexOf("d");
								int bad_H=bad_name_examples[i].IndexOf("H");
								if ( di.Name.Substring(0,di.Name.Length-1).EndsWith("SS")
									&& di.Name[bad_M]==bad_name_examples[i][bad_M]
								    && di.Name[bad_d]==bad_name_examples[i][bad_d]
								    && di.Name[bad_H]==bad_name_examples[i][bad_H]
								   ) {
									source_drive_letter=di.Name.Substring(di.Name.Length-1);
									bad_enable=true;
									break;
								}
							}
						}
						if (bad_enable) {
							Console.Error.WriteLine("bad_retroactive_folder: "+di.FullName);
							FileInfo[] fis=get_deepest_fis(di);
							if (fis!=null) {  // not empty folder
								foreach (FileInfo fi in fis) {
									string dated_path=get_retroactive_timed_folder_partialpath_from_UTC(fi.LastWriteTimeUtc);
									while (dated_path.EndsWith(Common.sDirSep)) dated_path=dated_path.Substring(0, dated_path.Length-1);
									while (dated_path.StartsWith(Common.sDirSep)) dated_path=dated_path.Substring(1);
									dated_path+=Common.sDirSep+source_drive_letter;
									string new_dir_path = null;
									try {
										new_dir_path=fi.Directory.FullName.Replace(di.Name, dated_path);
									}
									catch (System.IO.PathTooLongException innerEx) {
										// TODO handle this
										Console.Error.WriteLine("bad_dated_path:"+dated_path);
										continue;
									}
									new_dir_path.Replace(Common.sDirSep+Common.sDirSep, Common.sDirSep);
									if (new_dir_path.EndsWith(Common.sDirSep)) new_dir_path=new_dir_path.Substring(0, new_dir_path.Length-1);
									string new_path=Path.Combine(new_dir_path, fi.Name);
									Console.Error.WriteLine("move_bad_retroactive_from:"+fi.FullName);
									Console.Error.WriteLine("move_bad_retroactive_to:"+new_path);
									try {
										Directory.CreateDirectory(new_dir_path);
										fi.Attributes=FileAttributes.Normal;
										fi.MoveTo(new_path);
									}
									catch (Exception exn) {
										Console.Error.WriteLine("Could not finish correcting bad retroactive path: "+exn.ToString());
									}
								}
							}
							try {
								Console.Error.WriteLine("move_bad_empty_retroactive_path:"+di.FullName);
								di.Delete(true);
							}
							catch (Exception exn) {
								Console.Error.WriteLine("Could not finish removing bad empty retroactive path: "+exn.ToString());
							}
						}
						else {
							Console.Error.WriteLine("ok_retroactive_folder: "+di.FullName);
						}
					}
				}
				if (check_bad_retro_count==0) {
					Console.Error.WriteLine("retroactive_folder_check_failed_or_none_to_check_for: "+dest_root_di.FullName);
				}
				is_first_overlimit=true;
				//if (destinationComboBox.SelectedIndex>=0) {
				int thisRootNumber=Common.GetPseudoRootIndex_ByCustomInt(destinationComboBox.SelectedIndex);
				if (thisRootNumber>=0 && thisRootNumber!=preferenceValueBest_DriveIndex) {
					LocInfo thisLocInfo=Common.GetPseudoRoot(thisRootNumber);
					if (thisLocInfo!=null) {
						long preferenceValue=thisLocInfo.CustomLong;
						bool IsPreferenceSaveNeeded=false;
						if (preferenceValue<1) {
							preferenceValue=1;
							IsPreferenceSaveNeeded=true;
						}
						else if (preferenceValue==preferenceValueBest_Value && thisRootNumber!=preferenceValueBest_DriveIndex) {
							preferenceValue+=1;
							IsPreferenceSaveNeeded=true;
						}
						else if (preferenceValue<=preferenceValueBest_Value) {
							preferenceValue=preferenceValueBest_Value+1;
							IsPreferenceSaveNeeded=true;
						}
						if (IsPreferenceSaveNeeded) {
							bool IsWritten=setPreferenceLevelInPrefsFile(thisLocInfo,preferenceValue);
							thisLocInfo.CustomLong=preferenceValue;
							preferenceValueBest_DriveIndex=thisRootNumber;
							preferenceValueBest_Value=preferenceValue;
							preferenceValueBest_InitiallyChosen_Index=thisRootNumber;//in case of duplicate preference, prevent re-incrementing (already incremented above)
						}
					}
					else {
						Console.Error.WriteLine("Error saving preferences: LocInfo was null at index ["+thisRootNumber.ToString()+"]");
					}
				}
				//}
				goButton.Enabled=false;
				menuitemEditMain.Enabled=false;
				menuitemEditScript.Enabled=false;
				menuitemHelp_ViewOutputOfLastRun.Enabled=false;
				bUserCancelledLastRun=false;
				bCopyErrorLastRun=false;
				bDiskFullLastRun=false;
				//iFilesTooBigToFitLastRun//bFileTooBigToFitLastRun=false;
				bool bGood=true;
				
				//if (!FixSlashes()) bGood=false;
				//if (!SetDestFolder(destinationComboBox.Text)) bGood=false; //instead of this, path is already assured to be good since it is in the list, and ulByteCountDestTotalSize & ulByteCountDestAvailableFreeSpace are set by the IndexedChanged event handler
				
				if (bDeleteFilesNotOnSource_BeforeBackup) {
					//string sDestPrefix=Common.LocalFolderThenSlash(DestinationDriveRootDirectory_FullName_OrSlashIfRootDir)
					//    + ((DestSubfolderRelNameThenSlash!=null)?DestSubfolderRelNameThenSlash:"");
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
				if (!RunScript(Path.Combine(MainForm.BackupProfileFolder_FullName,BackupScriptFile_Name), recreateFullPathCheckBox.Checked, -1)) bGood=false;
				if (mainformNow.progressbarMain.Style==ProgressBarStyle.Marquee) {
					mainformNow.progressbarMain.Style=ProgressBarStyle.Continuous;
					mainformNow.progressbarMain.Value=bGood?mainformNow.progressbarMain.Maximum:(mainformNow.progressbarMain.Maximum/2);
				}
				
				Output((ulByteCountTotalProcessed/1024/1024/1024).ToString()+"GB = "+(ulByteCountTotalProcessed/1024/1024).ToString()+"MB = "+(ulByteCountTotalProcessed/1024).ToString()+"KB = "+ulByteCountTotalProcessed.ToString()+"bytes of "+(ulByteCountTotal/1024/1024).ToString()+"MB source data finished, "+(ulByteCountTotalActuallyCopied/1024/1024).ToString()+"MB difference copied).",true);
				try {
					if (bGood&&!MainForm.bUserCancelledLastRun) {
						Output("Opening log \""+Path.Combine(BackupProfileFolder_FullName, LogFile_Name)+"\"",true);
						StreamWriter outStream = new StreamWriter(Path.Combine(BackupProfileFolder_FullName, LogFile_Name));
						Output("Writing log (statistics)...",true);
						outStream.WriteLine("ulByteCountTotalProcessed:"+ulByteCountTotalProcessed.ToString());
						outStream.Close();
						Output("Writing log (statistics)...OK ("+LogFile_Name+")",true);
					}
				}
				catch (Exception exn) {
					Console.Error.WriteLine();
				}
				int iMessages=0;
				if (!bGood) {
					lbOut.Items.Add("Some files may be system files and are not required to be backed up, however RunScript failed.");
					iMessages++;
					Application.DoEvents();
					WriteLastRunLog();
					menuitemHelp_ViewOutputOfLastRun.Enabled=true;
					FileInfo fiSaved=new FileInfo(LastRunLog_FullName);
					//DialogResult dlgresultNow=MessageBox.Show("Finished.\n\nLog ("+iMessages.ToString()+" message(s)) saved to \""+fiSaved.FullName+"\"\n\n  Do you wish to to review the list?","Result", MessageBoxButtons.YesNo);
					MessageBox.Show("Finished.\n\nLog ("+iMessages.ToString()+" messages(s)) about certain files are listed near the end of \""+fiSaved.FullName+"\"",sMyName);//DialogResult dlg=MessageBox.Show(sFileList+"\n\n  Do you wish to to review the list (press cancel to exit)?","Result", MessageBoxButtons.OKCancel);
					try {
						System.Diagnostics.Process.Start(fiSaved.FullName);
					}
					catch {}					
					//asdf
					//if (dlgresultNow==DialogResult.Yes) {
					//	System.Diagnostics.Process.Start(sLastRunLog);
					//}
					
					this.goButton.Enabled = true;
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
						FileInfo fiSaved=new FileInfo(LastRunLog_FullName);
						string partialMsg="";
						if (sShowError.Length>0) {
							partialMsg=" with error: "+sShowError;
						}
						//if (alCopyError.Count>0) {
						//	partialMsg+=" with "+alCopyError.Count.ToString()+" error(s) starting with \n\"" +
						//		(string)alCopyError[0]+"\"";
						//}
						DialogResult answer = MessageBox.Show(
							"Finished Backup" + partialMsg + ".\n\nLog ("+iMessages.ToString()+" message(s)) saved to \""+fiSaved.FullName+"\"",
							sMyName,
							MessageBoxButtons.OK,
							MessageBoxIcon.Information
						);//DialogResult dlg=MessageBox.Show(sFileList+"\n\n  Do you wish to to review the list (press cancel to exit)?","Result", MessageBoxButtons.OKCancel);
						if (answer == DialogResult.Cancel) bUserSaysStayOpen=true; // deprecated
						else bUserSaysStayOpen=false;
					}//end if bCopyErrorLastRun
					else {
						WriteLastRunLog();
						FileInfo fiSaved=new FileInfo(LastRunLog_FullName);
						if (iMessages<=0) MessageBox.Show("Finished Backup.");//\n\nLog saved to \""+fiSaved.FullName+"\"",sMyName);
						else MessageBox.Show("Finished Backup.\n\nLog ("+iMessages.ToString()+" message(s)) saved to \""+fiSaved.FullName+"\"",sMyName);
					}
					if ((!bCopyErrorLastRun&&!bUserSaysStayOpen) && !bTestOnly && !bAlwaysStayOpen)
						Application.Exit(); // && !bCopyErrorLastRun
				}//end else bGood
				menuitemEditMain.Enabled=true;
				menuitemEditScript.Enabled=true;
			}
			else {
				MessageBox.Show(Common.LimitedWidth("No destination drive is present.  Connect a flash drive, external hard drive, or other backup media and open the "+sMyName+" icon again.",40,"\n",true));
			}			
		}//GoButtonClick
		
		
		
		void CancelButtonClick(object sender, EventArgs e)
		{
			//if (bBusyCopying) {
			//	menuitemCancel.Enabled=false;
			//	bUserCancelledLastRun=true;
			//}
			//else {
			if (!bUserCancelledLastRun) {
				//if (flisterNow!=null&&flisterNow.IsBusy) flisterNow.Stop();
				cancelButton.Enabled=false;
				bUserCancelledLastRun=true;
			}
			//}
		}
		
		bool EditScriptLine(string path, string key, string value) {
			string assignmentOp = ":";
			if (string.IsNullOrWhiteSpace(key)) {
				throw new ApplicationException("key is null or blank");
			}
			if (key.Contains(assignmentOp)) {
				throw new ApplicationException("key \""+key+"\" contains \""+assignmentOp+"\"");
			}
			bool bSuccessFullyResetStartup = false;
			bool bFoundLoadProfile = false;
			// FileInfo fi = new FileInfo(path);
			StreamReader streamIn = null;
			StreamWriter outStream = null;
			string tmpPath = path + ".tmp";
			try {
				string sMsg = "Attempting to edit "+Common.SafeString(StartupFile_FullName,true)+" and add \"LoadProfile:"+value+"\" if no valid LoadProfile statement is found...";
				tbStatus.Text = sMsg;
				Application.DoEvents();
				Console.Error.Write(sMsg);
				Console.Error.Flush();
				Console.Error.Write("reading data...");
				Console.Error.Flush();
				Debug.WriteLine("Opening \""+tmpPath+"\" for "+Common.sParticiple);
				outStream = new StreamWriter(tmpPath);
				if (File.Exists(path)) {
					Debug.WriteLine("Opening \""+path+"\" for "+Common.sParticiple);
					streamIn = new StreamReader(path);
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
						if (sLine.ToLower().Trim().StartsWith(key.ToLower()+assignmentOp)) {
							// &&sLine.ToLower()!="loadprofile:"
							// TODO: ^ why was this && check here??
							outStream.WriteLine(key+assignmentOp+value);
							bFoundLoadProfile=true;
						}
						else {
							outStream.WriteLine(sLine);
						}
						// value is written below if key is not found
						// (including if original file is not present)
					}
					Debug.WriteLine("Closing \""+path+"\" for "+Common.sParticiple);
					streamIn.Close();
					streamIn = null;
				}
				if (!bFoundLoadProfile) {
					Console.Error.Write("writing data...");
					Console.Error.Flush();
					//System.Threading.Thread.Sleep(100);//wait for file to be ready (is this ever needed???)
					outStream.WriteLine(key+":"+value);
					Console.Error.WriteLine("OK.");
				}
				Debug.WriteLine("Closing \""+tmpPath+"\" for "+Common.sParticiple);
				outStream.Close();
				outStream = null;
				if (File.Exists(path)) {
					Debug.WriteLine("Deleting old \""+path+"\" for "+Common.sParticiple);
					// File.Move throws exception if destination exists.
					File.Delete(path); // triggers file in use exception due to fi! So use fi.Delete:
					// fi.Delete();
				}
				Debug.WriteLine("Moving \""+tmpPath+"\" to \""+path+"\" for "+Common.sParticiple);
				File.Move(tmpPath, path);
				bSuccessFullyResetStartup = true;
			}
			catch (Exception exn) {
				Common.ShowExn(exn, "creating "+Common.SafeString(StartupFile_Name,true), System.Reflection.MethodBase.GetCurrentMethod().Name);
				throw exn;
			}
			finally {
				if (streamIn != null) streamIn.Close();
				if (outStream != null) outStream.Close();
			}
			return bSuccessFullyResetStartup;
		}
				
		private static bool IsStartupStarted=false;
		/// <summary>
		/// Timer tick event for delayed startup (wait for gui to load before parsing and showing options which is slow)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void StartupTimerTick(object sender, EventArgs e)
		{
			if (IsStartupStarted) {
				Debug.WriteLine("Warning: skipping startup since already started (startup timer ticked again).");
				return;
			}
			// FIXME: Load startup file *before* deciding which profile to load.
			IsStartupStarted=true;
			startupTimer.Stop();
			bool startupIsNew = false; // no profile was chosen. This stores the name.
			if (!File.Exists(StartupFile_FullName)) {
				writeDefault_StartupScript(StartupFile_FullName);
				startupIsNew = true;
			}
			bool backupIsNew = CreateDefaultConfiguration();
			bool machineScriptIsNew = CreateMachineConfiguration();
			
			DateTime dtNow=DateTime.Now;
			lbOutNow.Items.Add(dtNow.Year+"-"+dtNow.Month+"-"+dtNow.Day+" "+dtNow.Hour+":"+dtNow.Minute);
			optionsHelpLabel.Visible=false;
			
			string sMsg="";
			tbStatus.Text="Running startup script (please wait)...";
			Application.DoEvents();
			Debug.WriteLine("Checking for "+profilesFolder_FullName);
			DirectoryInfo diProfiles = new DirectoryInfo(profilesFolder_FullName);
			if (diProfiles.Exists) {
				this.profileCB.SuspendLayout();
				this.profileCB.Items.Clear();
				Debug.WriteLine("Loading "+profilesFolder_FullName);
				DirectoryInfo[] files = diProfiles.GetDirectories();
				// Debug.WriteLine("iterating "+profilesFolder_FullName);
				foreach (DirectoryInfo file in files) {
					Debug.WriteLine("checking for "+file.Name);
					if (!this.profileCB.Items.Contains(file.Name)) {
						this.profileCB.Items.Add(file.Name);
					}
				}
				ExpandProfileDropDown();				
			}
			this.profileCB.ResumeLayout();
			
			Console.Error.WriteLine("Timed startup is about to open " + Common.SafeString(StartupFile_FullName,true));
			RunScript(StartupFile_FullName, recreateFullPathCheckBox.Checked, 1000);
			
			this.profileLabel.Visible=true;
			Debug.WriteLine("Finished " + Common.SafeString(StartupFile_Name,true)+" in MainFormLoad");
			bFoundLoadProfile=false;
			bSuccessFullyResetStartup=false;
			sMsg="Attempting to get path of current user's Documents...";
			tbStatus.Text=sMsg;
			Application.DoEvents();
			sMsg="Welcome to "+sMyName+"!";
			tbStatus.Text=sMsg;
			try {
				if (!File.Exists(LastRunLog_FullName)) {
					menuitemHelp_ViewOutputOfLastRun.Enabled=false;
				}
			}
			catch {}
			
			if (!bLoadedProfile) { // startup.ini was missing a LoadProfile line
				// (RunScript(StartupFile_FullName ... ) has missing/failed LoadProfile, so:
				MessageBox.Show(
					"The "+StartupFile_Name+" didn't set a profile, so using "+Environment.MachineName,
					"Backup GoNow Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
				// This is for the case when startup.ini is corrupt (missing LoadProfile)!
				string fallbackProfile = Environment.MachineName;
				Console.Error.WriteLine(Common.SafeString(StartupFile_Name,true)+" did not load a profile so loading default (\""+DefaultProfile_Name+"\")");
				bool bTest = RunScriptLine("LoadProfile:"+fallbackProfile, recreateFullPathCheckBox.Checked, "<automation in StartupTimerTick>", -1, 500);
				Debug.WriteLine("Loading Profile \""+fallbackProfile+"\"..."+(bTest?"OK":"FAILED!")+" ProfileName="+ProfileName);
				Debug.WriteLine("Editing "+StartupFile_FullName+" in "+System.Reflection.MethodBase.GetCurrentMethod().Name);
				EditScriptLine(StartupFile_FullName, "LoadProfile", fallbackProfile);
				Debug.WriteLine("Done editing "+StartupFile_FullName+" in "+System.Reflection.MethodBase.GetCurrentMethod().Name);
				//System.Threading.Thread.Sleep(500);
				tbStatus.Text="Done checking startup {bLoadedProfile:"+(bLoadedProfile?"yes":"no")+"; bFoundLoadProfile:"+(bFoundLoadProfile?"yes":"no")+"; bSuccessFullyResetStartup:"+(bSuccessFullyResetStartup?"yes":"no")+"}.";
			}//end if !bLoadedProfile
			//alPseudoRootsNow=Common.PseudoRoots_DriveRootFullNameThenSlash_ToArrayList();
			//alSelectableDrives=Common.SelectableDrives_DriveRootFullNameThenSlash_ToArrayList();
			if (Common.GetPseudoRoots_EntriesCount()>0) {//if (alPseudoRootsNow!=null && alPseudoRootsNow.Count>0) {
				if (bExitIfNoUsableDrivesFound&&(Common.GetSelectableDriveMsg_EntriesCount()<=0) && !bAlwaysStayOpen) //if (bExitIfNoUsableDrivesFound&&(alSelectableDrives==null||alSelectableDrives.Count==0) && !bAlwaysStayOpen)
					Application.Exit();
			}
			else if (!bTestOnly&&!bAlwaysStayOpen)
				Application.Exit();
			
			CalculateMargins();
			UpdateSize();
			optionsHelpLabel.Visible=true;
		}//end StartupTimerTick
		
		public bool CreateDefaultConfiguration() {
			bool backupIsNew = false;
			string defaultProfileFolder_FullName=Path.Combine(profilesFolder_FullName,"BackupGoNowDefault");
			if (!Directory.Exists(defaultProfileFolder_FullName)) {
				Directory.CreateDirectory(defaultProfileFolder_FullName);
				backupIsNew = true;
			}
			string defaultMainScriptFile_FullName=Path.Combine(defaultProfileFolder_FullName,MainScriptFile_Name);
			if (!File.Exists(defaultMainScriptFile_FullName)) {
				writeDefault_MainScript(defaultMainScriptFile_FullName);
				backupIsNew = true;
			}
			string defaultBackupScriptFile_FullName=Path.Combine(defaultProfileFolder_FullName,BackupScriptFile_Name);
			if (!File.Exists(defaultBackupScriptFile_FullName)) {
				writeDefault_BackupScript(defaultBackupScriptFile_FullName);
				backupIsNew = true;
			}
			return backupIsNew;
		}
		
		public bool CreateMachineConfiguration() {
			// TODO: Consider making this accept a file path to copy from, or use writeDefault_* methods

			bool isNew = false;
			string machineProfileFolder_FullName=Path.Combine(profilesFolder_FullName,Environment.MachineName);
			if (!Directory.Exists(machineProfileFolder_FullName)) {
				Directory.CreateDirectory(machineProfileFolder_FullName);
				isNew = true;
			}
			//string CopyFileErrors="";
			string machineMainScript_FullName="";
			machineMainScript_FullName=Path.Combine( machineProfileFolder_FullName, MainScriptFile_Name );
			if (   !File.Exists(  machineMainScript_FullName  )   ) {
				Common.sParticiple="creating "+machineMainScript_FullName;
				tbStatus.Text="Copying "+MainScriptFile_FullName+" to new "+MainScriptFile_Name+"...";
				Application.DoEvents();
				
				//if (MainScriptFile_FullName!=thisDestFileFullName) {
				CopyFileWithoutComments(MainScriptFile_FullName, machineMainScript_FullName, false  );
				//}
				isNew = true;
			}
			// NOTE: BackupScriptFile_FullName is set to it below if it is new
			Common.sParticiple="continuing after rewriting "+machineMainScript_FullName;
			
			string machineBackupScript_FullName=Path.Combine(machineProfileFolder_FullName, BackupScriptFile_Name);
			if (!File.Exists(machineBackupScript_FullName)) {
				Common.sParticiple="rewriting "+machineBackupScript_FullName;
				tbStatus.Text="Copying "+BackupScriptFile_FullName+" profile to new "+machineBackupScript_FullName+"...";
				Application.DoEvents();
				//if (MainScriptFile_FullName!=thisDestFileFullName) {
				CopyFileWithoutComments(BackupScriptFile_FullName, machineBackupScript_FullName, false);
				//}
				isNew = true;
			}			
			Common.sParticiple="continuing after rewriting "+machineBackupScript_FullName;
			return isNew;
		}
		
		public void SaveOptions() {
			SaveOptions(null);
		}
		public void SaveOptions(string[] doNotSaveLineIfStartingWithAnyOfTheseStrings) {
			Debug.WriteLine("SaveOptions started.");
			StackTrace stackTrace = new StackTrace();
			Common.sParticiple = "saving options (from "+stackTrace.GetFrame(1).GetMethod().Name+")";
			// TODO: Save main.ini
			SaveBackupScript(doNotSaveLineIfStartingWithAnyOfTheseStrings);
			tbStatus.Text = "Saving profile...OK";
			Application.DoEvents();
			if (!File.Exists(StartupFile_FullName)) {
				Common.sParticiple = "changing startup file path to use machine profile";  // FIXME: Do this or change participle
			}
			Common.sParticiple = "saving startup file (from "+stackTrace.GetFrame(1).GetMethod().Name+")";
			SaveStartupFile(); // skips if in scriptFileNameStack
			Application.DoEvents();
			Debug.WriteLine("SaveOptions finished.");
		}
		
		/// <summary>
		/// Save the profile-specific backup script (only script.txt)
		/// </summary>
		/// <param name="doNotSaveLineIfStartingWithAnyOfTheseStrings"></param>
		void SaveBackupScript(string[] doNotSaveLineIfStartingWithAnyOfTheseStrings) {
			if (!Directory.Exists(MyAppDataFolder_FullName)) {
				Common.sParticiple = "creating subfolder in %APPDATA%";
				Directory.CreateDirectory(MyAppDataFolder_FullName);
			}
			if (!Directory.Exists(profilesFolder_FullName)) {
				Common.sParticiple = "creating folder for profiles in %APPDATA%"+Common.sDirSep+sMyName+Common.sDirSep+profilesFolder_Name;
				Directory.CreateDirectory(profilesFolder_FullName);
			}			
			if (!Directory.Exists(BackupProfileFolder_FullName)) {
				Common.sParticiple = "creating profile folder in %APPDATA%"+Common.sDirSep+sMyName;
				Directory.CreateDirectory(BackupProfileFolder_FullName);
			}

			Common.sParticiple = "locking \""+BackupScriptFile_FullName+"\"";
			StreamWriter outStream = null;
			string tmpPath = BackupScriptFile_FullName + ".tmp";
			int nonCommentCount = 0;
			try {
				outStream = new StreamWriter(tmpPath);
				tbStatus.Text = "Saving profile...";
				Application.DoEvents();
				bool IsDefault = false;
				for (int rowIndex=0; rowIndex<optionsTableLayoutPanel.RowCount; rowIndex++) {
					string line=optionsTableLayoutPanel.Controls["row"+rowIndex.ToString()+"CommandLabel"].Text;
					if (line.Trim().ToLower()=="#backup gonow default") IsDefault=true;
					if (!line.StartsWith("#")) {
						line+=":"+optionsTableLayoutPanel.Controls["row"+rowIndex.ToString()+"ValueLabel"].Text;
						nonCommentCount += 1;
					}
					if (!IsDefault || !line.StartsWith("#")) {
						if ( (doNotSaveLineIfStartingWithAnyOfTheseStrings==null)
						      || (!Common.StartsWithAny(line, doNotSaveLineIfStartingWithAnyOfTheseStrings)) ) {
							outStream.WriteLine(line);
						}
					}
				}
				outStream.Close();
				outStream = null;
				if (nonCommentCount < 1) {
					MessageBox.Show("There are no commands, so "+BackupScriptFile_Name
					                +" will not be saved. Close all "+sMyName
					                +" windows and try again. If the problem persists,"
					                +" contact support and examine \""+BackupProfileFolder_FullName+"\"");
					return;
				}
				if (File.Exists(BackupScriptFile_FullName)) {
					File.Delete(BackupScriptFile_FullName);
				}
				File.Move(tmpPath, BackupScriptFile_FullName);
			}
			catch (Exception exn) {
				throw exn;
			}
			finally {
				if (outStream != null) outStream.Close();
			}
		}
		
		/// <summary>
		/// Save BackupProfileFolder_FullName to StartupFile_FullName
		/// </summary>
		void SaveStartupFile() {
			if (scriptFileNameStack.Contains(StartupFile_FullName)) {
				Debug.WriteLine("Skipping SaveStartupFile since it is running.");
				return;
			}
			if (!Common.sParticiple.ToLower().Contains("startup file"))
				Common.sParticiple = "saving startup file";
			if (!Directory.Exists(MyAppDataFolder_FullName)) {
				Common.sParticiple += "creating subfolder in %APPDATA%";
				Directory.CreateDirectory(MyAppDataFolder_FullName);
			}
			tbStatus.Text = "Saving startup file...";
			Application.DoEvents();
			
			Debug.WriteLine("Editing "+StartupFile_FullName+" in "+System.Reflection.MethodBase.GetCurrentMethod().Name);
			EditScriptLine(StartupFile_FullName, "LoadProfile", ProfileName);
			Debug.WriteLine("Done editing "+StartupFile_FullName+" in "+System.Reflection.MethodBase.GetCurrentMethod().Name);
			tbStatus.Text = "Saving startup file...OK";
			Debug.WriteLine("Saved "+StartupFile_FullName+" with LoadProfile:"+ProfileName);
		}
		
		void MainFolderBrowserDialogHelpRequest(object sender, EventArgs e)
		{
			
		}
		
		void DestinationComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			//bool bFound=false;
			//foreach (string sNow in destinationComboBox.Items) {
			//	if (destinationComboBox.Text==sNow) bFound=true;
			//}
			//if (!bFound) destinationComboBox.SelectedIndex=0;
			//int FolderIndexNow=Common.InternalIndexOfPseudoRoot_WhereIsOrIsParentOf_FolderFullName(destinationComboBox.Text,false);
			LocInfo locinfoNow=Common.GetPseudoRoot_ByCustomInt(destinationComboBox.SelectedIndex);
			
			if (locinfoNow!=null) { //if (FolderIndexNow>=0) {
				ulByteCountDestTotalSize=(long)locinfoNow.TotalSize;
				ulByteCountDestAvailableFreeSpace=(long)locinfoNow.AvailableFreeSpace; //TotalFreeSpace doesn't count user quotas
				DestinationDriveRootDirectory_FullName_OrSlashIfRootDir=locinfoNow.DriveRoot_FullNameThenSlash;//locinfoNow.DriveRoot_FullNameThenSlash+locinfoNow.Subfolder_NameThenSlash_NoStartingSlash;
				if (DestinationDriveRootDirectory_FullName_OrSlashIfRootDir!=Common.sDirSep&&DestinationDriveRootDirectory_FullName_OrSlashIfRootDir.EndsWith(Common.sDirSep)) DestinationDriveRootDirectory_FullName_OrSlashIfRootDir=DestinationDriveRootDirectory_FullName_OrSlashIfRootDir.Substring(0,DestinationDriveRootDirectory_FullName_OrSlashIfRootDir.Length-Common.sDirSep.Length);
				bool bGB=locinfoNow.AvailableFreeSpace/1024/1024/1024 > 0;
				this.driveLabel.Text=Common.LocalFolderThenSlash(DestinationDriveRootDirectory_FullName_OrSlashIfRootDir)+((DestSubfolderRelNameThenSlash!=null)?DestSubfolderRelNameThenSlash:"")+" ("
					+ ((locinfoNow.AvailableFreeSpace!=Int64.MaxValue)  ?  ( bGB ? (((decimal)locinfoNow.AvailableFreeSpace/1024m/1024m/1024m).ToString("#")+"GB free"):(((decimal)locinfoNow.AvailableFreeSpace/1024m/1024m).ToString("0.###")+"MB free") )  :  "unknown free"  )
					+ ")";
			}
			else this.driveLabel.Text="";
			//tbStatus.Text="Destination is now \""+Common.LocalFolderThenSlash(DestinationDriveRootDirectory_FullName_OrSlashIfRootDir)+((DestSubfolderRelNameThenSlash!=null)?DestSubfolderRelNameThenSlash:"")+"\"";
		}

		void AddCommand(string commandString, string valueString) {
			string line=commandString+":"+((valueString!=null)?valueString:"");
			TableLayoutPanel thisTableLayoutPanel=optionsTableLayoutPanel;
			int atRowIndex=thisTableLayoutPanel.RowCount;
			thisTableLayoutPanel.RowStyles.Insert(atRowIndex, new RowStyle(SizeType.AutoSize));
			thisTableLayoutPanel.RowCount += 1;
			
			System.Windows.Forms.Label newCommandLabel = new System.Windows.Forms.Label();
			newCommandLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			newCommandLabel.AutoSize = true;
			newCommandLabel.Location = new System.Drawing.Point(0, 0);
			newCommandLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			newCommandLabel.Name = "row"+atRowIndex.ToString()+"CommandLabel";
			newCommandLabel.Size = new System.Drawing.Size(88, 19);
			newCommandLabel.TabIndex = 100+atRowIndex*thisTableLayoutPanel.ColumnCount+optionColumnIndex_Command;
			newCommandLabel.Text = commandString;
			newCommandLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			thisTableLayoutPanel.Controls.Add(newCommandLabel,optionColumnIndex_Command,atRowIndex);//thisCommandCell.Controls.Add(newCommandLabel);
			//Output("Added command cell "+newCommandLabel.Name,true);

			if (!line.StartsWith("#")) {
				System.Windows.Forms.Label newValueLabel = new System.Windows.Forms.Label();
				newValueLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
				newValueLabel.AutoSize = true;
				newValueLabel.Location = new System.Drawing.Point(0, 0);
				newValueLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
				newValueLabel.Name = "row"+atRowIndex.ToString()+"ValueLabel";
				newValueLabel.Size = new System.Drawing.Size(88, 19);
				newValueLabel.TabIndex = 100+atRowIndex*thisTableLayoutPanel.ColumnCount+optionColumnIndex_Value;
				newValueLabel.Text = valueString;
				newValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
				thisTableLayoutPanel.Controls.Add(newValueLabel,optionColumnIndex_Value,atRowIndex);//thisCommandCell.Controls.Add(newCommandLabel);
			
			
				System.Windows.Forms.Button newRemoveButton;
				newRemoveButton = new System.Windows.Forms.Button();
				newRemoveButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
				newRemoveButton.AutoSize = true;
				newRemoveButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
				newRemoveButton.Location = new System.Drawing.Point(0, 0);
				newRemoveButton.Name = "row"+atRowIndex.ToString()+"DeleteButton";
				newRemoveButton.Tag = "DeleteOptionIndex:"+atRowIndex.ToString();
				newRemoveButton.Size = new System.Drawing.Size(75, 21);
				newRemoveButton.TabIndex = 0;
				newRemoveButton.Text = "Remove";
				newRemoveButton.UseVisualStyleBackColor = true;
                newRemoveButton.Click += new System.EventHandler(this.AnyRemoveOptionIndexButtonClick);
                optionsTableLayoutPanel.Controls.Add(newRemoveButton, optionColumnIndex_DeleteButton, atRowIndex);
			}
			optionsTableLayoutPanel.ScrollControlIntoView(newCommandLabel);
			SaveOptions();
		}
		
		void AddFolderButtonClick(object sender, EventArgs e)
		{
			DialogResult thisDR=mainFolderBrowserDialog.ShowDialog();
			if (thisDR==DialogResult.OK) {
				string commandString="AddFolder";
				string valueString=mainFolderBrowserDialog.SelectedPath;
				AddCommand(commandString, valueString);
			}
			else {
				tbStatus.Text="You cancelled adding a folder.";
			}
		}

		void AddFileButtonClick(object sender, EventArgs e)
		{
			DialogResult thisDR=openFileDialog1.ShowDialog();
			if (thisDR==DialogResult.OK) {
				string commandString="AddFile";
				string valueString=mainFolderBrowserDialog.SelectedPath;
				AddCommand(commandString, valueString);
			}
			else {
				tbStatus.Text="You cancelled adding a file.";
			}
		}
		int DropDownWidth(ComboBox myCombo)
		{
		    int maxWidth = 0;
		    int temp = 0;
		    Label label1 = new Label();
		    label1.Font = myCombo.Font;
		
		    foreach (var obj in myCombo.Items)
		    {
		        label1.Text = obj.ToString();
		        temp = label1.PreferredWidth;
		        if (temp > maxWidth)
		        {
		            maxWidth = temp;
		        }
		    }
		    label1.Dispose();
		    return maxWidth;
		}
		void ExpandProfileDropDown() {
			ExpandProfileDropDown(this.profileCB);
		}
		void ExpandProfileDropDown(ComboBox comboBox1) {
			int width = DropDownWidth(comboBox1);  // width of *longest* item
//			if (this.profileCB.DropDownWidth < width)
//				this.profileCB.DropDownWidth = width;
			int arrow_width = SystemInformation.VerticalScrollBarWidth;
			if (this.profileCB.Width < width + arrow_width)
				this.profileCB.Width = width + arrow_width;
		}
		void ProfileCBSelectedIndexChanged(object sender, EventArgs e)
		{
			ExpandProfileDropDown();
			if (this.profileCBSuspendEvents) return;
			string profileName = (string)this.profileCB.Items[this.profileCB.SelectedIndex];
			Debug.WriteLine("User chose profile: "+profileName);
			RunScriptLine("LoadProfile:"+profileName, recreateFullPathCheckBox.Checked, "<profileCB selected>", -1, 10);
			SaveStartupFile();
			// ^ SaveStartupFile does set thisProfileFolder_FullName = profileName;
		}
	}//end MainForm
}//end namespace
