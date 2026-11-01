Downloadable Options

|File|Description|
|----|----|
| FoneCall_Setup.zip|FoneCall installer executable[^1]|
| FoneCall_Setup+NDP481-Web.zip|FoneCall_Setup and NDP481-Web[^2] executables|
| FoneCall_Portable.zip|FoneCall [binaries](https://www.google.com/search?q=what+are+application+binaries) containing the FoneCall_Portable.exe and required supporting files.  ~Unzip retaining folder structure.|
| FoneCall_Portable+NDP481-Web.zip| FoneCall binaries zip file and NDP481-Web[^2] executable|

[^1]: Setup makes the Registry entries to add FoneCall to the Start Menu and  support application add/uninstall.  No other modifications to the Registry are made
[^2]: NDP481-Web is Microsoft's official .Net Framework 4.8 runtime installer 

### 🚀 Installation Guide
#### Why no "Setup.exe"
Browsers, email clients and antivirus scanners frequently block direct downloads of .exe installers or flag them with aggressive SmartScreen warnings. Packaging the installer inside a `.ZIP archive` ensures you can download it safely.

#### SmartScreen Considerations
FoneCall is open-source and distributed without expensive public certificates. Without intervention on your part Windows Defender SmartScreen (Windows SS) will display a warning stating "Windows protected your PC" upon launching the installer. The warning can be quite valid if the installer source is truly "unknown". However, if the source is known and trusted the warning is unnecessary, irritating and insulting. 

*   ***The Root Cause:*** Windows SS makes no distinction between truly well known sources—such as GitHub—and other sources. 

    When you use the default Windows File Explorer to extract a ZIP file, it automatically propagates an internet tracking tag—called *Mark of the Web* (MotW)—to every single file inside the zip file. This often causes Windows Defender or SmartScreen to block the installer from launching or corrupts the extraction process.

*   ***The Solution:*** The unnecessary warning can be avoided by using `7-Zip`,  `.WinRAR` or other archiving utility to extract the FoneCall installer from the zip file. (`7-Zip` is preferred by the author.)

    Using third-party tools like 7-Zip or WinRAR safely typically extracts the files without forcefully tagging the installer with the MotW, allowing the setup to run smoothly.

> [!IMPORTANT]
> **TLDR:** Use `7-Zip`,  `.WinRAR` or other archiving utility to extract the FoneCall installer. Avoid Windows built-in unzip.

#### If you still se the Windows SS

|What You Will See|What To Do|
|----|----|
| ![Screen shots of Windows Defender SmartScreen.](/assets/images/WinSS.png)|1. On the blue SmartScreen window, click the small **"More info"** hyperlink.|


2. Click the **"Run anyway"** button that appears at the bottom.
*Note: Because our installer is configured for "User-Mode", it will install cleanly into your user directory without requiring an administrative password.*|

