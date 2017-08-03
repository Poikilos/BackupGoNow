# GoNowBackup
A backup program that anyone can use the first time, with good defaults!

## License
* Only use and copy in accordance with the LICENSE file.
* Author: Jacob Gustafson ( http://github.com/expertmm http://www.expertmultimedia.com )

## Usage
* After installing the program, modify settings using settings.txt in program's folder.

## Troubleshooting
* If the program fails to load, client needs to download the runtime (such as .NET framework or Mono)

## Changes
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

