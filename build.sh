#!/bin/sh
# SET PATH=%PATH%;C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727
outPath="bin/Backup GoNow.exe"
if [ -f "$outPath" ]; then
    echo "moving exe with old naming..."
    mv $outPath $outPath.wip
fi
outPath="bin/BackupGoNow.exe"
if [ -f "$outPath" ]; then
    echo "renaming old version..."
    mv "$outPath" "$outPath.wip"
fi
topDir="`pwd`"
if [ -f "$HOME/bgn.sh" ]; then
    echo "update the updater ('$HOME/bgn.sh')..."
    cd /tmp
    wget -O bgn.sh https://github.com/poikilos/linux-preinstall/raw/master/everyone/optional/bgn.sh && mv -f bgn.sh $HOME/
    cd "$topDir"
fi
# NOTE: If above was in quotes, mac put a quote in the filename and then
# had 'cannot contain "'  error when I try to rename.
if [ ! -d bin ]; then mkdir bin; fi
# formerly gmcs:
echo "calling mcs to build..."
mcs /target:winexe /out:$outPath /win32icon:BackupGoNow.ico \
    AssemblyInfo.cs \
    ../ForwardFileSync/Common.cs \
    ../ForwardFileSync/LocInfo.cs \
    MainForm.cs \
    MainForm.Designer.cs \
    Program.cs \
    MyCallBack_WinForms.cs \
    /resource:MainForm.resx,MainForm \
    /r:System.Drawing.dll \
    /r:System.Windows.Forms.dll \
    /r:System.Data.dll \
    /r:System.Xml.dll \
    1>out.txt 2>err.txt
# if [ -f "`command -v outputinspector`" ]; then
    # outputinspector
# fi
