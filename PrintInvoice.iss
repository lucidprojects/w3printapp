#define ExeName = "W3Pack"
#define ExeFolder "bin\Release\"

[Setup]
AppId={{F4A17E0D-3D35-4FC1-ADAF-D3D5EB604F6D}

#define ExeNameExt ExeName + ".exe"
#define ExePath ExeFolder + ExeNameExt
#define ExeVersion GetVersionNumbersString(ExePath)
#define ExeProduct GetStringFileInfo(ExePath, "ProductName")
#define ExeDescription GetStringFileInfo(ExePath, "FileDescription")
#define ExeCompany GetStringFileInfo(ExePath, "CompanyName")

[Setup]
AppName={#ExeProduct}
AppVersion={#ExeVersion}
AppVerName={#ExeProduct} v{#ExeVersion}
AppPublisher={#ExeCompany}
DefaultDirName={userappdata}\{#ExeProduct}
DefaultGroupName={#ExeCompany}\{#ExeProduct}
OutputBaseFilename={#ExeName}_v{#ExeVersion}_Setup
Compression=lzma
SolidCompression=no
UninstallDisplayIcon={userappdata}\{#ExeProduct}\{#ExeNameExt}
PrivilegesRequired=lowest

[Files]
Source: "{#ExeFolder}*.*"; DestDir: "{userappdata}\{#ExeProduct}"; Flags: ignoreversion recursesubdirs

[Icons]
Name: "{group}\{#ExeProduct}"; Filename: "{userappdata}\{#ExeProduct}\{#ExeNameExt}";
Name: "{userdesktop}\{#ExeProduct}"; Filename: "{userappdata}\{#ExeProduct}\{#ExeNameExt}";

[Run]
Filename: "{userappdata}\{#ExeProduct}\{#ExeNameExt}"; Description: "{cm:LaunchProgram,{#StringChange(ExeProduct, '&', '&&')}}"; Flags: nowait postinstall runascurrentuser