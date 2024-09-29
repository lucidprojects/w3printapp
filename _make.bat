@SET msbuild="C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\amd64\MSBuild.exe"
@SET innosetup="C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
@SET output=_Output

@ECHO.

MD %output%

@ECHO ----------------------------------------------------
@ECHO Deleting older installers ...
@IF EXIST .\%output%\*.exe DEL .\%output%\*.exe

@ECHO ----------------------------------------------------
@ECHO Building User version...

@%msbuild% -restore PrintInvoice.sln /t:Rebuild /graph /m /p:Configuration=Release /p:Platform="Any CPU"
@IF ERRORLEVEL 1 (
  @ECHO ----------------------------------------------------
  @ECHO ERROR: Failed to build the code, check the errors above
  @EXIT
)

@DEL bin\Release\*.pdb
@RMDIR /s /y bin\Release\nl

@ECHO ----------------------------------------------------
@ECHO Building installer...
@%innosetup% /O"%output%" "PrintInvoice.iss"
@IF ERRORLEVEL 1 (
  @ECHO ----------------------------------------------------
  @ECHO ERROR: Failed to build installer, check the errors above
  @EXIT
)