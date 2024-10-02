@SET msbuild="C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\amd64\MSBuild.exe"
@SET innosetup="C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
@SET output=_Output

@ECHO.

@RMDIR /s /y %output%
@MD %output%

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

@XCOPY /Y Configs\dev.config bin\Release\W3Pack.exe.config

@ECHO ----------------------------------------------------
@ECHO Building DEV installer...
@%innosetup% /DSuffix=DEV /O"%output%" "PrintInvoice.iss"
@IF ERRORLEVEL 1 (
  @ECHO ----------------------------------------------------
  @ECHO ERROR: Failed to build DEV installer, check the errors above
  @EXIT
)

@XCOPY /Y Configs\prod.config bin\Release\W3Pack.exe.config

@ECHO ----------------------------------------------------
@ECHO Building PROD installer...
@%innosetup% /DSuffix=PROD /O"%output%" "PrintInvoice.iss"
@IF ERRORLEVEL 1 (
  @ECHO ----------------------------------------------------
  @ECHO ERROR: Failed to build PROD installer, check the errors above
  @EXIT
)