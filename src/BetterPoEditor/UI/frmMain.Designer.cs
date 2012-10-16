namespace BePoE.UI
{
	partial class frmMain
	{
		/// <summary>
		/// Variabile di progettazione necessaria.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Liberare le risorse in uso.
		/// </summary>
		/// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Codice generato da Progettazione Windows Form

		/// <summary>
		/// Metodo necessario per il supporto della finestra di progettazione. Non modificare
		/// il contenuto del metodo con l'editor di codice.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
			this.ofdOpen = new System.Windows.Forms.OpenFileDialog();
			this.tsTools = new System.Windows.Forms.ToolStrip();
			this.tsiOpen = new System.Windows.Forms.ToolStripDropDownButton();
			this.tsiSave = new System.Windows.Forms.ToolStripButton();
			this.tsiCompile = new System.Windows.Forms.ToolStripButton();
			this.tsiFileFormat = new System.Windows.Forms.ToolStripDropDownButton();
			this.mniFileFormat_LineEndings = new System.Windows.Forms.ToolStripMenuItem();
			this.mniFileFormat_LineEndings_Linux = new System.Windows.Forms.ToolStripMenuItem();
			this.mniFileFormat_LineEndings_Windows = new System.Windows.Forms.ToolStripMenuItem();
			this.mniFileFormat_LineEndings_Mac = new System.Windows.Forms.ToolStripMenuItem();
			this.mniFileFormat_Encoding = new System.Windows.Forms.ToolStripMenuItem();
			this.mniFileFormat_Encoding_UTF8_NoBOM = new System.Windows.Forms.ToolStripMenuItem();
			this.mniFileFormat_Encoding_UTF8_BOM = new System.Windows.Forms.ToolStripMenuItem();
			this.mniFileFormat_Encoding_UTF16_BE = new System.Windows.Forms.ToolStripMenuItem();
			this.mniFileFormat_Encoding_UTF16_LE = new System.Windows.Forms.ToolStripMenuItem();
			this.mniFileFormat_Encoding_UTF32_BE = new System.Windows.Forms.ToolStripMenuItem();
			this.mniFileFormat_Encoding_UTF32_LE = new System.Windows.Forms.ToolStripMenuItem();
			this.mniFileFormat_Encoding_ANSI = new System.Windows.Forms.ToolStripMenuItem();
			this.tsiOptions = new System.Windows.Forms.ToolStripButton();
			this.tsiInfo = new System.Windows.Forms.ToolStripButton();
			this.tsiSep0 = new System.Windows.Forms.ToolStripSeparator();
			this.tsiView_Untranslated = new System.Windows.Forms.ToolStripButton();
			this.tsiView_Translated = new System.Windows.Forms.ToolStripButton();
			this.tsiView_Removed = new System.Windows.Forms.ToolStripButton();
			this.tsiSep1 = new System.Windows.Forms.ToolStripSeparator();
			this.tsiView_Fuzzy = new System.Windows.Forms.ToolStripButton();
			this.tsiView_Clear = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.tsiSearch = new System.Windows.Forms.ToolStripButton();
			this.scItems = new System.Windows.Forms.SplitContainer();
			this.lbxEntries = new System.Windows.Forms.ListBox();
			this.scEntry = new System.Windows.Forms.SplitContainer();
			this.tbxSourceContext = new System.Windows.Forms.TextBox();
			this.scSource = new System.Windows.Forms.SplitContainer();
			this.tcSource = new System.Windows.Forms.TabControl();
			this.tpSource0 = new System.Windows.Forms.TabPage();
			this.tbxSource0 = new System.Windows.Forms.TextBox();
			this.tcSource2 = new System.Windows.Forms.TabControl();
			this.tpExtractedComment = new System.Windows.Forms.TabPage();
			this.tbxExtractedComment = new System.Windows.Forms.TextBox();
			this.tpReferences = new System.Windows.Forms.TabPage();
			this.flpReferences = new System.Windows.Forms.FlowLayoutPanel();
			this.lblSourceContext = new System.Windows.Forms.Label();
			this.lnkCheckTranslationSpelling = new System.Windows.Forms.LinkLabel();
			this.lnkTidyTranslation = new System.Windows.Forms.LinkLabel();
			this.chkFuzzyTranslation = new System.Windows.Forms.CheckBox();
			this.scTranslation = new System.Windows.Forms.SplitContainer();
			this.tcTranslated = new System.Windows.Forms.TabControl();
			this.tpTranslated0 = new System.Windows.Forms.TabPage();
			this.tbxTranslated0 = new System.Windows.Forms.TextBox();
			this.tbxTranslatorComment = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.lnkTranslatorComment = new System.Windows.Forms.LinkLabel();
			this.lnkGoogleTranslate = new System.Windows.Forms.LinkLabel();
			this.ssStatus = new System.Windows.Forms.StatusStrip();
			this.sslStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.sslProgr = new System.Windows.Forms.ToolStripStatusLabel();
			this.ssbProgr = new System.Windows.Forms.ToolStripProgressBar();
			this.fbdReference = new System.Windows.Forms.FolderBrowserDialog();
			this.tsTools.SuspendLayout();
			this.scItems.Panel1.SuspendLayout();
			this.scItems.Panel2.SuspendLayout();
			this.scItems.SuspendLayout();
			this.scEntry.Panel1.SuspendLayout();
			this.scEntry.Panel2.SuspendLayout();
			this.scEntry.SuspendLayout();
			this.scSource.Panel1.SuspendLayout();
			this.scSource.Panel2.SuspendLayout();
			this.scSource.SuspendLayout();
			this.tcSource.SuspendLayout();
			this.tpSource0.SuspendLayout();
			this.tcSource2.SuspendLayout();
			this.tpExtractedComment.SuspendLayout();
			this.tpReferences.SuspendLayout();
			this.scTranslation.Panel1.SuspendLayout();
			this.scTranslation.Panel2.SuspendLayout();
			this.scTranslation.SuspendLayout();
			this.tcTranslated.SuspendLayout();
			this.tpTranslated0.SuspendLayout();
			this.ssStatus.SuspendLayout();
			this.SuspendLayout();
			// 
			// ofdOpen
			// 
			this.ofdOpen.DefaultExt = "po";
			this.ofdOpen.Filter = "PO Files|*.po|All files|*.*";
			this.ofdOpen.Title = "Select the PO file to open";
			// 
			// tsTools
			// 
			this.tsTools.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsiOpen,
            this.tsiSave,
            this.tsiCompile,
            this.tsiFileFormat,
            this.tsiOptions,
            this.tsiInfo,
            this.tsiSep0,
            this.tsiView_Untranslated,
            this.tsiView_Translated,
            this.tsiView_Removed,
            this.tsiSep1,
            this.tsiView_Fuzzy,
            this.tsiView_Clear,
            this.toolStripSeparator1,
            this.tsiSearch});
			this.tsTools.Location = new System.Drawing.Point(0, 0);
			this.tsTools.Name = "tsTools";
			this.tsTools.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.tsTools.Size = new System.Drawing.Size(800, 52);
			this.tsTools.TabIndex = 0;
			// 
			// tsiOpen
			// 
			this.tsiOpen.AutoSize = false;
			this.tsiOpen.Image = ((System.Drawing.Image)(resources.GetObject("tsiOpen.Image")));
			this.tsiOpen.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.tsiOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsiOpen.Name = "tsiOpen";
			this.tsiOpen.Size = new System.Drawing.Size(71, 49);
			this.tsiOpen.Text = "Open";
			this.tsiOpen.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.tsiOpen.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tsiOpen_MouseDown);
			this.tsiOpen.Click += new System.EventHandler(this.tsiOpen_Click);
			// 
			// tsiSave
			// 
			this.tsiSave.AutoSize = false;
			this.tsiSave.Image = ((System.Drawing.Image)(resources.GetObject("tsiSave.Image")));
			this.tsiSave.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.tsiSave.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsiSave.Name = "tsiSave";
			this.tsiSave.Size = new System.Drawing.Size(71, 49);
			this.tsiSave.Text = "Save";
			this.tsiSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.tsiSave.Click += new System.EventHandler(this.tsiSave_Click);
			// 
			// tsiCompile
			// 
			this.tsiCompile.AutoSize = false;
			this.tsiCompile.Image = ((System.Drawing.Image)(resources.GetObject("tsiCompile.Image")));
			this.tsiCompile.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.tsiCompile.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsiCompile.Name = "tsiCompile";
			this.tsiCompile.Size = new System.Drawing.Size(71, 49);
			this.tsiCompile.Text = "Compile";
			this.tsiCompile.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.tsiCompile.Click += new System.EventHandler(this.tsiCompile_Click);
			// 
			// tsiFileFormat
			// 
			this.tsiFileFormat.AutoSize = false;
			this.tsiFileFormat.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniFileFormat_LineEndings,
            this.mniFileFormat_Encoding});
			this.tsiFileFormat.Image = ((System.Drawing.Image)(resources.GetObject("tsiFileFormat.Image")));
			this.tsiFileFormat.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.tsiFileFormat.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsiFileFormat.Name = "tsiFileFormat";
			this.tsiFileFormat.Size = new System.Drawing.Size(71, 49);
			this.tsiFileFormat.Text = "File format";
			this.tsiFileFormat.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			// 
			// mniFileFormat_LineEndings
			// 
			this.mniFileFormat_LineEndings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.mniFileFormat_LineEndings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniFileFormat_LineEndings_Linux,
            this.mniFileFormat_LineEndings_Windows,
            this.mniFileFormat_LineEndings_Mac});
			this.mniFileFormat_LineEndings.Name = "mniFileFormat_LineEndings";
			this.mniFileFormat_LineEndings.Size = new System.Drawing.Size(128, 22);
			this.mniFileFormat_LineEndings.Text = "Line ending";
			// 
			// mniFileFormat_LineEndings_Linux
			// 
			this.mniFileFormat_LineEndings_Linux.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.mniFileFormat_LineEndings_Linux.Name = "mniFileFormat_LineEndings_Linux";
			this.mniFileFormat_LineEndings_Linux.Size = new System.Drawing.Size(151, 22);
			this.mniFileFormat_LineEndings_Linux.Text = "Linux / new Mac";
			this.mniFileFormat_LineEndings_Linux.Click += new System.EventHandler(this.mniFileFormat_LineEndings_Linux_Click);
			// 
			// mniFileFormat_LineEndings_Windows
			// 
			this.mniFileFormat_LineEndings_Windows.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.mniFileFormat_LineEndings_Windows.Name = "mniFileFormat_LineEndings_Windows";
			this.mniFileFormat_LineEndings_Windows.Size = new System.Drawing.Size(151, 22);
			this.mniFileFormat_LineEndings_Windows.Text = "Windows";
			this.mniFileFormat_LineEndings_Windows.Click += new System.EventHandler(this.mniFileFormat_LineEndings_Windows_Click);
			// 
			// mniFileFormat_LineEndings_Mac
			// 
			this.mniFileFormat_LineEndings_Mac.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.mniFileFormat_LineEndings_Mac.Name = "mniFileFormat_LineEndings_Mac";
			this.mniFileFormat_LineEndings_Mac.Size = new System.Drawing.Size(151, 22);
			this.mniFileFormat_LineEndings_Mac.Text = "Old Mac";
			this.mniFileFormat_LineEndings_Mac.Click += new System.EventHandler(this.mniFileFormat_LineEndings_Mac_Click);
			// 
			// mniFileFormat_Encoding
			// 
			this.mniFileFormat_Encoding.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.mniFileFormat_Encoding.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniFileFormat_Encoding_UTF8_NoBOM,
            this.mniFileFormat_Encoding_UTF8_BOM,
            this.mniFileFormat_Encoding_UTF16_BE,
            this.mniFileFormat_Encoding_UTF16_LE,
            this.mniFileFormat_Encoding_UTF32_BE,
            this.mniFileFormat_Encoding_UTF32_LE,
            this.mniFileFormat_Encoding_ANSI});
			this.mniFileFormat_Encoding.Name = "mniFileFormat_Encoding";
			this.mniFileFormat_Encoding.Size = new System.Drawing.Size(128, 22);
			this.mniFileFormat_Encoding.Text = "Encoding";
			// 
			// mniFileFormat_Encoding_UTF8_NoBOM
			// 
			this.mniFileFormat_Encoding_UTF8_NoBOM.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.mniFileFormat_Encoding_UTF8_NoBOM.Name = "mniFileFormat_Encoding_UTF8_NoBOM";
			this.mniFileFormat_Encoding_UTF8_NoBOM.Size = new System.Drawing.Size(176, 22);
			this.mniFileFormat_Encoding_UTF8_NoBOM.Text = "UTF-8 (without BOM)";
			this.mniFileFormat_Encoding_UTF8_NoBOM.Click += new System.EventHandler(this.mniFileFormat_Encoding_UTF8_NoBOM_Click);
			// 
			// mniFileFormat_Encoding_UTF8_BOM
			// 
			this.mniFileFormat_Encoding_UTF8_BOM.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.mniFileFormat_Encoding_UTF8_BOM.Name = "mniFileFormat_Encoding_UTF8_BOM";
			this.mniFileFormat_Encoding_UTF8_BOM.Size = new System.Drawing.Size(176, 22);
			this.mniFileFormat_Encoding_UTF8_BOM.Text = "UTF-8 (with BOM)";
			this.mniFileFormat_Encoding_UTF8_BOM.Click += new System.EventHandler(this.mniFileFormat_Encoding_UTF8_BOM_Click);
			// 
			// mniFileFormat_Encoding_UTF16_BE
			// 
			this.mniFileFormat_Encoding_UTF16_BE.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.mniFileFormat_Encoding_UTF16_BE.Name = "mniFileFormat_Encoding_UTF16_BE";
			this.mniFileFormat_Encoding_UTF16_BE.Size = new System.Drawing.Size(176, 22);
			this.mniFileFormat_Encoding_UTF16_BE.Text = "UTF-16 (big-endian)";
			this.mniFileFormat_Encoding_UTF16_BE.Click += new System.EventHandler(this.mniFileFormat_Encoding_UTF16_BE_Click);
			// 
			// mniFileFormat_Encoding_UTF16_LE
			// 
			this.mniFileFormat_Encoding_UTF16_LE.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.mniFileFormat_Encoding_UTF16_LE.Name = "mniFileFormat_Encoding_UTF16_LE";
			this.mniFileFormat_Encoding_UTF16_LE.Size = new System.Drawing.Size(176, 22);
			this.mniFileFormat_Encoding_UTF16_LE.Text = "UTF-16 (little-endian)";
			this.mniFileFormat_Encoding_UTF16_LE.Click += new System.EventHandler(this.mniFileFormat_Encoding_UTF16_LE_Click);
			// 
			// mniFileFormat_Encoding_UTF32_BE
			// 
			this.mniFileFormat_Encoding_UTF32_BE.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.mniFileFormat_Encoding_UTF32_BE.Name = "mniFileFormat_Encoding_UTF32_BE";
			this.mniFileFormat_Encoding_UTF32_BE.Size = new System.Drawing.Size(176, 22);
			this.mniFileFormat_Encoding_UTF32_BE.Text = "UTF-32 (big-endian)";
			this.mniFileFormat_Encoding_UTF32_BE.Click += new System.EventHandler(this.mniFileFormat_Encoding_UTF32_BE_Click);
			// 
			// mniFileFormat_Encoding_UTF32_LE
			// 
			this.mniFileFormat_Encoding_UTF32_LE.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.mniFileFormat_Encoding_UTF32_LE.Name = "mniFileFormat_Encoding_UTF32_LE";
			this.mniFileFormat_Encoding_UTF32_LE.Size = new System.Drawing.Size(176, 22);
			this.mniFileFormat_Encoding_UTF32_LE.Text = "UTF-32 (little-endian)";
			this.mniFileFormat_Encoding_UTF32_LE.Click += new System.EventHandler(this.mniFileFormat_Encoding_UTF32_LE_Click);
			// 
			// mniFileFormat_Encoding_ANSI
			// 
			this.mniFileFormat_Encoding_ANSI.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.mniFileFormat_Encoding_ANSI.Name = "mniFileFormat_Encoding_ANSI";
			this.mniFileFormat_Encoding_ANSI.Size = new System.Drawing.Size(176, 22);
			this.mniFileFormat_Encoding_ANSI.Text = "ANSI";
			// 
			// tsiOptions
			// 
			this.tsiOptions.Image = ((System.Drawing.Image)(resources.GetObject("tsiOptions.Image")));
			this.tsiOptions.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.tsiOptions.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsiOptions.Name = "tsiOptions";
			this.tsiOptions.Size = new System.Drawing.Size(48, 49);
			this.tsiOptions.Text = "Options";
			this.tsiOptions.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.tsiOptions.Click += new System.EventHandler(this.tsiOptions_Click);
			// 
			// tsiInfo
			// 
			this.tsiInfo.Image = ((System.Drawing.Image)(resources.GetObject("tsiInfo.Image")));
			this.tsiInfo.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.tsiInfo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsiInfo.Name = "tsiInfo";
			this.tsiInfo.Size = new System.Drawing.Size(40, 49);
			this.tsiInfo.Text = "About";
			this.tsiInfo.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.tsiInfo.Click += new System.EventHandler(this.tsiInfo_Click);
			// 
			// tsiSep0
			// 
			this.tsiSep0.Name = "tsiSep0";
			this.tsiSep0.Size = new System.Drawing.Size(6, 52);
			// 
			// tsiView_Untranslated
			// 
			this.tsiView_Untranslated.AutoSize = false;
			this.tsiView_Untranslated.Checked = true;
			this.tsiView_Untranslated.CheckState = System.Windows.Forms.CheckState.Checked;
			this.tsiView_Untranslated.Image = ((System.Drawing.Image)(resources.GetObject("tsiView_Untranslated.Image")));
			this.tsiView_Untranslated.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.tsiView_Untranslated.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsiView_Untranslated.Name = "tsiView_Untranslated";
			this.tsiView_Untranslated.Size = new System.Drawing.Size(73, 49);
			this.tsiView_Untranslated.Text = "Untranslated";
			this.tsiView_Untranslated.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.tsiView_Untranslated.ToolTipText = "Show items to be translated";
			this.tsiView_Untranslated.Click += new System.EventHandler(this.tsiView_Untranslated_Click);
			// 
			// tsiView_Translated
			// 
			this.tsiView_Translated.AutoSize = false;
			this.tsiView_Translated.Image = ((System.Drawing.Image)(resources.GetObject("tsiView_Translated.Image")));
			this.tsiView_Translated.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.tsiView_Translated.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsiView_Translated.Name = "tsiView_Translated";
			this.tsiView_Translated.Size = new System.Drawing.Size(73, 49);
			this.tsiView_Translated.Text = "Translated";
			this.tsiView_Translated.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.tsiView_Translated.ToolTipText = "Show translated items";
			this.tsiView_Translated.Click += new System.EventHandler(this.tsiView_Translated_Click);
			// 
			// tsiView_Removed
			// 
			this.tsiView_Removed.AutoSize = false;
			this.tsiView_Removed.Image = ((System.Drawing.Image)(resources.GetObject("tsiView_Removed.Image")));
			this.tsiView_Removed.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.tsiView_Removed.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsiView_Removed.Name = "tsiView_Removed";
			this.tsiView_Removed.Size = new System.Drawing.Size(73, 49);
			this.tsiView_Removed.Text = "Removed";
			this.tsiView_Removed.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.tsiView_Removed.ToolTipText = "Show removed items (no more in use)";
			this.tsiView_Removed.Click += new System.EventHandler(this.tsiView_Removed_Click);
			// 
			// tsiSep1
			// 
			this.tsiSep1.Name = "tsiSep1";
			this.tsiSep1.Size = new System.Drawing.Size(6, 52);
			// 
			// tsiView_Fuzzy
			// 
			this.tsiView_Fuzzy.AutoSize = false;
			this.tsiView_Fuzzy.Checked = true;
			this.tsiView_Fuzzy.CheckState = System.Windows.Forms.CheckState.Checked;
			this.tsiView_Fuzzy.Image = ((System.Drawing.Image)(resources.GetObject("tsiView_Fuzzy.Image")));
			this.tsiView_Fuzzy.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.tsiView_Fuzzy.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsiView_Fuzzy.Name = "tsiView_Fuzzy";
			this.tsiView_Fuzzy.Size = new System.Drawing.Size(41, 49);
			this.tsiView_Fuzzy.Text = "Fuzzy";
			this.tsiView_Fuzzy.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.tsiView_Fuzzy.ToolTipText = "Show items that are marked as \'fuzzy\'";
			this.tsiView_Fuzzy.Click += new System.EventHandler(this.tsiView_Fuzzy_Click);
			// 
			// tsiView_Clear
			// 
			this.tsiView_Clear.Checked = true;
			this.tsiView_Clear.CheckState = System.Windows.Forms.CheckState.Checked;
			this.tsiView_Clear.Image = ((System.Drawing.Image)(resources.GetObject("tsiView_Clear.Image")));
			this.tsiView_Clear.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.tsiView_Clear.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsiView_Clear.Name = "tsiView_Clear";
			this.tsiView_Clear.Size = new System.Drawing.Size(36, 49);
			this.tsiView_Clear.Text = "Clear";
			this.tsiView_Clear.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.tsiView_Clear.ToolTipText = "Show items that are not marked as \'fuzzy\'";
			this.tsiView_Clear.Click += new System.EventHandler(this.tsiView_Clear_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 52);
			// 
			// tsiSearch
			// 
			this.tsiSearch.Image = ((System.Drawing.Image)(resources.GetObject("tsiSearch.Image")));
			this.tsiSearch.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.tsiSearch.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsiSearch.Name = "tsiSearch";
			this.tsiSearch.Size = new System.Drawing.Size(44, 49);
			this.tsiSearch.Text = "Search";
			this.tsiSearch.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.tsiSearch.Click += new System.EventHandler(this.tsiSearch_Click);
			// 
			// scItems
			// 
			this.scItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
							| System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.scItems.Location = new System.Drawing.Point(12, 55);
			this.scItems.Name = "scItems";
			// 
			// scItems.Panel1
			// 
			this.scItems.Panel1.Controls.Add(this.lbxEntries);
			// 
			// scItems.Panel2
			// 
			this.scItems.Panel2.Controls.Add(this.scEntry);
			this.scItems.Size = new System.Drawing.Size(776, 374);
			this.scItems.SplitterDistance = 229;
			this.scItems.TabIndex = 1;
			// 
			// lbxEntries
			// 
			this.lbxEntries.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbxEntries.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.lbxEntries.FormattingEnabled = true;
			this.lbxEntries.Location = new System.Drawing.Point(0, 0);
			this.lbxEntries.Name = "lbxEntries";
			this.lbxEntries.Size = new System.Drawing.Size(229, 368);
			this.lbxEntries.TabIndex = 0;
			this.lbxEntries.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lbxEntries_DrawItem);
			this.lbxEntries.SelectedIndexChanged += new System.EventHandler(this.lbxEntries_SelectedIndexChanged);
			// 
			// scEntry
			// 
			this.scEntry.Dock = System.Windows.Forms.DockStyle.Fill;
			this.scEntry.Location = new System.Drawing.Point(0, 0);
			this.scEntry.Name = "scEntry";
			this.scEntry.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// scEntry.Panel1
			// 
			this.scEntry.Panel1.Controls.Add(this.tbxSourceContext);
			this.scEntry.Panel1.Controls.Add(this.scSource);
			this.scEntry.Panel1.Controls.Add(this.lblSourceContext);
			// 
			// scEntry.Panel2
			// 
			this.scEntry.Panel2.Controls.Add(this.lnkCheckTranslationSpelling);
			this.scEntry.Panel2.Controls.Add(this.lnkTidyTranslation);
			this.scEntry.Panel2.Controls.Add(this.chkFuzzyTranslation);
			this.scEntry.Panel2.Controls.Add(this.scTranslation);
			this.scEntry.Panel2.Controls.Add(this.lnkTranslatorComment);
			this.scEntry.Panel2.Controls.Add(this.lnkGoogleTranslate);
			this.scEntry.Size = new System.Drawing.Size(543, 374);
			this.scEntry.SplitterDistance = 141;
			this.scEntry.TabIndex = 0;
			// 
			// tbxSourceContext
			// 
			this.tbxSourceContext.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.tbxSourceContext.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbxSourceContext.HideSelection = false;
			this.tbxSourceContext.Location = new System.Drawing.Point(49, 121);
			this.tbxSourceContext.Multiline = true;
			this.tbxSourceContext.Name = "tbxSourceContext";
			this.tbxSourceContext.ReadOnly = true;
			this.tbxSourceContext.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.tbxSourceContext.Size = new System.Drawing.Size(494, 20);
			this.tbxSourceContext.TabIndex = 2;
			// 
			// scSource
			// 
			this.scSource.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
							| System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.scSource.Location = new System.Drawing.Point(0, 0);
			this.scSource.Name = "scSource";
			// 
			// scSource.Panel1
			// 
			this.scSource.Panel1.Controls.Add(this.tcSource);
			// 
			// scSource.Panel2
			// 
			this.scSource.Panel2.Controls.Add(this.tcSource2);
			this.scSource.Size = new System.Drawing.Size(543, 116);
			this.scSource.SplitterDistance = 350;
			this.scSource.TabIndex = 0;
			// 
			// tcSource
			// 
			this.tcSource.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
			this.tcSource.Controls.Add(this.tpSource0);
			this.tcSource.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tcSource.Location = new System.Drawing.Point(0, 0);
			this.tcSource.Name = "tcSource";
			this.tcSource.SelectedIndex = 0;
			this.tcSource.Size = new System.Drawing.Size(350, 116);
			this.tcSource.TabIndex = 0;
			// 
			// tpSource0
			// 
			this.tpSource0.Controls.Add(this.tbxSource0);
			this.tpSource0.Location = new System.Drawing.Point(4, 25);
			this.tpSource0.Name = "tpSource0";
			this.tpSource0.Padding = new System.Windows.Forms.Padding(3);
			this.tpSource0.Size = new System.Drawing.Size(342, 87);
			this.tpSource0.TabIndex = 0;
			this.tpSource0.Text = "tabPage1";
			this.tpSource0.UseVisualStyleBackColor = true;
			// 
			// tbxSource0
			// 
			this.tbxSource0.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbxSource0.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbxSource0.HideSelection = false;
			this.tbxSource0.Location = new System.Drawing.Point(3, 3);
			this.tbxSource0.Multiline = true;
			this.tbxSource0.Name = "tbxSource0";
			this.tbxSource0.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.tbxSource0.Size = new System.Drawing.Size(336, 81);
			this.tbxSource0.TabIndex = 0;
			// 
			// tcSource2
			// 
			this.tcSource2.Controls.Add(this.tpExtractedComment);
			this.tcSource2.Controls.Add(this.tpReferences);
			this.tcSource2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tcSource2.Location = new System.Drawing.Point(0, 0);
			this.tcSource2.Name = "tcSource2";
			this.tcSource2.SelectedIndex = 0;
			this.tcSource2.Size = new System.Drawing.Size(189, 116);
			this.tcSource2.TabIndex = 0;
			// 
			// tpExtractedComment
			// 
			this.tpExtractedComment.Controls.Add(this.tbxExtractedComment);
			this.tpExtractedComment.Location = new System.Drawing.Point(4, 22);
			this.tpExtractedComment.Name = "tpExtractedComment";
			this.tpExtractedComment.Padding = new System.Windows.Forms.Padding(3);
			this.tpExtractedComment.Size = new System.Drawing.Size(181, 90);
			this.tpExtractedComment.TabIndex = 0;
			this.tpExtractedComment.Text = "Source comments";
			this.tpExtractedComment.UseVisualStyleBackColor = true;
			// 
			// tbxExtractedComment
			// 
			this.tbxExtractedComment.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbxExtractedComment.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbxExtractedComment.HideSelection = false;
			this.tbxExtractedComment.Location = new System.Drawing.Point(3, 3);
			this.tbxExtractedComment.Multiline = true;
			this.tbxExtractedComment.Name = "tbxExtractedComment";
			this.tbxExtractedComment.ReadOnly = true;
			this.tbxExtractedComment.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.tbxExtractedComment.Size = new System.Drawing.Size(175, 84);
			this.tbxExtractedComment.TabIndex = 0;
			// 
			// tpReferences
			// 
			this.tpReferences.Controls.Add(this.flpReferences);
			this.tpReferences.Location = new System.Drawing.Point(4, 22);
			this.tpReferences.Name = "tpReferences";
			this.tpReferences.Padding = new System.Windows.Forms.Padding(3);
			this.tpReferences.Size = new System.Drawing.Size(181, 90);
			this.tpReferences.TabIndex = 1;
			this.tpReferences.Text = "References";
			this.tpReferences.UseVisualStyleBackColor = true;
			// 
			// flpReferences
			// 
			this.flpReferences.AutoScroll = true;
			this.flpReferences.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.flpReferences.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flpReferences.Location = new System.Drawing.Point(3, 3);
			this.flpReferences.Name = "flpReferences";
			this.flpReferences.Size = new System.Drawing.Size(175, 84);
			this.flpReferences.TabIndex = 0;
			// 
			// lblSourceContext
			// 
			this.lblSourceContext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblSourceContext.AutoSize = true;
			this.lblSourceContext.Location = new System.Drawing.Point(0, 125);
			this.lblSourceContext.Name = "lblSourceContext";
			this.lblSourceContext.Size = new System.Drawing.Size(43, 13);
			this.lblSourceContext.TabIndex = 1;
			this.lblSourceContext.Text = "Context";
			// 
			// lnkCheckTranslationSpelling
			// 
			this.lnkCheckTranslationSpelling.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lnkCheckTranslationSpelling.AutoSize = true;
			this.lnkCheckTranslationSpelling.Location = new System.Drawing.Point(316, 211);
			this.lnkCheckTranslationSpelling.Name = "lnkCheckTranslationSpelling";
			this.lnkCheckTranslationSpelling.Size = new System.Drawing.Size(63, 13);
			this.lnkCheckTranslationSpelling.TabIndex = 5;
			this.lnkCheckTranslationSpelling.TabStop = true;
			this.lnkCheckTranslationSpelling.Text = "Spell check";
			this.lnkCheckTranslationSpelling.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkCheckTranslationSpelling_LinkClicked);
			// 
			// lnkTidyTranslation
			// 
			this.lnkTidyTranslation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lnkTidyTranslation.AutoSize = true;
			this.lnkTidyTranslation.Location = new System.Drawing.Point(255, 211);
			this.lnkTidyTranslation.Name = "lnkTidyTranslation";
			this.lnkTidyTranslation.Size = new System.Drawing.Size(55, 13);
			this.lnkTidyTranslation.TabIndex = 4;
			this.lnkTidyTranslation.TabStop = true;
			this.lnkTidyTranslation.Text = "Verify html";
			this.lnkTidyTranslation.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkTidyTranslation_LinkClicked);
			// 
			// chkFuzzyTranslation
			// 
			this.chkFuzzyTranslation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.chkFuzzyTranslation.AutoSize = true;
			this.chkFuzzyTranslation.Location = new System.Drawing.Point(3, 209);
			this.chkFuzzyTranslation.Name = "chkFuzzyTranslation";
			this.chkFuzzyTranslation.Size = new System.Drawing.Size(50, 17);
			this.chkFuzzyTranslation.TabIndex = 1;
			this.chkFuzzyTranslation.Text = "fuzzy";
			this.chkFuzzyTranslation.UseVisualStyleBackColor = true;
			this.chkFuzzyTranslation.CheckedChanged += new System.EventHandler(this.chkFuzzyTranslation_CheckedChanged);
			// 
			// scTranslation
			// 
			this.scTranslation.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
							| System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.scTranslation.Location = new System.Drawing.Point(0, 0);
			this.scTranslation.Name = "scTranslation";
			// 
			// scTranslation.Panel1
			// 
			this.scTranslation.Panel1.Controls.Add(this.tcTranslated);
			// 
			// scTranslation.Panel2
			// 
			this.scTranslation.Panel2.Controls.Add(this.tbxTranslatorComment);
			this.scTranslation.Panel2.Controls.Add(this.label2);
			this.scTranslation.Size = new System.Drawing.Size(543, 207);
			this.scTranslation.SplitterDistance = 350;
			this.scTranslation.TabIndex = 0;
			// 
			// tcTranslated
			// 
			this.tcTranslated.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
			this.tcTranslated.Controls.Add(this.tpTranslated0);
			this.tcTranslated.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tcTranslated.Location = new System.Drawing.Point(0, 0);
			this.tcTranslated.Name = "tcTranslated";
			this.tcTranslated.SelectedIndex = 0;
			this.tcTranslated.Size = new System.Drawing.Size(350, 207);
			this.tcTranslated.TabIndex = 0;
			// 
			// tpTranslated0
			// 
			this.tpTranslated0.Controls.Add(this.tbxTranslated0);
			this.tpTranslated0.Location = new System.Drawing.Point(4, 25);
			this.tpTranslated0.Name = "tpTranslated0";
			this.tpTranslated0.Padding = new System.Windows.Forms.Padding(3);
			this.tpTranslated0.Size = new System.Drawing.Size(342, 178);
			this.tpTranslated0.TabIndex = 0;
			this.tpTranslated0.Text = "tabPage1";
			this.tpTranslated0.UseVisualStyleBackColor = true;
			// 
			// tbxTranslated0
			// 
			this.tbxTranslated0.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbxTranslated0.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbxTranslated0.HideSelection = false;
			this.tbxTranslated0.Location = new System.Drawing.Point(3, 3);
			this.tbxTranslated0.Multiline = true;
			this.tbxTranslated0.Name = "tbxTranslated0";
			this.tbxTranslated0.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.tbxTranslated0.Size = new System.Drawing.Size(336, 172);
			this.tbxTranslated0.TabIndex = 0;
			// 
			// tbxTranslatorComment
			// 
			this.tbxTranslatorComment.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
							| System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.tbxTranslatorComment.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbxTranslatorComment.HideSelection = false;
			this.tbxTranslatorComment.Location = new System.Drawing.Point(1, 25);
			this.tbxTranslatorComment.Multiline = true;
			this.tbxTranslatorComment.Name = "tbxTranslatorComment";
			this.tbxTranslatorComment.ReadOnly = true;
			this.tbxTranslatorComment.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.tbxTranslatorComment.Size = new System.Drawing.Size(184, 175);
			this.tbxTranslatorComment.TabIndex = 1;
			this.tbxTranslatorComment.TextChanged += new System.EventHandler(this.tbxTranslatorComment_TextChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(0, 6);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(105, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Translator comments";
			// 
			// lnkTranslatorComment
			// 
			this.lnkTranslatorComment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lnkTranslatorComment.AutoSize = true;
			this.lnkTranslatorComment.Location = new System.Drawing.Point(149, 211);
			this.lnkTranslatorComment.Name = "lnkTranslatorComment";
			this.lnkTranslatorComment.Size = new System.Drawing.Size(100, 13);
			this.lnkTranslatorComment.TabIndex = 3;
			this.lnkTranslatorComment.TabStop = true;
			this.lnkTranslatorComment.Text = "Translator comment";
			this.lnkTranslatorComment.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkTranslatorComment_LinkClicked);
			// 
			// lnkGoogleTranslate
			// 
			this.lnkGoogleTranslate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lnkGoogleTranslate.AutoSize = true;
			this.lnkGoogleTranslate.Location = new System.Drawing.Point(59, 211);
			this.lnkGoogleTranslate.Name = "lnkGoogleTranslate";
			this.lnkGoogleTranslate.Size = new System.Drawing.Size(84, 13);
			this.lnkGoogleTranslate.TabIndex = 2;
			this.lnkGoogleTranslate.TabStop = true;
			this.lnkGoogleTranslate.Text = "Google translate";
			this.lnkGoogleTranslate.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkGoogleTranslate_LinkClicked);
			// 
			// ssStatus
			// 
			this.ssStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sslStatus,
            this.sslProgr,
            this.ssbProgr});
			this.ssStatus.Location = new System.Drawing.Point(0, 432);
			this.ssStatus.Name = "ssStatus";
			this.ssStatus.Size = new System.Drawing.Size(800, 22);
			this.ssStatus.TabIndex = 2;
			this.ssStatus.Text = "statusStrip1";
			// 
			// sslStatus
			// 
			this.sslStatus.Name = "sslStatus";
			this.sslStatus.Size = new System.Drawing.Size(683, 17);
			this.sslStatus.Spring = true;
			this.sslStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// sslProgr
			// 
			this.sslProgr.Name = "sslProgr";
			this.sslProgr.Size = new System.Drawing.Size(0, 17);
			this.sslProgr.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// ssbProgr
			// 
			this.ssbProgr.Name = "ssbProgr";
			this.ssbProgr.Size = new System.Drawing.Size(100, 16);
			this.ssbProgr.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			// 
			// fbdReference
			// 
			this.fbdReference.Description = "Select the base reference folder";
			this.fbdReference.ShowNewFolderButton = false;
			// 
			// frmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 454);
			this.Controls.Add(this.ssStatus);
			this.Controls.Add(this.scItems);
			this.Controls.Add(this.tsTools);
			this.KeyPreview = true;
			this.Name = "frmMain";
			this.Text = "BetterPOEditor";
			this.Shown += new System.EventHandler(this.frmMain_Shown);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmMain_KeyDown);
			this.tsTools.ResumeLayout(false);
			this.tsTools.PerformLayout();
			this.scItems.Panel1.ResumeLayout(false);
			this.scItems.Panel2.ResumeLayout(false);
			this.scItems.ResumeLayout(false);
			this.scEntry.Panel1.ResumeLayout(false);
			this.scEntry.Panel1.PerformLayout();
			this.scEntry.Panel2.ResumeLayout(false);
			this.scEntry.Panel2.PerformLayout();
			this.scEntry.ResumeLayout(false);
			this.scSource.Panel1.ResumeLayout(false);
			this.scSource.Panel2.ResumeLayout(false);
			this.scSource.ResumeLayout(false);
			this.tcSource.ResumeLayout(false);
			this.tpSource0.ResumeLayout(false);
			this.tpSource0.PerformLayout();
			this.tcSource2.ResumeLayout(false);
			this.tpExtractedComment.ResumeLayout(false);
			this.tpExtractedComment.PerformLayout();
			this.tpReferences.ResumeLayout(false);
			this.scTranslation.Panel1.ResumeLayout(false);
			this.scTranslation.Panel2.ResumeLayout(false);
			this.scTranslation.Panel2.PerformLayout();
			this.scTranslation.ResumeLayout(false);
			this.tcTranslated.ResumeLayout(false);
			this.tpTranslated0.ResumeLayout(false);
			this.tpTranslated0.PerformLayout();
			this.ssStatus.ResumeLayout(false);
			this.ssStatus.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.OpenFileDialog ofdOpen;
		private System.Windows.Forms.ToolStrip tsTools;
		private System.Windows.Forms.ToolStripButton tsiSave;
		private System.Windows.Forms.ToolStripDropDownButton tsiFileFormat;
		private System.Windows.Forms.ToolStripMenuItem mniFileFormat_LineEndings;
		private System.Windows.Forms.ToolStripMenuItem mniFileFormat_Encoding;
		private System.Windows.Forms.ToolStripMenuItem mniFileFormat_Encoding_ANSI;
		private System.Windows.Forms.ToolStripMenuItem mniFileFormat_Encoding_UTF8_NoBOM;
		private System.Windows.Forms.ToolStripMenuItem mniFileFormat_Encoding_UTF8_BOM;
		private System.Windows.Forms.ToolStripMenuItem mniFileFormat_Encoding_UTF16_BE;
		private System.Windows.Forms.ToolStripMenuItem mniFileFormat_Encoding_UTF16_LE;
		private System.Windows.Forms.ToolStripMenuItem mniFileFormat_Encoding_UTF32_BE;
		private System.Windows.Forms.ToolStripMenuItem mniFileFormat_Encoding_UTF32_LE;
		private System.Windows.Forms.ToolStripMenuItem mniFileFormat_LineEndings_Linux;
		private System.Windows.Forms.ToolStripMenuItem mniFileFormat_LineEndings_Windows;
		private System.Windows.Forms.ToolStripMenuItem mniFileFormat_LineEndings_Mac;
		private System.Windows.Forms.ToolStripButton tsiInfo;
		private System.Windows.Forms.ToolStripButton tsiOptions;
		private System.Windows.Forms.ToolStripSeparator tsiSep0;
		private System.Windows.Forms.ToolStripDropDownButton tsiOpen;
		private System.Windows.Forms.ToolStripButton tsiView_Untranslated;
		private System.Windows.Forms.ToolStripButton tsiView_Translated;
		private System.Windows.Forms.ToolStripButton tsiView_Removed;
		private System.Windows.Forms.SplitContainer scItems;
		private System.Windows.Forms.ListBox lbxEntries;
		private System.Windows.Forms.StatusStrip ssStatus;
		private System.Windows.Forms.ToolStripStatusLabel sslStatus;
		private System.Windows.Forms.ToolStripStatusLabel sslProgr;
		private System.Windows.Forms.ToolStripProgressBar ssbProgr;
		private System.Windows.Forms.ToolStripSeparator tsiSep1;
		private System.Windows.Forms.ToolStripButton tsiView_Fuzzy;
		private System.Windows.Forms.ToolStripButton tsiView_Clear;
		private System.Windows.Forms.SplitContainer scEntry;
		private System.Windows.Forms.TabControl tcSource;
		private System.Windows.Forms.TabControl tcTranslated;
		private System.Windows.Forms.TabPage tpTranslated0;
		private System.Windows.Forms.TabPage tpSource0;
		private System.Windows.Forms.TextBox tbxSource0;
		private System.Windows.Forms.TextBox tbxTranslated0;
		private System.Windows.Forms.LinkLabel lnkGoogleTranslate;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton tsiSearch;
		private System.Windows.Forms.TextBox tbxSourceContext;
		private System.Windows.Forms.Label lblSourceContext;
		private System.Windows.Forms.FolderBrowserDialog fbdReference;
		private System.Windows.Forms.SplitContainer scSource;
		private System.Windows.Forms.TextBox tbxExtractedComment;
		private System.Windows.Forms.SplitContainer scTranslation;
		private System.Windows.Forms.TextBox tbxTranslatorComment;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.LinkLabel lnkTranslatorComment;
		private System.Windows.Forms.CheckBox chkFuzzyTranslation;
		private System.Windows.Forms.LinkLabel lnkTidyTranslation;
		private System.Windows.Forms.LinkLabel lnkCheckTranslationSpelling;
		private System.Windows.Forms.ToolStripButton tsiCompile;
		private System.Windows.Forms.TabControl tcSource2;
		private System.Windows.Forms.TabPage tpExtractedComment;
		private System.Windows.Forms.TabPage tpReferences;
		private System.Windows.Forms.FlowLayoutPanel flpReferences;
	}
}