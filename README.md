# Backup GoNow
A backup program that anyone can use the first time, with good defaults!

## License
* Only use and copy in accordance with the LICENSE file.
* Author: Jacob Gustafson ( http://github.com/expertmm http://www.expertmultimedia.com )

## Usage
* After installing the program, modify settings using settings.txt in program's folder.

## Troubleshooting
* If the program fails to load, client needs to download the runtime (such as .NET framework or Mono)

## Changes
* (2016-02-21) places files in "tl" folder if file path is too long to backup properly otherwise (and records full source path for use during restore) 
* (2016-02-21) single "launcher" file can be placed anywhere and generates any files it needs in your user profile
* (2015-07-30) Changed default profile -- main.ini: no longer use DestSubFolder option, no longer exclude D:, exclude some standard names of recovery drives (see Developer Notes section below) in order to not use them as destinations; script.txt: add only specific APPDATA and LOCALAPPDATA
* (2012-07-12) Fixed problem where showing incorrect error message due to comparing index >-2 instead of >-1 (added new messages for "being used by another process" & "UnauthorizedAccessException" instead of telling user that the filename was too long in those cases)
* (2012-07-12) only change lByteCountTotalActuallyAdded if Copy succeeded
* (2011-02-22) Moved general file handling and drive management methods to Common.cs and LocInfo.cs, and now these files are references to those from the ForwardFileSync project.
* (2010-12-12) added option to view log (using default txt viewer) under help menu
* (2010-12-12) remove whitespace from beginning and end of line in RunScriptLine method
* (2010-12-12) add RemoveEndsWhiteSpaceByRef method to Common.cs
* (2010-12-12) display line being run (in listbox using the format "   RunScriptLine(...)")
* (2010-11-18) display name of log file when done and say "(statistics)"
* (2010-11-14) Message should say out of space instead of filename too long when out of space exception string contains "system.io.ioexception: there is not enough space on the disk"
* (~2010-11-13) create batch file of failed directory creation and file copy operations 2009-12-10
* (~2010-11-13) send param of Output(...) method (uses lbOut) to Console.WriteLine (done 2009-12-09)
* (~2010-11-13) account for files previously backed up manually (so they aren't deleted).  
* (2010-02-10) estimates time remaining based on last run (using summary.log in profile folder)
* (2009-09-09) shows message when "Removing deleted/moved folder from backup"
* (2009-09-09) no longer overwrites default profile if exists
* (2009-09-08) added profiles feature and default profile (profiles folder must be in current working directory; profile is specified by startup.ini)
* (2009-09-08) Added new setting "AlwaysStayOpen:no" in main.ini: If "no", program closes after run unless there are copy errors, in which case program now asks to review list and "Cancel" quits Backup GoNow.
* (2009-06-09) create reconstructed source path when adding single files with the option to use said path (prevents missing destination folder exception)
* (2009-06-09) delete folders on backup that don't exist on source (previously only files were deleted)
* (2008-10-06) split settings into main.ini and script.txt, so main.ini excludes drives from list before script.txt runs
* (2008-10-06) renamed from GoNowBackup to Backup GoNow
* (2008-10-06) added dependency: FolderLister.cs and Chunker.cs from FolderLister
* (2007-08-15) removed infinite recursion in clean.sh
* (2007-08-15) updated build.sh to account for refactoring
* (2007-08-15) Turned off AutoScale for MainForm
* (2007-07-25) changed default settings to also backup self on linux
* (2007-07-25) updated MainFormFormClosed and MainFormFormClosing to use System.EventArgs
* (2007-07-25) commented broken line in main: Application.SetCompatibleTextRenderingDefault(false);
* (2007-01-25) prevent pushing button twice from crashing program

## Known Issues
* There is no restore option. All restores must be done manually, including files in the tl folder which were too long to backup using the same folder structure.
* Make sure AddFile IGNORES all filters (folder AND file filters!)
* InternalIndexOfPseudoRootWhereFolderStartsWithItsRoot and other drive handling methods should be called with case senstive set to TRUE if sDirSep is '/'.
* Change program menu icon (on menu bar) to custom icon (instead of compiler logo)
* Add separate command in menu to delete all files on dest that aren't on source (ONLY do UNDER drive folders, NOT top-level)
* (optimization) do not traverse folder if folder date is same as backup
* always exclude when name with wildcard IsLike: ?:\System Volume Information, ?:\Config.Msi, ?:\RECYCLED, ?:\RECYCLER, ?:\$Recycle.Bin, ?:\.Trashes, ?:\MSOCache, ?:\Boot, ?:\Recovery
* IF CAN'T DISPLAY FREE SPACE, show used space instead (count existing space used first)
* fix bug where says finished reading script text file even if user cancelled
* fix bug where cannot recreate destination folder (check for DirectoryInfo overlap that would cause inability to write subfolder)
* Now that listbox output is minimal, log file should be written in realtime
  * make sure that EVERYTHING written to listbox is logged
* Make filters AND masks case-insensitive
* Limit size of files to backup
* Fix whatever is causing this Console.Error output:
	Could not finish backing up file in BackupFile
	System.IO.IOException: There is not enough space on the disk.
		at System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)
		at System.IO.File.InternalCopy(String sourceFileName, String destFileName, Boolean overwrite)
		at OrangejuiceElectronica.MainForm.BackupFile(String sSrcFilePath, Boolean bUseReconstructedPath, FileInfo fiNow)
	Could not finish backing up file in BackupFile
	System.IO.IOException: There is not enough space on the disk.
		at System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)
		at System.IO.File.InternalCopy(String sourceFileName, String destFileName, Boolean overwrite)
		at OrangejuiceElectronica.MainForm.BackupFile(String sSrcFilePath, Boolean bUseReconstructedPath, FileInfo fiNow)
	Process is terminated due to StackOverflowException.

* Call conditional refresh (including updating scrollbar values) EVERY time a file begins AND ends copying EVEN IF doesn't need to be copied
	* problem may be caused by copying large files so create thread for file operations.
		* possibly calculate disc write speed (of course, only use files that are actually need to be copied and copy successfully)
			* then create a per-file progress bar for files that take more than a certain amount of time (change to animated bar if reaches expected time then back to regular after done)
			* OR OPTION 2: create a percenage indicator (MUST go up to only 99%!) inside message label and say "exceeded estimated time" if goes over.
* ! Make backup able to overwrite readonly files that changed (if doesn't)!
* Make backup able to delete readonly files that have been backed up
* Running Backup GoNow on Windows 7 did NOT remove Firefox's prefs.js from :\Backup\C\Document and Settings\...\
  (did not remove Documents and Settings, possibly because it exists as a virtual folder of some sort in Windows 7)
* Allow "*" in path (i.e. for Outlook identity or mozilla profile folder)
  * ALREADY USED - see script.txt
* Delete folders from backup that are not currently being backed up
  * somehow account for subfolders that were backed up using different masks
  * account for files that will be backed up later in the script
  (see "account for files previously backed up manually" in Changes section)
* add a "Pause" button (multithreaded+callback)
* ? fix commented line in main: Application.SetCompatibleTextRenderingDefault(false);

## Developer Notes
* This program requires Microsoft® .NET® framework 2.0 compiler or later, or Mono 2.0 compiler or later to compile.
* Any files in folder named "share" may need to be placed in the working directory of the program
* main.ini should always exclude the following standard labels of recovery drives:
	```
	#next line is for Windows 8 (R) default C: drive label on computers such as Lenovo (R) laptop circa 2014
	ExcludeDest:Windows8_OS
	#next line is Lenovo(R) recovery partition
	ExcludeDest:LENOVO
	ExcludeDest:HP_RECOVERY
	ExcludeDest:RECOVERY
	#next line is HP(R) C: drive circa 2006 such as Vista(R)
	ExcludeDest:OS
	#next line is HP(R) factory image circa 2006 such as Vista(R)
	ExcludeDest:FACTORY_IMAGE
	#next line is Dell(R) recovery
	ExcludeDest:Recovery
	ExcludeDest:DELLUTILITY
	```
