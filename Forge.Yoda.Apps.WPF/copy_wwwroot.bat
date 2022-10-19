@ECHO OFF

SET arg1=%1
SET arg2=%2
SET arg3=%3
SET arg4=%4

REM make sure, only one instance of this script is running at the same time
:init
SET "started="
2>nul (
	9>"%~f0.lock" (
		SET "started=1"
		CALL :start
	)
)
IF defined started (
	DEL "%~f0.lock" >nul 2>nul
) else (
	ECHO Process "%~f0" is already running
	PING localhost > nul
)

GOTO end



:start
CD /d %~dp0

ROBOCOPY "..\Forge.Yoda.Shared.UI.Core\wwwroot" "wwwroot" /MIR
IF %ERRORLEVEL% LSS 8 goto finish

ECHO Something failed & GOTO :eof

:finish
ECHO All done, no fatal errors.

ECHO Updating index.html template...
powershell -Command "(gc wwwroot/index.html) -replace '{PROJECT_NAME}', '%arg1%' | Out-File -encoding UTF8 wwwroot/index.html"
powershell -Command "(gc wwwroot/index.html) -replace '{PROJECT_SPECIFIC_BOOT_FILENAME}', '%arg2%' | Out-File -encoding UTF8 wwwroot/index.html"
powershell -Command "(gc wwwroot/index.html) -replace '{AUTOSTART}', '%arg3%' | Out-File -encoding UTF8 wwwroot/index.html"
powershell -Command "(gc wwwroot/index.html) -replace '{REGISTER_SERVICE_WORKER}', '%arg4%' | Out-File -encoding UTF8 wwwroot/index.html"
ECHO Done.

:end
EXIT /B 0
