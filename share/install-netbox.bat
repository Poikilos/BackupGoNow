SET BGN_PROFILES=%APPDATA%\Backup GoNow\profiles
SET BGN_PROFILE_NAME=Netbox-Win7
SET BGN_PROFILE=%BGN_PROFILES%\%BGN_PROFILE_NAME%
md "%BGN_PROFILE%"
copy /-y "profiles\%BGN_PROFILE_NAME%\*.*" "%BGN_PROFILE%\"
REM -y prevents overwrite
