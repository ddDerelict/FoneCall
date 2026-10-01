## FoneCall
FoneCall is a very simple scrcpy UI. It makes it easier to access scrcpy with your preferred Arguments via the Start Menu or Taskbar while avoiding display of the command window.  It is standard Windows desktop application alternative to the ".vbs" script file distributed with scrcpy.

The tool addresses some current constraints: 
1. Pinning scrcpy.exe itself to the Start Menu or Taskbar creates a Windows command window and doesn't accommodate Arguments
2. Pinning the ".vbs" file to the Start Menu or Taskbar is prohibited by Windows
3. Pinning a shortcut to the .vbs file is increasingly problematic

FoneCall only has 4 single purpose windows:
1. Intro (shown once after installation): describes tool usage.
2. Data Entry: where you specify execution parameters which are saved in an embedded SQLite DB. scrcpy.exe location is verified and Arguments  validated prior to saving.
3. Execution: starts scrcpy and provides and logs execution status. This is the application’s initial window after execution parameters have been saved
4. Arguments Reference and Update: displays scrcpy Help and provides the ability to add new scrcpy arguments. 

### What’s in the Box?
![Data Entry Window](/assets/images/DataEntry.png)
![scrcpy Execution Window](/assets/images/execution.png)

FoneCall requires .NET Framework 4.8 or higher which is included in the following:  
> + Windows 10: Version 1903 (May 2019 Update) and all later Windows 10 versions.
> + Windows 11: Included in the initial release (Version 22000) and later.
> + Windows Server: Windows Server 2019 (starting with version 1903 updates), Windows Server 2022, and Windows Server 2025.
*(Source: Google search)*

If your OS doesn’t already include support for DNFV 4.8 the installer is available [on this site](https://github.com/ddDerelict/FoneCall/releases) or if you prefer you can [download  from Microsoft]( https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48)

### Get FoneCall
[Download FoneCall](https://github.com/ddDerelict/FoneCall/releases)

FoneCall is tested for use exclusively with the official version of scrcpy.
(<https://github.com/Genymobile/scrcpy>). 
