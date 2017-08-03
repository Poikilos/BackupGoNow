REM copy "Debug\Backup GoNow.exe" .
REM del "Debug\Backup GoNow.exe"
REM copy "Release\Backup GoNow.exe" .
REM del "Release\Backup GoNow.exe"
del out.txt
del err.txt
"Backup GoNow.exe" 1>out.txt 2>err.txt
