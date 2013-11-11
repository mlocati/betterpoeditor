using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace BePoE.PO
{
	internal abstract class Line
	{
		public interface IIndexedLine
		{
			int Index { get;}
		}
		public enum Types
		{
			Comment,
			Data,
		}
		protected POFile _file;
		public POFile File
		{
			get
			{ return this._file; }
		}
		protected IO.TextLine _source;
		internal IO.TextLine Source
		{
			get
			{ return this._source; }
		}
		protected Line.Types _type;
		public Line.Types Type
		{
			get
			{ return this._type; }
		}
		protected bool _isContinuation;
		protected string _text;
		public bool IsContinuation
		{
			get
			{ return this._isContinuation; }
		}
		public string Text
		{
			get
			{ return this._text; }
		}
		public static Line ParseLine(POFile file, IO.TextLine source, Line previous)
		{
			if (source.Value[0] == '#')
				return CommentLine.Parse(file, source, previous);
			else
				return DataLine.Parse(file, source, previous);
		}
		protected Line(POFile file, IO.TextLine source, Line.Types type, bool isContinuation, string text)
		{
			this._file = file;
			this._source = source;
			this._type = type;
			this._isContinuation = isContinuation;
			this._text = text;
		}
		internal void SetText(string newText)
		{
			this._text = (newText == null) ? "" : newText;
			this._source.Value = this.ToString();
		}
	}

	internal class CommentLine : Line
	{
		public enum Kinds
		{
			TranslatorComment,
			ExtractedComment,
			Reference,
			Flags,
			PreviousUntraslated_Context,
			PreviousUntraslated_ID,
			PreviousUntraslated_IDPlural,
			Removed_Context,
			Removed_ID,
			Removed_IDPlural,
			Removed_Translated,
			Removed_TranslatedIndexed,
		}
		protected CommentLine.Kinds _kind;
		public CommentLine.Kinds Kind
		{
			get { return this._kind; }
		}
		private static Regex _rxContinuation_PreviousUntraslated = null;
		public static Regex RxContinuation_PreviousUntraslated
		{
			get
			{
				if (CommentLine._rxContinuation_PreviousUntraslated == null)
					CommentLine._rxContinuation_PreviousUntraslated = new Regex(@"^#\|\s*""(?<text>.*)""\s*$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.Singleline);
				return CommentLine._rxContinuation_PreviousUntraslated;
			}
		}

		private static Regex _rxContinuation_Removed = null;
		public static Regex RxContinuation_Removed
		{
			get
			{
				if (CommentLine._rxContinuation_Removed == null)
					CommentLine._rxContinuation_Removed = new Regex(@"^#~\s*""(?<text>.*)""\s*$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.Singleline);
				return CommentLine._rxContinuation_Removed;
			}
		}

		private static Dictionary<CommentLine.Kinds, Regex> _rxFindKind = null;
		public static Dictionary<CommentLine.Kinds, Regex> RxFindKind
		{
			get
			{
				if (CommentLine._rxFindKind == null)
				{
					Dictionary<CommentLine.Kinds, Regex> rx = new Dictionary<CommentLine.Kinds, Regex>();
					RegexOptions options = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.Singleline;
					rx.Add(CommentLine.Kinds.TranslatorComment, new Regex(@"^#($|\s$|\s(?<text>.*)$)", options));
					rx.Add(CommentLine.Kinds.ExtractedComment, new Regex(@"^#\.(\s*$|\s+(?<text>.*)$)", options));
					rx.Add(CommentLine.Kinds.Reference, new Regex(@"^#:(\s*$|\s+(?<text>.*)$)", options));
					rx.Add(CommentLine.Kinds.Flags, new Regex(@"^#,(\s*$|\s+(?<text>.*)$)", options));
					rx.Add(CommentLine.Kinds.PreviousUntraslated_Context, new Regex(@"^#\| msgctxt\s+""(?<text>.*)""\s*$", options));
					rx.Add(CommentLine.Kinds.PreviousUntraslated_ID, new Regex(@"^#\| msgid\s+""(?<text>.*)""\s*$", options));
					rx.Add(CommentLine.Kinds.PreviousUntraslated_IDPlural, new Regex(@"^#\| msgid_plural\s+""(?<text>.*)""\s*$", options));
					rx.Add(CommentLine.Kinds.Removed_Context, new Regex(@"^#~ msgctxt\s+""(?<text>.*)""\s*$", options));
					rx.Add(CommentLine.Kinds.Removed_ID, new Regex(@"^#~ msgid\s+""(?<text>.*)""\s*$", options));
					rx.Add(CommentLine.Kinds.Removed_IDPlural, new Regex(@"^#~ msgid_plural\s+""(?<text>.*)""\s*$", options));
					rx.Add(CommentLine.Kinds.Removed_Translated, new Regex(@"^#~ msgstr\s+""(?<text>.*)""\s*$", options));
					rx.Add(CommentLine.Kinds.Removed_TranslatedIndexed, new Regex(@"^#~ msgstr\[(?<index>[0-9]+)\]\s+""(?<text>.*)""\s*$", options));
					CommentLine._rxFindKind = rx;
				}
				return CommentLine._rxFindKind;
			}
		}
		public static CommentLine Parse(POFile file, IO.TextLine source, Line previous)
		{
			Match match;

			bool isContinuation;
			CommentLine.Kinds? kind;
			string text;
			int? index;

			string sourceValue = source.Value;
			if ((match = CommentLine.RxContinuation_PreviousUntraslated.Match(sourceValue)).Success)
			{
				if (previous == null)
					throw new Exception("Comment of kind 'previous-untranslated' continuation at block start.");
				if (previous.Type != Line.Types.Comment)
					throw new Exception("Comment of kind 'previous-untranslated' continuation after a non-comment line.");
				CommentLine prevComment = (CommentLine)previous;
				switch (prevComment._kind)
				{
					case CommentLine.Kinds.PreviousUntraslated_Context:
					case CommentLine.Kinds.PreviousUntraslated_ID:
					case CommentLine.Kinds.PreviousUntraslated_IDPlural:
						break;
					default:
						throw new Exception(string.Format("Comment of kind 'previous-untranslated' continuation after a comment of kind '{0}'.", prevComment._kind));
				}
				isContinuation = true;
				kind = prevComment._kind;
				text = match.Groups["text"].Value;
				Line.IIndexedLine idx = prevComment as Line.IIndexedLine;
				index = (idx != null) ? idx.Index : (int?)null;
			}
			else if ((match = CommentLine.RxContinuation_Removed.Match(sourceValue)).Success)
			{
				if (previous == null)
					throw new Exception("Comment of kind 'removed' continuation at block start.");
				if (previous.Type != Line.Types.Comment)
					throw new Exception("Comment of kind 'removed' continuation after a non-comment line.");
				CommentLine prevComment = (CommentLine)previous;
				switch (prevComment._kind)
				{
					case CommentLine.Kinds.Removed_Context:
					case CommentLine.Kinds.Removed_ID:
					case CommentLine.Kinds.Removed_IDPlural:
					case CommentLine.Kinds.Removed_Translated:
					case CommentLine.Kinds.Removed_TranslatedIndexed:
						break;
					default:
						throw new Exception(string.Format("Comment of kind 'removed' continuation after a comment of kind '{0}'.", prevComment._kind));
				}
				isContinuation = true;
				kind = prevComment._kind;
				text = match.Groups["text"].Value;
				Line.IIndexedLine idx = prevComment as Line.IIndexedLine;
				index = (idx != null) ? idx.Index : (int?)null;
			}
			else
			{
				isContinuation = false;
				kind = null;
				text = null;
				index = null;
				foreach (KeyValuePair<CommentLine.Kinds, Regex> kv in CommentLine.RxFindKind)
				{
					match = kv.Value.Match(sourceValue);
					if (!match.Success)
						continue;
					kind = kv.Key;
					text = match.Groups["text"].Value;
					index = (match.Groups["index"].Value.Length > 0) ? int.Parse(match.Groups["index"].Value) : (int?)null;
					break;
				}
			}
			if (index.HasValue)
				return new IndexedCommentLine(file, source, isContinuation, kind.Value, text, index.Value);
			else
				return new CommentLine(file, source, isContinuation, kind.Value, text);
		}

		protected CommentLine(POFile file, IO.TextLine source, bool isContinuation, CommentLine.Kinds kind, string text)
			: base(file, source, Line.Types.Comment, isContinuation, text)
		{
			this._kind = kind;
		}

		internal static CommentLine CreateNew(Entry entry, CommentLine.Kinds kind, int? index, bool isContinuation, IO.TextLine addRelativeTo, bool addRelativeToBefore, string text)
		{
			if (entry == null)
				throw new ArgumentNullException("entry");
			if (addRelativeTo == null)
				throw new ArgumentNullException("addRelativeTo");
			int insertAt = entry.File.TextFile.Lines.IndexOf(addRelativeTo);
			if (insertAt < 0)
				throw new ArgumentOutOfRangeException("addRelativeTo");
			if (!addRelativeToBefore)
				insertAt++;
			IO.TextLine textLine = new IO.TextLine(BuildTextFileData(kind, index, isContinuation, text));
			entry.File.TextFile.Lines.Insert(insertAt, textLine);
			CommentLine result;
			if (index.HasValue)
				result = new IndexedCommentLine(entry.File, textLine, isContinuation, kind, text, index.Value);
			else
				result = new CommentLine(entry.File, textLine, isContinuation, kind, text);
			entry.LineAdded(result);
			return result;
		}
		private static string BuildTextFileData(CommentLine.Kinds kind, int? index, bool isContinuation, string text)
		{
			if (isContinuation)
			{
				switch (kind)
				{
					case CommentLine.Kinds.PreviousUntraslated_Context:
					case CommentLine.Kinds.PreviousUntraslated_ID:
					case CommentLine.Kinds.PreviousUntraslated_IDPlural:
						return string.Format("#| \"{0}\"", text);
					case CommentLine.Kinds.Removed_Context:
					case CommentLine.Kinds.Removed_ID:
					case CommentLine.Kinds.Removed_IDPlural:
					case CommentLine.Kinds.Removed_Translated:
					case CommentLine.Kinds.Removed_TranslatedIndexed:
						return string.Format("#~ \"{0}\"", text);
					default:
						throw new NotImplementedException();
				}
			}
			else
			{
				switch (kind)
				{
					case CommentLine.Kinds.TranslatorComment:
						return string.Format("# {0}", text);
					case CommentLine.Kinds.ExtractedComment:
						return string.Format("#. {0}", text);
					case CommentLine.Kinds.Reference:
						return string.Format("#: {0}", text);
					case CommentLine.Kinds.Flags:
						return string.Format("#, {0}", text);
					case CommentLine.Kinds.PreviousUntraslated_Context:
						return string.Format("#| msgctxt \"{0}\"", text);
					case CommentLine.Kinds.PreviousUntraslated_ID:
						return string.Format("#| msgid \"{0}\"", text);
					case CommentLine.Kinds.PreviousUntraslated_IDPlural:
						return string.Format("#| msgid_plural \"{0}\"", text);
					case CommentLine.Kinds.Removed_Context:
						return string.Format("#~ msgctxt \"{0}\"", text);
					case CommentLine.Kinds.Removed_ID:
						return string.Format("#~ msgid \"{0}\"", text);
					case CommentLine.Kinds.Removed_IDPlural:
						return string.Format("#~ msgid_plural \"{0}\"", text);
					case CommentLine.Kinds.Removed_Translated:
						return string.Format("#~ msgstr \"{0}\"", text);
					case CommentLine.Kinds.Removed_TranslatedIndexed:
						return string.Format("#~ msgstr[{0}] \"{1}\"", index.Value, text);
					default:
						throw new NotImplementedException();
				}
			}
		}

		public override string ToString()
		{
			return BuildTextFileData(this._kind, ((this as Line.IIndexedLine) == null) ? ((int?)null) : ((Line.IIndexedLine)this).Index, this.IsContinuation, this._text);
		}
	}
	internal class IndexedCommentLine : CommentLine, Line.IIndexedLine
	{
		protected int _index;
		public int Index
		{
			get { return this._index; }
		}
		internal IndexedCommentLine(POFile file, IO.TextLine source, bool isContinuation, CommentLine.Kinds kind, string text, int index)
			: base(file, source, isContinuation, kind, text)
		{
			this._index = index;
		}

	}

	internal class DataLine : Line
	{
		public enum Kinds
		{
			Context,
			ID,
			IDPlural,
			Translated,
			TranslatedIndexed,
		}
		protected DataLine.Kinds _kind;
		public DataLine.Kinds Kind
		{
			get
			{ return this._kind; }
		}

		private static Dictionary<DataLine.Kinds, Regex> _rxFindKind = null;
		public static Dictionary<DataLine.Kinds, Regex> RxFindKind
		{
			get
			{
				if (DataLine._rxFindKind == null)
				{
					Dictionary<DataLine.Kinds, Regex> rx = new Dictionary<DataLine.Kinds, Regex>();
					RegexOptions options = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.Singleline;
					rx.Add(DataLine.Kinds.Context, new Regex(@"^msgctxt\s+""?(?<text>.*)""?\s*$", options));
					rx.Add(DataLine.Kinds.ID, new Regex(@"^msgid\s+""(?<text>.*)""\s*$", options));
					rx.Add(DataLine.Kinds.IDPlural, new Regex(@"^msgid_plural\s+""(?<text>.*)""\s*$", options));
					rx.Add(DataLine.Kinds.Translated, new Regex(@"^msgstr\s+""(?<text>.*)""\s*$", options));
					rx.Add(DataLine.Kinds.TranslatedIndexed, new Regex(@"^msgstr\[(?<index>[0-9]+)\]\s+""(?<text>.*)""\s*$", options));
					DataLine._rxFindKind = rx;
				}
				return DataLine._rxFindKind;
			}
		}
		private static Regex _rxContinuation = null;
		public static Regex RxContinuation
		{
			get
			{
				if (DataLine._rxContinuation == null)
					DataLine._rxContinuation = new Regex(@"^\s*""(?<text>.*)""\s*$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.Singleline);
				return DataLine._rxContinuation;
			}
		}

		public static DataLine Parse(POFile file, IO.TextLine source, Line previous)
		{
			Match match;

			bool isContinuation;
			DataLine.Kinds? kind;
			string text;
			int? index;

			string sourceValue = source.Value;
			if ((match = DataLine.RxContinuation.Match(sourceValue)).Success)
			{
				if (previous == null)
					throw new Exception("Data continuation at block start.");
				if (previous.Type != Line.Types.Data)
					throw new Exception("Data continuation after a non-data line.");
				DataLine prevData = (DataLine)previous;
				isContinuation = true;
				kind = prevData._kind;
				text = match.Groups["text"].Value;
				Line.IIndexedLine idx = prevData as Line.IIndexedLine;
				index = (idx != null) ? idx.Index : (int?)null;
			}
			else
			{
				isContinuation = false;
				kind = null;
				text = null;
				index = null;
				foreach (KeyValuePair<DataLine.Kinds, Regex> kv in DataLine.RxFindKind)
				{
					match = kv.Value.Match(sourceValue);
					if (!match.Success)
						continue;
					kind = kv.Key;
					text = match.Groups["text"].Value;
					index = (match.Groups["index"].Value.Length > 0) ? int.Parse(match.Groups["index"].Value) : (int?)null;
					break;
				}
			}
			if (!kind.HasValue)
				throw new Exception("Unknown line kind.");
			if (index.HasValue)
				return new IndexedDataLine(file, source, isContinuation, kind.Value, text, index.Value);
			else
				return new DataLine(file, source, isContinuation, kind.Value, text);
		}
		protected DataLine(POFile file, IO.TextLine source, bool isContinuation, DataLine.Kinds kind, string text)
			: base(file, source, Line.Types.Data, isContinuation, text)
		{
			this._kind = kind;
		}

		internal static DataLine CreateNew(PO.Entry entry, DataLine.Kinds kind, int? index, bool isContinuation, IO.TextLine addRelativeTo, bool addRelativeToBefore, string text)
		{
			if (entry == null)
				throw new ArgumentNullException("entry");
			if (addRelativeTo == null)
				throw new ArgumentNullException("addRelativeTo");
			int insertAt = entry.File.TextFile.Lines.IndexOf(addRelativeTo);
			if (insertAt < 0)
				throw new ArgumentOutOfRangeException("addRelativeTo");
			if (!addRelativeToBefore)
				insertAt++;
			IO.TextLine textLine = new IO.TextLine(BuildTextFileData(kind, index, isContinuation, text));
			entry.File.TextFile.Lines.Insert(insertAt, textLine);
			DataLine result;
			if (index.HasValue)
				result = new IndexedDataLine(entry.File, textLine, isContinuation, kind, text, index.Value);
			else
				result = new DataLine(entry.File, textLine, isContinuation, kind, text);
			entry.LineAdded(result);
			return result;
		}
		private static string BuildTextFileData(DataLine.Kinds kind, int? index, bool isContinuation, string text)
		{
			StringBuilder res = new StringBuilder();
			if (!isContinuation)
			{
				switch (kind)
				{
					case Kinds.Context:
						res.Append("msgctxt ");
						break;
					case Kinds.ID:
						res.Append("msgid ");
						break;
					case Kinds.IDPlural:
						res.Append("msgid_plural ");
						break;
					case Kinds.Translated:
						res.Append("msgstr ");
						break;
					case Kinds.TranslatedIndexed:
						res.Append("msgstr[").Append(index.Value).Append("] ");
						break;
					default:
						throw new NotImplementedException();
				}
			}
			res.Append('"');
			if (!string.IsNullOrEmpty(text))
				res.Append(text);
			res.Append('"');
			return res.ToString();
		}
		public override string ToString()
		{
			return BuildTextFileData(this._kind, ((this as Line.IIndexedLine) == null) ? ((int?)null) : ((Line.IIndexedLine)this).Index, this.IsContinuation, this._text);
		}
	}
	internal class IndexedDataLine : DataLine, Line.IIndexedLine
	{
		protected int _index;
		public int Index
		{
			get { return this._index; }
		}
		internal IndexedDataLine(POFile file, IO.TextLine source, bool isContinuation, DataLine.Kinds kind, string text, int index)
			: base(file, source, isContinuation, kind, text)
		{
			this._index = index;
		}
	}
}
