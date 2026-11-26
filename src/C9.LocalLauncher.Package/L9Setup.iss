; Inno Setup 6 script for the C9 Local Launcher (MAUI). Build it with
;   scripts\Build-C9Launcher.ps1 -Installer
; Installs per user into %LOCALAPPDATA%\Programs\L9 (no administrator rights): L9.exe and
; L9.local.json on top, the launcher runtime in bin\. L9 writes its logs beside L9.exe.
#ifndef SourceDir
  #error Pass /DSourceDir=<artifacts\launcher\win-x64> (folder with L9.exe and bin\)
#endif
#ifndef AppVersion
  #define AppVersion "2.0"
#endif
#ifndef Arch
  #define Arch "x64compatible"
#endif
#ifndef SetupName
  #define SetupName "L9-Setup-" + AppVersion
#endif

[Setup]
AppId={{5C0E7B6A-3F2D-4E19-9A8B-6D4C2E1F9A07}
AppName=C9 Local Launcher
AppVersion={#AppVersion}
AppPublisher=C9 Local
DefaultDirName={autopf}\L9
DefaultGroupName=C9 Local Launcher
DisableProgramGroupPage=yes
PrivilegesRequired=lowest
ArchitecturesAllowed={#Arch}
ArchitecturesInstallIn64BitMode={#Arch}
MinVersion=10.0.19041
OutputBaseFilename={#SetupName}
Compression=lzma2/max
SolidCompression=yes
WizardStyle=modern
UninstallDisplayIcon={app}\L9.exe
UninstallDisplayName=C9 Local Launcher

[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[InstallDelete]
; A new version replaces the whole runtime folder, so no DLL from an older version stays behind.
Type: filesandordirs; Name: "{app}\bin"

[Files]
Source: "{#SourceDir}\L9.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceDir}\bin\*"; DestDir: "{app}\bin"; Excludes: "*.pdb"; Flags: ignoreversion recursesubdirs createallsubdirs
; The build machine's configuration, if any; an existing L9.local.json is never overwritten or removed.
Source: "{#SourceDir}\L9.local.json"; DestDir: "{app}"; Flags: onlyifdoesntexist uninsneveruninstall skipifsourcedoesntexist

[Icons]
Name: "{autoprograms}\C9 Local Launcher"; Filename: "{app}\L9.exe"; WorkingDir: "{app}"
Name: "{autodesktop}\C9 Local Launcher"; Filename: "{app}\L9.exe"; WorkingDir: "{app}"; Tasks: desktopicon

[Run]
Filename: "{app}\L9.exe"; Description: "{cm:LaunchProgram,C9 Local Launcher}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
Type: files; Name: "{app}\L9.*.log"
