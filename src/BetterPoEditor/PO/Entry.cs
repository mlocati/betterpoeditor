using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace BePoE.PO
{
	public abstract class Entry
	{
		protected const int MAX_LINE_LENGTH = 80;

		public enum Kinds
		{
			Header,
			Standard,
			Removed,
		}
		public abstract Kinds Kind { get;}
		public abstract bool PluralSource { get;}
		public abstract int NumTranslated { get;}

		private class FlagInfoAttribute : Attribute
		{
			public readonly string Text;
			public FlagInfoAttribute(string text)
			{
				this.Text = text;
			}
		}
		[Flags]
		public enum Flag : ulong
		{
			[FlagInfo("")]
			None = 0x0ul,
			[FlagInfo("fuzzy")]
			Fuzzy = 0x1ul,
			[FlagInfo("c-format")]
			C_Format = 0x2ul,
			[FlagInfo("no-c-format")]
			NoC_Format = 0x4ul,
			[FlagInfo("objc-format")]
			ObjectiveC_Format = 0x8ul,
			[FlagInfo("no-objc-format")]
			NoObjectiveC_Format = 0x10ul,
			[FlagInfo("sh-format")]
			Shell_Format = 0x20ul,
			[FlagInfo("no-sh-format")]
			NoShell_Format = 0x40ul,
			[FlagInfo("python-format")]
			Python_Format = 0x80ul,
			[FlagInfo("no-python-format")]
			NoPython_Format = 0x100ul,
			[FlagInfo("lisp-format")]
			Lisp_Format = 0x200ul,
			[FlagInfo("no-lisp-format")]
			NoLisp_Format = 0x400ul,
			[FlagInfo("elisp-format")]
			EmacsLisp_Format = 0x800ul,
			[FlagInfo("no-elisp-format")]
			NoEmacsLisp_Format = 0x1000ul,
			[FlagInfo("librep-format")]
			LibRep_Format = 0x2000ul,
			[FlagInfo("no-librep-format")]
			NoLibRep_Format = 0x4000ul,
			[FlagInfo("scheme-format")]
			Scheme_Format = 0x8000ul,
			[FlagInfo("no-scheme-format")]
			NoScheme_Format = 0x10000ul,
			[FlagInfo("smalltalk-format")]
			Smalltalk_Format = 0x20000ul,
			[FlagInfo("no-smalltalk-format")]
			NoSmalltalk_Format = 0x40000ul,
			[FlagInfo("java-format")]
			Java_Format = 0x80000ul,
			[FlagInfo("no-java-format")]
			NoJava_Format = 0x100000ul,
			[FlagInfo("csharp-format")]
			CSharp_Format = 0x200000ul,
			[FlagInfo("no-csharp-format")]
			NoCSharp_Format = 0x400000ul,
			[FlagInfo("awk-format")]
			Awk_Format = 0x800000ul,
			[FlagInfo("no-awk-format")]
			NoAwk_Format = 0x1000000ul,
			[FlagInfo("object-pascal-format")]
			ObjectPascal_Format = 0x2000000ul,
			[FlagInfo("no-object-pascal-format")]
			NoObjectPascal_Format = 0x4000000ul,
			[FlagInfo("ycp-format")]
			Ycp_Format = 0x8000000ul,
			[FlagInfo("no-ycp-format")]
			NoYcp_Format = 0x10000000ul,
			[FlagInfo("tcl-format")]
			Tcl_Format = 0x20000000ul,
			[FlagInfo("no-tcl-format")]
			NoTcl_Format = 0x40000000ul,
			[FlagInfo("perl-format")]
			Perl_Format = 0x80000000ul,
			[FlagInfo("no-perl-format")]
			NoPerl_Format = 0x100000000ul,
			[FlagInfo("perl-brace-format")]
			PerlBrace_Format = 0x200000000ul,
			[FlagInfo("no-perl-brace-format")]
			NoPerlBrace_Format = 0x400000000ul,
			[FlagInfo("php-format")]
			Php_Format = 0x800000000ul,
			[FlagInfo("no-php-format")]
			NoPhp_Format = 0x1000000000ul,
			[FlagInfo("gcc-internal-format")]
			GccSources_Format = 0x2000000000ul,
			[FlagInfo("no-gcc-internal-format")]
			NoGccSources_Format = 0x4000000000ul,
			[FlagInfo("gfc-internal-format")]
			GNUFortranCompilerSourcesInternal_Format = 0x8000000000ul,
			[FlagInfo("no-gfc-internal-format")]
			NoGNUFortranCompilerSourcesInternal_Format = 0x10000000000ul,
			[FlagInfo("qt-format")]
			Qt_Format = 0x20000000000ul,
			[FlagInfo("no-qt-format")]
			NoQt_Format = 0x40000000000ul,
			[FlagInfo("qt-plural-forma")]
			QtPluralForms_Format = 0x80000000000ul,
			[FlagInfo("no-qt-plural-forma")]
			NoQtPluralForms_Format = 0x100000000000ul,
			[FlagInfo("kde-format")]
			Kde_Format = 0x200000000000ul,
			[FlagInfo("no-kde-format")]
			NoKde_Format = 0x400000000000ul,
			[FlagInfo("boost-format")]
			Boost_Format = 0x800000000000ul,
			[FlagInfo("no-boost-format")]
			NoBoost_Format = 0x1000000000000ul,
		}
		private static Dictionary<Flag, FlagInfoAttribute> _flagInfo = null;
		[ReadOnly(true)]
		private static Dictionary<Flag, FlagInfoAttribute> FlagInfo
		{
			get
			{
				if (Entry._flagInfo == null)
				{
					Dictionary<Flag, FlagInfoAttribute> fi = new Dictionary<Flag, FlagInfoAttribute>();
					Type enumType = typeof(Flag);
					Type infoType = typeof(FlagInfoAttribute);
					foreach (Flag flag in Enum.GetValues(enumType))
						fi.Add(flag, (FlagInfoAttribute)enumType.GetMember(flag.ToString())[0].GetCustomAttributes(infoType, false)[0]);
					Entry._flagInfo = fi;
				}
				return Entry._flagInfo;
			}
		}

		public static Entry Parse(bool isTheFirstOne, POFile file, List<IO.TextLine> textLines)
		{
			List<PO.Line> lines = new List<PO.Line>(textLines.Count);
			PO.Line prevLine = null;
			foreach (IO.TextLine textLine in textLines)
			{
				try
				{
					lines.Add(prevLine = PO.Line.ParseLine(file, textLine, prevLine));
				}
				catch (Exception x)
				{
					Exception inner = x;
					while (inner.InnerException != null)
						inner = inner.InnerException;
					throw new Exception(string.Format("Error at line {1}{0}{2}{0}Error message: {3}", Environment.NewLine, file.TextFile.Lines.IndexOf(textLine) + 1, textLine.Value, inner.Message));
				}
			}
			try
			{
				if (isTheFirstOne)
				{
					List<PO.Line> linesTester = new List<PO.Line>(lines.Count);
					foreach (PO.Line l in lines)
					{
						bool skip = false;
						if (l.Type == Line.Types.Comment)
						{
							switch (((PO.CommentLine)l).Kind)
							{
								case CommentLine.Kinds.Flags:
								case CommentLine.Kinds.TranslatorComment:
									skip = true;
									break;
							}
						}
						if (!skip)
							linesTester.Add(l);
					}
					if (
						(linesTester.Count >= 2)
						&&
						(!linesTester[0].IsContinuation)
						&&
						((linesTester[0] as Line.IIndexedLine) == null)
						&&
						(linesTester[0] is DataLine)
						&&
						(((DataLine)linesTester[0]).Kind == DataLine.Kinds.ID)
						&&
						(!linesTester[1].IsContinuation)
						&&
						((linesTester[1] as Line.IIndexedLine) == null)
						&&
						(linesTester[1] is DataLine)
						&&
						(((DataLine)linesTester[1]).Kind == DataLine.Kinds.Translated)
						)
					{
						bool ok = true;
						for (int i = 2; ok && (i < linesTester.Count); i++)
							if (!linesTester[i].IsContinuation)
								ok = false;
						if (ok)
							return new HeaderEntry(lines);
					}
				}
				bool hasData = false, hasPluralData = false, hasRemoved = false, hasPluralRemoved = false;
				foreach (PO.Line line in lines)
				{
					switch (line.Type)
					{
						case Line.Types.Comment:
							{
								CommentLine.Kinds kind = ((CommentLine)line).Kind;
								switch (kind)
								{
									case CommentLine.Kinds.Removed_Context:
									case CommentLine.Kinds.Removed_ID:
									case CommentLine.Kinds.Removed_IDPlural:
									case CommentLine.Kinds.Removed_Translated:
									case CommentLine.Kinds.Removed_TranslatedIndexed:
										hasRemoved = true;
										switch (kind)
										{
											case CommentLine.Kinds.Removed_IDPlural:
											case CommentLine.Kinds.Removed_TranslatedIndexed:
												hasPluralRemoved = true;
												break;

										}
										break;
								}
							}
							break;
						case Line.Types.Data:
							{
								hasData = true;
								DataLine.Kinds kind = ((DataLine)line).Kind;
								switch (kind)
								{
									case DataLine.Kinds.IDPlural:
									case DataLine.Kinds.TranslatedIndexed:
										hasPluralData = true;
										break;
								}
							}
							break;
					}
				}
				if (hasData)
				{
					if (hasRemoved)
						throw new Exception("Found both removed and standard lines.");
					if (hasPluralData)
						return new PluralDataEntry(lines);
					else
						return new SingleDataEntry(lines);
				}
				else if (hasRemoved)
				{
					if (hasPluralRemoved)
						return new PluralRemovedEntry(lines);
					else
						return new SingleRemovedEntry(lines);
				}
				else
					throw new Exception("Unknown entry kind.");
			}
			catch (Exception x)
			{
				Exception inner = x;
				while (inner.InnerException != null)
					inner = inner.InnerException;
				throw new Exception(string.Format("Error at lines {1}~{2}:{0}{3}", Environment.NewLine, file.TextFile.Lines.IndexOf(textLines[0]) + 1, file.TextFile.Lines.IndexOf(textLines[textLines.Count - 1]) + 1, inner.Message));

			}
		}

		#region Properties

		protected POFile _file;
		public POFile File
		{
			get
			{ return this._file; }
		}
		private List<PO.Line> _lines;

		private List<PO.CommentLine> _translatorCommentLines;
		private string _translatorComment;
		public string TranslatorComment
		{
			get
			{ return this._translatorComment; }
			set
			{
				if (value == null)
					value = "";
				if (value.Equals(this._translatorComment, StringComparison.Ordinal))
					return;
				string[] parts = (value.Length == 0) ? new string[] { } : value.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
				int nOld = this._translatorCommentLines.Count;
				while (nOld > parts.Length)
				{
					this.RemoveLine(this._translatorCommentLines[--nOld]);
					this._translatorCommentLines.RemoveAt(nOld);
				}
				while (nOld < parts.Length)
				{
					PO.CommentLine newOne;
					if (nOld == 0)
						newOne = PO.CommentLine.CreateNew(this, CommentLine.Kinds.TranslatorComment, null, false, this._lines[0].Source, true, "");
					else
						newOne = PO.CommentLine.CreateNew(this, CommentLine.Kinds.TranslatorComment, null, false, this._translatorCommentLines[nOld - 1].Source, false, "");
					this._translatorCommentLines.Add(newOne);
					nOld++;
				}

				for (int i = 0; i < parts.Length; i++)
				{
					this._translatorCommentLines[i].SetText(parts[i]);
				}
				this._translatorComment = value;
				this.File.SetDirty(true);
			}
		}

		private List<PO.CommentLine> _extractedCommentLines;
		private string _extractedComment;
		public string ExtractedComment
		{
			get
			{ return this._extractedComment; }
		}

		private List<PO.CommentLine> _referenceLines;
		private string[] _reference;
		[ReadOnly(true)]
		public string[] Reference
		{
			get
			{ return this._reference; }
		}

		private List<PO.CommentLine> _flagsLines;
		private Flag _flags;
		public Flag Flags
		{
			get
			{ return this._flags; }
		}
		protected void SetFlag(Flag flag, bool value)
		{
			Flag flags = value ? (this._flags | flag) : (this._flags & ~flag);
			if (this._flags == flags)
				return;
			int nOld = this._flagsLines.Count;
			if (flags == Flag.None)
			{
				while (nOld > 0)
				{
					this.RemoveLine(this._flagsLines[--nOld]);
					this._flagsLines.RemoveAt(nOld);
				}
			}
			else
			{
				if (nOld == 0)
				{
					IO.TextLine first = null;
					if ((first == null) && (this._translatorCommentLines.Count > 0))
						first = this._translatorCommentLines[this._translatorCommentLines.Count - 1].Source;
					if ((first == null) && (this._extractedCommentLines.Count > 0))
						first = this._extractedCommentLines[this._extractedCommentLines.Count - 1].Source;
					if ((first == null) && (this._referenceLines.Count > 0))
						first = this._referenceLines[this._referenceLines.Count - 1].Source;
					PO.CommentLine newOne;
					if (first != null)
						newOne = PO.CommentLine.CreateNew(this, CommentLine.Kinds.Flags, null, false, first, false, "");
					else
						newOne = PO.CommentLine.CreateNew(this, CommentLine.Kinds.Flags, null, false, this._lines[0].Source, true, "");
					this._flagsLines.Add(newOne);
					nOld++;
				}
				else
				{
					while (nOld > 1)
					{
						this.RemoveLine(this._flagsLines[--nOld]);
						this._flagsLines.RemoveAt(nOld);
					}
				}
				StringBuilder newText = null;
				foreach (Flag f in Enum.GetValues(typeof(Flag)))
				{
					if (f == Flag.None)
						continue;
					if ((flags & f) == f)
					{
						if (newText == null)
							newText = new StringBuilder();
						else
							newText.Append(',');
						newText.Append(FlagInfo[f].Text);
					}
				}
				this._flagsLines[0].SetText(newText.ToString());
			}
			this._flags = flags;
			this.File.SetDirty(true);
		}
		public bool Fuzzy
		{
			get
			{ return (this._flags & Flag.Fuzzy) == Flag.Fuzzy; }
		}

		private List<PO.CommentLine> _previousUntranslatedContextLines;
		private string _previousUntranslatedContext;
		public string PreviousUntranslatedContext
		{
			get
			{ return this._previousUntranslatedContext; }
		}

		private List<PO.CommentLine> _previousUntranslatedStringLines;
		private string _previousUntranslatedString;
		public string PreviousUntranslatedString
		{
			get
			{ return this._previousUntranslatedString; }
		}

		private List<PO.CommentLine> _previousUntranslatedStringPluralLines;
		private string _previousUntranslatedStringPlural;
		public string PreviousUntranslatedStringPlural
		{
			get
			{ return this._previousUntranslatedStringPlural; }
		}

		#endregion

		internal void RemoveLine(PO.Line line)
		{
			if (line == null)
				throw new ArgumentNullException("line");
			int myIndex = this._lines.IndexOf(line);
			if (myIndex < 0)
				throw new Exception("Line not found.");
			if (!this._file.TextFile.Lines.Remove(line.Source))
				throw new Exception("Text line not found.");
			this._lines.RemoveAt(myIndex);
		}

		internal void LineAdded(PO.Line line)
		{
			if (line == null)
				throw new ArgumentNullException("line");
			if (this._lines.Contains(line))
				throw new ArgumentException("The line is already in the file.", "line");
			int newSourceIndex = this._file.TextFile.Lines.IndexOf(line.Source);
			if (newSourceIndex < 0)
				throw new ArgumentException("The text line is not in the text file.", "line");
			int oldStartSourceIndex = this._file.TextFile.Lines.IndexOf(this._lines[0].Source);
			if (newSourceIndex < oldStartSourceIndex)
			{
				if (newSourceIndex == (oldStartSourceIndex - 1))
					this._lines.Insert(0, line);
				else
					throw new Exception("New line before the start of the entry block.");
			}
			else
			{
				int oldEndSourceIndex = this._file.TextFile.Lines.IndexOf(this._lines[this._lines.Count - 1].Source);
				if (newSourceIndex > oldEndSourceIndex)
				{
					if (newSourceIndex == (oldEndSourceIndex + 1))
						this._lines.Add(line);
					else
						throw new Exception("New line after the end of the entry block.");
				}
				else
					this._lines.Insert(newSourceIndex - oldStartSourceIndex, line);
			}
		}

		internal Entry(List<PO.Line> lines)
		{
			StringBuilder sb;

			if ((lines == null) || (lines.Count == 0))
				throw new ArgumentNullException("lines");
			this._lines = lines;
			this._file = this._lines[0].File;
			this._translatorCommentLines = new List<PO.CommentLine>();
			this._extractedCommentLines = new List<PO.CommentLine>();
			this._referenceLines = new List<PO.CommentLine>();
			this._flagsLines = new List<PO.CommentLine>();
			this._previousUntranslatedContextLines = new List<PO.CommentLine>();
			this._previousUntranslatedStringLines = new List<PO.CommentLine>();
			this._previousUntranslatedStringPluralLines = new List<PO.CommentLine>();
			PO.Line prevLine = null;
			foreach (PO.Line line in this._lines)
			{
				switch (line.Type)
				{
					case Line.Types.Comment:
						if ((prevLine != null) && (prevLine.Type == Line.Types.Data))
							throw new Exception("Comment line after data line.");
						PO.CommentLine comment = (PO.CommentLine)line;
						switch (comment.Kind)
						{
							case CommentLine.Kinds.TranslatorComment:
								this._translatorCommentLines.Add(comment);
								break;
							case CommentLine.Kinds.ExtractedComment:
								this._extractedCommentLines.Add(comment);
								break;
							case CommentLine.Kinds.Reference:
								this._referenceLines.Add(comment);
								break;
							case CommentLine.Kinds.Flags:
								this._flagsLines.Add(comment);
								break;
							case CommentLine.Kinds.PreviousUntraslated_Context:
								if ((this._previousUntranslatedContextLines.Count > 0))
								{
									if (!comment.IsContinuation)
										throw new Exception("Previous untranslated context started more than once.");
								}
								this._previousUntranslatedContextLines.Add(comment);
								break;
							case CommentLine.Kinds.PreviousUntraslated_ID:
								if (this._previousUntranslatedStringLines.Count > 0)
								{
									if (!comment.IsContinuation)
										throw new Exception("Previous untranslated string started more than once.");
								}
								this._previousUntranslatedStringLines.Add(comment);
								break;
							case CommentLine.Kinds.PreviousUntraslated_IDPlural:
								if (this._previousUntranslatedStringPluralLines.Count > 0)
								{
									if (!comment.IsContinuation)
										throw new Exception("Previous untranslated plural string started more than once.");
								}
								this._previousUntranslatedStringPluralLines.Add(comment);
								break;
						}
						break;
				}
				prevLine = line;
			}

			sb = null;
			foreach (PO.CommentLine comment in this._translatorCommentLines)
			{
				if (sb == null)
					sb = new StringBuilder();
				else
					sb.AppendLine();
				sb.Append(comment.Text);
			}
			this._translatorComment = (sb == null) ? "" : sb.ToString();

			sb = null;
			foreach (PO.CommentLine comment in this._extractedCommentLines)
			{
				if (sb == null)
					sb = new StringBuilder();
				else
					sb.AppendLine();
				sb.Append(comment.Text);
			}
			this._extractedComment = (sb == null) ? "" : sb.ToString();

			sb = null;
			foreach (PO.CommentLine comment in this._referenceLines)
			{
				if (sb == null)
					sb = new StringBuilder();
				else
					sb.Append(' ');
				sb.Append(comment.Text);
			}
			if (sb == null)
			{
				this._reference = new string[] { };
			}
			else
			{
				string s = sb.ToString().Trim(' ', '\t', '\r', '\n');
				if (s.Length == 0)
					this._reference = new string[] { };
				else if (s.IndexOf('"') < 0)
					this._reference = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				else
				{
					List<string> reference = new List<string>();
					while (s.Length > 0)
					{
						int p = s.IndexOf('"');
						if (p < 0)
						{
							reference.AddRange(s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
							s = "";
						}
						else
						{
							string s2;
							int q = s.IndexOf('"', p + 1);
							if (q < 0)
								throw new Exception("Malformed reference.");
							s2 = (p == 0) ? "" : s.Substring(0, p).TrimStart(' ', '\t', '\r', '\n');
							if (s2.Length > 0)
								reference.AddRange(s2.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
							s2 = (q == (p + 1)) ? "" : s.Substring(p + 1, q - p - 1).Trim(' ', '\t', '\r', '\n');
							if (s2.Length > 0)
								reference.AddRange(s2.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
							s = (q == (s.Length - 1)) ? "" : s.Substring(q + 1).TrimStart(' ', '\t', '\r', '\n');
						}
					}
					this._reference = reference.ToArray();
				}
			}

			sb = null;
			foreach (PO.CommentLine comment in this._flagsLines)
			{
				if (sb == null)
					sb = new StringBuilder();
				else
					sb.Append(',');
				sb.Append(comment.Text.Trim(' ', '\t'));
			}
			this._flags = Flag.None;
			if (sb != null)
			{
				foreach (string flag in sb.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					string flagTrimmed = flag.Trim();
					Flag? found = null;
					foreach (KeyValuePair<Flag, FlagInfoAttribute> kv in Entry.FlagInfo)
					{
						if (flagTrimmed.Equals(kv.Value.Text, StringComparison.Ordinal))
						{
							found = kv.Key;
							break;
						}
					}
					if (!found.HasValue)
					{
						throw new Exception(string.Format("Unknown flag: {0}", flagTrimmed));
					}
					this._flags |= found.Value;
				}
			}

			if (this._previousUntranslatedContextLines.Count > 0)
			{
				if (this._previousUntranslatedStringLines.Count == 0)
					throw new Exception("Previous untranslated context without untranslated text.");
			}
			if (this._previousUntranslatedStringPluralLines.Count > 0)
			{
				if (this._previousUntranslatedStringLines.Count == 0)
					throw new Exception("Plural untranslated text without singular untranslated text.");
			}

			sb = new StringBuilder();
			foreach (PO.CommentLine comment in this._previousUntranslatedContextLines)
			{
				sb.Append(Entry.UnescapeText(comment.Text));
			}
			this._previousUntranslatedContext = sb.ToString();

			sb = new StringBuilder();
			foreach (PO.CommentLine comment in this._previousUntranslatedStringLines)
			{
				sb.Append(Entry.UnescapeText(comment.Text));
			}
			this._previousUntranslatedString = sb.ToString();

			sb = new StringBuilder();
			foreach (PO.CommentLine comment in this._previousUntranslatedStringPluralLines)
			{
				sb.Append(Entry.UnescapeText(comment.Text));
			}
			this._previousUntranslatedStringPlural = sb.ToString();

		}

		protected static string UnescapeText(string text)
		{
			if (string.IsNullOrEmpty(text))
				return "";
			text = text.Replace(@"\r\n", @"\n");
			StringBuilder ris = new StringBuilder();
			int p0 = 0;
			for (; ; )
			{
				int p = text.IndexOf('\\', p0);
				if (p < 0)
				{
					if (p0 == 0)
						return text;
					else
						ris.Append(text.Substring(p0));
					break;
				}
				if (p > p0)
					ris.Append(text.Substring(p0, p - p0));
				if (p == (text.Length - 1))
					throw new Exception("Escape char without escaped char.");
				switch (text[p + 1])
				{
					case '\\':
					case '"':
						ris.Append(text[p + 1]);
						break;
					case 'n':
					case 'r':
						ris.Append(Environment.NewLine);
						break;
					case 't':
						ris.Append('\t');
						break;
					default:
						throw new Exception(string.Format("Unknown escaped char ({0}).", text[p + 1]));
				}
				p0 = p + 2;
				if (p0 == (text.Length))
					break;
			}
			return ris.ToString();
		}

		protected static string EscapeText(string text)
		{
			if (string.IsNullOrEmpty(text))
				return "";
			text = text.Replace("\r\n", "\n").Replace('\r', '\n');
			StringBuilder ris = new StringBuilder();
			foreach (char c in text)
			{
				switch (c)
				{
					case '\\':
						ris.Append("\\\\");
						break;
					case '"':
						ris.Append("\\\"");
						break;
					case '\n':
						ris.Append("\\n");
						break;
					case '\t':
						ris.Append("\\t");
						break;
					default:
						ris.Append(c);
						break;
				}
			}
			return ris.ToString();
		}
	}


	public abstract class RemovedEntry : Entry
	{
		private List<PO.CommentLine> _removedContextLines;
		private string _removedContext;
		public string RemovedContext
		{
			get
			{ return this._removedContext; }
		}

		private List<PO.CommentLine> _removedIDLines;
		private string _removedID;
		public string RemovedID
		{
			get
			{ return this._removedID; }
		}

		public override Kinds Kind
		{
			get { return Kinds.Removed; }
		}
		internal RemovedEntry(List<PO.Line> lines)
			: base(lines)
		{
			StringBuilder sb;

			this._removedContextLines = new List<PO.CommentLine>();
			this._removedIDLines = new List<PO.CommentLine>();
			foreach (PO.Line line in lines)
			{
				switch (line.Type)
				{
					case Line.Types.Comment:
						PO.CommentLine comment = (PO.CommentLine)line;
						switch (comment.Kind)
						{
							case CommentLine.Kinds.Removed_Context:
								if (this._removedContextLines.Count > 0)
								{
									if (!comment.IsContinuation)
										throw new Exception("Removed context started more than once.");
								}
								this._removedContextLines.Add(comment);
								break;
							case CommentLine.Kinds.Removed_ID:
								if (this._removedIDLines.Count > 0)
								{
									if (!comment.IsContinuation)
										throw new Exception("Removed string ID started more than once.");
								}
								this._removedIDLines.Add(comment);
								break;
						}
						break;
					case Line.Types.Data:
						throw new Exception("Data line in a comment-only block.");
				}
			}

			if (this._removedIDLines.Count == 0)
				throw new Exception("Removed text entry without removed ID lines.");

			sb = new StringBuilder();
			foreach (PO.CommentLine comment in this._removedContextLines)
			{
				sb.Append(Entry.UnescapeText(comment.Text));
			}
			this._removedContext = sb.ToString();

			sb = new StringBuilder();
			foreach (PO.CommentLine comment in this._removedIDLines)
			{
				sb.Append(Entry.UnescapeText(comment.Text));
			}
			this._removedID = sb.ToString();
		}
	}
	public class SingleRemovedEntry : RemovedEntry
	{
		public override bool PluralSource
		{
			get { return false; }
		}
		public override int NumTranslated
		{
			get { return 1; }
		}
		private List<PO.CommentLine> _removedTranslatedLines;
		private string _removedTranslated;
		public string RemovedTranslated
		{
			get
			{ return this._removedTranslated; }
		}
		internal SingleRemovedEntry(List<PO.Line> lines)
			: base(lines)
		{
			StringBuilder sb;

			this._removedTranslatedLines = new List<CommentLine>();
			foreach (PO.Line line in lines)
			{
				switch (line.Type)
				{
					case Line.Types.Comment:
						PO.CommentLine comment = (PO.CommentLine)line;
						switch (comment.Kind)
						{
							case CommentLine.Kinds.Removed_Translated:
								if (this._removedTranslatedLines.Count > 0)
								{
									if (!comment.IsContinuation)
										throw new Exception("Removed string started more than once.");
								}
								this._removedTranslatedLines.Add(comment);
								break;
							case CommentLine.Kinds.Removed_IDPlural:
								throw new Exception("Plural ID in single-id removed entry.");
							case CommentLine.Kinds.Removed_TranslatedIndexed:
								throw new Exception("Indexed translation in single-id removed entry.");
						}
						break;
				}
			}

			if (this._removedTranslatedLines.Count == 0)
				throw new Exception("Removed text entry without removed translated lines.");

			sb = new StringBuilder();
			foreach (PO.CommentLine comment in this._removedTranslatedLines)
			{
				sb.Append(Entry.UnescapeText(comment.Text));
			}
			this._removedTranslated = sb.ToString();

		}
	}
	public class PluralRemovedEntry : RemovedEntry
	{
		public override bool PluralSource
		{
			get { return true; }
		}
		public override int NumTranslated
		{
			get { return this._removedTranslated.Length; }
		}

		private List<PO.CommentLine> _removedIDPluralLines;
		private string _removedIDPlural;
		public string RemovedIDPlural
		{
			get
			{ return this._removedIDPlural; }
		}

		private List<PO.IndexedCommentLine>[] _removedTranslatedLines;
		private string[] _removedTranslated;
		[ReadOnly(true)]
		public string[] RemovedTranslated
		{
			get
			{ return this._removedTranslated; }
		}
		internal PluralRemovedEntry(List<PO.Line> lines)
			: base(lines)
		{
			StringBuilder sb;
			this._removedIDPluralLines = new List<CommentLine>();
			List<List<PO.IndexedCommentLine>> removedTranslatedLines = new List<List<IndexedCommentLine>>();
			foreach (PO.Line line in lines)
			{
				switch (line.Type)
				{
					case Line.Types.Comment:
						PO.CommentLine comment = (PO.CommentLine)line;
						switch (comment.Kind)
						{
							case CommentLine.Kinds.Removed_IDPlural:
								if (this._removedIDPluralLines.Count > 0)
								{
									if (!comment.IsContinuation)
										throw new Exception("Removed plural ID started more than once.");
								}
								this._removedIDPluralLines.Add(comment);
								break;
							case CommentLine.Kinds.Removed_TranslatedIndexed:
								IndexedCommentLine indexedComment = (PO.IndexedCommentLine)comment;
								while (removedTranslatedLines.Count <= indexedComment.Index)
									removedTranslatedLines.Add(new List<IndexedCommentLine>());
								if (removedTranslatedLines[indexedComment.Index].Count > 0)
								{
									if (!indexedComment.IsContinuation)
										throw new Exception("Removed string[] started more than once.");
								}
								removedTranslatedLines[indexedComment.Index].Add(indexedComment);
								break;
							case CommentLine.Kinds.Removed_Translated:
								throw new Exception("Plural ID in single-id removed entry.");
						}
						break;
				}
			}

			if (this._removedIDPluralLines.Count == 0)
				throw new Exception("Plural removed text entry without plural ID lines.");

			sb = new StringBuilder();
			foreach (PO.CommentLine comment in this._removedIDPluralLines)
			{
				sb.Append(Entry.UnescapeText(comment.Text));
			}
			this._removedIDPlural = sb.ToString();

			if (removedTranslatedLines.Count == 0)
				throw new Exception("Removed text entry without removed translated lines.");

			for (int i = 0; i < removedTranslatedLines.Count; i++)
				if (removedTranslatedLines[i].Count == 0)
					throw new Exception(string.Format("Removed text entries found until level {0:N0}, but level {1:N0} is empty.", removedTranslatedLines.Count, i + 1));

			this._removedTranslatedLines = removedTranslatedLines.ToArray();
			this._removedTranslated = new string[this._removedTranslatedLines.Length];
			for (int i = 0; i < removedTranslatedLines.Count; i++)
			{
				sb = new StringBuilder();
				foreach (PO.CommentLine comment in this._removedTranslatedLines[i])
				{
					sb.Append(Entry.UnescapeText(comment.Text));
				}
				this._removedTranslated[i] = sb.ToString();
			}
		}
	}

	public abstract class DataEntry : Entry
	{
		public void set_Fuzzy(bool value)
		{
			base.SetFlag(Flag.Fuzzy, value);
		}
		protected static string[] PrepareTranslated(int? index, string value)
		{
			string saveValue = Entry.EscapeText(value);
			List<string> parts = new List<string>();
			if ((saveValue.Length + (index.HasValue ? string.Format("msgstr[{0}] \"\"", index.Value) : "msgstr \"\"").Length) <= Entry.MAX_LINE_LENGTH)
				parts.Add(saveValue);
			else
			{
				parts.Add("");
				int maxPartLen = Entry.MAX_LINE_LENGTH - "\"\"".Length;
				int minPartLen = Math.Max(1, Entry.MAX_LINE_LENGTH >> 1);
				while (saveValue.Length > maxPartLen)
				{
					string s2 = saveValue.Substring(0, maxPartLen);
					int pLast = -1;
					for (int parseStep = 0; (pLast < 0) && (parseStep < 2); parseStep++)
					{
						for (int p = (s2.Length - 1); (pLast < 0) && (p > minPartLen); p--)
						{
							char c = s2[p];
							if (c == '\\')
								continue;
							switch (parseStep)
							{
								case 0:
									if (char.IsWhiteSpace(c))
										pLast = p;
									break;
								case 1:
									switch (char.GetUnicodeCategory(c))
									{
										case UnicodeCategory.ClosePunctuation:
										case UnicodeCategory.SpaceSeparator:
											pLast = p;
											break;
									}
									break;
							}
						}
					}
					if (pLast > 0)
					{
						s2 = s2.Substring(0, pLast + 1);
					}
					parts.Add(s2);
					saveValue = saveValue.Substring(s2.Length);
				}
				if (saveValue.Length > 0)
					parts.Add(saveValue);
			}
			return parts.ToArray();
		}
		internal void SetTranslatedLines(IList translatedLines, int? index, string[] parts)
		{
			int nOld = translatedLines.Count;
			if ((nOld < 1) || (parts == null) || (parts.Length < 1))
				throw new Exception();
			while (nOld > parts.Length)
			{
				this.RemoveLine((PO.DataLine)translatedLines[--nOld]);
				translatedLines.RemoveAt(nOld);
			}
			while (nOld < parts.Length)
			{
				translatedLines.Add(PO.DataLine.CreateNew(this, index.HasValue ? DataLine.Kinds.TranslatedIndexed : DataLine.Kinds.Translated, index, true, ((PO.DataLine)translatedLines[nOld - 1]).Source, false, ""));
				nOld++;
			}
			for (int i = 0; i < parts.Length; i++)
			{
				((PO.DataLine)translatedLines[i]).SetText(parts[i]);
			}
		}

		public abstract bool IsTranslated { get;}
		public virtual bool IsHeader
		{
			get
			{ return false; }
		}
		private List<PO.DataLine> _contextLines;
		private string _context;
		public string Context
		{
			get
			{ return this._context; }
		}

		private List<PO.DataLine> _idLines;
		private string _id;
		public string ID
		{
			get
			{ return this._id; }
		}

		public override Kinds Kind
		{
			get { return Kinds.Standard; }
		}

		internal DataEntry(List<PO.Line> lines)
			: base(lines)
		{
			StringBuilder sb;

			this._contextLines = new List<PO.DataLine>();
			this._idLines = new List<PO.DataLine>();
			foreach (PO.Line line in lines)
			{
				switch (line.Type)
				{
					case Line.Types.Data:
						PO.DataLine data = (PO.DataLine)line;
						switch (data.Kind)
						{
							case DataLine.Kinds.Context:
								if (this._contextLines.Count > 0)
								{
									if (!data.IsContinuation)
										throw new Exception("Context started more than once.");
								}
								this._contextLines.Add(data);
								break;
							case DataLine.Kinds.ID:
								if (this._idLines.Count > 0)
								{
									if (!data.IsContinuation)
										throw new Exception("ID started more than once.");
								}
								this._idLines.Add(data);
								break;
						}
						break;
					case Line.Types.Comment:
						PO.CommentLine comment = (PO.CommentLine)line;
						switch (comment.Kind)
						{
							case CommentLine.Kinds.Removed_Context:
								throw new Exception("Removed context line in data entry.");
							case CommentLine.Kinds.Removed_ID:
								throw new Exception("Removed ID line in data entry.");
							case CommentLine.Kinds.Removed_IDPlural:
								throw new Exception("Removed plural ID line in data entry.");
							case CommentLine.Kinds.Removed_Translated:
								throw new Exception("Removed translated line in data entry.");
							case CommentLine.Kinds.Removed_TranslatedIndexed:
								throw new Exception("Removed indexed translated line in data entry.");
						}
						break;
				}
			}

			if (this._idLines.Count == 0)
				throw new Exception("Text entry withoutID lines.");

			sb = new StringBuilder();
			foreach (PO.DataLine data in this._contextLines)
			{
				sb.Append(Entry.UnescapeText(data.Text));
			}
			this._context = sb.ToString();

			sb = new StringBuilder();
			foreach (PO.DataLine data in this._idLines)
			{
				sb.Append(Entry.UnescapeText(data.Text));
			}
			this._id = sb.ToString();
		}

	}
	public class SingleDataEntry : DataEntry
	{
		public override bool PluralSource
		{
			get { return false; }
		}
		public override int NumTranslated
		{
			get { return 1; }
		}

		private List<PO.DataLine> _translatedLines;
		private string _translated;
		public string Translated
		{
			get
			{ return this._translated; }
			set
			{
				if (value == null)
					value = "";
				if (value.Equals(this._translated, StringComparison.Ordinal))
					return;

				this.SetTranslatedLines(this._translatedLines, null, DataEntry.PrepareTranslated(null, value));
				this._translated = value;
				this.File.SetDirty(true);
			}
		}
		public override bool IsTranslated
		{
			get { return this._translated.Length > 0; }
		}
		internal SingleDataEntry(List<PO.Line> lines)
			: base(lines)
		{
			StringBuilder sb;

			this._translatedLines = new List<PO.DataLine>();
			foreach (PO.Line line in lines)
			{
				switch (line.Type)
				{
					case Line.Types.Data:
						PO.DataLine data = (PO.DataLine)line;
						switch (data.Kind)
						{
							case DataLine.Kinds.Translated:
								if (this._translatedLines.Count > 0)
								{
									if (!data.IsContinuation)
										throw new Exception("Translated string started more than once.");
								}
								this._translatedLines.Add(data);
								break;
							case DataLine.Kinds.IDPlural:
								throw new Exception("Plural ID in single-id data entry.");
							case DataLine.Kinds.TranslatedIndexed:
								throw new Exception("Indexed translation in single-id data entry.");
						}
						break;
				}
			}

			if (this._translatedLines.Count == 0)
				throw new Exception("Translated text entry without data lines.");

			sb = new StringBuilder();
			foreach (PO.DataLine data in this._translatedLines)
			{
				sb.Append(Entry.UnescapeText(data.Text));
			}
			this._translated = sb.ToString();

		}
	}
	public class PluralDataEntry : DataEntry
	{
		private bool _translationIsFake;
		public override bool PluralSource
		{
			get
			{ return true; }
		}
		public override int NumTranslated
		{
			get
			{ return this._translationIsFake ? this.File.Header.PluralForms.Value : this._translated.Length; }
		}

		private List<PO.DataLine> _idPluralLines;
		private string _idPlural;
		public string IDPlural
		{
			get
			{ return this._idPlural; }
		}

		private List<PO.IndexedDataLine>[] _translatedLines;
		private string[] _translated;
		public override bool IsTranslated
		{
			get
			{
				for (int i = 0; i < this._translated.Length; i++)
					if (this._translated[i].Length == 0)
						return false;
				return true;
			}
		}
		public bool NoTranslatedAtAll
		{
			get
			{
				for (int i = 0; i < this._translated.Length; i++)
					if (this._translated[i].Length > 0)
						return false;
				return true;
			}
		}
		public string get_Translated(int index)
		{
			return this._translationIsFake ? "" : this._translated[index];
		}
		public void set_Translated(int index, string value)
		{
			if (value == null)
				value = "";
			if (value.Equals(this.get_Translated(index), StringComparison.Ordinal))
				return;
			if (this._translationIsFake)
			{
				int oldNumTranslatedLines = this._translatedLines.Length;
				int newNumTranslatedLines = this.File.Header.PluralForms.Value;
				for (int i = (oldNumTranslatedLines - 1); i >= newNumTranslatedLines; i--)
				{
					int n = this._translatedLines[i].Count;
					while (n > 0)
					{
						this.RemoveLine(this._translatedLines[i][--n]);
						this._translatedLines[i].RemoveAt(n);
					}
				}
				Array.Resize<List<PO.IndexedDataLine>>(ref this._translatedLines, newNumTranslatedLines);
				for (int i = oldNumTranslatedLines; i < newNumTranslatedLines; i++)
				{
					this._translatedLines[i] = new List<IndexedDataLine>();
					this._translatedLines[i].Add((IndexedDataLine)PO.IndexedDataLine.CreateNew(this, DataLine.Kinds.TranslatedIndexed, i, false, this._translatedLines[i - 1][this._translatedLines[i - 1].Count - 1].Source, false, ""));
				}
				this._translated = new string[newNumTranslatedLines];
				for (int i = 0; i < newNumTranslatedLines; i++)
					this._translated[i] = "";
				this._translationIsFake = false;
			}
			this.SetTranslatedLines(this._translatedLines[index], index, DataEntry.PrepareTranslated(index, value));
			this._translated[index] = value;
			this.File.SetDirty(true);
		}
		internal PluralDataEntry(List<PO.Line> lines)
			: base(lines)
		{
			StringBuilder sb;

			this._idPluralLines = new List<PO.DataLine>();
			List<List<PO.IndexedDataLine>> translatedLines = new List<List<PO.IndexedDataLine>>();
			foreach (PO.Line line in lines)
			{
				switch (line.Type)
				{
					case Line.Types.Data:
						PO.DataLine data = (PO.DataLine)line;
						switch (data.Kind)
						{
							case DataLine.Kinds.IDPlural:
								if (this._idPluralLines.Count > 0)
								{
									if (!data.IsContinuation)
										throw new Exception("Plural ID started more than once.");
								}
								this._idPluralLines.Add(data);
								break;
							case DataLine.Kinds.TranslatedIndexed:
								IndexedDataLine indexedData = (PO.IndexedDataLine)data;
								while (translatedLines.Count <= indexedData.Index)
									translatedLines.Add(new List<IndexedDataLine>());
								if (translatedLines[indexedData.Index].Count > 0)
								{
									if (!indexedData.IsContinuation)
										throw new Exception("Translated string[] started more than once.");
								}
								translatedLines[indexedData.Index].Add(indexedData);
								break;
							case DataLine.Kinds.Translated:
								throw new Exception("Plural ID in single-id translated entry.");
						}
						break;
				}
			}

			if (this._idPluralLines.Count == 0)
				throw new Exception("Plural text entry without plural ID lines.");

			sb = new StringBuilder();
			foreach (PO.DataLine comment in this._idPluralLines)
			{
				sb.Append(Entry.UnescapeText(comment.Text));
			}
			this._idPlural = sb.ToString();

			if (translatedLines.Count == 0)
				throw new Exception("Entry without translated lines.");

			for (int i = 0; i < translatedLines.Count; i++)
				if (translatedLines[i].Count == 0)
					throw new Exception(string.Format("Entries found until level {0:N0}, but level {1:N0} is empty.", translatedLines.Count, i + 1));

			this._translatedLines = translatedLines.ToArray();
			this._translated = new string[this._translatedLines.Length];
			for (int i = 0; i < translatedLines.Count; i++)
			{
				sb = new StringBuilder();
				foreach (PO.DataLine data in this._translatedLines[i])
				{
					sb.Append(Entry.UnescapeText(data.Text));
				}
				this._translated[i] = sb.ToString();
			}
			this._translationIsFake = (this.File.Header != null) && this.File.Header.PluralForms.HasValue && (File.Header.PluralForms.Value != this.NumTranslated) && this.NoTranslatedAtAll;
		}
	}

	public class HeaderEntry : SingleDataEntry
	{
		public override bool IsHeader
		{
			get
			{ return true; }
		}

		public override Kinds Kind
		{
			get
			{ return Kinds.Header; }
		}
		private Dictionary<string, string> _keys;
		[ReadOnly(true)]
		public Dictionary<string, string> Keys
		{
			get
			{ return this._keys; }
		}

		private int? _pluralForms;
		public int? PluralForms
		{
			get
			{
				if (this._pluralForms.HasValue)
					return this._pluralForms.Value;
				else
					return null;
			}
		}
		private string[] _pluralDescriptions;
		[ReadOnly(true)]
		public string[] PluralDescriptions
		{
			get
			{ return this._pluralDescriptions; }
		}

		private string _languageCode;
		public string LanguageCode
		{
			get
			{ return this._languageCode; }
		}
		private static string DescribePluralCase(string s)
		{
			if (string.IsNullOrEmpty(s))
				return s;
			Match match;
			match = Regex.Match(s, @"^n\s*(?<how>(<|<=|==|!=|>=|>))\s*(?<n>\d+)$", RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.Singleline);
			if (match.Success)
			{
				try
				{
					string s2 = Utils.Text.NumberToName(int.Parse(match.Groups["n"].Value));
					switch (match.Groups["how"].Value)
					{
						case "<":
							s2 = string.Format("Less than {0}", s2);
							break;
						case "<=":
							s2 = string.Format("Less than or equals to {0}", s2);
							break;
						case "==":
							break;
						case "!=":
							s2 = string.Format("Not {0}", s2);
							break;
						case ">=":
							s2 = string.Format("Greater than or equals to {0}", s2);
							break;
						case ">":
							s2 = string.Format("Greater than {0}", s2);
							break;
						default:
							s2 = null;
							break;
					}
					if (s2 != null)
						return Utils.Text.Capitalize(s2);
				}
				catch
				{ }
			}
			return s.Replace("&&", " and ").Replace("||", " or ");
		}
		internal HeaderEntry(List<PO.Line> lines)
			: base(lines)
		{
			this._keys = new Dictionary<string, string>();
			foreach (string keyLine in this.Translated.Replace("\r\n", "\n").Replace('\r', '\n').Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
			{
				int p = keyLine.IndexOf(':');
				switch (p)
				{
					case -1:
						this._keys.Add(keyLine, "");
						break;
					case 0:
						this._keys.Add("", keyLine);
						break;
					default:
						this._keys.Add(keyLine.Substring(0, p), keyLine.Substring(p + 1).TrimStart());
						break;
				}
			}

			#region Plural
			this._pluralForms = null;
			this._pluralDescriptions = null;
			if (this._keys.ContainsKey("Plural-Forms"))
			{
				Match match = Regex.Match(this._keys["Plural-Forms"], @"(^|;\s*)nplurals\s*=\s*(?<nplurals>[0-9]+)\s*;\s*plural\s*=\s*(?<plural_chooser>[^;]+)(;|$)", RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.Singleline);
				if (match.Success)
				{
					int np;
					try
					{ np = int.Parse(match.Groups["nplurals"].Value); }
					catch
					{ np = -1; }

					if (np >= 1)
					{
						this._pluralForms = np;
						if (np == 1)
						{
							this._pluralDescriptions = new string[] { "Translated" };
						}
						else
						{
							string pluralChooserSource = Regex.Replace(match.Groups["plural_chooser"].Value, @"^[\s\r\n]+|[\s\r\n]+$", "");
							if (!string.IsNullOrEmpty(pluralChooserSource))
							{
								PluralAnalyzed pluralChooser = PluralAnalyzed.Parse(pluralChooserSource);
								if (pluralChooser != null)
								{
									if (pluralChooser is PluralAnalyzed_Flat)
									{
										PluralAnalyzed_Flat flat = (PluralAnalyzed_Flat)pluralChooser;
										if (np == 2)
										{
											string zero = null, one = null;
											Match m = Regex.Match(flat.Value, @"^(?<pre>[a-z0-9]+)(?<how>(<|<=|==|>=|>|!=))(?<post>[a-z0-9]+)$", RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.Singleline);
											if (m.Success)
											{
												string midInverted = null;
												switch (m.Groups["how"].Value)
												{
													case "<":
														midInverted = ">=";
														break;
													case "<=":
														midInverted = ">";
														break;
													case "==":
														midInverted = "!=";
														break;
													case ">=":
														midInverted = "<";
														break;
													case ">":
														midInverted = "<=";
														break;
													case "!=":
														midInverted = "==";
														break;
												}
												if (midInverted != null)
												{
													zero = DescribePluralCase(string.Format("{0} {1} {2}", m.Groups["pre"].Value, midInverted, m.Groups["post"].Value));
													one = DescribePluralCase(string.Format("{0} {1} {2}", m.Groups["pre"].Value, m.Groups["how"].Value, m.Groups["post"].Value));
												}
											}
											if ((zero == null) || (one == null))
											{
												zero = DescribePluralCase(string.Format("{0} FALSE", flat.Value));
												one = DescribePluralCase(string.Format("{0} TRUE", flat.Value));
											}
											this._pluralDescriptions = new string[] { zero, one };
										}
									}
									else
									{
										PluralAnalyzed_IfThenElse ite = (PluralAnalyzed_IfThenElse)pluralChooser;
										Dictionary<int, string> caseSplitted = new Dictionary<int, string>(np);
										if (!ite.SplitCase(caseSplitted, np))
											caseSplitted = null;
										else
										{
											bool hasElse = false;
											for (int i = 0; i < np; i++)
											{
												if (!caseSplitted.ContainsKey(i))
												{
													caseSplitted = null;
													break;
												}
												if (caseSplitted[i] == "")
												{
													if (hasElse)
													{
														caseSplitted = null;
														break;
													}
													hasElse = true;
												}
											}
										}
										if (caseSplitted != null)
										{
											this._pluralDescriptions = new string[np];
											for (int i = 0; i < np; i++)
											{
												if (caseSplitted[i] == "")
													this._pluralDescriptions[i] = "General case";
												else
													this._pluralDescriptions[i] = DescribePluralCase(caseSplitted[i]);
											}
										}
									}
								}
							}
						}
					}
				}
			}
			#endregion

			#region Language
			this._languageCode = "";
			if (this._keys.ContainsKey("X-Language"))
			{
				Match match = Regex.Match(this._keys["X-Language"], @"^(?<code>[a-z]{2})($|[_-](?<code2>[a-z]{2})$)", RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.Singleline | RegexOptions.IgnoreCase);
				if (match.Success)
				{
					if (string.IsNullOrEmpty(match.Groups["code2"].Value))
						this._languageCode = match.Groups["code"].Value.ToLowerInvariant();
					else
						this._languageCode = string.Format("{0}-{1}", match.Groups["code"].Value.ToLowerInvariant(), match.Groups["code2"].Value.ToUpperInvariant());
				}
			}
			#endregion
		}
		private abstract class PluralAnalyzed
		{
			private static bool ParenthesisOk(string s)
			{
				if (string.IsNullOrEmpty(s))
					return true;
				int p0 = s.IndexOf('(');
				if (p0 < 0)
					return s.IndexOf(')') < 0;
				else if (s.IndexOf(')') < 0)
					return false;
				int nOpen = 1;
				for (int p = (p0 + 1); p < s.Length; p++)
				{
					switch (s[p])
					{
						case '(':
							nOpen++;
							break;
						case ')':
							if (--nOpen < 0)
								return false;
							break;
					}
				}
				return (nOpen == 0);
			}
			private static string SimplifyString(string s)
			{
				if (string.IsNullOrEmpty(s))
					return "";
				s = Regex.Replace(s, @"[\s\r\n]+", "");
				for (; ; )
				{
					int len = s.Length;
					bool trimPars = false;
					if ((len > 2) && (s[0] == '(') && (s[len - 1] == ')'))
					{
						trimPars = PluralAnalyzed.ParenthesisOk(s.Substring(1, s.Length - 2));
					}
					if (trimPars)
						s = s.Substring(1, len - 2);
					else
						break;
				}
				return s;
			}
			public static PluralAnalyzed Parse(string s)
			{
				s = PluralAnalyzed.SimplifyString(s);
				if (s.Length == 0)
					return null;
				int pQM = s.IndexOf('?');
				if (pQM < 0)
				{
					if (s.IndexOf(':') < 0)
						return new PluralAnalyzed_Flat(s);
					return null;
				}
				if (pQM == 0)
					return null;
				int prePars = 0;
				for (int i = 0; i < pQM; i++)
				{
					switch (s[i])
					{
						case '(':
							prePars++;
							break;
						case ')':
							prePars--;
							break;
					}
				}
				if (prePars != 0)
					throw new Exception();
				PluralAnalyzed itemIf = PluralAnalyzed.Parse(s.Substring(0, pQM));
				if (itemIf == null)
					return null;
				prePars = 0;
				for (int i = pQM + 1; i < (s.Length - 1); i++)
				{
					switch (s[i])
					{
						case '(':
							prePars++;
							break;
						case ')':
							prePars--;
							break;
						case ':':
							if (prePars == 0)
							{
								PluralAnalyzed itemThen = PluralAnalyzed.Parse(s.Substring(pQM + 1, i - pQM - 1));
								if (itemThen == null)
									return null;
								PluralAnalyzed itemElse = PluralAnalyzed.Parse(s.Substring(i + 1));
								if (itemElse == null)
									return null;
								return new PluralAnalyzed_IfThenElse(itemIf, itemThen, itemElse);
							}
							break;
					}
				}
				return null;
			}
		}
		private class PluralAnalyzed_Flat : PluralAnalyzed
		{
			private string _value;
			public string Value
			{
				get
				{ return this._value; }
			}
			private int? _valueInt;
			public int? ValueInt
			{
				get
				{
					if (this._valueInt.HasValue)
						return this._valueInt.Value;
					else
						return null;
				}
			}
			public PluralAnalyzed_Flat(string value)
			{
				this._value = value;
				this._valueInt = null;
				if (value.Length > 0)
				{
					int i;
					if (int.TryParse(value, out i))
						this._valueInt = i;
				}
			}
			public override string ToString()
			{
				return this._value;
			}
		}

		private class PluralAnalyzed_IfThenElse : PluralAnalyzed
		{
			protected PluralAnalyzed _if;
			public PluralAnalyzed If
			{
				get
				{ return this._if; }
			}
			protected PluralAnalyzed _then;
			public PluralAnalyzed Then
			{
				get
				{ return this._then; }
			}
			protected PluralAnalyzed _else;
			public PluralAnalyzed Else
			{
				get
				{ return this._else; }
			}
			public PluralAnalyzed_IfThenElse(PluralAnalyzed itemIf, PluralAnalyzed itemThen, PluralAnalyzed itemElse)
			{
				this._if = itemIf;
				this._then = itemThen;
				this._else = itemElse;
			}
			public override string ToString()
			{
				StringBuilder sb = new StringBuilder();
				this.ToString(sb, 0);
				return sb.ToString();
			}
			private void ToString(StringBuilder sb, int level)
			{
				const char PADDER = '\t';
				string padding = (level > 0) ? new string(PADDER, level) : "";
				sb.AppendLine("IF").Append(padding).Append(PADDER);
				if (this._if is PluralAnalyzed_IfThenElse)
				{
					((PluralAnalyzed_IfThenElse)this._if).ToString(sb, level + 1);
					sb.AppendLine();
				}
				else
				{
					sb.AppendLine(this._if.ToString());
				}
				sb.Append(padding).AppendLine("THEN").Append(padding).Append(PADDER);
				if (this._then is PluralAnalyzed_IfThenElse)
				{
					((PluralAnalyzed_IfThenElse)this._then).ToString(sb, level + 1);
					sb.AppendLine();
				}
				else
				{
					sb.AppendLine(this._then.ToString());
				}
				sb.Append(padding).AppendLine("ELSE").Append(padding).Append(PADDER);
				if (this._else is PluralAnalyzed_IfThenElse)
				{
					((PluralAnalyzed_IfThenElse)this._else).ToString(sb, level + 1);
				}
				else
				{
					sb.Append(this._else.ToString());
				}
			}
			public bool SplitCase(Dictionary<int, string> caseSplitted, int totalCases)
			{
				if ((this._if is PluralAnalyzed_Flat) && (this._then is PluralAnalyzed_Flat) && (((PluralAnalyzed_Flat)this._then).ValueInt.HasValue))
				{
					int index = ((PluralAnalyzed_Flat)this._then).ValueInt.Value;
					if ((index < 0) || (index >= totalCases) || (caseSplitted.ContainsKey(index)))
						return false;
					caseSplitted.Add(index, ((PluralAnalyzed_Flat)this._if).Value);
					if (this._else is PluralAnalyzed_IfThenElse)
					{
						return ((PluralAnalyzed_IfThenElse)this._else).SplitCase(caseSplitted, totalCases);
					}
					else if ((this._else is PluralAnalyzed_Flat) && (((PluralAnalyzed_Flat)this._else).ValueInt.HasValue))
					{
						index = ((PluralAnalyzed_Flat)this._else).ValueInt.Value;
						if ((index < 0) || (index >= totalCases) || (caseSplitted.ContainsKey(index)))
							return false;
						caseSplitted.Add(index, "");
						return true;
					}
					return false;
				}
				else
					return false;

			}
		}
	}
}
