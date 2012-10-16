using System;
using TidyNet.Dom;

namespace TidyNet
{	
	/// <summary>
	/// Attribute/Value linked list node
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
	internal class AttVal : ICloneable
	{
		public AttVal()
		{
			_next = null;
			_dict = null;
			_asp = null;
			_php = null;
			_delim = 0;
			_attribute = null;
			_val = null;
		}
		
		public AttVal(AttVal next, Attribute dict, int delim, string attribute, string val)
		{
			_next = next;
			_dict = dict;
			_asp = null;
			_php = null;
			_delim = delim;
			_attribute = attribute;
			_val = val;
		}
		
		public AttVal(AttVal next, Attribute dict, Node asp, Node php, int delim, string attribute, string val)
		{
			_next = next;
			_dict = dict;
			_asp = asp;
			_php = php;
			_delim = delim;
			_attribute = attribute;
			_val = val;
		}
		
		virtual public bool BoolAttribute
		{
			get
			{
				Attribute attribute = _dict;
				if (attribute != null)
				{
					if (attribute.AttrCheck == AttrCheckImpl.CheckBool)
					{
						return true;
					}
				}
				
				return false;
			}
			
		}
		
		public AttVal Next
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

		public Attribute Dict
		{
			get
			{
				return _dict;
			}
			set
			{
				_dict = value;
			}
		}

		public Node Asp
		{
			get
			{
				return _asp;
			}
			set
			{
				_asp = value;
			}
		}

		public Node Php
		{
			get
			{
				return _php;
			}
			set
			{
				_php = value;
			}
		}

		public int Delim
		{
			get
			{
				return _delim;
			}
			set
			{
				_delim = value;
			}
		}

		public string Attribute
		{
			get
			{
				return _attribute;
			}
			set
			{
				_attribute = value;
			}
		}

		public string Val
		{
			get
			{
				return _val;
			}
			set
			{
				_val = value;
			}
		}
		
		public object Clone()
		{
			AttVal av = new AttVal();
			if (Next != null)
			{
				av.Next = (AttVal)Next.Clone();
			}
			if (Attribute != null)
			{
				av.Attribute = Attribute;
			}
			if (Val != null)
			{
				av.Val = Val;
			}
			av.Delim = Delim;
			if (Asp != null)
			{
				av.Asp = (Node) Asp.Clone();
			}
			if (Php != null)
			{
				av.Php = (Node) Php.Clone();
			}
			av.Dict = AttributeTable.DefaultAttributeTable.FindAttribute(this);
			return av;
		}

		/* ignore unknown attributes for proprietary elements */
		public virtual Attribute CheckAttribute(Lexer lexer, Node node)
		{
			TagTable tt = lexer.Options.tt;
			
			if (Asp == null && Php == null)
			{
				CheckUniqueAttribute(lexer, node);
			}
			
			Attribute attribute = Dict;
			if (attribute != null)
			{
				/* title is vers 2.0 for A and LINK otherwise vers 4.0 */
				if (attribute == AttributeTable.AttrTitle && (node.Tag == tt.TagA || node.Tag == tt.TagLink))
				{
					lexer.versions &= HtmlVersion.All;
				}
				else if ((attribute.Versions & HtmlVersion.Xml) != 0)
				{
					if (!(lexer.Options.XmlTags || lexer.Options.XmlOut))
					{
						Report.AttrError(lexer, node, Attribute, Report.XML_ATTRIBUTE_VALUE);
					}
				}
				else
				{
					lexer.versions &= attribute.Versions;
				}
				
				if (attribute.AttrCheck != null)
				{
					attribute.AttrCheck.Check(lexer, node, this);
				}
			}
			else if (!lexer.Options.XmlTags && !(node.Tag == null) && _asp == null && !(node.Tag != null && ((node.Tag.Versions & HtmlVersion.Proprietary) != HtmlVersion.Unknown)))
			{
				Report.AttrError(lexer, node, Attribute, Report.UNKNOWN_ATTRIBUTE);
			}

			return attribute;
		}
		
		/*
		the same attribute name can't be used
		more than once in each element
		*/
		public virtual void CheckUniqueAttribute(Lexer lexer, Node node)
		{
			AttVal attr;
			int count = 0;
			
			for (attr = Next; attr != null; attr = attr.Next)
			{
				if (Attribute != null && attr.Attribute != null && attr.Asp == null && attr.Php == null && String.Compare(Attribute, attr.Attribute) == 0)
				{
					++count;
				}
			}
			
			if (count > 0)
			{
				Report.AttrError(lexer, node, Attribute, Report.REPEATED_ATTRIBUTE);
			}
		}

		protected virtual internal IAttr Adapter
		{
			get
			{
				if (_adapter == null)
				{
					_adapter = new DomAttrImpl(this);
				}
				return _adapter;
			}
			set
			{
				_adapter = value;
			}
		}

		private IAttr _adapter = null;
		private AttVal _next;
		private Attribute _dict;
		private Node _asp;
		private Node _php;
		private int _delim;
		private string _attribute;
		private string _val;		
	}
}