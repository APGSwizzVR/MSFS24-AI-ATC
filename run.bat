@echo off
setlocal
cd /d "%~dp0"

if exist "MSFS24AiAtc.exe" (
    start "MSFS24 AI ATC" "MSFS24AiAtc.exe"
    exit /b 0
)

if exist "publish\MSFS24AiAtc.exe" (
    start "MSFS24 AI ATC" "publish\MSFS24AiAtc.exe"
    exit /b 0
)

if exist "src\MSFS24AiAtc\bin\Release\net8.0-windows\win-x64\publish\MSFS24AiAtc.exe" (
    start "MSFS24 AI ATC" "src\MSFS24AiAtc\bin\Release\net8.0-windows\win-x64\publish\MSFS24AiAtc.exe"
    exit /b 0
)

echo MSFS24 AI ATC executable was not found.
echo Build or publish the application first, then run this file again.
pause
exit /b 1
