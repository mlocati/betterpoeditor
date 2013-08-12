using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace BePoE.UI
{
	public partial class frmMain : Form, frmSearch.ISearchConnection, frmEntryMessages.IEntryMessageResultsConnection
	{
		#region Properties
		const string KEY_OPEN = "Open";
		const string KEY_SAVE = "Save";
		const string KEY_PREV_ITEM = "Previous item";
		const string KEY_NEXT_ITEM = "Next item";
		const string KEY_SEARCH = "Search";

		public class AcceletatorKeyInfoAttribute : DescriptionAttribute
		{
			[ReadOnly(true)]
			public readonly Keys Modifiers;
			public readonly Keys Key;
			public AcceletatorKeyInfoAttribute(string description, Keys modifiers, Keys key)
				: base(description)
			{
				this.Modifiers = modifiers;
				this.Key = key;
			}
		}
		private enum AcceletatorKey
		{
			[AcceletatorKeyInfo(KEY_OPEN, Keys.Control, Keys.O)]
			Open,
			[AcceletatorKeyInfo(KEY_SAVE, Keys.Control, Keys.S)]
			Save,
			[AcceletatorKeyInfo(KEY_PREV_ITEM, Keys.Control, Keys.Up)]
			PrevItem,
			[AcceletatorKeyInfo(KEY_NEXT_ITEM, Keys.Control, Keys.Down)]
			NextItem,
			[AcceletatorKeyInfo(KEY_SEARCH, Keys.Control, Keys.F)]
			Search,
			[AcceletatorKeyInfo(KEY_SEARCH, Keys.None, Keys.F3)]
			SearchAlt,
		}
		private Dictionary<AcceletatorKey, AcceletatorKeyInfoAttribute> _acceletatorKeys = null;
		private Dictionary<AcceletatorKey, AcceletatorKeyInfoAttribute> AcceletatorKeys
		{
			get
			{
				if (this._acceletatorKeys == null)
				{
					Type enumType = typeof(AcceletatorKey);
					Type infoType = typeof(AcceletatorKeyInfoAttribute);
					Dictionary<AcceletatorKey, AcceletatorKeyInfoAttribute> acceletatorKeys = new Dictionary<AcceletatorKey, AcceletatorKeyInfoAttribute>();
					foreach (AcceletatorKey k in Enum.GetValues(enumType))
						acceletatorKeys.Add(k, (AcceletatorKeyInfoAttribute)enumType.GetMember(k.ToString())[0].GetCustomAttributes(infoType, false)[0]);
					this._acceletatorKeys = acceletatorKeys;
				}
				return this._acceletatorKeys;
			}
		}

		private string _currentPOReferenceFolder = null;

		private PO.POFile _currentPO = null;
		private PO.POFile CurrentPO
		{
			get
			{ return this._currentPO; }
			set
			{
				if ((value != null) && (value == this._currentPO))
					return;
				this.CurrentEntries = null;
				if (this._currentPO != null)
				{
					try
					{ this._currentPO.DirtyChanged -= this.CurrentPO_DirtyChanged; }
					catch
					{ }
				}
				this._currentPOReferenceFolder = null;
				this._currentPO = value;
				this.tsiCompile.Enabled = this.tsiSave.Enabled = (value != null);
				this.tsiFileFormat.Enabled = (this._currentPO != null);
				this.tsiView_Removed.Enabled = this.tsiView_Translated.Enabled = this.tsiView_Untranslated.Enabled = (this._currentPO != null);
				this.tsiView_Fuzzy.Enabled = this.tsiView_Clear.Enabled = (this._currentPO != null);
				this.scItems.Visible = this._currentPO != null;
				this.ssbProgr.Minimum = 0;
				this.ssbProgr.Value = 0;
				if (this._currentPO == null)
				{
					this.sslStatus.Text = "No file loaded.";
					this.sslProgr.Text = "";
					this.sslProgr.Visible = false;
					this.ssbProgr.Visible = false;
					if (this.tsiSearch.Checked)
						this.tsiSearch.Checked = false;
					this.tsiSearch.Enabled = false;
				}
				else
				{
					this.sslStatus.Text = string.Format("{0} [{1}]", this._currentPO.TextFile.FileName, this._currentPO.Language.DisplayName);
					this.CodePageSet(this._currentPO.TextFile.CodePage, this._currentPO.TextFile.UseBOM);
					this.LineEndingSet(this._currentPO.TextFile.LineEnding);
					this.RefreshViewedItems();
					this.ssbProgr.Maximum = this._currentPO.TotalDataEntries;
					this.RefreshStatus();
					this.sslProgr.Visible = true;
					this.ssbProgr.Visible = true;
					this.UpdateSelectedEntry();
					this._currentPO.DirtyChanged += this.CurrentPO_DirtyChanged;
					this.tsiSearch.Enabled = true;
					if (this._currentPO.TranslatedItems == this._currentPO.TotalDataEntries)
					{
						if (!this.tsiView_Translated.Checked)
						{
							this.tsiView_Translated.Checked = true;
							this.RefreshViewedItems();
						}
					}
				}
				this.UpdateWindowTitle();
			}
		}

		private List<PO.Entry> _currentEntries = null;
		private List<PO.Entry> CurrentEntries
		{
			get
			{ return this._currentEntries; }
			set
			{
				if (value == this._currentEntries)
					return;
				if (this._searchForm != null)
					this._searchForm.ClearAllData();
				this.ClearEntryMessagesResults();
				this.lbxEntries.BeginUpdate();
				PO.Entry previousEntry = this.lbxEntries.SelectedItem as PO.Entry;
				this.lbxEntries.Items.Clear();
				this._currentEntries = value;
				if ((value != null) && (value.Count > 0))
					this.lbxEntries.Items.AddRange(value.ToArray());
				this.lbxEntries.EndUpdate();
				if ((previousEntry != null) && (value != null) && (value.Count > 0) && value.Contains(previousEntry))
					this.lbxEntries.SelectedItem = previousEntry;
				this.UpdateSelectedEntry();
			}
		}

		private enum TranslPlace
		{
			Source,
			Translated,
		}

		#endregion


		#region Constructor

		public frmMain()
		{
			InitializeComponent();
			Utils.Gfx.SetFormIcon(this);
			this.UpdateWindowTitle();
			this.ofdOpen.InitialDirectory = "";
			this._mniFileFormat_Encoding_ANSIs = new Dictionary<int, ToolStripMenuItem>(IO.TextFile.AnsiCodePages.Count);
			List<ToolStripItem> list = new List<ToolStripItem>(IO.TextFile.AnsiCodePages.Count);
			int defaultCP = System.Text.Encoding.Default.CodePage;
			ToolStripMenuItem defaultMNI = null;
			foreach (KeyValuePair<int, string> kv in IO.TextFile.AnsiCodePages)
			{
				ToolStripMenuItem mni = new ToolStripMenuItem(kv.Value);
				mni.Tag = kv.Key;
				mni.Click += new EventHandler(this.mniFileFormat_Encoding_ANSIs_Click);
				mni.DisplayStyle = ToolStripItemDisplayStyle.Text;
				list.Add(mni);
				if (kv.Key == defaultCP)
					defaultMNI = mni;
				this._mniFileFormat_Encoding_ANSIs.Add(kv.Key, mni);
			}
			for (int i = 0; i < (list.Count - 1); i++)
				for (int j = (i + 1); j < list.Count; j++)
					if (string.Compare(list[i].Text, list[j].Text, false) > 0)
					{
						ToolStripItem tmp = list[i];
						list[i] = list[j];
						list[j] = tmp;
					}
			int iDefault = (defaultMNI == null) ? -1 : list.IndexOf(defaultMNI);
			if (iDefault >= 0)
			{
				if (iDefault != 0)
				{
					list.Remove(defaultMNI);
					list.Insert(0, defaultMNI);
				}
				list.Insert(1, new ToolStripSeparator());
			}
			this.mniFileFormat_Encoding_ANSI.DropDownItems.AddRange(list.ToArray());
			this.CurrentPO = null;
			this.tsiOpenHistoryUpdated();
			this.tbxSource0.Font = Program.Vars.SourceFont;
			this.tbxTranslated0.Font = Program.Vars.TranslatedFont;
			this.RefreshSpecialChars();
		}

		#endregion


		#region Loading / saving

		private void LoadPOFile(string fileName, bool fromRecent)
		{
			this.Cursor = Cursors.WaitCursor;
			this.Refresh();
			try
			{
				this.CurrentPO = new PO.POFile(IO.TextFile.Load(fileName, this.AnsiCodePageGetter), this.LanguageGetter);
				try
				{
					Program.Vars.AddLastFile(fileName);
					this.tsiOpenHistoryUpdated();
				}
				catch
				{ }
			}
			catch (OperationCanceledException)
			{ }
			catch (Exception x)
			{
				this.Cursor = Cursors.Default;
				if (fromRecent)
				{
					if (MessageBox.Show(string.Format("{1}{0}{0}Remove the file from the recent files list?", Environment.NewLine, x.Message), Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
					{
						Program.Vars.RemoveLastFile(fileName);
						this.tsiOpenHistoryUpdated();
					}
				}
				else
				{
					MessageBox.Show(x.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void tsiOpen_Click(object sender, EventArgs e)
		{
			this.OpenPO();
		}
		private void OpenPO()
		{
			this.tsiOpen.HideDropDown();
			if (!this.AskDirty())
				return;
			if (string.IsNullOrEmpty(this.ofdOpen.InitialDirectory))
			{
				try
				{
					if (Program.Vars.LastFiles.Length > 0)
					{
						DirectoryInfo di = new FileInfo(Program.Vars.LastFiles[0]).Directory;
						while ((di != null) && (!di.Exists))
							di = di.Parent;
						if (di != null)
							this.ofdOpen.InitialDirectory = di.FullName;
					}
				}
				catch
				{ }
				if (string.IsNullOrEmpty(this.ofdOpen.InitialDirectory))
					this.ofdOpen.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			}
			if (this.ofdOpen.ShowDialog(this) == DialogResult.OK)
			{
				this.LoadPOFile(this.ofdOpen.FileName, false);
			}
		}

		private void tsiOpen_MouseDown(object sender, MouseEventArgs e)
		{
			this.tsiOpen.ShowDropDown();
		}

		private bool AskDirty()
		{
			if ((this.CurrentPO != null) && this.CurrentPO.Dirty)
				if (MessageBox.Show(string.Format("The file {1} has been modified.{0}{0}Do you want to proceed anyway?", Environment.NewLine, Path.GetFileName(this.CurrentPO.TextFile.FileName)), Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
					return false;
			return true;
		}
		private void tsiOpenRecent_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem mni = sender as ToolStripMenuItem;
			if (mni != null)
			{
				string fn = mni.Tag as string;
				if (!string.IsNullOrEmpty(fn))
				{
					if (this.AskDirty())
						this.LoadPOFile(fn, true);
				}
			}
		}

		private int? AnsiCodePageGetter(Dictionary<int, string> availableCodePages, int defaultCodePage, byte[] dataBytes, int[] ansiCharPositions)
		{
			using (frmSelectCodePage frm = new frmSelectCodePage(availableCodePages, defaultCodePage, dataBytes, ansiCharPositions))
			{
				if (frm.ShowDialog(this) == DialogResult.OK)
					return frm.SelectedCodePage;
			}
			return null;
		}

		private CultureInfo LanguageGetter(CultureInfo suggestedCultureInfo)
		{
			using (frmSelectLanguage frm = new frmSelectLanguage(suggestedCultureInfo))
			{
				if (frm.ShowDialog(this) == DialogResult.OK)
					return frm.Result;
				return null;
			}
		}

		private void tsiSave_Click(object sender, EventArgs e)
		{
			this.SavePO();
		}
		private void SavePO()
		{
			if (this.CurrentPO == null)
				return;
			this.Cursor = Cursors.WaitCursor;
			this.Refresh();
			try
			{
				this.CurrentPO.TextFile.Save();
				if (Program.Vars.CompileOnSave)
					this.CompilePO();
			}
			catch (Exception x)
			{
				this.Cursor = Cursors.Default;
				MessageBox.Show(x.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void tsiOpenHistoryUpdated()
		{
			int n = this.tsiOpen.DropDownItems.Count;
			while (n > 0)
			{
				ToolStripItem tsi = this.tsiOpen.DropDownItems[--n];
				try
				{ tsi.Click -= this.tsiOpenRecent_Click; }
				catch
				{ }
				this.tsiOpen.DropDownItems.RemoveAt(n);
				tsi.Dispose();
			}
			string[] fileNames = Program.Vars.LastFiles;
			for (int i = 0; i < fileNames.Length; i++)
			{
				ToolStripMenuItem tsi = new ToolStripMenuItem(string.Format("{0:N0}. {1}", i + 1, fileNames[i]));
				tsi.DisplayStyle = ToolStripItemDisplayStyle.Text;
				tsi.Tag = fileNames[i];
				tsi.Click += tsiOpenRecent_Click;
				this.tsiOpen.DropDownItems.Add(tsi);
			}
		}

		private void CurrentPO_DirtyChanged(PO.POFile file, bool newDirtyValue)
		{
			this.UpdateWindowTitle();
		}

		#endregion


		#region File format

		private Dictionary<int, ToolStripMenuItem> _mniFileFormat_Encoding_ANSIs;

		private void SetLineEnding(IO.LineEndings lineEnding)
		{
			if ((this._currentPO == null) || (this._currentPO.TextFile.LineEnding == lineEnding))
				return;
			try
			{
				this._currentPO.TextFile.LineEnding = lineEnding;
			}
			catch (Exception x)
			{
				while (x.InnerException != null)
					x = x.InnerException;
				MessageBox.Show(x.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			this.LineEndingSet(this._currentPO.TextFile.LineEnding);
		}
		private void LineEndingSet(IO.LineEndings lineEnding)
		{
			this.mniFileFormat_LineEndings_Linux.Checked = (lineEnding == IO.LineEndings.Linux);
			this.mniFileFormat_LineEndings_Windows.Checked = (lineEnding == IO.LineEndings.Windows);
			this.mniFileFormat_LineEndings_Mac.Checked = (lineEnding == IO.LineEndings.Mac);
		}

		private void SetCodePage(int codePage, bool useBOM)
		{
			if ((this._currentPO == null) || ((this._currentPO.TextFile.CodePage == codePage) && (this._currentPO.TextFile.UseBOM == useBOM)))
				return;
			try
			{
				this.CurrentPO.TextFile.SetEncoding(codePage, useBOM);
			}
			catch (Exception x)
			{
				while (x.InnerException != null)
					x = x.InnerException;
				MessageBox.Show(x.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			this.CodePageSet(this._currentPO.TextFile.CodePage, this._currentPO.TextFile.UseBOM);
		}
		private void CodePageSet(int codePage, bool useBOM)
		{
			this.mniFileFormat_Encoding_UTF8_NoBOM.Checked = (codePage == (int)IO.TextFile.SpecialCodePage.Utf8) && (!useBOM);
			this.mniFileFormat_Encoding_UTF8_BOM.Checked = (codePage == (int)IO.TextFile.SpecialCodePage.Utf8) && (useBOM);
			this.mniFileFormat_Encoding_UTF16_BE.Checked = (codePage == (int)IO.TextFile.SpecialCodePage.Utf16_BE);
			this.mniFileFormat_Encoding_UTF16_LE.Checked = (codePage == (int)IO.TextFile.SpecialCodePage.Utf16_LE);
			this.mniFileFormat_Encoding_UTF32_BE.Checked = (codePage == (int)IO.TextFile.SpecialCodePage.Utf32_BE);
			this.mniFileFormat_Encoding_UTF32_LE.Checked = (codePage == (int)IO.TextFile.SpecialCodePage.Utf32_LE);
			bool isAnsi = false;
			foreach (KeyValuePair<int, ToolStripMenuItem> kv in this._mniFileFormat_Encoding_ANSIs)
			{
				bool thisOne = (kv.Key == codePage);
				kv.Value.Checked = thisOne;
				if (thisOne)
					isAnsi = true;
			}
			this.mniFileFormat_Encoding_ANSI.Checked = isAnsi;
		}

		private void mniFileFormat_LineEndings_Linux_Click(object sender, EventArgs e)
		{
			this.SetLineEnding(IO.LineEndings.Linux);
		}
		private void mniFileFormat_LineEndings_Windows_Click(object sender, EventArgs e)
		{
			this.SetLineEnding(IO.LineEndings.Windows);
		}
		private void mniFileFormat_LineEndings_Mac_Click(object sender, EventArgs e)
		{
			this.SetLineEnding(IO.LineEndings.Mac);
		}

		private void mniFileFormat_Encoding_UTF8_NoBOM_Click(object sender, EventArgs e)
		{
			this.SetCodePage((int)IO.TextFile.SpecialCodePage.Utf8, false);
		}
		private void mniFileFormat_Encoding_UTF8_BOM_Click(object sender, EventArgs e)
		{
			this.SetCodePage((int)IO.TextFile.SpecialCodePage.Utf8, true);
		}
		private void mniFileFormat_Encoding_UTF16_BE_Click(object sender, EventArgs e)
		{
			this.SetCodePage((int)IO.TextFile.SpecialCodePage.Utf16_BE, true);
		}
		private void mniFileFormat_Encoding_UTF16_LE_Click(object sender, EventArgs e)
		{
			this.SetCodePage((int)IO.TextFile.SpecialCodePage.Utf16_LE, true);
		}
		private void mniFileFormat_Encoding_UTF32_BE_Click(object sender, EventArgs e)
		{
			this.SetCodePage((int)IO.TextFile.SpecialCodePage.Utf32_BE, true);
		}
		private void mniFileFormat_Encoding_UTF32_LE_Click(object sender, EventArgs e)
		{
			this.SetCodePage((int)IO.TextFile.SpecialCodePage.Utf32_LE, true);
		}
		private void mniFileFormat_Encoding_ANSIs_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem mni = sender as ToolStripMenuItem;
			if ((mni == null) || (mni.Tag == null) || (mni.Tag.GetType() != typeof(int)))
				return;
			this.SetCodePage((int)mni.Tag, false);
		}

		#endregion

		private void SetExtractedComment(string extractedComment)
		{
			this.tcSource2.SuspendLayout();
			try
			{
				if (string.IsNullOrEmpty(extractedComment))
				{
					this.tbxExtractedComment.Text = "";
					if (this.tcSource2.TabPages.Contains(this.tpExtractedComment))
						this.tcSource2.TabPages.Remove(this.tpExtractedComment);
				}
				else
				{
					this.tbxExtractedComment.Text = extractedComment;
					if (!this.tcSource2.TabPages.Contains(this.tpExtractedComment))
						this.tcSource2.TabPages.Add(this.tpExtractedComment);
				}
			}
			catch
			{
				throw;
			}
			finally
			{
				this.tcSource2Changed(true);
			}
		}
		private void SetReferences(string[] references)
		{
			this.tcSource2.SuspendLayout();
			try
			{

				int i = this.flpReferences.Controls.Count;
				while (i > 0)
				{
					Control control = this.flpReferences.Controls[--i];
					this.flpReferences.Controls.RemoveAt(i);
					control.Dispose();
				}
				if ((references == null) || (references.Length == 0))
				{
					if (this.tcSource2.TabPages.Contains(this.tpReferences))
						this.tcSource2.TabPages.Remove(this.tpReferences);
				}
				else
				{
					foreach (string reference in references)
					{
						LinkLabel link = new LinkLabel();
						link.UseMnemonic = false;
						link.Text = reference;
						link.AutoSize = true;
						link.Margin = new Padding(1);
						link.Padding = new Padding(0, 2, 0, 2);
						link.LinkClicked += Reference_LinkClicked;
						this.flpReferences.Controls.Add(link);
						this.flpReferences.SetFlowBreak(link, true);
					}
					if (!this.tcSource2.TabPages.Contains(this.tpReferences))
						this.tcSource2.TabPages.Add(this.tpReferences);
				}
			}
			catch
			{
				throw;
			}
			finally
			{
				this.tcSource2Changed(true);
			}
		}
		private void Reference_LinkClicked(object sender, EventArgs e)
		{
			LinkLabel link = sender as LinkLabel;
			string reference = (link == null) ? null : link.Text;
			if (string.IsNullOrEmpty(reference))
				return;
			if (Program.Vars.ViewerPath.Length == 0)
			{
				if (MessageBox.Show(string.Format("You need to setup the viewer program to view source files.{0}{0}Do you want to do it now?", Environment.NewLine), Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
				{
					this.ShowOptions();
					if (Program.Vars.ViewerPath.Length == 0)
						return;
				}
				else
					return;
			}
			string fileName = reference;
			fileName = fileName.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
			int? line = null;
			Match match = Regex.Match(fileName, @".+:(?<num>\d+)$", RegexOptions.ExplicitCapture | RegexOptions.Singleline);
			if (match.Success)
			{
				int l;
				if (int.TryParse(match.Groups["num"].Value, out l))
				{
					line = l;
					fileName = fileName.Substring(0, fileName.LastIndexOf(':'));
				}
			}
			string referenceFolder = (this._currentPOReferenceFolder == null) ? Path.GetDirectoryName(this.CurrentPO.TextFile.FileName) : this._currentPOReferenceFolder;
			string fullFileName;
			for (; ; )
			{
				fullFileName = Path.Combine(referenceFolder, fileName);
				if (File.Exists(fullFileName))
					break;
				if (MessageBox.Show(string.Format("The file{0}{1}{0}does not exists in the folder{0}{1}{0}{0}Do you want to choose a reference folder?", Environment.NewLine, fileName, referenceFolder), Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
					return;
				this.fbdReference.SelectedPath = referenceFolder;
				if (this.fbdReference.ShowDialog(this) != DialogResult.OK)
					return;
				referenceFolder = this.fbdReference.SelectedPath;
			}
			this._currentPOReferenceFolder = referenceFolder;
			try
			{
				ProcessStartInfo psi = new ProcessStartInfo();
				psi.Arguments = Program.Vars.ViewerParameters.Replace("%line%", line.HasValue ? line.Value.ToString() : "").Replace("%file%", fullFileName);
				psi.ErrorDialog = false;
				psi.FileName = Program.Vars.ViewerPath;
				psi.UseShellExecute = false;
				psi.WindowStyle = ProcessWindowStyle.Normal;
				psi.WorkingDirectory = Path.GetDirectoryName(fullFileName);
				using (Process proc = new Process())
				{
					proc.StartInfo = psi;
					proc.Start();
				}
			}
			catch (Exception x)
			{
				MessageBox.Show(x.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}
		private void tcSource2Changed(bool resumeLayout)
		{
			try
			{
				List<TabPage> curPages = new List<TabPage>(this.tcSource2.TabPages.Count);
				if (this.tcSource2.TabPages.Contains(this.tpExtractedComment))
					curPages.Add(this.tpExtractedComment);
				if (this.tcSource2.TabPages.Contains(this.tpReferences))
					curPages.Add(this.tpReferences);
				this.tcSource2.TabPages.Clear();
				if (curPages.Count > 0)
				{
					this.tcSource2.TabPages.AddRange(curPages.ToArray());
					this.scSource.Panel2Collapsed = false;
				}
				else
				{
					this.scSource.Panel2Collapsed = true;
				}
			}
			catch
			{
				throw;
			}
			finally
			{
				if (resumeLayout)
					this.tcSource2.ResumeLayout();
			}
		}
		private Dictionary<TranslPlace, TabbedText> _tabbedTexts = null;

		private Dictionary<TranslPlace, TabbedText> TabbedTexts
		{
			get
			{
				if (this._tabbedTexts == null)
				{
					Dictionary<TranslPlace, TabbedText> tt = new Dictionary<TranslPlace, TabbedText>();
					tt.Add(TranslPlace.Source, new TabbedText(this, TranslPlace.Source, this.tcSource));
					tt.Add(TranslPlace.Translated, new TabbedText(this, TranslPlace.Translated, this.tcTranslated));
					this._tabbedTexts = tt;
				}
				return this._tabbedTexts;
			}
		}
		private class TabbedText
		{
			private frmMain _mainForm;
			private TranslPlace _translPlace;
			private TabControl _tabControl;
			private TextBox _tbxBase;
			public int Count
			{
				get
				{ return this._tabControl.TabPages.Count; }
			}
			public int Index
			{
				get
				{ return this._tabControl.SelectedIndex; }
				set
				{ this._tabControl.SelectedIndex = value; }
			}
			public void OptionsApplied()
			{
				switch (this._translPlace)
				{
					case TranslPlace.Source:
						this._tbxBase.Font = Program.Vars.SourceFont;
						break;
					case TranslPlace.Translated:
						this._tbxBase.Font = Program.Vars.TranslatedFont;
						break;
				}
				foreach (TextBox tb in this.TextBoxes)
					tb.Font = this._tbxBase.Font;
			}
			public string[] TabNames
			{
				get
				{
					List<string> r = new List<string>(this._tabControl.TabCount);
					foreach (TabPage tp in this._tabControl.TabPages)
						r.Add(tp.Text);
					return r.ToArray();
				}
				set
				{
					if ((value == null) || (value.Length == 0))
						throw new ArgumentNullException("TabNames");
					int nNew = value.Length;
					int nCur = this._tabControl.TabCount;
					while (nCur > nNew)
					{
						nCur--;
						TabPage tp = this._tabControl.TabPages[nCur];
						while (tp.Controls.Count > 0)
						{
							Control c = tp.Controls[0];
							tp.Controls.RemoveAt(0);
							c.Dispose();
						}
						this._tabControl.TabPages.Remove(tp);
						tp.Dispose();
					}
					for (int i = 0; i < nCur; i++)
					{
						this._tabControl.TabPages[i].Text = (value[i] == null) ? "" : value[i];
					}
					while (nCur < nNew)
					{
						TabPage tp = new TabPage((value[nCur] == null) ? "" : value[nCur]);
						TextBox tb = new TextBox();
						tb.BorderStyle = this._tbxBase.BorderStyle;
						tb.HideSelection = this._tbxBase.HideSelection;
						tb.Multiline = this._tbxBase.Multiline;
						tb.Font = this._tbxBase.Font;
						tb.Left = this._tbxBase.Left;
						tb.Top = this._tbxBase.Top;
						tb.Width = this._tbxBase.Width;
						tb.Height = this._tbxBase.Height;
						tb.Anchor = this._tbxBase.Anchor;
						tb.ScrollBars = this._tbxBase.ScrollBars;
						tb.Visible = true;
						tp.Controls.Add(tb);
						this._tabControl.TabPages.Add(tp);
						nCur++;
					}
					this._reGetTextBoxes();
				}
			}
			public TabbedText(frmMain mainForm, TranslPlace translPlace, TabControl tabControl)
			{
				this._mainForm = mainForm;
				this._translPlace = translPlace;
				this._tabControl = tabControl;
				this._reGetTextBoxes();
				this._tbxBase = this.TextBoxes[0];
			}
			private TextBox[] _textBoxes = null;

			public TextBox[] TextBoxes
			{
				get
				{ return this._textBoxes; }
			}
			private void _reGetTextBoxes()
			{
				this._textBoxes = new TextBox[this.Count];
				for (int i = 0; i < this.Count; i++)
					this._textBoxes[i] = (TextBox)this._tabControl.TabPages[i].Controls[0];
			}
			public void SetText(int index, string text, bool readOnly)
			{
				if (this._translPlace == TranslPlace.Translated)
				{
					try
					{ this.TextBoxes[index].TextChanged -= this._mainForm.tbxTranslated_TextChanged; }
					catch
					{ }
					this.TextBoxes[index].RightToLeft = this._mainForm.CurrentPO.Language.TextInfo.IsRightToLeft ? RightToLeft.Yes : RightToLeft.No;
				}
				this.TextBoxes[index].ReadOnly = readOnly;
				if (string.IsNullOrEmpty(text))
					this.TextBoxes[index].Clear();
				else
					this.TextBoxes[index].Text = text;
				this.TextBoxes[index].ClearUndo();
				if (this._translPlace == TranslPlace.Translated)
				{
					try
					{ this.TextBoxes[index].TextChanged += this._mainForm.tbxTranslated_TextChanged; }
					catch
					{ }
				}
			}
			public TextBox TextBox
			{
				get
				{
					return this.TextBoxes[this.Index];
				}
			}
			public void ShowTab(int tabIndex)
			{
				if ((tabIndex >= 0) && (tabIndex <= this._tabControl.TabCount) && (this._tabControl.SelectedIndex != tabIndex))
					this._tabControl.SelectedIndex = tabIndex;
			}
		}
		private void UpdateWindowTitle()
		{
			StringBuilder sb = new StringBuilder(Application.ProductName);
			if (this.CurrentPO == null)
			{
				sb.Append(" v").Append(Program.VERSION);
			}
			else
			{
				sb.Append(" - ").Append(Path.GetFileName(this.CurrentPO.TextFile.FileName));
				if (this.CurrentPO.Dirty)
					sb.Append('*');
			}
			string s = sb.ToString();
			if (!s.Equals(this.Text, StringComparison.Ordinal))
				this.Text = s;
		}

		private void tsiView_Untranslated_Click(object sender, EventArgs e)
		{
			this.tsiView_Untranslated.Checked = !this.tsiView_Untranslated.Checked;
			this.RefreshViewedItems();
		}

		private void tsiView_Translated_Click(object sender, EventArgs e)
		{
			this.tsiView_Translated.Checked = !this.tsiView_Translated.Checked;
			this.RefreshViewedItems();
		}

		private void tsiView_Removed_Click(object sender, EventArgs e)
		{
			this.tsiView_Removed.Checked = !this.tsiView_Removed.Checked;
			this.RefreshViewedItems();
		}

		private void RefreshViewedItems()
		{
			List<PO.Entry> entries = new List<PO.Entry>();
			if (this.tsiView_Untranslated.Checked || this.tsiView_Translated.Checked || this.tsiView_Removed.Checked)
			{
				foreach (PO.Entry entry in this.CurrentPO.Entries)
				{
					bool ok = false;
					switch (entry.Kind)
					{
						case PO.Entry.Kinds.Header:
							break;
						case PO.Entry.Kinds.Standard:
							PO.DataEntry data = (PO.DataEntry)entry;
							if ((this.tsiView_Fuzzy.Checked && data.Fuzzy) || (this.tsiView_Clear.Checked && (!data.Fuzzy)))
							{
								if (this.tsiView_Translated.Checked && this.tsiView_Untranslated.Checked)
									ok = true;
								else if (this.tsiView_Translated.Checked || this.tsiView_Untranslated.Checked)
								{

									if (data.IsTranslated)
										ok = this.tsiView_Translated.Checked;
									else
										ok = this.tsiView_Untranslated.Checked;
								}
							}
							break;
						case PO.Entry.Kinds.Removed:
							ok = this.tsiView_Removed.Checked;
							break;
					}
					if (ok)
						entries.Add(entry);
				}
			}
			this.CurrentEntries = entries;
		}

		private void RefreshStatus()
		{
			this.sslProgr.Text = string.Format("{0:N0} / {1:N0}", this._currentPO.TranslatedItems, this._currentPO.TotalDataEntries);
			this.ssbProgr.Value = this._currentPO.TranslatedItems;
		}

		private void tsiView_Fuzzy_Click(object sender, EventArgs e)
		{
			this.tsiView_Fuzzy.Checked = !this.tsiView_Fuzzy.Checked;
			if (!this.tsiView_Clear.Checked)
				this.tsiView_Clear.Checked = true;
			this.RefreshViewedItems();
		}

		private void tsiView_Clear_Click(object sender, EventArgs e)
		{
			this.tsiView_Clear.Checked = !this.tsiView_Clear.Checked;
			if (!this.tsiView_Fuzzy.Checked)
				this.tsiView_Fuzzy.Checked = true;
			this.RefreshViewedItems();
		}

		private Font _lbxEntries_FuzzyFont = null;
		private Font lbxEntries_FuzzyFont
		{
			get
			{
				if (this._lbxEntries_FuzzyFont == null)
					this._lbxEntries_FuzzyFont = new Font(this.lbxEntries.Font, FontStyle.Italic);
				return this._lbxEntries_FuzzyFont;
			}
		}
		private void lbxEntries_DrawItem(object sender, DrawItemEventArgs e)
		{
			if (e.Index < 0)
			{
				e.DrawBackground();
			}
			else
			{
				bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
				ColorEnviro enviro = 0;
				PO.Entry entry = (PO.Entry)this.lbxEntries.Items[e.Index];
				string text = "";
				Font font = entry.Fuzzy ? this.lbxEntries_FuzzyFont : this.lbxEntries.Font;

				switch (entry.Kind)
				{
					case PO.Entry.Kinds.Removed:
						PO.RemovedEntry removed = (PO.RemovedEntry)entry;
						enviro = removed.Fuzzy ? ColorEnviro.Removed : ColorEnviro.Removed;
						text = removed.RemovedID;
						break;
					case PO.Entry.Kinds.Standard:
						PO.DataEntry data = (PO.DataEntry)entry;
						if (data.IsTranslated)
						{
							enviro = entry.Fuzzy ? ColorEnviro.Translated : ColorEnviro.Translated;
						}
						else
						{
							enviro = entry.Fuzzy ? ColorEnviro.Untranslated : ColorEnviro.Untranslated;
						}
						text = data.ID;
						break;
				}
				using (SolidBrush brush = new SolidBrush(Program.Vars.Colors[ColorEnviroPlace.Get(enviro, selected ? ColorPlace.BackSel : ColorPlace.BackUnsel)]))
				{
					e.Graphics.FillRectangle(brush, e.Bounds);
				}
				if (text.Length > 0)
				{
					using (SolidBrush brush = new SolidBrush(Program.Vars.Colors[ColorEnviroPlace.Get(enviro, selected ? ColorPlace.ForeSel : ColorPlace.ForeUnsel)]))
					{
						e.Graphics.DrawString(text, font, brush, e.Bounds, StringFormat.GenericDefault);
					}
				}
			}
			e.DrawFocusRectangle();
		}

		private void tsiOptions_Click(object sender, EventArgs e)
		{
			this.ShowOptions();
		}
		private void ShowOptions()
		{
			using (frmOptions frm = new frmOptions(this.OptionsApplied))
			{
				if (frm.ShowDialog() == DialogResult.OK)
				{
					this.OptionsApplied();
				}
			}
		}
		private void OptionsApplied()
		{
			this.lbxEntries.Refresh();
			foreach (TabbedText tt in this.TabbedTexts.Values)
				tt.OptionsApplied();
			this.RefreshSpecialChars();
		}

		private void lbxEntries_SelectedIndexChanged(object sender, EventArgs e)
		{
			List<Control> activeControls = new List<Control>();
			Control ac = this.ActiveControl;
			while (ac != null)
			{
				activeControls.Add(ac);
				if (ac is IContainerControl)
					ac = ((IContainerControl)ac).ActiveControl;
				else
					break;
			}
			UpdateSelectedEntry();
			try
			{
				for (int i = 0; i < activeControls.Count; i++)
					activeControls[i].Focus();
			}
			catch
			{ }
		}

		private void UpdateSelectedEntry()
		{
			this.SuspendLayout();
			try
			{
				PO.Entry entry = this.lbxEntries.SelectedItem as PO.Entry;
				this.ShowSearchResult(null, null);
				this.ClearEntryMessagesResults();
				if (entry == null)
					this.scEntry.Visible = false;
				if (entry == null)
				{
					this.SetExtractedComment(null);
					this.SetReferences(null);
				}
				else
				{
					this.SetExtractedComment(entry.ExtractedComment);
					this.SetReferences(entry.Reference);
					this.tbxTranslatorComment.Text = entry.TranslatorComment;
					this.chkFuzzyTranslation.Enabled = entry is PO.DataEntry;
					this.chkFuzzyTranslation.Checked = entry.Fuzzy;
					this.scTranslation.Panel2Collapsed = entry.TranslatorComment.Length == 0;
					string ctx = "";
					if (entry is PO.RemovedEntry)
						ctx = ((PO.RemovedEntry)entry).RemovedContext;
					else if (entry is PO.DataEntry)
						ctx = ((PO.DataEntry)entry).Context;
					else
						ctx = "";
					this.tbxSourceContext.Text = ctx;
					this.tbxSourceContext.Visible = this.lblSourceContext.Visible = (ctx.Length > 0);
					this.TabbedTexts[TranslPlace.Source].TabNames = entry.PluralSource ? new string[] { "Source text - Singular", "Source text - Plural" } : new string[] { "Source text" };
					string[] translTexts;
					if (entry.NumTranslated == 1)
					{
						translTexts = new string[] { "Translated" };
					}
					else
					{
						translTexts = new string[entry.NumTranslated];
						for (int i = 0; i < entry.NumTranslated; i++)
							translTexts[i] = entry.File.GetPluralTranslatedText(i, entry.NumTranslated);
					}
					this.lnkTranslatorComment.Visible = false;
					this.tbxTranslatorComment.Text = entry.TranslatorComment;
					this.scTranslation.Panel2Collapsed = entry.TranslatorComment.Length == 0;
					this.tbxTranslatorComment.ReadOnly = true;
					switch (entry.Kind)
					{
						case PO.Entry.Kinds.Removed:
							this.TabbedTexts[TranslPlace.Translated].TabNames = translTexts;
							PO.RemovedEntry removed = (PO.RemovedEntry)entry;
							this.TabbedTexts[TranslPlace.Source].SetText(0, removed.RemovedID, true);
							PO.SingleRemovedEntry removed1 = removed as PO.SingleRemovedEntry;
							if (removed1 != null)
							{
								this.TabbedTexts[TranslPlace.Translated].SetText(0, removed1.RemovedTranslated, true);
							}
							else
							{
								PO.PluralRemovedEntry removedN = (PO.PluralRemovedEntry)removed;
								this.TabbedTexts[TranslPlace.Source].SetText(1, removedN.RemovedIDPlural, true);
								for (int i = 0; i < entry.NumTranslated; i++)
									this.TabbedTexts[TranslPlace.Translated].SetText(i, removedN.RemovedTranslated[i], true);
							}
							break;
						case PO.Entry.Kinds.Standard:
							this.TabbedTexts[TranslPlace.Translated].TabNames = translTexts;
							this.lnkTranslatorComment.Visible = true;
							this.tbxTranslatorComment.ReadOnly = false;
							PO.DataEntry data = (PO.DataEntry)entry;
							this.TabbedTexts[TranslPlace.Source].SetText(0, data.ID, true);
							PO.SingleDataEntry data1 = data as PO.SingleDataEntry;
							if (data1 != null)
							{
								this.TabbedTexts[TranslPlace.Translated].SetText(0, data1.Translated, false);
							}
							else
							{
								PO.PluralDataEntry dataN = (PO.PluralDataEntry)data;
								this.TabbedTexts[TranslPlace.Source].SetText(1, dataN.IDPlural, true);
								for (int i = 0; i < entry.NumTranslated; i++)
									this.TabbedTexts[TranslPlace.Translated].SetText(i, dataN.get_Translated(i), false);
							}
							break;
						default:
							throw new NotImplementedException();
					}
				}
				if (entry != null)
					this.scEntry.Visible = true;
			}
			catch
			{
				throw;
			}
			finally
			{
				this.ResumeLayout();
				this.PerformLayout();
			}
		}



		private void lnkAutoTranslate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			TextBox from = this.TabbedTexts[TranslPlace.Source].TextBox;
			if (from == null)
				return;
			TextBox to = this.TabbedTexts[TranslPlace.Translated].TextBox;
			if ((to != null) && (to.ReadOnly || (!to.Enabled)))
				to = null;
			try
			{
				//string translated = Utils.Translator.Translate("en", this.CurrentPO.Language.TwoLetterISOLanguageName, from.Text);
				string translated = Utils.BingTranslator.Translate("en", this.CurrentPO.Language.TwoLetterISOLanguageName, from.Text);
				if (string.IsNullOrEmpty(translated))
				{
					MessageBox.Show("Not translated.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
				else if (to == null)
				{
					MessageBox.Show(translated, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				else
				{
					if (to.Text.Length > 0)
					{
						switch (MessageBox.Show(string.Format("Do you want to replace the current text with the translated one?{0}{0}If you say NO it will be appended", Environment.NewLine), Application.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
						{
							case DialogResult.Yes:
								to.Clear();
								break;
							case DialogResult.No:
								to.AppendText(Environment.NewLine);
								break;
							case DialogResult.Cancel:
								return;
						}
					}
					to.AppendText(translated);
				}
			}
			catch (Exception x)
			{
				MessageBox.Show(x.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}

		}

		private void tbxTranslated_TextChanged(object sender, EventArgs e)
		{
			TextBox tbx = sender as TextBox;
			if ((tbx == null) || tbx.ReadOnly || (tbx != this.TabbedTexts[TranslPlace.Translated].TextBox))
			{
				MessageBox.Show("Text changed??? Why???", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			PO.DataEntry entry = this.lbxEntries.SelectedItem as PO.DataEntry;
			if (entry != null)
			{
				if (entry is PO.PluralDataEntry)
				{
					((PO.PluralDataEntry)entry).set_Translated(this.TabbedTexts[TranslPlace.Translated].Index, tbx.Text);
				}
				else if (this.TabbedTexts[TranslPlace.Translated].Index == 0)
				{
					((PO.SingleDataEntry)entry).Translated = tbx.Text;
				}
				else
				{
					MessageBox.Show("Single-entry changed with index!=0??? Why???", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			else
			{
				MessageBox.Show("Text changed on non DataEntry??? Why???", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		#region Search

		private frmSearch _searchForm = null;
		private void tsiSearch_Click(object sender, EventArgs e)
		{
			this.SwitchSearch();
		}
		private void SwitchSearch()
		{
			if (!this.tsiSearch.Enabled)
				return;
			if (this.tsiSearch.Checked)
			{
				if (this._searchForm != null)
					this._searchForm.Hide();
				this.tsiSearch.Checked = false;
			}
			else
			{
				if (this._searchForm == null)
					this._searchForm = new frmSearch(this);
				this._searchForm.Show(this);
				this.tsiSearch.Checked = true;
			}
		}
		void frmSearch.ISearchConnection.WinUnloaded()
		{
			if (this._searchForm != null)
			{
				this._searchForm.ClearAllData();
				this._searchForm = null;
			}
			this.tsiSearch.Checked = false;
		}
		void frmSearch.ISearchConnection.HideWin()
		{
			if (this._searchForm != null)
				this._searchForm.Hide();
			this.tsiSearch.Checked = false;
		}
		List<PO.Entry> frmSearch.ISearchConnection.GetCurrentItems()
		{
			return this.CurrentEntries;

		}
		void frmSearch.ISearchConnection.ShowResult(frmSearch.SearchResult result, Form formToActivate)
		{
			this.ShowSearchResult(result, formToActivate);

		}
		private void ShowSearchResult(frmSearch.SearchResult result, Form formToActivate)
		{
			PO.Entry entry;
			if (result != null)
			{
				entry = result.Entry;
				if (!this.lbxEntries.Items.Contains(entry))
				{
					MessageBox.Show("Entry not found.");
					return;
				}
				this.lbxEntries.SelectedItem = entry;
			}
			else
			{
				entry = this.lbxEntries.SelectedItem as PO.Entry;
			}
			foreach (frmSearch.SearchPlace placeToReset in Enum.GetValues(typeof(frmSearch.SearchPlace)))
				this.ShowSearchResult(placeToReset, -1, 0, 0);
			if (result != null)
			{
				for (int n = 0; n < result.Places.Count; n++)
				{
					this.ShowSearchResult(result.Places[n], result.Indexes[n], result.Starts[n], result.Lengths[n]);
				}
			}
			if (formToActivate != null)
				formToActivate.Activate();
		}
		private void ShowSearchResult(frmSearch.SearchPlace place, int? index, int? start, int? length)
		{
			bool isResetting = index.HasValue && (index.Value < 0);
			switch (place)
			{
				case frmSearch.SearchPlace.Context:
					ShowSearchResult(this.tbxSourceContext, isResetting ? 0 : start, isResetting ? 0 : length);
					break;
				case frmSearch.SearchPlace.Reference:
					for (int i = 0; i < this.flpReferences.Controls.Count; i++)
					{
						if (isResetting || (index.Value == i))
						{
							LinkLabel link = (LinkLabel)this.flpReferences.Controls[i];
							link.LinkColor = isResetting ? Color.FromArgb(0, 0, 255) : Color.Red;
							if (!isResetting)
								this.flpReferences.ScrollControlIntoView(link);
						}
					}
					break;
				case frmSearch.SearchPlace.SourceComment:
					ShowSearchResult(this.tbxExtractedComment, isResetting ? 0 : start, isResetting ? 0 : length);
					break;
				case frmSearch.SearchPlace.TranslatorComment:
					ShowSearchResult(this.tbxTranslatorComment, isResetting ? 0 : start, isResetting ? 0 : length);
					break;
				case frmSearch.SearchPlace.Source:
					ShowSearchResult(this.TabbedTexts[TranslPlace.Source], index, start, length);
					break;
				case frmSearch.SearchPlace.Translated:
					ShowSearchResult(this.TabbedTexts[TranslPlace.Translated], index, start, length);
					break;
			}
		}
		private static void ShowSearchResult(TabbedText tt, int? index, int? start, int? length)
		{
			bool isResetting = index.HasValue && (index.Value < 0);
			int realIndex = index.HasValue ? index.Value : 0;
			if (!isResetting && index.HasValue && (index.Value >= 0) && (index.Value < tt.Count))
				tt.ShowTab(index.Value);
			for (int i = 0; i < tt.Count; i++)
			{
				if (isResetting || (i == realIndex))
					ShowSearchResult(tt.TextBoxes[i], isResetting ? 0 : start, isResetting ? 0 : length);
			}
		}
		private static void ShowSearchResult(TextBox tb, int? start, int? length)
		{
			if ((tb != null) && tb.Visible && (tb.Text.Length > 0))
			{
				int nChars = tb.Text.Length;
				if (start.HasValue && (start.Value >= 0) && (start.Value < nChars))
				{
					tb.SelectionStart = start.Value;
					if (length.HasValue && (length.Value >= 0) && ((start.Value + length.Value) <= nChars))
						tb.SelectionLength = Math.Min(nChars - start.Value, length.Value);
					tb.ScrollToCaret();
				}
				else
					tb.SelectAll();
			}
		}
		#endregion

		private void frmMain_KeyDown(object sender, KeyEventArgs e)
		{
			Keys modifiers = Keys.None;
			if (e.Alt)
				modifiers |= Keys.Alt;
			if (e.Control)
				modifiers |= Keys.Control;
			if (e.Shift)
				modifiers |= Keys.Shift;
			foreach (AcceletatorKeyInfoAttribute acc in this.AcceletatorKeys.Values)
			{
				if ((acc.Modifiers == modifiers) && (acc.Key == e.KeyCode))
				{
					switch (acc.Description)
					{
						case KEY_OPEN:
							this.OpenPO();
							e.Handled = true;
							break;
						case KEY_SAVE:
							this.SavePO();
							e.Handled = true;
							break;
						case KEY_PREV_ITEM:
							this.DeltaEntry(true);
							e.Handled = true;
							break;
						case KEY_NEXT_ITEM:
							this.DeltaEntry(false);
							e.Handled = true;
							break;
						case KEY_SEARCH:
							this.SwitchSearch();
							e.Handled = true;
							break;
					}
				}
			}
		}

		private void DeltaEntry(bool back)
		{
			if (this.CurrentPO == null)
				return;
			int count = this.lbxEntries.Items.Count;
			if (count < 1)
				return;
			int index = this.lbxEntries.SelectedIndex;
			if (index < 0)
			{
				if (back)
					this.lbxEntries.SelectedIndex = count - 1;
				else
					this.lbxEntries.SelectedIndex = 0;
			}
			else
			{
				if (count > 2)
				{
					if (back)
					{
						if (--index < 0)
							index = count - 1;
					}
					else
					{
						if (++index >= count)
							index = 0;
					}
					this.lbxEntries.SelectedIndex = index;
				}
			}
		}

		private void tsiInfo_Click(object sender, EventArgs e)
		{
			using (frmInfo frm = new frmInfo(this.AcceletatorKeys.Values))
			{
				frm.ShowDialog(this);
			}
		}

		private void lnkTranslatorComment_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			PO.DataEntry dataEntry = this.lbxEntries.SelectedItem as PO.DataEntry;
			if (dataEntry != null)
				this.scTranslation.Panel2Collapsed = !this.scTranslation.Panel2Collapsed;
		}

		private void tbxTranslatorComment_TextChanged(object sender, EventArgs e)
		{
			PO.DataEntry dataEntry = this.lbxEntries.SelectedItem as PO.DataEntry;
			if (dataEntry != null)
				dataEntry.TranslatorComment = this.tbxTranslatorComment.Text;
		}

		private void chkFuzzyTranslation_CheckedChanged(object sender, EventArgs e)
		{
			PO.DataEntry dataEntry = this.lbxEntries.SelectedItem as PO.DataEntry;
			if (dataEntry != null)
			{
				dataEntry.set_Fuzzy(this.chkFuzzyTranslation.Checked);
				this.lbxEntries.Refresh();
			}
		}

		private void lnkTidyTranslation_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			TextBox tbx = this.TabbedTexts[TranslPlace.Translated].TextBox;
			if (tbx == null)
				return;
			if (Regex.Replace(tbx.Text, @"\s", "").Length == 0)
			{
				MessageBox.Show("No text to verify!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}
			try
			{
				Utils.Tidy.Result[] result = Utils.Tidy.Result.Analyze(tbx.Text);
				if (result.Length == 0)
				{
					MessageBox.Show("The html is correct!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}
				frmEntryMessages.MessageDetail[] msgs = new frmEntryMessages.MessageDetail[result.Length];
				for (int i = 0; i < result.Length; i++)
					msgs[i] = new frmEntryMessages.MessageDetail(result[i]);
				this.SetEntryMessagesResults(tbx, msgs, true);
			}
			catch (Exception x)
			{
				MessageBox.Show(x.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (!this.AskDirty())
			{
				e.Cancel = true;
			}
			if (this._searchForm != null)
				this._searchForm.ClearAllData();
			this.ClearEntryMessagesResults();
			this.CurrentPO = null;
		}

		private frmEntryMessages _entryMessagesForm = null;
		internal void ClearEntryMessagesResults()
		{
			this.SetEntryMessagesResults(null, null, false);
		}
		internal void SetEntryMessagesResults(TextBox tbx, frmEntryMessages.MessageDetail[] messages, bool showDetailsPanel)
		{
			if ((tbx == null) || (messages == null) || (messages.Length == 0))
			{
				if (this._entryMessagesForm != null)
				{
					this._entryMessagesForm.Hide();
					this._entryMessagesForm.Setup(null, null, false);
				}
			}
			else
			{
				if (this._entryMessagesForm == null)
					this._entryMessagesForm = new frmEntryMessages(this, tbx, messages, showDetailsPanel);
				else
					this._entryMessagesForm.Setup(tbx, messages, showDetailsPanel);
				if (!this._entryMessagesForm.Visible)
					this._entryMessagesForm.Show(this);
			}
		}
		void frmEntryMessages.IEntryMessageResultsConnection.WinUnloaded()
		{
			this._entryMessagesForm = null;
		}
		void frmEntryMessages.IEntryMessageResultsConnection.HideWin()
		{
			this.ClearEntryMessagesResults();
		}

		private void tsiCompile_Click(object sender, EventArgs e)
		{
			this.CompilePO();
		}
		private void CompilePO()
		{
			if (this.CurrentPO == null)
				return;
			if (Program.Vars.MOCompilerPath.Length == 0)
			{
				if (MessageBox.Show(string.Format("You need to setup the mo compiler program to compile the file.{0}{0}Do you want to do it now?", Environment.NewLine), Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
				{
					this.ShowOptions();
					if (Program.Vars.MOCompilerPath.Length == 0)
						return;
				}
				else
					return;

			}

			this.Cursor = Cursors.WaitCursor;
			string tempFileNameSource = null;
			string tempFileNameDest = null;
			try
			{
				if (this.CurrentPO.Dirty)
				{
					tempFileNameSource = Path.GetTempFileName();
					this.CurrentPO.TextFile.SaveACopyAs(tempFileNameSource, true);
				}

				tempFileNameDest = Path.GetTempFileName();
				if (File.Exists(tempFileNameDest))
				{
					try
					{ File.Delete(tempFileNameDest); }
					catch
					{ }
				}
				List<string> moCompilerParameters = new List<string>();
				if(Program.Vars.MOCompiler_CheckFormat)
				{
					moCompilerParameters.Add("--check-format");
				}
				if(Program.Vars.MOCompiler_CheckHeader)
				{
					moCompilerParameters.Add("--check-header");
				}
				moCompilerParameters.Add(string.Format("--output-file=\"{0}\"", tempFileNameDest));
				moCompilerParameters.Add(string.Format("\"{0}\"", (tempFileNameSource == null) ? this.CurrentPO.TextFile.FileName : tempFileNameSource));
				ProcessStartInfo psi = new ProcessStartInfo();
				
				psi.Arguments = string.Join(" ", moCompilerParameters.ToArray());
				psi.ErrorDialog = false;
				psi.FileName = Program.Vars.MOCompilerPath;
				psi.UseShellExecute = false;
				psi.StandardErrorEncoding = System.Text.Encoding.Default;
				psi.RedirectStandardError = true;
				psi.RedirectStandardOutput = true;
				psi.WindowStyle = ProcessWindowStyle.Hidden;
				psi.CreateNoWindow = true;
				psi.WorkingDirectory = Path.GetDirectoryName(Program.Vars.MOCompilerPath);
				using (Process proc = new Process())
				{
					proc.StartInfo = psi;
					proc.Start();
					proc.WaitForExit(15000);
					string sErr = null;
					if (proc.ExitCode != 0)
					{
						const int ERR_DLL_NOT_FOUND = -1073741515;

						if (proc.ExitCode == ERR_DLL_NOT_FOUND)
							sErr = "Missing a dll library.";
						else
							sErr = (proc.ExitCode < 0) ? string.Format("Process returned an error (code 0x{0:X})", proc.ExitCode) : string.Format("Process returned an error (code {0:N0})", proc.ExitCode);
					}
					if ((proc.StandardError != null) && (!proc.StandardError.EndOfStream))
					{
						string se = proc.StandardError.ReadToEnd();
						if (!string.IsNullOrEmpty(se))
						{
							if (sErr == null)
								sErr = se;
							else
								sErr = string.Format("{1}{0}{2}", Environment.NewLine, sErr, se);
						}
					}
					else if ((proc.StandardOutput != null) && (!proc.StandardOutput.EndOfStream))
					{
						string se = proc.StandardOutput.ReadToEnd();
						if (!string.IsNullOrEmpty(se))
						{
							if (sErr == null)
								sErr = se;
							else
								sErr = string.Format("{1}{0}{2}", Environment.NewLine, sErr, se);
						}
					}
					if (sErr != null)
						throw new Exception(sErr);

				}
				string destFileName;
				if (this.CurrentPO.TextFile.FileName.ToLower().EndsWith(".mo"))
					destFileName = string.Format("{0}.mo", this.CurrentPO.TextFile.FileName);
				else
					destFileName = Path.ChangeExtension(this.CurrentPO.TextFile.FileName, ".mo");
				if (File.Exists(destFileName))
					File.Delete(destFileName);
				File.Move(tempFileNameDest, destFileName);
				tempFileNameDest = null;
			}
			catch (Exception x)
			{
				this.Cursor = Cursors.Default;
				MessageBox.Show(x.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			finally
			{
				if ((tempFileNameDest != null) && File.Exists(tempFileNameDest))
				{
					try
					{ File.Delete(tempFileNameDest); }
					catch
					{ }
					finally
					{ tempFileNameDest = null; }
				}
				if ((tempFileNameSource != null) && File.Exists(tempFileNameSource))
				{
					try
					{ File.Delete(tempFileNameSource); }
					catch
					{ }
					finally
					{ tempFileNameSource = null; }
				}
				this.Cursor = Cursors.Default;
			}

		}
		private void lnkCheckTranslationSpelling_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			TextBox tbx = this.TabbedTexts[TranslPlace.Translated].TextBox;
			if (tbx == null)
				return;
			if (Regex.Replace(tbx.Text, @"\s", "").Length == 0)
			{
				MessageBox.Show("No text to verify!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}
			try
			{
				Utils.SpellChecker.Result[] errors;
				using (Utils.SpellChecker.Worker worker = Program.SpellChecker.GetWorker(this.CurrentPO.Language))
					errors = worker.Analyze(tbx.Text, true);
				if (errors.Length == 0)
				{
					MessageBox.Show("The text is correct!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}
				if (tbx.Enabled && (!tbx.ReadOnly))
				{
					using (Utils.SpellChecker.Worker worker = Program.SpellChecker.GetWorker(this.CurrentPO.Language, this.components))
					{
						worker.Analyze(tbx);
					}
				}
				else
				{
					frmEntryMessages.MessageDetail[] msgs = new frmEntryMessages.MessageDetail[errors.Length];
					for (int i = 0; i < errors.Length; i++)
						msgs[i] = new frmEntryMessages.MessageDetail(errors[i]);
					this.SetEntryMessagesResults(tbx, msgs, false);
				}
			}
			catch (Exception x)
			{
				MessageBox.Show(x.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		private void frmMain_Shown(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(Program.InitialFile))
			{
				string fn = Program.InitialFile;
				Program.InitialFile = null;
				this.LoadPOFile(fn, false);
			}
		}

		private void RefreshSpecialChars()
		{
			this.ctxSpecialChars.Items.Clear();
			foreach (KeyValuePair<string, string> kv in Program.Vars.SpecialChars)
			{
				ToolStripMenuItem i = new ToolStripMenuItem();
				i.Text = kv.Key;
				i.Tag = kv.Value;
				this.ctxSpecialChars.Items.Add(i);
			}
			this.lnkSpecialChar.Enabled = this.ctxSpecialChars.Items.Count > 0;
		}

		private void ctxSpecialChars_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			ToolStripMenuItem i = e.ClickedItem as ToolStripMenuItem;
			if (i != null)
			{
				foreach (TextBox tb in this.TabbedTexts[TranslPlace.Translated].TextBoxes)
				{
					if (tb.Enabled && tb.Visible && (tb.Parent != null) && tb.Parent.Visible)
					{
						string insertMe = (string)i.Tag;
						int ss = tb.SelectionStart;
						int sl = tb.SelectionLength;
						string pre = (ss > 0) ? tb.Text.Substring(0, ss) : "";
						string post = ((ss + sl) < (tb.Text.Length - 1)) ? tb.Text.Substring(ss + sl) : "";
						tb.Text = pre + insertMe + post;
						ss = pre.Length + insertMe.Length;
						try
						{ tb.Focus(); }
						catch
						{ }
						try
						{ tb.SelectionStart = ss; tb.SelectionLength = 0; }
						catch
						{ }
						break;
					}
				}
			}
		}

		private void lnkSpecialChar_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (this.lnkSpecialChar.Enabled)
			{
				this.ctxSpecialChars.Show(this.lnkSpecialChar, new Point(this.lnkSpecialChar.Width >> 1, this.lnkSpecialChar.Height >> 1));
			}
		}
	}
}