#define BUG_REPORT // https://sourceforge.net/tracker/?func=detail&aid=2931462&group_id=137819&atid=739991
using System;
using System.IO;
using System.Text;
using System.Collections;

namespace TidyNet
{
	/// <summary>
	/// Lexer for html parser
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
	/// <remarks>
	/// Given a file stream fp it returns a sequence of tokens.
	/// 
	/// GetToken(fp) gets the next token
	/// UngetToken(fp) provides one level undo
	/// 
	/// The tags include an attribute list:
	/// 
	/// - linked list of attribute/value nodes
	/// - each node has 2 null-terminated strings.
	/// - entities are replaced in attribute values
	/// 
	/// white space is compacted if not in preformatted mode
	/// If not in preformatted mode then leading white space
	/// is discarded and subsequent white space sequences
	/// compacted to single space chars.
	/// 
	/// If XmlTags is no then Tag names are folded to upper
	/// case and attribute names to lower case.
	/// 
	/// Not yet done:
	/// -   Doctype subset and marked sections
	/// </remarks>
	internal class Lexer
	{
		public StreamIn input; /* file stream */
		public TidyMessageCollection messages = new TidyMessageCollection();
		public int badAccess; /* for accessibility errors */
		public int badLayout; /* for bad style errors */
		public int badChars; /* for bad char encodings */
		public int badForm; /* for mismatched/mispositioned form tags */
		public int lines; /* lines seen */
		public int columns; /* at start of current token */
		public bool waswhite; /* used to collapse contiguous white space */
		public bool pushed; /* true after token has been pushed back */
		public bool insertspace; /* when space is moved after end tag */
		public bool excludeBlocks; /* Netscape compatibility */
		public bool exiled; /* true if moved out of table */
		public bool isvoyager; /* true if xmlns attribute on html element */
		public HtmlVersion versions; /* bit vector of HTML versions */
		public HtmlVersion doctype; /* version as given by doctype (if any) */
		public bool badDoctype; /* e.g. if html or PUBLIC is missing */
		public int txtstart; /* start of current node */
		public int txtend; /* end of current node */
		public short state; /* state of lexer's finite state machine */
		public Node token;
		
		/* 
		lexer character buffer
		
		parse tree nodes span onto this buffer
		which contains the concatenated text
		contents of all of the elements.
		
		lexsize must be reset for each file.
		*/
		public byte[] lexbuf; /* byte buffer of UTF-8 chars */
		public int lexlength; /* allocated */
		public int lexsize; /* used */
		
		/* Inline stack for compatibility with Mosaic */
		public Node inode; /* for deferring text node */
		public int insert; /* for inferring inline tags */
		public Stack istack;
		public int istackbase; /* start of frame */
		
		public Style styles; /* used for cleaning up presentation markup */
		
		public TidyOptions Options = new TidyOptions();
		protected internal int seenBodyEndTag; /* used by parser */
		private ArrayList nodeList;
		
		public Lexer(StreamIn input, TidyOptions options)
		{
			this.input = input;
			this.lines = 1;
			this.columns = 1;
			this.state = LEX_CONTENT;
			this.badAccess = 0;
			this.badLayout = 0;
			this.badChars = 0;
			this.badForm = 0;
			this.waswhite = false;
			this.pushed = false;
			this.insertspace = false;
			this.exiled = false;
			this.isvoyager = false;
			this.versions = HtmlVersion.Everything;
			this.doctype = HtmlVersion.Unknown;
			this.badDoctype = false;
			this.txtstart = 0;
			this.txtend = 0;
			this.token = null;
			this.lexbuf = null;
			this.lexlength = 0;
			this.lexsize = 0;
			this.inode = null;
			this.insert = - 1;
			this.istack = new Stack();
			this.istackbase = 0;
			this.styles = null;
			this.Options = options;
			this.seenBodyEndTag = 0;
			this.nodeList = new ArrayList();
		}
		
		public virtual Node NewNode()
		{
			Node node = new Node();
			nodeList.Add(node);
			return node;
		}
		
		public virtual Node NewNode(short type, byte[] textarray, int start, int end)
		{
			Node node = new Node(type, textarray, start, end);
			nodeList.Add(node);
			return node;
		}
		
		public virtual Node NewNode(short type, byte[] textarray, int start, int end, string element)
		{
			Node node = new Node(type, textarray, start, end, element, Options.tt);
			nodeList.Add(node);
			return node;
		}
		
		public virtual Node CloneNode(Node node)
		{
			Node cnode = (Node) node.Clone();
			nodeList.Add(cnode);
			for (AttVal att = cnode.Attributes; att != null; att = att.Next)
			{
				if (att.Asp != null)
					nodeList.Add(att.Asp);
				if (att.Php != null)
					nodeList.Add(att.Php);
			}
			return cnode;
		}
		
		public virtual AttVal CloneAttributes(AttVal attrs)
		{
			AttVal cattrs = (AttVal) attrs.Clone();
			for (AttVal att = cattrs; att != null; att = att.Next)
			{
				if (att.Asp != null)
					nodeList.Add(att.Asp);
				if (att.Php != null)
					nodeList.Add(att.Php);
			}
			return cattrs;
		}
		
		protected internal virtual void UpdateNodeTextArrays(byte[] oldtextarray, byte[] newtextarray)
		{
			Node node;
			for (int i = 0; i < nodeList.Count; i++)
			{
				node = (Node) (nodeList[i]);
				if (node.Textarray == oldtextarray)
					node.Textarray = newtextarray;
			}
		}
		
		/* used for creating preformatted text from Word2000 */
		public virtual Node NewLineNode()
		{
			Node node = NewNode();
			
			node.Textarray = this.lexbuf;
			node.Start = this.lexsize;
			AddCharToLexer((int) '\n');
			node.End = this.lexsize;
			return node;
		}
		
		// Should always be able convert to/from UTF-8, so encoding exceptions are
		// converted to an Error to avoid adding throws declarations in
		// lots of methods.
		
		public static byte[] GetBytes(string str)
		{
			try
			{
				return Encoding.UTF8.GetBytes(str);
			}
			catch (IOException e)
			{
				throw new ApplicationException("string to UTF-8 conversion failed: " + e.Message);
			}
		}
		
		public static string GetString(byte[] bytes, int offset, int length)
		{
			try
			{
				return Encoding.UTF8.GetString(bytes, offset, length);
			}
			catch (IOException e)
			{
				throw new ApplicationException("UTF-8 to string conversion failed: " + e.Message);
			}
		}
		
		public virtual bool EndOfInput()
		{
			return this.input.IsEndOfStream;
		}
		
		public virtual void AddByte(int c)
		{
			if (this.lexsize + 1 >= this.lexlength)
			{
				while (this.lexsize + 1 >= this.lexlength)
				{
					if (this.lexlength == 0)
						this.lexlength = 8192;
					else
						this.lexlength = this.lexlength * 2;
				}
				
				byte[] temp = this.lexbuf;
				this.lexbuf = new byte[this.lexlength];
				if (temp != null)
				{
					Array.Copy(temp, 0, this.lexbuf, 0, temp.Length);
					UpdateNodeTextArrays(temp, this.lexbuf);
				}
			}
			
			this.lexbuf[this.lexsize++] = (byte) c;
			this.lexbuf[this.lexsize] = (byte) '\x0000'; /* debug */
		}
		
		public virtual void ChangeChar(byte c)
		{
			if (this.lexsize > 0)
			{
				this.lexbuf[this.lexsize - 1] = c;
			}
		}
		
		/* store char c as UTF-8 encoded byte stream */
		public virtual void AddCharToLexer(int c)
		{
			if (c < 128)
			{
				AddByte(c);
			}
			else if (c <= 0x7FF)
			{
				AddByte(0xC0 | (c >> 6));
				AddByte(0x80 | (c & 0x3F));
			}
			else if (c <= 0xFFFF)
			{
				AddByte(0xE0 | (c >> 12));
				AddByte(0x80 | ((c >> 6) & 0x3F));
				AddByte(0x80 | (c & 0x3F));
			}
			else if (c <= 0x1FFFFF)
			{
				AddByte(0xF0 | (c >> 18));
				AddByte(0x80 | ((c >> 12) & 0x3F));
				AddByte(0x80 | ((c >> 6) & 0x3F));
				AddByte(0x80 | (c & 0x3F));
			}
			else
			{
				AddByte(0xF8 | (c >> 24));
				AddByte(0x80 | ((c >> 18) & 0x3F));
				AddByte(0x80 | ((c >> 12) & 0x3F));
				AddByte(0x80 | ((c >> 6) & 0x3F));
				AddByte(0x80 | (c & 0x3F));
			}
		}
		
		public virtual void AddStringToLexer(string str)
		{
			for (int i = 0; i < str.Length; i++)
			{
				AddCharToLexer((int) str[i]);
			}
		}
		
		/*
		No longer attempts to insert missing ';' for unknown
		enitities unless one was present already, since this
		gives unexpected results.
		
		For example:   <a href="something.htm?foo&bar&fred">
		was tidied to: <a href="something.htm?foo&amp;bar;&amp;fred;">
		rather than:   <a href="something.htm?foo&amp;bar&amp;fred">
		
		My thanks for Maurice Buxton for spotting this.
		*/
		public virtual void ParseEntity(short mode)
		{
			short map;
			int start;
			bool first = true;
			bool semicolon = false;
			bool numeric = false;
			int c, ch, startcol;
			string str;
			
			start = this.lexsize - 1; /* to start at "&" */
			startcol = this.input.curcol - 1;
			
			while (true)
			{
				c = this.input.ReadChar();
				if (c == StreamIn.EndOfStream)
				{
					break;
				}
				if (c == ';')
				{
					semicolon = true;
					break;
				}
				
				if (first && c == '#')
				{
					AddCharToLexer(c);
					first = false;
					numeric = true;
					continue;
				}
				
				first = false;
				map = MAP((char) c);
				
				/* AQ: Added flag for numeric entities so that numeric entities
				with missing semi-colons are recognized.
				Eg. "&#114e&#112;..." is recognized as "rep"
				*/
				if (numeric && ((c == 'x') || ((map & DIGIT) != 0)))
				{
					AddCharToLexer(c);
					continue;
				}
				if (!numeric && ((map & NAMECHAR) != 0))
				{
					AddCharToLexer(c);
					continue;
				}
				
				/* otherwise put it back */
				
				this.input.UngetChar(c);
				break;
			}
			
			str = GetString(this.lexbuf, start, this.lexsize - start);
			ch = EntityTable.DefaultEntityTable.EntityCode(str);
			
			/* deal with unrecognized entities */
			if (ch <= 0)
			{
				/* set error position just before offending chararcter */
				this.lines = this.input.curline;
				this.columns = startcol;
				
				if (this.lexsize > start + 1)
				{
					Report.EntityError(this, Report.UNKNOWN_ENTITY, str, ch);
					
					if (semicolon)
						AddCharToLexer(';');
				}
					/* naked & */
				else
				{
					Report.EntityError(this, Report.UNESCAPED_AMPERSAND, str, ch);
				}
			}
			else
			{
				if (c != ';')
					/* issue warning if not terminated by ';' */
				{
					/* set error position just before offending chararcter */
					this.lines = this.input.curline;
					this.columns = startcol;
					Report.EntityError(this, Report.MISSING_SEMICOLON, str, c);
				}
				
				this.lexsize = start;
				
				if (ch == 160 && (mode & Preformatted) != 0)
					ch = ' ';
				
				AddCharToLexer(ch);
				
				if (ch == '&' && !Options.QuoteAmpersand)
				{
					AddCharToLexer('a');
					AddCharToLexer('m');
					AddCharToLexer('p');
					AddCharToLexer(';');
				}
			}
		}
		
		public virtual char ParseTagName()
		{
			short map;
			int c;
			
			/* fold case of first char in buffer */
			
			c = this.lexbuf[this.txtstart];
			map = MAP((char) c);
			
			if (!Options.XmlTags && (map & UPPERCASE) != 0)
			{
				c += (int) ((int) 'a' - (int) 'A');
				this.lexbuf[this.txtstart] = (byte) c;
			}
			
			while (true)
			{
				c = this.input.ReadChar();
				if (c == StreamIn.EndOfStream)
				{
					break;
				}
				map = MAP((char) c);
				
				if ((map & NAMECHAR) == 0)
				{
					break;
				}
				
				/* fold case of subsequent chars */
				
				if (!Options.XmlTags && (map & UPPERCASE) != 0)
				{
					c += (int) ((int) 'a' - (int) 'A');
				}
				
				AddCharToLexer(c);
			}
			
			this.txtend = this.lexsize;
			return (char) c;
		}
		
		public virtual void AddStringLiteral(string str)
		{
			for (int i = 0; i < str.Length; i++)
			{
				AddCharToLexer((int) str[i]);
			}
		}
		
		/* choose what version to use for new doctype */
		public virtual HtmlVersion GetHtmlVersion()
		{
			if ((versions & HtmlVersion.Html20) != HtmlVersion.Unknown)
			{
				return HtmlVersion.Html20;
			}
			
			if ((versions & HtmlVersion.Html32) != HtmlVersion.Unknown)
			{
				return HtmlVersion.Html32;
			}
			
			if ((versions & HtmlVersion.Html40Strict) != HtmlVersion.Unknown)
			{
				return HtmlVersion.Html40Strict;
			}
			
			if ((versions & HtmlVersion.Html40Loose) != HtmlVersion.Unknown)
			{
				return HtmlVersion.Html40Loose;
			}
			
			if ((versions & HtmlVersion.Frames) != HtmlVersion.Unknown)
			{
				return HtmlVersion.Frames;
			}
			
			return HtmlVersion.Unknown;
		}
		
		public virtual string HtmlVersionName()
		{
			HtmlVersion guessed = ApparentVersion();
			int j;
			
			for (j = 0; j < W3CVersion.Length; ++j)
			{
				if (guessed == W3CVersion[j].Version)
				{
					if (this.isvoyager)
					{
						return W3CVersion[j].VoyagerName;
					}
					
					return W3CVersion[j].Name;
				}
			}
			
			return null;
		}
		
		/* add meta element for Tidy */
		public virtual bool AddGenerator(Node root)
		{
			AttVal attval;
			Node node;
			Node head = root.FindHead(Options.tt);
			
			if (head != null)
			{
				for (node = head.Content; node != null; node = node.Next)
				{
					if (node.Tag == Options.tt.TagMeta)
					{
						attval = node.GetAttrByName("name");
						
						if (attval != null && attval.Val != null && String.Compare(attval.Val, "generator") == 0)
						{
							attval = node.GetAttrByName("content");
							
							if (attval != null && attval.Val != null && attval.Val.Length >= 9 && String.Compare(attval.Val.Substring(0, 9), "HTML Tidy") == 0)
							{
								return false;
							}
						}
					}
				}
				
				node = this.InferredTag("meta");
				node.AddAttribute("content", "HTML Tidy, see www.w3.org");
				node.AddAttribute("name", "generator");
				Node.InsertNodeAtStart(head, node);
				return true;
			}
			
			return false;
		}
		
		/* return true if substring s is in p and isn't all in upper case */
		/* this is used to check the case of SYSTEM, PUBLIC, DTD and EN */
		/* len is how many chars to check in p */
		private static bool FindBadSubString(string s, string p, int len)
		{
			int n = s.Length;
			int i = 0;
			string ps;
			
			while (n < len)
			{
				ps = p.Substring(i, (i + n) - (i));
				if (String.Compare(s, ps) == 0)
				{
					return !ps.Equals(s.Substring(0, n));
				}
				
				++i;
				--len;
			}
			
			return false;
		}
		
		public virtual bool CheckDocTypeKeyWords(Node doctype)
		{
			int len = doctype.End - doctype.Start;
			string s = GetString(this.lexbuf, doctype.Start, len);
			
			return !(FindBadSubString("SYSTEM", s, len) || FindBadSubString("PUBLIC", s, len) || FindBadSubString("//DTD", s, len) || FindBadSubString("//W3C", s, len) || FindBadSubString("//EN", s, len));
		}
		
		/* examine <!DOCTYPE> to identify version */
		public virtual HtmlVersion FindGivenVersion(Node doctype)
		{
			string p, s;
			int i, j;
			int len;
			string str1;
			string str2;
			
			/* if root tag for doctype isn't html give up now */
			str1 = GetString(this.lexbuf, doctype.Start, 5);
			if (String.Compare(str1, "html ") != 0)
			{
				return 0;
			}
			
			if (!CheckDocTypeKeyWords(doctype))
			{
				Report.Warning(this, doctype, null, Report.DTYPE_NOT_UPPER_CASE);
			}
			
			/* give up if all we are given is the system id for the doctype */
			str1 = GetString(this.lexbuf, doctype.Start + 5, 7);
			if (String.Compare(str1, "SYSTEM ") == 0)
			{
				/* but at least ensure the case is correct */
				if (!str1.Substring(0, (6) - (0)).Equals("SYSTEM"))
				{
					Array.Copy(GetBytes("SYSTEM"), 0, this.lexbuf, doctype.Start + 5, 6);
				}
				return 0; /* unrecognized */
			}
			
			if (String.Compare(str1, "PUBLIC ") == 0)
			{
				if (!str1.Substring(0, (6) - (0)).Equals("PUBLIC"))
					Array.Copy(GetBytes("PUBLIC "), 0, this.lexbuf, doctype.Start + 5, 6);
			}
			else
			{
				this.badDoctype = true;
			}
			
			for (i = doctype.Start; i < doctype.End; ++i)
			{
				if (this.lexbuf[i] == (byte) '"')
				{
					str1 = GetString(this.lexbuf, i + 1, 12);
					str2 = GetString(this.lexbuf, i + 1, 13);
					if (str1.Equals("-//W3C//DTD "))
					{
						/* compute length of identifier e.g. "HTML 4.0 Transitional" */
						for (j = i + 13; j < doctype.End && this.lexbuf[j] != (byte) '/'; ++j)
							;
						len = j - i - 13;
						p = GetString(this.lexbuf, i + 13, len);
						
						for (j = 1; j < W3CVersion.Length; ++j)
						{
							s = W3CVersion[j].Name;
							if (len == s.Length && s.Equals(p))
							{
								return W3CVersion[j].Version;
							}
						}
						
						/* else unrecognized version */
					}
					else if (str2.Equals("-//IETF//DTD "))
					{
						/* compute length of identifier e.g. "HTML 2.0" */
						for (j = i + 14; j < doctype.End && this.lexbuf[j] != (byte) '/'; ++j)
							;
						len = j - i - 14;
						
						p = GetString(this.lexbuf, i + 14, len);
						s = W3CVersion[0].Name;
						if (len == s.Length && s.Equals(p))
						{
							return W3CVersion[0].Version;
						}
						
						/* else unrecognized version */
					}
					break;
				}
			}
			
			return 0;
		}
		
		public virtual void FixHtmlNameSpace(Node root, string profile)
		{
			Node node;
			AttVal prev, attr;
			
			for (node = root.Content; node != null && node.Tag != Options.tt.TagHtml; node = node.Next)
			{
				;
			}
			
			if (node != null)
			{
				prev = null;
				
				for (attr = node.Attributes; attr != null; attr = attr.Next)
				{
					if (attr.Attribute.Equals("xmlns"))
					{
						break;
					}
					
					prev = attr;
				}
				
				if (attr != null)
				{
					if (!attr.Val.Equals(profile))
					{
						Report.Warning(this, node, null, Report.INCONSISTENT_NAMESPACE);
						attr.Val = profile;
					}
				}
				else
				{
					attr = new AttVal(node.Attributes, null, (int) '"', "xmlns", profile);
					attr.Dict = AttributeTable.DefaultAttributeTable.FindAttribute(attr);
					node.Attributes = attr;
				}
			}
		}
		
		public virtual bool SetXhtmlDocType(Node root)
		{
			string fpi = " ";
			string sysid = "";
			string namespace_Renamed = XHTML_NAMESPACE;
			Node doctype;
			
			doctype = root.FindDocType();
			
			if (Options.DocType == TidyNet.DocType.Omit)
			{
				if (doctype != null)
					Node.DiscardElement(doctype);
				return true;
			}
			
			if (Options.DocType == TidyNet.DocType.Auto)
			{
				/* see what flavor of XHTML this document matches */
				if ((this.versions & HtmlVersion.Html40Strict) != 0)
				{
					/* use XHTML strict */
					fpi = "-//W3C//DTD XHTML 1.0 Strict//EN";
					sysid = voyager_strict;
				}
				else if ((this.versions & HtmlVersion.Loose) != 0)
				{
					fpi = "-//W3C//DTD XHTML 1.0 Transitional//EN";
					sysid = voyager_loose;
				}
				else if ((this.versions & HtmlVersion.Frames) != 0)
				{
					/* use XHTML frames */
					fpi = "-//W3C//DTD XHTML 1.0 Frameset//EN";
					sysid = voyager_frameset;
				}
				else
				{
					/* lets assume XHTML transitional */
					fpi = "-//W3C//DTD XHTML 1.0 Transitional//EN";
					sysid = voyager_loose;
				}
			}
			else if (Options.DocType == TidyNet.DocType.Strict)
			{
				fpi = "-//W3C//DTD XHTML 1.0 Strict//EN";
				sysid = voyager_strict;
			}
			else if (Options.DocType == TidyNet.DocType.Loose)
			{
				fpi = "-//W3C//DTD XHTML 1.0 Transitional//EN";
				sysid = voyager_loose;
			}
			
			FixHtmlNameSpace(root, namespace_Renamed);
			
			if (doctype == null)
			{
				doctype = NewNode(Node.DocTypeTag, this.lexbuf, 0, 0);
				doctype.Next = root.Content;
				doctype.Parent = root;
				doctype.Prev = null;
				root.Content = doctype;
			}
			
			if (Options.DocType == TidyNet.DocType.User && Options.DocTypeStr != null)
			{
				fpi = Options.DocTypeStr;
				sysid = "";
			}
			
			this.txtstart = this.lexsize;
			this.txtend = this.lexsize;
			
			/* add public identifier */
			AddStringLiteral("html PUBLIC ");
			
			/* check if the fpi is quoted or not */
			if (fpi[0] == '"')
			{
				AddStringLiteral(fpi);
			}
			else
			{
				AddStringLiteral("\"");
				AddStringLiteral(fpi);
				AddStringLiteral("\"");
			}
			
			if (sysid.Length + 6 >= this.Options.WrapLen)
			{
				AddStringLiteral("\n\"");
			}
			else
			{
				AddStringLiteral("\n    \"");
			}

			/* add system identifier */
			AddStringLiteral(sysid);
			AddStringLiteral("\"");
			
			this.txtend = this.lexsize;
			
			doctype.Start = this.txtstart;
			doctype.End = this.txtend;
			
			return false;
		}
		
		public virtual HtmlVersion ApparentVersion()
		{
			switch (this.doctype)
			{
			case HtmlVersion.Unknown: 
				return GetHtmlVersion();

			case HtmlVersion.Html20: 
				if ((this.versions & HtmlVersion.Html20) != 0)
				{
					return HtmlVersion.Html20;
				}
				break;

			case HtmlVersion.Html32: 
				if ((this.versions & HtmlVersion.Html32) != 0)
				{
					return HtmlVersion.Html32;
				}
				break; /* to replace old version by new */

			case HtmlVersion.Html40Strict: 
				if ((this.versions & HtmlVersion.Html40Strict) != 0)
				{
					return HtmlVersion.Html40Strict;
				}
				break;
				
			case HtmlVersion.Html40Loose: 
				if ((this.versions & HtmlVersion.Html40Loose) != 0)
				{
					return HtmlVersion.Html40Loose;
				}
				break; /* to replace old version by new */

			case HtmlVersion.Frames:
				if ((this.versions & HtmlVersion.Frames) != 0)
				{
					return HtmlVersion.Frames;
				}
				break;
			}
			
			Report.Warning(this, null, null, Report.INCONSISTENT_VERSION);
			return GetHtmlVersion();
		}
		
		/* fixup doctype if missing */
		public virtual bool FixDocType(Node root)
		{
			Node doctype;
			HtmlVersion guessed = HtmlVersion.Html40Strict;
			int i;
			
			if (this.badDoctype)
			{
				Report.Warning(this, null, null, Report.MALFORMED_DOCTYPE);
			}
			
			if (Options.XmlOut)
			{
				return true;
			}
			
			doctype = root.FindDocType();
			
			if (Options.DocType == TidyNet.DocType.Omit)
			{
				if (doctype != null)
				{
					Node.DiscardElement(doctype);
				}
				return true;
			}
			
			if (Options.DocType == TidyNet.DocType.Strict)
			{
				Node.DiscardElement(doctype);
				doctype = null;
				guessed = HtmlVersion.Html40Strict;
			}
			else if (Options.DocType == TidyNet.DocType.Loose)
			{
				Node.DiscardElement(doctype);
				doctype = null;
				guessed = HtmlVersion.Html40Loose;
			}
			else if (Options.DocType == TidyNet.DocType.Auto)
			{
				if (doctype != null)
				{
					if (this.doctype == HtmlVersion.Unknown)
					{
						return false;
					}

					switch (this.doctype)
					{
					case HtmlVersion.Unknown:
						return false;

					case HtmlVersion.Html20:
						if ((this.versions & HtmlVersion.Html20) != 0)
						{
							return true;
						}
						break; /* to replace old version by new */
						
						
					case HtmlVersion.Html32:
						if ((this.versions & HtmlVersion.Html32) != 0)
						{
							return true;
						}
						break; /* to replace old version by new */
						
						
					case HtmlVersion.Html40Strict:
						if ((this.versions & HtmlVersion.Html40Strict) != 0)
						{
							return true;
						}
						break; /* to replace old version by new */
						
						
					case HtmlVersion.Html40Loose:
						if ((this.versions & HtmlVersion.Html40Loose) != 0)
						{
							return true;
						}
						break; /* to replace old version by new */
						
						
					case HtmlVersion.Frames:
						if ((this.versions & HtmlVersion.Frames) != 0)
						{
							return true;
						}
						break; /* to replace old version by new */
					}
					
					/* INCONSISTENT_VERSION warning is now issued by ApparentVersion() */
				}
				
				/* choose new doctype */
				guessed = GetHtmlVersion();
			}
			
			if (guessed == HtmlVersion.Unknown)
			{
				return false;
			}
			
			/* for XML use the Voyager system identifier */
			if (this.Options.XmlOut || this.Options.XmlTags || this.isvoyager)
			{
				if (doctype != null)
					Node.DiscardElement(doctype);
				
				for (i = 0; i < W3CVersion.Length; ++i)
				{
					if (guessed == W3CVersion[i].Version)
					{
						FixHtmlNameSpace(root, W3CVersion[i].Profile);
						break;
					}
				}
				
				return true;
			}
			
			if (doctype == null)
			{
				doctype = NewNode(Node.DocTypeTag, this.lexbuf, 0, 0);
				doctype.Next = root.Content;
				doctype.Parent = root;
				doctype.Prev = null;
				root.Content = doctype;
			}
			
			this.txtstart = this.lexsize;
			this.txtend = this.lexsize;
			
			/* use the appropriate public identifier */
			AddStringLiteral("html PUBLIC ");
			
			if (Options.DocType == TidyNet.DocType.User && Options.DocTypeStr != null)
			{
				AddStringLiteral(Options.DocTypeStr);
			}
			else if (guessed == HtmlVersion.Html20)
			{
				AddStringLiteral("\"-//IETF//DTD HTML 2.0//EN\"");
			}
			else
			{
				AddStringLiteral("\"-//W3C//DTD ");
				
				for (i = 0; i < W3CVersion.Length; ++i)
				{
					if (guessed == W3CVersion[i].Version)
					{
						AddStringLiteral(W3CVersion[i].Name);
						break;
					}
				}
				
				AddStringLiteral("//EN\"");
			}
			
			this.txtend = this.lexsize;
			
			doctype.Start = this.txtstart;
			doctype.End = this.txtend;
			
			return true;
		}
		
		/* ensure XML document starts with <?XML version="1.0"?> */
		public virtual bool FixXmlPI(Node root)
		{
			Node xml;
			int s;
			
			if (root.Content != null && root.Content.Type == Node.ProcInsTag)
			{
				s = root.Content.Start;
				
				if (this.lexbuf[s] == (byte) 'x' && this.lexbuf[s + 1] == (byte) 'm' && this.lexbuf[s + 2] == (byte) 'l')
				{
					return true;
				}
			}
			
			xml = NewNode(Node.ProcInsTag, this.lexbuf, 0, 0);
			xml.Next = root.Content;
			
			if (root.Content != null)
			{
				root.Content.Prev = xml;
				xml.Next = root.Content;
			}
			
			root.Content = xml;
			
			this.txtstart = this.lexsize;
			this.txtend = this.lexsize;
			AddStringLiteral("xml version=\"1.0\"");
			if (this.Options.CharEncoding == CharEncoding.Latin1)
			{
				AddStringLiteral(" encoding=\"ISO-8859-1\"");
			}
			this.txtend = this.lexsize;
			
			xml.Start = this.txtstart;
			xml.End = this.txtend;
			return false;
		}
		
		public virtual Node InferredTag(string name)
		{
			Node node;
			
			node = NewNode(Node.StartTag, this.lexbuf, this.txtstart, this.txtend, name);
			node.Isimplicit = true;
			return node;
		}
		
		public static bool ExpectsContent(Node node)
		{
			if (node.Type != Node.StartTag)
			{
				return false;
			}
			
			/* unknown element? */
			if (node.Tag == null)
			{
				return true;
			}
			
			if ((node.Tag.Model & ContentModel.Empty) != 0)
			{
				return false;
			}
			
			return true;
		}
		
		/*
		create a text node for the contents of
		a CDATA element like style or script
		which ends with </foo> for some foo.
		*/
		public virtual Node GetCDATA(Node container)
		{
			int c, lastc, start, len, i;
			string str;
			bool endtag = false;
			
			lines = input.curline;
			columns = input.curcol;
			waswhite = false;
			txtstart = lexsize;
			txtend = lexsize;
			
			lastc = (int) '\x0000';
			start = - 1;
			
			while (true)
			{
				c = input.ReadChar();
				if (c == StreamIn.EndOfStream)
				{
					break;
				}
				/* treat \r\n as \n and \r as \n */
				
				if (c == (int) '/' && lastc == (int) '<')
				{
					if (endtag)
					{
						lines = input.curline;
						columns = input.curcol - 3;
						
						Report.Warning(this, null, null, Report.BAD_CDATA_CONTENT);
					}
					
					start = lexsize + 1; /* to first letter */
					endtag = true;
				}
				else if (c == (int) '>' && start >= 0)
				{
					len = lexsize - start;
					if (len == container.Element.Length)
					{
						str = GetString(lexbuf, start, len);
						if (String.Compare(str, container.Element) == 0)
						{
							txtend = start - 2;
							break;
						}
					}
					
					lines = input.curline;
					columns = input.curcol - 3;
					
					Report.Warning(this, null, null, Report.BAD_CDATA_CONTENT);
					
					/* if javascript insert backslash before / */
					
					if (ParserImpl.isJavaScript(container))
					{
						for (i = lexsize; i > start - 1; --i)
						{
							lexbuf[i] = lexbuf[i - 1];
						}
						
						lexbuf[start - 1] = (byte) '\\';
						lexsize++;
					}
					
					start = - 1;
				}
				else if (c == (int) '\r')
				{
					c = input.ReadChar();
					
					if (c != (int) '\n')
					{
						input.UngetChar(c);
					}
					
					c = (int) '\n';
				}
				
				AddCharToLexer((int) c);
				txtend = lexsize;
				lastc = c;
			}
			
			if (c == StreamIn.EndOfStream)
			{
				Report.Warning(this, container, null, Report.MISSING_ENDTAG_FOR);
			}
			
			if (txtend > txtstart)
			{
				token = NewNode(Node.TextNode, lexbuf, txtstart, txtend);
				return token;
			}
			
			return null;
		}
		
		public virtual void UngetToken()
		{
			pushed = true;
		}
		
		public const short IgnoreWhitespace = 0;
		public const short MixedContent = 1;
		public const short Preformatted = 2;
		public const short IgnoreMarkup = 3;
		
		/*
		modes for GetToken()
		
		MixedContent   -- for elements which don't accept PCDATA
		Preformatted       -- white space preserved as is
		IgnoreMarkup       -- for CDATA elements such as script, style
		*/
		public virtual Node GetToken(short mode)
		{
			short map;
			int c = 0;
			int lastc;
			int badcomment = 0;
			MutableBoolean isempty = new MutableBoolean();
			AttVal attributes;
			
			if (pushed)
			{
				/* duplicate inlines in preference to pushed text nodes when appropriate */
				if (token.Type != Node.TextNode || (insert == - 1 && inode == null))
				{
					pushed = false;
					return token;
				}
			}
			
			/* at start of block elements, unclosed inline
			elements are inserted into the token stream */
			
			if (insert != - 1 || inode != null)
			{
				return InsertedToken();
			}
			
			lines = input.curline;
			columns = input.curcol;
			waswhite = false;
			
			txtstart = lexsize;
			txtend = lexsize;
			
			while (true)
			{
				c = input.ReadChar();
				if (c == StreamIn.EndOfStream)
				{
					break;
				}

				if (insertspace && mode != IgnoreWhitespace)
				{
					AddCharToLexer(' ');
					waswhite = true;
					insertspace = false;
				}
				
				/* treat \r\n as \n and \r as \n */
				
				if (c == '\r')
				{
					c = input.ReadChar();
					
					if (c != '\n')
					{
						input.UngetChar(c);
					}
					
					c = '\n';
				}
				
				AddCharToLexer(c);
				
				switch (state)
				{
				case LEX_CONTENT: 
					map = MAP((char) c);
						
					/*
						Discard white space if appropriate. Its cheaper
						to do this here rather than in parser methods
						for elements that don't have mixed content.
						*/
					if (((map & WHITE) != 0) && (mode == IgnoreWhitespace) && lexsize == txtstart + 1)
					{
						--lexsize;
						waswhite = false;
						lines = input.curline;
						columns = input.curcol;
						continue;
					}
						
					if (c == '<')
					{
						state = LEX_GT;
						continue;
					}
						
					if ((map & WHITE) != 0)
					{
						/* was previous char white? */
						if (waswhite)
						{
							if (mode != Preformatted && mode != IgnoreMarkup)
							{
								--lexsize;
								lines = input.curline;
								columns = input.curcol;
							}
						}
							/* prev char wasn't white */
						else
						{
							waswhite = true;
							lastc = c;
								
							if (mode != Preformatted && mode != IgnoreMarkup && c != ' ')
							{
								ChangeChar((byte) ' ');
							}
						}
							
						continue;
					}
					else if (c == '&' && mode != IgnoreMarkup)
					{
						ParseEntity(mode);
					}
						
					/* this is needed to avoid trimming trailing whitespace */
					if (mode == IgnoreWhitespace)
						mode = MixedContent;
						
					waswhite = false;
					continue;
					
					
				case LEX_GT: 
					if (c == '/')
					{
						c = input.ReadChar();
						if (c == StreamIn.EndOfStream)
						{
							input.UngetChar(c);
							continue;
						}
							
						AddCharToLexer(c);
						map = MAP((char) c);
							
						if ((map & LETTER) != 0)
						{
							lexsize -= 3;
							txtend = lexsize;
							input.UngetChar(c);
							state = LEX_ENDTAG;
							lexbuf[lexsize] = (byte) '\x0000'; /* debug */
							input.curcol -= 2;
								
							/* if some text before the </ return it now */
							if (txtend > txtstart)
							{
								/* trim space char before end tag */
								if (mode == IgnoreWhitespace && lexbuf[lexsize - 1] == (byte) ' ')
								{
									lexsize -= 1;
									txtend = lexsize;
								}
									
								token = NewNode(Node.TextNode, lexbuf, txtstart, txtend);
								return token;
							}
								
							continue; /* no text so keep going */
						}
							
						/* otherwise treat as CDATA */
						waswhite = false;
						state = LEX_CONTENT;
						continue;
					}
						
					if (mode == IgnoreMarkup)
					{
						/* otherwise treat as CDATA */
						waswhite = false;
						state = LEX_CONTENT;
						continue;
					}
						
					/*
						look out for comments, doctype or marked sections
						this isn't quite right, but its getting there ...
						*/
					if (c == '!')
					{
						c = input.ReadChar();
						if (c == '-')
						{
							c = input.ReadChar();
							if (c == '-')
							{
								state = LEX_COMMENT; /* comment */
								lexsize -= 2;
								txtend = lexsize;
									
								/* if some text before < return it now */
								if (txtend > txtstart)
								{
									token = NewNode(Node.TextNode, lexbuf, txtstart, txtend);
									return token;
								}
									
								txtstart = lexsize;
								continue;
							}
								
							Report.Warning(this, null, null, Report.MALFORMED_COMMENT);
						}
						else if (c == 'd' || c == 'D')
						{
							state = LEX_DOCTYPE; /* doctype */
							lexsize -= 2;
							txtend = lexsize;
							mode = IgnoreWhitespace;
								
							/* skip until white space or '>' */
								
							for (; ; )
							{
								c = input.ReadChar();
									
								if (c == StreamIn.EndOfStream || c == '>')
								{
									input.UngetChar(c);
									break;
								}
									
								map = MAP((char) c);
								if ((map & WHITE) == 0)
								{
									continue;
								}
									
								/* and skip to end of whitespace */
									
								for (; ; )
								{
									c = input.ReadChar();
										
									if (c == StreamIn.EndOfStream || c == '>')
									{
										input.UngetChar(c);
										break;
									}
										
									map = MAP((char) c);
										
									if ((map & WHITE) != 0)
									{
										continue;
									}
										
									input.UngetChar(c);
									break;
								}
									
								break;
							}
								
							/* if some text before < return it now */
							if (txtend > txtstart)
							{
								token = NewNode(Node.TextNode, lexbuf, txtstart, txtend);
								return token;
							}
								
							txtstart = lexsize;
							continue;
						}
						else if (c == '[')
						{
							/* Word 2000 embeds <![if ...]> ... <![endif]> sequences */
							lexsize -= 2;
							state = LEX_SECTION;
							txtend = lexsize;
								
							/* if some text before < return it now */
							if (txtend > txtstart)
							{
								token = NewNode(Node.TextNode, lexbuf, txtstart, txtend);
								return token;
							}
								
							txtstart = lexsize;
							continue;
						}
							
						/* otherwise swallow chars up to and including next '>' */
						while (true)
						{
							c = input.ReadChar();
							if (c == '>')
							{
								break;
							}
							if (c == - 1)
							{
								input.UngetChar(c);
								break;
							}
						}
							
						lexsize -= 2;
						lexbuf[lexsize] = (byte) '\x0000';
						state = LEX_CONTENT;
						continue;
					}
						
					/*
						processing instructions
						*/
						
					if (c == '?')
					{
						lexsize -= 2;
						state = LEX_PROCINSTR;
						txtend = lexsize;
							
						/* if some text before < return it now */
						if (txtend > txtstart)
						{
							token = NewNode(Node.TextNode, lexbuf, txtstart, txtend);
							return token;
						}
							
						txtstart = lexsize;
						continue;
					}
						
					/* Microsoft ASP's e.g. <% ... server-code ... %> */
					if (c == '%')
					{
						lexsize -= 2;
						state = LEX_ASP;
						txtend = lexsize;
							
						/* if some text before < return it now */
						if (txtend > txtstart)
						{
							token = NewNode(Node.TextNode, lexbuf, txtstart, txtend);
							return token;
						}
							
						txtstart = lexsize;
						continue;
					}
						
					/* Netscapes JSTE e.g. <# ... server-code ... #> */
					if (c == '#')
					{
						lexsize -= 2;
						state = LEX_JSTE;
						txtend = lexsize;
							
						/* if some text before < return it now */
						if (txtend > txtstart)
						{
							token = NewNode(Node.TextNode, lexbuf, txtstart, txtend);
							return token;
						}
							
						txtstart = lexsize;
						continue;
					}
						
					map = MAP((char) c);
						
					/* check for start tag */
					if ((map & LETTER) != 0)
					{
						input.UngetChar(c); /* push back letter */
						lexsize -= 2; /* discard "<" + letter */
						txtend = lexsize;
						state = LEX_STARTTAG; /* ready to read tag name */
							
						/* if some text before < return it now */
						if (txtend > txtstart)
						{
							token = NewNode(Node.TextNode, lexbuf, txtstart, txtend);
							return token;
						}
							
						continue; /* no text so keep going */
					}
						
					/* otherwise treat as CDATA */
					state = LEX_CONTENT;
					waswhite = false;
					continue;
					
					
				case LEX_ENDTAG: 
					txtstart = lexsize - 1;
					input.curcol += 2;
					c = ParseTagName();
					token = NewNode(Node.EndTag, lexbuf, txtstart, txtend, GetString(lexbuf, txtstart, txtend - txtstart));
					lexsize = txtstart;
					txtend = txtstart;
						
					/* skip to '>' */
					while (c != '>')
					{
						c = input.ReadChar();
						if (c == StreamIn.EndOfStream)
						{
							break;
						}
					}
						
					if (c == StreamIn.EndOfStream)
					{
						input.UngetChar(c);
						continue;
					}
						
					state = LEX_CONTENT;
					waswhite = false;
					return token; /* the endtag token */
					
					
				case LEX_STARTTAG: 
					txtstart = lexsize - 1; /* set txtstart to first letter */
					c = ParseTagName();
					isempty.Val = false;
					attributes = null;
					token = NewNode((isempty.Val ? Node.StartEndTag : Node.StartTag), lexbuf, txtstart, txtend, GetString(lexbuf, txtstart, txtend - txtstart));
						
					/* parse attributes, consuming closing ">" */
					if (c != '>')
					{
						if (c == '/')
						{
							input.UngetChar(c);
						}
							
						attributes = ParseAttrs(isempty);
					}
						
					if (isempty.Val)
					{
						token.Type = Node.StartEndTag;
					}
						
					token.Attributes = attributes;
					lexsize = txtstart;
					txtend = txtstart;
						
					/* swallow newline following start tag */
					/* special check needed for CRLF sequence */
					/* this doesn't apply to empty elements */
						
					if (ExpectsContent(token) || token.Tag == Options.tt.TagBr)
					{
						c = input.ReadChar();
						if (c == '\r')
						{
							c = input.ReadChar();
								
							if (c != '\n')
							{
								input.UngetChar(c);
							}
						}
						else if (c != '\n' && c != '\f')
						{
							input.UngetChar(c);
						}
							
						waswhite = true; /* to swallow leading whitespace */
					}
					else
					{
						waswhite = false;
					}
						
					state = LEX_CONTENT;
						
					if (token.Tag == null)
					{
						Report.Error(this, null, token, Report.UNKNOWN_ELEMENT);
					}
					else if (!Options.XmlTags)
					{
						versions &= token.Tag.Versions;
							
						if ((token.Tag.Versions & HtmlVersion.Proprietary) != 0)
						{
							if (!Options.MakeClean && (token.Tag == Options.tt.TagNobr || token.Tag == Options.tt.TagWbr))
							{
								Report.Warning(this, null, token, Report.PROPRIETARY_ELEMENT);
							}
						}
							
						if (token.Tag.CheckAttribs != null)
						{
							token.CheckUniqueAttributes(this);
							token.Tag.CheckAttribs.Check(this, this.token);
						}
						else
						{
							token.CheckAttributes(this);
						}
					}
					return token; /* return start tag */

				case LEX_COMMENT: 
					if (c != '-')
					{
						continue;
					}
						
					c = input.ReadChar();
					AddCharToLexer(c);
					if (c != '-')
					{
						continue;
					}
						
					while (true)
					{
						c = input.ReadChar();
							
						if (c == '>')
						{
							if (badcomment != 0)
							{
								Report.Warning(this, null, null, Report.MALFORMED_COMMENT);
							}
								
							txtend = lexsize - 2; // AQ 8Jul2000
							lexbuf[lexsize] = (byte) '\x0000';
							state = LEX_CONTENT;
							waswhite = false;
							token = NewNode(Node.CommentTag, lexbuf, txtstart, txtend);
								
							/* now look for a line break */
								
							c = input.ReadChar();
								
							if (c == '\r')
							{
								c = input.ReadChar();
									
								if (c != '\n')
								{
									token.Linebreak = true;
								}
							}
								
							if (c == '\n')
							{
								token.Linebreak = true;
							}
							else
							{
								input.UngetChar(c);
							}
								
							return token;
						}
							
						/* note position of first such error in the comment */
						if (badcomment == 0)
						{
							lines = input.curline;
							columns = input.curcol - 3;
						}
							
						badcomment++;
						if (Options.FixComments)
						{
							lexbuf[lexsize - 2] = (byte) '=';
						}
							
						AddCharToLexer(c);
							
						/* if '-' then look for '>' to end the comment */
						if (c != '-')
						{
							break;
						}
					}
						
					/* otherwise continue to look for --> */
					lexbuf[lexsize - 2] = (byte) '=';
					continue;
					
					
				case LEX_DOCTYPE: 
					map = MAP((char) c);
						
					if ((map & WHITE) != 0)
					{
						if (waswhite)
						{
							lexsize -= 1;
						}
							
						waswhite = true;
					}
					else
					{
						waswhite = false;
					}
						
					if (c != '>')
					{
						continue;
					}
						
					lexsize -= 1;
					txtend = lexsize;
					lexbuf[lexsize] = (byte) '\x0000';
					state = LEX_CONTENT;
					waswhite = false;
					token = NewNode(Node.DocTypeTag, lexbuf, txtstart, txtend);
					/* make a note of the version named by the doctype */
					doctype = FindGivenVersion(token);
					return token;
					
					
				case LEX_PROCINSTR: 
						
					if (lexsize - txtstart == 3)
					{
						if ((GetString(lexbuf, txtstart, 3)).Equals("php"))
						{
							state = LEX_PHP;
							continue;
						}
					}
						
					if (Options.XmlPIs)
					{
						/* insist on ?> as terminator */
						if (c != '?')
						{
							continue;
						}
							
						/* now look for '>' */
						c = input.ReadChar();
							
						if (c == StreamIn.EndOfStream)
						{
							Report.Warning(this, null, null, Report.UNEXPECTED_END_OF_FILE);
							input.UngetChar(c);
							continue;
						}
							
						AddCharToLexer(c);
					}
						
					if (c != '>')
					{
						continue;
					}
						
					lexsize -= 1;
					txtend = lexsize;
					lexbuf[lexsize] = (byte) '\x0000';
					state = LEX_CONTENT;
					waswhite = false;
					token = NewNode(Node.ProcInsTag, lexbuf, txtstart, txtend);
					return token;
					
					
				case LEX_ASP: 
					if (c != '%')
					{
						continue;
					}
						
					/* now look for '>' */
					c = input.ReadChar();

					if (c != '>')
					{
						input.UngetChar(c);
						continue;
					}
						
					lexsize -= 1;
					txtend = lexsize;
					lexbuf[lexsize] = (byte) '\x0000';
					state = LEX_CONTENT;
					waswhite = false;
					token = NewNode(Node.AspTag, lexbuf, txtstart, txtend);
					return this.token;

				case LEX_JSTE: 
					if (c != '#')
					{
						continue;
					}
	
					/* now look for '>' */
					c = input.ReadChar();
					if (c != '>')
					{
						input.UngetChar(c);
						continue;
					}
						
					lexsize -= 1;
					txtend = lexsize;
					lexbuf[lexsize] = (byte) '\x0000';
					state = LEX_CONTENT;
					waswhite = false;
					token = NewNode(Node.JsteTag, lexbuf, txtstart, txtend);
					return token;

				case LEX_PHP: 
					if (c != '?')
					{
						continue;
					}
						
					/* now look for '>' */
					c = input.ReadChar();
					if (c != '>')
					{
						input.UngetChar(c);
						continue;
					}
						
					lexsize -= 1;
					txtend = lexsize;
					lexbuf[lexsize] = (byte) '\x0000';
					state = LEX_CONTENT;
					waswhite = false;
					token = NewNode(Node.PhpTag, lexbuf, txtstart, txtend);
					return token;
					
					
				case LEX_SECTION: 
					if (c == '[')
					{
						if (lexsize == (txtstart + 6) && (GetString(lexbuf, txtstart, 6)).Equals("CDATA["))
						{
							state = LEX_CDATA;
							lexsize -= 6;
							continue;
						}
					}
						
					if (c != ']')
					{
						continue;
					}

					/* now look for '>' */
					c = input.ReadChar();
					if (c != '>')
					{
						input.UngetChar(c);
						continue;
					}
						
					lexsize -= 1;
					txtend = lexsize;
					lexbuf[lexsize] = (byte) '\x0000';
					state = LEX_CONTENT;
					waswhite = false;
					token = NewNode(Node.SectionTag, lexbuf, txtstart, txtend);
					return token;

				case LEX_CDATA: 
					if (c != ']')
					{
						continue;
					}

					/* now look for ']' */
					c = input.ReadChar();
					if (c != ']')
					{
						input.UngetChar(c);
						continue;
					}
						
					/* now look for '>' */
					c = input.ReadChar();
					if (c != '>')
					{
						input.UngetChar(c);
						continue;
					}
						
					lexsize -= 1;
					txtend = lexsize;
					lexbuf[lexsize] = (byte) '\x0000';
					state = LEX_CONTENT;
					waswhite = false;
					token = NewNode(Node.CDATATag, lexbuf, txtstart, txtend);
					return token;
				}
			}
			
			if (state == LEX_CONTENT)
			{
				/* text string */
				txtend = lexsize;
				if (txtend > txtstart)
				{
					input.UngetChar(c);
					if (lexbuf[lexsize - 1] == (byte) ' ')
					{
						lexsize -= 1;
						txtend = lexsize;
					}
					
					token = NewNode(Node.TextNode, lexbuf, txtstart, txtend);
					return token;
				}
			}
			else if (state == LEX_COMMENT)
			{
				/* comment */
				if (c == StreamIn.EndOfStream)
				{
					Report.Warning(this, null, null, Report.MALFORMED_COMMENT);
				}
				
				txtend = lexsize;
				lexbuf[lexsize] = (byte) '\x0000';
				state = LEX_CONTENT;
				waswhite = false;
				token = NewNode(Node.CommentTag, lexbuf, txtstart, txtend);
				return token;
			}
			
			return null;
		}
		
		/*
		parser for ASP within start tags
		
		Some people use ASP for to customize attributes
		Tidy isn't really well suited to dealing with ASP
		This is a workaround for attributes, but won't
		deal with the case where the ASP is used to tailor
		the attribute value. Here is an example of a work
		around for using ASP in attribute values:
		
		href="<%=rsSchool.Fields("ID").Value%>"
		
		where the ASP that generates the attribute value
		is masked from Tidy by the quotemarks.
		
		*/
		public virtual Node ParseAsp()
		{
			int c;
			Node asp = null;
			
			txtstart = lexsize;
			for (;;)
			{
				c = input.ReadChar();
				AddCharToLexer(c);

				if (c != '%')
				{
					continue;
				}

				c = input.ReadChar();
				AddCharToLexer(c);
				
				if (c == '>')
				{
					break;
				}
			}
			
			lexsize -= 2;
			txtend = lexsize;
			
			if (txtend > txtstart)
			{
				asp = NewNode(Node.AspTag, lexbuf, txtstart, txtend);
			}
			
			txtstart = txtend;
			return asp;
		}
		
		/*
		PHP is like ASP but is based upon XML
		processing instructions, e.g. <?php ... ?>
		*/
		public virtual Node ParsePhp()
		{
			int c;
			Node php = null;
			
			txtstart = lexsize;
			
			for (;;)
			{
				c = input.ReadChar();
				AddCharToLexer(c);

				if (c != '?')
				{
					continue;
				}
				
				c = input.ReadChar();
				AddCharToLexer(c);
				if (c == '>')
				{
					break;
				}
			}
			
			lexsize -= 2;
			txtend = lexsize;
			
			if (txtend > txtstart)
			{
				php = NewNode(Node.PhpTag, lexbuf, txtstart, txtend);
			}
			
			txtstart = txtend;
			return php;
		}
		
		/* consumes the '>' terminating start tags */
		public virtual string ParseAttribute(MutableBoolean isempty, MutableObject asp, MutableObject php)
		{
			int start = 0;
			// int len = 0;   Removed by BUGFIX for 126265
			short map;
			string attr;
			int c = 0;
			
			asp.Object = null; /* clear asp pointer */
			php.Object = null; /* clear php pointer */
			/* skip white space before the attribute */
			
			for (;;)
			{
				c = input.ReadChar();
				if (c == '/')
				{
					c = input.ReadChar();
					if (c == '>')
					{
						isempty.Val = true;
						return null;
					}
					
					input.UngetChar(c);
					c = '/';
					break;
				}
				
				if (c == '>')
				{
					return null;
				}
				
				if (c == '<')
				{
					c = input.ReadChar();
					
					if (c == '%')
					{
						asp.Object = ParseAsp();
						return null;
					}
					else if (c == '?')
					{
						php.Object = ParsePhp();
						return null;
					}
					
					input.UngetChar(c);
					Report.AttrError(this, token, null, Report.UNEXPECTED_GT);
					return null;
				}
				
				if (c == '"' || c == '\'')
				{
					Report.AttrError(this, token, null, Report.UNEXPECTED_QUOTEMARK);
					continue;
				}
				
				if (c == StreamIn.EndOfStream)
				{
					Report.AttrError(this, token, null, Report.UNEXPECTED_END_OF_FILE);
					input.UngetChar(c);
					return null;
				}
				
				map = MAP((char) c);
				
				if ((map & WHITE) == 0)
				{
					break;
				}
			}
			
			start = lexsize;
			
			for (;;)
			{
				/* but push back '=' for parseValue() */
				if (c == '=' || c == '>')
				{
					input.UngetChar(c);
					break;
				}
				
				if (c == '<' || c == StreamIn.EndOfStream)
				{
					input.UngetChar(c);
					break;
				}
				
				map = MAP((char) c);
				
				if ((map & WHITE) != 0)
					break;
				
				/* what should be done about non-namechar characters? */
				/* currently these are incorporated into the attr name */
				
				if (!Options.XmlTags && (map & UPPERCASE) != 0)
				{
					c += (int) ('a' - 'A');
				}
				
				//  ++len;    Removed by BUGFIX for 126265 
				AddCharToLexer(c);
				
				c = input.ReadChar();
			}
			
			// Following line added by GLP to fix BUG 126265.  This is a temporary comment
			// and should be removed when Tidy is fixed.
			int len = lexsize - start;
			attr = (len > 0?GetString(lexbuf, start, len):null);
			lexsize = start;
			
			return attr;
		}
		
		/*
		invoked when < is seen in place of attribute value
		but terminates on whitespace if not ASP, PHP or Tango
		this routine recognizes ' and " quoted strings
		*/
		public virtual int ParseServerInstruction()
		{
			int c, map, delim = '"';
			bool isrule = false;
			
			c = input.ReadChar();
			AddCharToLexer(c);
			
			/* check for ASP, PHP or Tango */
			if (c == '%' || c == '?' || c == '@')
			{
				isrule = true;
			}
			
			for (;;)
			{
				c = input.ReadChar();
				
				if (c == StreamIn.EndOfStream)
				{
					break;
				}
				
				if (c == '>')
				{
					if (isrule)
					{
						AddCharToLexer(c);
					}
					else
					{
						this.input.UngetChar(c);
					}
					break;
				}
				
				/* if not recognized as ASP, PHP or Tango */
				/* then also finish value on whitespace */
				if (!isrule)
				{
					map = MAP((char) c);
					
					if ((map & WHITE) != 0)
					{
						break;
					}
				}
				
				AddCharToLexer(c);
				
				if (c == '"')
				{
					do 
					{
						c = input.ReadChar();
						AddCharToLexer(c);
					}
					while (c != '"');
					delim = '\'';
					continue;
				}
				
				if (c == '\'')
				{
					do 
					{
						c = input.ReadChar();
						AddCharToLexer(c);
					}
					while (c != '\'');
				}
			}
			
			return delim;
		}
		
		/* values start with "=" or " = " etc. */
		/* doesn't consume the ">" at end of start tag */
		
		public virtual string ParseValue(string name, bool foldCase, MutableBoolean isempty, MutableInteger pdelim)
		{
			int len = 0;
			int start;
			short map;
			bool seen_gt = false;
			bool munge = true;
			int c = 0;
			int lastc, delim, quotewarning;
			string val;
			
			delim = 0;
			pdelim.Val = (int) '"';
			
			/*
			Henry Zrepa reports that some folk are using the
			embed element with script attributes where newlines
			are significant and must be preserved
			*/
			if (Options.LiteralAttribs)
				munge = false;
			
			/* skip white space before the '=' */
			
			for (; ; )
			{
				c = input.ReadChar();
				
				if (c == StreamIn.EndOfStream)
				{
					input.UngetChar(c);
					break;
				}
				
				map = MAP((char) c);
				
				if ((map & WHITE) == 0)
				{
					break;
				}
			}
			
			/*
			c should be '=' if there is a value
			other legal possibilities are white
			space, '/' and '>'
			*/
			
			if (c != '=')
			{
				input.UngetChar(c);
				return null;
			}
			
			/* skip white space after '=' */
			
			for (; ; )
			{
				c = input.ReadChar();
				if (c == StreamIn.EndOfStream)
				{
					input.UngetChar(c);
					break;
				}
				
				map = MAP((char) c);
				
				if ((map & WHITE) == 0)
					break;
			}
			
			/* check for quote marks */
			
			if (c == '"' || c == '\'')
				delim = c;
			else if (c == '<')
			{
				start = lexsize;
				AddCharToLexer(c);
				pdelim.Val = ParseServerInstruction();
				len = lexsize - start;
				lexsize = start;
				return (len > 0?GetString(lexbuf, start, len):null);
			}
			else
			{
				input.UngetChar(c);
			}
			
			/*
			and read the value string
			check for quote mark if needed
			*/
			
			quotewarning = 0;
			start = lexsize;
			c = '\x0000';
			
			for (; ; )
			{
				lastc = c; /* track last character */
				c = input.ReadChar();
				
				if (c == StreamIn.EndOfStream)
				{
					Report.AttrError(this, token, null, Report.UNEXPECTED_END_OF_FILE);
					input.UngetChar(c);
					break;
				}
				
				if (delim == (char) 0)
				{
					if (c == '>')
					{
						input.UngetChar(c);
						break;
					}
					
					if (c == '"' || c == '\'')
					{
						Report.AttrError(this, token, null, Report.UNEXPECTED_QUOTEMARK);
						break;
					}
					
					if (c == '<')
					{
						/* in.UngetChar(c); */
						Report.AttrError(this, token, null, Report.UNEXPECTED_GT);
						/* break; */
					}
					
					/*
					For cases like <br clear=all/> need to avoid treating /> as
					part of the attribute value, however care is needed to avoid
					so treating <a href=http://www.acme.com/> in this way, which
					would map the <a> tag to <a href="http://www.acme.com"/>
					*/
					if (c == '/')
					{
						/* peek ahead in case of /> */
						c = input.ReadChar();
						if (c == '>' && !AttributeTable.DefaultAttributeTable.IsUrl(name))
						{
							isempty.Val = true;
							input.UngetChar(c);
							break;
						}
						
						/* unget peeked char */
						input.UngetChar(c);
						c = '/';
					}
				}
					/* delim is '\'' or '"' */
				else
				{
					if (c == delim)
					{
						break;
					}
					
					/* treat CRLF, CR and LF as single line break */
					
					if (c == '\r')
					{
						c = input.ReadChar();
						if (c != '\n')
						{
							input.UngetChar(c);
						}
						
						c = '\n';
					}
					
					if (c == '\n' || c == '<' || c == '>')
						++quotewarning;
					
					if (c == '>')
						seen_gt = true;
				}
				
				if (c == '&')
				{
					AddCharToLexer(c);
					ParseEntity((short) 0);
					continue;
				}
				
				/*
				kludge for JavaScript attribute values
				with line continuations in string literals
				*/
				if (c == '\\')
				{
					c = input.ReadChar();
					
					if (c != '\n')
					{
						input.UngetChar(c);
						c = '\\';
					}
				}
				
				map = MAP((char) c);
				
				if ((map & WHITE) != 0)
				{
					if (delim == (char) 0)
						break;
					
					if (munge)
					{
						c = ' ';
						
						if (lastc == ' ')
							continue;
					}
				}
				else if (foldCase && (map & UPPERCASE) != 0)
					c += (int) ('a' - 'A');
				
				AddCharToLexer(c);
			}
			
			if (quotewarning > 10 && seen_gt && munge)
			{
				/*
				there is almost certainly a missing trailling quote mark
				as we have see too many newlines, < or > characters.
				
				an exception is made for Javascript attributes and the
				javascript URL scheme which may legitimately include < and >
				*/
				if (!AttributeTable.DefaultAttributeTable.IsScript(name) && !(AttributeTable.DefaultAttributeTable.IsUrl(name) && (GetString(lexbuf, start, 11)).Equals("javascript:")))
					Report.Error(this, null, null, Report.SUSPECTED_MISSING_QUOTE);
			}
			
			len = lexsize - start;
			lexsize = start;
			
			if (len > 0 || delim != 0)
			{
				val = GetString(lexbuf, start, len);
			}
			else
			{
				val = null;
			}
			
			/* note delimiter if given */
			if (delim != 0)
				pdelim.Val = delim;
			else
				pdelim.Val = (int) '"';
			
			return val;
		}
		
		/* attr must be non-null */
		public static bool IsValidAttrName(string attr)
		{
			short map;
			char c;
			int i;
			
			/* first character should be a letter */
			c = attr[0];
			map = MAP(c);
			
			if (!((map & LETTER) != 0))
				return false;
			
			/* remaining characters should be namechars */
			for (i = 1; i < attr.Length; i++)
			{
				c = attr[i];
				map = MAP(c);
				
				if ((map & NAMECHAR) != 0)
					continue;
				
				return false;
			}
			
			return true;
		}
		
		/* swallows closing '>' */
		
		public virtual AttVal ParseAttrs(MutableBoolean isempty)
		{
			AttVal av, list;
			string attribute, val;
			MutableInteger delim = new MutableInteger();
			MutableObject asp = new MutableObject();
			MutableObject php = new MutableObject();
			
			list = null;
			
			while (!EndOfInput())
			{
				attribute = ParseAttribute(isempty, asp, php);
				
				if (attribute == null)
				{
					/* check if attributes are created by ASP markup */
					if (asp.Object != null)
					{
						av = new AttVal(list, null, (Node) asp.Object, null, '\x0000', null, null);
						list = av;
						continue;
					}
					
					/* check if attributes are created by PHP markup */
					if (php.Object != null)
					{
						av = new AttVal(list, null, null, (Node) php.Object, '\x0000', null, null);
						list = av;
						continue;
					}
					
					break;
				}
				
				val = ParseValue(attribute, false, isempty, delim);
				
				if (attribute != null && IsValidAttrName(attribute))
				{
					av = new AttVal(list, null, null, null, delim.Val, attribute, val);
					av.Dict = AttributeTable.DefaultAttributeTable.FindAttribute(av);
					list = av;
				}
				else
				{
					av = new AttVal(null, null, null, null, 0, attribute, val);
					Report.AttrError(this, token, val, Report.BAD_ATTRIBUTE_VALUE);
				}
			}
			
			return list;
		}
		
		/*
		push a copy of an inline node onto stack
		but don't push if implicit or OBJECT or APPLET
		(implicit tags are ones generated from the istack)
		
		One issue arises with pushing inlines when
		the tag is already pushed. For instance:
		
		<p><em>text
		<p><em>more text
		
		Shouldn't be mapped to
		
		<p><em>text</em></p>
		<p><em><em>more text</em></em>
		*/
		public virtual void PushInline(Node node)
		{
			InlineStack stack;
			
			if (node.Isimplicit)
				return;
			
			if (node.Tag == null)
				return;
			
			if ((node.Tag.Model & ContentModel.Inline) == 0)
				return;
			
			if ((node.Tag.Model & ContentModel.Object) != 0)
				return;
			
			if (node.Tag != Options.tt.TagFont && IsPushed(node))
				return;
			
			// make sure there is enough space for the stack
			stack = new InlineStack();
			stack.Tag = node.Tag;
			stack.Element = node.Element;
			if (node.Attributes != null)
			{
				stack.Attributes = CloneAttributes(node.Attributes);
			}
			
			istack.Push(stack);
		}
		
		/* pop inline stack */
		public virtual void PopInline(Node node)
		{
			InlineStack stack;
			
			if (node != null)
			{
				
				if (node.Tag == null)
					return;
				
				if ((node.Tag.Model & ContentModel.Inline) == 0)
					return;
				
				if ((node.Tag.Model & ContentModel.Object) != 0)
					return;
				
				// if node is </a> then pop until we find an <a>
				if (node.Tag == Options.tt.TagA)
				{
					
					while (istack.Count > 0)
					{
						stack = (InlineStack) istack.Pop();
						if (stack.Tag == Options.tt.TagA)
						{
							break;
						}
					}
					
					if (insert >= istack.Count)
					{
						insert = - 1;
					}
					return;
				}
			}
			
			if (istack.Count > 0)
			{
				stack = (InlineStack) istack.Pop();
				if (insert >= istack.Count)
				{
					insert = - 1;
				}
			}
		}
		
		public virtual bool IsPushed(Node node)
		{
			int i;
			InlineStack stack;
			
			for (i = istack.Count - 1; i >= 0; --i)
			{
				stack = (InlineStack) (istack.ToArray())[istack.Count - (i + 1)];
				if (stack.Tag == node.Tag)
				{
					return true;
				}
			}
			
			return false;
		}
		
		/*
		This has the effect of inserting "missing" inline
		elements around the contents of blocklevel elements
		such as P, TD, TH, DIV, PRE etc. This procedure is
		called at the start of ParseBlock. when the inline
		stack is not empty, as will be the case in:
		
		<i><h1>italic heading</h1></i>
		
		which is then treated as equivalent to
		
		<h1><i>italic heading</i></h1>
		
		This is implemented by setting the lexer into a mode
		where it gets tokens from the inline stack rather than
		from the input stream.
		*/
		public virtual int InlineDup(Node node)
		{
			int n;
			
			n = istack.Count - istackbase;
			if (n > 0)
			{
				insert = istackbase;
				inode = node;
			}
			
			return n;
		}
		
		public virtual Node InsertedToken()
		{
			Node node;
			InlineStack stack;
			int n;
			
			// this will only be null if inode != null
			if (insert == - 1)
			{
				node = inode;
				inode = null;
				return node;
			}
			
			// is this is the "latest" node then update
			// the position, otherwise use current values
			
			if (inode == null)
			{
				lines = input.curline;
				columns = input.curcol;
			}
			
			node = NewNode(Node.StartTag, lexbuf, txtstart, txtend); // GLP:  Bugfix 126261.  Remove when this change
			//       is fixed in istack.c in the original Tidy
			node.Isimplicit = true;
			stack = (InlineStack) (istack.ToArray())[istack.Count - (insert + 1)];
			node.Element = stack.Element;
			node.Tag = stack.Tag;
			if (stack.Attributes != null)
			{
				node.Attributes = CloneAttributes(stack.Attributes);
			}
			
			// advance lexer to next item on the stack
			n = insert;
			
			// and recover state if we have reached the end
			if (++n < istack.Count)
			{
				insert = n;
			}
			else
			{
				insert = - 1;
			}
			
			return node;
		}

		public virtual bool CanPrune(Node element)
		{
			if (element.Type == Node.TextNode)
				return true;
			
			if (element.Content != null)
				return false;
			
			if (element.Tag == Options.tt.TagA && element.Attributes != null)
				return false;
			
			if (element.Tag == Options.tt.TagP && !Options.DropEmptyParas)
				return false;
			
			if (element.Tag == null)
				return false;
			
			if ((element.Tag.Model & ContentModel.Row) != 0)
				return false;
			
			if (element.Tag == Options.tt.TagApplet)
				return false;
			
			if (element.Tag == Options.tt.TagObject)
				return false;
			
			if (element.Attributes != null && (element.GetAttrByName("id") != null || element.GetAttrByName("name") != null))
				return false;
			
			return true;
		}
		
		/* duplicate name attribute as an id */
		public virtual void FixId(Node node)
		{
			AttVal name = node.GetAttrByName("name");
			AttVal id = node.GetAttrByName("id");
			
			if (name != null)
			{
				if (id != null)
				{
					if (!id.Val.Equals(name.Val))
					{
						Report.AttrError(this, node, "name", Report.ID_NAME_MISMATCH);
					}
				}
				else if (Options.XmlOut)
				{
					node.AddAttribute("id", name.Val);
				}
			}
		}
		
		/*
		defer duplicates when entering a table or other
		element where the inlines shouldn't be duplicated
		*/
		public virtual void DeferDup()
		{
			insert = - 1;
			inode = null;
		}
		
		/* Private methods and fields */
		
		/* lexer char types */
		private const short DIGIT = 1;
		private const short LETTER = 2;
		private const short NAMECHAR = 4;
		private const short WHITE = 8;
		private const short NEWLINE = 16;
		private const short LOWERCASE = 32;
		private const short UPPERCASE = 64;
		
		/* lexer GetToken states */
		
		private const short LEX_CONTENT = 0;
		private const short LEX_GT = 1;
		private const short LEX_ENDTAG = 2;
		private const short LEX_STARTTAG = 3;
		private const short LEX_COMMENT = 4;
		private const short LEX_DOCTYPE = 5;
		private const short LEX_PROCINSTR = 6;
		private const short LEX_ENDCOMMENT = 7;
		private const short LEX_CDATA = 8;
		private const short LEX_SECTION = 9;
		private const short LEX_ASP = 10;
		private const short LEX_JSTE = 11;
		private const short LEX_PHP = 12;
		
		private static void MapStr(string str, int code)
		{
			int j;
			
			for (int i = 0; i < str.Length; i++)
			{
				j = (int) str[i];
				lexmap[j] |= code;
			}
		}
		
		
		private static short MAP(char c)
		{
			return ((int) c < 128 ? (short)lexmap[(int) c] : (short)0);
		}
		
		private static bool IsWhite(char c)
		{
			short m = MAP(c);
			
			return (m & WHITE) != 0;
		}
		
		private static bool IsDigit(char c)
		{
			short m;
			
			m = MAP(c);
			
			return (m & DIGIT) != 0;
		}
		
		private static bool IsLetter(char c)
		{
			short m;
			
			m = MAP(c);
			
			return (m & LETTER) != 0;
		}
		
		private static char ToLower(char c)
		{
			short m = MAP(c);
			
			if ((m & UPPERCASE) != 0)
				c = (char) ((int) c + (int) 'a' - (int) 'A');
			
			return c;
		}
		
		private static char ToUpper(char c)
		{
			short m = MAP(c);
			
			if ((m & LOWERCASE) != 0)
				c = (char) ((int) c + (int) 'A' - (int) 'a');
			
			return c;
		}
		
		public static char FoldCase(char c, bool tocaps, bool xmlTags)
		{
			short m;
			
			if (!xmlTags)
			{
				m = MAP(c);
				
				if (tocaps)
				{
					if ((m & LOWERCASE) != 0)
						c = (char) ((int) c + (int) 'A' - (int) 'a');
				}
					/* force to lower case */
				else
				{
					if ((m & UPPERCASE) != 0)
						c = (char) ((int) c + (int) 'a' - (int) 'A');
				}
			}
			
			return c;
		}
		
		
		private class W3CVersionInfo
		{
			internal string Name;
			internal string VoyagerName;
			internal string Profile;
			internal HtmlVersion Version;
			
			public W3CVersionInfo(string name, string voyagerName, string profile, HtmlVersion version)
			{
				Name = name;
				VoyagerName = voyagerName;
				Profile = profile;
				Version = version;
			}
		}
		
		/* the 3 URIs  for the XHTML 1.0 DTDs */
		private const string voyager_loose = "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd";
		private const string voyager_strict = "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd";
		private const string voyager_frameset = "http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd";
		
		private const string XHTML_NAMESPACE = "http://www.w3.org/1999/xhtml";
		
		private static Lexer.W3CVersionInfo[] W3CVersion = new Lexer.W3CVersionInfo[]
			{
				new W3CVersionInfo("HTML 4.01", "XHTML 1.0 Strict", voyager_strict, HtmlVersion.Html40Strict),
				new W3CVersionInfo("HTML 4.01 Transitional", "XHTML 1.0 Transitional", voyager_loose, HtmlVersion.Html40Loose), 
				new W3CVersionInfo("HTML 4.01 Frameset", "XHTML 1.0 Frameset", voyager_frameset, HtmlVersion.Frames),
				new W3CVersionInfo("HTML 4.0", "XHTML 1.0 Strict", voyager_strict, HtmlVersion.Html40Strict),
				new W3CVersionInfo("HTML 4.0 Transitional", "XHTML 1.0 Transitional", voyager_loose, HtmlVersion.Html40Loose),
				new W3CVersionInfo("HTML 4.0 Frameset", "XHTML 1.0 Frameset", voyager_frameset, HtmlVersion.Frames),
				new W3CVersionInfo("HTML 3.2", "XHTML 1.0 Transitional", voyager_loose, HtmlVersion.Html32),
				new W3CVersionInfo("HTML 2.0", "XHTML 1.0 Strict", voyager_strict, HtmlVersion.Html20)
			};

		/* used to classify chars for lexical purposes */
		private static int[] lexmap = new int[128];

		static Lexer()
		{
			MapStr("\r\n\f", (short) (NEWLINE | WHITE));
			MapStr(" \t", WHITE);
			MapStr("-.:_", NAMECHAR);
			MapStr("0123456789", (short) (DIGIT | NAMECHAR));
#if BUG_REPORT
			MapStr("abcdef", (short)(DIGIT | LOWERCASE | LETTER | NAMECHAR));
			MapStr("ABCDEF", (short)(DIGIT | UPPERCASE | LETTER | NAMECHAR));
			MapStr("ghijklmnopqrstuvwxyz", (short)(LOWERCASE | LETTER | NAMECHAR));
			MapStr("GHIJKLMNOPQRSTUVWXYZ", (short)(UPPERCASE | LETTER | NAMECHAR));
#else
			MapStr("abcdefghijklmnopqrstuvwxyz", (short) (LOWERCASE | LETTER | NAMECHAR));
			MapStr("ABCDEFGHIJKLMNOPQRSTUVWXYZ", (short) (UPPERCASE | LETTER | NAMECHAR));
#endif
		}
	}
}
