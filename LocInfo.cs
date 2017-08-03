/*
 * Created by SharpDevelop.
 * User: Owner
 * Date: 4/28/2009
 * Time: 11:36 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.IO;

namespace OrangejuiceElectronica
{
	/// <summary>
	/// Description of LocInfo.
	/// </summary>
	public class LocInfo {
		public static string sDirSep=char.ToString(Path.DirectorySeparatorChar);

		public string sErr="";//if not blank, then this drive is not usable!
		public string VolumeLabel="";
		public string DriveRoot_FullNameThenSlash="";//public string FullName="";
		public string PrefixFolder_Name="";
		public string DriveLetterFolder="";//only exists if was copied from a windows drive using a windows computer
		public string Subfolder_NameThenSlash_NoStartingSlash="";
		public long TotalSize=0;
		public long UsedSpace=0;
		
		public long AvailableFreeSpace {
			get { return TotalSize-UsedSpace; }
			set { UsedSpace=TotalSize-value; if (UsedSpace>TotalSize) UsedSpace=TotalSize; }
		}
		
		public LocInfo() {
			TotalSize=long.MaxValue;
			UsedSpace=0;
		}
		private void InitBlank(bool bStartWith_sErr_SetToUnusable) {
			sErr=bStartWith_sErr_SetToUnusable?"uninitialized LocInfo":"";
			VolumeLabel="";
			DriveRoot_FullNameThenSlash="";
			PrefixFolder_Name="";
			DriveLetterFolder="";//only exists if was copied from a windows drive using a windows computer
			TotalSize=long.MaxValue;
			UsedSpace=0;
		}
		public bool FromPath(string sPath, string sPrefixFolder, bool bFirst1CharacterFolderAfterPrefixIsDriveLetter) {
			InitBlank(false);
			Console.Error.WriteLine("FromPath is NOT YET IMPLEMENTED");
			return false;
		}
		public void AppendErr(string msg) {
			if (msg==null||msg=="") msg="(unrecorded error)";
			if (this.sErr==null) this.sErr="";
			if (this.sErr!="") this.sErr+="; ";
			this.sErr+=msg;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="liSource"></param>
		/// <param name="DestDriveRoot_FullNameThenSlash"></param>
		/// <param name="DestDrivePrefixFolder_Name"></param>
		/// <returns>if returned LocInfo_Returned.sErr is not blank (""), the drive is unusable for the reason given in the string</returns>
		public bool ToBackupLocInfo(LocInfo liSource, string DestDriveRoot_FullName_MayEndWithSlashOrNot, string DestDrivePrefixFolder_Name) {
			InitBlank(false);
			bool bGood=true;
			try {
				if (DestDriveRoot_FullName_MayEndWithSlashOrNot!=null) {
					if (liSource!=null) {
						this.DriveRoot_FullNameThenSlash=DestDriveRoot_FullName_MayEndWithSlashOrNot;
						if (!this.DriveRoot_FullNameThenSlash.EndsWith(sDirSep)) this.DriveRoot_FullNameThenSlash+=sDirSep;
						
						if (DestDrivePrefixFolder_Name!=null) PrefixFolder_Name=DestDrivePrefixFolder_Name;
						if (PrefixFolder_Name.Contains("/")||PrefixFolder_Name.Contains("\\")) {
							this.AppendErr("Error in ToBackupLocInfo: PrefixFolder contains slash");
						}
						
						if (liSource.DriveRoot_FullNameThenSlash.Length>=2&&liSource.DriveRoot_FullNameThenSlash[1]==':') {
							this.DriveLetterFolder=liSource.DriveRoot_FullNameThenSlash.Substring(0,1);
						}
						else {
							this.DriveLetterFolder="";
						}
					}
					else {
						bGood=false;
						sErr="There was no source location info specified";
					}
				}
				else {
					bGood=false;
					sErr="There was no destination drive root specified";
				}
			}
			catch (Exception exn) {
				bGood=false;
				this.sErr="Could not finish in ToBackupLocInfo:"+Environment.NewLine+exn.ToString();
			}
			return bGood;
		}//end ToBackupLocInfo
	}
}
