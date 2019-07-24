#!/bin/sh
#SET PATH=%PATH%;C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727
if [ -f "bin/Backup\ GoNow.exe" ]; then
    mv bin/Backup\ GoNow.exe bin/Backup\ GoNow.exe.wip
fi
#NOTE: if above was in quotes, mac put a quote in the filename and then had 'cannot contain "'  error when I try to rename!
#formerly gmcs:
mcs /target:winexe /out:bin/Backup\ GoNow.exe /win32icon:BackupGoNow.ico \
    AssemblyInfo.cs \
    Common.cs \
    LocInfo.cs \
    MainForm.cs \
    MainForm.Designer.cs \
    Program.cs \
    ../FolderLister/src/FolderLister.cs \
    ../UniWinForms/src/Chunker.cs \
    /resource:MainForm.resx,MainForm \
    /r:System.Drawing.dll \
    /r:System.Windows.Forms.dll \
    /r:System.Data.dll \
    /r:System.Xml.dll
