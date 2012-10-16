using System;
using TidyNet.Dom;

namespace TidyNet
{
	/// <summary>
	/// HTML Parser implementation
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
	internal class ParserImpl
	{
		public static IParser ParseHTML
		{
			get
			{
				return _parseHTML;
			}
		}

		public static IParser ParseHead
		{
			get
			{
				return _parseHead;
			}
		}

		public static IParser ParseTitle
		{
			get
			{
				return _parseTitle;
			}
		}

		public static IParser ParseScript
		{
			get
			{
				return _parseScript;
			}
		}

		public static IParser ParseBody
		{
			get
			{
				return _parseBody;
			}
		}

		public static IParser ParseFrameSet
		{
			get
			{
				return _parseFrameSet;
			}
		}

		public static IParser ParseInline
		{
			get
			{
				return _parseInline;
			}
		}

		public static IParser ParseList
		{
			get
			{
				return _parseList;
			}
		}

		public static IParser ParseDefList
		{
			get
			{
				return _parseDefList;
			}
		}

		public static IParser ParsePre
		{
			get
			{
				return _parsePre;
			}
		}

		public static IParser ParseBlock
		{
			get
			{
				return _parseBlock;
			}
		}

		public static IParser ParseTableTag
		{
			get
			{
				return _parseTableTag;
			}
		}

		public static IParser ParseColGroup
		{
			get
			{
				return _parseColGroup;
			}
		}

		public static IParser ParseRowGroup
		{
			get
			{
				return _parseRowGroup;
			}
		}

		public static IParser ParseRow
		{
			get
			{
				return _parseRow;
			}
		}

		public static IParser ParseNoFrames
		{
			get
			{
				return _parseNoFrames;
			}
		}

		public static IParser ParseSelect
		{
			get
			{
				return _parseSelect;
			}
		}

		public static IParser ParseText
		{
			get
			{
				return _parseText;
			}
		}

		public static IParser ParseOptGroup
		{
			get
			{
				return _parseOptGroup;
			}
		}
				
		private static void  parseTag(Lexer lexer, Node node, short mode)
		{
			// Local fix by GLP 2000-12-21.  Need to reset insertspace if this 
			// is both a non-inline and empty tag (base, link, meta, isindex, hr, area).
			// Remove this code once the fix is made in Tidy.
			
			/// <summary>***  (Original code follows)
			/// if ((node.Tag.Model & ContentModel.Empty) != 0)
			/// {
			/// lexer.waswhite = false;
			/// return;
			/// }
			/// else if (!((node.Tag.Model & ContentModel.INLINE) != 0))
			/// lexer.insertspace = false;
			/// *****
			/// </summary>
			
			if (!((node.Tag.Model & ContentModel.Inline) != 0))
			{
				lexer.insertspace = false;
			}
			
			if ((node.Tag.Model & ContentModel.Empty) != 0)
			{
				lexer.waswhite = false;
				return;
			}
			
			if (node.Tag.Parser == null || node.Type == Node.StartEndTag)
			{
				return;
			}
			
			node.Tag.Parser.Parse(lexer, node, mode);
		}

		private static void moveToHead(Lexer lexer, Node element, Node node)
		{
			Node head;
			TagTable tt = lexer.Options.tt;
			
			
			if (node.Type == Node.StartTag || node.Type == Node.StartEndTag)
			{
				Report.Warning(lexer, element, node, Report.TAG_NOT_ALLOWED_IN);
				
				while (element.Tag != tt.TagHtml)
				{
					element = element.Parent;
				}
				
				for (head = element.Content; head != null; head = head.Next)
				{
					if (head.Tag == tt.TagHead)
					{
						Node.InsertNodeAtEnd(head, node);
						break;
					}
				}
				
				if (node.Tag.Parser != null)
				{
					parseTag(lexer, node, Lexer.IgnoreWhitespace);
				}
			}
			else
			{
				Report.Warning(lexer, element, node, Report.DISCARDING_UNEXPECTED);
			}
		}
		
		public class ParseHTMLCheckTable : IParser
		{
			public virtual void Parse(Lexer lexer, Node html, short mode)
			{
				Node node, head;
				Node frameset = null;
				Node noframes = null;
				
				lexer.Options.XmlTags = false;
				lexer.seenBodyEndTag = 0;
				TagTable tt = lexer.Options.tt;
				
				for (;;)
				{
					node = lexer.GetToken(Lexer.IgnoreWhitespace);
					
					if (node == null)
					{
						node = lexer.InferredTag("head");
						break;
					}
					
					if (node.Tag == tt.TagHead)
						break;
					
					if (node.Tag == html.Tag && node.Type == Node.EndTag)
					{
						Report.Warning(lexer, html, node, Report.DISCARDING_UNEXPECTED);
						continue;
					}
					
					/* deal with comments etc. */
					if (Node.InsertMisc(html, node))
					{
						continue;
					}
					
					lexer.UngetToken();
					node = lexer.InferredTag("head");
					break;
				}
				
				head = node;
				Node.InsertNodeAtEnd(html, head);
				TidyNet.ParserImpl.ParseHead.Parse(lexer, head, mode);
				
				for (;;)
				{
					node = lexer.GetToken(Lexer.IgnoreWhitespace);
					
					if (node == null)
					{
						if (frameset == null)
						{
							/* create an empty body */
							node = lexer.InferredTag("body");
						}

						return;
					}
					
					/* robustly handle html tags */
					if (node.Tag == html.Tag)
					{
						if (node.Type != Node.StartTag && frameset == null)
						{
							Report.Warning(lexer, html, node, Report.DISCARDING_UNEXPECTED);
						}
						
						continue;
					}
					
					/* deal with comments etc. */
					if (Node.InsertMisc(html, node))
					{
						continue;
					}
					
					/* if frameset document coerce <body> to <noframes> */
					if (node.Tag == tt.TagBody)
					{
						if (node.Type != Node.StartTag)
						{
							Report.Warning(lexer, html, node, Report.DISCARDING_UNEXPECTED);
							continue;
						}
						
						if (frameset != null)
						{
							lexer.UngetToken();
							
							if (noframes == null)
							{
								noframes = lexer.InferredTag("noframes");
								Node.InsertNodeAtEnd(frameset, noframes);
								Report.Warning(lexer, html, noframes, Report.INSERTING_TAG);
							}
							
							TidyNet.ParserImpl.parseTag(lexer, noframes, mode);
							continue;
						}
						
						break; /* to parse body */
					}
					
					/* flag an error if we see more than one frameset */
					if (node.Tag == tt.TagFrameset)
					{
						if (node.Type != Node.StartTag)
						{
							Report.Warning(lexer, html, node, Report.DISCARDING_UNEXPECTED);
							continue;
						}
						
						if (frameset != null)
						{
							Report.Error(lexer, html, node, Report.DUPLICATE_FRAMESET);
						}
						else
						{
							frameset = node;
						}
						
						Node.InsertNodeAtEnd(html, node);
						TidyNet.ParserImpl.parseTag(lexer, node, mode);
						
						/*
						see if it includes a noframes element so
						that we can merge subsequent noframes elements
						*/
						
						for (node = frameset.Content; node != null; node = node.Next)
						{
							if (node.Tag == tt.TagNoframes)
							{
								noframes = node;
							}
						}
						continue;
					}
					
					/* if not a frameset document coerce <noframes> to <body> */
					if (node.Tag == tt.TagNoframes)
					{
						if (node.Type != Node.StartTag)
						{
							Report.Warning(lexer, html, node, Report.DISCARDING_UNEXPECTED);
							continue;
						}
						
						if (frameset == null)
						{
							Report.Warning(lexer, html, node, Report.DISCARDING_UNEXPECTED);
							node = lexer.InferredTag("body");
							break;
						}
						
						if (noframes == null)
						{
							noframes = node;
							Node.InsertNodeAtEnd(frameset, noframes);
						}
						
						TidyNet.ParserImpl.parseTag(lexer, noframes, mode);
						continue;
					}
					
					if (node.Type == Node.StartTag || node.Type == Node.StartEndTag)
					{
						if (node.Tag != null && (node.Tag.Model & ContentModel.Head) != 0)
						{
							TidyNet.ParserImpl.moveToHead(lexer, html, node);
							continue;
						}
					}
					
					lexer.UngetToken();
					
					/* insert other content into noframes element */
					
					if (frameset != null)
					{
						if (noframes == null)
						{
							noframes = lexer.InferredTag("noframes");
							Node.InsertNodeAtEnd(frameset, noframes);
						}
						else
						{
							Report.Warning(lexer, html, node, Report.NOFRAMES_CONTENT);
						}

						TidyNet.ParserImpl.parseTag(lexer, noframes, mode);
						continue;
					}
					
					node = lexer.InferredTag("body");
					break;
				}
				
				/* node must be body */
				
				Node.InsertNodeAtEnd(html, node);
				TidyNet.ParserImpl.parseTag(lexer, node, mode);
			}
		}
		
		
		public class ParseHeadCheckTable : IParser
		{
			public virtual void Parse(Lexer lexer, Node head, short mode)
			{
				Node node;
				int HasTitle = 0;
				int HasBase = 0;
				TagTable tt = lexer.Options.tt;
				
				while (true)
				{
					node = lexer.GetToken(Lexer.IgnoreWhitespace);
					if (node == null)
					{
						break;
					}
					if (node.Tag == head.Tag && node.Type == Node.EndTag)
					{
						head.Closed = true;
						break;
					}
					
					if (node.Type == Node.TextNode)
					{
						lexer.UngetToken();
						break;
					}
					
					/* deal with comments etc. */
					if (Node.InsertMisc(head, node))
					{
						continue;
					}
					
					if (node.Type == Node.DocTypeTag)
					{
						Node.InsertDocType(lexer, head, node);
						continue;
					}
					
					/* discard unknown tags */
					if (node.Tag == null)
					{
						Report.Warning(lexer, head, node, Report.DISCARDING_UNEXPECTED);
						continue;
					}
					
					if (!((node.Tag.Model & ContentModel.Head) != 0))
					{
						lexer.UngetToken();
						break;
					}
					
					if (node.Type == Node.StartTag || node.Type == Node.StartEndTag)
					{
						if (node.Tag == tt.TagTitle)
						{
							++HasTitle;
							
							if (HasTitle > 1)
							{
								Report.Warning(lexer, head, node, Report.TOO_MANY_ELEMENTS);
							}
						}
						else if (node.Tag == tt.TagBase)
						{
							++HasBase;
							
							if (HasBase > 1)
							{
								Report.Warning(lexer, head, node, Report.TOO_MANY_ELEMENTS);
							}
						}
						else if (node.Tag == tt.TagNoscript)
						{
							Report.Warning(lexer, head, node, Report.TAG_NOT_ALLOWED_IN);
						}
						
						Node.InsertNodeAtEnd(head, node);
						TidyNet.ParserImpl.parseTag(lexer, node, Lexer.IgnoreWhitespace);
						continue;
					}
					
					/* discard unexpected text nodes and end tags */
					Report.Warning(lexer, head, node, Report.DISCARDING_UNEXPECTED);
				}
				
				if (HasTitle == 0)
				{
					Report.Warning(lexer, head, null, Report.MISSING_TITLE_ELEMENT);
					Node.InsertNodeAtEnd(head, lexer.InferredTag("title"));
				}
			}
		}
		
		
		public class ParseTitleCheckTable : IParser
		{
			public virtual void Parse(Lexer lexer, Node title, short mode)
			{
				Node node;
				
				while (true)
				{
					node = lexer.GetToken(Lexer.MixedContent);
					if (node == null)
					{
						break;
					}
					if (node.Tag == title.Tag && node.Type == Node.EndTag)
					{
						title.Closed = true;
						Node.TrimSpaces(lexer, title);
						return;
					}
					
					if (node.Type == Node.TextNode)
					{
						/* only called for 1st child */
						if (title.Content == null)
						{
							Node.TrimInitialSpace(lexer, title, node);
						}
						
						if (node.Start >= node.End)
						{
							continue;
						}
						
						Node.InsertNodeAtEnd(title, node);
						continue;
					}
					
					/* deal with comments etc. */
					if (Node.InsertMisc(title, node))
					{
						continue;
					}
					
					/* discard unknown tags */
					if (node.Tag == null)
					{
						Report.Warning(lexer, title, node, Report.DISCARDING_UNEXPECTED);
						continue;
					}
					
					/* pushback unexpected tokens */
					Report.Warning(lexer, title, node, Report.MISSING_ENDTAG_BEFORE);
					lexer.UngetToken();
					Node.TrimSpaces(lexer, title);
					return;
				}
				
				Report.Warning(lexer, title, node, Report.MISSING_ENDTAG_FOR);
			}
		}
		
		
		public class ParseScriptCheckTable : IParser
		{
			public virtual void Parse(Lexer lexer, Node script, short mode)
			{
				/*
				This isn't quite right for CDATA content as it recognises
				tags within the content and parses them accordingly.
				This will unfortunately screw up scripts which include
				< + letter,  < + !, < + ?  or  < + / + letter
				*/
				
				Node node;
				
				node = lexer.GetCDATA(script);
				
				if (node != null)
				{
					Node.InsertNodeAtEnd(script, node);
				}
			}
		}
		
		
		public class ParseBodyCheckTable : IParser
		{
			public virtual void Parse(Lexer lexer, Node body, short mode)
			{
				Node node;
				bool checkstack, iswhitenode;
				
				mode = Lexer.IgnoreWhitespace;
				checkstack = true;
				TagTable tt = lexer.Options.tt;
				
				while (true)
				{
					node = lexer.GetToken(mode);
					if (node == null)
					{
						break;
					}
					if (node.Tag == body.Tag && node.Type == Node.EndTag)
					{
						body.Closed = true;
						Node.TrimSpaces(lexer, body);
						lexer.seenBodyEndTag = 1;
						mode = Lexer.IgnoreWhitespace;
						
						if (body.Parent.Tag == tt.TagNoframes)
						{
							break;
						}
						
						continue;
					}
					
					if (node.Tag == tt.TagNoframes)
					{
						if (node.Type == Node.StartTag)
						{
							Node.InsertNodeAtEnd(body, node);
							TidyNet.ParserImpl.ParseBlock.Parse(lexer, node, mode);
							continue;
						}
						
						if (node.Type == Node.EndTag && body.Parent.Tag == tt.TagNoframes)
						{
							Node.TrimSpaces(lexer, body);
							lexer.UngetToken();
							break;
						}
					}
					
					if ((node.Tag == tt.TagFrame || node.Tag == tt.TagFrameset) && body.Parent.Tag == tt.TagNoframes)
					{
						Node.TrimSpaces(lexer, body);
						lexer.UngetToken();
						break;
					}
					
					if (node.Tag == tt.TagHtml)
					{
						if (node.Type == Node.StartTag || node.Type == Node.StartEndTag)
						{
							Report.Warning(lexer, body, node, Report.DISCARDING_UNEXPECTED);
						}
						
						continue;
					}
					
					iswhitenode = false;
					
					if (node.Type == Node.TextNode && node.End <= node.Start + 1 && node.Textarray[node.Start] == (sbyte) ' ')
					{
						iswhitenode = true;
					}
					
					/* deal with comments etc. */
					if (Node.InsertMisc(body, node))
					{
						continue;
					}
					
					if (lexer.seenBodyEndTag == 1 && !iswhitenode)
					{
						++lexer.seenBodyEndTag;
						Report.Warning(lexer, body, node, Report.CONTENT_AFTER_BODY);
					}
					
					/* mixed content model permits text */
					if (node.Type == Node.TextNode)
					{
						if (iswhitenode && mode == Lexer.IgnoreWhitespace)
						{
							continue;
						}
						
						if (lexer.Options.EncloseText && !iswhitenode)
						{
							Node para;
							
							lexer.UngetToken();
							para = lexer.InferredTag("p");
							Node.InsertNodeAtEnd(body, para);
							TidyNet.ParserImpl.parseTag(lexer, para, mode);
							mode = Lexer.MixedContent;
							continue;
						}
						else
						{
							/* strict doesn't allow text here */
							lexer.versions &= ~ (HtmlVersion.Html40Strict | HtmlVersion.Html20);
						}
						
						if (checkstack)
						{
							checkstack = false;
							
							if (lexer.InlineDup(node) > 0)
							{
								continue;
							}
						}
						
						Node.InsertNodeAtEnd(body, node);
						mode = Lexer.MixedContent;
						continue;
					}
					
					if (node.Type == Node.DocTypeTag)
					{
						Node.InsertDocType(lexer, body, node);
						continue;
					}
					/* discard unknown  and PARAM tags */
					if (node.Tag == null || node.Tag == tt.TagParam)
					{
						Report.Warning(lexer, body, node, Report.DISCARDING_UNEXPECTED);
						continue;
					}
					
					/*
					Netscape allows LI and DD directly in BODY
					We infer UL or DL respectively and use this
					boolean to exclude block-level elements so as
					to match Netscape's observed behaviour.
					*/
					lexer.excludeBlocks = false;
					
					if (!((node.Tag.Model & ContentModel.Block) != 0) && !((node.Tag.Model & ContentModel.Inline) != 0))
					{
						/* avoid this error message being issued twice */
						if (!((node.Tag.Model & ContentModel.Head) != 0))
						{
							Report.Warning(lexer, body, node, Report.TAG_NOT_ALLOWED_IN);
						}
						
						if ((node.Tag.Model & ContentModel.Html) != 0)
						{
							/* copy body attributes if current body was inferred */
							if (node.Tag == tt.TagBody && body.Isimplicit && body.Attributes == null)
							{
								body.Attributes = node.Attributes;
								node.Attributes = null;
							}
							
							continue;
						}
						
						if ((node.Tag.Model & ContentModel.Head) != 0)
						{
							TidyNet.ParserImpl.moveToHead(lexer, body, node);
							continue;
						}
						
						if ((node.Tag.Model & ContentModel.List) != 0)
						{
							lexer.UngetToken();
							node = lexer.InferredTag("ul");
							Node.AddClass(node, "noindent");
							lexer.excludeBlocks = true;
						}
						else if ((node.Tag.Model & ContentModel.Deflist) != 0)
						{
							lexer.UngetToken();
							node = lexer.InferredTag("dl");
							lexer.excludeBlocks = true;
						}
						else if ((node.Tag.Model & (ContentModel.Table | ContentModel.Rowgrp | ContentModel.Row)) != 0)
						{
							lexer.UngetToken();
							node = lexer.InferredTag("table");
							lexer.excludeBlocks = true;
						}
						else
						{
							/* AQ: The following line is from the official C
							version of tidy.  It doesn't make sense to me
							because the '!' operator has higher precedence
							than the '&' operator.  It seems to me that the
							expression always evaluates to 0.
							
							if (!node->tag->model & (CM_ROW | CM_FIELD))
							
							AQ: 13Jan2000 fixed in C tidy
							*/
							if (!((node.Tag.Model & (ContentModel.Row | ContentModel.Field)) != 0))
							{
								lexer.UngetToken();
								return;
							}
							
							/* ignore </td> </th> <option> etc. */
							continue;
						}
					}
					
					if (node.Type == Node.EndTag)
					{
						if (node.Tag == tt.TagBr)
						{
							node.Type = Node.StartTag;
						}
						else if (node.Tag == tt.TagP)
						{
							Node.CoerceNode(lexer, node, tt.TagBr);
							Node.InsertNodeAtEnd(body, node);
							node = lexer.InferredTag("br");
						}
						else if ((node.Tag.Model & ContentModel.Inline) != 0)
						{
							lexer.PopInline(node);
						}
					}
					
					if (node.Type == Node.StartTag || node.Type == Node.StartEndTag)
					{
						if (((node.Tag.Model & ContentModel.Inline) != 0) && !((node.Tag.Model & ContentModel.Mixed) != 0))
						{
							/* HTML4 strict doesn't allow inline content here */
							/* but HTML2 does allow img elements as children of body */
							if (node.Tag == tt.TagImg)
							{
								lexer.versions &= ~ HtmlVersion.Html40Strict;
							}
							else
							{
								lexer.versions &= ~ (HtmlVersion.Html40Strict | HtmlVersion.Html20);
							}
							
							if (checkstack && !node.Isimplicit)
							{
								checkstack = false;
								
								if (lexer.InlineDup(node) > 0)
								{
									continue;
								}
							}
							
							mode = Lexer.MixedContent;
						}
						else
						{
							checkstack = true;
							mode = Lexer.IgnoreWhitespace;
						}
						
						if (node.Isimplicit)
						{
							Report.Warning(lexer, body, node, Report.INSERTING_TAG);
						}
						
						Node.InsertNodeAtEnd(body, node);
						TidyNet.ParserImpl.parseTag(lexer, node, mode);
						continue;
					}
					
					/* discard unexpected tags */
					Report.Warning(lexer, body, node, Report.DISCARDING_UNEXPECTED);
				}
			}
		}
		
		
		public class ParseFrameSetCheckTable : IParser
		{
			public virtual void Parse(Lexer lexer, Node frameset, short mode)
			{
				Node node;
				TagTable tt = lexer.Options.tt;
				
				lexer.badAccess |= Report.USING_FRAMES;
				
				while (true)
				{
					node = lexer.GetToken(Lexer.IgnoreWhitespace);
					if (node == null)
					{
						break;
					}
					if (node.Tag == frameset.Tag && node.Type == Node.EndTag)
					{
						frameset.Closed = true;
						Node.TrimSpaces(lexer, frameset);
						return;
					}
					
					/* deal with comments etc. */
					if (Node.InsertMisc(frameset, node))
					{
						continue;
					}
					
					if (node.Tag == null)
					{
						Report.Warning(lexer, frameset, node, Report.DISCARDING_UNEXPECTED);
						continue;
					}
					
					if (node.Type == Node.StartTag || node.Type == Node.StartEndTag)
					{
						if (node.Tag != null && (node.Tag.Model & ContentModel.Head) != 0)
						{
							TidyNet.ParserImpl.moveToHead(lexer, frameset, node);
							continue;
						}
					}
					
					if (node.Tag == tt.TagBody)
					{
						lexer.UngetToken();
						node = lexer.InferredTag("noframes");
						Report.Warning(lexer, frameset, node, Report.INSERTING_TAG);
					}
					
					if (node.Type == Node.StartTag && (node.Tag.Model & ContentModel.Frames) != 0)
					{
						Node.InsertNodeAtEnd(frameset, node);
						lexer.excludeBlocks = false;
						TidyNet.ParserImpl.parseTag(lexer, node, Lexer.MixedContent);
						continue;
					}
					else if (node.Type == Node.StartEndTag && (node.Tag.Model & ContentModel.Frames) != 0)
					{
						Node.InsertNodeAtEnd(frameset, node);
						continue;
					}
					
					/* discard unexpected tags */
					Report.Warning(lexer, frameset, node, Report.DISCARDING_UNEXPECTED);
				}
				
				Report.Warning(lexer, frameset, node, Report.MISSING_ENDTAG_FOR);
			}
		}
		
		
		public class ParseInlineCheckTable : IParser
		{
			public virtual void Parse(Lexer lexer, Node element, short mode)
			{
				Node node, parent;
				TagTable tt = lexer.Options.tt;
				
				if ((element.Tag.Model & ContentModel.Empty) != 0)
				{
					return;
				}
				
				if (element.Tag == tt.TagA)
				{
					if (element.Attributes == null)
					{
						Report.Warning(lexer, element.Parent, element, Report.DISCARDING_UNEXPECTED);
						Node.DiscardElement(element);
						return;
					}
				}
				
				/*
				ParseInline is used for some block level elements like H1 to H6
				For such elements we need to insert inline emphasis tags currently
				on the inline stack. For Inline elements, we normally push them
				onto the inline stack provided they aren't implicit or OBJECT/APPLET.
				This test is carried out in PushInline and PopInline, see istack.c
				We don't push A or SPAN to replicate current browser behavior
				*/
				if (((element.Tag.Model & ContentModel.Block) != 0) || (element.Tag == tt.TagDt))
				{
					lexer.InlineDup(null);
				}
				else if ((element.Tag.Model & ContentModel.Inline) != 0 && element.Tag != tt.TagA && element.Tag != tt.TagSpan)
				{
					lexer.PushInline(element);
				}
				
				if (element.Tag == tt.TagNobr)
				{
					lexer.badLayout |= Report.USING_NOBR;
				}
				else if (element.Tag == tt.TagFont)
				{
					lexer.badLayout |= Report.USING_FONT;
				}
				
				/* Inline elements may or may not be within a preformatted element */
				if (mode != Lexer.Preformatted)
				{
					mode = Lexer.MixedContent;
				}
				
				while (true)
				{
					node = lexer.GetToken(mode);
					if (node == null)
					{
						break;
					}
					/* end tag for current element */
					if (node.Tag == element.Tag && node.Type == Node.EndTag)
					{
						if ((element.Tag.Model & ContentModel.Inline) != 0 && element.Tag != tt.TagA)
						{
							lexer.PopInline(node);
						}
						
						if (!((mode & Lexer.Preformatted) != 0))
						{
							Node.TrimSpaces(lexer, element);
						}
						/*
						if a font element wraps an anchor and nothing else
						then move the font element inside the anchor since
						otherwise it won't alter the anchor text color
						*/
						if (element.Tag == tt.TagFont && element.Content != null && element.Content == element.Last)
						{
							Node child = element.Content;
							
							if (child.Tag == tt.TagA)
							{
								child.Parent = element.Parent;
								child.Next = element.Next;
								child.Prev = element.Prev;
								
								if (child.Prev != null)
								{
									child.Prev.Next = child;
								}
								else
								{
									child.Parent.Content = child;
								}
								
								if (child.Next != null)
								{
									child.Next.Prev = child;
								}
								else
								{
									child.Parent.Last = child;
								}
								
								element.Next = null;
								element.Prev = null;
								element.Parent = child;
								element.Content = child.Content;
								element.Last = child.Last;
								child.Content = element;
								child.Last = element;
								for (child = element.Content; child != null; child = child.Next)
								{
									child.Parent = element;
								}
							}
						}
						element.Closed = true;
						Node.TrimSpaces(lexer, element);
						Node.TrimEmptyElement(lexer, element);
						return;
					}
					
					/* <u>...<u>  map 2nd <u> to </u> if 1st is explicit */
					/* otherwise emphasis nesting is probably unintentional */
					/* big and small have cumulative effect to leave them alone */
					if (node.Type == Node.StartTag && node.Tag == element.Tag && lexer.IsPushed(node) && !node.Isimplicit && !element.Isimplicit && node.Tag != null && ((node.Tag.Model & ContentModel.Inline) != 0) && node.Tag != tt.TagA && node.Tag != tt.TagFont && node.Tag != tt.TagBig && node.Tag != tt.TagSmall)
					{
						if (element.Content != null && node.Attributes == null)
						{
							Report.Warning(lexer, element, node, Report.COERCE_TO_ENDTAG);
							node.Type = Node.EndTag;
							lexer.UngetToken();
							continue;
						}
						
						Report.Warning(lexer, element, node, Report.NESTED_EMPHASIS);
					}
					
					if (node.Type == Node.TextNode)
					{
						/* only called for 1st child */
						if (element.Content == null && !((mode & Lexer.Preformatted) != 0))
						{
							Node.TrimSpaces(lexer, element);
						}
						
						if (node.Start >= node.End)
						{
							continue;
						}
						
						Node.InsertNodeAtEnd(element, node);
						continue;
					}
					
					/* mixed content model so allow text */
					if (Node.InsertMisc(element, node))
					{
						continue;
					}
					
					/* deal with HTML tags */
					if (node.Tag == tt.TagHtml)
					{
						if (node.Type == Node.StartTag || node.Type == Node.StartEndTag)
						{
							Report.Warning(lexer, element, node, Report.DISCARDING_UNEXPECTED);
							continue;
						}
						
						/* otherwise infer end of inline element */
						lexer.UngetToken();
						if (!((mode & Lexer.Preformatted) != 0))
						{
							Node.TrimSpaces(lexer, element);
						}
						Node.TrimEmptyElement(lexer, element);
						return;
					}
					
					/* within <dt> or <pre> map <p> to <br> */
					if (node.Tag == tt.TagP && node.Type == Node.StartTag && ((mode & Lexer.Preformatted) != 0 || element.Tag == tt.TagDt || element.IsDescendantOf(tt.TagDt)))
					{
						node.Tag = tt.TagBr;
						node.Element = "br";
						Node.TrimSpaces(lexer, element);
						Node.InsertNodeAtEnd(element, node);
						continue;
					}
					
					/* ignore unknown and PARAM tags */
					if (node.Tag == null || node.Tag == tt.TagParam)
					{
						Report.Warning(lexer, element, node, Report.DISCARDING_UNEXPECTED);
						continue;
					}
					
					if (node.Tag == tt.TagBr && node.Type == Node.EndTag)
					{
						node.Type = Node.StartTag;
					}
					
					if (node.Type == Node.EndTag)
					{
						/* coerce </br> to <br> */
						if (node.Tag == tt.TagBr)
						{
							node.Type = Node.StartTag;
						}
						else if (node.Tag == tt.TagP)
						{
							/* coerce unmatched </p> to <br><br> */
							if (!element.IsDescendantOf(tt.TagP))
							{
								Node.CoerceNode(lexer, node, tt.TagBr);
								Node.TrimSpaces(lexer, element);
								Node.InsertNodeAtEnd(element, node);
								node = lexer.InferredTag("br");
								continue;
							}
						}
						else if ((node.Tag.Model & ContentModel.Inline) != 0 && node.Tag != tt.TagA && !((node.Tag.Model & ContentModel.Object) != 0) && (element.Tag.Model & ContentModel.Inline) != 0)
						{
							/* allow any inline end tag to end current element */
							lexer.PopInline(element);
							
							if (element.Tag != tt.TagA)
							{
								if (node.Tag == tt.TagA && node.Tag != element.Tag)
								{
									Report.Warning(lexer, element, node, Report.MISSING_ENDTAG_BEFORE);
									lexer.UngetToken();
								}
								else
								{
									Report.Warning(lexer, element, node, Report.NON_MATCHING_ENDTAG);
								}
								
								if (!((mode & Lexer.Preformatted) != 0))
								{
									Node.TrimSpaces(lexer, element);
								}
								Node.TrimEmptyElement(lexer, element);
								return;
							}
							
							/* if parent is <a> then discard unexpected inline end tag */
							Report.Warning(lexer, element, node, Report.DISCARDING_UNEXPECTED);
							continue;
						}
							/* special case </tr> etc. for stuff moved in front of table */
						else if (lexer.exiled && node.Tag.Model != 0 && (node.Tag.Model & ContentModel.Table) != 0)
						{
							lexer.UngetToken();
							Node.TrimSpaces(lexer, element);
							Node.TrimEmptyElement(lexer, element);
							return;
						}
					}
					
					/* allow any header tag to end current header */
					if ((node.Tag.Model & ContentModel.Heading) != 0 && (element.Tag.Model & ContentModel.Heading) != 0)
					{
						if (node.Tag == element.Tag)
						{
							Report.Warning(lexer, element, node, Report.NON_MATCHING_ENDTAG);
						}
						else
						{
							Report.Warning(lexer, element, node, Report.MISSING_ENDTAG_BEFORE);
							lexer.UngetToken();
						}
						if (!((mode & Lexer.Preformatted) != 0))
						{
							Node.TrimSpaces(lexer, element);
						}
						Node.TrimEmptyElement(lexer, element);
						return;
					}
					
					/*
					an <A> tag to ends any open <A> element
					but <A href=...> is mapped to </A><A href=...>
					*/
					if (node.Tag == tt.TagA && !node.Isimplicit && lexer.IsPushed(node))
					{
						/* coerce <a> to </a> unless it has some attributes */
						if (node.Attributes == null)
						{
							node.Type = Node.EndTag;
							Report.Warning(lexer, element, node, Report.COERCE_TO_ENDTAG);
							lexer.PopInline(node);
							lexer.UngetToken();
							continue;
						}
						
						lexer.UngetToken();
						Report.Warning(lexer, element, node, Report.MISSING_ENDTAG_BEFORE);
						lexer.PopInline(element);
						if (!((mode & Lexer.Preformatted) != 0))
						{
							Node.TrimSpaces(lexer, element);
						}
						Node.TrimEmptyElement(lexer, element);
						return;
					}
					
					if ((element.Tag.Model & ContentModel.Heading) != 0)
					{
						if (node.Tag == tt.TagCenter || node.Tag == tt.TagDiv)
						{
							if (node.Type != Node.StartTag && node.Type != Node.StartEndTag)
							{
								Report.Warning(lexer, element, node, Report.DISCARDING_UNEXPECTED);
								continue;
							}
							
							Report.Warning(lexer, element, node, Report.TAG_NOT_ALLOWED_IN);
							
							/* insert center as parent if heading is empty */
							if (element.Content == null)
							{
								Node.InsertNodeAsParent(element, node);
								continue;
							}
							
							/* split heading and make center parent of 2nd part */
							Node.InsertNodeAfterElement(element, node);
							
							if (!((mode & Lexer.Preformatted) != 0))
							{
								Node.TrimSpaces(lexer, element);
							}
							
							element = lexer.CloneNode(element);
							element.Start = lexer.lexsize;
							element.End = lexer.lexsize;
							Node.InsertNodeAtEnd(node, element);
							continue;
						}
						
						if (node.Tag == tt.TagHr)
						{
							if (node.Type != Node.StartTag && node.Type != Node.StartEndTag)
							{
								Report.Warning(lexer, element, node, Report.DISCARDING_UNEXPECTED);
								continue;
							}
							
							Report.Warning(lexer, element, node, Report.TAG_NOT_ALLOWED_IN);
							
							/* insert hr before heading if heading is empty */
							if (element.Content == null)
							{
								Node.InsertNodeBeforeElement(element, node);
								continue;
							}
							
							/* split heading and insert hr before 2nd part */
							Node.InsertNodeAfterElement(element, node);
							
							if (!((mode & Lexer.Preformatted) != 0))
							{
								Node.TrimSpaces(lexer, element);
							}
							
							element = lexer.CloneNode(element);
							element.Start = lexer.lexsize;
							element.End = lexer.lexsize;
							Node.InsertNodeAfterElement(node, element);
							continue;
						}
					}
					
					if (element.Tag == tt.TagDt)
					{
						if (node.Tag == tt.TagHr)
						{
							Node dd;
							
							if (node.Type != Node.StartTag && node.Type != Node.StartEndTag)
							{
								Report.Warning(lexer, element, node, Report.DISCARDING_UNEXPECTED);
								continue;
							}
							
							Report.Warning(lexer, element, node, Report.TAG_NOT_ALLOWED_IN);
							dd = lexer.InferredTag("dd");
							
							/* insert hr within dd before dt if dt is empty */
							if (element.Content == null)
							{
								Node.InsertNodeBeforeElement(element, dd);
								Node.InsertNodeAtEnd(dd, node);
								continue;
							}
							
							/* split dt and insert hr within dd before 2nd part */
							Node.InsertNodeAfterElement(element, dd);
							Node.InsertNodeAtEnd(dd, node);
							
							if (!((mode & Lexer.Preformatted) != 0))
							{
								Node.TrimSpaces(lexer, element);
							}
							
							element = lexer.CloneNode(element);
							element.Start = lexer.lexsize;
							element.End = lexer.lexsize;
							Node.InsertNodeAfterElement(dd, element);
							continue;
						}
					}
					
					/* 
					if this is the end tag for an ancestor element
					then infer end tag for this element
					*/
					if (node.Type == Node.EndTag)
					{
						for (parent = element.Parent; parent != null; parent = parent.Parent)
						{
							if (node.Tag == parent.Tag)
							{
								if (!((element.Tag.Model & ContentModel.Opt) != 0) && !element.Isimplicit)
								{
									Report.Warning(lexer, element, node, Report.MISSING_ENDTAG_BEFORE);
								}
								
								if (element.Tag == tt.TagA)
								{
									lexer.PopInline(element);
								}

								lexer.UngetToken();
								
								if (!((mode & Lexer.Preformatted) != 0))
								{
									Node.TrimSpaces(lexer, element);
								}
								
								Node.TrimEmptyElement(lexer, element);
								return;
							}
						}
					}
					
					/* block level tags end this element */
					if (!((node.Tag.Model & ContentModel.Inline) != 0))
					{
						if (node.Type != Node.StartTag)
						{
							Report.Warning(lexer, element, node, Report.DISCARDING_UNEXPECTED);
							continue;
						}
						
						if (!((element.Tag.Model & ContentModel.Opt) != 0))
						{
							Report.Warning(lexer, element, node, Report.MISSING_ENDTAG_BEFORE);
						}
						
						if ((node.Tag.Model & ContentModel.Head) != 0 && !((node.Tag.Model & ContentModel.Block) != 0))
						{
							TidyNet.ParserImpl.moveToHead(lexer, element, node);
							continue;
						}
						
						/*
						prevent anchors from propagating into block tags
						except for headings h1 to h6
						*/
						if (element.Tag == tt.TagA)
						{
							if (node.Tag != null && !((node.Tag.Model & ContentModel.Heading) != 0))
							{
								lexer.PopInline(element);
							}
							else if (!(element.Content != null))
							{
								Node.DiscardElement(element);
								lexer.UngetToken();
								return;
							}
						}
						
						lexer.UngetToken();
						
						if (!((mode & Lexer.Preformatted) != 0))
						{
							Node.TrimSpaces(lexer, element);
						}
						
						Node.TrimEmptyElement(lexer, element);
						return;
					}
					
					/* parse inline element */
					if (node.Type == Node.StartTag || node.Type == Node.StartEndTag)
					{
						if (node.Isimplicit)
						{
							Report.Warning(lexer, element, node, Report.INSERTING_TAG);
						}
						
						/* trim white space before <br> */
						if (node.Tag == tt.TagBr)
						{
							Node.TrimSpaces(lexer, element);
						}
						
						Node.InsertNodeAtEnd(element, node);
						TidyNet.ParserImpl.parseTag(lexer, node, mode);
						continue;
					}
					
					/* discard unexpected tags */
					Report.Warning(lexer, element, node, Report.DISCARDING_UNEXPECTED);
				}
				
				if (!((element.Tag.Model & ContentModel.Opt) != 0))
				{
					Report.Warning(lexer, element, node, Report.MISSING_ENDTAG_FOR);
				}
				
				Node.TrimEmptyElement(lexer, element);
			}
		}
		
		
		public class ParseListCheckTable : IParser
		{
			public virtual void Parse(Lexer lexer, Node list, short mode)
			{
				Node node;
				Node parent;
				TagTable tt = lexer.Options.tt;
				
				if ((list.Tag.Model & ContentModel.Empty) != 0)
				{
					return;
				}
				
				lexer.insert = - 1; /* defer implicit inline start tags */
				
				while (true)
				{
					node = lexer.GetToken(Lexer.IgnoreWhitespace);
					if (node == null)
					{
						break;
					}
					
					if (node.Tag == list.Tag && node.Type == Node.EndTag)
					{
						if ((list.Tag.Model & ContentModel.Obsolete) != 0)
						{
							Node.CoerceNode(lexer, list, tt.TagUl);
						}
						
						list.Closed = true;
						Node.TrimEmptyElement(lexer, list);
						return;
					}
					
					/* deal with comments etc. */
					if (Node.InsertMisc(list, node))
					{
						continue;
					}
					
					if (node.Type != Node.TextNode && node.Tag == null)
					{
						Report.Warning(lexer, list, node, Report.DISCARDING_UNEXPECTED);
						continue;
					}
					
					/* 
					if this is the end tag for an ancestor element
					then infer end tag for this element
					*/
					if (node.Type == Node.EndTag)
					{
						if (node.Tag == tt.TagForm)
						{
							lexer.badForm = 1;
							Report.Warning(lexer, list, node, Report.DISCARDING_UNEXPECTED);
							continue;
						}
						
						if (node.Tag != null && (node.Tag.Model & ContentModel.Inline) != 0)
						{
							Report.Warning(lexer, list, node, Report.DISCARDING_UNEXPECTED);
							lexer.PopInline(node);
							continue;
						}
						
						for (parent = list.Parent; parent != null; parent = parent.Parent)
						{
							if (node.Tag == parent.Tag)
							{
								Report.Warning(lexer, list, node, Report.MISSING_ENDTAG_BEFORE);
								lexer.UngetToken();
								
								if ((list.Tag.Model & ContentModel.Obsolete) != 0)
								{
									Node.CoerceNode(lexer, list, tt.TagUl);
								}
								
								Node.TrimEmptyElement(lexer, list);
								return;
							}
						}
						
						Report.Warning(lexer, list, node, Report.DISCARDING_UNEXPECTED);
						continue;
					}
					
					if (node.Tag != tt.TagLi)
					{
						lexer.UngetToken();
						
						if (node.Tag != null && (node.Tag.Model & ContentModel.Block) != 0 && lexer.excludeBlocks)
						{
							Report.Warning(lexer, list, node, Report.MISSING_ENDTAG_BEFORE);
							Node.TrimEmptyElement(lexer, list);
							return;
						}
						
						node = lexer.InferredTag("li");
						node.AddAttribute("style", "list-style: none");
						Report.Warning(lexer, list, node, Report.MISSING_STARTTAG);
					}
					
					/* node should be <LI> */
					Node.InsertNodeAtEnd(list, node);
					TidyNet.ParserImpl.parseTag(lexer, node, Lexer.IgnoreWhitespace);
				}
				
				if ((list.Tag.Model & ContentModel.Obsolete) != 0)
				{
					Node.CoerceNode(lexer, list, tt.TagUl);
				}
				
				Report.Warning(lexer, list, node, Report.MISSING_ENDTAG_FOR);
				Node.TrimEmptyElement(lexer, list);
			}
		}
		
		
		public class ParseDefListCheckTable : IParser
		{
			public virtual void Parse(Lexer lexer, Node list, short mode)
			{
				Node node, parent;
				TagTable tt = lexer.Options.tt;
				
				if ((list.Tag.Model & ContentModel.Empty) != 0)
					return;
				
				lexer.insert = - 1; /* defer implicit inline start tags */
				
				while (true)
				{
					node = lexer.GetToken(Lexer.IgnoreWhitespace);
					if (node == null)
					{
						break;
					}
					if (node.Tag == list.Tag && node.Type == Node.EndTag)
					{
						list.Closed = true;
						Node.TrimEmptyElement(lexer, list);
						return;
					}
					
					/* deal with comments etc. */
					if (Node.InsertMisc(list, node))
					{
						continue;
					}
					
					if (node.Type == Node.TextNode)
					{
						lexer.UngetToken();
						node = lexer.InferredTag("dt");
						Report.Warning(lexer, list, node, Report.MISSING_STARTTAG);
					}
					
					if (node.Tag == null)
					{
						Report.Warning(lexer, list, node, Report.DISCARDING_UNEXPECTED);
						continue;
					}
					
					/* 
					if this is the end tag for an ancestor element
					then infer end tag for this element
					*/
					if (node.Type == Node.EndTag)
					{
						if (node.Tag == tt.TagForm)
						{
							lexer.badForm = 1;
							Report.Warning(lexer, list, node, Report.DISCARDING_UNEXPECTED);
							continue;
						}
						
						for (parent = list.Parent; parent != null; parent = parent.Parent)
						{
							if (node.Tag == parent.Tag)
							{
								Report.Warning(lexer, list, node, Report.MISSING_ENDTAG_BEFORE);
								
								lexer.UngetToken();
								Node.TrimEmptyElement(lexer, list);
								return;
							}
						}
					}
					
					/* center in a dt or a dl breaks the dl list in two */
					if (node.Tag == tt.TagCenter)
					{
						if (list.Content != null)
						{
							Node.InsertNodeAfterElement(list, node);
						}
						else
						{
							/* trim empty dl list */
							Node.InsertNodeBeforeElement(list, node);
							Node.DiscardElement(list);
						}
						
						/* and parse contents of center */
						TidyNet.ParserImpl.parseTag(lexer, node, mode);
						
						/* now create a new dl element */
						list = lexer.InferredTag("dl");
						Node.InsertNodeAfterElement(node, list);
						continue;
					}
					
					if (!(node.Tag == tt.TagDt || node.Tag == tt.TagDd))
					{
						lexer.UngetToken();
						
						if (!((node.Tag.Model & (ContentModel.Block | ContentModel.Inline)) != 0))
						{
							Report.Warning(lexer, list, node, Report.TAG_NOT_ALLOWED_IN);
							Node.TrimEmptyElement(lexer, list);
							return;
						}
						
						/* if DD appeared directly in BODY then exclude blocks */
						if (!((node.Tag.Model & ContentModel.Inline) != 0) && lexer.excludeBlocks)
						{
							Node.TrimEmptyElement(lexer, list);
							return;
						}
						
						node = lexer.InferredTag("dd");
						Report.Warning(lexer, list, node, Report.MISSING_STARTTAG);
					}
					
					if (node.Type == Node.EndTag)
					{
						Report.Warning(lexer, list, node, Report.DISCARDING_UNEXPECTED);
						continue;
					}
					
					/* node should be <DT> or <DD>*/
					Node.InsertNodeAtEnd(list, node);
					TidyNet.ParserImpl.parseTag(lexer, node, Lexer.IgnoreWhitespace);
				}
				
				Report.Warning(lexer, list, node, Report.MISSING_ENDTAG_FOR);
				Node.TrimEmptyElement(lexer, list);
			}
		}
		
		
		public class ParsePreCheckTable : IParser
		{
			public virtual void Parse(Lexer lexer, Node pre, short mode)
			{
				Node node, parent;
				TagTable tt = lexer.Options.tt;
				
				if ((pre.Tag.Model & ContentModel.Empty) != 0)
				{
					return;
				}
				
				if ((pre.Tag.Model & ContentModel.Obsolete) != 0)
				{
					Node.CoerceNode(lexer, pre, tt.TagPre);
				}
				
				lexer.InlineDup(null); /* tell lexer to insert inlines if needed */
				
				while (true)
				{
					node = lexer.GetToken(Lexer.Preformatted);
					if (node == null)
					{
						break;
					}
					if (node.Tag == pre.Tag && node.Type == Node.EndTag)
					{
						Node.TrimSpaces(lexer, pre);
						pre.Closed = true;
						Node.TrimEmptyElement(lexer, pre);
						return;
					}
					
					if (node.Tag == tt.TagHtml)
					{
						if (node.Type == Node.StartTag || node.Type == Node.StartEndTag)
						{
							Report.Warning(lexer, pre, node, Report.DISCARDING_UNEXPECTED);
						}
						
						continue;
					}
					
					if (node.Type == Node.TextNode)
					{
						/* if first check for inital newline */
						if (pre.Content == null)
						{
							if (node.Textarray[node.Start] == (sbyte) '\n')
							{
								++node.Start;
							}
							
							if (node.Start >= node.End)
							{
								continue;
							}
						}
						
						Node.InsertNodeAtEnd(pre, node);
						continue;
					}
					
					/* deal with comments etc. */
					if (Node.InsertMisc(pre, node))
					{
						continue;
					}
					
					/* discard unknown  and PARAM tags */
					if (node.Tag == null || node.Tag == tt.TagParam)
					{
						Report.Warning(lexer, pre, node, Report.DISCARDING_UNEXPECTED);
						continue;
					}
					
					if (node.Tag == tt.TagP)
					{
						if (node.Type == Node.StartTag)
						{
							Report.Warning(lexer, pre, node, Report.USING_BR_INPLACE_OF);
							
							/* trim white space before <p> in <pre>*/
							Node.TrimSpaces(lexer, pre);
							
							/* coerce both <p> and </p> to <br> */
							Node.CoerceNode(lexer, node, tt.TagBr);
							Node.InsertNodeAtEnd(pre, node);
						}
						else
						{
							Report.Warning(lexer, pre, node, Report.DISCARDING_UNEXPECTED);
						}
						continue;
					}
					
					if ((node.Tag.Model & ContentModel.Head) != 0 && !((node.Tag.Model & ContentModel.Block) != 0))
					{
						TidyNet.ParserImpl.moveToHead(lexer, pre, node);
						continue;
					}
					
					/* 
					if this is the end tag for an ancestor element
					then infer end tag for this element
					*/
					if (node.Type == Node.EndTag)
					{
						if (node.Tag == tt.TagForm)
						{
							lexer.badForm = 1;
							Report.Warning(lexer, pre, node, Report.DISCARDING_UNEXPECTED);
							continue;
						}
						
						for (parent = pre.Parent; parent != null; parent = parent.Parent)
						{
							if (node.Tag == parent.Tag)
							{
								Report.Warning(lexer, pre, node, Report.MISSING_ENDTAG_BEFORE);
								
								lexer.UngetToken();
								Node.TrimSpaces(lexer, pre);
								Node.TrimEmptyElement(lexer, pre);
								return;
							}
						}
					}
					
					/* what about head content, HEAD, BODY tags etc? */
					if (!((node.Tag.Model & ContentModel.Inline) != 0))
					{
						if (node.Type != Node.StartTag)
						{
							Report.Warning(lexer, pre, node, Report.DISCARDING_UNEXPECTED);
							continue;
						}
						
						Report.Warning(lexer, pre, node, Report.MISSING_ENDTAG_BEFORE);
						lexer.excludeBlocks = true;
						
						/* check if we need to infer a container */
						if ((node.Tag.Model & ContentModel.List) != 0)
						{
							lexer.UngetToken();
							node = lexer.InferredTag("ul");
							Node.AddClass(node, "noindent");
						}
						else if ((node.Tag.Model & ContentModel.Deflist) != 0)
						{
							lexer.UngetToken();
							node = lexer.InferredTag("dl");
						}
						else if ((node.Tag.Model & ContentModel.Table) != 0)
						{
							lexer.UngetToken();
							node = lexer.InferredTag("table");
						}
						
						Node.InsertNodeAfterElement(pre, node);
						pre = lexer.InferredTag("pre");
						Node.InsertNodeAfterElement(node, pre);
						TidyNet.ParserImpl.parseTag(lexer, node, Lexer.IgnoreWhitespace);
						lexer.excludeBlocks = false;
						continue;
					}
					/*
					if (!((node.Tag.Model & ContentModel.INLINE) != 0))
					{
					Report.Warning(lexer, pre, node, Report.MISSING_ENDTAG_BEFORE);
					lexer.UngetToken();
					return;
					}
					*/
					if (node.Type == Node.StartTag || node.Type == Node.StartEndTag)
					{
						/* trim white space before <br> */
						if (node.Tag == tt.TagBr)
						{
							Node.TrimSpaces(lexer, pre);
						}
						
						Node.InsertNodeAtEnd(pre, node);
						TidyNet.ParserImpl.parseTag(lexer, node, Lexer.Preformatted);
						continue;
					}
					
					/* discard unexpected tags */
					Report.Warning(lexer, pre, node, Report.DISCARDING_UNEXPECTED);
				}
				
				Report.Warning(lexer, pre, node, Report.MISSING_ENDTAG_FOR);
				Node.TrimEmptyElement(lexer, pre);
			}
		}
		
		
		public class ParseBlockCheckTable : IParser
		{
			/*
			element is node created by the lexer
			upon seeing the start tag, or by the
			parser when the start tag is inferred
			*/
			public virtual void Parse(Lexer lexer, Node element, short mode)
			{
				Node node, parent;
				bool checkstack;
				int istackbase = 0;
				TagTable tt = lexer.Options.tt;
				
				checkstack = true;
				
				if ((element.Tag.Model & ContentModel.Empty) != 0)
				{
					return;
				}
				
				if (element.Tag == tt.TagForm && element.IsDescendantOf(tt.TagForm))
				{
					Report.Warning(lexer, element, null, Report.ILLEGAL_NESTING);
				}
				
				/*
				InlineDup() asks the lexer to insert inline emphasis tags
				currently pushed on the istack, but take care to avoid
				propagating inline emphasis inside OBJECT or APPLET.
				For these elements a fresh inline stack context is created
				and disposed of upon reaching the end of the element.
				They thus behave like table cells in this respect.
				*/
				if ((element.Tag.Model & ContentModel.Object) != 0)
				{
					istackbase = lexer.istackbase;
					lexer.istackbase = lexer.istack.Count;
				}
				
				if (!((element.Tag.Model & ContentModel.Mixed) != 0))
				{
					lexer.InlineDup(null);
				}
				
				mode = Lexer.IgnoreWhitespace;
				
				while (true)
				{
					node = lexer.GetToken(mode);
					if (node == null)
					{
						break;
					}

					/* end tag for this element */
					if (node.Type == Node.EndTag && node.Tag != null && (node.Tag == element.Tag || element.Was == node.Tag))
					{
						if ((element.Tag.Model & ContentModel.Object) != 0)
						{
							/* pop inline stack */
							while (lexer.istack.Count > lexer.istackbase)
							{
								lexer.PopInline(null);
							}
							lexer.istackbase = istackbase;
						}
						
						element.Closed = true;
						Node.TrimSpaces(lexer, element);
						Node.TrimEmptyElement(lexer, element);
						return;
					}
					
					if (node.Tag == tt.TagHtml || node.Tag == tt.TagHead || node.Tag == tt.TagBody)
					{
						if (node.Type == Node.StartTag || node.Type == Node.StartEndTag)
						{
							Report.Warning(lexer, element, node, Report.DISCARDING_UNEXPECTED);
						}
						
						continue;
					}
					
					if (node.Type == Node.EndTag)
					{
						if (node.Tag == null)
						{
							Report.Warning(lexer, element, node, Report.DISCARDING_UNEXPECTED);
							
							continue;
						}
						else if (node.Tag == tt.TagBr)
						{
							node.Type = Node.StartTag;
						}
						else if (node.Tag == tt.TagP)
						{
							Node.CoerceNode(lexer, node, tt.TagBr);
							Node.InsertNodeAtEnd(element, node);
							node = lexer.InferredTag("br");
						}
						else
						{
							/* 
							if this is the end tag for an ancestor element
							then infer end tag for this element
							*/
							for (parent = element.Parent; parent != null; parent = parent.Parent)
							{
								if (node.Tag == parent.Tag)
								{
									if (!((element.Tag.Model & ContentModel.Opt) != 0))
									{
										Report.Warning(lexer, element, node, Report.MISSING_ENDTAG_BEFORE);
									}
									
									lexer.UngetToken();
									
									if ((element.Tag.Model & ContentModel.Object) != 0)
									{
										/* pop inline stack */
										while (lexer.istack.Count > lexer.istackbase)
										{
											lexer.PopInline(null);
										}
										lexer.istackbase = istackbase;
									}
									
									Node.TrimSpaces(lexer, element);
									Node.TrimEmptyElement(lexer, element);
									return;
								}
							}
							/* special case </tr> etc. for stuff moved in front of table */
							if (lexer.exiled && node.Tag.Model != 0 && (node.Tag.Model & ContentModel.Table) != 0)
							{
								lexer.UngetToken();
								Node.TrimSpaces(lexer, element);
								Node.TrimEmptyElement(lexer, element);
								return;
							}
						}
					}
					
					/* mixed content model permits text */
					if (node.Type == Node.TextNode)
					{
						bool iswhitenode = false;
						
						if (node.Type == Node.TextNode && node.End <= node.Start + 1 && lexer.lexbuf[node.Start] == (sbyte) ' ')
						{
							iswhitenode = true;
						}
						
						if (lexer.Options.EncloseBlockText && !iswhitenode)
						{
							lexer.UngetToken();
							node = lexer.InferredTag("p");
							Node.InsertNodeAtEnd(element, node);
							TidyNet.ParserImpl.parseTag(lexer, node, Lexer.MixedContent);
							continue;
						}
						
						if (checkstack)
						{
							checkstack = false;
							
							if (!((element.Tag.Model & ContentModel.Mixed) != 0))
							{
								if (lexer.InlineDup(node) > 0)
								{
									continue;
								}
							}
						}
						
						Node.InsertNodeAtEnd(element, node);
						mode = Lexer.MixedContent;
						/*
						HTML4 strict doesn't allow mixed content for
						elements with %block; as their content model
						*/
						lexer.versions &= ~ HtmlVersion.Html40Strict;
						continue;
					}
					
					if (Node.InsertMisc(element, node))
					{
						continue;
					}
					
					/* allow PARAM elements? */
					if (node.Tag == tt.TagParam)
					{
						if (((element.Tag.Model & ContentModel.Param) != 0) && (node.Type == Node.StartTag || node.Type == Node.StartEndTag))
						{
							Node.InsertNodeAtEnd(element, node);
							continue;
						}
						
						/* otherwise discard it */
						Report.Warning(lexer, element, node, Report.DISCARDING_UNEXPECTED);
						continue;
					}
					
					/* allow AREA elements? */
					if (node.Tag == tt.TagArea)
					{
						if ((element.Tag == tt.TagMap) && (node.Type == Node.StartTag || node.Type == Node.StartEndTag))
						{
							Node.InsertNodeAtEnd(element, node);
							continue;
						}
						
						/* otherwise discard it */
						Report.Warning(lexer, element, node, Report.DISCARDING_UNEXPECTED);
						continue;
					}
					
					/* ignore unknown start/end tags */
					if (node.Tag == null)
					{
						Report.Warning(lexer, element, node, Report.DISCARDING_UNEXPECTED);
						continue;
					}
					
					/*
					Allow ContentModel.INLINE elements here.
					
					Allow ContentModel.BLOCK elements here unless
					lexer.excludeBlocks is yes.
					
					LI and DD are special cased.
					
					Otherwise infer end tag for this element.
					*/
					
					if (!((node.Tag.Model & ContentModel.Inline) != 0))
					{
						if (node.Type != Node.StartTag && node.Type != Node.StartEndTag)
						{
							Report.Warning(lexer, element, node, Report.DISCARDING_UNEXPECTED);
							continue;
						}
						
						if (element.Tag == tt.TagTd || element.Tag == tt.TagTh)
						{
							/* if parent is a table cell, avoid inferring the end of the cell */
							
							if ((node.Tag.Model & ContentModel.Head) != 0)
							{
								TidyNet.ParserImpl.moveToHead(lexer, element, node);
								continue;
							}
							
							if ((node.Tag.Model & ContentModel.List) != 0)
							{
								lexer.UngetToken();
								node = lexer.InferredTag("ul");
								Node.AddClass(node, "noindent");
								lexer.excludeBlocks = true;
							}
							else if ((node.Tag.Model & ContentModel.Deflist) != 0)
							{
								lexer.UngetToken();
								node = lexer.InferredTag("dl");
								lexer.excludeBlocks = true;
							}
							
							/* infer end of current table cell */
							if (!((node.Tag.Model & ContentModel.Block) != 0))
							{
								lexer.UngetToken();
								Node.TrimSpaces(lexer, element);
								Node.TrimEmptyElement(lexer, element);
								return;
							}
						}
						else if ((node.Tag.Model & ContentModel.Block) != 0)
						{
							if (lexer.excludeBlocks)
							{
								if (!((element.Tag.Model & ContentModel.Opt) != 0))
									Report.Warning(lexer, element, node, Report.MISSING_ENDTAG_BEFORE);
								
								lexer.UngetToken();
								
								if ((element.Tag.Model & ContentModel.Object) != 0)
									lexer.istackbase = istackbase;
								
								Node.TrimSpaces(lexer, element);
								Node.TrimEmptyElement(lexer, element);
								return;
							}
						}
						/* things like list items */
						else
						{
							if (!((element.Tag.Model & ContentModel.Opt) != 0) && !element.Isimplicit)
								Report.Warning(lexer, element, node, Report.MISSING_ENDTAG_BEFORE);
							
							if ((node.Tag.Model & ContentModel.Head) != 0)
							{
								TidyNet.ParserImpl.moveToHead(lexer, element, node);
								continue;
							}
							
							lexer.UngetToken();
							
							if ((node.Tag.Model & ContentModel.List) != 0)
							{
								if (element.Parent != null && element.Parent.Tag != null && element.Parent.Tag.Parser == TidyNet.ParserImpl.ParseList)
								{
									Node.TrimSpaces(lexer, element);
									Node.TrimEmptyElement(lexer, element);
									return;
								}
								
								node = lexer.InferredTag("ul");
								Node.AddClass(node, "noindent");
							}
							else if ((node.Tag.Model & ContentModel.Deflist) != 0)
							{
								if (element.Parent.Tag == tt.TagDl)
								{
									Node.TrimSpaces(lexer, element);
									Node.TrimEmptyElement(lexer, element);
									return;
								}
								
								node = lexer.InferredTag("dl");
							}
							else if ((node.Tag.Model & ContentModel.Table) != 0 || (node.Tag.Model & ContentModel.Row) != 0)
							{
								node = lexer.InferredTag("table");
							}
							else if ((element.Tag.Model & ContentModel.Object) != 0)
							{
								/* pop inline stack */
								while (lexer.istack.Count > lexer.istackbase)
								{
									lexer.PopInline(null);
								}
								lexer.istackbase = istackbase;
								Node.TrimSpaces(lexer, element);
								Node.TrimEmptyElement(lexer, element);
								return;
							}
							else
							{
								Node.TrimSpaces(lexer, element);
								Node.TrimEmptyElement(lexer, element);
								return;
							}
						}
					}
					
					/* parse known element */
					if (node.Type == Node.StartTag || node.Type == Node.StartEndTag)
					{
						if ((node.Tag.Model & ContentModel.Inline) != 0)
						{
							if (checkstack && !node.Isimplicit)
							{
								checkstack = false;
								
								if (lexer.InlineDup(node) > 0)
									continue;
							}
							
							mode = Lexer.MixedContent;
						}
						else
						{
							checkstack = true;
							mode = Lexer.IgnoreWhitespace;
						}
						
						/* trim white space before <br> */
						if (node.Tag == tt.TagBr)
						{
							Node.TrimSpaces(lexer, element);
						}
						
						Node.InsertNodeAtEnd(element, node);
						
						if (node.Isimplicit)
						{
							Report.Warning(lexer, element, node, Report.INSERTING_TAG);
						}
						
						TidyNet.ParserImpl.parseTag(lexer, node, Lexer.IgnoreWhitespace);
						continue;
					}
					
					/* discard unexpected tags */
					if (node.Type == Node.EndTag)
						lexer.PopInline(node);
					/* if inline end tag */
					
					Report.Warning(lexer, element, node, Report.DISCARDING_UNEXPECTED);
				}
				
				if (!((element.Tag.Model & ContentModel.Opt) != 0))
				{
					Report.Warning(lexer, element, node, Report.MISSING_ENDTAG_FOR);
				}
				
				if ((element.Tag.Model & ContentModel.Object) != 0)
				{
					/* pop inline stack */
					while (lexer.istack.Count > lexer.istackbase)
					{
						lexer.PopInline(null);
					}
					lexer.istackbase = istackbase;
				}
				
				Node.TrimSpaces(lexer, element);
				Node.TrimEmptyElement(lexer, element);
			}
		}
		
		
		public class ParseTableTagCheckTable : IParser
		{
			public virtual void Parse(Lexer lexer, Node table, short mode)
			{
				Node node, parent;
				int istackbase;
				TagTable tt = lexer.Options.tt;
				
				lexer.DeferDup();
				istackbase = lexer.istackbase;
				lexer.istackbase = lexer.istack.Count;
				
				while (true)
				{
					node = lexer.GetToken(Lexer.IgnoreWhitespace);
					if (node == null)
						break;
					if (node.Tag == table.Tag && node.Type == Node.EndTag)
					{
						lexer.istackbase = istackbase;
						table.Closed = true;
						Node.TrimEmptyElement(lexer, table);
						return;
					}
					
					/* deal with comments etc. */
					if (Node.InsertMisc(table, node))
						continue;
					
					/* discard unknown tags */
					if (node.Tag == null && node.Type != Node.TextNode)
					{
						Report.Warning(lexer, table, node, Report.DISCARDING_UNEXPECTED);
						continue;
					}
					
					/* if TD or TH or text or inline or block then infer <TR> */
					
					if (node.Type != Node.EndTag)
					{
						if (node.Tag == tt.TagTd || node.Tag == tt.TagTh || node.Tag == tt.tagTable)
						{
							lexer.UngetToken();
							node = lexer.InferredTag("tr");
							Report.Warning(lexer, table, node, Report.MISSING_STARTTAG);
						}
						else if (node.Type == Node.TextNode || (node.Tag.Model & (ContentModel.Block | ContentModel.Inline)) != 0)
						{
							Node.InsertNodeBeforeElement(table, node);
							Report.Warning(lexer, table, node, Report.TAG_NOT_ALLOWED_IN);
							lexer.exiled = true;
							
							/* AQ: TODO
							Line 2040 of parser.c (13 Jan 2000) reads as follows:
							if (!node->type == TextNode)
							This will always evaluate to false.
							This has been reported to Dave Raggett <dsr@w3.org>
							*/
							//Should be?: if (!(node.Type == Node.TextNode))
//							if (false)
//								TidyNet.ParserImpl.parseTag(lexer, node, Lexer.IgnoreWhitespace);
							
							lexer.exiled = false;
							continue;
						}
						else if ((node.Tag.Model & ContentModel.Head) != 0)
						{
							TidyNet.ParserImpl.moveToHead(lexer, table, node);
							continue;
						}
					}
					
					/* 
					if this is the end tag for an ancestor element
					then infer end tag for this element
					*/
					if (node.Type == Node.EndTag)
					{
						if (node.Tag == tt.TagForm)
						{
							lexer.badForm = 1;
							Report.Warning(lexer, table, node, Report.DISCARDING_UNEXPECTED);
							continue;
						}
						
						if (node.Tag != null && (node.Tag.Model & (ContentModel.Table | ContentModel.Row)) != 0)
						{
							Report.Warning(lexer, table, node, Report.DISCARDING_UNEXPECTED);
							continue;
						}
						
						for (parent = table.Parent; parent != null; parent = parent.Parent)
						{
							if (node.Tag == parent.Tag)
							{
								Report.Warning(lexer, table, node, Report.MISSING_ENDTAG_BEFORE);
								lexer.UngetToken();
								lexer.istackbase = istackbase;
								Node.TrimEmptyElement(lexer, table);
								return;
							}
						}
					}
					
					if (!((node.Tag.Model & ContentModel.Table) != 0))
					{
						lexer.UngetToken();
						Report.Warning(lexer, table, node, Report.TAG_NOT_ALLOWED_IN);
						lexer.istackbase = istackbase;
						Node.TrimEmptyElement(lexer, table);
						return;
					}
					
					if (node.Type == Node.StartTag || node.Type == Node.StartEndTag)
					{
						Node.InsertNodeAtEnd(table, node); ;
						TidyNet.ParserImpl.parseTag(lexer, node, Lexer.IgnoreWhitespace);
						continue;
					}
					
					/* discard unexpected text nodes and end tags */
					Report.Warning(lexer, table, node, Report.DISCARDING_UNEXPECTED);
				}
				
				Report.Warning(lexer, table, node, Report.MISSING_ENDTAG_FOR);
				Node.TrimEmptyElement(lexer, table);
				lexer.istackbase = istackbase;
			}
		}
		
		
		public class ParseColGroupCheckTable : IParser
		{
			public virtual void Parse(Lexer lexer, Node colgroup, short mode)
			{
				Node node, parent;
				TagTable tt = lexer.Options.tt;
				
				if ((colgroup.Tag.Model & ContentModel.Empty) != 0)
					return;
				
				while (true)
				{
					node = lexer.GetToken(Lexer.IgnoreWhitespace);
					if (node == null)
						break;
					if (node.Tag == colgroup.Tag && node.Type == Node.EndTag)
					{
						colgroup.Closed = true;
						return;
					}
					
					/* 
					if this is the end tag for an ancestor element
					then infer end tag for this element
					*/
					if (node.Type == Node.EndTag)
					{
						if (node.Tag == tt.TagForm)
						{
							lexer.badForm = 1;
							Report.Warning(lexer, colgroup, node, Report.DISCARDING_UNEXPECTED);
							continue;
						}
						
						for (parent = colgroup.Parent; parent != null; parent = parent.Parent)
						{
							
							if (node.Tag == parent.Tag)
							{
								lexer.UngetToken();
								return;
							}
						}
					}
					
					if (node.Type == Node.TextNode)
					{
						lexer.UngetToken();
						return;
					}
					
					/* deal with comments etc. */
					if (Node.InsertMisc(colgroup, node))
						continue;
					
					/* discard unknown tags */
					if (node.Tag == null)
					{
						Report.Warning(lexer, colgroup, node, Report.DISCARDING_UNEXPECTED);
						continue;
					}
					
					if (node.Tag != tt.TagCol)
					{
						lexer.UngetToken();
						return;
					}
					
					if (node.Type == Node.EndTag)
					{
						Report.Warning(lexer, colgroup, node, Report.DISCARDING_UNEXPECTED);
						continue;
					}
					
					/* node should be <COL> */
					Node.InsertNodeAtEnd(colgroup, node);
					TidyNet.ParserImpl.parseTag(lexer, node, Lexer.IgnoreWhitespace);
				}
			}
		}
		
		
		public class ParseRowGroupCheckTable : IParser
		{
			public virtual void Parse(Lexer lexer, Node rowgroup, short mode)
			{
				Node node, parent;
				TagTable tt = lexer.Options.tt;
				
				if ((rowgroup.Tag.Model & ContentModel.Empty) != 0)
					return;
				
				while (true)
				{
					node = lexer.GetToken(Lexer.IgnoreWhitespace);
					if (node == null)
						break;
					if (node.Tag == rowgroup.Tag)
					{
						if (node.Type == Node.EndTag)
						{
							rowgroup.Closed = true;
							Node.TrimEmptyElement(lexer, rowgroup);
							return;
						}
						
						lexer.UngetToken();
						return;
					}
					
					/* if </table> infer end tag */
					if (node.Tag == tt.tagTable && node.Type == Node.EndTag)
					{
						lexer.UngetToken();
						Node.TrimEmptyElement(lexer, rowgroup);
						return;
					}
					
					/* deal with comments etc. */
					if (Node.InsertMisc(rowgroup, node))
						continue;
					
					/* discard unknown tags */
					if (node.Tag == null && node.Type != Node.TextNode)
					{
						Report.Warning(lexer, rowgroup, node, Report.DISCARDING_UNEXPECTED);
						continue;
					}
					
					/*
					if TD or TH then infer <TR>
					if text or inline or block move before table
					if head content move to head
					*/
					
					if (node.Type != Node.EndTag)
					{
						if (node.Tag == tt.TagTd || node.Tag == tt.TagTh)
						{
							lexer.UngetToken();
							node = lexer.InferredTag("tr");
							Report.Warning(lexer, rowgroup, node, Report.MISSING_STARTTAG);
						}
						else if (node.Type == Node.TextNode || (node.Tag.Model & (ContentModel.Block | ContentModel.Inline)) != 0)
						{
							Node.MoveBeforeTable(rowgroup, node, tt);
							Report.Warning(lexer, rowgroup, node, Report.TAG_NOT_ALLOWED_IN);
							lexer.exiled = true;
							
							if (node.Type != Node.TextNode)
								TidyNet.ParserImpl.parseTag(lexer, node, Lexer.IgnoreWhitespace);
							
							lexer.exiled = false;
							continue;
						}
						else if ((node.Tag.Model & ContentModel.Head) != 0)
						{
							Report.Warning(lexer, rowgroup, node, Report.TAG_NOT_ALLOWED_IN);
							TidyNet.ParserImpl.moveToHead(lexer, rowgroup, node);
							continue;
						}
					}
					
					/* 
					if this is the end tag for ancestor element
					then infer end tag for this element
					*/
					if (node.Type == Node.EndTag)
					{
						if (node.Tag == tt.TagForm)
						{
							lexer.badForm = 1;
							Report.Warning(lexer, rowgroup, node, Report.DISCARDING_UNEXPECTED);
							continue;
						}
						
						if (node.Tag == tt.TagTr || node.Tag == tt.TagTd || node.Tag == tt.TagTh)
						{
							Report.Warning(lexer, rowgroup, node, Report.DISCARDING_UNEXPECTED);
							continue;
						}
						
						for (parent = rowgroup.Parent; parent != null; parent = parent.Parent)
						{
							if (node.Tag == parent.Tag)
							{
								lexer.UngetToken();
								Node.TrimEmptyElement(lexer, rowgroup);
								return;
							}
						}
					}
					
					/*
					if THEAD, TFOOT or TBODY then implied end tag
					
					*/
					if ((node.Tag.Model & ContentModel.Rowgrp) != 0)
					{
						if (node.Type != Node.EndTag)
							lexer.UngetToken();
						
						Node.TrimEmptyElement(lexer, rowgroup);
						return;
					}
					
					if (node.Type == Node.EndTag)
					{
						Report.Warning(lexer, rowgroup, node, Report.DISCARDING_UNEXPECTED);
						continue;
					}
					
					if (!(node.Tag == tt.TagTr))
					{
						node = lexer.InferredTag("tr");
						Report.Warning(lexer, rowgroup, node, Report.MISSING_STARTTAG);
						lexer.UngetToken();
					}
					
					/* node should be <TR> */
					Node.InsertNodeAtEnd(rowgroup, node);
					TidyNet.ParserImpl.parseTag(lexer, node, Lexer.IgnoreWhitespace);
				}
				
				Node.TrimEmptyElement(lexer, rowgroup);
			}
		}
		
		
		public class ParseRowCheckTable : IParser
		{
			public virtual void Parse(Lexer lexer, Node row, short mode)
			{
				Node node, parent;
				bool exclude_state;
				TagTable tt = lexer.Options.tt;
				
				if ((row.Tag.Model & ContentModel.Empty) != 0)
					return;
				
				while (true)
				{
					node = lexer.GetToken(Lexer.IgnoreWhitespace);
					if (node == null)
						break;
					if (node.Tag == row.Tag)
					{
						if (node.Type == Node.EndTag)
						{
							row.Closed = true;
							Node.FixEmptyRow(lexer, row);
							return;
						}
						
						lexer.UngetToken();
						Node.FixEmptyRow(lexer, row);
						return;
					}
					
					/* 
					if this is the end tag for an ancestor element
					then infer end tag for this element
					*/
					if (node.Type == Node.EndTag)
					{
						if (node.Tag == tt.TagForm)
						{
							lexer.badForm = 1;
							Report.Warning(lexer, row, node, Report.DISCARDING_UNEXPECTED);
							continue;
						}
						
						if (node.Tag == tt.TagTd || node.Tag == tt.TagTh)
						{
							Report.Warning(lexer, row, node, Report.DISCARDING_UNEXPECTED);
							continue;
						}
						
						for (parent = row.Parent; parent != null; parent = parent.Parent)
						{
							if (node.Tag == parent.Tag)
							{
								lexer.UngetToken();
								Node.TrimEmptyElement(lexer, row);
								return;
							}
						}
					}
					
					/* deal with comments etc. */
					if (Node.InsertMisc(row, node))
						continue;
					
					/* discard unknown tags */
					if (node.Tag == null && node.Type != Node.TextNode)
					{
						Report.Warning(lexer, row, node, Report.DISCARDING_UNEXPECTED);
						continue;
					}
					
					/* discard unexpected <table> element */
					if (node.Tag == tt.tagTable)
					{
						Report.Warning(lexer, row, node, Report.DISCARDING_UNEXPECTED);
						continue;
					}
					
					/* THEAD, TFOOT or TBODY */
					if (node.Tag != null && (node.Tag.Model & ContentModel.Rowgrp) != 0)
					{
						lexer.UngetToken();
						Node.TrimEmptyElement(lexer, row);
						return;
					}
					
					if (node.Type == Node.EndTag)
					{
						Report.Warning(lexer, row, node, Report.DISCARDING_UNEXPECTED);
						continue;
					}
					
					/*
					if text or inline or block move before table
					if head content move to head
					*/
					
					if (node.Type != Node.EndTag)
					{
						if (node.Tag == tt.TagForm)
						{
							lexer.UngetToken();
							node = lexer.InferredTag("td");
							Report.Warning(lexer, row, node, Report.MISSING_STARTTAG);
						}
						else if (node.Type == Node.TextNode || (node.Tag.Model & (ContentModel.Block | ContentModel.Inline)) != 0)
						{
							Node.MoveBeforeTable(row, node, tt);
							Report.Warning(lexer, row, node, Report.TAG_NOT_ALLOWED_IN);
							lexer.exiled = true;
							
							if (node.Type != Node.TextNode)
								TidyNet.ParserImpl.parseTag(lexer, node, Lexer.IgnoreWhitespace);
							
							lexer.exiled = false;
							continue;
						}
						else if ((node.Tag.Model & ContentModel.Head) != 0)
						{
							Report.Warning(lexer, row, node, Report.TAG_NOT_ALLOWED_IN);
							TidyNet.ParserImpl.moveToHead(lexer, row, node);
							continue;
						}
					}
					
					if (!(node.Tag == tt.TagTd || node.Tag == tt.TagTh))
					{
						Report.Warning(lexer, row, node, Report.TAG_NOT_ALLOWED_IN);
						continue;
					}
					
					/* node should be <TD> or <TH> */
					Node.InsertNodeAtEnd(row, node);
					exclude_state = lexer.excludeBlocks;
					lexer.excludeBlocks = false;
					TidyNet.ParserImpl.parseTag(lexer, node, Lexer.IgnoreWhitespace);
					lexer.excludeBlocks = exclude_state;
					
					/* pop inline stack */
					
					while (lexer.istack.Count > lexer.istackbase)
						lexer.PopInline(null);
				}
				
				Node.TrimEmptyElement(lexer, row);
			}
		}
		
		
		public class ParseNoFramesCheckTable : IParser
		{
			public virtual void Parse(Lexer lexer, Node noframes, short mode)
			{
				Node node;
				TagTable tt = lexer.Options.tt;
				
				lexer.badAccess |= Report.USING_NOFRAMES;
				mode = Lexer.IgnoreWhitespace;
				
				while (true)
				{
					node = lexer.GetToken(mode);
					if (node == null)
						break;
					if (node.Tag == noframes.Tag && node.Type == Node.EndTag)
					{
						noframes.Closed = true;
						Node.TrimSpaces(lexer, noframes);
						return;
					}
					
					if ((node.Tag == tt.TagFrame || node.Tag == tt.TagFrameset))
					{
						Report.Warning(lexer, noframes, node, Report.MISSING_ENDTAG_BEFORE);
						Node.TrimSpaces(lexer, noframes);
						lexer.UngetToken();
						return;
					}
					
					if (node.Tag == tt.TagHtml)
					{
						if (node.Type == Node.StartTag || node.Type == Node.StartEndTag)
							Report.Warning(lexer, noframes, node, Report.DISCARDING_UNEXPECTED);
						
						continue;
					}
					
					/* deal with comments etc. */
					if (Node.InsertMisc(noframes, node))
						continue;
					
					if (node.Tag == tt.TagBody && node.Type == Node.StartTag)
					{
						Node.InsertNodeAtEnd(noframes, node);
						TidyNet.ParserImpl.parseTag(lexer, node, Lexer.IgnoreWhitespace);
						continue;
					}
					
					/* implicit body element inferred */
					if (node.Type == Node.TextNode || node.Tag != null)
					{
						lexer.UngetToken();
						node = lexer.InferredTag("body");
						if (lexer.Options.XmlOut)
							Report.Warning(lexer, noframes, node, Report.INSERTING_TAG);
						Node.InsertNodeAtEnd(noframes, node);
						TidyNet.ParserImpl.parseTag(lexer, node, Lexer.IgnoreWhitespace);
						continue;
					}
					/* discard unexpected end tags */
					Report.Warning(lexer, noframes, node, Report.DISCARDING_UNEXPECTED);
				}
				
				Report.Warning(lexer, noframes, node, Report.MISSING_ENDTAG_FOR);
			}
		}
		
		
		public class ParseSelectCheckTable : IParser
		{
			public virtual void Parse(Lexer lexer, Node field, short mode)
			{
				Node node;
				TagTable tt = lexer.Options.tt;
				
				lexer.insert = - 1; /* defer implicit inline start tags */
				
				while (true)
				{
					node = lexer.GetToken(Lexer.IgnoreWhitespace);
					if (node == null)
						break;
					if (node.Tag == field.Tag && node.Type == Node.EndTag)
					{
						field.Closed = true;
						Node.TrimSpaces(lexer, field);
						return;
					}
					
					/* deal with comments etc. */
					if (Node.InsertMisc(field, node))
						continue;
					
					if (node.Type == Node.StartTag && (node.Tag == tt.TagOption || node.Tag == tt.TagOptgroup || node.Tag == tt.TagScript))
					{
						Node.InsertNodeAtEnd(field, node);
						TidyNet.ParserImpl.parseTag(lexer, node, Lexer.IgnoreWhitespace);
						continue;
					}
					
					/* discard unexpected tags */
					Report.Warning(lexer, field, node, Report.DISCARDING_UNEXPECTED);
				}
				
				Report.Warning(lexer, field, node, Report.MISSING_ENDTAG_FOR);
			}
		}
		
		
		public class ParseTextCheckTable : IParser
		{
			public virtual void Parse(Lexer lexer, Node field, short mode)
			{
				Node node;
				TagTable tt = lexer.Options.tt;
				
				lexer.insert = - 1; /* defer implicit inline start tags */
				
				if (field.Tag == tt.TagTextarea)
					mode = Lexer.Preformatted;
				
				while (true)
				{
					node = lexer.GetToken(mode);
					if (node == null)
						break;
					if (node.Tag == field.Tag && node.Type == Node.EndTag)
					{
						field.Closed = true;
						Node.TrimSpaces(lexer, field);
						return;
					}
					
					/* deal with comments etc. */
					if (Node.InsertMisc(field, node))
						continue;
					
					if (node.Type == Node.TextNode)
					{
						/* only called for 1st child */
						if (field.Content == null && !((mode & Lexer.Preformatted) != 0))
							Node.TrimSpaces(lexer, field);
						
						if (node.Start >= node.End)
						{
							continue;
						}
						
						Node.InsertNodeAtEnd(field, node);
						continue;
					}
					
					if (node.Tag == tt.TagFont)
					{
						Report.Warning(lexer, field, node, Report.DISCARDING_UNEXPECTED);
						continue;
					}
					
					/* terminate element on other tags */
					if (!((field.Tag.Model & ContentModel.Opt) != 0))
						Report.Warning(lexer, field, node, Report.MISSING_ENDTAG_BEFORE);
					
					lexer.UngetToken();
					Node.TrimSpaces(lexer, field);
					return;
				}
				
				if (!((field.Tag.Model & ContentModel.Opt) != 0))
					Report.Warning(lexer, field, node, Report.MISSING_ENDTAG_FOR);
			}
		}
		
		
		public class ParseOptGroupCheckTable : IParser
		{
			public virtual void Parse(Lexer lexer, Node field, short mode)
			{
				Node node;
				TagTable tt = lexer.Options.tt;
				
				lexer.insert = - 1; /* defer implicit inline start tags */
				
				while (true)
				{
					node = lexer.GetToken(Lexer.IgnoreWhitespace);
					if (node == null)
						break;
					if (node.Tag == field.Tag && node.Type == Node.EndTag)
					{
						field.Closed = true;
						Node.TrimSpaces(lexer, field);
						return;
					}
					
					/* deal with comments etc. */
					if (Node.InsertMisc(field, node))
						continue;
					
					if (node.Type == Node.StartTag && (node.Tag == tt.TagOption || node.Tag == tt.TagOptgroup))
					{
						if (node.Tag == tt.TagOptgroup)
							Report.Warning(lexer, field, node, Report.CANT_BE_NESTED);
						
						Node.InsertNodeAtEnd(field, node);
						TidyNet.ParserImpl.parseTag(lexer, node, Lexer.MixedContent);
						continue;
					}
					
					/* discard unexpected tags */
					Report.Warning(lexer, field, node, Report.DISCARDING_UNEXPECTED);
				}
			}
		}

		/*
		HTML is the top level element
		*/
		public static Node parseDocument(Lexer lexer)
		{
			Node node, document, html;
			Node doctype = null;
			TagTable tt = lexer.Options.tt;

			document = lexer.NewNode();
			document.Type = Node.RootNode;
			
			while (true)
			{
				node = lexer.GetToken(Lexer.IgnoreWhitespace);
				if (node == null)
				{
					break;
				}
				
				/* deal with comments etc. */
				if (Node.InsertMisc(document, node))
				{
					continue;
				}
				
				if (node.Type == Node.DocTypeTag)
				{
					if (doctype == null)
					{
						Node.InsertNodeAtEnd(document, node);
						doctype = node;
					}
					else
					{
						Report.Warning(lexer, document, node, Report.DISCARDING_UNEXPECTED);
					}
					continue;
				}
				
				if (node.Type == Node.EndTag)
				{
					Report.Warning(lexer, document, node, Report.DISCARDING_UNEXPECTED); //TODO?
					continue;
				}
				
				if (node.Type != Node.StartTag || node.Tag != tt.TagHtml)
				{
					lexer.UngetToken();
					html = lexer.InferredTag("html");
				}
				else
				{
					html = node;
				}
				
				Node.InsertNodeAtEnd(document, html);
				ParseHTML.Parse(lexer, html, (short) 0); // TODO?
				break;
			}
			
			return document;
		}
		
		/// <summary>  Indicates whether or not whitespace should be preserved for this element.
		/// If an <code>xml:space</code> attribute is found, then if the attribute value is
		/// <code>preserve</code>, returns <code>true</code>.  For any other value, returns
		/// <code>false</code>.  If an <code>xml:space</code> attribute was <em>not</em>
		/// found, then the following element names result in a return value of <code>true:
		/// pre, script, style,</code> and <code>xsl:text</code>.  Finally, if a
		/// <code>TagTable</code> was passed in and the element appears as the "pre" element
		/// in the <code>TagTable</code>, then <code>true</code> will be returned.
		/// Otherwise, <code>false</code> is returned.
		/// </summary>
		/// <param name="element">The <code>Node</code> to test to see if whitespace should be
		/// preserved.
		/// </param>
		/// <param name="tt">The <code>TagTable</code> to test for the <code>getNodePre()</code>
		/// function.  This may be <code>null</code>, in which case this test
		/// is bypassed.
		/// </param>
		/// <returns> <code>true</code> or <code>false</code>, as explained above.
		/// 
		/// </returns>
		
		public static bool XMLPreserveWhiteSpace(Node element, TagTable tt)
		{
			AttVal attribute;
			
			/* search attributes for xml:space */
			for (attribute = element.Attributes; attribute != null; attribute = attribute.Next)
			{
				if (attribute.Attribute.Equals("xml:space"))
				{
					if (attribute.Val.Equals("preserve"))
					{
						return true;
					}
					
					return false;
				}
			}
			
			/* kludge for html docs without explicit xml:space attribute */
			if (String.Compare(element.Element, "pre") == 0 || String.Compare(element.Element, "script") == 0 || String.Compare(element.Element, "style") == 0)
			{
				return true;
			}
			
			if ((tt != null) && (tt.FindParser(element) == ParsePre))
			{
				return true;
			}
			
			/* kludge for XSL docs */
			if (String.Compare(element.Element, "xsl:text") == 0)
			{
				return true;
			}
			
			return false;
		}
		
		/*
		XML documents
		*/
		public static void  parseXMLElement(Lexer lexer, Node element, short mode)
		{
			Node node;
			
			/* Jeff Young's kludge for XSL docs */
			if (String.Compare(element.Element, "xsl:text") == 0)
			{
				return;
			}
			
			/* if node is pre or has xml:space="preserve" then do so */
			
			if (XMLPreserveWhiteSpace(element, lexer.Options.tt))
			{
				mode = Lexer.Preformatted;
			}
			
			while (true)
			{
				node = lexer.GetToken(mode);
				if (node == null)
				{
					break;
				}
				if (node.Type == Node.EndTag && node.Element.Equals(element.Element))
				{
					element.Closed = true;
					break;
				}
				
				/* discard unexpected end tags */
				if (node.Type == Node.EndTag)
				{
					Report.Error(lexer, element, node, Report.UNEXPECTED_ENDTAG);
					continue;
				}
				
				/* parse content on seeing start tag */
				if (node.Type == Node.StartTag)
				{
					parseXMLElement(lexer, node, mode);
				}
				
				Node.InsertNodeAtEnd(element, node);
			}
			
			/*
			if first child is text then trim initial space and
			delete text node if it is empty.
			*/
			
			node = element.Content;
			
			if (node != null && node.Type == Node.TextNode && mode != Lexer.Preformatted)
			{
				if (node.Textarray[node.Start] == (sbyte) ' ')
				{
					node.Start++;
					
					if (node.Start >= node.End)
					{
						Node.DiscardElement(node);
					}
				}
			}
			
			/*
			if last child is text then trim final space and
			delete the text node if it is empty
			*/
			
			node = element.Last;
			
			if (node != null && node.Type == Node.TextNode && mode != Lexer.Preformatted)
			{
				if (node.Textarray[node.End - 1] == (sbyte) ' ')
				{
					node.End--;
					
					if (node.Start >= node.End)
					{
						Node.DiscardElement(node);
					}
				}
			}
		}
		
		public static Node parseXMLDocument(Lexer lexer)
		{
			Node node, document, doctype;
			
			document = lexer.NewNode();
			document.Type = Node.RootNode;
			doctype = null;
			lexer.Options.XmlTags = true;
			
			while (true)
			{
				node = lexer.GetToken(Lexer.IgnoreWhitespace);
				if (node == null)
				{
					break;
				}

				/* discard unexpected end tags */
				if (node.Type == Node.EndTag)
				{
					Report.Warning(lexer, null, node, Report.UNEXPECTED_ENDTAG);
					continue;
				}
				
				/* deal with comments etc. */
				if (Node.InsertMisc(document, node))
				{
					continue;
				}
				
				if (node.Type == Node.DocTypeTag)
				{
					if (doctype == null)
					{
						Node.InsertNodeAtEnd(document, node);
						doctype = node;
					}
					else
					{
						Report.Warning(lexer, document, node, Report.DISCARDING_UNEXPECTED);
					}
					// TODO
					continue;
				}
				
				/* if start tag then parse element's content */
				if (node.Type == Node.StartTag)
				{
					Node.InsertNodeAtEnd(document, node);
					parseXMLElement(lexer, node, Lexer.IgnoreWhitespace);
				}
			}
			
			if (doctype != null && !lexer.CheckDocTypeKeyWords(doctype))
			{
				Report.Warning(lexer, doctype, null, Report.DTYPE_NOT_UPPER_CASE);
			}
			
			/* ensure presence of initial <?XML version="1.0"?> */
			if (lexer.Options.XmlPi)
			{
				lexer.FixXmlPI(document);
			}
			
			return document;
		}
		
		public static bool isJavaScript(Node node)
		{
			bool result = false;
			AttVal attr;
			
			if (node.Attributes == null)
			{
				return true;
			}
			
			for (attr = node.Attributes; attr != null; attr = attr.Next)
			{
				if ((String.Compare(attr.Attribute, "language") == 0 || String.Compare(attr.Attribute, "type") == 0) && wsubstr(attr.Val, "javascript"))
				{
					result = true;
				}
			}
			
			return result;
		}

		private static bool wsubstr(string s1, string s2)
		{
			int i;
			int len1 = s1.Length;
			int len2 = s2.Length;
			
			for (i = 0; i <= len1 - len2; ++i)
			{
				if (s2.ToUpper().Equals(s1.Substring(i).ToUpper()))
				{
					return true;
				}
			}
			
			return false;
		}
		
		private static IParser _parseHTML = new ParseHTMLCheckTable();
		private static IParser _parseHead = new ParseHeadCheckTable();
		private static IParser _parseTitle = new ParseTitleCheckTable();
		private static IParser _parseScript = new ParseScriptCheckTable();
		private static IParser _parseBody = new ParseBodyCheckTable();
		private static IParser _parseFrameSet = new ParseFrameSetCheckTable();
		private static IParser _parseInline = new ParseInlineCheckTable();
		private static IParser _parseList = new ParseListCheckTable();
		private static IParser _parseDefList = new ParseDefListCheckTable();
		private static IParser _parsePre = new ParsePreCheckTable();
		private static IParser _parseBlock = new ParseBlockCheckTable();
		private static IParser _parseTableTag = new ParseTableTagCheckTable();
		private static IParser _parseColGroup = new ParseColGroupCheckTable();
		private static IParser _parseRowGroup = new ParseRowGroupCheckTable();
		private static IParser _parseRow = new ParseRowCheckTable();
		private static IParser _parseNoFrames = new ParseNoFramesCheckTable();
		private static IParser _parseSelect = new ParseSelectCheckTable();
		private static IParser _parseText = new ParseTextCheckTable();
		private static IParser _parseOptGroup = new ParseOptGroupCheckTable();
	}
}