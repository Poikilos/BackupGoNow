copy "Debug\Backup GoNow.exe" .
del "Debug\Backup GoNow.exe"
REM copy "Release\Backup GoNow.exe" .
REM del "Release\Backup GoNow.exe"
"Backup GoNow.exe" 1>out.txt 2>err.txt
