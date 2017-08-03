/*
 *  Created by SharpDevelop (To change this template use Tools | Options | Coding | Edit Standard Headers).
 * User: Jake Gustafson (Owner)
 * Date: 1/25/2007
 * Time: 12:52 PM
 * 
 */

using System;

namespace GoNowBackup {
	/// <summary>
	/// Description of Actions.
	/// </summary>
	public class Action {
		//Notes:
		//-Indeces of sarrCommands MUST match Type constants
		//-Members of sarrCommands must be lowercase, since commands are case insensitive and converted to lowercase
		public static readonly string[] sarrCommands= {"#","<file>","<folder>","<folder ignoring subfolders>",
												"<save to file>","<save to folder>","<save to drive>","<command>"};
		public static readonly bool[] barrCommandIsAssignment={false,false,false,false,
														true,true,true,true};
		public const int TypeIgnore =			0;
		public const int TypeFile =				1;
		public const int TypeFolder =			2;
		public const int TypeFolderRecursive =	3;
	
		public const int TypeSetTargetFile =	4;
		public const int TypeSetTargetFolder =	5;
		public const int TypeSetTargetDrive =	6;
		public const int TypeSetCommand =		7;
		public static bool IsUsable(int ActionType) {
			return (ActionType>=0 && ActionType<sarrCommands.Length && ActionType!=TypeIgnore);
		}
		public static bool IsAssignment(int ActionType) {
			bool bReturn=false;
			if (ActionType<barrCommandIsAssignment.Length&&ActionType>=0) {
				bReturn=barrCommandIsAssignment[ActionType];
			}
			return bReturn;
		}
		public static int FromLine(out string sDataSubstringReturn, string sCommandLine) {
			bool bGood=false;
			int iType=-1;
			sDataSubstringReturn="";
			try {
				if (sCommandLine=="") {
					iType=TypeIgnore;
					sDataSubstringReturn="";
				}
				for (int iNow=0; iNow<sarrCommands.Length; iNow++) {
					if (  sCommandLine.ToLower().StartsWith(sarrCommands[iNow] )
					  &&  ( sCommandLine.Length>sarrCommands[iNow].Length )  ) {
						iType=iNow;
						sDataSubstringReturn=sCommandLine.Substring(sarrCommands[iNow].Length,sCommandLine.Length-sarrCommands[iNow].Length);
						break;
					}
				}
			}
			catch (Exception exn) {
				MainForm.ShowError("Exception in test for GoodLine: "+exn.ToString());
				iType=-1;
				sDataSubstringReturn="";
			}
			return iType;
		}//end CommandFromLine
	}//end class Action
}//end namespace
