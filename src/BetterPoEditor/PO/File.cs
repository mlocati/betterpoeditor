using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace BePoE.PO
{
	public partial class POFile
	{
		#region Events

		public delegate void DirtyChangedEventHandler(POFile poFile, bool newDirtyValue);
		public event DirtyChangedEventHandler DirtyChanged;

		#endregion

		#region Properties

		private IO.TextFile _textFile;
		public IO.TextFile TextFile
		{
			get
			{ return this._textFile; }
		}

		private HeaderEntry _header;
		public HeaderEntry Header
		{
			get
			{ return this._header; }
		}

		private CultureInfo _language;
		public CultureInfo Language
		{
			get
			{ return this._language; }
		}

		private Entry[] _entries;
		public Entry[] Entries
		{
			get
			{ return this._entries; }
		}

		private bool _dirty;
		public bool Dirty
		{
			get
			{ return this._dirty; }
		}
		internal void SetDirty(bool value)
		{
			if (this._dirty != value)
			{
				this._dirty = value;
				if (this.DirtyChanged != null)
					this.DirtyChanged(this, value);
			}
		}

		public string GetPluralTranslatedText(int pluralIndex, int pluralTot)
		{
			if ((this.Header != null) && (this.Header.PluralForms.HasValue) && (this.Header.PluralForms.Value == pluralTot) && (this.Header.PluralDescriptions != null))
				return this.Header.PluralDescriptions[pluralIndex];
			else if (pluralTot == 1)
				return string.Format("Translated");
			else
				return string.Format("Translated ({0:N0} of {1:N0})", pluralIndex + 1, pluralTot);
		}

		public readonly int TotalDataEntries;
		public int TranslatedItems
		{
			get
			{
				int s = 0;
				foreach (Entry entry in this._entries)
				{
					if (entry.Kind == Entry.Kinds.Standard)
					{
						DataEntry data = (DataEntry)entry;
						if (data.IsTranslated)
							s++;
					}
				}
				return s;
			}
		}
		#endregion


		#region Constructor

		public delegate CultureInfo LanguageGetter(CultureInfo suggestedLanguage);
		public POFile(IO.TextFile textFile)
			: this(textFile, null)
		{ }
		public POFile(IO.TextFile textFile, LanguageGetter languageGetter)
		{
			this._dirty = false;
			this._textFile = textFile;
			this._textFile.Saved += this.TextFile_Saved;
			List<IO.TextLine> group = null;
			List<Entry> entries = new List<Entry>();
			Regex rxNoSpaces = new Regex(@"\s+", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.Singleline);
			bool firstGroup = true;
			this._header = null;
			foreach (IO.TextLine line in textFile.Lines)
			{
				if (rxNoSpaces.Replace(line.Value, "").Length > 0)
				{
					if (group == null)
						group = new List<IO.TextLine>();
					group.Add(line);
				}
				else
				{
					if (group != null)
					{
						Entry entry = Entry.Parse(firstGroup, this, group);
						entries.Add(entry);
						if (entry is HeaderEntry)
							this._header = (HeaderEntry)entry;
						firstGroup = false;
						group = null;
					}
				}
			}
			if (group != null)
			{
				Entry entry = Entry.Parse(firstGroup, this, group);
				entries.Add(entry);
				if (entry is HeaderEntry)
					this._header = (HeaderEntry)entry;
				firstGroup = false;
				group = null;
			}
			this._entries = entries.ToArray();
			this._header = (this._entries.Length > 0) ? (this._entries[0] as HeaderEntry) : null;
			this._language = null;
			if ((this._header != null) && (this.Header.LanguageCode.Length > 0))
			{
				try
				{ this._language = new CultureInfo(this.Header.LanguageCode); }
				catch
				{ }
			}
			if (this._language == null)
			{
				if (languageGetter != null)
				{
					CultureInfo suggest = null;
					Match match = Regex.Match(Path.GetFileNameWithoutExtension(textFile.FileName), @"(^|\b)(?<code>[a-z]{2})($|[_-](?<code2>[a-z]{2})$)", RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.Singleline | RegexOptions.IgnoreCase);
					if (match.Success)
					{
						if (!string.IsNullOrEmpty(match.Groups["code2"].Value))
						{
							try
							{ suggest = new CultureInfo(string.Format("{0}-{1}", match.Groups["code"].Value.ToLowerInvariant(), match.Groups["code2"].Value.ToUpperInvariant())); }
							catch
							{ }
							if (suggest == null)
							{
								try
								{
									suggest = new CultureInfo(match.Groups["code2"].Value.ToLowerInvariant());
								}
								catch
								{ }
							}
						}
						if (suggest == null)
						{
							try
							{ suggest = new CultureInfo(match.Groups["code"].Value.ToLowerInvariant()); }
							catch
							{ }
						}
					}
					this._language = languageGetter(suggest);
					if (this._language == null)
						throw new OperationCanceledException();
				}
			}
			if (this._language == null)
				throw new Exception("Unable to determine the language.");
			this.TotalDataEntries = 0;
			foreach (Entry entry in this._entries)
				if (entry.Kind == Entry.Kinds.Standard)
					this.TotalDataEntries++;
		}

		#endregion

		private void TextFile_Saved(object sender, EventArgs e)
		{
			if (sender == this._textFile)
				this.SetDirty(false);
		}

	}
}
