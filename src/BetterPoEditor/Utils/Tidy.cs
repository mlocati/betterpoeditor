using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Windows.Forms;

namespace BePoE.Utils
{
	internal static class Tidy
	{
		public class Result
		{
			public enum Levels
			{
				Warning = 1,
				Error = 2,
			}

			private Levels _level;
			public Levels Level
			{
				get
				{ return this._level; }
			}
			private int? _line;
			public int? Line1
			{
				get
				{ return this._line.HasValue ? this._line.Value : (int?)null; }
			}
			private int? _column;
			public int? Column1
			{
				get
				{ return this._column.HasValue ? this._column.Value : (int?)null; }
			}
			private int? _selectionStart;
			public int? SelectionStart
			{
				get
				{ return this._selectionStart.HasValue ? this._selectionStart.Value : (int?)null; }
			}
			private int? _selectionLength;
			public int? SelectionLength
			{
				get
				{ return this._selectionLength.HasValue ? this._selectionLength.Value : (int?)null; }
			}
			private string _message;
			public string Message
			{
				get
				{ return this._message; }
			}
			private Result(Levels level, int? line, int? column, int? selectionStart, int? selectionLength, string message)
			{
				this._level = level;
				this._line = line.HasValue ? line.Value : (int?)null;
				this._column = column.HasValue ? column.Value : (int?)null;
				this._selectionStart = selectionStart.HasValue ? selectionStart.Value : (int?)null;
				this._selectionLength = selectionLength.HasValue ? selectionLength.Value : (int?)null;
				this._message = (message == null) ? "" : message;
			}
			internal static Tidy.Result Parse(Text.Line[] sourceLines, TidyNet.TidyMessage tidy, int linesToSkip)
			{
				if (tidy == null)
					return null;
				Levels level;
				switch (tidy.Level)
				{
					case TidyNet.MessageLevel.Info:
						return null;
					case TidyNet.MessageLevel.Warning:
						level = Levels.Warning;
						break;
					default:
						level = Levels.Error;
						break;
				}
				int? selectionStart = null;
				int? selectionLength = null;
				bool ok = false;
				int lineBase0 = tidy.Line - linesToSkip - 1;
				int columnBase0 = tidy.Column - 1;
				if ((lineBase0 >= 0) && (lineBase0 < sourceLines.Length))
				{
					int ss = 0;
					for (int i = 0; i < lineBase0; i++)
					{
						ss += sourceLines[i].FullLength;
					}
					if (columnBase0 < 0)
					{
						ok = true;
						selectionStart = ss;
						selectionLength = sourceLines[lineBase0].TextLength;
					}
					else if (columnBase0 < sourceLines[lineBase0].TextLength)
					{
						ok = true;
						selectionStart = ss + columnBase0;
						if (columnBase0 >= (sourceLines[lineBase0].TextLength - 1))
						{
							selectionLength = sourceLines[lineBase0].TextLength - columnBase0;
						}
						else
						{
							string substr = sourceLines[lineBase0].Text.Substring(columnBase0 + 1);
							Match match = Regex.Match(substr, @"[\W]", RegexOptions.ExplicitCapture | RegexOptions.Singleline);
							selectionLength = 1 + (match.Success ? match.Index : 0);
						}
					}
				}
				return new Tidy.Result(level, ok ? (tidy.Line - linesToSkip) : (int?)null, ok ? tidy.Column : (int?)null, selectionStart, selectionLength, tidy.Message);
			}
			public override string ToString()
			{
				return this.ToString(false);
			}
			public string ToString(bool multiLine)
			{
				StringBuilder sb = new StringBuilder();
				if (multiLine)
					sb.Append(this._level.ToString());
				else
					sb.Append("[").Append(this._level.ToString()).Append("] ");
				if (this._line.HasValue)
				{
					if (multiLine)
						sb.Append(' ');
					sb.Append("@ line ").Append(this._line.Value.ToString("N0"));
					if (this._column.HasValue)
						sb.Append(", col ").Append(this._column.Value.ToString("N0"));
					if (!multiLine)
						sb.Append(": ");
				}
				if (multiLine)
					sb.AppendLine().AppendLine().Append(this._message);
				else
					sb.Append(this._message.Replace("\r\n", " ").Replace('\n', ' ').Replace('\r', ' '));
				return sb.ToString();
			}
		

		public static Tidy.Result[] Analyze(string html)
		{
			return Analyze(html, string.IsNullOrEmpty(html) ? true : (html.IndexOf("<html", StringComparison.InvariantCultureIgnoreCase) < 0));
		}
		public static Tidy.Result[] Analyze(string html, bool isPart)
		{
			List<Tidy.Result> result = new List<Tidy.Result>();
			if (!string.IsNullOrEmpty(html))
			{

				int skipLines = 0;
				byte[] buffer;
				if (isPart)
				{
					StringBuilder sb = new StringBuilder();
					sb.AppendLine(@"<!doctype html>");
					skipLines++;
					sb.AppendLine("<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" /><title>Tidier</title></head><body>");
					skipLines++;
					sb.AppendLine(html);
					sb.Append("</body></html>");
					buffer = ASCIIEncoding.UTF8.GetBytes(sb.ToString());
				}
				else
				{
					buffer = ASCIIEncoding.UTF8.GetBytes(html);
				}
				TidyNet.TidyMessageCollection msg;
				using (MemoryStream msIn = new MemoryStream())
				{
					msIn.Write(buffer, 0, buffer.Length);
					msIn.Position = 0;
					using (MemoryStream msOut = new MemoryStream())
					{
						TidyNet.Tidy tidy = new TidyNet.Tidy();
						msg = new TidyNet.TidyMessageCollection();
						tidy.Parse(msIn, msOut, msg);
					}
				}
				Utils.Text.Line[] sourceLines = Utils.Text.Line.Split(html);
				foreach (TidyNet.TidyMessage m1 in msg)
				{
					Tidy.Result result1 = Tidy.Result.Parse(sourceLines, m1, skipLines);
					if (result1 != null)
						result.Add(result1);
				}
			}
			return result.ToArray();
		}

	}
}
}
