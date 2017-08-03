//all rights reserved Jake Gustafson 2007
//created using Kate 2007-09-23
//
using System;
using System.IO;
using System.Collections;//ArrayList etc
namespace OrangejuiceElectronica {
	class Chunker { //TODO: change to RStream and move to RetroEngine
		#region variables
		public static bool bDebug=false;
		private FileStream fsNow=null;
		private byte[] ThisChunk=null;
		private long ByteNowAbsolute;
		private long SizeOfFinalPartialChunk;//bytes of last chunk that exist (i.e. before end of file)
		private long FileLength;
		private long ChunkNow; //chunk in file (ByteNowAbsolute/ChunkSize)
		private long ChunkCount;
		private string sLastFile="";
		public int DefaultChunkSize=1024000;//bytes (i.e. 10240 is 10K)
		//private int ChunkLoaded {
		//	get {
		//		return (ByteNowAbsolute<0)?ByteNowAbsolute:ByteNowAbsolute/ChunkSize;
		//	}
		//}
		private long ByteNowRelative {
			get { return ByteNowAbsolute%ChunkSize; }
		}
		private long PositionOfChunkNow {
			get { return ByteNowAbsolute-ByteNowRelative;}//same as Chunk*ChunkSize
		}
		public long Position {
			get { return ByteNowAbsolute; }
			set { SetPosition(value); }
		}
		public long Length {
			get { return FileLength; }
		}
		public static DriveInfo[] GetLogicalDrivesInfo() {
			string[] sarrDrives=null;
			DriveInfo[] driveinfoarrReturn=null;
			int iDone=0;
			int iExn=0;
			try {
				Console.Error.Write("Looking for drives");
				sarrDrives=Environment.GetLogicalDrives();
				if (sarrDrives!=null&&sarrDrives.Length>0) {
					Console.Error.Write("...getting info");
					driveinfoarrReturn=new DriveInfo[sarrDrives.Length];
					Console.Error.Write(" for drives");
					Console.Error.Flush();
					for (int iNow=0; iNow<sarrDrives.Length; iNow++) {
						Console.Error.Write(".");
						Console.Error.Flush();
						try {
							if (sarrDrives[iNow]!=null) driveinfoarrReturn[iNow]=new DriveInfo(sarrDrives[iNow]);
							else driveinfoarrReturn[iNow]=null;
						}
						catch (Exception exn) {
							Console.Error.WriteLine();
							iExn++;
							MainForm.ShowExn(exn,"getting drive info","GetLogicalDrivesInfo");
						}
						iDone++;
					}
				}
				if (iExn<=0) Console.Error.WriteLine(((iDone>0)?"":"  ")+"OK");
			}
			catch (Exception exn) {
				if (iExn<=0) Console.Error.WriteLine(((iDone>0)?"":"  ")+"FAILED");
				MainForm.ShowExn(exn,"getting drive array","GetLogicalDrivesInfo");
			}
			return driveinfoarrReturn;
		}//end GetLogicalDrivesInfo //TODO: move GetLogicalDrivesInfo to Base

		private long ThisChunkSize_AccountingForSmallEndChunkIfSmall {
			get {return (ChunkNow==ChunkCount-1)?SizeOfFinalPartialChunk:ChunkSize;}
		}
		public long ChunkSize {
			get {return (ThisChunk!=null)?ThisChunk.Length:0; }
			set {
				try {
					ThisChunk=new byte[value];//debug ignores previous contents
					ResetPositions();//DOES open file and read first chunk
				}
				catch (Exception exn) {
					Console.Error.WriteLine("Exception error setting Chunker ChunkSize: "+exn.ToString());
				}
			}
		}//end ChunkSize
		#endregion variables
		
		#region constructors
		
		public Chunker() {
			Init("",true,DefaultChunkSize);
		}
		public Chunker(string sFile) {
			Init(sFile,false,DefaultChunkSize);
		}
		public Chunker(string sFile, bool bReserved_JustSayFalse) {
			Init(sFile,bReserved_JustSayFalse,DefaultChunkSize);
		}
		~Chunker() {
			if (fsNow!=null) Close();
		}
		private bool Init(string sFile, bool bForWriting, int SetChunkSize) {
			ChunkSize=SetChunkSize;
			return Open(sFile,bForWriting);//DOES reset positions, which reads first chunk
		}
		#endregion constructors

		#region file methods
		///<summary>
		///Reads the next byte as a boolean
		///</summary>
		public bool Read(out bool val) {
			byte byVal;
			bool bTest=Read(out byVal);
			val=byVal!=0;
			return bTest;
		}
		public bool Read(out byte val) {
			bool bHasThat=(ByteNowAbsolute<FileLength);//< ONLY ok since 1 byte!
			if (bHasThat) {
				val=ThisChunk[ByteNowRelative];
				Advance();
			}
			else {
				val=0;
				Console.Error.WriteLine("Error: \""+sLastFile+"\" does not have another byte value.");
			}
			return bHasThat;
		}
		///<summary>
		///Reads a 1-byte ASCII character to val else returns false.
		///</summary>
		public bool ReadAscii(out char val) {
			byte byVal;
			bool bTest=Read(out byVal);
			val=(char)byVal;
			return bTest;
		}
		///<summary>
		///Performs a big-endian read of a 2-byte unicode character to val else returns false.
		///</summary>
		public bool ReadUnicode(out char val) {
			byte byVal;
			bool bTest=Read(out byVal);
			val=(char)byVal;
			if (bTest) {
				bTest=Read(out byVal);
				val=(char)((ushort)val+((ushort)byVal)<<8);// <<8 is the same as *256
			}
			//else Console.Error.WriteLine("Error: tried to read unicode character starting after file ended.");
			return bTest;
		}//end ReadUnicode
		#endregion file methods
		
		#region utilities
		/* see Common.cs
		public static string ConvertDirectorySeparatorsToNormal(string sSrcPathX) {
			string sReturn="";
			char cNow;
			if (sSrcPathX!=null) {
				for (int iNow=0; iNow<sSrcPathX.Length; iNow++) {
					cNow=sSrcPathX[iNow];
					if (IsAnyDirectorySeparator(cNow)) cNow=Path.DirectorySeparatorChar;
					sReturn+=char.ToString(cNow);
				}
			}
			return sReturn;
		}//end ConvertDirectorySeparatorsToNormal
		public static string sLastExn="";
		public static bool CreateFolderRecursively(ref ArrayList alFoldersCreated_FullNameCollection, string sFullPath) {
			bool bGood=false;
			sLastExn="";
			int iEnder=0;
			if (alFoldersCreated_FullNameCollection==null) alFoldersCreated_FullNameCollection=new ArrayList();
			try {
				DirectoryInfo diNow=new DirectoryInfo(sFullPath);
				if (!diNow.Exists) {
					string sDirSep=char.ToString(Path.DirectorySeparatorChar);
					if (!sFullPath.EndsWith(sDirSep)) sFullPath+=sDirSep;
					while (iEnder<sFullPath.Length) {
						iEnder=sFullPath.IndexOf(Path.DirectorySeparatorChar,iEnder+1);
						if (iEnder>-1) {
							if (!Directory.Exists(sFullPath.Substring(0,iEnder))) {
								alFoldersCreated_FullNameCollection.Add(sFullPath.Substring(0,iEnder));
								try {
									Directory.CreateDirectory(sFullPath.Substring(0,iEnder));
									bGood=true;
								}
								catch (Exception exn) {
									Console.Error.WriteLine("Error in CreateFolderRecursively(\""+sFullPath+"\") {current-substring:"+sFullPath.Substring(0,iEnder)+"}:");
									Console.Error.WriteLine(exn.ToString());
									Console.Error.WriteLine();
									sLastExn=exn.ToString();
								}
							}
						}
						else break;
					}
				}
				else bGood=true;
			}
			catch (Exception exn) {
				Console.Error.WriteLine("Error in CreateFolderRecursively(\""+sFullPath+"\") {current-substring:"+sFullPath.Substring(0,iEnder)+"}:");
				Console.Error.WriteLine(exn.ToString());
				Console.Error.WriteLine();
				sLastExn=exn.ToString();
			}
			return bGood;
		}//end CreateFolderRecursively
		public static int IndexOfAnyDirectorySeparatorChar(string Haystack) {
			int iFound=-1;
			if (Haystack!=null&&Haystack!="") {
				for (int iNow=0; iNow<Haystack.Length; iNow++) {
					if (Chunker.IsAnyDirectorySeparator(Haystack[iNow])) {
						iFound=iNow;
						break;
					}
				}
			}
			return iFound;
		}//end IndexOfAnyDirectorySeparatorChar
		public static string RemoveDoubleDirectorySeparators(string sPathX) {
			string sReturn="";
			bool bPrevWasDirSep=false;
			if (sPathX!=null) {
				for (int iNow=0; iNow<sPathX.Length; iNow++) {
					char cNow=sPathX[iNow];
					if (!bPrevWasDirSep||!Chunker.IsAnyDirectorySeparator(cNow)) {
						sReturn+=char.ToString(cNow);
					}
					if (cNow==Path.DirectorySeparatorChar) bPrevWasDirSep=true;
					else bPrevWasDirSep=false;
				}
			}
			return sReturn;
		}//end RemoveDoubleDirectorySeparators
		*/
		public static int ChunksNeeded(int iUnits, int iUnitsPerChunk) {
			return (int)(iUnits/iUnitsPerChunk)+(iUnits%iUnitsPerChunk!=0?1:0);
		}
		public static long ChunksNeeded(long iUnits, long iUnitsPerChunk) {
			return (long)(iUnits/iUnitsPerChunk)+(iUnits%iUnitsPerChunk!=0?1:0);
		}
		private void CalculateVars() {
			if (fsNow!=null) {
				FileLength=(fsNow!=null)?fsNow.Length:0;
				SizeOfFinalPartialChunk=SizeOfFinalPartialChunk%ChunkSize;
				ChunkCount=ChunksNeeded(FileLength, ChunkSize);//=(int)(iUnits/iUnitsPerChunk)+(iUnits%iUnitsPerChunk!=0?1:0);
			}
			//else Console.Error.WriteLine("Error in Chunker: Tried to calculate file statistics without a file open!");
		}
		//Contains(string sFile, byte[] byarrNeedle) { //see Common.cs
		public bool ContainsAt(byte[] byarrNeedle, long iAt) {
			SetPosition(iAt);
			byte valNow;
			int iRel=0;
			int iMatches=0;
			while (Read(out valNow)&&iRel<byarrNeedle.Length) {
				if (valNow==byarrNeedle[iRel]) iMatches++;
				else break;
				iRel++;
			}
			return iMatches==byarrNeedle.Length;
		}//end ContainsAt
		public bool Contains(byte[] byarrNeedle) {
			bool bReturn=false;
			try {
				long iBytes=fsNow.Length;
				if (fsNow.Length==0) Console.Error.WriteLine("0 Bytes read from last file \""+sLastFile+"\"");
				if (bDebug) {
					Console.Error.Write("Checking "+iBytes.ToString()+" bytes...");
					Console.Error.Flush();
				}
				for (long iNow=0; iNow<iBytes; iNow++) {
					if (ContainsAt(byarrNeedle,iNow)) {
						bReturn=true;
						break;
					}
				}
				if (bDebug) {
					Console.Error.WriteLine("done checking \""+sLastFile+"\" for content ("+(bReturn?"found":"not found")+").");
					Console.Error.WriteLine();
				}
			}
			catch (Exception exn) {
				Console.Error.WriteLine("Error in chunker.Contains while checking file \""+this.sLastFile+"\":");
				Console.Error.WriteLine(exn.ToString());
				Console.Error.WriteLine();
			}
			return bReturn;
		}//end Contains
		
		#endregion utilities
		
		#region file operations
		private bool Open(string sFile, bool bForWriting) {
			bool bGood=true;
			if (!bForWriting) {
				bGood=OpenRead(sFile);
			}
			else {
				bGood=false;
				Console.Error.WriteLine("Chunker is not for writing files, only reading.");
			}
			return bGood;
		}
		public bool OpenRead(string sFile) {
			bool bGood=true;
			sLastFile=sFile;
			if (fsNow!=null) {
				Console.Error.WriteLine("WARNING: Chunker opened new file for reading, so old file was automatically closed.");
				Close();
			}
			try {
				ResetPositions();//DOES open file and set first chunk
				CalculateVars();
			}
			catch (Exception exn) {
				Console.Error.WriteLine("Chunker error opening \""+sFile+"\":");
				Console.Error.WriteLine(exn.ToString());
				Console.Error.WriteLine();
				bGood=false;
			}
			return bGood;
		}
		public bool Close() {
			bool bGood=true;
			try {
				fsNow.Close();
				fsNow=null;
			}
			catch (Exception exn) {
				bGood=false;
				Console.Error.WriteLine("Chunker error closing \""+sLastFile+"\":");
				Console.Error.WriteLine(exn.ToString());
				Console.Error.WriteLine();
			}
			return bGood;
		}
		private bool Advance() {
			bool bGood=true;
			long ByteNewAbsolute=ByteNowAbsolute+1;
			try {
				if (ByteNewAbsolute<=Length) {//if read
					if (ByteNewAbsolute<Length) {
						if (ChunkNow<(ByteNewAbsolute/ChunkSize)) {
							fsNow.Read(ThisChunk,0,ThisChunk.Length);//2nd param is position in the array NOT the file.
							ChunkNow++;
							//nByteNowRelative=0;
						}
						//else nByteNowRelative++;
						ByteNowAbsolute=ByteNewAbsolute;
					}
				}
				else {
					Console.Error.WriteLine("Cannot advance to location ["+ByteNewAbsolute.ToString()+"] of "+Length.ToString()+"-length file \""+sLastFile+"\".");
					bGood=false;
				}
			}
			catch (Exception exn) {
				bGood=false;
				Console.Error.WriteLine("Chunker error advancing in \""+sLastFile+"\":");
				Console.Error.WriteLine(exn.ToString());
				Console.Error.WriteLine();
			}
			return bGood;
		}
		private bool Advance(int iBytesToAdvance) {
			bool bGood=true;
			for (int iNow=0; iNow<iBytesToAdvance; iNow++) {
				if (!Advance()) {
					bGood=false;
					break;
				}
			}
			return bGood;
		}
		private void SetPosition(long Loc) {
			ResetPositions();
			byte byTemp;
			if (ChunkCount==1) {
				ByteNowAbsolute=Loc;
				ChunkNow=0;
			}
			else {
				for (long iNow=0; iNow<Loc; iNow++) {
					this.Read(out byTemp);
				}
			}
		}
		private void ResetPositions() {
			ByteNowAbsolute=0;
			ChunkNow=0;
			try {
				ByteNowAbsolute=0;
				ChunkNow=0;
				if (fsNow==null||FileLength>ChunkSize) {
					if (fsNow!=null) {
						Close();
						fsNow=null;
					}
					if (sLastFile!="") {
						fsNow=new FileStream(sLastFile, FileMode.Open, FileAccess.Read);
						ChunkCount=ChunksNeeded(fsNow.Length,ChunkSize);
						fsNow.Read(ThisChunk,0,ThisChunk.Length);//load first chunk
					}
				}
				else {
					ChunkCount=1;
				}
			}
			catch (Exception exn) {
				Console.Error.WriteLine("Chunker cannot reset position: \""+sLastFile+"\".");//UniWinForms.WriteLine(exn);
				Console.Error.WriteLine(exn.ToString());
				Console.Error.WriteLine();
			}
		}
		#endregion file operations
		
	}//end Chunker
}//end namespace
