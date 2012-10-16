using NetSpell.SpellChecker;
using NetSpell.SpellChecker.Dictionary;
using NetSpell.SpellChecker.Forms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace BePoE.Utils
{
	internal class SpellChecker
	{
		public enum BadReason
		{
			Doubled,
			Mispelled,
		}
		public class Result
		{
			public readonly BadReason Reason;
			public readonly int? SelectionStart;
			public readonly int? SelectionLength;
			public readonly string Word;
			internal Result(BadReason reason, SpellingEventArgs e, string originalText)
			{
				this.Reason = reason;
				this.SelectionStart = null;
				this.SelectionLength = null;
				this.Word = "";
				if ((e != null) && (!string.IsNullOrEmpty(e.Word)) && (e.TextIndex >= 0))
				{
					this.Word = e.Word;
					this.SelectionStart = e.TextIndex;
					this.SelectionLength = e.Word.Length;
				}
			}
			public override string ToString()
			{
				return this.ToString(false);
			}
			public string ToString(bool multiLine)
			{
				return string.Format("{0}: {1}", this.Reason, this.Word);
			}
		}
		private DirectoryInfo _dictionaryFolder;
		private DirectoryInfo DictionaryFolder
		{
			get
			{ return this._dictionaryFolder; }
		}
		public string DictionaryFolderPath
		{
			get
			{ return this._dictionaryFolder.FullName; }
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException("DictionaryFolderPath");
				if (!Directory.Exists(value))
					throw new FileNotFoundException(string.Format("The dictionary folder{0}{1}{0}does not exist.", Environment.NewLine, value), value);
				this._dictionaryFolder = new DirectoryInfo(value);
			}
		}

		public SpellChecker(string dictionaryFolderPath)
		{
			this.DictionaryFolderPath = dictionaryFolderPath;
		}
		private string GetDictionaryFileName(CultureInfo cultureInfo)
		{
			if (cultureInfo == null)
				cultureInfo = CultureInfo.CurrentCulture;
			List<string> curFiles = new List<string>();
			foreach (FileInfo fi in this.DictionaryFolder.GetFiles("*.dic"))
				curFiles.Add(fi.Name);
			string[] fileNames = new string[] { cultureInfo.Name.Replace('_', '-'), cultureInfo.Name.Replace('-', '_'), cultureInfo.TwoLetterISOLanguageName };
			foreach (string test1 in fileNames)
			{
				string fileName = string.Format("{0}.dic", test1);
				foreach (string existing in curFiles)
					if (fileName.Equals(existing, StringComparison.InvariantCultureIgnoreCase))
						return existing;
			}
			StringBuilder sb = new StringBuilder();
			sb.Append("Unable a dictionary for the culture ").Append(cultureInfo.EnglishName).Append('.').AppendLine();
			sb.AppendLine();
			sb.Append("You can create a new dictionary using the DictionaryBuilder and placing the file in the folder").AppendLine();
			sb.Append(this.DictionaryFolder).AppendLine();
			sb.AppendLine();
			sb.Append("The dictionary file name must be one of these:");
			foreach (string test1 in fileNames)
				sb.AppendLine().Append("  ").Append(test1).Append(".dic");
			sb.AppendLine();
			throw new Exception(sb.ToString());
		}
		private string GetUserDictionaryFileName(CultureInfo cultureInfo)
		{
			if (cultureInfo == null)
				cultureInfo = CultureInfo.CurrentCulture;
			string fileName = Path.Combine(this.DictionaryFolder.FullName, string.Format("{0}.user", cultureInfo.Name));
			if (!File.Exists(fileName))
			{
				File.Create(fileName).Dispose();
			}
			return fileName;
		}
		public Worker GetWorker()
		{
			return this.GetWorker(null, null);
		}
		public Worker GetWorker(CultureInfo cultureInfo)
		{
			return this.GetWorker(cultureInfo, null);
		}
		public Worker GetWorker(System.ComponentModel.IContainer container)
		{
			return this.GetWorker(null, container);
		}
		public Worker GetWorker(CultureInfo cultureInfo, System.ComponentModel.IContainer container)
		{
			return new Worker(this, cultureInfo, container);
		}
		public class Worker : IDisposable
		{
			private enum CallReasons
			{
				None,
				String,
				TextBox,
			}

			public void Dispose()
			{
				if (this._spelling != null)
				{
					try
					{ this._spelling.Dispose(); }
					catch
					{ }
					this._spelling = null;
				}

			}
			private List<Result> _currentResult;
			private CallReasons _currentReason;
			private string _currentText;
			private TextBox _currentTextBox;


			private CultureInfo _cultureInfo;
			private Spelling _spelling;
			internal Worker(SpellChecker spellChecker, CultureInfo cultureInfo, System.ComponentModel.IContainer container)
			{
				this._cultureInfo = cultureInfo;
				this._currentResult = null;
				this._currentReason = CallReasons.None;
				this._currentText = null;
				this._currentTextBox = null;
				this._spelling = new Spelling();
				if (container == null)
					this._spelling.Dictionary = new WordDictionary();
				else
					this._spelling.Dictionary = new WordDictionary(container);
				this._spelling.Dictionary.DictionaryFolder = spellChecker.DictionaryFolderPath;
				this._spelling.Dictionary.DictionaryFile = spellChecker.GetDictionaryFileName(this._cultureInfo);
				this._spelling.Dictionary.EnableUserFile = true;
				this._spelling.Dictionary.UserFile = spellChecker.GetUserDictionaryFileName(this._cultureInfo);
				this._spelling.IgnoreAllCapsWords = false;
				this._spelling.IgnoreHtml = false;
				this._spelling.IgnoreWordsWithDigits = false;
				this._spelling.DoubledWord += this._spelling_DoubledWord;
				this._spelling.MisspelledWord += this._spelling_MisspelledWord;
				this._spelling.DeletedWord += this._spelling_DeletedWord;
				this._spelling.ReplacedWord += this._spelling_ReplacedWord;
			}
			public bool IgnoreAllCapsWords
			{
				get
				{ return this._spelling.IgnoreAllCapsWords; }
				set
				{ this._spelling.IgnoreAllCapsWords = value; }
			}
			public bool IgnoreHtml
			{
				get
				{ return this._spelling.IgnoreHtml; }
				set
				{ this._spelling.IgnoreHtml = value; }
			}
			public bool IgnoreWordsWithDigits
			{
				get
				{ return this._spelling.IgnoreWordsWithDigits; }
				set
				{ this._spelling.IgnoreWordsWithDigits = value; }
			}

			private void _spelling_DoubledWord(object sender, SpellingEventArgs e)
			{
				switch (this._currentReason)
				{
					case CallReasons.String:
						this._currentResult.Add(new Result(BadReason.Doubled, e, this._currentText));
						break;
				}
			}
			private void _spelling_MisspelledWord(object sender, SpellingEventArgs e)
			{
				switch (this._currentReason)
				{
					case CallReasons.String:
						this._currentResult.Add(new Result(BadReason.Mispelled, e, this._currentText));
						break;
				}
			}
			private void _spelling_DeletedWord(object sender, SpellingEventArgs e)
			{
				switch (this._currentReason)
				{
					case CallReasons.TextBox:
						int start = this._currentTextBox.SelectionStart;
						int length = this._currentTextBox.SelectionLength;
						this._currentTextBox.Select(e.TextIndex, e.Word.Length);
						this._currentTextBox.SelectedText = "";
						if (start > this._currentTextBox.Text.Length)
							start = this._currentTextBox.Text.Length;
						if ((start + length) > this._currentTextBox.Text.Length)
							length = 0;
						this._currentTextBox.Select(start, length);
						break;
				}
			}
			private void _spelling_ReplacedWord(object sender, ReplaceWordEventArgs e)
			{
				switch (this._currentReason)
				{
					case CallReasons.TextBox:
						int start = this._currentTextBox.SelectionStart;
						int length = this._currentTextBox.SelectionLength;
						this._currentTextBox.Select(e.TextIndex, e.Word.Length);
						this._currentTextBox.SelectedText = e.ReplacementWord;
						if (start > this._currentTextBox.Text.Length)
							start = this._currentTextBox.Text.Length;
						if ((start + length) > this._currentTextBox.Text.Length)
							length = 0;
						this._currentTextBox.Select(start, length);
						break;
				}
			}


			public Result[] Analyze(string text)
			{
				return this.Analyze(text, false);
			}
			public Result[] Analyze(string text, bool stopAtFirst)
			{
				this._currentResult = new List<Result>();
				this._currentReason = CallReasons.String;
				this._currentText = text;
				try
				{
					if (!string.IsNullOrEmpty(text))
					{
						this._spelling.AlertComplete = false;
						this._spelling.ShowDialog = false;
						this._spelling.Text = this._currentText;
						for (int wi = 0; wi < this._spelling.WordCount; wi++)
						{
							this._spelling.SpellCheck(wi, wi);
							if (stopAtFirst && (this._currentResult.Count > 0))
								break;
						}
					}
					return this._currentResult.ToArray();
				}
				catch
				{
					throw;
				}
				finally
				{
					this._currentReason = CallReasons.None;
					this._currentText = null;
					this._currentResult = null;
				}
			}

			public void Analyze(TextBox textBox)
			{
				this._currentResult = new List<Result>();
				this._currentReason = CallReasons.TextBox;
				this._currentTextBox = textBox;
				try
				{
					this._spelling.AlertComplete = true;
					this._spelling.ShowDialog = true;
					this._spelling.Text = this._currentTextBox.Text;
					this._spelling.ModalDialog = true;
					this._spelling.SpellCheck();
				}
				catch
				{
					throw;
				}
				finally
				{
					this._currentReason = CallReasons.None;
					this._currentResult = null;
					this._currentTextBox = null;
				}
			}

		}
	}
}
