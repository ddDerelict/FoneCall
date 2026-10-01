using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FoneCall
{
    public partial class RunOptMngr : MsgBoxImplement
    {
        public RunOptMngr()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            FormBorderStyle = FormBorderStyle.None;
            I_form = this;
            WinCtrl.RegisterWin( this );

            SetEventHandlers();
            CstmBorders.Vari_W( LoadingLBL, 3, Color.WhiteSmoke );
            CstmStyles.BTN_Disabled( SaveBTN );
            CstmStyles.BTN_Enabled_Default( CloseBTN );
        }
        protected override void OnShown( EventArgs e )
        {
            base.OnShown( e );

            SuspendLayout();

            PopulateDGVs();
            AdjustForm();
            LoadingLBL.Visible = false;
            DGV_PNL.Visible = true;
            DGV.ClearSelection();
            CloseBTN.Text = "Close";
            CloseBTN.Select();

            ResumeLayout( false );
            PerformLayout();
        }
        private void SetEventHandlers()
        {
            DGV.Enter += DGV_Enter;
            DGV.UserAddedRow += DGV_UserAddedRow;
            DGV.UserDeletedRow += DGV_UserDeletedRow;
            DGV.EditingControlShowing += DGV_EditingControlShowing;
            DGV.DefaultValuesNeeded += DGV_DefaultValuesNeeded;
            SaveBTN.Click += SaveBTN_Click;
            CloseBTN.Click += CancelBTN_Click;
            ReleasesLINK.LinkClicked += Releases_LinkClicked;
        }

        private void AdjustForm()
        {
            int margin = DGV_PNL.Left, dgv2bot = (2 * margin) + SaveBTN.Height;

            SaveBTN.Location = new Point( DGV.Left, DGV.Bottom + margin );
            CloseBTN.Location = new Point( DGV.Right - CloseBTN.Width, SaveBTN.Top );
            DGV_PNL.Size = new Size( DGV.Right + 2, SaveBTN.Bottom + 2 );
            ClientSize = new Size( DGV_PNL.Width + (2 * margin), DGV_PNL.Bottom + margin );
            CstmBorders.ClearBorder( this );
            CstmBorders.Vari_W( this, 4, Color.WhiteSmoke );
            foreach (var lbl in Controls.OfType<Label>()) if (lbl.Name.Contains( "bord_" )) lbl.BringToFront();

            TextPNL.Location = new Point( (ClientSize.Width - TextPNL.Width) / 2, TextPNL.Top );
            DGV_PNL.Location = new Point( DGV_PNL.Left, TextPNL.Bottom + 10 );

            Screen scr = Screen.FromControl( this );
            Location = new Point( Left, (scr.WorkingArea.Height - ClientSize.Height) / 2 );
        }

        private void PopulateDGVs()
        {
            List<Args> args = DBC.GetArgs();
            int argsNum = args.Count;
            if (args != null && argsNum > 0)
            {
                for (int i = 0; i < args.Count; i++)
                {
                    // Ensure neither Arg nor Role is null to avoid CS8602/CS8604
                    string roleValue = args[i].Role ?? string.Empty;
                    string argValue = args[i].Arg ?? string.Empty;
                    string explainValue = args[i].Explain ?? string.Empty;
                    DGV.Rows.Add( roleValue, argValue, explainValue );
                }
            }
            else return;

            DGV.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            DGV.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            DGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            DGV.AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke;

            // auto sizing leaves unsightly gray areas; sizing hard coded since content is certain
            DGV.Columns[1].Width = 260;
            DGV.Columns[2].Width = 500;
            DGV.Width = 875;

            Screen scr = Screen.FromControl( this );
            int margin = DGV_PNL.Left,
                waw = scr.WorkingArea.Width,
                avail_2_DGV = waw - (2 * margin) - 20;
            if (avail_2_DGV < DGV.Width)
            {
                double adjuster = (double)avail_2_DGV / DGV.Width;
                int adjdCol_1_W = (int)(DGV.Columns[1].Width * adjuster),
                    adjdCol_2_W = (int)(DGV.Columns[2].Width * adjuster);
                DGV.Width -= DGV.Columns[1].Width - adjdCol_1_W + (DGV.Columns[2].Width - adjdCol_2_W);
                DGV.Columns[1].Width = adjdCol_1_W;
                DGV.Columns[2].Width = adjdCol_2_W;
            }

            int dgv2bot = (2 * margin) + SaveBTN.Height;
            double vu_H = scr.WorkingArea.Height * 0.7;
            DGV.Height = (int)vu_H - TextPNL.Bottom - 10 - dgv2bot;
        }
        private void DGV_EditingControlShowing( object sender, DataGridViewEditingControlShowingEventArgs e )
        {
            ToolStripMenuItem miCopy = new ToolStripMenuItem( "Copy Selection" );
            ToolStripMenuItem miPaste = new ToolStripMenuItem( "Paste" );
            ToolStripMenuItem[] menuItems = new ToolStripMenuItem[] { miCopy, miPaste };
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            contextMenu.Items.AddRange( menuItems );
            e.Control.ContextMenuStrip = contextMenu;

            miCopy.ShortcutKeys = Keys.Control | Keys.C;
            miPaste.ShortcutKeys = Keys.Control | Keys.V;

            miCopy.Click += ( s, ev ) => DGV_Edit( copy: true, paste: false );
            miPaste.Click += ( s, ev ) => DGV_Edit( copy: false, paste: true );
        }
        private void DGV_Edit( bool copy, bool paste )
        {
            if (copy)
            {
                StringBuilder sb = new StringBuilder();
                foreach (DataGridViewRow row in DGV.SelectedRows)
                {
                    string cellValue = row.Cells[0].Value as string;
                    if (!string.IsNullOrEmpty( cellValue )) sb.AppendLine( cellValue );
                }
                Clipboard.SetText( sb.ToString() );
            }
            else
            {
                string s = Clipboard.GetText();
                string[] lines = s.Split( new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries );
                if (lines.Length > 0)
                {
                    foreach (string line in lines) DGV.Rows.Add( line );
                    CstmStyles.BTN_Enabled_Default( SaveBTN );
                }

                CstmStyles.BTN_Enabled_Green( SaveBTN );
            }
        }
        private void DGV_Enter( object sender, EventArgs e )
        {
            CstmStyles.BTN_Enabled_Green( SaveBTN );
            CloseBTN.Text = "Cancel";
        }
        private void DGV_DefaultValuesNeeded( object sender, DataGridViewRowEventArgs e )
        {
            e.Row.Cells[0].Value = Xfer.R_arg;
        }
        private void DGV_UserAddedRow( object sender, DataGridViewRowEventArgs e )
        {
            CstmStyles.BTN_Enabled_Green( SaveBTN );
            CloseBTN.Text = "Cancel";
        }
        private void DGV_UserDeletedRow( object sender, DataGridViewRowEventArgs e )
        {
            CstmStyles.BTN_Enabled_Green( SaveBTN );
            CloseBTN.Text = "Cancel";
        }
        private void Releases_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            string url = "https://github.com/Genymobile/scrcpy/releases";
            Process.Start( new ProcessStartInfo( url ) { UseShellExecute = true } );
        }
        private void SaveBTN_Click( object sender, EventArgs e )
        {
            CloseBTN.Text = "Close";
            DBC.ClearArgs();
            foreach (DataGridViewRow row in DGV.Rows)
            {
                object cellObj_1 = row.Cells[1].Value;
                if (cellObj_1 == null) continue;

                object cellObj_0 = row.Cells[0].Value;
                object cellObj_2 = row.Cells[2].Value;
                string cellValu_0 = cellObj_0 as string ?? string.Empty,
                       cellValu_1 = cellObj_1 as string ?? string.Empty,
                       cellValu_2 = cellObj_2 as string ?? string.Empty;
                if (string.IsNullOrEmpty( cellValu_1 )) continue;
                else
                {
                    cellValu_1 = cellValu_1.Replace( "'", @"''" );
                    cellValu_2 = cellValu_2.Replace( "'", @"''" );
                    DBC.SaveArgs( role: cellValu_0, arg: cellValu_1, explain: cellValu_2 );
                }
            }
        }
        private void CancelBTN_Click( object sender, EventArgs e )
        {
            Close();
        }
        protected override void OnFormClosed( FormClosedEventArgs e )
        {
            base.OnFormClosed( e );
            WinCtrl.UnregisterWin( this );
        }
    }
}
