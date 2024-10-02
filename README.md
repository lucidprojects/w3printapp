# w3printapp

## To build an installer
1. Install **Visual Studio Build Tools**: [download link](https://aka.ms/vs/17/release/vs_BuildTools.exe). Make sure to select ".NET desktop build tools" (see the screenshot below).
2. Download and install the latest version of **Inno Setup**: [download link](https://jrsoftware.org/isdl.php).
3. Run the `make.bat` file.
4. Find the Dev/Prod installers in the `_Output` folder.
5. **Dev** and **Prod** installers differ only by the bundled configuration file (containing the backend service URL).

![image](https://github.com/user-attachments/assets/83ca85bb-d086-420d-88eb-db3ad22f8daa)

## To install on a user machine
1. If a version prior to **2.1.0.11** is installed on the user's machine, you will need to uninstall it manually.
2. Simply run either the **Dev** or **Prod** installer.
3. The installer will download and install **.NET Framework 4.8** if it's not already present.
4. The installer upgrades existing versions (starting from v2.1.0.11), so no need for prior uninstallation.

## High-level changes since v2.1.0.11
- The app is now compatible with newer versions of Windows.
- RAW printing modes have been replaced with the **PdfiumViewer** library for printing.
- Upgraded to **.NET Framework 4.8**.
- UI modernization across the entire app.
- Significant code cleanup and refactoring.
- Reduced UI flickering.
- Fixed crashes caused by unhandled exceptions.
- Added an "About" box.
