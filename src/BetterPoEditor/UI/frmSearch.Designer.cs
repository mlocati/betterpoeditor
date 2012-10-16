namespace BePoE.UI
{
	partial class frmSearch
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
			this.tcPages = new System.Windows.Forms.TabControl();
			this.tpStandard = new System.Windows.Forms.TabPage();
			this.chkStd_RegEx = new System.Windows.Forms.CheckBox();
			this.btnStd_Go = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.chkStd_WReference = new System.Windows.Forms.CheckBox();
			this.chkStd_WContext = new System.Windows.Forms.CheckBox();
			this.chkStd_WSourceComment = new System.Windows.Forms.CheckBox();
			this.chkStd_WTranslatorComment = new System.Windows.Forms.CheckBox();
			this.chkStd_WSource = new System.Windows.Forms.CheckBox();
			this.chkStd_WTranslated = new System.Windows.Forms.CheckBox();
			this.chkStd_WholeWords = new System.Windows.Forms.CheckBox();
			this.chkStd_Case = new System.Windows.Forms.CheckBox();
			this.tbxStd_Term = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.tpTidy = new System.Windows.Forms.TabPage();
			this.chkTidy_Unescape = new System.Windows.Forms.CheckBox();
			this.btnTidy_Go = new System.Windows.Forms.Button();
			this.chkTidy_Errs = new System.Windows.Forms.CheckBox();
			this.chkTidy_Warns = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.cbxTidy_Where = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.tpSpellCheck = new System.Windows.Forms.TabPage();
			this.chkSpellCheck_IgnoreWordsWithDigits = new System.Windows.Forms.CheckBox();
			this.chkSpellCheck_IgnoreHtml = new System.Windows.Forms.CheckBox();
			this.chkSpellCheck_IgnoreAllCapsWords = new System.Windows.Forms.CheckBox();
			this.btnSpellCheck_Go = new System.Windows.Forms.Button();
			this.tpX = new System.Windows.Forms.TabPage();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.btnNumOccur_Go = new System.Windows.Forms.Button();
			this.tbxNumOccur_Text = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label4 = new System.Windows.Forms.Label();
			this.chkSame_Case = new System.Windows.Forms.CheckBox();
			this.btnSame_Go = new System.Windows.Forms.Button();
			this.gbxSearchResults = new System.Windows.Forms.GroupBox();
			this.scFoundItems = new System.Windows.Forms.SplitContainer();
			this.lbxFound = new System.Windows.Forms.ListBox();
			this.ssSarch = new System.Windows.Forms.StatusStrip();
			this.sslSearch = new System.Windows.Forms.ToolStripStatusLabel();
			this.sslStop = new System.Windows.Forms.ToolStripStatusLabel();
			this.sspSearch = new System.Windows.Forms.ToolStripProgressBar();
			this.tbxFoundDetail = new System.Windows.Forms.TextBox();
			this.tcPages.SuspendLayout();
			this.tpStandard.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.tpTidy.SuspendLayout();
			this.tpSpellCheck.SuspendLayout();
			this.tpX.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.gbxSearchResults.SuspendLayout();
			this.scFoundItems.Panel1.SuspendLayout();
			this.scFoundItems.Panel2.SuspendLayout();
			this.scFoundItems.SuspendLayout();
			this.ssSarch.SuspendLayout();
			this.SuspendLayout();
			// 
			// tcPages
			// 
			this.tcPages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
							| System.Windows.Forms.AnchorStyles.Left)));
			this.tcPages.Controls.Add(this.tpStandard);
			this.tcPages.Controls.Add(this.tpTidy);
			this.tcPages.Controls.Add(this.tpSpellCheck);
			this.tcPages.Controls.Add(this.tpX);
			this.tcPages.Location = new System.Drawing.Point(12, 12);
			this.tcPages.Name = "tcPages";
			this.tcPages.SelectedIndex = 0;
			this.tcPages.Size = new System.Drawing.Size(275, 242);
			this.tcPages.TabIndex = 0;
			// 
			// tpStandard
			// 
			this.tpStandard.Controls.Add(this.chkStd_RegEx);
			this.tpStandard.Controls.Add(this.btnStd_Go);
			this.tpStandard.Controls.Add(this.groupBox1);
			this.tpStandard.Controls.Add(this.chkStd_WholeWords);
			this.tpStandard.Controls.Add(this.chkStd_Case);
			this.tpStandard.Controls.Add(this.tbxStd_Term);
			this.tpStandard.Controls.Add(this.label1);
			this.tpStandard.Location = new System.Drawing.Point(4, 22);
			this.tpStandard.Name = "tpStandard";
			this.tpStandard.Padding = new System.Windows.Forms.Padding(3);
			this.tpStandard.Size = new System.Drawing.Size(267, 216);
			this.tpStandard.TabIndex = 0;
			this.tpStandard.Text = "Standard";
			this.tpStandard.UseVisualStyleBackColor = true;
			// 
			// chkStd_RegEx
			// 
			this.chkStd_RegEx.AutoSize = true;
			this.chkStd_RegEx.Location = new System.Drawing.Point(209, 54);
			this.chkStd_RegEx.Name = "chkStd_RegEx";
			this.chkStd_RegEx.Size = new System.Drawing.Size(52, 17);
			this.chkStd_RegEx.TabIndex = 4;
			this.chkStd_RegEx.Text = "regex";
			this.chkStd_RegEx.UseVisualStyleBackColor = true;
			// 
			// btnStd_Go
			// 
			this.btnStd_Go.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.btnStd_Go.Location = new System.Drawing.Point(96, 181);
			this.btnStd_Go.Name = "btnStd_Go";
			this.btnStd_Go.Size = new System.Drawing.Size(75, 23);
			this.btnStd_Go.TabIndex = 6;
			this.btnStd_Go.Text = "Search";
			this.btnStd_Go.UseVisualStyleBackColor = true;
			this.btnStd_Go.Click += new System.EventHandler(this.btnStd_Go_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.groupBox1.Controls.Add(this.chkStd_WReference);
			this.groupBox1.Controls.Add(this.chkStd_WContext);
			this.groupBox1.Controls.Add(this.chkStd_WSourceComment);
			this.groupBox1.Controls.Add(this.chkStd_WTranslatorComment);
			this.groupBox1.Controls.Add(this.chkStd_WSource);
			this.groupBox1.Controls.Add(this.chkStd_WTranslated);
			this.groupBox1.Location = new System.Drawing.Point(16, 77);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(234, 90);
			this.groupBox1.TabIndex = 5;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Search scope";
			// 
			// chkStd_WReference
			// 
			this.chkStd_WReference.AutoSize = true;
			this.chkStd_WReference.Location = new System.Drawing.Point(116, 65);
			this.chkStd_WReference.Name = "chkStd_WReference";
			this.chkStd_WReference.Size = new System.Drawing.Size(71, 17);
			this.chkStd_WReference.TabIndex = 5;
			this.chkStd_WReference.Text = "reference";
			this.chkStd_WReference.UseVisualStyleBackColor = true;
			// 
			// chkStd_WContext
			// 
			this.chkStd_WContext.AutoSize = true;
			this.chkStd_WContext.Location = new System.Drawing.Point(6, 65);
			this.chkStd_WContext.Name = "chkStd_WContext";
			this.chkStd_WContext.Size = new System.Drawing.Size(61, 17);
			this.chkStd_WContext.TabIndex = 4;
			this.chkStd_WContext.Text = "context";
			this.chkStd_WContext.UseVisualStyleBackColor = true;
			// 
			// chkStd_WSourceComment
			// 
			this.chkStd_WSourceComment.AutoSize = true;
			this.chkStd_WSourceComment.Location = new System.Drawing.Point(6, 42);
			this.chkStd_WSourceComment.Name = "chkStd_WSourceComment";
			this.chkStd_WSourceComment.Size = new System.Drawing.Size(104, 17);
			this.chkStd_WSourceComment.TabIndex = 2;
			this.chkStd_WSourceComment.Text = "source comment";
			this.chkStd_WSourceComment.UseVisualStyleBackColor = true;
			// 
			// chkStd_WTranslatorComment
			// 
			this.chkStd_WTranslatorComment.AutoSize = true;
			this.chkStd_WTranslatorComment.Location = new System.Drawing.Point(116, 42);
			this.chkStd_WTranslatorComment.Name = "chkStd_WTranslatorComment";
			this.chkStd_WTranslatorComment.Size = new System.Drawing.Size(115, 17);
			this.chkStd_WTranslatorComment.TabIndex = 3;
			this.chkStd_WTranslatorComment.Text = "translator comment";
			this.chkStd_WTranslatorComment.UseVisualStyleBackColor = true;
			// 
			// chkStd_WSource
			// 
			this.chkStd_WSource.AutoSize = true;
			this.chkStd_WSource.Checked = true;
			this.chkStd_WSource.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkStd_WSource.Location = new System.Drawing.Point(6, 19);
			this.chkStd_WSource.Name = "chkStd_WSource";
			this.chkStd_WSource.Size = new System.Drawing.Size(78, 17);
			this.chkStd_WSource.TabIndex = 0;
			this.chkStd_WSource.Text = "source text";
			this.chkStd_WSource.UseVisualStyleBackColor = true;
			// 
			// chkStd_WTranslated
			// 
			this.chkStd_WTranslated.AutoSize = true;
			this.chkStd_WTranslated.Checked = true;
			this.chkStd_WTranslated.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkStd_WTranslated.Location = new System.Drawing.Point(116, 19);
			this.chkStd_WTranslated.Name = "chkStd_WTranslated";
			this.chkStd_WTranslated.Size = new System.Drawing.Size(92, 17);
			this.chkStd_WTranslated.TabIndex = 1;
			this.chkStd_WTranslated.Text = "translated text";
			this.chkStd_WTranslated.UseVisualStyleBackColor = true;
			// 
			// chkStd_WholeWords
			// 
			this.chkStd_WholeWords.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.chkStd_WholeWords.AutoSize = true;
			this.chkStd_WholeWords.Location = new System.Drawing.Point(113, 54);
			this.chkStd_WholeWords.Name = "chkStd_WholeWords";
			this.chkStd_WholeWords.Size = new System.Drawing.Size(85, 17);
			this.chkStd_WholeWords.TabIndex = 3;
			this.chkStd_WholeWords.Text = "whole words";
			this.chkStd_WholeWords.UseVisualStyleBackColor = true;
			// 
			// chkStd_Case
			// 
			this.chkStd_Case.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.chkStd_Case.AutoSize = true;
			this.chkStd_Case.Location = new System.Drawing.Point(9, 54);
			this.chkStd_Case.Name = "chkStd_Case";
			this.chkStd_Case.Size = new System.Drawing.Size(93, 17);
			this.chkStd_Case.TabIndex = 2;
			this.chkStd_Case.Text = "case sensitive";
			this.chkStd_Case.UseVisualStyleBackColor = true;
			// 
			// tbxStd_Term
			// 
			this.tbxStd_Term.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.tbxStd_Term.Location = new System.Drawing.Point(9, 28);
			this.tbxStd_Term.Name = "tbxStd_Term";
			this.tbxStd_Term.Size = new System.Drawing.Size(252, 20);
			this.tbxStd_Term.TabIndex = 1;
			this.tbxStd_Term.Leave += new System.EventHandler(this.tbxStd_Term_Leave);
			this.tbxStd_Term.Enter += new System.EventHandler(this.tbxStd_Term_Enter);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(69, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Search terms";
			// 
			// tpTidy
			// 
			this.tpTidy.Controls.Add(this.chkTidy_Unescape);
			this.tpTidy.Controls.Add(this.btnTidy_Go);
			this.tpTidy.Controls.Add(this.chkTidy_Errs);
			this.tpTidy.Controls.Add(this.chkTidy_Warns);
			this.tpTidy.Controls.Add(this.label3);
			this.tpTidy.Controls.Add(this.cbxTidy_Where);
			this.tpTidy.Controls.Add(this.label2);
			this.tpTidy.Location = new System.Drawing.Point(4, 22);
			this.tpTidy.Name = "tpTidy";
			this.tpTidy.Padding = new System.Windows.Forms.Padding(3);
			this.tpTidy.Size = new System.Drawing.Size(267, 216);
			this.tpTidy.TabIndex = 2;
			this.tpTidy.Text = "Verify html";
			this.tpTidy.UseVisualStyleBackColor = true;
			// 
			// chkTidy_Unescape
			// 
			this.chkTidy_Unescape.AutoSize = true;
			this.chkTidy_Unescape.Checked = true;
			this.chkTidy_Unescape.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkTidy_Unescape.Location = new System.Drawing.Point(53, 122);
			this.chkTidy_Unescape.Name = "chkTidy_Unescape";
			this.chkTidy_Unescape.Size = new System.Drawing.Size(166, 17);
			this.chkTidy_Unescape.TabIndex = 5;
			this.chkTidy_Unescape.Text = "unescape strings (eg \\\" => \" )";
			this.chkTidy_Unescape.UseVisualStyleBackColor = true;
			// 
			// btnTidy_Go
			// 
			this.btnTidy_Go.Location = new System.Drawing.Point(99, 149);
			this.btnTidy_Go.Name = "btnTidy_Go";
			this.btnTidy_Go.Size = new System.Drawing.Size(75, 23);
			this.btnTidy_Go.TabIndex = 6;
			this.btnTidy_Go.Text = "Verify html!";
			this.btnTidy_Go.UseVisualStyleBackColor = true;
			this.btnTidy_Go.Click += new System.EventHandler(this.btnTidy_Go_Click);
			// 
			// chkTidy_Errs
			// 
			this.chkTidy_Errs.AutoSize = true;
			this.chkTidy_Errs.Checked = true;
			this.chkTidy_Errs.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkTidy_Errs.Location = new System.Drawing.Point(112, 76);
			this.chkTidy_Errs.Name = "chkTidy_Errs";
			this.chkTidy_Errs.Size = new System.Drawing.Size(52, 17);
			this.chkTidy_Errs.TabIndex = 3;
			this.chkTidy_Errs.Text = "errors";
			this.chkTidy_Errs.UseVisualStyleBackColor = true;
			// 
			// chkTidy_Warns
			// 
			this.chkTidy_Warns.AutoSize = true;
			this.chkTidy_Warns.Location = new System.Drawing.Point(112, 99);
			this.chkTidy_Warns.Name = "chkTidy_Warns";
			this.chkTidy_Warns.Size = new System.Drawing.Size(68, 17);
			this.chkTidy_Warns.TabIndex = 4;
			this.chkTidy_Warns.Text = "warnings";
			this.chkTidy_Warns.UseVisualStyleBackColor = true;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(9, 77);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(97, 13);
			this.label3.TabIndex = 2;
			this.label3.Text = "Messages to report";
			// 
			// cbxTidy_Where
			// 
			this.cbxTidy_Where.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.cbxTidy_Where.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxTidy_Where.FormattingEnabled = true;
			this.cbxTidy_Where.Location = new System.Drawing.Point(112, 45);
			this.cbxTidy_Where.Name = "cbxTidy_Where";
			this.cbxTidy_Where.Size = new System.Drawing.Size(146, 21);
			this.cbxTidy_Where.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(9, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(68, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Text to verify";
			// 
			// tpSpellCheck
			// 
			this.tpSpellCheck.Controls.Add(this.chkSpellCheck_IgnoreWordsWithDigits);
			this.tpSpellCheck.Controls.Add(this.chkSpellCheck_IgnoreHtml);
			this.tpSpellCheck.Controls.Add(this.chkSpellCheck_IgnoreAllCapsWords);
			this.tpSpellCheck.Controls.Add(this.btnSpellCheck_Go);
			this.tpSpellCheck.Location = new System.Drawing.Point(4, 22);
			this.tpSpellCheck.Name = "tpSpellCheck";
			this.tpSpellCheck.Padding = new System.Windows.Forms.Padding(3);
			this.tpSpellCheck.Size = new System.Drawing.Size(267, 216);
			this.tpSpellCheck.TabIndex = 3;
			this.tpSpellCheck.Text = "Spell check";
			this.tpSpellCheck.UseVisualStyleBackColor = true;
			// 
			// chkSpellCheck_IgnoreWordsWithDigits
			// 
			this.chkSpellCheck_IgnoreWordsWithDigits.AutoSize = true;
			this.chkSpellCheck_IgnoreWordsWithDigits.Location = new System.Drawing.Point(66, 102);
			this.chkSpellCheck_IgnoreWordsWithDigits.Name = "chkSpellCheck_IgnoreWordsWithDigits";
			this.chkSpellCheck_IgnoreWordsWithDigits.Size = new System.Drawing.Size(135, 17);
			this.chkSpellCheck_IgnoreWordsWithDigits.TabIndex = 2;
			this.chkSpellCheck_IgnoreWordsWithDigits.Text = "ignore words with digits";
			this.chkSpellCheck_IgnoreWordsWithDigits.UseVisualStyleBackColor = true;
			// 
			// chkSpellCheck_IgnoreHtml
			// 
			this.chkSpellCheck_IgnoreHtml.AutoSize = true;
			this.chkSpellCheck_IgnoreHtml.Location = new System.Drawing.Point(66, 79);
			this.chkSpellCheck_IgnoreHtml.Name = "chkSpellCheck_IgnoreHtml";
			this.chkSpellCheck_IgnoreHtml.Size = new System.Drawing.Size(77, 17);
			this.chkSpellCheck_IgnoreHtml.TabIndex = 1;
			this.chkSpellCheck_IgnoreHtml.Text = "ignore html";
			this.chkSpellCheck_IgnoreHtml.UseVisualStyleBackColor = true;
			// 
			// chkSpellCheck_IgnoreAllCapsWords
			// 
			this.chkSpellCheck_IgnoreAllCapsWords.AutoSize = true;
			this.chkSpellCheck_IgnoreAllCapsWords.Location = new System.Drawing.Point(66, 56);
			this.chkSpellCheck_IgnoreAllCapsWords.Name = "chkSpellCheck_IgnoreAllCapsWords";
			this.chkSpellCheck_IgnoreAllCapsWords.Size = new System.Drawing.Size(125, 17);
			this.chkSpellCheck_IgnoreAllCapsWords.TabIndex = 0;
			this.chkSpellCheck_IgnoreAllCapsWords.Text = "ignore all-caps words";
			this.chkSpellCheck_IgnoreAllCapsWords.UseVisualStyleBackColor = true;
			// 
			// btnSpellCheck_Go
			// 
			this.btnSpellCheck_Go.Location = new System.Drawing.Point(96, 138);
			this.btnSpellCheck_Go.Name = "btnSpellCheck_Go";
			this.btnSpellCheck_Go.Size = new System.Drawing.Size(75, 23);
			this.btnSpellCheck_Go.TabIndex = 3;
			this.btnSpellCheck_Go.Text = "Spell check";
			this.btnSpellCheck_Go.UseVisualStyleBackColor = true;
			this.btnSpellCheck_Go.Click += new System.EventHandler(this.btnSpellCheck_Go_Click);
			// 
			// tpX
			// 
			this.tpX.Controls.Add(this.groupBox3);
			this.tpX.Controls.Add(this.groupBox2);
			this.tpX.Location = new System.Drawing.Point(4, 22);
			this.tpX.Name = "tpX";
			this.tpX.Padding = new System.Windows.Forms.Padding(3);
			this.tpX.Size = new System.Drawing.Size(267, 216);
			this.tpX.TabIndex = 1;
			this.tpX.Text = "Special";
			this.tpX.UseVisualStyleBackColor = true;
			// 
			// groupBox3
			// 
			this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox3.Controls.Add(this.btnNumOccur_Go);
			this.groupBox3.Controls.Add(this.tbxNumOccur_Text);
			this.groupBox3.Controls.Add(this.label6);
			this.groupBox3.Controls.Add(this.label5);
			this.groupBox3.Location = new System.Drawing.Point(6, 92);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(255, 88);
			this.groupBox3.TabIndex = 1;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Different occurrencies";
			// 
			// btnNumOccur_Go
			// 
			this.btnNumOccur_Go.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnNumOccur_Go.Location = new System.Drawing.Point(174, 59);
			this.btnNumOccur_Go.Name = "btnNumOccur_Go";
			this.btnNumOccur_Go.Size = new System.Drawing.Size(75, 23);
			this.btnNumOccur_Go.TabIndex = 3;
			this.btnNumOccur_Go.Text = "Search";
			this.btnNumOccur_Go.UseVisualStyleBackColor = true;
			this.btnNumOccur_Go.Click += new System.EventHandler(this.btnNumOccur_Go_Click);
			// 
			// tbxNumOccur_Text
			// 
			this.tbxNumOccur_Text.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.tbxNumOccur_Text.Location = new System.Drawing.Point(40, 60);
			this.tbxNumOccur_Text.Name = "tbxNumOccur_Text";
			this.tbxNumOccur_Text.Size = new System.Drawing.Size(128, 20);
			this.tbxNumOccur_Text.TabIndex = 2;
			this.tbxNumOccur_Text.Text = "<";
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(6, 64);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(28, 13);
			this.label6.TabIndex = 1;
			this.label6.Text = "Text";
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.label5.Location = new System.Drawing.Point(6, 16);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(243, 40);
			this.label5.TabIndex = 0;
			this.label5.Text = "Search entries having a different number of occurrencies of a specified text in t" +
				 "he source and translated texts";
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.chkSame_Case);
			this.groupBox2.Controls.Add(this.btnSame_Go);
			this.groupBox2.Location = new System.Drawing.Point(6, 6);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(255, 80);
			this.groupBox2.TabIndex = 0;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Source text == translated text";
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.label4.Location = new System.Drawing.Point(6, 16);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(243, 30);
			this.label4.TabIndex = 0;
			this.label4.Text = "Search entries having the same source and translated texts";
			// 
			// chkSame_Case
			// 
			this.chkSame_Case.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.chkSame_Case.AutoSize = true;
			this.chkSame_Case.Location = new System.Drawing.Point(9, 53);
			this.chkSame_Case.Name = "chkSame_Case";
			this.chkSame_Case.Size = new System.Drawing.Size(93, 17);
			this.chkSame_Case.TabIndex = 1;
			this.chkSame_Case.Text = "case sensitive";
			this.chkSame_Case.UseVisualStyleBackColor = true;
			// 
			// btnSame_Go
			// 
			this.btnSame_Go.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSame_Go.Location = new System.Drawing.Point(174, 49);
			this.btnSame_Go.Name = "btnSame_Go";
			this.btnSame_Go.Size = new System.Drawing.Size(75, 23);
			this.btnSame_Go.TabIndex = 2;
			this.btnSame_Go.Text = "Search";
			this.btnSame_Go.UseVisualStyleBackColor = true;
			this.btnSame_Go.Click += new System.EventHandler(this.btnSame_Go_Click);
			// 
			// gbxSearchResults
			// 
			this.gbxSearchResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
							| System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.gbxSearchResults.Controls.Add(this.scFoundItems);
			this.gbxSearchResults.Location = new System.Drawing.Point(300, 12);
			this.gbxSearchResults.Name = "gbxSearchResults";
			this.gbxSearchResults.Size = new System.Drawing.Size(165, 242);
			this.gbxSearchResults.TabIndex = 1;
			this.gbxSearchResults.TabStop = false;
			this.gbxSearchResults.Text = "Search results";
			// 
			// scFoundItems
			// 
			this.scFoundItems.Dock = System.Windows.Forms.DockStyle.Fill;
			this.scFoundItems.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.scFoundItems.Location = new System.Drawing.Point(3, 16);
			this.scFoundItems.Name = "scFoundItems";
			this.scFoundItems.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// scFoundItems.Panel1
			// 
			this.scFoundItems.Panel1.Controls.Add(this.lbxFound);
			this.scFoundItems.Panel1.Controls.Add(this.ssSarch);
			// 
			// scFoundItems.Panel2
			// 
			this.scFoundItems.Panel2.Controls.Add(this.tbxFoundDetail);
			this.scFoundItems.Size = new System.Drawing.Size(159, 223);
			this.scFoundItems.SplitterDistance = 149;
			this.scFoundItems.TabIndex = 2;
			// 
			// lbxFound
			// 
			this.lbxFound.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lbxFound.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbxFound.FormattingEnabled = true;
			this.lbxFound.IntegralHeight = false;
			this.lbxFound.Location = new System.Drawing.Point(0, 0);
			this.lbxFound.Name = "lbxFound";
			this.lbxFound.Size = new System.Drawing.Size(159, 149);
			this.lbxFound.TabIndex = 0;
			this.lbxFound.SelectedIndexChanged += new System.EventHandler(this.lbxFound_SelectedIndexChanged);
			// 
			// ssSarch
			// 
			this.ssSarch.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sslSearch,
            this.sslStop,
            this.sspSearch});
			this.ssSarch.Location = new System.Drawing.Point(0, 127);
			this.ssSarch.Name = "ssSarch";
			this.ssSarch.Size = new System.Drawing.Size(159, 22);
			this.ssSarch.SizingGrip = false;
			this.ssSarch.TabIndex = 1;
			this.ssSarch.Visible = false;
			// 
			// sslSearch
			// 
			this.sslSearch.Name = "sslSearch";
			this.sslSearch.Size = new System.Drawing.Size(42, 17);
			this.sslSearch.Spring = true;
			this.sslSearch.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// sslStop
			// 
			this.sslStop.IsLink = true;
			this.sslStop.Name = "sslStop";
			this.sslStop.Size = new System.Drawing.Size(28, 17);
			this.sslStop.Text = "stop";
			this.sslStop.Visible = false;
			this.sslStop.Click += new System.EventHandler(this.sslStop_Click);
			// 
			// sspSearch
			// 
			this.sspSearch.Name = "sspSearch";
			this.sspSearch.Size = new System.Drawing.Size(100, 16);
			this.sspSearch.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.sspSearch.ToolTipText = "Working...";
			// 
			// tbxFoundDetail
			// 
			this.tbxFoundDetail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbxFoundDetail.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbxFoundDetail.Location = new System.Drawing.Point(0, 0);
			this.tbxFoundDetail.Multiline = true;
			this.tbxFoundDetail.Name = "tbxFoundDetail";
			this.tbxFoundDetail.ReadOnly = true;
			this.tbxFoundDetail.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.tbxFoundDetail.Size = new System.Drawing.Size(159, 70);
			this.tbxFoundDetail.TabIndex = 0;
			// 
			// frmSearch
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(477, 263);
			this.Controls.Add(this.gbxSearchResults);
			this.Controls.Add(this.tcPages);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(446, 266);
			this.Name = "frmSearch";
			this.ShowInTaskbar = false;
			this.Text = "Search";
			this.Shown += new System.EventHandler(this.frmSearch_Shown);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSearch_FormClosing);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmSearch_KeyDown);
			this.tcPages.ResumeLayout(false);
			this.tpStandard.ResumeLayout(false);
			this.tpStandard.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.tpTidy.ResumeLayout(false);
			this.tpTidy.PerformLayout();
			this.tpSpellCheck.ResumeLayout(false);
			this.tpSpellCheck.PerformLayout();
			this.tpX.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.gbxSearchResults.ResumeLayout(false);
			this.scFoundItems.Panel1.ResumeLayout(false);
			this.scFoundItems.Panel1.PerformLayout();
			this.scFoundItems.Panel2.ResumeLayout(false);
			this.scFoundItems.Panel2.PerformLayout();
			this.scFoundItems.ResumeLayout(false);
			this.ssSarch.ResumeLayout(false);
			this.ssSarch.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tcPages;
		private System.Windows.Forms.TabPage tpStandard;
		private System.Windows.Forms.TabPage tpX;
		private System.Windows.Forms.CheckBox chkStd_WTranslated;
		private System.Windows.Forms.CheckBox chkStd_WSource;
		private System.Windows.Forms.CheckBox chkStd_WholeWords;
		private System.Windows.Forms.CheckBox chkStd_Case;
		private System.Windows.Forms.TextBox tbxStd_Term;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox chkStd_WSourceComment;
		private System.Windows.Forms.CheckBox chkStd_WTranslatorComment;
		private System.Windows.Forms.CheckBox chkStd_WContext;
		private System.Windows.Forms.CheckBox chkStd_WReference;
		private System.Windows.Forms.Button btnStd_Go;
		private System.Windows.Forms.Button btnSame_Go;
		private System.Windows.Forms.GroupBox gbxSearchResults;
		private System.Windows.Forms.ListBox lbxFound;
		private System.Windows.Forms.StatusStrip ssSarch;
		private System.Windows.Forms.ToolStripStatusLabel sslSearch;
		private System.Windows.Forms.ToolStripProgressBar sspSearch;
		private System.Windows.Forms.CheckBox chkSame_Case;
		private System.Windows.Forms.CheckBox chkStd_RegEx;
		private System.Windows.Forms.SplitContainer scFoundItems;
		private System.Windows.Forms.TextBox tbxFoundDetail;
		private System.Windows.Forms.TabPage tpTidy;
		private System.Windows.Forms.ComboBox cbxTidy_Where;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnTidy_Go;
		private System.Windows.Forms.CheckBox chkTidy_Errs;
		private System.Windows.Forms.CheckBox chkTidy_Warns;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.CheckBox chkTidy_Unescape;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox tbxNumOccur_Text;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button btnNumOccur_Go;
		private System.Windows.Forms.Button btnSpellCheck_Go;
		private System.Windows.Forms.TabPage tpSpellCheck;
		private System.Windows.Forms.CheckBox chkSpellCheck_IgnoreWordsWithDigits;
		private System.Windows.Forms.CheckBox chkSpellCheck_IgnoreHtml;
		private System.Windows.Forms.CheckBox chkSpellCheck_IgnoreAllCapsWords;
		private System.Windows.Forms.ToolStripStatusLabel sslStop;
	}
}