using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Collections; 
using System.Threading;

///Variables that should be set by calling program: 
// - IgnoreFiles (whether to ignore files in the listing [just show folders])
// - sSearchRoot (where to begin the search--default is current directory [Directory.GetCurrentDirectory()])
//optional vars:
//- sbNow [static] (Windows Forms StatusBar object, can be set to that of any window)
//- MaxDepth (default is no limit)

namespace OrangejuiceElectronica {
	class FolderLister {
		#region variables
		public static bool bDebug=false;
		public bool IsBusy {
			get { return bBusy; }
		}
		public static bool bShowMultistarWarning=true;
		public static ArrayList alExclusions=new ArrayList();
		private static bool bBusy=false;
		public bool bShowFolders=false;
		public string sFolderPrecedent="";
		private bool bIgnoreFiles=false;
		private DirectoryInfo dirinfoStart;
		private int iDepth=1;
		private int MaxFolderDepth=0;//0=no limit
		private ThreadStart deltsDoList;
		private Thread tDoList;
		//public string sMyDocs;
		public string sSearchRoot;
		private bool bShowFullPath=true;
		public bool bOnlyShowPositiveMatches=true;
		public static StatusBar sbNow=null;
		private int iCount=0;
		private static StreamWriter streamOut=null;
		//private bool bStreamOutMustBeClosed=false;
		private StreamWriter streamReplace=null;
		private string sFileReplacing="(no file)"; ///TODO: implement this
		public string sFileReplacingAppendOld=".pre-folderlister";///TODO: rename main file to this before replacing
		public string sFileReplacingAppendNew=".post-folderlister";///TODO: work on this file, then only when SUCCESS, rename to original filename.
		private bool bMustIncludeAllTerms=false;
		private byte[][][] by3dContentSearch=null;//2nd dimension is always an OR search, and contains different encodings (3d dim is raw data).
		private int MinFileSize=0;//can be used even without maximum
		private int MaxFileSize=0;//0 is no limit
		private static string sFileOutput="";
		private static ArrayList alOut=null;
		public static void SetOutputFile(string sFileX) {
			sFileOutput=sFileX;
		}
		private int iContentSearches {
			get {
				int iCount=0;
				if (by3dContentSearch!=null) {
					for (int iNow=0; iNow<by3dContentSearch.Length; iNow++) {
						if (by3dContentSearch[iNow]!=null&&by3dContentSearch[iNow][0]!=null&&by3dContentSearch[iNow][0].Length>0) iCount++;
					}
				}
				return iCount;
			}
		}
		public bool bContentSearch {
			get {
				if (by3dContentSearch!=null) {
					for (int iNow=0; iNow<by3dContentSearch.Length; iNow++) {
						if (by3dContentSearch[iNow]!=null) return true;
					}
				}
				return false;
			}
		}
		public int MinimumFileSize {
			get {
				return MinFileSize;
			}
			set {
				if (value>0) MinFileSize=value;
			}
		}
		public int MaximumFileSize {
			get {
				return MaxFileSize;
			}
			set {
				if (value>=0) MaxFileSize=value;
				else MaxFileSize=0;
				
			}
		}
		public bool IgnoreFiles {
			get {
				return bIgnoreFiles;
			}
			set {
				if (!IsBusy) bIgnoreFiles=value;
			}
		}
		public int MaxDepth {
			get {
				return MaxFolderDepth;
			}
			set {
				if (!IsBusy) MaxFolderDepth=value;
			}
		}
		public static string sStatus {
			set {
				try {
					Console.Error.WriteLine(value);
				}
				catch {//do not report this
				}
				//}
				if (sbNow!=null) {
					try {
						sbNow.Text=value;
					}
					catch {//do not report this
					}
				}
			}
		}
		private static string sBuffer="";
		#endregion variables

		#region output
		private static void WriteLine(string TextX) {
			if (alOut!=null) {
				alOut.Add(sBuffer+TextX);
				sBuffer="";
			}
			else if (streamOut!=null) streamOut.WriteLine(TextX);
			else Console.WriteLine(TextX);
		}
		private static void WriteLine() {
			if (alOut!=null) {
				alOut.Add(sBuffer);
				sBuffer="";
			}
			else if (streamOut!=null) streamOut.WriteLine();
			else Console.WriteLine();
		}
		private static void Write(string TextX) {
			if (alOut!=null) sBuffer+=TextX;
			else if (streamOut!=null) streamOut.Write(TextX);
			else Console.Write(TextX);
		}
		private static void Flush() {
			if (alOut!=null) {} //do nothing
			else if (streamOut!=null) streamOut.Flush();
			else Console.Out.Flush();
		}
		#endregion output
		
		#region constructors
		public FolderLister() {
			//sMyDocs=Directory.GetCurrentDirectory();//Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			sSearchRoot=Directory.GetCurrentDirectory();
			deltsDoList = new ThreadStart(DoList);
	 		tDoList=null;
		}
		#endregion constructors
		
		#region traversal
		public static bool IsLike(string Haystack, string Needle) {
			int iNeedle=0;
			string sNeedleOrig=Needle;
			int iFind=0;
			int iStars=0;
			int iLastStar=-1;
			bool bReturn=false;
			if (Needle=="*") bReturn=true;
			else {
				while (Needle.IndexOf("**")>-1) Needle=Needle.Replace("**","*");
				for (iNeedle=0; iNeedle<Needle.Length; iNeedle++) {
					if (Needle[iNeedle]!='*') iFind++;
					else {
						iStars++;
						iLastStar=iNeedle;
					}
				}//end for counting stars
				if (Needle.Length>2&&Needle.StartsWith("*")&&Needle.EndsWith("*")&&iStars==2) {
					if (Haystack.IndexOf(Needle.Substring(1,Needle.Length-2))>-1)
						bReturn=true;
				}
				else {
					if (iStars==1) {
						if (Needle[0]=='*') {
							if (Haystack.EndsWith(Needle.Substring(1))) //already did Needle.Length==1
								return true;
						}
						else if (Needle[Needle.Length-1]=='*') {
							if (Haystack.StartsWith(Needle.Substring(0,Needle.Length-1)))
								return true;
						}
					}
					else if (iStars==0) {
						return Haystack==Needle;
					}
					else {
						if (bShowMultistarWarning) {
							MessageBox.Show("IsLike(\""+sNeedleOrig+"\"): (parsed as \""+Needle+"\") More than one * and stars not at ends is not implemented (*.* is accepted).  NO matches will be found.");
							bShowMultistarWarning=false;
						}
					}//end else more than one star and stars not at end
				}//else does not start and end with stars (and 2 stars)
			}//end else not "*"
			return bReturn;
		}//end IsLike
		private bool UseFolder(DirectoryInfo dirinfoBranch) {
			bool bReturn=true;
			string sLastExclusion="";
			foreach (string sExclusion in alExclusions) {
				sLastExclusion=sExclusion;
				if (IsLike(dirinfoBranch.Name,sExclusion)) {
					bReturn=false;
					break;
				}
			}
			if (bDebug) Console.Error.WriteLine("("+alExclusions.Count.ToString()+(alExclusions.Count==1?"\""+sLastExclusion+"\"":"")+" excluded) Use folder \""+dirinfoBranch.Name+"\"? "+(bReturn?"Yes":"No"));
			//if (bDebug) Console.Error.WriteLine();
			//if (bDebug) Console.Error.WriteLine("Folder \""+dirinfoBranch.Name+"\" ok: "+(bReturn?"yes":"no"));
			//if (bDebug) Console.Error.WriteLine();
			return bReturn;
		}
		private void SubList(DirectoryInfo dirinfoBranch) {
			int iDepthPrev=iDepth;
			iDepth++;
			if (bDebug) Console.Error.WriteLine();
			//if (bDebug) Console.Error.WriteLine(dirinfoBranch.FullName+":");
			foreach (DirectoryInfo diNow in dirinfoBranch.GetDirectories()) {
				Application.DoEvents();
				if ( (MaxFolderDepth==0||iDepth<MaxFolderDepth) && UseFolder(diNow) ) {
					if (bShowFolders) FolderLister.WriteLine(sFolderPrecedent+diNow.FullName);
					SubList(diNow);
				}
			}//end foreach folder
			if (!bIgnoreFiles) FileList(dirinfoBranch);
			if (bDebug) Console.Error.WriteLine();
			if (bDebug) Console.Error.WriteLine();
			iDepth=iDepthPrev;
		}//end SubList
		private bool UseFile(DirectoryInfo dirinfoBranch, FileInfo fiNow) {
			bool bReturn=true;
			foreach (string sExclusion in alExclusions) {
				if (IsLike(fiNow.Name,sExclusion)) bReturn=false;
			}
			string sVerb="initializing";
			if (bReturn) {
				try {
					if (!bReturn&&bDebug) {
						if (!fiNow.Exists) Console.Error.Write("(does not exist)");
						if (MaxFileSize!=0&&fiNow.Length>MaxFileSize) Console.Error.Write("(file is too big ["+fiNow.Length.ToString()+"bytes])");
						if (MinFileSize!=0&&fiNow.Length<MaxFileSize) Console.Error.Write("(file is too small ["+fiNow.Length.ToString()+"bytes])");
					}
					
					if ( fiNow.Exists && (MaxFileSize==0||fiNow.Length<=MaxFileSize)
					         && (MinFileSize==0||fiNow.Length>=MinFileSize) ) {
						sVerb="checking whether content search is appropriate";
						if (bDebug) Console.Error.Write("File size "+fiNow.Length+" for "+fiNow.Name+" is ok--"+sVerb+".");
						if (bContentSearch) {
							bReturn=false;
							int iFound=0;
							sVerb="counting content search buffers";
							int iFind=bMustIncludeAllTerms?iContentSearches:1;
							///TODO: if (bReplace) iFind=iContentSearches //so that it replaces in ALL matches
							sVerb="running content searches";
							for (int iTerm=0; iTerm<by3dContentSearch.Length; iTerm++) {
								sVerb="running content search "+iTerm.ToString();;
								if (by3dContentSearch[iTerm]!=null) {
									for (int iEncoding=0; iEncoding<by3dContentSearch[iTerm].Length; iEncoding++) {
										if (by3dContentSearch[iTerm][iEncoding]!=null&&by3dContentSearch[iTerm][iEncoding].Length>0) {
											if (FileContains(fiNow.FullName,by3dContentSearch[iTerm][iEncoding])) { //sOnlyIfContains);//dirinfoBranch.FullName+char.ToString(System.IO.Path.DirectorySeparatorChar)+fiNow.Name,sOnlyIfContains);
												iFound++;
												if (iFound>=iFind) break;
											}
										}
										else Console.Error.WriteLine("Term "+iTerm.ToString()+" encoding "+iEncoding.ToString()+" is blank!");
									}
								}//end if not found yet
								if (iFound>=iFind) break;
							}//end for find strings
							if (bDebug&&iFound<iFind) Console.Error.Write("("+iFound.ToString()+"/"+iFind.ToString()+" match)");
							sVerb="exiting content search";
							if (iFound<iFind) bReturn=false;//ok since this line only runs if bContentSearch
						}//end if content search
						else bReturn=true;//no content search needed
					}//end if exists and good size
					//else no good, don't set bReturn
				}
				catch (Exception exn) {
					bReturn=false;
					Console.Error.WriteLine("Exception error in FolderLister UseFile check while "+sVerb+": \""+exn.ToString()+"\"");
				}
			}//end if bReturn (is not like exclusions)
			//if (bDebug) Console.Error.WriteLine();
			//if (bDebug) Console.Error.WriteLine("File \""+fiNow.Name+"\" fullname \""+fiNow.FullName+"\" ok: "+(bReturn?"yes":"no"));
			//if (bDebug) Console.Error.WriteLine();
			return bReturn;
		}//end UseFile
		private void FileList(DirectoryInfo dirinfoBranch) {
			iDepth++;
			int iDepthCurrent=iDepth;
			if (!bIgnoreFiles) {
				foreach (FileInfo fiNow in dirinfoBranch.GetFiles()) {
					Application.DoEvents();
					if (UseFile(dirinfoBranch,fiNow)) {
						if (bShowFullPath) FolderLister.WriteLine(fiNow.FullName);
						else FolderLister.WriteLine(fiNow.Name);
					}
					else if (!bOnlyShowPositiveMatches) FolderLister.WriteLine((!bShowFullPath?dirinfoBranch.FullName+char.ToString(System.IO.Path.DirectorySeparatorChar):"")+fiNow.Name+" {match:no}");
				}
			}
		}//end FileList
		private void DoList() {
			//(values of global vars are set by the calling function to the values of form fields)
			try {
				//if (!bBusy) {
					if (sFileOutput!="") streamOut=new StreamWriter(sFileOutput);
					else {
						if (streamOut!=null) streamOut.Close();
						streamOut=null;
					}
					bBusy=true;
					dirinfoStart=new DirectoryInfo(sSearchRoot);
					iCount=0;
					Console.Error.WriteLine();
					Console.Error.WriteLine();
					Console.Error.WriteLine("Size range: "+MinFileSize.ToString()+"bytes to "+MaxFileSize.ToString()+"bytes");
					Console.Error.WriteLine("Listing...");
					//Console.Error.Flush();
					//if (!bOnlyShowPositiveMatches) FolderLister.WriteLine(dirinfoStart.FullName);
					if (bShowFolders) FolderLister.WriteLine(sFolderPrecedent+dirinfoStart.FullName);
					foreach (DirectoryInfo diNow in dirinfoStart.GetDirectories()) {
						Application.DoEvents();
						try {
							iDepth=1;
							if (UseFolder(diNow)) {
								if (bShowFolders) FolderLister.WriteLine(sFolderPrecedent+diNow.FullName);
								SubList(diNow);
							}
						}
						catch (Exception exn) {
							string sExn=exn.ToString();
							if (sExn.IndexOf("Unauthorized")>-1) Console.Error.WriteLine("DoList{count:"+iCount.ToString()+"}: Access to "+diNow.FullName+" was forbidden.");
							else {
								Console.Error.WriteLine("Error in DoList{count:"+iCount.ToString()+"}:");
								Console.Error.WriteLine(exn.ToString());
							}
						}
					}//end for each directory
					if (!bIgnoreFiles) FileList(dirinfoStart);
					sStatus="Finished!";
					//this.lblFinished.Visible=true;
					//this.lblFinished.Show();
					bBusy=false;//(ONLY reset if WAS NOT busy)
					if (streamOut!=null) streamOut.Close();
				//}//end if not busy
			}
			catch (Exception exn) {
				Console.Error.WriteLine("Exception error in DoList {count:"+iCount.ToString()+"}:  "+exn.ToString());
			}
			bBusy=false;
		}//end DoList
		public void StartRecordingLines() {
			alOut=new ArrayList();
		}
		public string[] GetLines() {
			string[] sarrReturn=null;
			if (!bBusy) {
				if (alOut!=null&&alOut.Count>0) {
					sarrReturn=new string[alOut.Count];
					int iNow=0;
					foreach (string sLine in alOut) {
						sarrReturn[iNow]=sLine;
						iNow++;
					}
				}
			}
			else {
				string sMsg="Cannot return lines while busy!";
				Console.Error.WriteLine(sMsg);
				if (bDebug) MessageBox.Show(sMsg);
			}
			return sarrReturn;
		}
		/// <summary>
		/// starts listing files--starts a thread which runs DoList().  Set global parameters first if options other than default are desired.
		/// </summary>
		public void Start() {
			try {
				if (!IsBusy) {
					bBusy=true;
					tDoList = new Thread(deltsDoList);
					tDoList.Start();
				}
				else {
					string sMsg="Busy: Cannot start listing.";
					if (bDebug) {
						MessageBox.Show(sMsg);
					}
					Console.Error.WriteLine(sMsg);
				}
			}
			catch (Exception exn)  {
				Console.Error.WriteLine("Exception error in FolderLister Start {count:"+iCount.ToString()+"}:  "+exn.ToString());
			}
		}//end Start
		/// <summary>
		/// stops FolderLister listing thread
		/// </summary>
		public void Stop() {
			try {
				tDoList.Abort();
				bBusy=false;
			}
			catch (Exception exn) {
				Console.Error.WriteLine("Exception error Stopping FolderLister: "+exn.ToString());
			}
		}
		#endregion traversal
		
	
		#region utilities
		public string Plural(string sBase, string sSingularEnding, string sPluralEnding, int iCount) {
			return iCount==1?sBase+sSingularEnding:sBase+sPluralEnding;
		}//TODO: move to UniWinForms
		~FolderLister() {
			//CloseStreams();
		}
		public static bool FileContains(string sFile, byte[] byarrNeedle) {
			bool bMatch=false;
			try {
				if (bDebug) Console.Error.WriteLine("Starting \""+sFile+"\" content search");
				bMatch=Chunker.Contains(sFile,byarrNeedle);
			}
			catch (Exception exn) {
				string sExn=exn.ToString();
				if (sExn.IndexOf("FileNotFoundException")>-1) sExn="FileContains(...,binary): Cannot find  \""+sFile+"\" for binary search-may be broken symbolic link or in different relative path.";
				Console.Error.WriteLine(sExn);
			}
			return bMatch;
		}//end FileContains(...,byarrNeedle) //TODO: move to UniWinForms
		public void ClearContentSearch() {//was ClearSearchTerms
			by3dContentSearch=null;
		}
		public void SetContentSearch(string Term, bool bAlsoAllowUnicode, bool bAlsoAllowBigEndianMacUnicode) {
			SetContentSearch(Term,true,bAlsoAllowUnicode,bAlsoAllowBigEndianMacUnicode);
		}
		public void SetContentSearch(string Term, bool bAllowUnicode, bool bAllowBigEndianMacUnicode, bool bAllowAsciiCanProvideMatch) {
			SetContentSearch(new string[]{Term},true,bAllowUnicode, bAllowBigEndianMacUnicode, bAllowAsciiCanProvideMatch);
		}
		public void SetContentSearch(string[] Terms, bool MustIncludeAllTerms) {
			SetContentSearch(Terms,MustIncludeAllTerms,false,false,true);
		}
		public void SetContentSearch(string[] Terms, bool MustIncludeAllTerms, bool bAllowUnicode, bool bAllowBigEndianMacUnicode, bool bAllowAscii) {
			by3dContentSearch=null;
			bMustIncludeAllTerms=MustIncludeAllTerms;
			if (MaxFileSize==0) MaxFileSize=102400; //TODO: allow user to intervene here
			if (Terms!=null&&Terms.Length>0) {
				int iGoodTermRelatives=0;
				for (int iX=0; iX<Terms.Length; iX++) {
					if (Terms[iX]!=null&&Terms[iX].Length>0) iGoodTermRelatives++;
				}
				if (iGoodTermRelatives>0) {
					if (MaxFileSize<1) MaxFileSize=2048000000;//2048000000bytes==2GB
					if (!bAllowAscii&&!bAllowUnicode) bAllowAscii=true;
					by3dContentSearch=new byte[iGoodTermRelatives][][];
					int iGoodTermRelative=0;
					for (int iTerm=0; iTerm<Terms.Length; iTerm++) {
						if (Terms[iTerm]!=null&&Terms[iTerm].Length>0) {
							by3dContentSearch[iGoodTermRelative]=new byte[(bAllowAscii?1:0)+(bAllowUnicode?1:0)+(bAllowBigEndianMacUnicode?1:0)][];
							int iEncodings=0;
							if (bAllowAscii) {
								by3dContentSearch[iGoodTermRelative][iEncodings]=new byte[Terms[iTerm].Length];
								for (int iChar=0; iChar<Terms[iTerm].Length; iChar++) {
									by3dContentSearch[iGoodTermRelative][iEncodings][iChar]=(byte)((uint)Terms[iTerm][iChar]&0xFF);
								}
								iEncodings++;
							}
							if (bAllowUnicode) {
								by3dContentSearch[iGoodTermRelative][iEncodings]=new byte[Terms[iTerm].Length*2];
								for (int iChar=0; iChar<Terms[iTerm].Length; iChar++) {
									by3dContentSearch[iGoodTermRelative][iEncodings][iChar*2]=(byte)((uint)Terms[iTerm][iChar]&0xFF);
									by3dContentSearch[iGoodTermRelative][iEncodings][iChar*2+1]=(byte)((uint)Terms[iTerm][iChar]>>8);
								}
								iEncodings++;
							}
							if (bAllowBigEndianMacUnicode) {
								by3dContentSearch[iGoodTermRelative][iEncodings]=new byte[Terms[iTerm].Length*2];
								for (int iChar=0; iChar<Terms[iTerm].Length; iChar++) {
									by3dContentSearch[iGoodTermRelative][iEncodings][iChar*2]=(byte)((uint)Terms[iTerm][iChar]>>8);
									by3dContentSearch[iGoodTermRelative][iEncodings][iChar*2+1]=(byte)((uint)Terms[iTerm][iChar]&0xFF);
								}
								iEncodings++;
							}
							if (bDebug) Console.Error.WriteLine(iEncodings.ToString()+" encoding"+(iEncodings==1?"":"s")+" added for "+Terms[iTerm]);
							iGoodTermRelative++;
						}//end if term is not blank
					}//end for terms
					if (bDebug) Console.Error.WriteLine(iGoodTermRelative.ToString()+" term"+(iGoodTermRelative==1?" ("+Terms[0]+")":"s")+" added.");
				}//if any terms given
				else Console.Error.WriteLine("SetContentSearch error: Terms array with empty terms sent!");
			}//terms array is not blank
			else Console.Error.WriteLine("SetContentSearch error: Blank terms array sent!");
		}//end SetContentSearch
		/// <summary>
		/// 
		/// </summary>
		/// <param name="array_of_byte_arrays"></param>
		public void SetContentSearch(byte[] byarrContains) {
			by3dContentSearch=new byte[1][][];
			by3dContentSearch[0]=new byte[1][];
			by3dContentSearch[0][0]=byarrContains;
		}

		#endregion utilities
	}//end FolderLister
}//end namespace
