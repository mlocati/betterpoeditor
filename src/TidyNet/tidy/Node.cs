using System;
using System.Text;
using TidyNet.Dom;

namespace TidyNet
{
	/// <summary>
	/// Node
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
	/// Used for elements and text nodes element name is null for text nodes
	/// start and end are offsets into lexbuf which contains the textual content of
	/// all elements in the parse tree.
	/// 
	/// parent and content allow traversal of the parse tree in any direction.
	/// attributes are represented as a linked list of AttVal nodes which hold the
	/// strings for attribute/value pairs.
	/// </remarks>
	internal class Node
	{
		public const short RootNode = 0;
		public const short DocTypeTag = 1;
		public const short CommentTag = 2;
		public const short ProcInsTag = 3;
		public const short TextNode = 4;
		public const short StartTag = 5;
		public const short EndTag = 6;
		public const short StartEndTag = 7;
		public const short CDATATag = 8;
		public const short SectionTag = 9;
		public const short AspTag = 10;
		public const short JsteTag = 11;
		public const short PhpTag = 12;

		public Node() : this(TextNode, null, 0, 0)
		{
		}
		
		public Node(short type, byte[] textarray, int start, int end)
		{
			_parent = null;
			_prev = null;
			_next = null;
			_last = null;
			_start = start;
			_end = end;
			_textarray = textarray;
			_type = type;
			_closed = false;
			_isimplicit = false;
			_linebreak = false;
			_was = null;
			_tag = null;
			_element = null;
			_attributes = null;
			_content = null;
		}
		
		public Node(short type, byte[] textarray, int start, int end, string element, TagTable tt)
		{
			_parent = null;
			_prev = null;
			_next = null;
			_last = null;
			_start = start;
			_end = end;
			_textarray = textarray;
			_type = type;
			_closed = false;
			_isimplicit = false;
			_linebreak = false;
			_was = null;
			_tag = null;
			_element = element;
			_attributes = null;
			_content = null;
			if (type == StartTag || type == StartEndTag || type == EndTag)
			{
				tt.FindTag(this);
			}
		}
		
		virtual public bool IsElement
		{
			get
			{
				return (_type == StartTag || _type == StartEndTag ? true : false);
			}
			
		}

		protected virtual internal string Element
		{
			get
			{
				return _element;
			}
			set
			{
				_element = value;
			}
		}

		protected virtual internal INode Adapter
		{
			get
			{
				if (_adapter == null)
				{
					switch (_type)
					{
						
					case RootNode: 
						_adapter = new DomDocumentImpl(this);
						break;
						
					case StartTag: 
					case StartEndTag: 
						_adapter = new DOMElementImpl(this);
						break;
						
					case DocTypeTag: 
						_adapter = new DomDocumentTypeImpl(this);
						break;
						
					case CommentTag: 
						_adapter = new DomCommentImpl(this);
						break;
						
					case TextNode: 
						_adapter = new DomTextImpl(this);
						break;
						
					case CDATATag: 
						_adapter = new DomCdataSectionImpl(this);
						break;
						
					case ProcInsTag: 
						_adapter = new DomProcessingInstructionImpl(this);
						break;
						
					default: 
						_adapter = new DomNodeImpl(this);
						break;
						
					}
				}

				return _adapter;
			}
		}

		protected virtual internal short Type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}
		
		/* used to clone heading nodes when split by an <HR> */
		protected internal object Clone()
		{
			Node node = new Node();
			
			node.Parent = _parent;
			if (_textarray != null)
			{
				node.Textarray = new byte[_end - _start];
				node.Start = 0;
				node.End = _end - _start;
				if (node.End > 0)
				{
					Array.Copy(_textarray, _start, node.Textarray, node.Start, node.End);
				}
			}
			node.Type = _type;
			node.Closed = _closed;
			node.Isimplicit = _isimplicit;
			node.Linebreak = _linebreak;
			node.Was = _was;
			node.Tag = _tag;
			if (_element != null)
			{
				node.Element = _element;
			}
			if (_attributes != null)
			{
				node.Attributes = (AttVal) _attributes.Clone();
			}
			return node;
		}
		
		public virtual AttVal GetAttrByName(string name)
		{
			AttVal attr;
			
			for (attr = _attributes; attr != null; attr = attr.Next)
			{
				if (name != null && attr.Attribute != null && attr.Attribute.Equals(name))
				{
					break;
				}
			}
			
			return attr;
		}
		
		/* default method for checking an element's attributes */
		public virtual void CheckAttributes(Lexer lexer)
		{
			AttVal attval;
			
			for (attval = _attributes; attval != null; attval = attval.Next)
			{
				attval.CheckAttribute(lexer, this);
			}
		}
		
		public virtual void CheckUniqueAttributes(Lexer lexer)
		{
			AttVal attval;
			
			for (attval = _attributes; attval != null; attval = attval.Next)
			{
				if (attval.Asp == null && attval.Php == null)
				{
					attval.CheckUniqueAttribute(lexer, this);
				}
			}
		}
		
		public virtual void AddAttribute(string name, string val)
		{
			AttVal av = new AttVal(null, null, null, null, '"', name, val);
			av.Dict = AttributeTable.DefaultAttributeTable.FindAttribute(av);
			
			if (_attributes == null)
			{
				_attributes = av;
				/* append to end of attributes */
			}
			else
			{
				AttVal here = _attributes;
				
				while (here.Next != null)
				{
					here = here.Next;
				}
				
				here.Next = av;
			}
		}
		
		/* remove attribute from node then free it */
		public virtual void RemoveAttribute(AttVal attr)
		{
			AttVal av;
			AttVal prev = null;
			AttVal next;
			
			for (av = _attributes; av != null; av = next)
			{
				next = av.Next;
				
				if (av == attr)
				{
					if (prev != null)
					{
						prev.Next = next;
					}
					else
					{
						_attributes = next;
					}
				}
				else
				{
					prev = av;
				}
			}
		}
		
		/* find doctype element */
		public virtual Node FindDocType()
		{
			Node node;
			
			for (node = Content; node != null && node.Type != DocTypeTag; node = node.Next)
			{
				;
			}
			
			return node;
		}
		
		public virtual void DiscardDocType()
		{
			Node node;
			
			node = FindDocType();
			if (node != null)
			{
				if (node.Prev != null)
				{
					node.Prev.Next = node.Next;
				}
				else
				{
					node.Parent.Content = node.Next;
				}
				
				if (node.Next != null)
				{
					node.Next.Prev = node.Prev;
				}
				
				node.Next = null;
			}
		}
		
		/* remove node from markup tree and discard it */
		public static Node DiscardElement(Node element)
		{
			Node next = null;
			
			if (element != null)
			{
				next = element.Next;
				RemoveNode(element);
			}
			
			return next;
		}
		
		/* insert node into markup tree */
		public static void InsertNodeAtStart(Node element, Node node)
		{
			node.Parent = element;
			
			if (element.Content == null)
			{
				element.Last = node;
			}
			else
			{
				element.Content.Prev = node;
			}
			// AQ added 13 Apr 2000
			
			node.Next = element.Content;
			node.Prev = null;
			element.Content = node;
		}
		
		/* insert node into markup tree */
		public static void InsertNodeAtEnd(Node element, Node node)
		{
			node.Parent = element;
			node.Prev = element.Last;
			
			if (element.Last != null)
			{
				element.Last.Next = node;
			}
			else
			{
				element.Content = node;
			}
			
			element.Last = node;
		}
		
		/*
		insert node into markup tree in pace of element
		which is moved to become the child of the node
		*/
		public static void InsertNodeAsParent(Node element, Node node)
		{
			node.Content = element;
			node.Last = element;
			node.Parent = element.Parent;
			element.Parent = node;
			
			if (node.Parent.Content == element)
			{
				node.Parent.Content = node;
			}
			
			if (node.Parent.Last == element)
			{
				node.Parent.Last = node;
			}
			
			node.Prev = element.Prev;
			element.Prev = null;
			
			if (node.Prev != null)
			{
				node.Prev.Next = node;
			}
			
			node.Next = element.Next;
			element.Next = null;
			
			if (node.Next != null)
			{
				node.Next.Prev = node;
			}
		}
		
		/* insert node into markup tree before element */
		public static void InsertNodeBeforeElement(Node element, Node node)
		{
			Node parent;
			
			parent = element.Parent;
			node.Parent = parent;
			node.Next = element;
			node.Prev = element.Prev;
			element.Prev = node;
			
			if (node.Prev != null)
			{
				node.Prev.Next = node;
			}
			
			if (parent.Content == element)
			{
				parent.Content = node;
			}
		}
		
		/* insert node into markup tree after element */
		public static void InsertNodeAfterElement(Node element, Node node)
		{
			Node parent;
			
			parent = element.Parent;
			node.Parent = parent;
			
			// AQ - 13Jan2000 fix for parent == null
			if (parent != null && parent.Last == element)
			{
				parent.Last = node;
			}
			else
			{
				node.Next = element.Next;
				// AQ - 13Jan2000 fix for node.next == null
				if (node.Next != null)
				{
					node.Next.Prev = node;
				}
			}
			
			element.Next = node;
			node.Prev = element;
		}
		
		public static void TrimEmptyElement(Lexer lexer, Node element)
		{
			TagTable tt = lexer.Options.tt;
			
			if (lexer.CanPrune(element))
			{
				if (element.Type != TextNode)
				{
					Report.Warning(lexer, element, null, Report.TRIM_EMPTY_ELEMENT);
				}
				
				DiscardElement(element);
			}
			else if (element.Tag == tt.TagP && element.Content == null)
			{
				/* replace <p></p> by <br><br> to preserve formatting */
				Node node = lexer.InferredTag("br");
				Node.CoerceNode(lexer, element, tt.TagBr);
				Node.InsertNodeAfterElement(element, node);
			}
		}
		
		/*
		This maps 
		<em>hello </em><strong>world</strong>
		to
		<em>hello</em> <strong>world</strong>
		
		If last child of element is a text node
		then trim trailing white space character
		moving it to after element's end tag.
		*/
		public static void TrimTrailingSpace(Lexer lexer, Node element, Node last)
		{
			byte c;
			TagTable tt = lexer.Options.tt;
			
			if (last != null && last.Type == Node.TextNode && last.End > last.Start)
			{
				c = lexer.lexbuf[last.End - 1];
				
				if (c == 160 || c == (byte) ' ')
				{
					/* take care with <td>&nbsp;</td> */
					if (element.Tag == tt.TagTd || element.Tag == tt.TagTh)
					{
						if (last.End > last.Start + 1)
						{
							last.End -= 1;
						}
					}
					else
					{
						last.End -= 1;
						
						if (((element.Tag.Model & ContentModel.Inline) != 0) && !((element.Tag.Model & ContentModel.Field) != 0))
						{
							lexer.insertspace = true;
						}
						
						/* if empty string then delete from parse tree */
						if (last.Start == last.End)
						{
							TrimEmptyElement(lexer, last);
						}
					}
				}
			}
		}
		
		/*
		This maps 
		<p>hello<em> world</em>
		to
		<p>hello <em>world</em>
		
		Trims initial space, by moving it before the
		start tag, or if this element is the first in
		parent's content, then by discarding the space
		*/
		public static void TrimInitialSpace(Lexer lexer, Node element, Node text)
		{
			Node prev, node;
			
			// GLP: Local fix to Bug 119789. Remove this comment when parser.c is updated.
			//      31-Oct-00. 
			if (text.Type == TextNode && text.Textarray[text.Start] == (byte) ' ' && (text.Start < text.End))
			{
				if (((element.Tag.Model & ContentModel.Inline) != 0) && !((element.Tag.Model & ContentModel.Field) != 0) && element.Parent.Content != element)
				{
					prev = element.Prev;
					
					if (prev != null && prev.Type == TextNode)
					{
						if (prev.Textarray[prev.End - 1] != (byte) ' ')
						{
							prev.Textarray[prev.End++] = (byte) ' ';
						}
						
						++element.Start;
					}
					/* create new node */
					else
					{
						node = lexer.NewNode();
						// Local fix for bug 228486 (GLP).  This handles the case
						// where we need to create a preceeding text node but there are
						// no "slots" in textarray that we can steal from the current
						// element.  Therefore, we create a new textarray containing
						// just the blank.  When Tidy is fixed, this should be removed.
						if (element.Start >= element.End)
						{
							node.Start = 0;
							node.End = 1;
							node.Textarray = new byte[1];
						}
						else
						{
							node.Start = element.Start++;
							node.End = element.Start;
							node.Textarray = element.Textarray;
						}
						node.Textarray[node.Start] = (byte) ' ';
						node.Prev = prev;
						if (prev != null)
						{
							prev.Next = node;
						}
						node.Next = element;
						element.Prev = node;
						node.Parent = element.Parent;
					}
				}
				
				/* discard the space  in current node */
				++text.Start;
			}
		}
		
		/* 
		Move initial and trailing space out.
		This routine maps:
		
		hello<em> world</em>
		to
		hello <em>world</em>
		and
		<em>hello </em><strong>world</strong>
		to
		<em>hello</em> <strong>world</strong>
		*/
		public static void TrimSpaces(Lexer lexer, Node element)
		{
			Node text = element.Content;
			TagTable tt = lexer.Options.tt;
			
			if (text != null && text.Type == Node.TextNode && element.Tag != tt.TagPre)
			{
				TrimInitialSpace(lexer, element, text);
			}
			
			text = element.Last;
			
			if (text != null && text.Type == Node.TextNode)
			{
				TrimTrailingSpace(lexer, element, text);
			}
		}
		
		public virtual bool IsDescendantOf(Dict tag)
		{
			Node parent;
			
			for (parent = _parent; parent != null; parent = parent.Parent)
			{
				if (parent.Tag == tag)
				{
					return true;
				}
			}
			
			return false;
		}
		
		/*
		the doctype has been found after other tags,
		and needs moving to before the html element
		*/
		public static void InsertDocType(Lexer lexer, Node element, Node doctype)
		{
			TagTable tt = lexer.Options.tt;
			
			Report.Warning(lexer, element, doctype, Report.DOCTYPE_AFTER_TAGS);
			
			while (element.Tag != tt.TagHtml)
			{
				element = element.Parent;
			}
			
			InsertNodeBeforeElement(element, doctype);
		}
		
		public virtual Node FindBody(TagTable tt)
		{
			Node node;
			
			node = _content;
			
			while (node != null && node.Tag != tt.TagHtml)
			{
				node = node.Next;
			}
			
			if (node == null)
			{
				return null;
			}
			
			node = node.Content;
			
			while (node != null && node.Tag != tt.TagBody)
			{
				node = node.Next;
			}
			
			return node;
		}
		
		
		/*
		unexpected content in table row is moved to just before
		the table in accordance with Netscape and IE. This code
		assumes that node hasn't been inserted into the row.
		*/
		public static void MoveBeforeTable(Node row, Node node, TagTable tt)
		{
			Node table;
			
			/* first find the table element */
			for (table = row.Parent; table != null; table = table.Parent)
			{
				if (table.Tag == tt.tagTable)
				{
					if (table.Parent.Content == table)
					{
						table.Parent.Content = node;
					}
					
					node.Prev = table.Prev;
					node.Next = table;
					table.Prev = node;
					node.Parent = table.Parent;
					
					if (node.Prev != null)
					{
						node.Prev.Next = node;
					}
					
					break;
				}
			}
		}
		
		/*
		if a table row is empty then insert an empty cell
		this practice is consistent with browser behavior
		and avoids potential problems with row spanning cells
		*/
		public static void FixEmptyRow(Lexer lexer, Node row)
		{
			Node cell;
			
			if (row.Content == null)
			{
				cell = lexer.InferredTag("td");
				InsertNodeAtEnd(row, cell);
				Report.Warning(lexer, row, cell, Report.MISSING_STARTTAG);
			}
		}
		
		public static void CoerceNode(Lexer lexer, Node node, Dict tag)
		{
			Node tmp = lexer.InferredTag(tag.Name);
			Report.Warning(lexer, node, tmp, Report.OBSOLETE_ELEMENT);
			node.Was = node.Tag;
			node.Tag = tag;
			node.Type = StartTag;
			node.Isimplicit = true;
			node.Element = tag.Name;
		}
		
		/* extract a node and its children from a markup tree */
		public static void RemoveNode(Node node)
		{
			if (node.Prev != null)
			{
				node.Prev.Next = node.Next;
			}
			
			if (node.Next != null)
			{
				node.Next.Prev = node.Prev;
			}
			
			if (node.Parent != null)
			{
				if (node.Parent.Content == node)
				{
					node.Parent.Content = node.Next;
				}
				
				if (node.Parent.Last == node)
				{
					node.Parent.Last = node.Prev;
				}
			}
			
			node.Parent = node.Prev = node.Next = null;
		}
		
		public static bool InsertMisc(Node element, Node node)
		{
			if (node.Type == CommentTag || node.Type == ProcInsTag || node.Type == CDATATag || node.Type == SectionTag || node.Type == AspTag || node.Type == JsteTag || node.Type == PhpTag)
			{
				InsertNodeAtEnd(element, node);
				return true;
			}
			
			return false;
		}
		
		/*
		used to determine how attributes
		without values should be printed
		this was introduced to deal with
		user defined tags e.g. Cold Fusion
		*/
		public static bool IsNewNode(Node node)
		{
			if (node != null && node.Tag != null)
			{
				return ((node.Tag.Model & ContentModel.New) != 0);
			}
			
			return true;
		}
		
		public virtual bool HasOneChild()
		{
			return (_content != null && _content.Next == null);
		}
		
		/* find html element */
		public virtual Node FindHtml(TagTable tt)
		{
			Node node;
			
			for (node = _content; node != null && node.Tag != tt.TagHtml; node = node.Next)
			{
				;
			}
			
			return node;
		}
		
		public virtual Node FindHead(TagTable tt)
		{
			Node node;
			
			node = FindHtml(tt);
			
			if (node != null)
			{
				for (node = node.Content; node != null && node.Tag != tt.TagHead; node = node.Next)
				{
					;
				}
			}
			
			return node;
		}
		
		public virtual bool CheckNodeIntegrity()
		{
			Node child;
			bool found = false;
			
			if (_prev != null)
			{
				if (_prev.Next != this)
				{
					return false;
				}
			}
			
			if (_next != null)
			{
				if (_next.Prev != this)
				{
					return false;
				}
			}
			
			if (_parent != null)
			{
				if (_prev == null && _parent.Content != this)
				{
					return false;
				}
				
				if (_next == null && _parent.Last != this)
				{
					return false;
				}
				
				for (child = _parent.Content; child != null; child = child.Next)
				{
					if (child == this)
					{
						found = true;
						break;
					}
				}
				
				if (!found)
				{
					return false;
				}
			}
			
			for (child = _content; child != null; child = child.Next)
			{
				if (!child.CheckNodeIntegrity())
				{
					return false;
				}
			}
			
			return true;
		}
		
		public static void AddClass(Node node, string classname)
		{
			AttVal classattr = node.GetAttrByName("class");
			
			/*
			if there already is a class attribute
			then append class name after a space
			*/
			if (classattr != null)
			{
				classattr.Val = classattr.Val + " " + classname;
			}
				/* create new class attribute */
			else
			{
				node.AddAttribute("class", classname);
			}
		}
		
		public override string ToString()
		{
			StringBuilder s = new StringBuilder();
			Node n = this;
			
			while (n != null)
			{
				s.AppendFormat("[Node type={0},element=", nodeTypeString[n.Type]);
				if (n.Element != null)
				{
					s.Append(n.Element);
				}
				else
				{
					s.Append("null");
				}
				if (n.Type == TextNode || n.Type == CommentTag || n.Type == ProcInsTag)
				{
					s.Append(",text=");
					if (n.Textarray != null && n.Start <= n.End)
					{
						s.AppendFormat("\"{0}\"", Lexer.GetString(n.Textarray, n.Start, n.End - n.Start));
					}
					else
					{
						s.Append("null");
					}
				}
				s.Append(",content=");
				if (n.Content != null)
				{
					s.Append(n.Content.ToString());
				}
				else
				{
					s.Append("null");
				}
				s.Append("]");
				if (n.Next != null)
				{
					s.Append(",");
				}
				n = n.Next;
			}

			return s.ToString();
		}

		protected internal virtual Node CloneNode(bool deep)
		{
			Node node = (Node) Clone();
			if (deep)
			{
				Node child;
				Node newChild;
				for (child = _content; child != null; child = child.Next)
				{
					newChild = child.CloneNode(deep);
					InsertNodeAtEnd(node, newChild);
				}
			}
			return node;
		}
		
		internal Node Parent
		{
			get
			{
				return _parent;
			}
			set
			{
				_parent = value;
			}
		}

		internal Node Prev
		{
			get
			{
				return _prev;
			}
			set
			{
				_prev = value;
			}
		}

		internal Node Next
		{
			get
			{
				return _next;
			}
			set
			{
				_next = value;
			}
		}

		internal Node Last
		{
			get
			{
				return _last;
			}
			set
			{
				_last = value;
			}
		}

		internal int Start
		{
			get
			{
				return _start;
			}
			set
			{
				_start = value;
			}
		}

		internal int End
		{
			get
			{
				return _end;
			}
			set
			{
				_end = value;
			}
		}

		internal byte[] Textarray
		{
			get
			{
				return _textarray;
			}
			set
			{
				_textarray = value;
			}
		}

		internal bool Closed
		{
			get
			{
				return _closed;
			}
			set
			{
				_closed = value;
			}
		}

		internal bool Isimplicit
		{
			get
			{
				return _isimplicit;
			}
			set
			{
				_isimplicit = value;
			}
		}

		internal bool Linebreak
		{
			get
			{
				return _linebreak;
			}
			set
			{
				_linebreak = value;
			}
		}

		internal Dict Was
		{
			get
			{
				return _was;
			}
			set
			{
				_was = value;
			}
		}

		internal Dict Tag
		{
			get
			{
				return _tag;
			}
			set
			{
				_tag = value;
			}
		}

		internal AttVal Attributes
		{
			get
			{
				return _attributes;
			}
			set
			{
				_attributes = value;
			}
		}

		internal Node Content
		{
			get
			{
				return _content;
			}
			set
			{
				_content = value;
			}
		}

		protected INode _adapter = null;
		protected Node _parent;
		protected Node _prev;
		protected Node _next;
		protected Node _last;
		protected int _start; /* start of span onto text array */
		protected int _end; /* end of span onto text array */
		protected byte[] _textarray; /* the text array */
		protected short _type; /* TextNode, StartTag, EndTag etc. */
		protected bool _closed; /* true if closed by explicit end tag */
		protected bool _isimplicit; /* true if inferred */
		protected bool _linebreak; /* true if followed by a line break */
		protected Dict _was; /* old tag when it was changed */
		protected Dict _tag; /* tag's dictionary definition */
		protected string _element; /* name (null for text nodes) */
		protected AttVal _attributes;
		protected Node _content;

		private static readonly string[] nodeTypeString = new string[]{"RootNode", "DocTypeTag", "CommentTag", "ProcInsTag", "TextNode", "StartTag", "EndTag", "StartEndTag", "SectionTag", "AspTag", "PhpTag"};
	}
}