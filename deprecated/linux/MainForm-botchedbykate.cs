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
//using System.Management;//for getting free disk space (ManagementObject)


namespace OrangejuiceElectronica
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		//TODO: Option to remove files from the backup drive that aren't in the backup script
		public static bool bRealTime=true;
		public static int iDepth=0;
		public static int iFilesProcessed=0;
		public static bool bDebug=false;
		public static bool bAutoScroll=true;
		public static string sMyName="Backup GoNow";
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
		private static bool bExitIfNoUsableDrivesFound=false;
		private static bool bAlwaysStayOpen=false;
		private static bool bUserCancelledLastRun=false;
		private static bool bCopyErrorLastRun=false;
		private static bool bDiskFullLastRun=false;
		private static int iSkipped=0;
		private static ArrayList alSkipped=new ArrayList();
		private static ArrayList alCopyError=new ArrayList();
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
		private static ulong ulByteCountDestTotalSize=0;
		private static ulong ulByteCountDestAvailableFreeSpace=0;
		private static long lByteCountTotalActuallyAdded=0;
		private static string GetDestDriveRoot() {//="";//drive or folder
			try {
				return (driveinfoarrNow!=null&&iDriveDest>-1&&iDriveDest<driveinfoarrNow.Length) ? driveinfoarrNow[iDriveDest].RootDirectory.FullName : "";
			}
			catch (Exception exn) {
				string sMsg="Error getting drive root folder: "+ToOneLine(exn);
				Output(sMsg);
				Console.Error.WriteLine(sMsg);
			}
			return "";
		}
		private static string sDestSub="";
		public static string sWasUpToDate="Was Up to Date";
		public static Brush brushItemOther = Brushes.Black;
		public static SolidBrush brushItemWasUpToDate = new SolidBrush(Color.FromArgb(192, 192, 192)); //Brushes.Gray;
		public static DriveInfo[] driveinfoarrNow=null;
		public static int iDriveDest=0;

		private static string sDestPathSlash {
			get { return sDestRoot+sDestSub; }
		}
		private static string sDirSep {
			get { return char.ToString(Path.DirectorySeparatorChar); }
		}
		public MainForm() {
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			//string sMyPath = Assembly.GetExecutingAssembly().Location;
			//int iLastSlash=sMyPath.LastIndexOf(char.ToString(Path.DirectorySeparatorChar));
			//if (iLastSlash>-1) {
			//	sMyPath=sMyPath.Substring(0,iLastSlash);
			//	Directory.SetCurrentDirectory(sMyPath);
			//}
		}//end MainForm constructor

		public static bool ToBool(string sNow) {
			return sNow.ToLower()=="yes"||sNow=="1"||sNow.ToLower()=="true";
		}
		void BackupFolder(DirectoryInfo diBase) {
			iDepth++;
			//if (bDebug) Output("Getting ready to copy "+(diBase.Size/1024/1024).ToString()+"MB...");			
			foreach (DirectoryInfo diNow in diBase.GetDirectories()) {
				ReconstructPathOnBackup(diNow.FullName);
				if (!bUserCancelledLastRun&&!bDiskFullLastRun
					&&flisterNow.UseFolder(diNow))
					BackupFolder(diNow);
			}
			foreach (FileInfo fiNow in diBase.GetFiles()) {
				if (bUserCancelledLastRun||bDiskFullLastRun) break;
				if (flisterNow.UseFile(diBase,fiNow)) {
					BackupFile(fiNow.FullName,true);
					if (bDebug) Output("  ("+fiNow.FullName+")",true);
				}
				if (bUserCancelledLastRun||bDiskFullLastRun) break; //note: do NOT stop if Copy Error only
			}//end foreach file
			iDepth--;
		}//end BackupFolder recursively

		bool RunScript(string sFileX) {
			bool bGood=false;
			StreamReader streamIn=null;
			if (alSkipped!=null) alSkipped.Clear();
			else alSkipped=new ArrayList();
			try {
				streamIn=new StreamReader(sFileX);
				string sLine;
				flisterNow=new FolderLister();
				//flisterNow.MinimumFileSize=1;//1byte (trying to avoid bad symlinks here)
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
						else if (sCommand=="addmask") {
							FolderLister.alMasks.Add(sValue);
							string sTemp="";
							foreach (string sMask in FolderLister.alMasks) {
								sTemp+=(sTemp==""?"":", ")+sMask;
							}
							if (bDebug) Output("#Masks changed: "+sTemp);
						}
						else if (sCommand=="removemask") {
							if (sValue=="*") FolderLister.alMasks.Clear();
							else FolderLister.alMasks.Remove(sValue);
							string sTemp="";
							foreach (string sMask in FolderLister.alMasks) {
								sTemp+=(sTemp==""?"":", ")+sMask;
							}
							if (bDebug) Output("#Masks changed: "+sTemp);
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
							if (sValue=="*") FolderLister.alExclusions.Clear();
							else FolderLister.alExclusions.Remove(sValue);
							string sTemp="";
							foreach (string sExclusion in FolderLister.alExclusions) {
								sTemp+=(sTemp==""?"":", ")+sExclusion;
							}
							if (bDebug) Output("#Exclusions changed: "+sTemp);
						}
						else if (sCommand=="addfile") {
							try {ulByteCountTotal+=(ulong)(new FileInfo(sValue)).Length;}
							catch {}
							if (File.Exists(sValue)) {
								FileInfo fiX=new FileInfo(sValue);
								ulByteCountTotal+=(ulong)fiX.Length;
								//if (fiX.Exists())
								ReconstructPathOnBackup(fiX.DirectoryName);
								BackupFile(sValue,true);
							}
							else {
								bCopyErrorLastRun=true;
								alCopyError.Add("File specified in configuration does not exist: "+sValue);
							}
						}
						else if (sCommand=="addfgth;
							ulByteCountTotalActuallyCopied+=(ulong)fiNow.Length;
						}
						Output(sDone+"Copied New: \""+sDestFile+"\"");
					}
				}
				else Output(sDone+"Could not find \""+sSrcFilePath+"\"");
			}
			catch (Exception exn) {
				if (exn.ToString().ToLower().IndexOf("system.io.ioexception: disk full")>-1) bDiskFullLastRun=true;
				else if (exn.ToString().ToLower().IndexOf("system.io.directorynotfoundexception")>-1) {
					alCopyError.Add("Could not recreate source folder: "+sSrcFilePath+" -- "+ToOneLine(exn.ToString()));
				}
				else {
					alCopyError.Add("Could not read: "+sSrcFilePath+" -- "+ToOneLine(exn.ToString()));
					bCopyErrorLastRun=true;
				}
				Console.Error.WriteLine("Error in BackupFile: ");
				Console.Error.WriteLine(exn.ToString());
				Console.Error.WriteLine();
			}older") {
							flisterNow.sSearchRoot=sValue;
							//string sDirSep=char.ToString(Path.DirectorySeparatorChar);
							Output("Loading \""+flisterNow.sSearchRoot+"\""+(flisterNow.MaskCount>0?(" (only "+flisterNow.MasksToCSV()+")"):"")+"...");
							//string sTempFile=Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)+"FolderList.tmp";
							//FolderLister.SetOutputFile(sTempFile);
							flisterNow.StartRecordingLines();
							if (bRealTime) {
								//sVerb="getting directory info";
								DirectoryInfo diRoot=new DirectoryInfo(flisterNow.sSearchRoot);
								iDepth=-1;
								if (diRoot.Exists) BackupFolder(diRoot);
								//else 
							}
							else {
								flisterNow.Start();
								btnCancel.Enabled=true;
								while (flisterNow.IsBusy) {
									Thread.Sleep(500);
									mainformNow.Refresh();
									Application.DoEvents();
								}
								bBusyCopying=true;
								Thread.Sleep(1000);
								if (FolderLister.alSkipped!=null&&FolderLister.alSkipped.Count>0) {
									foreach (string sSkippedNow in FolderLister.alSkipped) {
										alSkipped.Add(sSkippedNow);
									}
								}
								string[] sarrListed=flisterNow.GetLines();
								ulByteCountFolderNow=flisterNow.ByteCount;
								ulByteCountTotal+=ulByteCountFolderNow;
								ulByteCountFolderNowDone=0;
								if (bDebug) Output("Getting ready to copy "+(ulByteCountFolderNow/1024/1024).ToString()+"MB...");
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
										if (bDebug) Output(sListedItem,true);
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
						else if (sCommand=="exitifnousabledrivesfound") {
							bExitIfNoUsableDrivesFound=ToBool(sValue);
						}
						else if (sCommand=="alwaysstayopen") {
							bAlwaysStayOpen=ToBool(sValue);
						}
						else if (sCommand=="testonly") {
							bDebug=ToBool(sValue);
							Output("Test mode turned "+(bDebug?"on":"off")+"."+(bDebug?"  No files will be copied.":""));
						}
						else if (sCommand=="destsubfolder") {
							sDestSub=sValue;
							FixBackupDest();
						}
					}//end if has ":" in right place
					iLine++;
					if (bDiskFullLastRun||bUserCancelledLastRun) break; //do NOT stop if Copy Error only
				}//end while lines in script
				//if (bDebug) {
				if (alSkipped.Count>0) {
					Output("");
					Output("Could not list "+alSkipped.Count.ToString()+":",true);
					foreach (string sSkippedNow in alSkipped) {
						Output("(could not list) "+sSkippedNow);
					}
				}
				if (alCopyError.Count>0) {
					Output("Could not copy "+alCopyError.Count.ToString(),true);
					foreach (string sCopyErrorNow in alCopyError) {
						Output("(could not copy) "+sCopyErrorNow);
					}
				}
					Output("Finished reading "+sFileX+" (listed: "+iListedLines+"; could not list: "+alSkipped.Count.ToString()+"; copy errors: "+alCopyError.Count.ToString()+").",true);
				alSkipped.Clear();
				alCopyError.Clear();
				//}
				bGood=true;
			}
			catch (Exception exn) {
				string sMsg="Error running \""+sFileX+"\":";
				if (bDebug) MessageBox.Show(sMsg+"\n"+exn.ToString(),"Backup GoNow");
				Console.Error.WriteLine();
				Console.Error.WriteLine(sMsg);
				Console.Error.WriteLine(exn.ToString());
				bGood=false;
			}
			try {
				if (streamIn!=null) streamIn.Close();
			}
			catch {}
			return bGood;
		}//end RunScript
		public void SetDestDriveRoot(string sDriveRoot) {
			iDriveDest=-1;
			if (sDriveRoot!=null&&sDriveRoot!="") {
				if (sDriveRoot.Length>1&&sDriveRoot.EndsWith(sDirSep)) {
					sDriveRoot=sDriveRoot.Substring(0,sDriveRoot.Length-1);
				}
				if (driveinfoarrNow!=null) {
					for (int iNow=0; iNow<driveinfoarrNow.Length; iNow++) {
						if (sDriveRoot==driveinfoarrNow[iNow].RootDirectory.FullName) {
							iDriveDest=iNow;
							break;
						}
					}
				}
			}
			if (iDriveDest>-1) {
				ulByteCountDestTotalSize=(ulong)driveinfoarrNow[iDriveDest].TotalSize;
				ulByteCountDestAvailableFreeSpace=(ulong)driveinfoarrNow[iDriveDest].AvailableFreeSpace; //TotalFreeSpace doesn't count user quotas
			}
			else {
				ulByteCountDestTotalSize=0;
				ulByteCountDestAvailableFreeSpace=0;
			}
		}//end SetDestDriveRoot
		public bool FixBackupDest() {
			bool bGood=false;
			SetDestDriveRoot(this.cbDest.Text);
			string sDestRoot=GetDestDriveRoot();
			if (sDestRoot!="") {
				bGood=true;
				if (!sDestRoot.EndsWith(sDirSep)) sDestRoot+=sDirSep;
			}
			else bGood=false;
			if (sDestSub!="") {
				while (sDestSub.StartsWith(sDirSep)) sDestSub=(sDestSub.Length>1)?sDestSub.Substring(1):"";
				if (!sDestSub.EndsWith(sDirSep)) sDestSub+=sDirSep;
			}
			return bGood;
		}
		private static bool bShowReconstructedBackupPathError=true;
		public string ReconstructedBackupPath(string sSrcPath) {
			//if (bDebug) Output("Reconstruction(as received): "+sSrcPath);
			string sReturn=sDestPathSlash;
			string sDestAppend=sSrcPath;
			int iStart=0;
			if (sDestAppend[iStart]=='/') {
				while (iStart<sDestAppend.Length&&sDestAppend[iStart]=='/') {
					iStart++;
				}
			}
			//else iStart=Chunker.IndexOfAnyDirectorySeparatorChar(sDestAppend); //uncommenting this removes the "C" folder if using this program in windows to backup local files
			string sDirSep=char.ToString(Path.DirectorySeparatorChar);
			if (iStart>-1&&iStart<sDestAppend.Length) {
				sDestAppend=sDestAppend.Substring(iStart);
				//if (bDebug) Output("Reconstruction(before normalize): "+sDestAppend);
				sDestAppend=Chunker.ConvertDirectorySeparatorsToNormal(sDestAppend);
				//if (bDebug) Output("Reconstruction(before removedouble): "+sDestAppend);
				sDestAppend=Chunker.RemoveDoubleDirectorySeparators(sDestAppend);
				if (sDestAppend!=null&&sDestAppend!=""&&sDestAppend.StartsWith(sDirSep)) {
					if (sDestAppend.Length>1) sDestAppend=sDestAppend.Substring(1);
					else sDestAppend="";
				}
			}
			else sDestAppend="";

			if (sDestAppend=="") {
				if (bShowReconstructedBackupPathError) {
					MessageBox.Show("The backup source cannot be parsed so these files will be placed in the root of \""+sDestPathSlash+"\".");
					bShowReconstructedBackupPathError=false;
				}
				sReturn=sDestPathSlash;
			}
			else sReturn+=sDestAppend;
			if ( !sReturn.EndsWith(sDirSep) )
				sReturn+=sDirSep;
			return sReturn;
		}//end ReconstructedBackupPath
		public bool ReconstructPathOnBackup(string sSrcPath) {
			string sBackupFolder=ReconstructedBackupPath(sSrcPath);
			bool bGood=Chunker.CreateFolderRecursively(sBackupFolder);
			if (!bGood&&Chunker.sLastExn.ToLower().IndexOf("system.io.ioexception: disk full")>-1) bDiskFullLastRun=true;
			if (bDebug) Output("Created \""+sBackupFolder+"\"");
			return bGood;
		}
		public void BackupFile(string sSrcFilePath, bool bUseReconstructedPath) {
			BackupFile(sSrcPath,bUseReconstructedPath,null);
		}
		public void BackupFile(string sSrcFilePath, bool bUseReconstructedPath, FileInfo fiNow) {
			decimal dDone=-1.0m;
			int iDone=0;//whole-number percentage
			string sDone="";
			if (ulByteCountFolderNow>0) {
				try {
					dDone=(decimal)ulByteCountFolderNowDone/(decimal)ulByteCountFolderNow;
					iDone=(int)(dDone*100m);
					sDone="("+iDone.ToString()+"%) ";
				}
				catch {}
			}
			try {
				if (fiNow==null) fiNow=new FileInfo(sSrcFilePath);
				string sDirSep=char.ToString(Path.DirectorySeparatorChar);
				if (fiNow.Exists) {
					ulByteCountFolderNowDone+=(ulong)fiNow.Length;
					ulByteCountTotalProcessed+=(ulong)fiNow.Length;
					string sBackupFolder=bUseReconstructedPath?ReconstructedBackupPath(fiNow.Directory.FullName):sDestPathSlash;
					if (!sBackupFolder.EndsWith(sDirSep)) sBackupFolder+=sDirSep;
					string sDestFile=sBackupFolder+fiNow.Name;
					FileInfo fiDest=new FileInfo(sDestFile);
					if (fiDest.Exists) {
						if (fiDest.LastWriteTime<fiNow.LastWriteTime||fiDest.Length!=fiNow.Length) {
							if (	fiDest.LastWriteTime>=fiNow.LastWriteTime
									&&fiDest.Length!=fiNow.Length )
								Output(sDone+"Resaving: \""+sDestFile+"\"");
							else
								Output(sDone+"Updating: \""+sDestFile+"\"");
							if (!bDebug) {
								File.Copy(sSrcFilePath,sDestFile,true);
								lByteCountTotalActuallyAdded+=(long)fiNow.Length-(long)fiDest.Length;
								ulByteCountTotalActuallyCopied+=(ulong)fiNow.Length;
							}
						}
						else {
							//already newer or same timestamp so ignore
							Output(sDone+sWasUpToDate+": \""+sDestFile+"\"",false);
						}
					}
					else {
						if (!bDebug) {
							File.Copy(sSrcFilePath,sDestFile);
							lByteCountTotalActuallyAdded+=(long)fiNow.Length;
							ulByteCountTotalActuallyCopied+=(ulong)fiNow.Length;
						}
						Output(sDone+"Copied New: \""+sDestFile+"\"");
					}
				}
				else Output(sDone+"Could not find \""+sSrcFilePath+"\"");
			}
			catch (Exception exn) {
				if (exn.ToString().ToLower().IndexOf("system.io.ioexception: disk full")>-1) bDiskFullLastRun=true;
				else if (exn.ToString().ToLower().IndexOf("system.io.directorynotfoundexception")>-1) {
					alCopyError.Add("Could not recreate source folder: "+sSrcFilePath+" -- "+ToOneLine(exn.ToString()));
				}
				else {
					alCopyError.Add("Could not read: "+sSrcFilePath+" -- "+ToOneLine(exn.ToString()));
					bCopyErrorLastRun=true;
				}
				Console.Error.WriteLine("Error in BackupFile: ");
				Console.Error.WriteLine(exn.ToString());
				Console.Error.WriteLine();
			}
			iFilesProcessed++;
		}//end BackupFile
		public static bool bShowOutputException=true;
		public static void Output(string sLineX) {
			Output(sLineX,false);
		}
		private static ArrayList alDisplayQueue=new ArrayList();
		public static void Flush() {
			if (alDisplayQueue!=null&&alDisplayQueue.Count>0) {
				lbOutNow.BeginUpdate();
				foreach (string sNow in alDisplayQueue) {
					lbOutNow.Items.Add(sNow);
				}
				lbOutNow.EndUpdate();
				if (bAutoScroll) MainForm.lbOutNow.SelectedIndex=MainForm.lbOutNow.Items.Count-1;
				lbOutNow.Refresh();
				Application.DoEvents();
				mainformNow.Refresh();
				alDisplayQueue.Clear();
				iTickLastRefresh=Environment.TickCount;
			}
		}
		public static void Output(string sLineX, bool bForceRefresh) {
			string sPercentFree="";
			try {
				alDisplayQueue.Add(sLineX);
				if ( bForceRefresh || (Environment.TickCount-iTickLastRefresh>iTicksRefreshInterval) ) {
					//decimal dDone=-1.0m;
					//if (ulByteCountTotal>0) dDone=(decimal)ulByteCountTotalProcessed/(decimal)ulByteCountTotal;
					ulong ulFree=ulByteCountDestAvailableFreeSpace-ulByteCountTotalProcessed;
					decimal mFree=0;
					try {
						mFree=(decimal)((decimal)ulFree-(decimal)lByteCountTotalActuallyAdded) / (decimal)ulByteCountDestTotalSize;
					}
					catch (Exception exn) {
						mFree=-1.0m;
					}
					mFree*=100.0m;
					if (mFree>=0) sPercentFree=String.Format("{0:P}",mFree)+":";
					if (bRealTime) {
						//int iDot=sPercentFree.IndexOf(".");
						//if (iDot>-1) {
						//	if (iDot
						//}
						tbStatus.Text=String.Format( "Processed: {0} files ~ {1}MB processed -- {2}{3}MB available remaining",
							iFilesProcessed, (ulByteCountTotalProcessed/1024/1024), sPercent, (ulFree/1024/1024) )
							+(bAutoScroll?"":"...");
					}
					else tbStatus.Text=String.Format( "{0}MB / {1}MB counted so far",
							((ulByteCountTotalProcessed)/1024/1024),(ulByteCountTotal/1024/1024) )
							  +  (bAutoScroll?"":"...");
					Flush();
				}
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
		void LbOutMouseEnter(object sender, EventArgs e) {
			bAutoScroll=false;
		}
		void LbOutMouseLeave(object sender, EventArgs e) {
			bAutoScroll=true;
		}
		void LbOutMouseUp(object sender, MouseEventArgs e) {
			//bAutoScroll=true;
		}
		void LbOutMouseDown(object sender, MouseEventArgs e) {
			bAutoScroll=false;
		}
		void BtnGoClick(object sender, EventArgs e)
		{
			if (this.cbDest.Text!="") {
				btnGo.Enabled=false;
				bUserCancelledLastRun=false;
				bCopyErrorLastRun=false;
				bDiskFullLastRun=false;
				bool bGood=true;
				
				if (!FixBackupDest()) bGood=false;
				if (!RunScript(sFileScript)) bGood=false;
				
				Output((ulByteCountTotalProcessed/1024/1024/1024).ToString()+"GB = "+(ulByteCountTotalProcessed/1024/1024).ToString()+"MB = "+(ulByteCountTotalProcessed/1024).ToString()+"KB = "+ulByteCountTotalProcessed.ToString()+"bytes of "+(ulByteCountTotal/1024/1024).ToString()+"MB source data finished, "+(ulByteCountTotalActuallyCopied/1024/1024).ToString()+"MB difference copied).",true);
				if (!bGood) MessageBox.Show("Backup was not complete.");
				else {
					if (bUserCancelledLastRun) MessageBox.Show("Cancelled Backup");
					else if (bDiskFullLastRun) MessageBox.Show("Destination drive is full - Could not finish");
					else if (bCopyErrorLastRun) MessageBox.Show("Copy Error - Could not finish");
					else MessageBox.Show("Finished Backup");
					if (!bDebug&&!bAlwaysStayOpen) Application.Exit();
					
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
			sMyProcess = sMyProcess.Substring(sMyProcess.LastIndexOf('\\') + 1);
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
			//string[] sarrDrive=Environment.GetLogicalDrives();
			//foreach (string sDrivePathNow in sarrDrive) {
			//	if (ValidDest(sDrivePathNow)) {
			//		cbDest.Items.Add(sDrivePathNow);
			//		iValidDrivesFound++;
			//		iDestinations++;
			//	}
			//}
			//foreach (string sExtraDest in alExtraDestinations) {
			//	cbDest.Items.Add(sExtraDest);
			//	iDestinations++;
			//}
			if (DriveInfo.GetDrives().Count>0) {
				driveinfoarrNow=new DriveInfo[DriveInfo.GetDrives().Count];
				foreach (DriveInfo driveinfoNow in DriveInfo.GetDrives()) {
					if (driveinfoNow.IsReady) {
						if (ValidDest(driveinfoNow.RootDirectory.FullName)) {
							cbDest.Items.Add(driveinfoNow.RootDirectory.FullName);
							driveinfoarrNow[iDestinations]=driveinfoNow;
							iDestinations++;
						}
					}
				}
				if (iDestinations>0) {
					driveinfoarrOld=driveinfoarrNow;
					driveinfoarrNow=new DriveInfo[iDestinations];
					int iOld=0;
					for (int iNow=0; iNow<iDestinations; iNow++) {
						driveinfoarrNow[iNow]=driveinfoarrOld[iOld];
						iOld++;
					}
				}
				else driveinfoarrNow=null;
			}
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
	else
	{
			Console.WriteLine("Drive " + drive.Name + " is not ready.");
	}
}
*/
			cbDest.EndUpdate();
			
			
			//FolderLister.Echo("Test");
			string sMsg="No backup drive can be found.  Try connecting the drive and then try again.";
			if (iValidDrivesFound+iDestinations>0) {
				if (bExitIfNoUsableDrivesFound&&iValidDrivesFound==0) {
					MessageBox.Show(sMsg);
					if (!bAlwaysStayOpen) Application.Exit();
				}
				cbDest.SelectedIndex=0;
			}
			else {
				MessageBox.Show(sMsg);
				if (!bDebug&&!bAlwaysStayOpen) Application.Exit();
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
				btnCancel.Enabled=false;
				bUserCancelledLastRun=true;
			}
			else {
				if (flisterNow!=null&&flisterNow.IsBusy) flisterNow.Stop();
				btnCancel.Enabled=false;
				bUserCancelledLastRun=true;
			}
		}
		private void LbOutDrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e) {
			e.DrawBackground();
			
			//TODO: find out why no gray lines are being drawn (supposed to use brushItemWasUpToDate)
			if (((ListBox)sender).Items[e.Index].ToString().IndexOf(sWasUpToDate)>-1) 
				e.Graphics.DrawString( ((ListBox)sender).Items[e.Index].ToString(), e.Font, brushItemWasUpToDate,e.Bounds,StringFormat.GenericDefault);
			else {
				e.Graphics.DrawString( ((ListBox)sender).Items[e.Index].ToString(), e.Font, brushItemOther,e.Bounds,StringFormat.GenericDefault);
			}
			e.DrawFocusRectangle();//only draws if focused
		}//end paint item override
		public static string ToOneLine(string sNow) {
			sNow=sNow.Replace("\n"," ");
			sNow=sNow.Replace("\r"," ");
			while (sNow.Contains("  ")) sNow=sNow.Replace("  "," ");
			return sNow;
		}
		public static string ToOneLine(Exception exn) {
			return ToOneLine(exn.ToString());
		}
	}//end MainForm
}//end namespace
