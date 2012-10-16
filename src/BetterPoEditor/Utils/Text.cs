using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Windows.Forms;

namespace BePoE.Utils
{
	internal static class Text
	{
		private static string[] _numberToName_ones = new string[] { "", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
		private static string[] _numberToName_teens = new string[] { "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
		private static string[] _numberToName_tens = new string[] { "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };
		private static string[] _numberToName_thousandsGroups = { "", " thousand", " million", " billion" };
		private static void _numberToName_Worker(int n, int thousands, ref StringBuilder leftDigits)
		{
			if (n == 0)
				return;
			if (leftDigits == null)
				leftDigits = new StringBuilder();
			else
				leftDigits.Append(' ');
			if (n < 10)
				leftDigits.Append(_numberToName_ones[n]);
			else if (n < 20)
			{
				leftDigits.Append(_numberToName_teens[n - 10]);
			}
			else if (n < 100)
			{
				StringBuilder sb = new StringBuilder(_numberToName_tens[n / 10 - 2]);
				_numberToName_Worker(n % 10, 0, ref sb);
				leftDigits.Append(sb.ToString());
			}
			else if (n < 1000)
			{
				StringBuilder sb = new StringBuilder(_numberToName_ones[n / 100]).Append(" hundred");
				_numberToName_Worker(n % 100, 0, ref sb);
				leftDigits.Append(sb.ToString());
			}
			else
			{
				StringBuilder sb = null;
				_numberToName_Worker(n / 1000, thousands + 1, ref sb);
				_numberToName_Worker(n % 1000, 0, ref sb);
				leftDigits.Append(sb.ToString());
			}
			leftDigits.Append(_numberToName_thousandsGroups[thousands]);
		}
		public static string NumberToName(int num)
		{
			if (num == 0)
				return "zero";
			if (num < 0)
				return string.Format("negative {0}", NumberToName(-num));
			StringBuilder sb = null;
			_numberToName_Worker(num, 0, ref sb);
			return sb.ToString();
		}

		public static string Capitalize(string s)
		{
			if (string.IsNullOrEmpty(s))
				return "";
			else
				return (s.Length > 1) ? string.Format("{0}{1}", char.ToUpper(s[0]), s.Substring(1)) : s.ToUpper();
		}

		public class Line
		{
			private string _text;
			public string Text
			{
				get
				{ return this._text; }
			}
			private string _lineEnding;
			public string LineEnding
			{
				get
				{ return this._lineEnding; }
			}
			public int TextLength
			{
				get
				{ return this._text.Length; }
			}
			public int FullLength
			{
				get
				{ return this._text.Length + this._lineEnding.Length; }
			}
			private Line(string text, string lineEnding)
			{
				this._text = (text == null) ? "" : text;
				this._lineEnding = (lineEnding == null) ? "" : lineEnding;
			}
			public static Line[] Split(string text)
			{
				List<Line> result = new List<Line>();
				if (string.IsNullOrEmpty(text))
					result.Add(new Line("", ""));
				else
				{
					StringBuilder curLine = null;
					for (int i = 0; i < text.Length; i++)
					{
						switch (text[i])
						{
							case '\r':
								if (((i + 1) < text.Length) && (text[i + 1] == '\n'))
								{
									result.Add(new Line((curLine == null) ? "" : curLine.ToString(), "\r\n"));
									i++;
								}
								else
								{
									result.Add(new Line((curLine == null) ? "" : curLine.ToString(), "\r"));
								}
								curLine = null;
								break;
							case '\n':
								result.Add(new Line((curLine == null) ? "" : curLine.ToString(), "\n"));
								curLine = null;
								break;
							default:
								if (curLine == null)
									curLine = new StringBuilder();
								curLine.Append(text[i]);
								break;
						}
					}
					if (curLine != null)
						result.Add(new Line(curLine.ToString(), ""));
				}
				return result.ToArray();
			}
		}
	}
}
