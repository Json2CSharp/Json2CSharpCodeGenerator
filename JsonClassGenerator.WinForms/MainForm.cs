using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using Microsoft.Win32;
using Xamasoft.JsonClassGenerator.CodeWriterConfiguration;
using Xamasoft.JsonClassGenerator.CodeWriters;
using Xamasoft.JsonClassGenerator.Models;

namespace Xamasoft.JsonClassGenerator.WinForms
{
    public partial class MainForm : Form
    {
        private bool preventReentrancy = false;

        public CSharpCodeWriterConfig writerConfig { get; set; } = new CSharpCodeWriterConfig();
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

            this.openButton.Click += this.OpenButton_Click;

            this.optAttribJP          .CheckedChanged += this.OnAttributesModeCheckedChanged;
            this.optAttribJpn         .CheckedChanged += this.OnAttributesModeCheckedChanged;
            this.optAttribNone        .CheckedChanged += this.OnAttributesModeCheckedChanged;

            this.optMemberFields      .CheckedChanged += this.OnMemberModeCheckedChanged;
            this.optMemberProps       .CheckedChanged += this.OnMemberModeCheckedChanged;

            this.optTypesMutablePoco  .CheckedChanged += this.OnOutputTypeModeCheckedChanged;
            this.optTypesImmutablePoco.CheckedChanged += this.OnOutputTypeModeCheckedChanged;
            this.optTypesRecords      .CheckedChanged += this.OnOutputTypeModeCheckedChanged;

            this.optsPascalCase       .CheckedChanged += this.OnOptionsChanged;

            this.wrapText.CheckedChanged += this.WrapText_CheckedChanged;

            this.copyOutput.Click += this.CopyOutput_Click;
            this.copyOutput.Enabled = false;

            this.jsonInputTextbox.TextChanged += this.JsonInputTextbox_TextChanged;
            this.jsonInputTextbox.DragDrop    += this.JsonInputTextbox_DragDrop;
            this.jsonInputTextbox.DragOver    += this.JsonInputTextbox_DragOver;
            //this.jsonInputTextbox.paste // annoyingly, it isn't (easily) feasible to hook/detect TextBox paste events, even globally... grrr.

            // Invoke event-handlers to set initial toolstrip text:
            this.optsAttributeMode.Tag = this.optsAttributeMode.Text + ": {0}";
            this.optMembersMode   .Tag = this.optMembersMode   .Text + ": {0}";
            this.optTypesMode     .Tag = this.optTypesMode     .Text + ": {0}";

            this.OnAttributesModeCheckedChanged( this.optAttribJP        , EventArgs.Empty );
            this.OnMemberModeCheckedChanged    ( this.optMemberProps     , EventArgs.Empty );
            this.OnOutputTypeModeCheckedChanged( this.optTypesMutablePoco, EventArgs.Empty );
        }

        private void WrapText_CheckedChanged(Object sender, EventArgs e)
        {
            ToolStripButton tsb = (ToolStripButton)sender;

            // For some reason, toggling WordWrap causes a text selection in `jsonInputTextbox`. So, doing this:
            try
            {
                this.jsonInputTextbox.HideSelection = true;
                // ayayayay: https://stackoverflow.com/questions/1140250/how-to-remove-the-focus-from-a-textbox-in-winforms
                this.ActiveControl = this.toolStrip;

#if WINFORMS_TEXTBOX_GET_SCROLL_POSITION_WORKS_ARGH // It's non-trivial: https://stackoverflow.com/questions/4494162/change-scrollbar-position-in-textbox
                //int idx1 = this.jsonInputTextbox.GetFirstCharIndexOfCurrentLine(); // but what is the "current line"?
                int firstLineCharIndex = -1;
                if( this.jsonInputTextbox.Height > 10 )
                {
                    // https://stackoverflow.com/questions/10175400/maintain-textbox-scroll-position-while-adding-line
                    this.jsonInputTextbox.GetCharIndexFromPosition( new Point( 3, 3 ) );
                }
#endif

                this.jsonInputTextbox   .WordWrap = tsb.Checked;
                this.csharpOutputTextbox.WordWrap = tsb.Checked;

#if WINFORMS_TEXTBOX_GET_SCROLL_POSITION_WORKS_ARGH
                if( firstLineCharIndex > 0 ) // Greater than zero, not -1, because `GetCharIndexFromPosition` returns a meaningless zero sometimes.
                {
                    this.jsonInputTextbox.SelectionStart = firstLineCharIndex;
                    this.jsonInputTextbox.ScrollToCaret();
                }
#endif
            }
            finally
            {
                this.jsonInputTextbox.HideSelection = false;
            }
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

        #region Methods to ensure only a single checkbox-style menu item is checked at-a-time, and that the ToolStripDropDownButton's text indicates the currently selected option:

        private void OnAttributesModeCheckedChanged(Object sender, EventArgs e)
        {
            this.EnsureSingleCheckedDropDownItemAndUpdateToolStripItemText( (ToolStripMenuItem)sender, defaultItem: this.optAttribJP, parent: this.optsAttributeMode );

            this.GenerateCSharp();
        }

        private void OnMemberModeCheckedChanged(Object sender, EventArgs e)
        {
            this.EnsureSingleCheckedDropDownItemAndUpdateToolStripItemText( (ToolStripMenuItem)sender, defaultItem: this.optMemberProps, parent: this.optMembersMode );

            this.GenerateCSharp();
        }

        private void OnOutputTypeModeCheckedChanged(Object sender, EventArgs e)
        {
            this.EnsureSingleCheckedDropDownItemAndUpdateToolStripItemText( (ToolStripMenuItem)sender, defaultItem: this.optTypesMutablePoco, parent: this.optTypesMode );

            this.GenerateCSharp();
        }

        private void EnsureSingleCheckedDropDownItemAndUpdateToolStripItemText(ToolStripMenuItem subject, ToolStripMenuItem defaultItem, ToolStripDropDownButton parent)
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

        private void OnOptionsChanged(Object sender, EventArgs e)
        {
            this.GenerateCSharp();
        }

        #region Drag and Drop

        private void JsonInputTextbox_DragOver(Object sender, DragEventArgs e)
        {
            bool acceptable =
                e.Data.GetDataPresent( DataFormats.FileDrop ) ||
//              e.Data.GetDataPresent( DataFormats.Text ) ||
//              e.Data.GetDataPresent( DataFormats.OemText ) ||
                e.Data.GetDataPresent( DataFormats.UnicodeText, autoConvert: true )// ||
//              e.Data.GetDataPresent( DataFormats.Html ) ||
//              e.Data.GetDataPresent( DataFormats.StringFormat ) ||
//              e.Data.GetDataPresent( DataFormats.Rtf )
            ;

            if( acceptable )
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void JsonInputTextbox_DragDrop(Object sender, DragEventArgs e)
        {
            if( e.Data.GetDataPresent( DataFormats.FileDrop ) )
            {
                String[] fileNames = (String[])e.Data.GetData( DataFormats.FileDrop );
                if( fileNames.Length >= 1 )
                {
                    // hmm, maybe allow multiple files by concatenating them all into a horrible JSON array? :D
                    this.TryLoadJsonFile( fileNames[0] );
                }
            }
            else if( e.Data.GetDataPresent( DataFormats.UnicodeText, autoConvert: true ) )
            {
                this.statusStrip.Text = "";

                String text = (String)e.Data.GetData( DataFormats.UnicodeText, autoConvert: true );
                if( text != null )
                {
                    this.jsonInputTextbox.Text = text; // This will invoke `GenerateCSharp()`.

                    this.statusStrip.Text = "Loaded JSON from drag and drop data.";
                }
            }
        }

        /// <summary>This regex won't match <c>\r\n</c>, only <c>\n</c>.</summary>
        private static readonly Regex _onlyUnixLineBreaks = new Regex( "(?<!\r)\n", RegexOptions.Compiled ); // Don't use `[^\r]?\n` because it *will* match `\r\n`, and don't use `[^\r]\n` because it won't match a leading `$\n` in a file.

        private static String RepairLineBreaks( String text )
        {
            if( _onlyUnixLineBreaks.IsMatch( text ) )
            {
                return _onlyUnixLineBreaks.Replace( text, replacement: "\r\n" );
            }

            return text;
        }

        #endregion

        #region Open JSON file

        private void OpenButton_Click(Object sender, EventArgs e)
        {
            if( this.ofd.ShowDialog( owner: this ) == DialogResult.OK )
            {
                this.TryLoadJsonFile( this.ofd.FileName );
            }
        }

        private void TryLoadJsonFile( String filePath )
        {
            if ( String.IsNullOrWhiteSpace( filePath ) )
            {
                this.csharpOutputTextbox.Text = "Error: an empty file path was specified.";
            }
//          else if ( filePath.IndexOfAny( Path.GetInvalidFileNameChars() ) > -1 )
//          {
//              const String fmt = "Invalid file path: \"{0}\"";
//              this.csharpOutputTextbox.Text = String.Format( CultureInfo.CurrentCulture, fmt, filePath );
//          }
            else
            {
                FileInfo jsonFileInfo;
                try
                {
                    jsonFileInfo = new FileInfo( filePath );
                }
                catch( Exception ex )
                {
                    const String fmt = "Invalid file path: \"{0}\"\r\n{1}";
                    this.csharpOutputTextbox.Text = String.Format( CultureInfo.CurrentCulture, fmt, filePath, ex.ToString() );
                    return;
                }

                this.TryLoadJsonFile( jsonFileInfo );
            }
        }

        private void TryLoadJsonFile( FileInfo jsonFile )
        {
            if( jsonFile is null ) return;

            this.statusStrip.Text = "";

            try
            {
                jsonFile.Refresh();
                if( jsonFile.Exists )
                {
                    String jsonText = File.ReadAllText( jsonFile.FullName );
                    this.jsonInputTextbox.Text = jsonText; // This will invoke `GenerateCSharp()`.

                    this.statusStrip.Text = "Loaded \"" + jsonFile.FullName + "\" successfully.";
                }
                else
                {
                    this.csharpOutputTextbox.Text = String.Format( CultureInfo.CurrentCulture, "Error: File \"{0}\" does not exist.", jsonFile.FullName );
                }
            }
            catch( Exception ex )
            {
                const String fmt = "Error loading file: \"{0}\"\r\n{1}";

                this.csharpOutputTextbox.Text = String.Format( CultureInfo.CurrentCulture, fmt, jsonFile.FullName, ex.ToString() );
            }
        }

        #endregion

        private void ConfigureGenerator( JsonClassGenerator config )
        {
            writerConfig.UsePascalCase = this.optsPascalCase.Checked;

            if( this.optAttribJP.Checked )
            {
                writerConfig.AttributeLibrary = JsonLibrary.NewtonsoftJson;
            }
            else// implicit: ( this.optAttribJpn.Checked )
            {
                writerConfig.AttributeLibrary = JsonLibrary.SystemTextJson;
            }

            //

            if( this.optMemberProps.Checked )
            {
                writerConfig.OutputMembers = OutputMembers.AsProperties;
            }
            else// implicit: ( this.optMemberFields.Checked )
            {
                writerConfig.OutputMembers = OutputMembers.AsPublicFields;
            }

            //

            if( this.optTypesImmutablePoco.Checked )
            {
                writerConfig.OutputType = OutputTypes.ImmutableClass;
            }
            else if( this.optTypesMutablePoco.Checked )
            {
                writerConfig.OutputType = OutputTypes.MutableClass;
            }
            else// implicit: ( this.optTypesRecords.Checked )
            {
                writerConfig.OutputType = OutputTypes.ImmutableRecord;
            }
        }

        private void JsonInputTextbox_TextChanged(Object sender, EventArgs e)
        {
            if( this.preventReentrancy ) return;
            this.preventReentrancy = true;
            try
            {
                this.jsonInputTextbox.Text = RepairLineBreaks( this.jsonInputTextbox.Text );

                this.GenerateCSharp();
            }
            finally
            {
                this.preventReentrancy = false;
            }
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
            generator.CodeWriter = new CSharpCodeWriter(writerConfig);
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
