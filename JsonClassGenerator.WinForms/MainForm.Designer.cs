
namespace Xamasoft.JsonClassGenerator.WinForms
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if( disposing && ( components != null ) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.split = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.jsonInputTextbox = new System.Windows.Forms.TextBox();
            this.inputLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.csharpOutputTextbox = new System.Windows.Forms.TextBox();
            this.outputLabel = new System.Windows.Forms.Label();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.openButton = new System.Windows.Forms.ToolStripButton();
            this.optsPascalCase = new System.Windows.Forms.ToolStripButton();
            this.sep1 = new System.Windows.Forms.ToolStripSeparator();
            this.optsAttributeMode = new System.Windows.Forms.ToolStripDropDownButton();
            this.optAttribJP = new System.Windows.Forms.ToolStripMenuItem();
            this.optAttribJpn = new System.Windows.Forms.ToolStripMenuItem();
            this.sep2 = new System.Windows.Forms.ToolStripSeparator();
            this.optMembersMode = new System.Windows.Forms.ToolStripDropDownButton();
            this.optMemberProps = new System.Windows.Forms.ToolStripMenuItem();
            this.optMemberFields = new System.Windows.Forms.ToolStripMenuItem();
            this.sep3 = new System.Windows.Forms.ToolStripSeparator();
            this.copyOutput = new System.Windows.Forms.ToolStripButton();
            this.wrapText = new System.Windows.Forms.ToolStripButton();
            this.optTypesMode = new System.Windows.Forms.ToolStripDropDownButton();
            this.optTypesMutablePoco = new System.Windows.Forms.ToolStripMenuItem();
            this.optTypesImmutablePoco = new System.Windows.Forms.ToolStripMenuItem();
            this.optTypesRecords = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.ofd = new System.Windows.Forms.OpenFileDialog();
            this.optsUseNullable = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            ((System.ComponentModel.ISupportInitialize)(this.split)).BeginInit();
            this.split.Panel1.SuspendLayout();
            this.split.Panel2.SuspendLayout();
            this.split.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // split
            // 
            this.split.Dock = System.Windows.Forms.DockStyle.Fill;
            this.split.Location = new System.Drawing.Point(0, 25);
            this.split.Name = "split";
            // 
            // split.Panel1
            // 
            this.split.Panel1.Controls.Add(this.tableLayoutPanel1);
            // 
            // split.Panel2
            // 
            this.split.Panel2.Controls.Add(this.tableLayoutPanel2);
            this.split.Size = new System.Drawing.Size(1288, 423);
            this.split.SplitterDistance = 644;
            this.split.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AllowDrop = true;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.jsonInputTextbox, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.inputLabel, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(644, 423);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // jsonInputTextbox
            // 
            this.jsonInputTextbox.AcceptsReturn = true;
            this.jsonInputTextbox.AcceptsTab = true;
            this.jsonInputTextbox.AllowDrop = true;
            this.jsonInputTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.jsonInputTextbox.Location = new System.Drawing.Point(3, 32);
            this.jsonInputTextbox.Multiline = true;
            this.jsonInputTextbox.Name = "jsonInputTextbox";
            this.jsonInputTextbox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.jsonInputTextbox.Size = new System.Drawing.Size(638, 388);
            this.jsonInputTextbox.TabIndex = 0;
            this.jsonInputTextbox.WordWrap = false;
            // 
            // inputLabel
            // 
            this.inputLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.inputLabel.Location = new System.Drawing.Point(1, 4);
            this.inputLabel.Margin = new System.Windows.Forms.Padding(1, 4, 4, 2);
            this.inputLabel.Name = "inputLabel";
            this.inputLabel.Size = new System.Drawing.Size(639, 23);
            this.inputLabel.TabIndex = 1;
            this.inputLabel.Text = "Paste JSON Input or drag and drop a *.json file:";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.csharpOutputTextbox, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.outputLabel, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(640, 423);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // csharpOutputTextbox
            // 
            this.csharpOutputTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.csharpOutputTextbox.Location = new System.Drawing.Point(3, 32);
            this.csharpOutputTextbox.Multiline = true;
            this.csharpOutputTextbox.Name = "csharpOutputTextbox";
            this.csharpOutputTextbox.ReadOnly = true;
            this.csharpOutputTextbox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.csharpOutputTextbox.Size = new System.Drawing.Size(634, 388);
            this.csharpOutputTextbox.TabIndex = 0;
            this.csharpOutputTextbox.WordWrap = false;
            // 
            // outputLabel
            // 
            this.outputLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputLabel.Location = new System.Drawing.Point(1, 4);
            this.outputLabel.Margin = new System.Windows.Forms.Padding(1, 4, 4, 2);
            this.outputLabel.Name = "outputLabel";
            this.outputLabel.Size = new System.Drawing.Size(635, 23);
            this.outputLabel.TabIndex = 1;
            this.outputLabel.Text = "Generated C# output:";
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openButton,
            this.optsPascalCase,
            this.sep1,
            this.optsUseNullable,
            this.toolStripSeparator1,
            this.optsAttributeMode,
            this.sep2,
            this.optMembersMode,
            this.sep3,
            this.copyOutput,
            this.wrapText,
            this.optTypesMode});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(1288, 25);
            this.toolStrip.TabIndex = 0;
            // 
            // openButton
            // 
            this.openButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(49, 22);
            this.openButton.Text = "Open...";
            // 
            // optsPascalCase
            // 
            this.optsPascalCase.Checked = true;
            this.optsPascalCase.CheckOnClick = true;
            this.optsPascalCase.CheckState = System.Windows.Forms.CheckState.Checked;
            this.optsPascalCase.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.optsPascalCase.Name = "optsPascalCase";
            this.optsPascalCase.Size = new System.Drawing.Size(94, 22);
            this.optsPascalCase.Text = "Use Pascal Case";
            // 
            // sep1
            // 
            this.sep1.Name = "sep1";
            this.sep1.Size = new System.Drawing.Size(6, 25);
            // 
            // optsAttributeMode
            // 
            this.optsAttributeMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.optsAttributeMode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optAttribJP,
            this.optAttribJpn});
            this.optsAttributeMode.Name = "optsAttributeMode";
            this.optsAttributeMode.Size = new System.Drawing.Size(106, 22);
            this.optsAttributeMode.Text = "Attributes mode";
            // 
            // optAttribJP
            // 
            this.optAttribJP.Checked = true;
            this.optAttribJP.CheckOnClick = true;
            this.optAttribJP.CheckState = System.Windows.Forms.CheckState.Checked;
            this.optAttribJP.Name = "optAttribJP";
            this.optAttribJP.Size = new System.Drawing.Size(204, 22);
            this.optAttribJP.Text = "Use [JsonProperty]";
            // 
            // optAttribJpn
            // 
            this.optAttribJpn.CheckOnClick = true;
            this.optAttribJpn.Name = "optAttribJpn";
            this.optAttribJpn.Size = new System.Drawing.Size(204, 22);
            this.optAttribJpn.Text = "Use [JsonPropertyName]";
            // 
            // sep2
            // 
            this.sep2.Name = "sep2";
            this.sep2.Size = new System.Drawing.Size(6, 25);
            // 
            // optMembersMode
            // 
            this.optMembersMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.optMembersMode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optMemberProps,
            this.optMemberFields});
            this.optMembersMode.Name = "optMembersMode";
            this.optMembersMode.Size = new System.Drawing.Size(99, 22);
            this.optMembersMode.Text = "Member mode";
            // 
            // optMemberProps
            // 
            this.optMemberProps.Checked = true;
            this.optMemberProps.CheckOnClick = true;
            this.optMemberProps.CheckState = System.Windows.Forms.CheckState.Checked;
            this.optMemberProps.Name = "optMemberProps";
            this.optMemberProps.Size = new System.Drawing.Size(180, 22);
            this.optMemberProps.Text = "Use properties";
            // 
            // optMemberFields
            // 
            this.optMemberFields.CheckOnClick = true;
            this.optMemberFields.Name = "optMemberFields";
            this.optMemberFields.Size = new System.Drawing.Size(180, 22);
            this.optMemberFields.Text = "Use fields";
            // 
            // sep3
            // 
            this.sep3.Name = "sep3";
            this.sep3.Size = new System.Drawing.Size(6, 25);
            // 
            // copyOutput
            // 
            this.copyOutput.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.copyOutput.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.copyOutput.Name = "copyOutput";
            this.copyOutput.Size = new System.Drawing.Size(167, 22);
            this.copyOutput.Text = "Copy C# Output to Clipboard";
            // 
            // wrapText
            // 
            this.wrapText.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.wrapText.CheckOnClick = true;
            this.wrapText.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.wrapText.Name = "wrapText";
            this.wrapText.Size = new System.Drawing.Size(62, 22);
            this.wrapText.Text = "Wrap text";
            // 
            // optTypesMode
            // 
            this.optTypesMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.optTypesMode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optTypesMutablePoco,
            this.optTypesImmutablePoco,
            this.optTypesRecords});
            this.optTypesMode.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.optTypesMode.Name = "optTypesMode";
            this.optTypesMode.Size = new System.Drawing.Size(89, 22);
            this.optTypesMode.Text = "Output types";
            // 
            // optTypesMutablePoco
            // 
            this.optTypesMutablePoco.Checked = true;
            this.optTypesMutablePoco.CheckOnClick = true;
            this.optTypesMutablePoco.CheckState = System.Windows.Forms.CheckState.Checked;
            this.optTypesMutablePoco.Name = "optTypesMutablePoco";
            this.optTypesMutablePoco.Size = new System.Drawing.Size(208, 22);
            this.optTypesMutablePoco.Text = "Mutable POCO classes";
            // 
            // optTypesImmutablePoco
            // 
            this.optTypesImmutablePoco.CheckOnClick = true;
            this.optTypesImmutablePoco.Name = "optTypesImmutablePoco";
            this.optTypesImmutablePoco.Size = new System.Drawing.Size(208, 22);
            this.optTypesImmutablePoco.Text = "Immutable POCO classes";
            // 
            // optTypesRecords
            // 
            this.optTypesRecords.CheckOnClick = true;
            this.optTypesRecords.Name = "optTypesRecords";
            this.optTypesRecords.Size = new System.Drawing.Size(208, 22);
            this.optTypesRecords.Text = "Immutable record classes";
            // 
            // statusStrip
            // 
            this.statusStrip.Location = new System.Drawing.Point(0, 448);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1288, 22);
            this.statusStrip.TabIndex = 1;
            // 
            // ofd
            // 
            this.ofd.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            this.ofd.SupportMultiDottedExtensions = true;
            // 
            // optsUseNullable
            // 
            this.optsUseNullable.CheckOnClick = true;
            this.optsUseNullable.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.optsUseNullable.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.optsUseNullable.Name = "optsUseNullable";
            this.optsUseNullable.Size = new System.Drawing.Size(77, 22);
            this.optsUseNullable.Text = "Use Nullable";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1288, 470);
            this.Controls.Add(this.split);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.toolStrip);
            this.DoubleBuffered = true;
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "JSON-to-C#";
            this.split.Panel1.ResumeLayout(false);
            this.split.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.split)).EndInit();
            this.split.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

        #endregion

        private System.Windows.Forms.SplitContainer split;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton optsPascalCase;
        private System.Windows.Forms.ToolStripMenuItem optAttribJpn;
        private System.Windows.Forms.ToolStripMenuItem optAttribNone;
        private System.Windows.Forms.ToolStripDropDownButton optMembersMode;
        private System.Windows.Forms.ToolStripMenuItem optMemberProps;
        private System.Windows.Forms.ToolStripMenuItem optMemberFields;
        private System.Windows.Forms.TextBox jsonInputTextbox;
        private System.Windows.Forms.TextBox csharpOutputTextbox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label inputLabel;
        private System.Windows.Forms.Label outputLabel;
        private System.Windows.Forms.ToolStripSeparator sep1;
        private System.Windows.Forms.ToolStripSeparator sep2;
        private System.Windows.Forms.ToolStripSeparator sep3;
        private System.Windows.Forms.ToolStripDropDownButton optsAttributeMode;
        private System.Windows.Forms.ToolStripMenuItem optAttribJP;
        private System.Windows.Forms.ToolStripButton copyOutput;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripButton openButton;
        private System.Windows.Forms.OpenFileDialog ofd;
        private System.Windows.Forms.ToolStripButton wrapText;
        private System.Windows.Forms.ToolStripDropDownButton optTypesMode;
        private System.Windows.Forms.ToolStripMenuItem optTypesMutablePoco;
        private System.Windows.Forms.ToolStripMenuItem optTypesImmutablePoco;
        private System.Windows.Forms.ToolStripMenuItem optTypesRecords;
        private System.Windows.Forms.ToolStripButton optsUseNullable;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}

