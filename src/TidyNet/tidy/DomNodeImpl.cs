using System;
using TidyNet.Dom;

namespace TidyNet
{
	/// <summary>
	/// DomNodeImpl
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
	/// <version>1.4, 1999/09/04 DOM support</version>
	/// <version>1.5, 1999/10/23 Tidy Release 27 Sep 1999</version>
	/// <version>1.6, 1999/11/01 Tidy Release 22 Oct 1999</version>
	/// <version>1.7, 1999/12/06 Tidy Release 30 Nov 1999</version>
	/// <version>1.8, 2000/01/22 Tidy Release 13 Jan 2000</version>
	/// <version>1.9, 2000/06/03 Tidy Release 30 Apr 2000</version>
	/// <version>1.10, 2000/07/22 Tidy Release 8 Jul 2000</version>
	/// <version>1.11, 2000/08/16 Tidy Release 4 Aug 2000</version>
	internal class DomNodeImpl : INode
	{
		protected internal DomNodeImpl(Node adaptee)
		{
			_adaptee = adaptee;
		}
		
		public Node Adaptee
		{
			get
			{
				return _adaptee;
			}
			set
			{
				_adaptee = value;
			}
		}
		
		virtual public string NodeValue
		{
			get
			{
				string val = String.Empty; //BAK 10/10/2000 replaced null
				if (Adaptee.Type == Node.TextNode || Adaptee.Type == Node.CDATATag || Adaptee.Type == Node.CommentTag || Adaptee.Type == Node.ProcInsTag)
				{
					if (Adaptee.Textarray != null && Adaptee.Start < Adaptee.End)
					{
						val = Lexer.GetString(Adaptee.Textarray, Adaptee.Start, Adaptee.End - Adaptee.Start);
					}
				}
				return val;
			}
			
			set
			{
				if (Adaptee.Type == Node.TextNode || Adaptee.Type == Node.CDATATag || Adaptee.Type == Node.CommentTag || Adaptee.Type == Node.ProcInsTag)
				{
					byte[] textarray = Lexer.GetBytes(value);
					Adaptee.Textarray = textarray;
					Adaptee.Start = 0;
					Adaptee.End = textarray.Length;
				}
			}
		}
		
		virtual public string NodeName
		{
			get
			{
				return Adaptee.Element;
			}
		}

		virtual public Dom.NodeType NodeType
		{
			get
			{
				switch (Adaptee.Type)
				{
				case Node.RootNode: 
					return NodeType.DOCUMENT_NODE;

				case Node.DocTypeTag: 
					return NodeType.DOCUMENT_TYPE_NODE;

				case Node.CommentTag: 
					return NodeType.COMMENT_NODE;

				case Node.ProcInsTag: 
					return NodeType.PROCESSING_INSTRUCTION_NODE;

				case Node.TextNode: 
					return NodeType.TEXT_NODE;

				case Node.CDATATag: 
					return NodeType.CDATA_SECTION_NODE;

				case Node.StartTag: 
				case Node.StartEndTag: 
					return NodeType.ELEMENT_NODE;

				default:
					return NodeType.ELEMENT_NODE;
				}
			}
		}

		virtual public INode ParentNode
		{
			get
			{
				if (Adaptee.Parent != null)
				{
					return Adaptee.Parent.Adapter;
				}
				else
				{
					return null;
				}
			}
		}
		
		virtual public INodeList ChildNodes
		{
			get
			{
				return new DomNodeListImpl(Adaptee);
			}
		}

		virtual public INode FirstChild
		{
			get
			{
				if (Adaptee.Content != null)
				{
					return Adaptee.Content.Adapter;
				}
				else
				{
					return null;
				}
			}
		}

		virtual public INode LastChild
		{
			get
			{
				if (Adaptee.Last != null)
				{
					return Adaptee.Last.Adapter;
				}
				else
				{
					return null;
				}
			}
		}

		virtual public INode PreviousSibling
		{
			get
			{
				if (Adaptee.Prev != null)
				{
					return Adaptee.Prev.Adapter;
				}
				else
				{
					return null;
				}
			}
		}
		
		virtual public INode NextSibling
		{
			get
			{
				if (Adaptee.Next != null)
				{
					return Adaptee.Next.Adapter;
				}
				else
				{
					return null;
				}
			}
		}
		
		virtual public INamedNodeMap Attributes
		{
			get
			{
				return new DomAttrMapImpl(Adaptee.Attributes);
			}
		}

		virtual public IDocument OwnerDocument
		{
			get
			{
				Node node;
				
				node = this.Adaptee;
				if (node != null && node.Type == Node.RootNode)
				{
					return null;
				}
				
				for (node = this.Adaptee; node != null && node.Type != Node.RootNode; node = node.Parent)
				{
					;
				}
				
				if (node != null)
				{
					return (IDocument) node.Adapter;
				}
				else
				{
					return null;
				}
			}
		}
		
		/// <summary> DOM2 - not implemented.</summary>
		virtual public string NamespaceURI
		{
			get
			{
				return null;
			}
		}
		
		/// <summary> DOM2 - not implemented.</summary>
		virtual public string Prefix
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		/// <summary> DOM2 - not implemented.</summary>
		virtual public string LocalName
		{
			get
			{
				return null;
			}
		}

		public virtual INode InsertBefore(INode newChild, INode refChild)
		{
			// TODO - handle newChild already in tree
			
			if (newChild == null)
			{
				return null;
			}
			if (!(newChild is DomNodeImpl))
			{
				throw new DomException(DomException.WrongDocument, "newChild not instanceof DomNodeImpl");
			}
			DomNodeImpl newCh = (DomNodeImpl) newChild;
			
			if (this.Adaptee.Type == Node.RootNode)
			{
				if (newCh.Adaptee.Type != Node.DocTypeTag && newCh.Adaptee.Type != Node.ProcInsTag)
				{
					throw new DomException(DomException.HierarchyRequest, "newChild cannot be a child of this node");
				}
			}
			else if (this.Adaptee.Type == Node.StartTag)
			{
				if (newCh.Adaptee.Type != Node.StartTag && newCh.Adaptee.Type != Node.StartEndTag && newCh.Adaptee.Type != Node.CommentTag && newCh.Adaptee.Type != Node.TextNode && newCh.Adaptee.Type != Node.CDATATag)
				{
					throw new DomException(DomException.HierarchyRequest, "newChild cannot be a child of this node");
				}
			}
			if (refChild == null)
			{
				Node.InsertNodeAtEnd(this.Adaptee, newCh.Adaptee);
				if (this.Adaptee.Type == Node.StartEndTag)
				{
					this.Adaptee.Type = Node.StartTag;
				}
			}
			else
			{
				Node refNode = this.Adaptee.Content;
				while (refNode != null)
				{
					if (refNode.Adapter == refChild)
					{
						break;
					}
					refNode = refNode.Next;
				}
				if (refNode == null)
				{
					throw new DomException(DomException.NotFound, "refChild not found");
				}
				Node.InsertNodeBeforeElement(refNode, newCh.Adaptee);
			}
			return newChild;
		}
		
		public virtual INode ReplaceChild(INode newChild, INode oldChild)
		{
			// TODO - handle newChild already in tree
			
			if (newChild == null)
			{
				return null;
			}
			if (!(newChild is DomNodeImpl))
			{
				throw new DomException(DomException.WrongDocument, "newChild not instanceof DomNodeImpl");
			}
			DomNodeImpl newCh = (DomNodeImpl) newChild;
			
			if (this.Adaptee.Type == Node.RootNode)
			{
				if (newCh.Adaptee.Type != Node.DocTypeTag && newCh.Adaptee.Type != Node.ProcInsTag)
				{
					throw new DomException(DomException.HierarchyRequest, "newChild cannot be a child of this node");
				}
			}
			else if (this.Adaptee.Type == Node.StartTag)
			{
				if (newCh.Adaptee.Type != Node.StartTag && newCh.Adaptee.Type != Node.StartEndTag && newCh.Adaptee.Type != Node.CommentTag && newCh.Adaptee.Type != Node.TextNode && newCh.Adaptee.Type != Node.CDATATag)
				{
					throw new DomException(DomException.HierarchyRequest, "newChild cannot be a child of this node");
				}
			}
			if (oldChild == null)
			{
				throw new DomException(DomException.NotFound, "oldChild not found");
			}
			else
			{
				Node n;
				Node refNode = this.Adaptee.Content;
				while (refNode != null)
				{
					if (refNode.Adapter == oldChild)
					{
						break;
					}
					refNode = refNode.Next;
				}
				if (refNode == null)
				{
					throw new DomException(DomException.NotFound, "oldChild not found");
				}
				newCh.Adaptee.Next = refNode.Next;
				newCh.Adaptee.Prev = refNode.Prev;
				newCh.Adaptee.Last = refNode.Last;
				newCh.Adaptee.Parent = refNode.Parent;
				newCh.Adaptee.Content = refNode.Content;
				if (refNode.Parent != null)
				{
					if (refNode.Parent.Content == refNode)
					{
						refNode.Parent.Content = newCh.Adaptee;
					}
					if (refNode.Parent.Last == refNode)
					{
						refNode.Parent.Last = newCh.Adaptee;
					}
				}
				if (refNode.Prev != null)
				{
					refNode.Prev.Next = newCh.Adaptee;
				}
				if (refNode.Next != null)
				{
					refNode.Next.Prev = newCh.Adaptee;
				}
				for (n = refNode.Content; n != null; n = n.Next)
				{
					if (n.Parent == refNode)
					{
						n.Parent = newCh.Adaptee;
					}
				}
			}
			return oldChild;
		}
		
		public virtual INode RemoveChild(INode oldChild)
		{
			if (oldChild == null)
			{
				return null;
			}
			
			Node refNode = this.Adaptee.Content;
			while (refNode != null)
			{
				if (refNode.Adapter == oldChild)
				{
					break;
				}
				refNode = refNode.Next;
			}
			if (refNode == null)
			{
				throw new DomException(DomException.NotFound, "refChild not found");
			}
			Node.DiscardElement(refNode);
			
			if (Adaptee.Content == null && this.Adaptee.Type == Node.StartTag)
			{
				Adaptee.Type = Node.StartEndTag;
			}
			
			return oldChild;
		}
		
		public virtual INode AppendChild(INode newChild)
		{
			// TODO - handle newChild already in tree
			
			if (newChild == null)
			{
				return null;
			}

			if (!(newChild is DomNodeImpl))
			{
				throw new DomException(DomException.WrongDocument, "newChild not instanceof DomNodeImpl");
			}
			
			DomNodeImpl newCh = (DomNodeImpl) newChild;
			
			if (this.Adaptee.Type == Node.RootNode)
			{
				if (newCh.Adaptee.Type != Node.DocTypeTag && newCh.Adaptee.Type != Node.ProcInsTag)
				{
					throw new DomException(DomException.HierarchyRequest, "newChild cannot be a child of this node");
				}
			}
			else if (this.Adaptee.Type == Node.StartTag)
			{
				if (newCh.Adaptee.Type != Node.StartTag && newCh.Adaptee.Type != Node.StartEndTag && newCh.Adaptee.Type != Node.CommentTag && newCh.Adaptee.Type != Node.TextNode && newCh.Adaptee.Type != Node.CDATATag)
				{
					throw new DomException(DomException.HierarchyRequest, "newChild cannot be a child of this node");
				}
			}
			
			Node.InsertNodeAtEnd(Adaptee, newCh.Adaptee);
			
			if (Adaptee.Type == Node.StartEndTag)
			{
				Adaptee.Type = Node.StartTag;
			}
			
			return newChild;
		}
		
		public virtual bool HasChildNodes()
		{
			return (Adaptee.Content != null);
		}
		
		public virtual INode CloneNode(bool deep)
		{
			Node node = Adaptee.CloneNode(deep);
			node.Parent = null;
			return node.Adapter;
		}
		
		/// <summary> DOM2 - not implemented.</summary>
		public virtual void Normalize()
		{
		}
		
		/// <summary> DOM2 - not implemented.</summary>
		public virtual bool Supports(string feature, string version)
		{
			return IsSupported(feature, version);
		}

		/// <summary> DOM2 - not implemented.</summary>
		public virtual bool IsSupported(string feature, string version)
		{
			return false;
		}
		
		public virtual bool HasAttributes()
		{
			return Adaptee.Attributes != null;
		}

		private Node _adaptee;
	}
}