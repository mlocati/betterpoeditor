using System;
using TidyNet.Dom;

namespace TidyNet
{
	/// <summary>
	/// DomAttrImpl
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
	internal class DomAttrImpl : DomNodeImpl, IAttr
	{
		protected internal DomAttrImpl(AttVal adaptee) : base(null)
		{
			_attValAdaptee = adaptee;
		}
		
		override public string NodeValue
		{
			get
			{
				return Value;
			}
			set
			{
				Value = value;
			}
		}

		override public string NodeName
		{
			get
			{
				return Name;
			}
		}

		override public Dom.NodeType NodeType
		{
			get
			{
				return NodeType.ATTRIBUTE_NODE;
			}
		}

		override public INode ParentNode
		{
			get
			{
				return null;
			}
		}

		override public INodeList ChildNodes
		{
			get
			{
				// NOT SUPPORTED
				return null;
			}
		}

		override public INode FirstChild
		{
			get
			{
				// NOT SUPPORTED
				return null;
			}
		}

		override public INode LastChild
		{
			get
			{
				// NOT SUPPORTED
				return null;
			}
		}

		override public INode PreviousSibling
		{
			get
			{
				return null;
			}
		}

		override public INode NextSibling
		{
			get
			{
				return null;
			}
		}

		override public INamedNodeMap Attributes
		{
			get
			{
				return null;
			}
		}

		override public IDocument OwnerDocument
		{
			get
			{
				return null;
			}
		}

		virtual public string Name
		{
			get
			{
				return _attValAdaptee.Attribute;
			}
		}

		virtual public bool Specified
		{
			get
			{
				return true;
			}
		}

		/// <summary> Returns value of this attribute.  If this attribute has a null value,
		/// then the attribute name is returned instead.
		/// Thanks to Brett Knights <brett@knightsofthenet.com> for this fix.
		/// </summary>
		virtual public string Value
		{
			get
			{
				return (_attValAdaptee.Val == null) ? _attValAdaptee.Attribute : _attValAdaptee.Val;
			}
			set
			{
				_attValAdaptee.Val = value;
			}
		}

		/// <summary> DOM2 - not implemented.
		/// </summary>
		virtual public IElement OwnerElement
		{
			get
			{
				return null;
			}
		}
		
		public override INode InsertBefore(INode newChild, INode refChild)
		{
			throw new DomException(DomException.NoModificationAllowed, "Not supported");
		}
		
		public override INode ReplaceChild(INode newChild, INode oldChild)
		{
			throw new DomException(DomException.NoModificationAllowed, "Not supported");
		}
		
		public override INode RemoveChild(INode oldChild)
		{
			throw new DomException(DomException.NoModificationAllowed, "Not supported");
		}
		
		public override INode AppendChild(INode newChild)
		{
			throw new DomException(DomException.NoModificationAllowed, "Not supported");
		}
		
		public override bool HasChildNodes()
		{
			return false;
		}
		
		public override INode CloneNode(bool deep)
		{
			return null;
		}

		public AttVal AttValAdaptee
		{
			get
			{
				return _attValAdaptee;
			}
		}

		private AttVal _attValAdaptee;
	}
}