using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FoneCall
{
    internal class InitializeApp : ApplicationContext
    {
        private static MsgBoxImplement _cmb { get; set; } = new MsgBoxImplement();
        internal static StringBuilder _logMsgs { get; set; }

        internal InitializeApp()
        {
            Xfer.OpenWins = new List<ICaller>();
            RunContext();
        }
        private static async void RunContext()
        {
            /// the DB is loaded during development and DB without user data included in the
            /// installer package || app split into three forms due problem with panel
            /// visibility flashing during load
            /// n.b..I believe problem could be solved with SuspendLayout() see RunOptMngr

            _logMsgs = new StringBuilder();
            #region logging header
            string filler5 = new string( '=', 5 ),
                   ver = DBC.GetVer();
            var netVer = RuntimeInformation.FrameworkDescription;
            LogR.Norm( $"\n\n{filler5} Starting FoneCall {Xfer.RunDate}" +
                       $" || {netVer} {filler5}\n" +
                       $"EXE DIR {Paths.AppExeFQN}\n" +
                       $"DB File {Paths.AppDB}\n" +
                       $"Logging {Paths.AppLog}\n" +
                       $"scrcpy {ver} || SQLite v{DBC.GetSQLiteVersion()}" );
            #endregion

            await RunChecks( startHeader: true );
            LogR.Norm( _logMsgs.ToString() );

            //DBC.UpdateKVP( KVP_Keys.FirstRun, true ); // for testing
            //DBC.ClearExeSpec(); // for testing

            #region logging start state
            string startState = DBC.KVP_GetValu<bool>( KVP_Keys.FirstRun )
                ? "initial run post-installation; showing usage instructions"
                : DBC.GetExeSpec() == null
                  ? "scrcpy.exe not in DB; showing data entry"
                  : "scrcpy.exe in DB; showing scrcpy execution";
            LogR.Norm( $"start state: {startState}" );
            #endregion

            // used in dev to populate preloaded content in DB; left in or posterity 
            //SaveRTFtoDB();
            //SaveOptionsToDB();

            _cmb.I_form = null;
            if (DBC.KVP_GetValu<bool>( KVP_Keys.FirstRun )) WinCtrl.Call_Welcome( _cmb );
            else if (DBC.GetExeSpec() == null) WinCtrl.Call_DataEntry( _cmb );
            else WinCtrl.Call_RunSCRCPY( _cmb, noDelay: false );
        }
        public static void SaveOptionsToDB()
        {
            int hderLine = 0;
            string fqn = @"D:\Computer\scrcpy-win64-v4.1\scrcpy 4.1 help.txt",
                   role = "", optTxt = "", explain = "";
            Args args = new Args();
            StringBuilder sb = new StringBuilder();
            string[] opt = File.ReadAllLines( fqn ).Where( arg => !string.IsNullOrWhiteSpace( arg ) ).ToArray();

            for (int i = 0; i < opt.Length; i++)
            {
                string currValu = opt[i];
                if (opt[i].StartsWith( "Options" ))
                {
                    role = Xfer.R_arg;
                    optTxt = "ARGUMENTS";
                    explain = $"Options impacting scrcpy execution during initialization and after phone is mirrored on the computer screen.\n" +
                        $"data source: Help for scrcpy 4.1 on {DateTime.Now: yyyy MMM dd}";
                    DBC.SaveArgs( role, optTxt, explain );
                }
                else if (opt[i].StartsWith( "    -" ))
                {
                    optTxt = opt[i].Trim();
                    hderLine = i;
                    sb.Clear();
                }
                else
                {
                    if (i + 1 < opt.Length - 1 && i == hderLine + 1) sb.Append( opt[i].Trim() );
                    else sb.Append( $"\n{opt[i].Trim()}" );
                    if (i + 1 < opt.Length - 1 && opt[i + 1].StartsWith( "    -" ))
                    {
                        optTxt = optTxt.Replace( "'", @"''" );
                        string exp = sb.ToString();
                        exp = exp.Replace( "'", @"''" );
                        DBC.SaveArgs( role, optTxt, exp );
                    }
                }
            }
        }
        public static void SaveRTFtoDB()
        {
            #region rtf code behind acquisition
            // 2 RTF editors used Microsoft Word and Jarte Plus
            // Word for doc dev and Jarte for "code behind"
            // Word''s  "code behind" was too complex to manipulate
            // programmatically and Jarte too limited for creation
            // process was [1] create .docx in Word [2] Save As .rtf  [3] close doc
            // [4] open in Jarte [5] Save As working name referenced in ReadAllText
            #endregion
            string rtfCode = File.ReadAllText( @"E:\__DOCS\FoneCallWelcome.rtf" );
            // removes non-printable NUL appended to Jarte RTF files
            rtfCode = Regex.Replace( rtfCode, @"\p{C}+", string.Empty );
            rtfCode = rtfCode.Replace( "'", "''" ); // SQLite single quote escape
            DBC.DeleteKVPrecord( KVP_Keys.Intro );
            DBC.SaveKVP( KVP_Keys.Intro, rtfCode );
            LogR.Norm( "RTF saved to DB" );
        }
        private static async Task RunChecks( bool startHeader )
        {
            string msg = "";

            try
            {
                #region Perform Startup checks
                if (Directory.Exists( Paths.AppData ))
                {
                    bool dbOK = await DBC.CheckDB();
                    _logMsgs.AppendLine( $"DB integrity OK = {dbOK}" );
                    if (!dbOK) callMsgBx( noLog: false, noDataDir: false, notCatch: true, ex: null );
                }
                else callMsgBx( noLog: false, noDataDir: true, notCatch: true, ex: null );
                #endregion
            }
            catch (Exception ex)
            {
                callMsgBx( noLog: false, noDataDir: false, notCatch: false, ex );
            }

            await LogSizeCheck();

            void callMsgBx( bool noLog, bool noDataDir, bool notCatch, Exception ex )
            {
                #region MSGBOX PARAMETERS
                _cmb.I_MsgType = States.Fail;
                _cmb.I_Msg = msg;
                _cmb.I_Txt_Btn1 = noLog ? "New Log" : notCatch ? "Exit" : "OK";
                #endregion
                string mbReturn = WinCtrl.CstmmMB_Call( _cmb );
                LogR.Norm( $"Warning option selected: {mbReturn}" );
            }
        }
        private static async Task LogSizeCheck()
        {
            long logSize;
            bool trimmed = false;
            int appStarts = default;
            string sessHdr = "===== Starting FoneCall",
                   fqn = Paths.AppLog,
                   logName = Path.GetFileName( fqn );

            if (File.Exists( fqn )) await manageChk();

            async Task manageChk()
            {
                bool retry = true;
                int retries = 0, maxRetries = 10;
                while (retry && retries < maxRetries)
                {
                    try
                    {
                        // Source - https://stackoverflow.com/a/876513
                        // ensure file is available
                        using (FileStream fs = File.Open( fqn, FileMode.Open, FileAccess.Read, FileShare.None ))
                        { fs.Close(); }

                        await Task.Run( () =>
                        {
                            performChk();
                        } );
                        retry = false;
                    }
                    catch (Exception)
                    {
                        retries++;
                        Thread.Sleep( 150 );
                    }
                }
            }
            void performChk()
            {
                appStarts = 0;
                FileInfo fileInfo = new FileInfo( fqn );
                logSize = fileInfo.Length;
                string[] logContent = File.ReadAllLines( fqn );

                for (int i = 0; i < logContent.Length; i++)
                    if (logContent[i].Contains( sessHdr )) appStarts++;

                fileInfo.Refresh();

                if (logSize > 1000000) //0.001 GB/1 MB; approx 28000 lines
                {
                    string tmpLog = fqn.Replace( ".txt", "_tmp.txt" );

                    File.WriteAllText( tmpLog, "!!!!! LOG HAS BEEN TRIMMED !!!!!" );

                    string[] logB4 = File.ReadAllLines( fqn );

                    // remove oldest session
                    int headerLine = 0;
                    List<string> nuLogLines = new List<string>();
                    for (int i = 0; i < logB4.Length; i++)
                    {
                        if (logB4[i].Contains( sessHdr )) headerLine++;
                        if (headerLine >= 20) nuLogLines.Add( logB4[i] );
                    }
                    try
                    {
                        File.AppendAllLines( tmpLog, nuLogLines );
                        File.Copy( tmpLog, fqn, true );
                        File.Delete( tmpLog );
                        trimmed = true;
                    }
                    catch (Exception ex)
                    {
                        _logMsgs.AppendLine( $">>>error trimming log file {logName} = {ex}" );
                    }
                }

                if (trimmed)
                {
                    fileInfo.Refresh();
                    string[] logContentTrimed = File.ReadAllLines( fqn );

                    appStarts = default;
                    for (int i = 0; i < logContentTrimed.Length; i++) if (logContentTrimed[i].Contains( "###" )) appStarts++;
                    appStarts /= 3;
                    logSize = fileInfo.Length;
                    _logMsgs.AppendLine( $"\tLog {logName} trimmed this cycle" );
                    _logMsgs.AppendLine( $"\ttrimmed log in bytes: {logSize} || lines: {logContentTrimed.Length} || starts: {appStarts}\n" );
                }
                else _logMsgs.Append( $"log {logName} in bytes: {logSize}" +
                                          $" || lines: {logContent.Length} || sessions: {appStarts}" );
            }
        }
    }
}
