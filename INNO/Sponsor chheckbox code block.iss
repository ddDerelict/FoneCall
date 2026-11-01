; =====================================================================
; GLOBAL APPLICATION CONFIGURATION VARIABLES
; =====================================================================
#define AppName "FoneCall"
#define AppVersion "1.0"
#define AppExeName "FoneCall.exe"

; Change these two lines to instantly update all web URLs in your installer
#define GitHubOrg "fonecall-dev"
#define GitHubRepo "FoneCall"

[Setup]
AppName={#AppName}
AppVersion={#AppVersion}
DefaultDirName={autopf}\{#AppName}
OutputBaseFilename={#AppName}-Installer

[Run]
Filename: "{app}\{#AppExeName}"; Description: "Launch application"; Flags: postinstall nowait skipifsilent

[Code]
var
  SuccessState: Boolean;
  SponsorCheckBox: TNewCheckBox;

function InitializeSetup(): Boolean;
begin
  // Built-in environment check; if required .NET is not found, the wizard aborts early
  Result := IsDotNetInstalled(net48, 0); 
  
  if not Result then
  begin
    // Displays critical alert before exiting the setup sequence
    MsgBox('Your system does not appear to have the requisite .NET Framework 4.8 installed. ' +
           'Please install .NET Framework 4.8 before retrying the FoneCall installation. Setup will exit.',  
           mbCriticalError, MB_OK);
  end;
end;

procedure InitializeWizard();
begin
  { Create the checkbox on the Finished Page }
  SponsorCheckBox := TNewCheckBox.Create(WizardForm);
  SponsorCheckBox.Parent := WizardForm.FinishedPage;
  
  { Align it neatly underneath the default Run/Launch checkboxes }
  SponsorCheckBox.Left := WizardForm.RunList.Left;
  SponsorCheckBox.Width := WizardForm.RunList.Width;
  SponsorCheckBox.Height := ScaleY(20);
  SponsorCheckBox.Top := WizardForm.RunList.Top + WizardForm.RunList.Height + ScaleY(8);
  
  SponsorCheckBox.Caption := 'Support development (Open GitHub Sponsors page)';
  SponsorCheckBox.Checked := False;
end;

procedure CurStepChanged(CurStep: TSetupStep);
var
  ErrorCode: Integer;
begin
  { Trigger 1: Wizard is displaying the final page right before closing }
  if CurStep = ssPostInstall then
  begin
    if (SponsorCheckBox <> nil) and SponsorCheckBox.Checked then
    begin
      ShellExec('open', 'https://github.com{#GitHubOrg}', '', '', SW_SHOWNORMAL, ewNoWait, ErrorCode);
    end;
  end;

  { Trigger 2: Installation fully completed through all steps successfully }
  if CurStep = ssDone then
  begin
    SuccessState := True;
  end;
end;

procedure DeinitializeSetup();
var
  ErrorCode: Integer;
begin
  // If SuccessState is false, the user either cancelled or a fatal extraction error occurred
  if not SuccessState then
  begin
    // Double-check that we didn't fail simply because .NET was missing to avoid redundant error prompts
    if IsDotNetInstalled(net48, 0) then
    begin
      if MsgBox('Installation failed to complete. Would you like to open our GitHub Issues page to check for a solution or report this problem?', 
                mbConfirmation, MB_YESNO) = IDYES then
      begin
        ShellExecAsOriginalUser('open', 'https://github.com{#GitHubOrg}/{#GitHubRepo}/issues', '', '', SW_SHOWNORMAL, ewNoWait, ErrorCode);
      end;
    end;
  end;
end;
