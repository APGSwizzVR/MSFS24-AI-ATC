@echo off
setlocal EnableExtensions
cd /d "%~dp0"

set "APP=MSFS24AiAtc.exe"
set "PUBLISH=src\MSFS24AiAtc\bin\Release\net8.0-windows\win-x64\publish\MSFS24AiAtc.exe"
set "PROJECT=src\MSFS24AiAtc\MSFS24AiAtc.csproj"

if exist "%APP%" goto launch_root
if exist "%PUBLISH%" goto launch_publish
where dotnet >nul 2>&1
if errorlevel 1 goto no_dotnet

echo ========================================
echo MSFS24 AI ATC - Building application
echo ========================================
echo.
echo First run: this may take a few minutes.
echo.
dotnet publish "%PROJECT%" -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
if errorlevel 1 goto build_failed

if not exist "%PUBLISH%" goto build_failed
goto launch_publish
:launch_root
echo Starting MSFS24 AI ATC...
start "MSFS24 AI ATC" "%APP%"
goto done
:launch_publish
echo Starting MSFS24 AI ATC...
start "MSFS24 AI ATC" "%PUBLISH%"
goto done
:no_dotnet
echo.
echo .NET 8 SDK was not found.
echo Install the .NET 8 SDK, then run this file again.
echo.
pause
goto end
:build_failed
echo.
echo The application build failed.
echo Check the messages above for the actual error.
echo.
pause
goto end
:done
echo MSFS24 AI ATC launched.
timeout /t 2 /nobreak >nul
:end
endlocal
exit /b 0
