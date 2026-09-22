[Setup]
AppName=MSFS24 AI ATC
AppVersion=0.2.0
AppPublisher=APGSwizzVR
DefaultDirName={autopf}\MSFS24 AI ATC
DefaultGroupName=MSFS24 AI ATC
OutputDir=output
OutputBaseFilename=MSFS24-AI-ATC-Setup
ArchitecturesInstallIn64BitMode=x64
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=admin

[Files]
Source: "..\src\MSFS24AiAtc\bin\x64\Release\net8.0-windows\win-x64\publish\*"; DestDir: "{app}"; Flags: recursesubdirs ignoreversion
Source: "..\lib\SimConnect\SimConnect.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\lib\SimConnect\Microsoft.FlightSimulator.SimConnect.dll"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\MSFS24 AI ATC"; Filename: "{app}\MSFS24AiAtc.exe"
Name: "{autodesktop}\MSFS24 AI ATC"; Filename: "{app}\MSFS24AiAtc.exe"

[Run]
Filename: "{app}\MSFS24AiAtc.exe"; Description: "Launch MSFS24 AI ATC"; Flags: nowait postinstall skipifsilent
