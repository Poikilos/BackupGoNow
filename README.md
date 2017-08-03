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
* ? fix commented line in main: Application.SetCompatibleTextRenderingDefault(false);

## Developer Notes
* This program requires Microsoft® .NET® framework 2.0 compiler or later, or Mono 2.0 compiler or later to compile.
* Any files in folder named "share" may need to be placed in the working directory of the program

