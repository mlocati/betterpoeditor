using System;
using TidyNet.Dom;

namespace TidyNet
{
	/// <summary>
	/// Clean up misuse of presentation markup
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
	/// Filters from other formats such as Microsoft Word
	/// often make excessive use of presentation markup such
	/// as font tags, B, I, and the align attribute. By applying
	/// a set of production rules, it is straight forward to
	/// transform this to use CSS.
	/// 
	/// Some rules replace some of the children of an element by
	/// style properties on the element, e.g.
	/// 
	/// &lt;p&gt;&lt;b&gt;...&lt;/b&gt;&lt;/p&gt; -&gt; &lt;p style="font-weight: bold"&gt;...&lt;/p&gt;
	/// 
	/// Such rules are applied to the element's content and then 
	/// to the element itself until none of the rules more apply.
	/// Having applied all the rules to an element, it will have
	/// a style attribute with one or more properties. 
	/// 
	/// Other rules strip the element they apply to, replacing
	/// it by style properties on the contents, e.g.
	/// 
	/// &lt;dir&gt;&lt;li&gt;&lt;p&gt;...&lt;/li&gt;&lt;/dir&gt; -&gt; &lt;p style="margin-left 1em"&gt;...
	/// 
	/// These rules are applied to an element before processing
	/// its content and replace the current element by the first
	/// element in the exposed content.
	/// 
	/// After applying both sets of rules, you can replace the
	/// style attribute by a class value and style rule in the
	/// document head. To support this, an association of styles
	/// and class names is built.
	/// 
	/// A naive approach is to rely on string matching to test
	/// when two property lists are the same. A better approach
	/// would be to first sort the properties before matching.
	/// </remarks>
	internal class Clean
	{
		public Clean(TagTable tt)
		{
			_tt = tt;
		}
		
		private StyleProp InsertProperty(StyleProp props, string name, string val)
		{
			StyleProp first, prev, prop;
			int cmp;
			
			prev = null;
			first = props;
			
			while (props != null)
			{
				cmp = props.Name.CompareTo(name);
				if (cmp == 0)
				{
					/* this property is already defined, ignore new value */
					return first;
				}
				else if (cmp > 0)
				{
					// props.name > name
					/* insert before this */
					
					prop = new StyleProp(name, val, props);
					
					if (prev != null)
					{
						prev.Next = prop;
					}
					else
					{
						first = prop;
					}
					
					return first;
				}
				
				prev = props;
				props = props.Next;
			}
			
			prop = new StyleProp(name, val);
			if (prev != null)
			{
				prev.Next = prop;
			}
			else
			{
				first = prop;
			}

			return first;
		}

		/*
		Create sorted linked list of properties from style string
		It temporarily places nulls in place of ':' and ';' to
		delimit the strings for the property name and value.
		Some systems don't allow you to null literal strings,
		so to avoid this, a copy is made first.
		*/
		private StyleProp CreateProps(StyleProp prop, string style)
		{
			int name_end;
			int value_end;
			int value_start = 0;
			int name_start = 0;
			bool more;
			
			name_start = 0;
			while (name_start < style.Length)
			{
				while (name_start < style.Length && style[name_start] == ' ')
				{
					++name_start;
				}
				
				name_end = name_start;
				
				while (name_end < style.Length)
				{
					if (style[name_end] == ':')
					{
						value_start = name_end + 1;
						break;
					}
					
					++name_end;
				}
				
				if (name_end >= style.Length || style[name_end] != ':')
				{
					break;
				}
				
				while (value_start < style.Length && style[value_start] == ' ')
				{
					++value_start;
				}
				
				value_end = value_start;
				more = false;
				
				while (value_end < style.Length)
				{
					if (style[value_end] == ';')
					{
						more = true;
						break;
					}
					
					++value_end;
				}
				
				prop = InsertProperty(prop, style.Substring(name_start, (name_end) - (name_start)), style.Substring(value_start, (value_end) - (value_start)));
				
				if (more)
				{
					name_start = value_end + 1;
					continue;
				}
				
				break;
			}
			
			return prop;
		}
		
		private string CreatePropString(StyleProp props)
		{
			string style = "";
			int len;
			StyleProp prop;
			
			/* compute length */
			
			for (len = 0, prop = props; prop != null; prop = prop.Next)
			{
				len += prop.Name.Length + 2;
				len += prop.Val.Length + 2;
			}
			
			for (prop = props; prop != null; prop = prop.Next)
			{
				style = string.Concat(style, prop.Name);
				style = string.Concat(style, ": ");
				
				style = string.Concat(style, prop.Val);
				
				if (prop.Next == null)
				{
					break;
				}
				
				style = string.Concat(style, "; ");
			}
			
			return style;
		}
		
		/*
		create string with merged properties
		*/
		private string AddProperty(string style, string property)
		{
			StyleProp prop;
			
			prop = CreateProps(null, style);
			prop = CreateProps(prop, property);
			style = CreatePropString(prop);
			return style;
		}
		
		private string GenSymClass(string tag)
		{
			string str;
			
			str = "c" + classNum;
			classNum++;
			return str;
		}
		
		private string FindStyle(Lexer lexer, string tag, string properties)
		{
			Style style;
			
			for (style = lexer.styles; style != null; style = style.Next)
			{
				if (style.Tag.Equals(tag) && style.Properties.Equals(properties))
				{
					return style.TagClass;
				}
			}
			
			style = new Style(tag, GenSymClass(tag), properties, lexer.styles);
			lexer.styles = style;
			return style.TagClass;
		}
		
		/*
		Find style attribute in node, and replace it
		by corresponding class attribute. Search for
		class in style dictionary otherwise gensym
		new class and add to dictionary.
		
		Assumes that node doesn't have a class attribute
		*/
		private void Style2Rule(Lexer lexer, Node node)
		{
			AttVal styleattr, classattr;
			string classname;
			
			styleattr = node.GetAttrByName("style");
			
			if (styleattr != null)
			{
				classname = FindStyle(lexer, node.Element, styleattr.Val);
				classattr = node.GetAttrByName("class");
				
				/*
				if there already is a class attribute
				then append class name after a space
				*/
				if (classattr != null)
				{
					classattr.Val = classattr.Val + " " + classname;
					node.RemoveAttribute(styleattr);
				}
				else
				{
					/* reuse style attribute for class attribute */
					styleattr.Attribute = "class";
					styleattr.Val = classname;
				}
			}
		}
		
		private void AddColorRule(Lexer lexer, string selector, string color)
		{
			if (color != null)
			{
				lexer.AddStringLiteral(selector);
				lexer.AddStringLiteral(" { color: ");
				lexer.AddStringLiteral(color);
				lexer.AddStringLiteral(" }\n");
			}
		}
		
		/*
		move presentation attribs from body to style element
		
		background="foo" ->  body { background-image: url(foo) }
		bgcolor="foo"    ->  body { background-color: foo }
		text="foo"       ->  body { color: foo }
		link="foo"       ->  :link { color: foo }
		vlink="foo"      ->  :visited { color: foo }
		alink="foo"      ->  :active { color: foo }
		*/
		private void CleanBodyAttrs(Lexer lexer, Node body)
		{
			AttVal attr;
			string bgurl = null;
			string bgcolor = null;
			string color = null;
			
			attr = body.GetAttrByName("background");
			
			if (attr != null)
			{
				bgurl = attr.Val;
				attr.Val = null;
				body.RemoveAttribute(attr);
			}
			
			attr = body.GetAttrByName("bgcolor");
			
			if (attr != null)
			{
				bgcolor = attr.Val;
				attr.Val = null;
				body.RemoveAttribute(attr);
			}
			
			attr = body.GetAttrByName("text");
			
			if (attr != null)
			{
				color = attr.Val;
				attr.Val = null;
				body.RemoveAttribute(attr);
			}
			
			if (bgurl != null || bgcolor != null || color != null)
			{
				lexer.AddStringLiteral(" body {\n");
				
				if (bgurl != null)
				{
					lexer.AddStringLiteral("  background-image: url(");
					lexer.AddStringLiteral(bgurl);
					lexer.AddStringLiteral(");\n");
				}
				
				if (bgcolor != null)
				{
					lexer.AddStringLiteral("  background-color: ");
					lexer.AddStringLiteral(bgcolor);
					lexer.AddStringLiteral(";\n");
				}
				
				if (color != null)
				{
					lexer.AddStringLiteral("  color: ");
					lexer.AddStringLiteral(color);
					lexer.AddStringLiteral(";\n");
				}
				
				lexer.AddStringLiteral(" }\n");
			}
			
			attr = body.GetAttrByName("link");
			
			if (attr != null)
			{
				AddColorRule(lexer, " :link", attr.Val);
				body.RemoveAttribute(attr);
			}
			
			attr = body.GetAttrByName("vlink");
			
			if (attr != null)
			{
				AddColorRule(lexer, " :visited", attr.Val);
				body.RemoveAttribute(attr);
			}
			
			attr = body.GetAttrByName("alink");
			
			if (attr != null)
			{
				AddColorRule(lexer, " :active", attr.Val);
				body.RemoveAttribute(attr);
			}
		}
		
		private bool NiceBody(Lexer lexer, Node doc)
		{
			Node body = doc.FindBody(lexer.Options.tt);
			
			if (body != null)
			{
				if (body.GetAttrByName("background") != null || body.GetAttrByName("bgcolor") != null || body.GetAttrByName("text") != null || body.GetAttrByName("link") != null || body.GetAttrByName("vlink") != null || body.GetAttrByName("alink") != null)
				{
					lexer.badLayout |= Report.USING_BODY;
					return false;
				}
			}
			
			return true;
		}
		
		/* create style element using rules from dictionary */
		private void CreateStyleElement(Lexer lexer, Node doc)
		{
			Node node, head, body;
			Style style;
			AttVal av;
			
			if (lexer.styles == null && NiceBody(lexer, doc))
			{
				return;
			}
			
			node = lexer.NewNode(Node.StartTag, null, 0, 0, "style");
			node.Isimplicit = true;
			
			/* insert type attribute */
			av = new AttVal(null, null, '"', "type", "text/css");
			av.Dict = AttributeTable.DefaultAttributeTable.FindAttribute(av);
			node.Attributes = av;
			
			body = doc.FindBody(lexer.Options.tt);
			
			lexer.txtstart = lexer.lexsize;
			
			if (body != null)
			{
				CleanBodyAttrs(lexer, body);
			}
			
			for (style = lexer.styles; style != null; style = style.Next)
			{
				lexer.AddCharToLexer(' ');
				lexer.AddStringLiteral(style.Tag);
				lexer.AddCharToLexer('.');
				lexer.AddStringLiteral(style.TagClass);
				lexer.AddCharToLexer(' ');
				lexer.AddCharToLexer('{');
				lexer.AddStringLiteral(style.Properties);
				lexer.AddCharToLexer('}');
				lexer.AddCharToLexer('\n');
			}
			
			lexer.txtend = lexer.lexsize;
			
			Node.InsertNodeAtEnd(node, lexer.NewNode(Node.TextNode, lexer.lexbuf, lexer.txtstart, lexer.txtend));
			
			/*
			now insert style element into document head
			
			doc is root node. search its children for html node
			the head node should be first child of html node
			*/
			
			head = doc.FindHead(lexer.Options.tt);
			
			if (head != null)
			{
				Node.InsertNodeAtEnd(head, node);
			}
		}
		
		/* ensure bidirectional links are consistent */
		private void FixNodeLinks(Node node)
		{
			Node child;
			
			if (node.Prev != null)
			{
				node.Prev.Next = node;
			}
			else
			{
				node.Parent.Content = node;
			}
			
			if (node.Next != null)
			{
				node.Next.Prev = node;
			}
			else
			{
				node.Parent.Last = node;
			}
			
			for (child = node.Content; child != null; child = child.Next)
			{
				child.Parent = node;
			}
		}
		
		/*
		used to strip child of node when
		the node has one and only one child
		*/
		private void StripOnlyChild(Node node)
		{
			Node child;
			
			child = node.Content;
			node.Content = child.Content;
			node.Last = child.Last;
			child.Content = null;
			
			for (child = node.Content; child != null; child = child.Next)
			{
				child.Parent = node;
			}
		}
		
		/* used to strip font start and end tags */
		private void DiscardContainer(Node element, MutableObject pnode)
		{
			Node node;
			Node parent = element.Parent;
			
			if (element.Content != null)
			{
				element.Last.Next = element.Next;
				
				if (element.Next != null)
				{
					element.Next.Prev = element.Last;
					element.Last.Next = element.Next;
				}
				else
				{
					parent.Last = element.Last;
				}
				
				if (element.Prev != null)
				{
					element.Content.Prev = element.Prev;
					element.Prev.Next = element.Content;
				}
				else
				{
					parent.Content = element.Content;
				}

				for (node = element.Content; node != null; node = node.Next)
				{
					node.Parent = parent;
				}
				
				pnode.Object = element.Content;
			}
			else
			{
				if (element.Next != null)
				{
					element.Next.Prev = element.Prev;
				}
				else
				{
					parent.Last = element.Prev;
				}
				
				if (element.Prev != null)
				{
					element.Prev.Next = element.Next;
				}
				else
				{
					parent.Content = element.Next;
				}
				
				pnode.Object = element.Next;
			}
			
			element.Next = null;
			element.Content = null;
		}
		
		/*
		Add style property to element, creating style
		attribute as needed and adding ; delimiter
		*/
		private void AddStyleProperty(Node node, string property)
		{
			AttVal av;
			
			for (av = node.Attributes; av != null; av = av.Next)
			{
				if (av.Attribute.Equals("style"))
				{
					break;
				}
			}
			
			/* if style attribute already exists then insert property */
			
			if (av != null)
			{
				string s;
				
				s = AddProperty(av.Val, property);
				av.Val = s;
			}
			else
			{
				/* else create new style attribute */
				av = new AttVal(node.Attributes, null, '"', "style", property);
				av.Dict = AttributeTable.DefaultAttributeTable.FindAttribute(av);
				node.Attributes = av;
			}
		}
		
		/*
		Create new string that consists of the
		combined style properties in s1 and s2
		
		To merge property lists, we build a linked
		list of property/values and insert properties
		into the list in order, merging values for
		the same property name.
		*/
		private string MergeProperties(string s1, string s2)
		{
			string s;
			StyleProp prop;
			
			prop = CreateProps(null, s1);
			prop = CreateProps(prop, s2);
			s = CreatePropString(prop);
			return s;
		}
		
		private void MergeStyles(Node node, Node child)
		{
			AttVal av;
			string s1, s2, style;
			
			for (s2 = null, av = child.Attributes; av != null; av = av.Next)
			{
				if (av.Attribute.Equals("style"))
				{
					s2 = av.Val;
					break;
				}
			}
			
			for (s1 = null, av = node.Attributes; av != null; av = av.Next)
			{
				if (av.Attribute.Equals("style"))
				{
					s1 = av.Val;
					break;
				}
			}
			
			if (s1 != null)
			{
				if (s2 != null)
				{
					/* merge styles from both */
					style = MergeProperties(s1, s2);
					av.Val = style;
				}
			}
			else if (s2 != null)
			{
				/* copy style of child */
				av = new AttVal(node.Attributes, null, '"', "style", s2);
				av.Dict = AttributeTable.DefaultAttributeTable.FindAttribute(av);
				node.Attributes = av;
			}
		}
		
		private string FontSize2Name(string size)
		{
			string[] sizes = new string[] {"60%", "70%", "80%", null, "120%", "150%", "200%"};
			string buf;
			
			if (size.Length > 0 && '0' <= size[0] && size[0] <= '6')
			{
				int n = size[0] - '0';
				return sizes[n];
			}
			
			if (size.Length > 0 && size[0] == '-')
			{
				if (size.Length > 1 && '0' <= size[1] && size[1] <= '6')
				{
					int n = size[1] - '0';
					double x;
					
					for (x = 1.0; n > 0; --n)
					{
						x *= 0.8;
					}
					
					x *= 100.0;

					buf = String.Format("{0}%", (int)x);
					
					return buf;
				}
				
				return "smaller"; /*"70%"; */
			}
			
			if (size.Length > 1 && '0' <= size[1] && size[1] <= '6')
			{
				int n = size[1] - '0';
				double x;
				
				for (x = 1.0; n > 0; --n)
				{
					x *= 1.2;
				}
				
				x *= 100.0;

				buf = String.Format("{0}%", (int)x);
				
				return buf;
			}
			
			return "larger"; /* "140%" */
		}
		
		private void AddFontFace(Node node, string face)
		{
			AddStyleProperty(node, "font-family: " + face);
		}
		
		private void AddFontSize(Node node, string size)
		{
			string val;
			
			if (size.Equals("6") && node.Tag == _tt.TagP)
			{
				node.Element = "h1";
				_tt.FindTag(node);
				return;
			}
			
			if (size.Equals("5") && node.Tag == _tt.TagP)
			{
				node.Element = "h2";
				_tt.FindTag(node);
				return;
			}
			
			if (size.Equals("4") && node.Tag == _tt.TagP)
			{
				node.Element = "h3";
				_tt.FindTag(node);
				return;
			}
			
			val = FontSize2Name(size);
			
			if (val != null)
			{
				AddStyleProperty(node, "font-size: " + val);
			}
		}
		
		private void AddFontColor(Node node, string color)
		{
			AddStyleProperty(node, "color: " + color);
		}
		
		private void AddAlign(Node node, string align)
		{
			/* force alignment value to lower case */
			AddStyleProperty(node, "text-align: " + align.ToLower());
		}
		
		/*
		add style properties to node corresponding to
		the font face, size and color attributes
		*/
		private void AddFontStyles(Node node, AttVal av)
		{
			while (av != null)
			{
				if (av.Attribute.Equals("face"))
				{
					AddFontFace(node, av.Val);
				}
				else if (av.Attribute.Equals("size"))
				{
					AddFontSize(node, av.Val);
				}
				else if (av.Attribute.Equals("color"))
				{
					AddFontColor(node, av.Val);
				}
				
				av = av.Next;
			}
		}

		/*
		Symptom: <p align=center>
		Action: <p style="text-align: center">
		*/
		private void TextAlign(Lexer lexer, Node node)
		{
			AttVal av, prev;
			
			prev = null;
			
			for (av = node.Attributes; av != null; av = av.Next)
			{
				if (av.Attribute.Equals("align"))
				{
					if (prev != null)
					{
						prev.Next = av.Next;
					}
					else
					{
						node.Attributes = av.Next;
					}
					
					if (av.Val != null)
					{
						AddAlign(node, av.Val);
					}
					
					break;
				}
				
				prev = av;
			}
		}
		
		/*
		The clean up rules use the pnode argument to return the
		next node when the orignal node has been deleted
		*/
		
		/*
		Symptom: <dir> <li> where <li> is only child
		Action: coerce <dir> <li> to <div> with indent.
		*/
		
		private bool Dir2Div(Lexer lexer, Node node, MutableObject pnode)
		{
			Node child;
			
			if (node.Tag == _tt.TagDir || node.Tag == _tt.TagUl || node.Tag == _tt.TagOl)
			{
				child = node.Content;
				
				if (child == null)
				{
					return false;
				}
				
				/* check child has no peers */
				
				if (child.Next != null)
				{
					return false;
				}
				
				if (child.Tag != _tt.TagLi)
				{
					return false;
				}
				
				if (!child.Isimplicit)
				{
					return false;
				}
				
				/* coerce dir to div */
				
				node.Tag = _tt.TagDiv;
				node.Element = "div";
				AddStyleProperty(node, "margin-left: 2em");
				StripOnlyChild(node);
				return true;
			}
			
			return false;
		}
		
		/*
		Symptom: <center>
		Action: replace <center> by <div style="text-align: center">
		*/
		
		private bool Center2Div(Lexer lexer, Node node, MutableObject pnode)
		{
			if (node.Tag == _tt.TagCenter)
			{
				if (lexer.Options.DropFontTags)
				{
					if (node.Content != null)
					{
						Node last = node.Last;
						Node parent = node.Parent;
						
						DiscardContainer(node, pnode);
						
						node = lexer.InferredTag("br");
						
						if (last.Next != null)
						{
							last.Next.Prev = node;
						}
						
						node.Next = last.Next;
						last.Next = node;
						node.Prev = last;
						
						if (parent.Last == last)
						{
							parent.Last = node;
						}
						
						node.Parent = parent;
					}
					else
					{
						Node prev = node.Prev;
						Node next = node.Next;
						Node parent = node.Parent;
						DiscardContainer(node, pnode);
						
						node = lexer.InferredTag("br");
						node.Next = next;
						node.Prev = prev;
						node.Parent = parent;
						
						if (next != null)
						{
							next.Prev = node;
						}
						else
						{
							parent.Last = node;
						}
						
						if (prev != null)
						{
							prev.Next = node;
						}
						else
						{
							parent.Content = node;
						}
					}
					
					return true;
				}
				node.Tag = _tt.TagDiv;
				node.Element = "div";
				AddStyleProperty(node, "text-align: center");
				return true;
			}
			
			return false;
		}
		
		/*
		Symptom <div><div>...</div></div>
		Action: merge the two divs
		
		This is useful after nested <dir>s used by Word
		for indenting have been converted to <div>s
		*/
		private bool MergeDivs(Lexer lexer, Node node, MutableObject pnode)
		{
			Node child;
			
			if (node.Tag != _tt.TagDiv)
			{
				return false;
			}
			
			child = node.Content;
			
			if (child == null)
			{
				return false;
			}
			
			if (child.Tag != _tt.TagDiv)
			{
				return false;
			}
			
			if (child.Next != null)
			{
				return false;
			}
			
			MergeStyles(node, child);
			StripOnlyChild(node);
			return true;
		}
		
		/*
		Symptom: <ul><li><ul>...</ul></li></ul>
		Action: discard outer list
		*/
		private bool NestedList(Lexer lexer, Node node, MutableObject pnode)
		{
			Node child, list;

			if (node.Tag == _tt.TagUl || node.Tag == _tt.TagOl)
			{
				child = node.Content;
				
				if (child == null)
				{
					return false;
				}
				
				/* check child has no peers */
				
				if (child.Next != null)
				{
					return false;
				}
				
				list = child.Content;
				
				if (list == null)
				{
					return false;
				}
				
				if (list.Tag != node.Tag)
				{
					return false;
				}
				
				pnode.Object = node.Next;
				
				/* move inner list node into position of outer node */
				list.Prev = node.Prev;
				list.Next = node.Next;
				list.Parent = node.Parent;
				FixNodeLinks(list);
				
				/* get rid of outer ul and its li */
				child.Content = null;
				node.Content = null;
				node.Next = null;
				
				/*
				If prev node was a list the chances are this node
				should be appended to that list. Word has no way of
				recognizing nested lists and just uses indents
				*/
				
				if (list.Prev != null)
				{
					node = list;
					list = node.Prev;
					
					if (list.Tag == _tt.TagUl || list.Tag == _tt.TagOl)
					{
						list.Next = node.Next;
						
						if (list.Next != null)
						{
							list.Next.Prev = list;
						}
						
						child = list.Last; /* <li> */
						
						node.Parent = child;
						node.Next = null;
						node.Prev = child.Last;
						FixNodeLinks(node);
					}
				}
				
				CleanNode(lexer, node);
				return true;
			}
			
			return false;
		}
		
		/*
		Symptom: the only child of a block-level element is a
		presentation element such as B, I or FONT
		
		Action: add style "font-weight: bold" to the block and
		strip the <b> element, leaving its children.
		
		example:
		
		<p>
		<b><font face="Arial" size="6">Draft Recommended Practice</font></b>
		</p>
		
		becomes:
		
		<p style="font-weight: bold; font-family: Arial; font-size: 6">
		Draft Recommended Practice
		</p>
		
		This code also replaces the align attribute by a style attribute.
		However, to avoid CSS problems with Navigator 4, this isn't done
		for the elements: caption, tr and table
		*/
		private bool BlockStyle(Lexer lexer, Node node, MutableObject pnode)
		{
			Node child;
			
			if ((node.Tag.Model & (ContentModel.Block | ContentModel.List | ContentModel.Deflist | ContentModel.Table)) != 0)
			{
				if (node.Tag != _tt.tagTable && node.Tag != _tt.TagTr && node.Tag != _tt.TagLi)
				{
					/* check for align attribute */
					if (node.Tag != _tt.TagCaption)
					{
						TextAlign(lexer, node);
					}
					
					child = node.Content;
					
					if (child == null)
					{
						return false;
					}
					
					/* check child has no peers */
					
					if (child.Next != null)
					{
						return false;
					}
					
					if (child.Tag == _tt.TagB)
					{
						MergeStyles(node, child);
						AddStyleProperty(node, "font-weight: bold");
						StripOnlyChild(node);
						return true;
					}
					
					if (child.Tag == _tt.TagI)
					{
						MergeStyles(node, child);
						AddStyleProperty(node, "font-style: italic");
						StripOnlyChild(node);
						return true;
					}
					
					if (child.Tag == _tt.TagFont)
					{
						MergeStyles(node, child);
						AddFontStyles(node, child.Attributes);
						StripOnlyChild(node);
						return true;
					}
				}
			}
			
			return false;
		}
		
		/* the only child of table cell or an inline element such as em */
		private bool InlineStyle(Lexer lexer, Node node, MutableObject pnode)
		{
			Node child;
			
			if (node.Tag != _tt.TagFont && (node.Tag.Model & (ContentModel.Inline | ContentModel.Row)) != 0)
			{
				child = node.Content;
				
				if (child == null)
				{
					return false;
				}
				
				/* check child has no peers */
				
				if (child.Next != null)
				{
					return false;
				}
				
				if (child.Tag == _tt.TagB && lexer.Options.LogicalEmphasis)
				{
					MergeStyles(node, child);
					AddStyleProperty(node, "font-weight: bold");
					StripOnlyChild(node);
					return true;
				}
				
				if (child.Tag == _tt.TagI && lexer.Options.LogicalEmphasis)
				{
					MergeStyles(node, child);
					AddStyleProperty(node, "font-style: italic");
					StripOnlyChild(node);
					return true;
				}
				
				if (child.Tag == _tt.TagFont)
				{
					MergeStyles(node, child);
					AddFontStyles(node, child.Attributes);
					StripOnlyChild(node);
					return true;
				}
			}
			
			return false;
		}
		
		/*
		Replace font elements by span elements, deleting
		the font element's attributes and replacing them
		by a single style attribute.
		*/
		private bool Font2Span(Lexer lexer, Node node, MutableObject pnode)
		{
			AttVal av, style, next;
			
			if (node.Tag == _tt.TagFont)
			{
				if (lexer.Options.DropFontTags)
				{
					DiscardContainer(node, pnode);
					return false;
				}
				
				/* if FONT is only child of parent element then leave alone */
				if (node.Parent.Content == node && node.Next == null)
				{
					return false;
				}
				
				AddFontStyles(node, node.Attributes);
				
				/* extract style attribute and free the rest */
				av = node.Attributes;
				style = null;
				
				while (av != null)
				{
					next = av.Next;
					
					if (av.Attribute.Equals("style"))
					{
						av.Next = null;
						style = av;
					}
					
					av = next;
				}
				
				node.Attributes = style;
				
				node.Tag = _tt.TagSpan;
				node.Element = "span";
				
				return true;
			}
			
			return false;
		}
		
		/*
		Applies all matching rules to a node.
		*/
		private Node CleanNode(Lexer lexer, Node node)
		{
			Node next = null;
			MutableObject o = new MutableObject();
			bool b = false;
			
			for (next = node; node.IsElement; node = next)
			{
				o.Object = next;
				
				b = Dir2Div(lexer, node, o);
				next = (Node) o.Object;
				if (b)
				{
					continue;
				}
				
				b = NestedList(lexer, node, o);
				next = (Node) o.Object;
				if (b)
				{
					continue;
				}
				
				b = Center2Div(lexer, node, o);
				next = (Node) o.Object;
				if (b)
				{
					continue;
				}

				b = MergeDivs(lexer, node, o);
				next = (Node) o.Object;
				if (b)
				{
					continue;
				}
				
				b = BlockStyle(lexer, node, o);
				next = (Node) o.Object;
				if (b)
				{
					continue;
				}
				
				b = InlineStyle(lexer, node, o);
				next = (Node) o.Object;
				if (b)
				{
					continue;
				}
				
				b = Font2Span(lexer, node, o);
				next = (Node) o.Object;
				if (b)
				{
					continue;
				}

				break;
			}
			
			return next;
		}
		
		private Node CreateStyleProperties(Lexer lexer, Node node)
		{
			Node child;
			
			if (node.Content != null)
			{
				for (child = node.Content; child != null; child = child.Next)
				{
					child = CreateStyleProperties(lexer, child);
				}
			}
			
			return CleanNode(lexer, node);
		}
		
		private void DefineStyleRules(Lexer lexer, Node node)
		{
			Node child;
			
			if (node.Content != null)
			{
				for (child = node.Content; child != null; child = child.Next)
				{
					DefineStyleRules(lexer, child);
				}
			}
			
			Style2Rule(lexer, node);
		}
		
		public virtual void CleanTree(Lexer lexer, Node doc)
		{
			doc = CreateStyleProperties(lexer, doc);
			
			if (!lexer.Options.MakeClean)
			{
				DefineStyleRules(lexer, doc);
				CreateStyleElement(lexer, doc);
			}
		}
		
		/* simplifies <b><b> ... </b> ...</b> etc. */
		public virtual void NestedEmphasis(Node node)
		{
			MutableObject o = new MutableObject();
			Node next;
			
			while (node != null)
			{
				next = node.Next;
				
				if ((node.Tag == _tt.TagB || node.Tag == _tt.TagI) && node.Parent != null && node.Parent.Tag == node.Tag)
				{
					/* strip redundant inner element */
					o.Object = next;
					DiscardContainer(node, o);
					next = (Node) o.Object;
					node = next;
					continue;
				}
				
				if (node.Content != null)
				{
					NestedEmphasis(node.Content);
				}
				
				node = next;
			}
		}
		
		/* replace i by em and b by strong */
		public virtual void EmFromI(Node node)
		{
			while (node != null)
			{
				if (node.Tag == _tt.TagI)
				{
					node.Element = _tt.TagEm.Name;
					node.Tag = _tt.TagEm;
				}
				else if (node.Tag == _tt.TagB)
				{
					node.Element = _tt.TagStrong.Name;
					node.Tag = _tt.TagStrong;
				}
				
				if (node.Content != null)
				{
					EmFromI(node.Content);
				}
				
				node = node.Next;
			}
		}
		
		/*
		Some people use dir or ul without an li
		to indent the content. The pattern to
		look for is a list with a single implicit
		li. This is recursively replaced by an
		implicit blockquote.
		*/
		public virtual void List2BQ(Node node)
		{
			while (node != null)
			{
				if (node.Content != null)
				{
					List2BQ(node.Content);
				}
				
				if (node.Tag != null && node.Tag.Parser == ParserImpl.ParseList && node.HasOneChild() && node.Content.Isimplicit)
				{
					StripOnlyChild(node);
					node.Element = _tt.TagBlockquote.Name;
					node.Tag = _tt.TagBlockquote;
					node.Isimplicit = true;
				}
				
				node = node.Next;
			}
		}
		
		/*
		Replace implicit blockquote by div with an indent
		taking care to reduce nested blockquotes to a single
		div with the indent set to match the nesting depth
		*/
		public virtual void BQ2Div(Node node)
		{
			int indent;
			string indent_buf;
			
			while (node != null)
			{
				if (node.Tag == _tt.TagBlockquote && node.Isimplicit)
				{
					indent = 1;
					
					while (node.HasOneChild() && node.Content.Tag == _tt.TagBlockquote && node.Isimplicit)
					{
						++indent;
						StripOnlyChild(node);
					}
					
					if (node.Content != null)
					{
						BQ2Div(node.Content);
					}
					
					indent_buf = "margin-left: " + (2 * indent).ToString() + "em";
					
					node.Element = _tt.TagDiv.Name;
					node.Tag = _tt.TagDiv;
					node.AddAttribute("style", indent_buf);
				}
				else if (node.Content != null)
				{
					BQ2Div(node.Content);
				}
				
				
				node = node.Next;
			}
		}
		
		/* node is <![if ...]> prune up to <![endif]> */
		public virtual Node PruneSection(Lexer lexer, Node node)
		{
			for (; ; )
			{
				/* discard node and returns next */
				node = Node.DiscardElement(node);
				
				if (node == null)
					return null;
				
				if (node.Type == Node.SectionTag)
				{
					if ((Lexer.GetString(node.Textarray, node.Start, 2)).Equals("if"))
					{
						node = PruneSection(lexer, node);
						continue;
					}
					
					if ((Lexer.GetString(node.Textarray, node.Start, 5)).Equals("endif"))
					{
						node = Node.DiscardElement(node);
						break;
					}
				}
			}
			
			return node;
		}
		
		public virtual void DropSections(Lexer lexer, Node node)
		{
			while (node != null)
			{
				if (node.Type == Node.SectionTag)
				{
					/* prune up to matching endif */
					if ((Lexer.GetString(node.Textarray, node.Start, 2)).Equals("if"))
					{
						node = PruneSection(lexer, node);
						continue;
					}
					
					/* discard others as well */
					node = Node.DiscardElement(node);
					continue;
				}
				
				if (node.Content != null)
				{
					DropSections(lexer, node.Content);
				}
				
				node = node.Next;
			}
		}
		
		public virtual void PurgeAttributes(Node node)
		{
			AttVal attr = node.Attributes;
			AttVal next = null;
			AttVal prev = null;
			
			while (attr != null)
			{
				next = attr.Next;
				
				/* special check for class="Code" denoting pre text */
				if (attr.Attribute != null && attr.Val != null && attr.Attribute.Equals("class") && attr.Val.Equals("Code"))
				{
					prev = attr;
				}
				else if (attr.Attribute != null && (attr.Attribute.Equals("class") || attr.Attribute.Equals("style") || attr.Attribute.Equals("lang") || attr.Attribute.StartsWith("x:") || ((attr.Attribute.Equals("height") || attr.Attribute.Equals("width")) && (node.Tag == _tt.TagTd || node.Tag == _tt.TagTr || node.Tag == _tt.TagTh))))
				{
					if (prev != null)
					{
						prev.Next = next;
					}
					else
					{
						node.Attributes = next;
					}
				}
				else
				{
					prev = attr;
				}
				
				attr = next;
			}
		}

		/* Word2000 uses span excessively, so we strip span out */
		public virtual Node StripSpan(Lexer lexer, Node span)
		{
			Node node;
			Node prev = null;
			Node content;
			
			/*
			deal with span elements that have content
			by splicing the content in place of the span
			after having processed it
			*/
			
			CleanWord2000(lexer, span.Content);
			content = span.Content;
			
			if (span.Prev != null)
			{
				prev = span.Prev;
			}
			else if (content != null)
			{
				node = content;
				content = content.Next;
				Node.RemoveNode(node);
				Node.InsertNodeBeforeElement(span, node);
				prev = node;
			}
			
			while (content != null)
			{
				node = content;
				content = content.Next;
				Node.RemoveNode(node);
				Node.InsertNodeAfterElement(prev, node);
				prev = node;
			}
			
			if (span.Next == null)
			{
				span.Parent.Last = prev;
			}
			
			node = span.Next;
			span.Content = null;
			Node.DiscardElement(span);
			return node;
		}
		
		/* map non-breaking spaces to regular spaces */
		private void NormalizeSpaces(Lexer lexer, Node node)
		{
			while (node != null)
			{
				if (node.Content != null)
				{
					NormalizeSpaces(lexer, node.Content);
				}
				
				if (node.Type == Node.TextNode)
				{
					int i;
					MutableInteger c = new MutableInteger();
					int p = node.Start;
					
					for (i = node.Start; i < node.End; ++i)
					{
						c.Val = (int) node.Textarray[i];
						
						/* look for UTF-8 multibyte character */
						if (c.Val > 0x7F)
						{
							i += PPrint.GetUTF8(node.Textarray, i, c);
						}
						
						if (c.Val == 160)
						{
							c.Val = ' ';
						}
						
						p = PPrint.PutUTF8(node.Textarray, p, c.Val);
					}
				}
				
				node = node.Next;
			}
		}
		
		/*
		This is a major clean up to strip out all the extra stuff you get
		when you save as web page from Word 2000. It doesn't yet know what
		to do with VML tags, but these will appear as errors unless you
		declare them as new tags, such as o:p which needs to be declared
		as inline.
		*/
		public virtual void CleanWord2000(Lexer lexer, Node node)
		{
			/* used to a list from a sequence of bulletted p's */
			Node list = null;
			
			while (node != null)
			{
				/* discard Word's style verbiage */
				if (node.Tag == _tt.TagStyle || node.Tag == _tt.TagMeta || node.Type == Node.CommentTag)
				{
					node = Node.DiscardElement(node);
					continue;
				}
				
				/* strip out all span tags Word scatters so liberally! */
				if (node.Tag == _tt.TagSpan)
				{
					node = StripSpan(lexer, node);
					continue;
				}
				
				/* get rid of Word's xmlns attributes */
				if (node.Tag == _tt.TagHtml)
				{
					/* check that it's a Word 2000 document */
					if (node.GetAttrByName("xmlns:o") == null)
					{
						return;
					}
				}
				
				if (node.Tag == _tt.TagLink)
				{
					AttVal attr = node.GetAttrByName("rel");
					
					if (attr != null && attr.Val != null && attr.Val.Equals("File-List"))
					{
						node = Node.DiscardElement(node);
						continue;
					}
				}
				
				/* discard empty paragraphs */
				if (node.Content == null && node.Tag == _tt.TagP)
				{
					node = Node.DiscardElement(node);
					continue;
				}
				
				if (node.Tag == _tt.TagP)
				{
					AttVal attr = node.GetAttrByName("class");
					
					/* map sequence of <p class="MsoListBullet"> to <ul>...</ul> */
					if (attr != null && attr.Val != null && attr.Val.Equals("MsoListBullet"))
					{
						Node.CoerceNode(lexer, node, _tt.TagLi);
						
						if (list == null || list.Tag != _tt.TagUl)
						{
							list = lexer.InferredTag("ul");
							Node.InsertNodeBeforeElement(node, list);
						}
						
						PurgeAttributes(node);
						
						if (node.Content != null)
						{
							CleanWord2000(lexer, node.Content);
						}
						
						/* remove node and append to contents of list */
						Node.RemoveNode(node);
						Node.InsertNodeAtEnd(list, node);
						node = list.Next;
					}
					else if (attr != null && attr.Val != null && attr.Val.Equals("Code"))
					{
						/* map sequence of <p class="Code"> to <pre>...</pre> */
						Node br = lexer.NewLineNode();
						NormalizeSpaces(lexer, node);
						
						if (list == null || list.Tag != _tt.TagPre)
						{
							list = lexer.InferredTag("pre");
							Node.InsertNodeBeforeElement(node, list);
						}
						
						/* remove node and append to contents of list */
						Node.RemoveNode(node);
						Node.InsertNodeAtEnd(list, node);
						StripSpan(lexer, node);
						Node.InsertNodeAtEnd(list, br);
						node = list.Next;
					}
					else
					{
						list = null;
					}
				}
				else
				{
					list = null;
				}
				
				/* strip out style and class attributes */
				if (node.Type == Node.StartTag || node.Type == Node.StartEndTag)
				{
					PurgeAttributes(node);
				}
				
				if (node.Content != null)
				{
					CleanWord2000(lexer, node.Content);
				}
				
				node = node.Next;
			}
		}
		
		public virtual bool IsWord2000(Node root, TagTable tt)
		{
			Node html = root.FindHtml(tt);
			
			return (html != null && html.GetAttrByName("xmlns:o") != null);
		}

		private int classNum = 1;
		private TagTable _tt;
	}
}