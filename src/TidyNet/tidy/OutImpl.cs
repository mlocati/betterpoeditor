using System;
using System.IO;
using System.Text;
using System.Configuration;

namespace TidyNet
{
	/// <summary>
	/// Output Stream Implementation
	/// 
	/// (c) 1998-2000 (W3C) MIT, INRIA, Keio University
	/// See Tidy.cs for the copyright notice.
	/// Derived from <a href="http://www.w3.org/People/Raggett/tidy">
	/// HTML Tidy Release 4 Aug 2000</a>
	/// 
	/// </summary>
	/// <author>Dave Raggett &lt;dsr@w3.org&gt;</author>
	/// <author>Andy Quick &lt;ac.quick@sympatico.ca&gt; (translation to Java)</author>
	/// <author>Seth Yates &lt;seth_yates@hotmail.com&gt; (translation to C#)</author>
	/// <version>1.0, 1999/05/22</version>
	/// <version>1.0.1, 1999/05/29</version>
	/// <version>1.1, 1999/06/18 Java Bean</version>
	/// <version>1.2, 1999/07/10 Tidy Release 7 Jul 1999</version>
	/// <version>1.3, 1999/07/30 Tidy Release 26 Jul 1999</version>
	/// <version>1.4, 1999/09/04 DOM support</version>
	/// <version>1.5, 1999/10/23 Tidy Release 27 Sep 1999</version>
	/// <version>1.6, 1999/11/01 Tidy Release 22 Oct 1999</version>
	/// <version>1.7, 1999/12/06 Tidy Release 30 Nov 1999</version>
	/// <version>1.8, 2000/01/22 Tidy Release 13 Jan 2000</version>
	/// <version>1.9, 2000/06/03 Tidy Release 30 Apr 2000</version>
	/// <version>1.10, 2000/07/22 Tidy Release 8 Jul 2000</version>
	/// <version>1.11, 2000/08/16 Tidy Release 4 Aug 2000</version>
	internal class OutImpl : Out
	{
		public OutImpl()
		{
			Output = null;
		}
		
		public override void Outc(sbyte c)
		{
			Outc(((int) c) & 0xFF); // Convert to unsigned.
		}
		
		/* For mac users, should we map Unicode back to MacRoman? */
		public override void Outc(int c)
		{
			int ch;
			
			try
			{
				if (Encoding == CharEncoding.UTF8)
				{
					if (c < 128)
					{
						Output.WriteByte((byte) c);
					}
					else if (c <= 0x7FF)
					{
						ch = (0xC0 | (c >> 6)); Output.WriteByte((byte) ch);
						ch = (0x80 | (c & 0x3F)); Output.WriteByte((byte) ch);
					}
					else if (c <= 0xFFFF)
					{
						ch = (0xE0 | (c >> 12)); Output.WriteByte((byte) ch);
						ch = (0x80 | ((c >> 6) & 0x3F)); Output.WriteByte((byte) ch);
						ch = (0x80 | (c & 0x3F)); Output.WriteByte((byte) ch);
					}
					else if (c <= 0x1FFFFF)
					{
						ch = (0xF0 | (c >> 18)); Output.WriteByte((byte) ch);
						ch = (0x80 | ((c >> 12) & 0x3F)); Output.WriteByte((byte) ch);
						ch = (0x80 | ((c >> 6) & 0x3F)); Output.WriteByte((byte) ch);
						ch = (0x80 | (c & 0x3F)); Output.WriteByte((byte) ch);
					}
					else
					{
						ch = (0xF8 | (c >> 24)); Output.WriteByte((byte) ch);
						ch = (0x80 | ((c >> 18) & 0x3F)); Output.WriteByte((byte) ch);
						ch = (0x80 | ((c >> 12) & 0x3F)); Output.WriteByte((byte) ch);
						ch = (0x80 | ((c >> 6) & 0x3F)); Output.WriteByte((byte) ch);
						ch = (0x80 | (c & 0x3F)); Output.WriteByte((byte) ch);
					}
				}
				else if (Encoding == CharEncoding.ISO2022)
				{
					if (c == 0x1b)
					{
						/* ESC */
						State = StreamIn.FSM_ESC;
					}
					else
					{
						switch (State)
						{
						case StreamIn.FSM_ESC: 
							if (c == '$')
							{
								State = StreamIn.FSM_ESCD;
							}
							else if (c == '(')
							{
								State = StreamIn.FSM_ESCP;
							}
							else
							{
								State = StreamIn.FSM_ASCII;
							}
							break;
							
						case StreamIn.FSM_ESCD: 
							if (c == '(')
							{
								State = StreamIn.FSM_ESCDP;
							}
							else
							{
								State = StreamIn.FSM_NONASCII;
							}
							break;

						case StreamIn.FSM_ESCDP: 
							State = StreamIn.FSM_NONASCII;
							break;

						case StreamIn.FSM_ESCP: 
							State = StreamIn.FSM_ASCII;
							break;

						case StreamIn.FSM_NONASCII: 
							c &= 0x7F;
							break;
						}
					}
					
					Output.WriteByte((byte) c);
				}
				else
				{
					Output.WriteByte((byte) c);
				}
			}
			catch (IOException e)
			{
				Console.Error.WriteLine("OutImpl.outc: " + e.ToString());
			}
		}
		
		public override void Newline()
		{
			try
			{
				byte[] temp_sbyteArray;
				temp_sbyteArray = nlBytes;
				Output.Write(temp_sbyteArray, 0, temp_sbyteArray.Length);
				Output.Flush();
			}
			catch (IOException e)
			{
				Console.Error.WriteLine("OutImpl.newline: " + e.ToString());
			}
		}
		
		private static readonly byte[] nlBytes;

		static OutImpl()
		{
			string lineSeparator = (string)ConfigurationSettings.AppSettings["line.separator"];
			if (lineSeparator == null) lineSeparator = Environment.NewLine;
			nlBytes = System.Text.Encoding.UTF8.GetBytes(lineSeparator);
		}
	}
	
}