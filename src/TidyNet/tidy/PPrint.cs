using System;
using System.IO;
	
namespace TidyNet
{
	/// <summary>
	/// Pretty print parse tree
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
	/// <remarks>Block-level and unknown elements are printed on
	/// new lines and their contents indented 2 spaces
	/// Inline elements are printed inline.
	/// Inline content is wrapped on spaces (except in
	/// attribute values or preformatted text, after
	/// start tags and before end tags
	/// </remarks>
	internal class PPrint
	{
		public const short EFFECT_BLEND = - 1;
		public const short EFFECT_BOX_IN = 0;
		public const short EFFECT_BOX_OUT = 1;
		public const short EFFECT_CIRCLE_IN = 2;
		public const short EFFECT_CIRCLE_OUT = 3;
		public const short EFFECT_WIPE_UP = 4;
		public const short EFFECT_WIPE_DOWN = 5;
		public const short EFFECT_WIPE_RIGHT = 6;
		public const short EFFECT_WIPE_LEFT = 7;
		public const short EFFECT_VERT_BLINDS = 8;
		public const short EFFECT_HORZ_BLINDS = 9;
		public const short EFFECT_CHK_ACROSS = 10;
		public const short EFFECT_CHK_DOWN = 11;
		public const short EFFECT_RND_DISSOLVE = 12;
		public const short EFFECT_SPLIT_VIRT_IN = 13;
		public const short EFFECT_SPLIT_VIRT_OUT = 14;
		public const short EFFECT_SPLIT_HORZ_IN = 15;
		public const short EFFECT_SPLIT_HORZ_OUT = 16;
		public const short EFFECT_STRIPS_LEFT_DOWN = 17;
		public const short EFFECT_STRIPS_LEFT_UP = 18;
		public const short EFFECT_STRIPS_RIGHT_DOWN = 19;
		public const short EFFECT_STRIPS_RIGHT_UP = 20;
		public const short EFFECT_RND_BARS_HORZ = 21;
		public const short EFFECT_RND_BARS_VERT = 22;
		public const short EFFECT_RANDOM = 23;
		
		public PPrint(TidyOptions options)
		{
			_options = options;
		}
		
		/*
		1010  A
		1011  B
		1100  C
		1101  D
		1110  E
		1111  F
		*/
		
		/* return one less that the number of bytes used by UTF-8 char */
		/* str points to 1st byte, *ch initialized to 1st byte */
		public static int GetUTF8(byte[] str, int start, MutableInteger ch)
		{
			int c, n, i, bytes;
			
			c = ((int) str[start]) & 0xFF; // Convert to unsigned.
			
			if ((c & 0xE0) == 0xC0)
			{
				/* 110X XXXX  two bytes */
				n = c & 31;
				bytes = 2;
			}
			else if ((c & 0xF0) == 0xE0)
			{
				/* 1110 XXXX  three bytes */
				n = c & 15;
				bytes = 3;
			}
			else if ((c & 0xF8) == 0xF0)
			{
				/* 1111 0XXX  four bytes */
				n = c & 7;
				bytes = 4;
			}
			else if ((c & 0xFC) == 0xF8)
			{
				/* 1111 10XX  five bytes */
				n = c & 3;
				bytes = 5;
			}
			else if ((c & 0xFE) == 0xFC)
			{
				/* 1111 110X  six bytes */
				n = c & 1;
				bytes = 6;
			}
			else
			{
				/* 0XXX XXXX one byte */
				ch.Val = c;
				return 0;
			}
			
			/* successor bytes should have the form 10XX XXXX */
			for (i = 1; i < bytes; ++i)
			{
				c = ((int) str[start + i]) & 0xFF; // Convert to unsigned.
				n = (n << 6) | (c & 0x3F);
			}
			
			ch.Val = n;
			return bytes - 1;
		}
		
		/* store char c as UTF-8 encoded byte stream */
		public static int PutUTF8(byte[] buf, int start, int c)
		{
			if (c < 128)
			{
				buf[start++] = (byte) c;
			}
			else if (c <= 0x7FF)
			{
				buf[start++] = (byte) (0xC0 | (c >> 6));
				buf[start++] = (byte) (0x80 | (c & 0x3F));
			}
			else if (c <= 0xFFFF)
			{
				buf[start++] = (byte) (0xE0 | (c >> 12));
				buf[start++] = (byte) (0x80 | ((c >> 6) & 0x3F));
				buf[start++] = (byte) (0x80 | (c & 0x3F));
			}
			else if (c <= 0x1FFFFF)
			{
				buf[start++] = (byte) (0xF0 | (c >> 18));
				buf[start++] = (byte) (0x80 | ((c >> 12) & 0x3F));
				buf[start++] = (byte) (0x80 | ((c >> 6) & 0x3F));
				buf[start++] = (byte) (0x80 | (c & 0x3F));
			}
			else
			{
				buf[start++] = (byte) (0xF8 | (c >> 24));
				buf[start++] = (byte) (0x80 | ((c >> 18) & 0x3F));
				buf[start++] = (byte) (0x80 | ((c >> 12) & 0x3F));
				buf[start++] = (byte) (0x80 | ((c >> 6) & 0x3F));
				buf[start++] = (byte) (0x80 | (c & 0x3F));
			}
			
			return start;
		}
		
		private void AddC(int c, int index)
		{
			if (index + 1 >= lbufsize)
			{
				while (index + 1 >= lbufsize)
				{
					if (lbufsize == 0)
						lbufsize = 256;
					else
						lbufsize = lbufsize * 2;
				}
				
				int[] temp = new int[lbufsize];
				if (linebuf != null)
					Array.Copy(linebuf, 0, temp, 0, index);
				linebuf = temp;
			}
			
			linebuf[index] = c;
		}
		
		private void WrapLine(Out fout, int indent)
		{
			int i, p, q;
			
			if (wraphere == 0)
			{
				return;
			}
			
			for (i = 0; i < indent; ++i)
			{
				fout.Outc((int) ' ');
			}
			
			for (i = 0; i < wraphere; ++i)
			{
				fout.Outc(linebuf[i]);
			}

			if (InString)
			{
				fout.Outc((int) ' ');
				fout.Outc((int) '\\');
			}
			
			fout.Newline();
			
			if (linelen > wraphere)
			{
				p = 0;
				
				if (linebuf[wraphere] == ' ')
				{
					++wraphere;
				}
				
				q = wraphere;
				AddC('\x0000', linelen);
				
				while (true)
				{
					linebuf[p] = linebuf[q];
					if (linebuf[q] == 0)
					{
						break;
					}
					p++;
					q++;
				}
				linelen -= wraphere;
			}
			else
			{
				linelen = 0;
			}
			
			wraphere = 0;
		}
		
		private void WrapAttrVal(Out fout, int indent, bool inString)
		{
			int i, p, q;
			
			for (i = 0; i < indent; ++i)
			{
				fout.Outc((int) ' ');
			}
			
			for (i = 0; i < wraphere; ++i)
			{
				fout.Outc(linebuf[i]);
			}
			
			fout.Outc((int) ' ');
			
			if (inString)
			{
				fout.Outc((int) '\\');
			}
			
			fout.Newline();
			
			if (linelen > wraphere)
			{
				p = 0;

				if (linebuf[wraphere] == ' ')
				{
					++wraphere;
				}
				
				q = wraphere;
				AddC('\x0000', linelen);
				
				while (true)
				{
					linebuf[p] = linebuf[q];
					if (linebuf[q] == 0)
					{
						break;
					}
					p++;
					q++;
				}
				linelen -= wraphere;
			}
			else
			{
				linelen = 0;
			}

			wraphere = 0;
		}

		public virtual void FlushLine(Out fout, int indent)
		{
			int i;
			
			if (linelen > 0)
			{
				if (indent + linelen >= _options.WrapLen)
				{
					WrapLine(fout, indent);
				}
				
				if (!inAttVal || _options.IndentAttributes)
				{
					for (i = 0; i < indent; ++i)
					{
						fout.Outc((int) ' ');
					}
				}
				
				for (i = 0; i < linelen; ++i)
				{
					fout.Outc(linebuf[i]);
				}
			}
			
			fout.Newline();
			linelen = 0;
			wraphere = 0;
			inAttVal = false;
		}
		
		public virtual void CondFlushLine(Out fout, int indent)
		{
			int i;
			
			if (linelen > 0)
			{
				if (indent + linelen >= _options.WrapLen)
				{
					WrapLine(fout, indent);
				}
				
				if (!inAttVal || _options.IndentAttributes)
				{
					for (i = 0; i < indent; ++i)
					{
						fout.Outc((int) ' ');
					}
				}
				
				for (i = 0; i < linelen; ++i)
				{
					fout.Outc(linebuf[i]);
				}
				
				fout.Newline();
				linelen = 0;
				wraphere = 0;
				inAttVal = false;
			}
		}
		
		private void PrintChar(int c, int mode)
		{
			string entity;
			
			if (c == ' ' && !((mode & (PREFORMATTED | COMMENT | ATTRIBVALUE)) != 0))
			{
				/* coerce a space character to a non-breaking space */
				if ((mode & NOWRAP) != 0)
				{
					/* by default XML doesn't define &nbsp; */
					if (_options.NumEntities || _options.XmlTags)
					{
						AddC('&', linelen++);
						AddC('#', linelen++);
						AddC('1', linelen++);
						AddC('6', linelen++);
						AddC('0', linelen++);
						AddC(';', linelen++);
					}
						/* otherwise use named entity */
					else
					{
						AddC('&', linelen++);
						AddC('n', linelen++);
						AddC('b', linelen++);
						AddC('s', linelen++);
						AddC('p', linelen++);
						AddC(';', linelen++);
					}
					return;
				}
				else
				{
					wraphere = linelen;
				}
			}
			
			/* comment characters are passed raw */
			if ((mode & COMMENT) != 0)
			{
				AddC(c, linelen++);
				return;
			}
			
			/* except in CDATA map < to &lt; etc. */
			if (!((mode & CDATA) != 0))
			{
				if (c == '<')
				{
					AddC('&', linelen++);
					AddC('l', linelen++);
					AddC('t', linelen++);
					AddC(';', linelen++);
					return;
				}
				
				if (c == '>')
				{
					AddC('&', linelen++);
					AddC('g', linelen++);
					AddC('t', linelen++);
					AddC(';', linelen++);
					return;
				}
				
				/*
				naked '&' chars can be left alone or
				quoted as &amp; The latter is required
				for XML where naked '&' are illegal.
				*/
				if (c == '&' && _options.QuoteAmpersand)
				{
					AddC('&', linelen++);
					AddC('a', linelen++);
					AddC('m', linelen++);
					AddC('p', linelen++);
					AddC(';', linelen++);
					return;
				}
				
				if (c == '"' && _options.QuoteMarks)
				{
					AddC('&', linelen++);
					AddC('q', linelen++);
					AddC('u', linelen++);
					AddC('o', linelen++);
					AddC('t', linelen++);
					AddC(';', linelen++);
					return;
				}
				
				if (c == '\'' && _options.QuoteMarks)
				{
					AddC('&', linelen++);
					AddC('#', linelen++);
					AddC('3', linelen++);
					AddC('9', linelen++);
					AddC(';', linelen++);
					return;
				}
				
				if (c == 160 && _options.CharEncoding != CharEncoding.Raw)
				{
					if (_options.QuoteNbsp)
					{
						AddC('&', linelen++);
						
						if (_options.NumEntities)
						{
							AddC('#', linelen++);
							AddC('1', linelen++);
							AddC('6', linelen++);
							AddC('0', linelen++);
						}
						else
						{
							AddC('n', linelen++);
							AddC('b', linelen++);
							AddC('s', linelen++);
							AddC('p', linelen++);
						}
						
						AddC(';', linelen++);
					}
					else
					{
						AddC(c, linelen++);
					}

					return;
				}
			}
			
			/* otherwise ISO 2022 characters are passed raw */
			if (_options.CharEncoding == CharEncoding.ISO2022 || _options.CharEncoding == CharEncoding.Raw)
			{
				AddC(c, linelen++);
				return;
			}
			
			/* if preformatted text, map &nbsp; to space */
			if (c == 160 && ((mode & PREFORMATTED) != 0))
			{
				AddC(' ', linelen++);
				return;
			}
			
			/*
			Filters from Word and PowerPoint often use smart
			quotes resulting in character codes between 128
			and 159. Unfortunately, the corresponding HTML 4.0
			entities for these are not widely supported. The
			following converts dashes and quotation marks to
			the nearest ASCII equivalent. My thanks to
			Andrzej Novosiolov for his help with this code.
			*/
			
			if (_options.MakeClean)
			{
				if (c >= 0x2013 && c <= 0x201E)
				{
					switch (c)
					{
					case 0x2013: 
					case 0x2014: 
						c = '-';
						break;

					case 0x2018: 
					case 0x2019: 
					case 0x201A: 
						c = '\'';
						break;

					case 0x201C: 
					case 0x201D: 
					case 0x201E: 
						c = '"';
						break;
					}
				}
			}
			
			/* don't map latin-1 chars to entities */
			if (_options.CharEncoding == CharEncoding.Latin1)
			{
				if (c > 255)
				{
					/* multi byte chars */
					if (!_options.NumEntities)
					{
						entity = EntityTable.DefaultEntityTable.EntityName((short) c);
						if (entity != null)
						{
							entity = "&" + entity + ";";
						}
						else
						{
							entity = "&#" + c + ";";
						}
					}
					else
					{
						entity = "&#" + c + ";";
					}
					
					for (int i = 0; i < entity.Length; i++)
					{
						AddC((int) entity[i], linelen++);
					}
					
					return;
				}
				
				if (c > 126 && c < 160)
				{
					entity = "&#" + c + ";";
					
					for (int i = 0; i < entity.Length; i++)
					{
						AddC((int) entity[i], linelen++);
					}
					
					return;
				}
				
				AddC(c, linelen++);
				return;
			}
			
			/* don't map utf8 chars to entities */
			if (_options.CharEncoding == CharEncoding.UTF8)
			{
				AddC(c, linelen++);
				return;
			}
			
			/* use numeric entities only  for XML */
			if (_options.XmlTags)
			{
				/* if ASCII use numeric entities for chars > 127 */
				if (c > 127 && _options.CharEncoding == CharEncoding.ASCII)
				{
					entity = "&#" + c + ";";
					
					for (int i = 0; i < entity.Length; i++)
					{
						AddC((int) entity[i], linelen++);
					}
					
					return;
				}
				
				/* otherwise output char raw */
				AddC(c, linelen++);
				return;
			}
			
			/* default treatment for ASCII */
			if (c > 126 || (c < ' ' && c != '\t'))
			{
				if (!_options.NumEntities)
				{
					entity = EntityTable.DefaultEntityTable.EntityName((short) c);
					if (entity != null)
					{
						entity = "&" + entity + ";";
					}
					else
					{
						entity = "&#" + c + ";";
					}
				}
				else
				{
					entity = "&#" + c + ";";
				}
				
				for (int i = 0; i < entity.Length; i++)
				{
					AddC((int) entity[i], linelen++);
				}
				
				return;
			}
			
			AddC(c, linelen++);
		}
		
		/* 
		The line buffer is uint not char so we can
		hold Unicode values unencoded. The translation
		to UTF-8 is deferred to the outc routine called
		to flush the line buffer.
		*/
		private void PrintText(Out fout, int mode, int indent, byte[] textarray, int start, int end)
		{
			int i, c;
			MutableInteger ci = new MutableInteger();
			
			for (i = start; i < end; ++i)
			{
				if (indent + linelen >= _options.WrapLen)
				{
					WrapLine(fout, indent);
				}
				
				c = ((int) textarray[i]) & 0xFF; // Convert to unsigned.
				
				/* look for UTF-8 multibyte character */
				if (c > 0x7F)
				{
					i += GetUTF8(textarray, i, ci);
					c = ci.Val;
				}
				
				if (c == '\n')
				{
					FlushLine(fout, indent);
					continue;
				}
				
				PrintChar(c, mode);
			}
		}
		
		private void PrintString(Out fout, int indent, string str)
		{
			for (int i = 0; i < str.Length; i++)
			{
				AddC((int) str[i], linelen++);
			}
		}
		
		private void PrintAttrValue(Out fout, int indent, string val, int delim, bool wrappable)
		{
			int c;
			MutableInteger ci = new MutableInteger();
			bool wasinstring = false;
			byte[] valueChars = null;
			int i;
			int mode = (wrappable?(int) (NORMAL | ATTRIBVALUE):(int) (PREFORMATTED | ATTRIBVALUE));
			
			if (val != null)
			{
				valueChars = Lexer.GetBytes(val);
			}
			
			/* look for ASP, Tango or PHP instructions for computed attribute value */
			if (valueChars != null && valueChars.Length >= 5 && valueChars[0] == '<')
			{
				char[] tmpChar;
				tmpChar = new char[valueChars.Length];
				valueChars.CopyTo(tmpChar, 0);
				if (valueChars[1] == '%' || valueChars[1] == '@' || (new string(tmpChar, 0, 5)).Equals("<?php"))
					mode |= CDATA;
			}
			
			if (delim == 0)
			{
				delim = '"';
			}

			AddC('=', linelen++);
			
			/* don't wrap after "=" for xml documents */
			if (!_options.XmlOut)
			{
				if (indent + linelen < _options.WrapLen)
				{
					wraphere = linelen;
				}
				
				if (indent + linelen >= _options.WrapLen)
				{
					WrapLine(fout, indent);
				}
				
				if (indent + linelen < _options.WrapLen)
				{
					wraphere = linelen;
				}
				else
				{
					CondFlushLine(fout, indent);
				}
			}
			
			AddC(delim, linelen++);
			
			if (val != null)
			{
				InString = false;
				
				i = 0;
				while (i < valueChars.Length)
				{
					c = ((int) valueChars[i]) & 0xFF; // Convert to unsigned.
					
					if (wrappable && c == ' ' && indent + linelen < _options.WrapLen)
					{
						wraphere = linelen;
						wasinstring = InString;
					}
					
					if (wrappable && wraphere > 0 && indent + linelen >= _options.WrapLen)
						WrapAttrVal(fout, indent, wasinstring);
					
					if (c == delim)
					{
						string entity;
						
						entity = (c == '"'?"&quot;":"&#39;");
						
						for (int j = 0; j < entity.Length; j++)
						{
							AddC(entity[j], linelen++);
						}

						++i;
						continue;
					}
					else if (c == '"')
					{
						if (_options.QuoteMarks)
						{
							AddC('&', linelen++);
							AddC('q', linelen++);
							AddC('u', linelen++);
							AddC('o', linelen++);
							AddC('t', linelen++);
							AddC(';', linelen++);
						}
						else
						{
							AddC('"', linelen++);
						}
						
						if (delim == '\'')
						{
							InString = !InString;
						}
						
						++i;
						continue;
					}
					else if (c == '\'')
					{
						if (_options.QuoteMarks)
						{
							AddC('&', linelen++);
							AddC('#', linelen++);
							AddC('3', linelen++);
							AddC('9', linelen++);
							AddC(';', linelen++);
						}
						else
						{
							AddC('\'', linelen++);
						}
						
						if (delim == '"')
						{
							InString = !InString;
						}
						
						++i;
						continue;
					}
					
					/* look for UTF-8 multibyte character */
					if (c > 0x7F)
					{
						i += GetUTF8(valueChars, i, ci);
						c = ci.Val;
					}
					
					++i;
					
					if (c == '\n')
					{
						FlushLine(fout, indent);
						continue;
					}
					
					PrintChar(c, mode);
				}
			}
			
			InString = false;
			AddC(delim, linelen++);
		}
		
		private void PrintAttribute(Out fout, int indent, Node node, AttVal attr)
		{
			string name;
			bool wrappable = false;
			
			if (_options.IndentAttributes)
			{
				FlushLine(fout, indent);
				indent += _options.Spaces;
			}
			
			name = attr.Attribute;
			
			if (indent + linelen >= _options.WrapLen)
			{
				WrapLine(fout, indent);
			}
			
			if (!_options.XmlTags && !_options.XmlOut && attr.Dict != null)
			{
				if (AttributeTable.DefaultAttributeTable.IsScript(name))
				{
					wrappable = _options.WrapScriptlets;
				}
				else if (!attr.Dict.Nowrap && _options.WrapAttVals)
				{
					wrappable = true;
				}
			}
			
			if (indent + linelen < _options.WrapLen)
			{
				wraphere = linelen;
				AddC(' ', linelen++);
			}
			else
			{
				CondFlushLine(fout, indent);
				AddC(' ', linelen++);
			}
			
			for (int i = 0; i < name.Length; i++)
			{
				AddC((int) Lexer.FoldCase(name[i], _options.UpperCaseAttrs, _options.XmlTags), linelen++);
			}
			
			if (indent + linelen >= _options.WrapLen)
			{
				WrapLine(fout, indent);
			}
			
			if (attr.Val == null)
			{
				if (_options.XmlTags || _options.XmlOut)
				{
					PrintAttrValue(fout, indent, attr.Attribute, attr.Delim, true);
				}
				else if (!attr.BoolAttribute && !Node.IsNewNode(node))
				{
					PrintAttrValue(fout, indent, "", attr.Delim, true);
				}
				else if (indent + linelen < _options.WrapLen)
				{
					wraphere = linelen;
				}
			}
			else
			{
				PrintAttrValue(fout, indent, attr.Val, attr.Delim, wrappable);
			}
		}
		
		private void PrintAttrs(Out fout, int indent, Node node, AttVal attr)
		{
			if (attr != null)
			{
				if (attr.Next != null)
				{
					PrintAttrs(fout, indent, node, attr.Next);
				}
				
				if (attr.Attribute != null)
				{
					PrintAttribute(fout, indent, node, attr);
				}
				else if (attr.Asp != null)
				{
					AddC(' ', linelen++);
					PrintAsp(fout, indent, attr.Asp);
				}
				else if (attr.Php != null)
				{
					AddC(' ', linelen++);
					PrintPhp(fout, indent, attr.Php);
				}
			}
			
			/* add xml:space attribute to pre and other elements */
			if (_options.XmlOut && _options.XmlSpace && ParserImpl.XMLPreserveWhiteSpace(node, _options.tt) && node.GetAttrByName("xml:space") == null)
			{
				PrintString(fout, indent, " xml:space=\"preserve\"");
			}
		}
		
		/*
		Line can be wrapped immediately after inline start tag provided
		if follows a text node ending in a space, or it parent is an
		inline element that that rule applies to. This behaviour was
		reverse engineered from Netscape 3.0
		*/
		private static bool AfterSpace(Node node)
		{
			Node prev;
			int c;
			
			if (node == null || node.Tag == null || !((node.Tag.Model & ContentModel.Inline) != 0))
			{
				return true;
			}
			
			prev = node.Prev;
			
			if (prev != null)
			{
				if (prev.Type == Node.TextNode && prev.End > prev.Start)
				{
					c = ((int) prev.Textarray[prev.End - 1]) & 0xFF; // Convert to unsigned.
					
					if (c == 160 || c == ' ' || c == '\n')
					{
						return true;
					}
				}
				
				return false;
			}
			
			return AfterSpace(node.Parent);
		}
		
		private void PrintTag(Lexer lexer, Out fout, int mode, int indent, Node node)
		{
			string p;
			TagTable tt = _options.tt;
			
			AddC('<', linelen++);
			
			if (node.Type == Node.EndTag)
			{
				AddC('/', linelen++);
			}
			
			p = node.Element;
			for (int i = 0; i < p.Length; i++)
			{
				AddC((int) Lexer.FoldCase(p[i], _options.UpperCaseTags, _options.XmlTags), linelen++);
			}
			
			PrintAttrs(fout, indent, node, node.Attributes);
			
			if ((_options.XmlOut || lexer != null && lexer.isvoyager) && (node.Type == Node.StartEndTag || (node.Tag.Model & ContentModel.Empty) != 0))
			{
				AddC(' ', linelen++); /* compatibility hack */
				AddC('/', linelen++);
			}
			
			AddC('>', linelen++); ;
			
			if (node.Type != Node.StartEndTag && !((mode & PREFORMATTED) != 0))
			{
				if (indent + linelen >= _options.WrapLen)
				{
					WrapLine(fout, indent);
				}
				
				if (indent + linelen < _options.WrapLen)
				{
					/*
					wrap after start tag if is <br/> or if it's not
					inline or it is an empty tag followed by </a>
					*/
					if (AfterSpace(node))
					{
						if (!((mode & NOWRAP) != 0) && (!((node.Tag.Model & ContentModel.Inline) != 0) || (node.Tag == tt.TagBr) || (((node.Tag.Model & ContentModel.Empty) != 0) && node.Next == null && node.Parent.Tag == tt.TagA)))
						{
							wraphere = linelen;
						}
					}
				}
				else
				{
					CondFlushLine(fout, indent);
				}
			}
		}
		
		private void PrintEndTag(Out fout, int mode, int indent, Node node)
		{
			string p;
			
			/*
			Netscape ignores SGML standard by not ignoring a
			line break before </A> or </U> etc. To avoid rendering 
			this as an underlined space, I disable line wrapping
			before inline end tags by the #if 0 ... #endif
			*/
			//if (false)
			//{
			//	if (indent + linelen < _options.WrapLen && !((mode & NOWRAP) != 0))
			//		wraphere = linelen;
			//}
			
			AddC('<', linelen++);
			AddC('/', linelen++);
			
			p = node.Element;
			for (int i = 0; i < p.Length; i++)
			{
				AddC((int) Lexer.FoldCase(p[i], _options.UpperCaseTags, _options.XmlTags), linelen++);
			}
			
			AddC('>', linelen++);
		}
		
		private void PrintComment(Out fout, int indent, Node node)
		{
			if (indent + linelen < _options.WrapLen)
			{
				wraphere = linelen;
			}
			
			AddC('<', linelen++);
			AddC('!', linelen++);
			AddC('-', linelen++);
			AddC('-', linelen++);
			PrintText(fout, COMMENT, indent, node.Textarray, node.Start, node.End);
			// See Lexer.java: AQ 8Jul2000
			AddC('-', linelen++);
			AddC('-', linelen++);
			AddC('>', linelen++);
			
			if (node.Linebreak)
			{
				FlushLine(fout, indent);
			}
		}
		
		private void PrintDocType(Out fout, int indent, Node node)
		{
			bool q = _options.QuoteMarks;
			
			_options.QuoteMarks = false;
			
			if (indent + linelen < _options.WrapLen)
			{
				wraphere = linelen;
			}
			
			CondFlushLine(fout, indent);
			
			AddC('<', linelen++);
			AddC('!', linelen++);
			AddC('D', linelen++);
			AddC('O', linelen++);
			AddC('C', linelen++);
			AddC('T', linelen++);
			AddC('Y', linelen++);
			AddC('P', linelen++);
			AddC('E', linelen++);
			AddC(' ', linelen++);
			
			if (indent + linelen < _options.WrapLen)
			{
				wraphere = linelen;
			}
			
			PrintText(fout, (short) 0, indent, node.Textarray, node.Start, node.End);
			
			if (linelen < _options.WrapLen)
			{
				wraphere = linelen;
			}
			
			AddC('>', linelen++);
			_options.QuoteMarks = q;
			CondFlushLine(fout, indent);
		}
		
		private void PrintPI(Out fout, int indent, Node node)
		{
			if (indent + linelen < _options.WrapLen)
			{
				wraphere = linelen;
			}
			
			AddC('<', linelen++);
			AddC('?', linelen++);
			
			/* set CDATA to pass < and > unescaped */
			PrintText(fout, CDATA, indent, node.Textarray, node.Start, node.End);
			
			if (node.Textarray[node.End - 1] != (byte) '?')
			{
				AddC('?', linelen++);
			}
			
			AddC('>', linelen++);
			CondFlushLine(fout, indent);
		}
		
		/* note ASP and JSTE share <% ... %> syntax */
		private void PrintAsp(Out fout, int indent, Node node)
		{
			int savewraplen = _options.WrapLen;
			
			/* disable wrapping if so requested */
			
			if (!_options.WrapAsp || !_options.WrapJste)
			{
				_options.WrapLen = 0xFFFFFF;
			}
			/* a very large number */
			
			AddC('<', linelen++);
			AddC('%', linelen++);
			
			PrintText(fout, (_options.WrapAsp?CDATA:COMMENT), indent, node.Textarray, node.Start, node.End);
			
			AddC('%', linelen++);
			AddC('>', linelen++);

			/* CondFlushLine(fout, indent); */
			_options.WrapLen = savewraplen;
		}
		
		/* JSTE also supports <# ... #> syntax */
		private void PrintJste(Out fout, int indent, Node node)
		{
			int savewraplen = _options.WrapLen;
			
			/* disable wrapping if so requested */
			
			if (!_options.WrapJste)
			{
				_options.WrapLen = 0xFFFFFF;
			}

			/* a very large number */
			
			AddC('<', linelen++);
			AddC('#', linelen++);
			
			PrintText(fout, (_options.WrapJste?CDATA:COMMENT), indent, node.Textarray, node.Start, node.End);
			
			AddC('#', linelen++);
			AddC('>', linelen++);
			/* CondFlushLine(fout, indent); */
			_options.WrapLen = savewraplen;
		}
		
		/* PHP is based on XML processing instructions */
		private void PrintPhp(Out fout, int indent, Node node)
		{
			int savewraplen = _options.WrapLen;
			
			/* disable wrapping if so requested */
			
			if (!_options.WrapPhp)
			{
				_options.WrapLen = 0xFFFFFF;
			}
			/* a very large number */
			AddC('<', linelen++);
			AddC('?', linelen++);
			
			PrintText(fout, (_options.WrapPhp?CDATA:COMMENT), indent, node.Textarray, node.Start, node.End);
			
			AddC('?', linelen++);
			AddC('>', linelen++);
			/* PCondFlushLine(fout, indent); */
			_options.WrapLen = savewraplen;
		}
		
		private void PrintCDATA(Out fout, int indent, Node node)
		{
			int savewraplen = _options.WrapLen;
			
			CondFlushLine(fout, indent);
			
			/* disable wrapping */
			
			_options.WrapLen = 0xFFFFFF; /* a very large number */
			
			AddC('<', linelen++);
			AddC('!', linelen++);
			AddC('[', linelen++);
			AddC('C', linelen++);
			AddC('D', linelen++);
			AddC('A', linelen++);
			AddC('T', linelen++);
			AddC('A', linelen++);
			AddC('[', linelen++);
			
			PrintText(fout, COMMENT, indent, node.Textarray, node.Start, node.End);
			
			AddC(']', linelen++);
			AddC(']', linelen++);
			AddC('>', linelen++);
			CondFlushLine(fout, indent);
			_options.WrapLen = savewraplen;
		}
		
		private void PrintSection(Out fout, int indent, Node node)
		{
			int savewraplen = _options.WrapLen;
			
			/* disable wrapping if so requested */
			if (!_options.WrapSection)
			{
				_options.WrapLen = 0xFFFFFF;
			}

			/* a very large number */
			
			AddC('<', linelen++);
			AddC('!', linelen++);
			AddC('[', linelen++);
			
			PrintText(fout, (_options.WrapSection?CDATA:COMMENT), indent, node.Textarray, node.Start, node.End);
			
			AddC(']', linelen++);
			AddC('>', linelen++);
			/* PCondFlushLine(fout, indent); */
			_options.WrapLen = savewraplen;
		}
		
		private bool ShouldIndent(Node node)
		{
			TagTable tt = _options.tt;
			
			if (!_options.IndentContent)
				return false;
			
			if (_options.SmartIndent)
			{
				if (node.Content != null && ((node.Tag.Model & ContentModel.NoIndent) != 0))
				{
					for (node = node.Content; node != null; node = node.Next)
					{
						if (node.Tag != null && (node.Tag.Model & ContentModel.Block) != 0)
						{
							return true;
						}
					}
					
					return false;
				}
				
				if ((node.Tag.Model & ContentModel.Heading) != 0)
				{
					return false;
				}
				
				if (node.Tag == tt.TagP)
				{
					return false;
				}
				
				if (node.Tag == tt.TagTitle)
				{
					return false;
				}
			}
			
			if ((node.Tag.Model & (ContentModel.Field | ContentModel.Object)) != 0)
			{
				return true;
			}
			
			if (node.Tag == tt.TagMap)
			{
				return true;
			}
			
			return !((node.Tag.Model & ContentModel.Inline) != 0);
		}
		
		public virtual void PrintTree(Out fout, int mode, int indent, Lexer lexer, Node node)
		{
			Node content, last;
			TagTable tt = _options.tt;
			
			if (node == null)
				return;
			
			if (node.Type == Node.TextNode)
			{
				PrintText(fout, mode, indent, node.Textarray, node.Start, node.End);
			}
			else if (node.Type == Node.CommentTag)
			{
				PrintComment(fout, indent, node);
			}
			else if (node.Type == Node.RootNode)
			{
				for (content = node.Content; content != null; content = content.Next)
				{
					PrintTree(fout, mode, indent, lexer, content);
				}
			}
			else if (node.Type == Node.DocTypeTag)
			{
				PrintDocType(fout, indent, node);
			}
			else if (node.Type == Node.ProcInsTag)
			{
				PrintPI(fout, indent, node);
			}
			else if (node.Type == Node.CDATATag)
			{
				PrintCDATA(fout, indent, node);
			}
			else if (node.Type == Node.SectionTag)
			{
				PrintSection(fout, indent, node);
			}
			else if (node.Type == Node.AspTag)
			{
				PrintAsp(fout, indent, node);
			}
			else if (node.Type == Node.JsteTag)
			{
				PrintJste(fout, indent, node);
			}
			else if (node.Type == Node.PhpTag)
			{
				PrintPhp(fout, indent, node);
			}
			else if ((node.Tag.Model & ContentModel.Empty) != 0 || node.Type == Node.StartEndTag)
			{
				if (!((node.Tag.Model & ContentModel.Inline) != 0))
				{
					CondFlushLine(fout, indent);
				}
				
				if (node.Tag == tt.TagBr && node.Prev != null && node.Prev.Tag != tt.TagBr && _options.BreakBeforeBR)
				{
					FlushLine(fout, indent);
				}
				
				if (_options.MakeClean && node.Tag == tt.TagWbr)
				{
					PrintString(fout, indent, " ");
				}
				else
				{
					PrintTag(lexer, fout, mode, indent, node);
				}
				
				if (node.Tag == tt.TagParam || node.Tag == tt.TagArea)
				{
					CondFlushLine(fout, indent);
				}
				else if (node.Tag == tt.TagBr || node.Tag == tt.TagHr)
				{
					FlushLine(fout, indent);
				}
			}
			else
			{
				/* some kind of container element */
				if (node.Tag != null && node.Tag.Parser == ParserImpl.ParsePre)
				{
					CondFlushLine(fout, indent);
					
					indent = 0;
					CondFlushLine(fout, indent);
					PrintTag(lexer, fout, mode, indent, node);
					FlushLine(fout, indent);
					
					for (content = node.Content; content != null; content = content.Next)
					{
						PrintTree(fout, (int) (mode | PREFORMATTED | NOWRAP), indent, lexer, content);
					}
					
					CondFlushLine(fout, indent);
					PrintEndTag(fout, mode, indent, node);
					FlushLine(fout, indent);
					
					if (_options.IndentContent == false && node.Next != null)
					{
						FlushLine(fout, indent);
					}
				}
				else if (node.Tag == tt.TagStyle || node.Tag == tt.TagScript)
				{
					CondFlushLine(fout, indent);
					
					indent = 0;
					CondFlushLine(fout, indent);
					PrintTag(lexer, fout, mode, indent, node);
					FlushLine(fout, indent);
					
					for (content = node.Content; content != null; content = content.Next)
					{
						PrintTree(fout, (int) (mode | PREFORMATTED | NOWRAP | CDATA), indent, lexer, content);
					}
					
					CondFlushLine(fout, indent);
					PrintEndTag(fout, mode, indent, node);
					FlushLine(fout, indent);
					
					if (_options.IndentContent == false && node.Next != null)
					{
						FlushLine(fout, indent);
					}
				}
				else if ((node.Tag.Model & ContentModel.Inline) != 0)
				{
					if (_options.MakeClean)
					{
						/* discards <font> and </font> tags */
						if (node.Tag == tt.TagFont)
						{
							for (content = node.Content; content != null; content = content.Next)
							{
								PrintTree(fout, mode, indent, lexer, content);
							}
							return;
						}
						
						/* replace <nobr>...</nobr> by &nbsp; or &#160; etc. */
						if (node.Tag == tt.TagNobr)
						{
							for (content = node.Content; content != null; content = content.Next)
							{
								PrintTree(fout, (int) (mode | NOWRAP), indent, lexer, content);
							}
							return;
						}
					}
					
					/* otherwise a normal inline element */
					
					PrintTag(lexer, fout, mode, indent, node);
					
					/* indent content for SELECT, TEXTAREA, MAP, OBJECT and APPLET */
					
					if (ShouldIndent(node))
					{
						CondFlushLine(fout, indent);
						indent += _options.Spaces;
						
						for (content = node.Content; content != null; content = content.Next)
						{
							PrintTree(fout, mode, indent, lexer, content);
						}
						
						CondFlushLine(fout, indent);
						indent -= _options.Spaces;
						CondFlushLine(fout, indent);
					}
					else
					{
						for (content = node.Content; content != null; content = content.Next)
						{
							PrintTree(fout, mode, indent, lexer, content);
						}
					}
					
					PrintEndTag(fout, mode, indent, node);
				}
				else
				{
					/* other tags */
					CondFlushLine(fout, indent);
					
					if (_options.SmartIndent && node.Prev != null)
					{
						FlushLine(fout, indent);
					}

					if (_options.HideEndTags == false || !(node.Tag != null && ((node.Tag.Model & ContentModel.OmitSt) != 0)))
					{
						PrintTag(lexer, fout, mode, indent, node);
						
						if (ShouldIndent(node))
						{
							CondFlushLine(fout, indent);
						}
						else if ((node.Tag.Model & ContentModel.Html) != 0 || node.Tag == tt.TagNoframes || ((node.Tag.Model & ContentModel.Head) != 0 && !(node.Tag == tt.TagTitle)))
						{
							FlushLine(fout, indent);
						}
					}
					
					if (node.Tag == tt.TagBody && _options.BurstSlides)
					{
						PrintSlide(fout, mode, (_options.IndentContent?indent + _options.Spaces:indent), lexer);
					}
					else
					{
						last = null;
						
						for (content = node.Content; content != null; content = content.Next)
						{
							/* kludge for naked text before block level tag */
							if (last != null && !_options.IndentContent && last.Type == Node.TextNode && content.Tag != null && (content.Tag.Model & ContentModel.Block) != 0)
							{
								FlushLine(fout, indent);
								FlushLine(fout, indent);
							}
							
							PrintTree(fout, mode, (ShouldIndent(node)?indent + _options.Spaces:indent), lexer, content);
							
							last = content;
						}
					}
					
					/* don't flush line for td and th */
					if (ShouldIndent(node) || (((node.Tag.Model & ContentModel.Html) != 0 || node.Tag == tt.TagNoframes || ((node.Tag.Model & ContentModel.Head) != 0 && !(node.Tag == tt.TagTitle))) && _options.HideEndTags == false))
					{
						CondFlushLine(fout, (_options.IndentContent?indent + _options.Spaces:indent));
						
						if (_options.HideEndTags == false || !((node.Tag.Model & ContentModel.Opt) != 0))
						{
							PrintEndTag(fout, mode, indent, node);
							FlushLine(fout, indent);
						}
					}
					else
					{
						if (_options.HideEndTags == false || !((node.Tag.Model & ContentModel.Opt) != 0))
						{
							PrintEndTag(fout, mode, indent, node);
						}
						
						FlushLine(fout, indent);
					}
					
					if (_options.IndentContent == false && node.Next != null && _options.HideEndTags == false && (node.Tag.Model & (ContentModel.Block | ContentModel.List | ContentModel.Deflist | ContentModel.Table)) != 0)
					{
						FlushLine(fout, indent);
					}
				}
			}
		}
		
		public virtual void PrintXmlTree(Out fout, int mode, int indent, Lexer lexer, Node node)
		{
			TagTable tt = _options.tt;
			
			if (node == null)
			{
				return;
			}

			if (node.Type == Node.TextNode)
			{
				PrintText(fout, mode, indent, node.Textarray, node.Start, node.End);
			}
			else if (node.Type == Node.CommentTag)
			{
				CondFlushLine(fout, indent);
				PrintComment(fout, 0, node);
				CondFlushLine(fout, 0);
			}
			else if (node.Type == Node.RootNode)
			{
				Node content;
				
				for (content = node.Content; content != null; content = content.Next)
				{
					PrintXmlTree(fout, mode, indent, lexer, content);
				}
			}
			else if (node.Type == Node.DocTypeTag)
			{
				PrintDocType(fout, indent, node);
			}
			else if (node.Type == Node.ProcInsTag)
			{
				PrintPI(fout, indent, node);
			}
			else if (node.Type == Node.SectionTag)
			{
				PrintSection(fout, indent, node);
			}
			else if (node.Type == Node.AspTag)
			{
				PrintAsp(fout, indent, node);
			}
			else if (node.Type == Node.JsteTag)
			{
				PrintJste(fout, indent, node);
			}
			else if (node.Type == Node.PhpTag)
			{
				PrintPhp(fout, indent, node);
			}
			else if ((node.Tag.Model & ContentModel.Empty) != 0 || node.Type == Node.StartEndTag)
			{
				CondFlushLine(fout, indent);
				PrintTag(lexer, fout, mode, indent, node);
				FlushLine(fout, indent);
				
				if (node.Next != null)
				{
					FlushLine(fout, indent);
				}
			}
			else
			{
				/* some kind of container element */
				Node content;
				bool mixed = false;
				int cindent;
				
				for (content = node.Content; content != null; content = content.Next)
				{
					if (content.Type == Node.TextNode)
					{
						mixed = true;
						break;
					}
				}
				
				CondFlushLine(fout, indent);
				
				if (ParserImpl.XMLPreserveWhiteSpace(node, tt))
				{
					indent = 0;
					cindent = 0;
					mixed = false;
				}
				else if (mixed)
				{
					cindent = indent;
				}
				else
				{
					cindent = indent + _options.Spaces;
				}
				
				PrintTag(lexer, fout, mode, indent, node);
				
				if (!mixed)
				{
					FlushLine(fout, indent);
				}
				
				for (content = node.Content; content != null; content = content.Next)
				{
					PrintXmlTree(fout, mode, cindent, lexer, content);
				}
				
				if (!mixed)
				{
					CondFlushLine(fout, cindent);
				}
				PrintEndTag(fout, mode, indent, node);
				CondFlushLine(fout, indent);
				
				if (node.Next != null)
				{
					FlushLine(fout, indent);
				}
			}
		}
		
		
		/* split parse tree by h2 elements and output to separate files */
		
		/* counts number of h2 children belonging to node */
		public virtual int CountSlides(Node node)
		{
			int n = 1;
			TagTable tt = _options.tt;
			
			for (node = node.Content; node != null; node = node.Next)
			{
				if (node.Tag == tt.TagH2)
				{
					++n;
				}
			}
			
			return n;
		}
		
		/*
		inserts a space gif called "dot.gif" to ensure
		that the  slide is at least n pixels high
		*/
		private void PrintVertSpacer(Out fout, int indent)
		{
			CondFlushLine(fout, indent);
			PrintString(fout, indent, "<img width=\"0\" height=\"0\" hspace=\"1\" src=\"dot.gif\" vspace=\"%d\" align=\"left\">");
			CondFlushLine(fout, indent);
		}
		
		private void PrintNavBar(Out fout, int indent)
		{
			string buf;
			
			CondFlushLine(fout, indent);
			PrintString(fout, indent, "<center><small>");
			
			if (slide > 1)
			{
				buf = "<a href=\"slide" + (slide - 1).ToString() + ".html\">previous</a> | ";
				PrintString(fout, indent, buf);
				CondFlushLine(fout, indent);
				
				if (slide < count)
				{
					PrintString(fout, indent, "<a href=\"slide1.html\">start</a> | ");
				}
				else
				{
					PrintString(fout, indent, "<a href=\"slide1.html\">start</a>");
				}
				
				CondFlushLine(fout, indent);
			}
			
			if (slide < count)
			{
				buf = "<a href=\"slide" + (slide + 1).ToString() + ".html\">next</a>";
				PrintString(fout, indent, buf);
			}
			
			PrintString(fout, indent, "</small></center>");
			CondFlushLine(fout, indent);
		}
		
		/*
		Called from printTree to print the content of a slide from
		the node slidecontent. On return slidecontent points to the
		node starting the next slide or null. The variables slide
		and count are used to customise the navigation bar.
		*/
		public virtual void PrintSlide(Out fout, int mode, int indent, Lexer lexer)
		{
			Node content, last;
			TagTable tt = _options.tt;
			
			/* insert div for onclick handler */
			string s;
			s = "<div onclick=\"document.location='slide" + (slide < count?slide + 1:1).ToString() + ".html'\">";
			PrintString(fout, indent, s);
			CondFlushLine(fout, indent);
			
			/* first print the h2 element and navbar */
			if (slidecontent.Tag == tt.TagH2)
			{
				PrintNavBar(fout, indent);
				
				/* now print an hr after h2 */
				
				AddC('<', linelen++);
				
				
				AddC((int) Lexer.FoldCase('h', _options.UpperCaseTags, _options.XmlTags), linelen++);
				AddC((int) Lexer.FoldCase('r', _options.UpperCaseTags, _options.XmlTags), linelen++);
				
				if (_options.XmlOut == true)
				{
					PrintString(fout, indent, " />");
				}
				else
				{
					AddC('>', linelen++);
				}

				if (_options.IndentContent == true)
				{
					CondFlushLine(fout, indent);
				}

				/* PrintVertSpacer(fout, indent); */
				
				/*CondFlushLine(fout, indent); */
				
				/* print the h2 element */
				PrintTree(fout, mode, (_options.IndentContent?indent + _options.Spaces:indent), lexer, slidecontent);
				
				slidecontent = slidecontent.Next;
			}
			
			/* now continue until we reach the next h2 */
			
			last = null;
			content = slidecontent;
			
			for (; content != null; content = content.Next)
			{
				if (content.Tag == tt.TagH2)
				{
					break;
				}

				/* kludge for naked text before block level tag */
				if (last != null && !_options.IndentContent && last.Type == Node.TextNode && content.Tag != null && (content.Tag.Model & ContentModel.Block) != 0)
				{
					FlushLine(fout, indent);
					FlushLine(fout, indent);
				}
				
				PrintTree(fout, mode, (_options.IndentContent?indent + _options.Spaces:indent), lexer, content);
				
				last = content;
			}
			
			slidecontent = content;
			
			/* now print epilog */
			
			CondFlushLine(fout, indent);
			
			PrintString(fout, indent, "<br clear=\"all\">");
			CondFlushLine(fout, indent);
			
			AddC('<', linelen++);
			
			
			AddC((int) Lexer.FoldCase('h', _options.UpperCaseTags, _options.XmlTags), linelen++);
			AddC((int) Lexer.FoldCase('r', _options.UpperCaseTags, _options.XmlTags), linelen++);
			
			if (_options.XmlOut == true)
			{
				PrintString(fout, indent, " />");
			}
			else
			{
				AddC('>', linelen++);
			}

			if (_options.IndentContent == true)
			{
				CondFlushLine(fout, indent);
			}

			PrintNavBar(fout, indent);
			
			/* end tag for div */
			PrintString(fout, indent, "</div>");
			CondFlushLine(fout, indent);
		}
		
		/*
		Add meta element for page transition effect, this works on IE but not NS
		*/
		public virtual void AddTransitionEffect(Lexer lexer, Node root, short effect, double duration)
		{
			Node head = root.FindHead(lexer.Options.tt);
			string transition;
			
			if (0 <= effect && effect <= 23)
			{
				transition = "revealTrans(Duration=" + (duration).ToString() + ",Transition=" + effect + ")";
			}
			else
			{
				transition = "blendTrans(Duration=" + (duration).ToString() + ")";
			}
			
			if (head != null)
			{
				Node meta = lexer.InferredTag("meta");
				meta.AddAttribute("http-equiv", "Page-Enter");
				meta.AddAttribute("content", transition);
				Node.InsertNodeAtStart(head, meta);
			}
		}
		
		public virtual void CreateSlides(Lexer lexer, Node root)
		{
			Node body;
			string buf;
			Out output = new OutImpl();
			
			body = root.FindBody(lexer.Options.tt);
			count = CountSlides(body);
			slidecontent = body.Content;
			AddTransitionEffect(lexer, root, EFFECT_BLEND, 3.0);
			
			for (slide = 1; slide <= count; ++slide)
			{
				buf = "slide" + slide + ".html";
				output.State = StreamIn.FSM_ASCII;
				output.Encoding = _options.CharEncoding;
				
				try
				{
					output.Output = new FileStream(buf, FileMode.Create);
					PrintTree(output, (short) 0, 0, lexer, root);
					FlushLine(output, 0);
					output.Output.Close();
				}
				catch (IOException e)
				{
					Console.Error.WriteLine(buf + e.ToString());
				}
			}
			
			/*
			delete superfluous slides by deleting slideN.html
			for N = count+1, count+2, etc. until no such file
			is found.     
			*/
			
			for (;;)
			{
				buf = "slide" + slide + "html";
				
				bool tmpBool;
				if (File.Exists((new FileInfo(buf)).FullName))
				{
					File.Delete((new FileInfo(buf)).FullName);
					tmpBool = true;
				}
				else if (Directory.Exists((new FileInfo(buf)).FullName))
				{
					Directory.Delete((new FileInfo(buf)).FullName);
					tmpBool = true;
				}
				else
				{
					tmpBool = false;
				}
				if (!tmpBool)
				{
					break;
				}
				
				++slide;
			}
		}

		private const int NORMAL = 0;
		private const int PREFORMATTED = 1;
		private const int COMMENT = 2;
		private const int ATTRIBVALUE = 4;
		private const int NOWRAP = 8;
		private const int CDATA = 16;
		
		private int[] linebuf = null;
		private int lbufsize = 0;
		private int linelen = 0;
		private int wraphere = 0;
		private bool inAttVal = false;
		private bool InString = false;
		
		private int slide = 0;
		private int count = 0;
		private Node slidecontent = null;
		
		private TidyOptions _options;		
	}
}