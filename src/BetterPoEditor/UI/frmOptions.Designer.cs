namespace BePoE.UI
{
	partial class frmOptions
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
			this.btnKo = new System.Windows.Forms.Button();
			this.btnApply = new System.Windows.Forms.Button();
			this.btnOk = new System.Windows.Forms.Button();
			this.tlpColors = new System.Windows.Forms.TableLayoutPanel();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.lnkDefaultColors = new System.Windows.Forms.LinkLabel();
			this.cdPick = new System.Windows.Forms.ColorDialog();
			this.label11 = new System.Windows.Forms.Label();
			this.lnkFontSource = new System.Windows.Forms.LinkLabel();
			this.label12 = new System.Windows.Forms.Label();
			this.lnkFontTranslation = new System.Windows.Forms.LinkLabel();
			this.fdPick = new System.Windows.Forms.FontDialog();
			this.tcTabs = new System.Windows.Forms.TabControl();
			this.tpGeneral = new System.Windows.Forms.TabPage();
			this.nudMaxSearchResults = new System.Windows.Forms.NumericUpDown();
			this.label6 = new System.Windows.Forms.Label();
			this.chkCompileOnSave = new System.Windows.Forms.CheckBox();
			this.tpAppearance = new System.Windows.Forms.TabPage();
			this.tlpFonts = new System.Windows.Forms.TableLayoutPanel();
			this.tpPaths = new System.Windows.Forms.TabPage();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label19 = new System.Windows.Forms.Label();
			this.tbxMOCompilerParams = new System.Windows.Forms.TextBox();
			this.label18 = new System.Windows.Forms.Label();
			this.label17 = new System.Windows.Forms.Label();
			this.btnMOCompilerPath = new System.Windows.Forms.Button();
			this.tbxMOCompilerPath = new System.Windows.Forms.TextBox();
			this.label20 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label16 = new System.Windows.Forms.Label();
			this.tbxViewerParams = new System.Windows.Forms.TextBox();
			this.label14 = new System.Windows.Forms.Label();
			this.label15 = new System.Windows.Forms.Label();
			this.btnViewerPath = new System.Windows.Forms.Button();
			this.tbxViewerPath = new System.Windows.Forms.TextBox();
			this.label13 = new System.Windows.Forms.Label();
			this.ofdViewer = new System.Windows.Forms.OpenFileDialog();
			this.ofdMOCompiler = new System.Windows.Forms.OpenFileDialog();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.tlpColors.SuspendLayout();
			this.tcTabs.SuspendLayout();
			this.tpGeneral.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudMaxSearchResults)).BeginInit();
			this.tpAppearance.SuspendLayout();
			this.tlpFonts.SuspendLayout();
			this.tpPaths.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnKo
			// 
			this.btnKo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnKo.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnKo.Location = new System.Drawing.Point(266, 385);
			this.btnKo.Name = "btnKo";
			this.btnKo.Size = new System.Drawing.Size(75, 23);
			this.btnKo.TabIndex = 1;
			this.btnKo.Text = "Cancel";
			this.btnKo.UseVisualStyleBackColor = true;
			this.btnKo.Click += new System.EventHandler(this.btnKo_Click);
			// 
			// btnApply
			// 
			this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnApply.Location = new System.Drawing.Point(347, 385);
			this.btnApply.Name = "btnApply";
			this.btnApply.Size = new System.Drawing.Size(75, 23);
			this.btnApply.TabIndex = 2;
			this.btnApply.Text = "Apply";
			this.btnApply.UseVisualStyleBackColor = true;
			this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
			// 
			// btnOk
			// 
			this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOk.Location = new System.Drawing.Point(428, 385);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 3;
			this.btnOk.Text = "OK";
			this.btnOk.UseVisualStyleBackColor = true;
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// tlpColors
			// 
			this.tlpColors.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.tlpColors.ColumnCount = 5;
			this.tlpColors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpColors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
			this.tlpColors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
			this.tlpColors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
			this.tlpColors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
			this.tlpColors.Controls.Add(this.label2, 1, 0);
			this.tlpColors.Controls.Add(this.label3, 2, 0);
			this.tlpColors.Controls.Add(this.label5, 3, 0);
			this.tlpColors.Controls.Add(this.label4, 4, 0);
			this.tlpColors.Controls.Add(this.label1, 0, 1);
			this.tlpColors.Controls.Add(this.label7, 0, 2);
			this.tlpColors.Controls.Add(this.label9, 0, 3);
			this.tlpColors.Location = new System.Drawing.Point(45, 19);
			this.tlpColors.Name = "tlpColors";
			this.tlpColors.RowCount = 4;
			this.tlpColors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpColors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tlpColors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tlpColors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tlpColors.Size = new System.Drawing.Size(387, 86);
			this.tlpColors.TabIndex = 0;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label2.Location = new System.Drawing.Point(67, 0);
			this.label2.Margin = new System.Windows.Forms.Padding(0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(80, 26);
			this.label2.TabIndex = 0;
			this.label2.Text = "foreground\r\nunselected";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label3.Location = new System.Drawing.Point(147, 0);
			this.label3.Margin = new System.Windows.Forms.Padding(0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(80, 26);
			this.label3.TabIndex = 1;
			this.label3.Text = "background\r\nunselected";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label5.Location = new System.Drawing.Point(227, 0);
			this.label5.Margin = new System.Windows.Forms.Padding(0);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(80, 26);
			this.label5.TabIndex = 2;
			this.label5.Text = "foreground\r\nselected";
			this.label5.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label4.Location = new System.Drawing.Point(307, 0);
			this.label4.Margin = new System.Windows.Forms.Padding(0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(80, 26);
			this.label4.TabIndex = 3;
			this.label4.Text = "background\r\nselected";
			this.label4.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label1.Location = new System.Drawing.Point(0, 26);
			this.label1.Margin = new System.Windows.Forms.Padding(0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(67, 20);
			this.label1.TabIndex = 4;
			this.label1.Text = "Untranslated";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label7.Location = new System.Drawing.Point(0, 46);
			this.label7.Margin = new System.Windows.Forms.Padding(0);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(67, 20);
			this.label7.TabIndex = 5;
			this.label7.Text = "Translated";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label9.Location = new System.Drawing.Point(0, 66);
			this.label9.Margin = new System.Windows.Forms.Padding(0);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(67, 20);
			this.label9.TabIndex = 6;
			this.label9.Text = "Removed";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lnkDefaultColors
			// 
			this.lnkDefaultColors.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.lnkDefaultColors.AutoSize = true;
			this.lnkDefaultColors.Location = new System.Drawing.Point(168, 108);
			this.lnkDefaultColors.Name = "lnkDefaultColors";
			this.lnkDefaultColors.Size = new System.Drawing.Size(118, 13);
			this.lnkDefaultColors.TabIndex = 1;
			this.lnkDefaultColors.TabStop = true;
			this.lnkDefaultColors.Text = "reverto to default colors";
			this.lnkDefaultColors.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkDefaultColors_LinkClicked);
			// 
			// cdPick
			// 
			this.cdPick.AnyColor = true;
			this.cdPick.FullOpen = true;
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(3, 5);
			this.label11.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(61, 13);
			this.label11.TabIndex = 0;
			this.label11.Text = "Source text";
			// 
			// lnkFontSource
			// 
			this.lnkFontSource.AutoSize = true;
			this.lnkFontSource.Location = new System.Drawing.Point(103, 5);
			this.lnkFontSource.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
			this.lnkFontSource.Name = "lnkFontSource";
			this.lnkFontSource.Size = new System.Drawing.Size(76, 13);
			this.lnkFontSource.TabIndex = 1;
			this.lnkFontSource.TabStop = true;
			this.lnkFontSource.Text = "lnkFontSource";
			this.lnkFontSource.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkFontSource_LinkClicked);
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(3, 28);
			this.label12.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(77, 13);
			this.label12.TabIndex = 2;
			this.label12.Text = "Translated text";
			// 
			// lnkFontTranslation
			// 
			this.lnkFontTranslation.AutoSize = true;
			this.lnkFontTranslation.Location = new System.Drawing.Point(103, 28);
			this.lnkFontTranslation.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
			this.lnkFontTranslation.Name = "lnkFontTranslation";
			this.lnkFontTranslation.Size = new System.Drawing.Size(94, 13);
			this.lnkFontTranslation.TabIndex = 3;
			this.lnkFontTranslation.TabStop = true;
			this.lnkFontTranslation.Text = "lnkFontTranslation";
			this.lnkFontTranslation.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkFontTranslation_LinkClicked);
			// 
			// fdPick
			// 
			this.fdPick.FontMustExist = true;
			// 
			// tcTabs
			// 
			this.tcTabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
							| System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.tcTabs.Controls.Add(this.tpGeneral);
			this.tcTabs.Controls.Add(this.tpAppearance);
			this.tcTabs.Controls.Add(this.tpPaths);
			this.tcTabs.Location = new System.Drawing.Point(12, 12);
			this.tcTabs.Name = "tcTabs";
			this.tcTabs.SelectedIndex = 0;
			this.tcTabs.Size = new System.Drawing.Size(491, 349);
			this.tcTabs.TabIndex = 0;
			// 
			// tpGeneral
			// 
			this.tpGeneral.Controls.Add(this.nudMaxSearchResults);
			this.tpGeneral.Controls.Add(this.label6);
			this.tpGeneral.Controls.Add(this.chkCompileOnSave);
			this.tpGeneral.Location = new System.Drawing.Point(4, 22);
			this.tpGeneral.Name = "tpGeneral";
			this.tpGeneral.Padding = new System.Windows.Forms.Padding(3);
			this.tpGeneral.Size = new System.Drawing.Size(483, 323);
			this.tpGeneral.TabIndex = 0;
			this.tpGeneral.Text = "General";
			this.tpGeneral.UseVisualStyleBackColor = true;
			// 
			// nudMaxSearchResults
			// 
			this.nudMaxSearchResults.Location = new System.Drawing.Point(264, 71);
			this.nudMaxSearchResults.Maximum = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
			this.nudMaxSearchResults.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudMaxSearchResults.Name = "nudMaxSearchResults";
			this.nudMaxSearchResults.Size = new System.Drawing.Size(79, 20);
			this.nudMaxSearchResults.TabIndex = 2;
			this.nudMaxSearchResults.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.nudMaxSearchResults.ThousandsSeparator = true;
			this.nudMaxSearchResults.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(139, 73);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(119, 13);
			this.label6.TabIndex = 1;
			this.label6.Text = "Maximum search results";
			// 
			// chkCompileOnSave
			// 
			this.chkCompileOnSave.AutoSize = true;
			this.chkCompileOnSave.Location = new System.Drawing.Point(190, 29);
			this.chkCompileOnSave.Name = "chkCompileOnSave";
			this.chkCompileOnSave.Size = new System.Drawing.Size(103, 17);
			this.chkCompileOnSave.TabIndex = 0;
			this.chkCompileOnSave.Text = "compile on save";
			this.chkCompileOnSave.UseVisualStyleBackColor = true;
			// 
			// tpAppearance
			// 
			this.tpAppearance.Controls.Add(this.groupBox4);
			this.tpAppearance.Controls.Add(this.groupBox3);
			this.tpAppearance.Location = new System.Drawing.Point(4, 22);
			this.tpAppearance.Name = "tpAppearance";
			this.tpAppearance.Padding = new System.Windows.Forms.Padding(3);
			this.tpAppearance.Size = new System.Drawing.Size(483, 323);
			this.tpAppearance.TabIndex = 1;
			this.tpAppearance.Text = "Appearance";
			this.tpAppearance.UseVisualStyleBackColor = true;
			// 
			// tlpFonts
			// 
			this.tlpFonts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.tlpFonts.AutoSize = true;
			this.tlpFonts.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tlpFonts.ColumnCount = 2;
			this.tlpFonts.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
			this.tlpFonts.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpFonts.Controls.Add(this.label11, 0, 0);
			this.tlpFonts.Controls.Add(this.lnkFontTranslation, 1, 1);
			this.tlpFonts.Controls.Add(this.lnkFontSource, 1, 0);
			this.tlpFonts.Controls.Add(this.label12, 0, 1);
			this.tlpFonts.Location = new System.Drawing.Point(6, 19);
			this.tlpFonts.Name = "tlpFonts";
			this.tlpFonts.RowCount = 2;
			this.tlpFonts.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tlpFonts.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tlpFonts.Size = new System.Drawing.Size(442, 46);
			this.tlpFonts.TabIndex = 0;
			// 
			// tpPaths
			// 
			this.tpPaths.Controls.Add(this.groupBox1);
			this.tpPaths.Controls.Add(this.groupBox2);
			this.tpPaths.Location = new System.Drawing.Point(4, 22);
			this.tpPaths.Name = "tpPaths";
			this.tpPaths.Padding = new System.Windows.Forms.Padding(3);
			this.tpPaths.Size = new System.Drawing.Size(483, 323);
			this.tpPaths.TabIndex = 3;
			this.tpPaths.Text = "Paths";
			this.tpPaths.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.label19);
			this.groupBox1.Controls.Add(this.tbxMOCompilerParams);
			this.groupBox1.Controls.Add(this.label18);
			this.groupBox1.Controls.Add(this.label17);
			this.groupBox1.Controls.Add(this.btnMOCompilerPath);
			this.groupBox1.Controls.Add(this.tbxMOCompilerPath);
			this.groupBox1.Controls.Add(this.label20);
			this.groupBox1.Location = new System.Drawing.Point(9, 161);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(468, 158);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "PO -> PO compiler";
			// 
			// label19
			// 
			this.label19.AutoSize = true;
			this.label19.ForeColor = System.Drawing.SystemColors.GrayText;
			this.label19.Location = new System.Drawing.Point(6, 136);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(290, 13);
			this.label19.TabIndex = 6;
			this.label19.Text = "Example for msgfmt: %source% --output-file=\"%destination%\"";
			this.label19.UseMnemonic = false;
			// 
			// tbxMOCompilerParams
			// 
			this.tbxMOCompilerParams.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.tbxMOCompilerParams.Location = new System.Drawing.Point(6, 81);
			this.tbxMOCompilerParams.Name = "tbxMOCompilerParams";
			this.tbxMOCompilerParams.Size = new System.Drawing.Size(453, 20);
			this.tbxMOCompilerParams.TabIndex = 4;
			// 
			// label18
			// 
			this.label18.ForeColor = System.Drawing.SystemColors.GrayText;
			this.label18.Location = new System.Drawing.Point(6, 104);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(453, 32);
			this.label18.TabIndex = 5;
			this.label18.Text = "%source% will be replaced with the source .po file, %destination% will be replace" +
				 "d with the .mo file to generate";
			this.label18.UseMnemonic = false;
			// 
			// label17
			// 
			this.label17.AutoSize = true;
			this.label17.Location = new System.Drawing.Point(6, 64);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(102, 13);
			this.label17.TabIndex = 3;
			this.label17.Text = "Compiler parameters";
			// 
			// btnMOCompilerPath
			// 
			this.btnMOCompilerPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnMOCompilerPath.Location = new System.Drawing.Point(384, 36);
			this.btnMOCompilerPath.Name = "btnMOCompilerPath";
			this.btnMOCompilerPath.Size = new System.Drawing.Size(75, 23);
			this.btnMOCompilerPath.TabIndex = 2;
			this.btnMOCompilerPath.Text = "Browse...";
			this.btnMOCompilerPath.UseVisualStyleBackColor = true;
			this.btnMOCompilerPath.Click += new System.EventHandler(this.btnMOCompilerPath_Click);
			// 
			// tbxMOCompilerPath
			// 
			this.tbxMOCompilerPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.tbxMOCompilerPath.Location = new System.Drawing.Point(6, 37);
			this.tbxMOCompilerPath.Name = "tbxMOCompilerPath";
			this.tbxMOCompilerPath.Size = new System.Drawing.Size(375, 20);
			this.tbxMOCompilerPath.TabIndex = 1;
			// 
			// label20
			// 
			this.label20.AutoSize = true;
			this.label20.Location = new System.Drawing.Point(6, 20);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(87, 13);
			this.label20.TabIndex = 0;
			this.label20.Text = "Compiler location";
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.label16);
			this.groupBox2.Controls.Add(this.tbxViewerParams);
			this.groupBox2.Controls.Add(this.label14);
			this.groupBox2.Controls.Add(this.label15);
			this.groupBox2.Controls.Add(this.btnViewerPath);
			this.groupBox2.Controls.Add(this.tbxViewerPath);
			this.groupBox2.Controls.Add(this.label13);
			this.groupBox2.Location = new System.Drawing.Point(10, 10);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(467, 145);
			this.groupBox2.TabIndex = 0;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Source viewer";
			// 
			// label16
			// 
			this.label16.AutoSize = true;
			this.label16.ForeColor = System.Drawing.SystemColors.GrayText;
			this.label16.Location = new System.Drawing.Point(6, 116);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(344, 13);
			this.label16.TabIndex = 6;
			this.label16.Text = "Example for Notepad++: -n%line% \"%file%\"  - For gedit:  +%line% \"%file%";
			this.label16.UseMnemonic = false;
			// 
			// tbxViewerParams
			// 
			this.tbxViewerParams.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.tbxViewerParams.Location = new System.Drawing.Point(6, 80);
			this.tbxViewerParams.Name = "tbxViewerParams";
			this.tbxViewerParams.Size = new System.Drawing.Size(453, 20);
			this.tbxViewerParams.TabIndex = 4;
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(6, 64);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(94, 13);
			this.label14.TabIndex = 3;
			this.label14.Text = "Viewer parameters";
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.ForeColor = System.Drawing.SystemColors.GrayText;
			this.label15.Location = new System.Drawing.Point(6, 103);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(408, 13);
			this.label15.TabIndex = 5;
			this.label15.Text = "%file% will be replaced with the file name, %line% will be replaced with the line" +
				 " number";
			this.label15.UseMnemonic = false;
			// 
			// btnViewerPath
			// 
			this.btnViewerPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnViewerPath.Location = new System.Drawing.Point(383, 36);
			this.btnViewerPath.Name = "btnViewerPath";
			this.btnViewerPath.Size = new System.Drawing.Size(75, 23);
			this.btnViewerPath.TabIndex = 2;
			this.btnViewerPath.Text = "Browse...";
			this.btnViewerPath.UseVisualStyleBackColor = true;
			this.btnViewerPath.Click += new System.EventHandler(this.btnViewerPath_Click);
			// 
			// tbxViewerPath
			// 
			this.tbxViewerPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.tbxViewerPath.Location = new System.Drawing.Point(6, 37);
			this.tbxViewerPath.Name = "tbxViewerPath";
			this.tbxViewerPath.Size = new System.Drawing.Size(374, 20);
			this.tbxViewerPath.TabIndex = 1;
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(6, 20);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(79, 13);
			this.label13.TabIndex = 0;
			this.label13.Text = "Viewer location";
			// 
			// ofdViewer
			// 
			this.ofdViewer.Filter = "Executable files|*.exe|All files|*.*";
			this.ofdViewer.Title = "Select the viewer program";
			// 
			// ofdMOCompiler
			// 
			this.ofdMOCompiler.Filter = "Executable files|*.exe|All files|*.*";
			this.ofdMOCompiler.Title = "Select the .mo compiler program";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.tlpColors);
			this.groupBox3.Controls.Add(this.lnkDefaultColors);
			this.groupBox3.Location = new System.Drawing.Point(14, 21);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(454, 128);
			this.groupBox3.TabIndex = 0;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Colors";
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.tlpFonts);
			this.groupBox4.Location = new System.Drawing.Point(14, 172);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(454, 101);
			this.groupBox4.TabIndex = 1;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Fonts";
			// 
			// frmOptions
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnKo;
			this.ClientSize = new System.Drawing.Size(515, 420);
			this.Controls.Add(this.tcTabs);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.btnApply);
			this.Controls.Add(this.btnKo);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmOptions";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Options";
			this.tlpColors.ResumeLayout(false);
			this.tlpColors.PerformLayout();
			this.tcTabs.ResumeLayout(false);
			this.tpGeneral.ResumeLayout(false);
			this.tpGeneral.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudMaxSearchResults)).EndInit();
			this.tpAppearance.ResumeLayout(false);
			this.tlpFonts.ResumeLayout(false);
			this.tlpFonts.PerformLayout();
			this.tpPaths.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnKo;
		private System.Windows.Forms.Button btnApply;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.TableLayoutPanel tlpColors;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.LinkLabel lnkDefaultColors;
		private System.Windows.Forms.ColorDialog cdPick;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.LinkLabel lnkFontSource;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.LinkLabel lnkFontTranslation;
		private System.Windows.Forms.FontDialog fdPick;
		private System.Windows.Forms.TabControl tcTabs;
		private System.Windows.Forms.TabPage tpGeneral;
		private System.Windows.Forms.TabPage tpAppearance;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Button btnViewerPath;
		private System.Windows.Forms.TextBox tbxViewerPath;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.TextBox tbxViewerParams;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.OpenFileDialog ofdViewer;
		private System.Windows.Forms.TableLayoutPanel tlpFonts;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.TextBox tbxMOCompilerParams;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.Button btnMOCompilerPath;
		private System.Windows.Forms.TextBox tbxMOCompilerPath;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.OpenFileDialog ofdMOCompiler;
		private System.Windows.Forms.TabPage tpPaths;
		private System.Windows.Forms.CheckBox chkCompileOnSave;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.NumericUpDown nudMaxSearchResults;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.GroupBox groupBox4;
	}
}