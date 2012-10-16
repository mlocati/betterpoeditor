using System;
using TidyNet.Dom;

namespace TidyNet
{
	/// <summary>
	/// DomDocumentImpl
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
	internal class DomDocumentImpl : DomNodeImpl, IDocument
	{
		protected internal DomDocumentImpl(Node Adaptee) : base(Adaptee)
		{
			tt = new TagTable();
		}
		
		virtual public TagTable TagTable
		{
			set
			{
				this.tt = value;
			}
		}

		override public string NodeName
		{
			get
			{
				return "#document";
			}
		}
		
		override public NodeType NodeType
		{
			get
			{
				return NodeType.DOCUMENT_NODE;
			}
		}
		
		virtual public IDocumentType Doctype
		{
			get
			{
				Node node = Adaptee.Content;
				while (node != null)
				{
					if (node.Type == Node.DocTypeTag)
					{
						break;
					}
					node = node.Next;
				}
				if (node != null)
				{
					return (IDocumentType) node.Adapter;
				}
				else
				{
					return null;
				}
			}
		}
		
		virtual public IDomImplementation Implementation
		{
			get
			{
				// NOT SUPPORTED
				return null;
			}
		}
		
		virtual public IElement DocumentElement
		{
			get
			{
				Node node = Adaptee.Content;
				while (node != null)
				{
					if (node.Type == Node.StartTag || node.Type == Node.StartEndTag)
					{
						break;
					}
					node = node.Next;
				}
				if (node != null)
				{
					return (IElement) node.Adapter;
				}
				else
				{
					return null;
				}
			}
		}
		
		public virtual IElement CreateElement(string tagName)
		{
			Node node = new Node(Node.StartEndTag, null, 0, 0, tagName, tt);
			if (node != null)
			{
				if (node.Tag == null)
				{
					// Fix Bug 121206
					node.Tag = tt.XmlTags;
				}
				return (IElement) node.Adapter;
			}
			else
			{
				return null;
			}
		}
		
		public virtual IDocumentFragment CreateDocumentFragment()
		{
			// NOT SUPPORTED
			return null;
		}
		
		public virtual IText CreateTextNode(string data)
		{
			byte[] textarray = Lexer.GetBytes(data);
			Node node = new Node(Node.TextNode, textarray, 0, textarray.Length);
			if (node != null)
			{
				return (IText) node.Adapter;
			}
			else
			{
				return null;
			}
		}
		
		public virtual IComment CreateComment(string data)
		{
			byte[] textarray = Lexer.GetBytes(data);
			Node node = new Node(Node.CommentTag, textarray, 0, textarray.Length);
			if (node != null)
			{
				return (IComment) node.Adapter;
			}
			else
			{
				return null;
			}
		}
		
		public virtual ICdataSection CreateCdataSection(string data)
		{
			// NOT SUPPORTED
			return null;
		}
		
		public virtual IProcessingInstruction CreateProcessingInstruction(string target, string data)
		{
			throw new DomException(DomException.NotSupported, "HTML document");
		}
		
		public virtual IAttr CreateAttribute(string name)
		{
			AttVal av = new AttVal(null, null, (int) '"', name, null);
			if (av != null)
			{
				av.Dict = AttributeTable.DefaultAttributeTable.FindAttribute(av);
				return (IAttr) av.Adapter;
			}
			else
			{
				return null;
			}
		}
		
		public virtual IEntityReference CreateEntityReference(string name)
		{
			// NOT SUPPORTED
			return null;
		}
		
		public virtual INodeList GetElementsByTagName(string tagname)
		{
			return new DomNodeListByTagNameImpl(this.Adaptee, tagname);
		}
		
		/// <summary> DOM2 - not implemented. </summary>
		public virtual INode ImportNode(INode importedNode, bool deep)
		{
			return null;
		}
		
		/// <summary> DOM2 - not implemented. </summary>
		public virtual IAttr CreateAttributeNS(string namespaceURI, string qualifiedName)
		{
			return null;
		}
		
		/// <summary> DOM2 - not implemented. </summary>
		public virtual IElement CreateElementNS(string namespaceURI, string qualifiedName)
		{
			return null;
		}
		
		/// <summary> DOM2 - not implemented. </summary>
		public virtual INodeList GetElementsByTagNameNS(string namespaceURI, string localName)
		{
			return null;
		}
		
		/// <summary> DOM2 - not implemented. </summary>
		public virtual IElement GetElementById(string elementId)
		{
			return null;
		}
		
		private TagTable tt;
	}
}