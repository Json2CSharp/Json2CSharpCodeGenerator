using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Win32;

namespace Xamasoft.JsonClassGenerator.WinForms
{
    public partial class MainForm : Form
    {
        private bool preventReentrancy = false;

        public MainForm()
        {
            // `IconTitleFont` is what WinForms *should* be using by default.
            // Need to set `this.Font` first, before `this.InitializeComponent();` to ensure font inheritance by controls in the form.
            this.Font = SystemFonts.IconTitleFont;

            this.InitializeComponent();

            this.ResetFonts();

            // Also: https://docs.microsoft.com/en-us/dotnet/desktop/winforms/how-to-respond-to-font-scheme-changes-in-a-windows-forms-application?view=netframeworkdesktop-4.8
            SystemEvents.UserPreferenceChanged += this.SystemEvents_UserPreferenceChanged;

            //

            this.optAttribJP    .CheckedChanged += this.OnAttributesModeCheckedChanged;
            this.optAttribJpn   .CheckedChanged += this.OnAttributesModeCheckedChanged;
            this.optAttribNone  .CheckedChanged += this.OnAttributesModeCheckedChanged;

            this.optMemberFields.CheckedChanged += this.OnMemberModeCheckedChanged;
            this.optMemberProps .CheckedChanged += this.OnMemberModeCheckedChanged;

            this.copyOutput.Click += this.CopyOutput_Click;
            this.copyOutput.Enabled = false;

            this.jsonInputTextbox.TextChanged += this.JsonInputTextbox_TextChanged;

            // Invoke event-handlers to set initial toolstrip text:
            this.optsAttributeMode.Tag = this.optsAttributeMode.Text + ": {0}";
            this.optMembersMode   .Tag = this.optMembersMode   .Text + ": {0}";

            this.OnAttributesModeCheckedChanged( this.optAttribJP   , EventArgs.Empty );
            this.OnMemberModeCheckedChanged    ( this.optMemberProps, EventArgs.Empty );
        }

        #region WinForms Taxes

        private static Font GetMonospaceFont( Single emFontSizePoints )
        {
            // See if Consolas or Lucida Sans Typewriter is available before falling-back:
            String[] preferredFonts = new[] { "Consolas", "Lucida Sans Typewriter" };
            foreach( String fontName in preferredFonts )
            {
                if( TestFont( fontName, emFontSizePoints ) )
                {
                    return new Font( fontName, emFontSizePoints, FontStyle.Regular );
                }
            }

            // Fallback:
            return new Font( FontFamily.GenericMonospace, emSize: emFontSizePoints );
        }

        private static Boolean TestFont( String fontName, Single emFontSizePoints )
        {
            try
            {
                using( Font test = new Font( fontName, emFontSizePoints, FontStyle.Regular ) )
                {
                    return test.Name == fontName;
                }
            }
            catch
            {
                return false;
            }
        }

        private void SystemEvents_UserPreferenceChanged(Object sender, UserPreferenceChangedEventArgs e)
        {
            switch( e.Category )
            {
            case UserPreferenceCategory.Accessibility:
            case UserPreferenceCategory.Window:
            case UserPreferenceCategory.VisualStyle:
            case UserPreferenceCategory.Menu:
                this.ResetFonts();
                break;
            }
        }

        private void ResetFonts()
        {
            this.Font = SystemFonts.IconTitleFont;

            Font monospaceFont = GetMonospaceFont( emFontSizePoints: SystemFonts.IconTitleFont.SizeInPoints );
            this.jsonInputTextbox   .Font = monospaceFont;
            this.csharpOutputTextbox.Font = monospaceFont;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            
            if( !e.Cancel )
            {
                SystemEvents.UserPreferenceChanged -= new UserPreferenceChangedEventHandler(SystemEvents_UserPreferenceChanged);
            }
        }

        #endregion

        #region Highlander: There can only be one!

        private void OnAttributesModeCheckedChanged(Object sender, EventArgs e)
        {
            this.WhoaohohohhWhoaohohohhWhoaohohohhWhoaohohohhWhoaohohohhWhoaohohohhWhoaohohohhWhoaohohohhWhoaohohohh( (ToolStripMenuItem)sender, defaultItem: this.optAttribJP, parent: this.optsAttributeMode );

            this.GenerateCSharp();
        }

        private void OnMemberModeCheckedChanged(Object sender, EventArgs e)
        {
            this.WhoaohohohhWhoaohohohhWhoaohohohhWhoaohohohhWhoaohohohhWhoaohohohhWhoaohohohhWhoaohohohhWhoaohohohh( (ToolStripMenuItem)sender, defaultItem: this.optMemberProps, parent: this.optMembersMode );

            this.GenerateCSharp();
        }

        /// <summary>https://www.youtube.com/watch?v=Qy1J_i32wTg</summary>
        private void WhoaohohohhWhoaohohohhWhoaohohohhWhoaohohohhWhoaohohohhWhoaohohohhWhoaohohohhWhoaohohohhWhoaohohohh(ToolStripMenuItem subject, ToolStripMenuItem defaultItem, ToolStripDropDownButton parent)
        {
            if( this.preventReentrancy ) return;
            try
            {
                this.preventReentrancy = true;

                ToolStripMenuItem singleCheckedItem;
                if( subject.Checked )
                {
                    singleCheckedItem = subject;
                    this.UncheckOthers( subject, parent );
                }
                else
                {
                    this.EnsureAtLeast1IsCheckedAfterItemWasUnchecked( subject, defaultItem, parent );
                    singleCheckedItem = parent.DropDownItems.Cast<ToolStripMenuItem>().Single( item => item.Checked );
                }

                String parentTextFormat = (String)parent.Tag;
                parent.Text = String.Format( format: parentTextFormat, arg0: singleCheckedItem.Text );
            }
            finally
            {
                this.preventReentrancy = false;
            }
        }

        private void UncheckOthers( ToolStripMenuItem sender, ToolStripDropDownButton parent )
        {
            foreach( ToolStripMenuItem menuItem in parent.DropDownItems.Cast<ToolStripMenuItem>() ) // I really hate old-style IEnumerable, *grumble*
            {
                if( !Object.ReferenceEquals( menuItem, sender ) )
                {
                    menuItem.Checked = false;
                }
            }
        }

        private void EnsureAtLeast1IsCheckedAfterItemWasUnchecked( ToolStripMenuItem subject, ToolStripMenuItem defaultItem, ToolStripDropDownButton parent )
        {
            int countChecked = parent.DropDownItems.Cast<ToolStripMenuItem>().Count( item => item.Checked );

            if( countChecked == 1 )
            {
                // Is exactly 1 checked already? If so, then NOOP.
            }
            else if( countChecked > 1 )
            {
                // If more than 1 are checked, then check only the default:
                defaultItem.Checked = true;
                this.UncheckOthers( sender: defaultItem, parent );
            }
            else
            {
                // If none are checked, then *if* the unchecked item is NOT the default item, then check the default item:
                if( !Object.ReferenceEquals( subject, defaultItem ) )
                {
                    defaultItem.Checked = true;
                }
                else
                {
                    // Otherwise, check the first non-default item:
                    ToolStripMenuItem nextBestItem = parent.DropDownItems.Cast<ToolStripMenuItem>().First( item => item != defaultItem );
                    nextBestItem.Checked = true;
                }
            }
        }

        #endregion

        //

        private void ConfigureGenerator( IJsonClassGeneratorConfig config )
        {
            config.UsePascalCase = this.optsPascalCase.Checked;
            
            //

            if( this.optAttribJP.Checked )
            {
                config.UseJsonAttributes   = true;
                config.UseJsonPropertyName = false;
            }
            else if( this.optAttribJpn.Checked )
            {
                config.UseJsonAttributes   = false;
                config.UseJsonPropertyName = true;
            }
            else// implicit: ( this.optAttribNone.Checked )
            {
                config.UseJsonAttributes   = false;
                config.UseJsonPropertyName = false;
            }

            //

            if( this.optMemberProps.Checked )
            {
                config.UseProperties = true;
                config.UseFields     = false;
            }
            else// implicit: ( this.optMemberFields.Checked )
            {
                config.UseProperties = false;
                config.UseFields     = true;
            }

            config.ImmutableClasses = this.optImmutable.Checked;
        }

        private void JsonInputTextbox_TextChanged(Object sender, EventArgs e)
        {
            this.GenerateCSharp();
        }

        private void GenerateCSharp()
        {
            this.copyOutput.Enabled = false;

            String jsonText = this.jsonInputTextbox.Text;
            if( String.IsNullOrWhiteSpace( jsonText ) )
            {
                this.csharpOutputTextbox.Text = String.Empty;
                return;
            }

            JsonClassGenerator generator = new JsonClassGenerator();
            this.ConfigureGenerator( generator );

            try
            {
                StringBuilder sb = generator.GenerateClasses( jsonText, errorMessage: out String errorMessage );
                if( !String.IsNullOrWhiteSpace( errorMessage ) )
                {
                    this.csharpOutputTextbox.Text = "Error:\r\n" + errorMessage;
                }
                else
                {
                    this.csharpOutputTextbox.Text = sb.ToString();
                     this.copyOutput.Enabled = true;
                }
            }
            catch( Exception ex )
            {
                this.csharpOutputTextbox.Text = "Error:\r\n" + ex.ToString();
            }
        }

        private void CopyOutput_Click(Object sender, EventArgs e)
        {
            if( this.csharpOutputTextbox.Text?.Length > 0 )
            {
                Clipboard.SetText( this.csharpOutputTextbox.Text, TextDataFormat.UnicodeText );
            }
        }
    }
}
