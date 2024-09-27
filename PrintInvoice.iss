#define ExeName = "W3Pack"
#define ExeFolder "bin\Release\"

[Setup]
AppId={{9CF4F1A4-E080-4435-8DFB-9C13DB6FBA60}

#define ExeNameExt ExeName+".exe"
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
AppPublisherURL={#MyURL}
DefaultDirName={userappdata}\{#ExeProduct}
DefaultGroupName={#ExeCompany}\{#ExeProduct}
OutputBaseFilename={#ExeName}_v{#ExeVersion}_Setup
Compression=none
SolidCompression=no
UninstallDisplayIcon={userappdata}\{#ExeProduct}\{#ExeNameExt}
PrivilegesRequired=lowest

[Files]
Source: "{#ExeFolder}*.*"; DestDir: "{userappdata}\{#ExeProduct}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#ExeProduct}"; Filename: "{userappdata}\{#ExeProduct}\{#ExeNameExt}";

[Run]
Filename: "{userappdata}\{#ExeProduct}\{#ExeNameExt}"; Description: "{cm:LaunchProgram,{#StringChange(ExeProduct, '&', '&&')}}"; Flags: nowait postinstall runascurrentuser