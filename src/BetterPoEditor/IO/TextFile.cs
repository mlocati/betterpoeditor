using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace BePoE.IO
{
	public class TextFile
	{
		public event EventHandler Saved;
		private void OnSaved()
		{
			if (this.Saved != null)
				this.Saved(this, new EventArgs());
		}
		

		public delegate int? AnsiCodePageGetter(Dictionary<int, string> availableCodePages, int defaultCodePage, byte[] dataBytes, int[] ansiCharPositions);

		public class CodePageInfoAttribute : DescriptionAttribute
		{
			private byte[] _bom;
			public byte[] Bom
			{
				get
				{ return this._bom; }
			}
			private bool _bomRequired;
			public bool BomRequired
			{
				get
				{ return this._bomRequired; }
			}
			public CodePageInfoAttribute(string description, byte[] bom, bool bomRequired)
				: base(description)
			{
				this._bom = ((bom == null) || (bom.Length == 0)) ? new byte[] { } : bom;
				this._bomRequired = bomRequired;
			}
		}
		public enum SpecialCodePage : int
		{
			[CodePageInfo("UTF-8", new byte[] { 0xEF, 0xBB, 0xBF }, false)]
			Utf8 = 65001,
			[CodePageInfo("UTF-16 (Little-endian)", new byte[] { 0xFF, 0xFE }, true)]
			Utf16_LE = 1200,
			[CodePageInfo("UTF-16 (Big-endian)", new byte[] { 0xFE, 0xFF }, true)]
			Utf16_BE = 1201,
			[CodePageInfo("UTF-32 (Little-endian)", new byte[] { 0xFF, 0xFE, 0x00, 0x00 }, true)]
			Utf32_LE = 12000,
			[CodePageInfo("UTF-32 (Big-endian)", new byte[] { 0x00, 0x00, 0xFE, 0xFF }, true)]
			Utf32_BE = 12001,
		}
		private static Dictionary<SpecialCodePage, CodePageInfoAttribute> _specialCodePageInfo = null;
		private static Dictionary<SpecialCodePage, CodePageInfoAttribute> SpecialCodePageInfo
		{
			get
			{
				if (TextFile._specialCodePageInfo == null)
				{
					Dictionary<SpecialCodePage, CodePageInfoAttribute> specialCodePageInfo = new Dictionary<SpecialCodePage, CodePageInfoAttribute>();
					Type enumType = typeof(SpecialCodePage);
					Type attribType = typeof(CodePageInfoAttribute);
					foreach (SpecialCodePage specialCodePage in Enum.GetValues(enumType))
						specialCodePageInfo.Add(specialCodePage, (CodePageInfoAttribute)enumType.GetMember(specialCodePage.ToString())[0].GetCustomAttributes(attribType, false)[0]);
					TextFile._specialCodePageInfo = specialCodePageInfo;
				}
				return TextFile._specialCodePageInfo;
			}
		}

		private static Dictionary<int, string> _ansiCodePages = null;
		public static Dictionary<int, string> AnsiCodePages
		{
			get
			{
				if (TextFile._ansiCodePages == null)
				{
					Dictionary<int, string> ansiCodePages = new Dictionary<int, string>();
					Type enumType = typeof(SpecialCodePage);
					foreach (EncodingInfo ei in System.Text.Encoding.GetEncodings())
					{
						string dn;
						try
						{
							dn = ei.DisplayName;
						}
						catch
						{
							dn = null;
						}
						if(!string.IsNullOrEmpty(dn))
						{
							if (ansiCodePages.ContainsKey(ei.CodePage))
							{
								ansiCodePages[ei.CodePage] = string.Format("{0} / {1}", ansiCodePages[ei.CodePage], dn);
							}
							else if (!Enum.IsDefined(enumType, ei.CodePage))
							{
								ansiCodePages.Add(ei.CodePage, dn);
							}
						}
					}
					TextFile._ansiCodePages = ansiCodePages;
				}
				return TextFile._ansiCodePages;
			}
		}

		private string _fileName;
		public string FileName
		{
			get
			{ return this._fileName; }
		}
		private List<IO.TextLine> _lines;
		public List<IO.TextLine> Lines
		{
			get
			{ return this._lines; }
		}
		private int _codePage;
		public int CodePage
		{
			get
			{ return this._codePage; }
		}
		private bool _useBOM;
		public bool UseBOM
		{
			get
			{ return this._useBOM; }
		}
		public void SetEncoding(int codePage, bool useBOM)
		{
			if ((this._codePage == codePage) && (this._useBOM == useBOM))
				return;
			if (TextFile.AnsiCodePages.ContainsKey(codePage))
			{
				if (useBOM)
					throw new ArgumentOutOfRangeException("useBOM", useBOM, "The Byte-Order-Mark can't be set for ANSI code pages.");
			}
			else if (Enum.IsDefined(typeof(SpecialCodePage), codePage))
			{
				SpecialCodePage scp = (SpecialCodePage)codePage;
				if (TextFile.SpecialCodePageInfo[scp].BomRequired && (!useBOM))
					throw new ArgumentOutOfRangeException("useBOM", useBOM, string.Format("The Byte-Order-Mark must be set for the {0} encoding.", TextFile.SpecialCodePageInfo[scp].Description));
			}
			else
			{
				throw new ArgumentOutOfRangeException("codePage", codePage, string.Format("Unknown code page ({0}).", codePage));
			}
			if (this._codePage != codePage)
			{
				Encoding encoding = Encoding.GetEncoding(codePage);
				for (int iLine = 0; iLine < this._lines.Count; iLine++)
				{
					string line = this._lines[iLine].Value;
					if (!string.IsNullOrEmpty(line))
						if (string.Compare(line, encoding.GetString(encoding.GetBytes(line)), false) != 0)
							throw new ArgumentOutOfRangeException("codePage", codePage, string.Format("The code page {1} can't be used to save this file. For example it can not handle the string al line {2:N0}:{0}{3}", Environment.NewLine, encoding.EncodingName, iLine, line));
				}
			}
			this._codePage = codePage;
			this._useBOM = useBOM;
		}
		private IO.LineEndings _lineEnding;
		public IO.LineEndings LineEnding
		{
			get
			{ return this._lineEnding; }
			set
			{
				if (!Enum.IsDefined(typeof(IO.LineEndings), value))
					throw new ArgumentOutOfRangeException("LineEnding", value, "Unknown line ending.");
				this._lineEnding = value;
			}
		}

		private TextFile(string fileName, int codePage, bool useBOM, string[] lines, IO.LineEndings lineEnding)
		{
			this._fileName = fileName;
			this._codePage = codePage;
			this._useBOM = useBOM;
			if ((lines == null) || (lines.Length == 0))
				this._lines = new List<IO.TextLine>();
			else
			{
				this._lines = new List<IO.TextLine>(lines.Length);
				foreach (string l in lines)
					this._lines.Add(new IO.TextLine(l));
			}
			this._lineEnding = lineEnding;
		}
		public static TextFile Load(string fileName)
		{
			return TextFile.Load(fileName, null);
		}
		public static TextFile Load(string fileName, AnsiCodePageGetter ansiCodePageGetter)
		{
			if (string.IsNullOrEmpty(fileName))
				throw new ArgumentNullException("fileName");
			if (!File.Exists(fileName))
				throw new FileNotFoundException("File not found.", fileName);
			byte[] allBytes;
			using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				if (fs.Length == 0)
					allBytes = new byte[] { };
				else
				{
					allBytes = new byte[fs.Length];
					int nread = fs.Read(allBytes, 0, allBytes.Length);
					if (nread != allBytes.Length)
						throw new Exception();
				}
			}
			int codePage;
			int textStart;
			if (allBytes.Length == 0)
			{
				codePage = (int)SpecialCodePage.Utf8;
				textStart = 0;
			}
			else
			{
				KeyValuePair<SpecialCodePage, CodePageInfoAttribute>? codePageGot = null;
				foreach (KeyValuePair<SpecialCodePage, CodePageInfoAttribute> kv in TextFile.SpecialCodePageInfo)
				{
					bool maybe;
					if (kv.Value.Bom.Length > 0)
					{
						if (allBytes.Length >= kv.Value.Bom.Length)
						{
							byte[] bytesBom = new byte[kv.Value.Bom.Length];
							Array.Copy(allBytes, bytesBom, bytesBom.Length);
							maybe = Array.Equals(bytesBom, kv.Value.Bom);
						}
						else
						{
							maybe = false;
						}
					}
					else
						maybe = true;
					if (maybe && ((codePageGot == null) || (codePageGot.Value.Value.Bom.Length < kv.Value.Bom.Length)))
						codePageGot = kv;
				}
				if (codePageGot != null)
				{
					codePage = (int)codePageGot.Value.Key;
					textStart = codePageGot.Value.Value.Bom.Length;
				}
				else
				{
					textStart = 0;
					List<int> ansiPositions = new List<int>();
					for (int i = 0; i < allBytes.Length; i++)
					{
						byte cur = allBytes[i];
						if (cur <= 0x80)
							continue;
						if (i == (allBytes.Length - 1))
						{
							ansiPositions.Add(i);
							continue;
						}
						byte nxt1 = allBytes[i + 1];
						if ((cur >= 0xC0) && (cur <= 0xDF) && (nxt1 >= 0x80) && (nxt1 <= 0xBF))
						{
							i += 1; //ok: 2-byte char
						}
						else
						{
							if (i == (allBytes.Length - 2))
							{
								ansiPositions.Add(i);
								continue;
							}
							byte nxt2 = allBytes[i + 2];
							if ((cur >= 0xE0) && (cur <= 0xEF) && (nxt1 >= 0x80) && (nxt1 <= 0xBF) && (nxt2 >= 0x80) && (nxt2 <= 0xBF))
							{
								i += 2; //ok: 3-byte char
							}
							else
							{
								if (i == (allBytes.Length - 3))
								{
									ansiPositions.Add(i);
									continue;
								}
								byte nxt3 = allBytes[i + 3];
								if ((cur >= 0xF0) && (cur <= 0xF7) && (nxt1 >= 0x80) && (nxt1 <= 0xBF) && (nxt2 >= 0x80) && (nxt2 <= 0xBF) && (nxt3 >= 0x80) && (nxt3 <= 0xBF))
								{
									i += 3; //ok: 4-byte char
								}
								else
								{
									ansiPositions.Add(i);
									continue;
								}
							}
						}
					}
					if (ansiPositions.Count == 0)
					{
						codePage = (int)SpecialCodePage.Utf8;
					}
					else
					{
						int defaultCodePage = System.Text.Encoding.Default.CodePage;
						if (!TextFile.AnsiCodePages.ContainsKey(defaultCodePage))
							foreach (KeyValuePair<int, string> kv in TextFile.AnsiCodePages)
							{
								defaultCodePage = kv.Key;
								break;
							}
						if (ansiCodePageGetter == null)
						{
							codePage = defaultCodePage;
						}
						else
						{
							int? cp = ansiCodePageGetter(TextFile.AnsiCodePages, defaultCodePage, allBytes, ansiPositions.ToArray());
							if (cp.HasValue)
								codePage = cp.Value;
							else
								throw new OperationCanceledException();
						}
					}
				}
			}
			string text;
			if ((allBytes.Length - textStart) == 0)
				text = "";
			else
				text = Encoding.GetEncoding((int)codePage).GetString(allBytes, textStart, allBytes.Length - textStart);
			string newLineSequence = null;
			for (int i = 0; (newLineSequence == null) && (i < text.Length); i++)
			{
				switch (text[i])
				{
					case '\r':
						if ((i < text.Length - 1) && (text[i + 1] == '\n'))
							newLineSequence = "\r\n";
						else
							newLineSequence = "\r";
						break;
					case '\n':
						newLineSequence = "\n";
						break;
				}
			}
			if (newLineSequence == null)
				newLineSequence = Environment.NewLine;
			IO.LineEndings lineEnding;
			switch (newLineSequence)
			{
				case "\r\n":
					lineEnding = IO.LineEndings.Windows;
					break;
				case "\n":
					lineEnding = IO.LineEndings.Linux;
					break;
				case "\r":
					lineEnding = IO.LineEndings.Mac;
					break;
				default:
					throw new Exception("Unknown line separator.");
			}
			return new TextFile(fileName, codePage, textStart > 0, (newLineSequence.Length == 1) ? text.Split(newLineSequence[0]) : text.Split(new string[] { newLineSequence }, StringSplitOptions.None), lineEnding);
		}
		private void _save(FileStream fs)
		{
			if (this._useBOM && Enum.IsDefined(typeof(SpecialCodePage), this._codePage))
			{
				byte[] bom = TextFile.SpecialCodePageInfo[(SpecialCodePage)this._codePage].Bom;
				if ((bom != null) && (bom.Length > 0))
				{
					fs.Write(bom, 0, bom.Length);
				}
			}
			if (this._lines.Count > 0)
			{
				Encoding encoding = Encoding.GetEncoding(this._codePage);

				IO.TextLine lastLine = this._lines[this._lines.Count - 1];
				byte[] newLine = null;
				switch (this._lineEnding)
				{
					case IO.LineEndings.Windows:
						newLine = new byte[] { (byte)'\r', (byte)'\n' };
						break;
					case IO.LineEndings.Linux:
						newLine = new byte[] { (byte)'\n' };
						break;
					case IO.LineEndings.Mac:
						newLine = new byte[] { (byte)'\r' };
						break;
				}
				foreach (IO.TextLine line in this._lines)
				{
					if (line.Value.Length > 0)
					{
						byte[] buf = encoding.GetBytes(line.Value);
						fs.Write(buf, 0, buf.Length);
					}
					if (line != lastLine)
						fs.Write(newLine, 0, newLine.Length);
				}
			}
			fs.Flush();
		}
		public void SaveACopyAs(string fileName, bool overwrite)
		{
			using (FileStream fs = new FileStream(fileName, overwrite ? FileMode.Create : FileMode.CreateNew, FileAccess.Write, FileShare.Read))
			{
				this._save(fs);
			}
		}
		public void Save()
		{
			string tempFileName = Path.GetTempFileName();
			try
			{
				using (FileStream fs = new FileStream(tempFileName, FileMode.Create, FileAccess.Write, FileShare.Read))
				{
					this._save(fs);
				}
				File.Delete(this._fileName);
				File.Move(tempFileName, this._fileName);
				this.OnSaved();
			}
			catch
			{
				if (File.Exists(tempFileName))
				{
					try
					{ File.Delete(tempFileName); }
					catch
					{ }
				}
				throw;
			}
		}
	}
}
