using System;
using TidyNet.Dom;

namespace TidyNet
{
	/// <summary>
	/// DOMElementImpl
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
	internal class DOMElementImpl : DomNodeImpl, IElement
	{
		protected internal DOMElementImpl(Node adaptee) : base(adaptee)
		{
		}
		
		override public NodeType NodeType
		{
			get
			{
				return NodeType.ELEMENT_NODE;
			}
		}

		virtual public string TagName
		{
			get
			{
				return base.NodeName;
			}
		}
		
		public virtual string GetAttribute(string name)
		{
			if (Adaptee == null)
			{
				return null;
			}
			
			AttVal att = Adaptee.Attributes;
			while (att != null)
			{
				if (att.Attribute.Equals(name))
				{
					break;
				}
				att = att.Next;
			}
			if (att != null)
			{
				return att.Val;
			}
			else
			{
				return String.Empty;
			}
		}
		
		public virtual void SetAttribute(string name, string val)
		{
			if (Adaptee == null)
			{
				return;
			}
			
			AttVal att = Adaptee.Attributes;
			while (att != null)
			{
				if (att.Attribute.Equals(name))
				{
					break;
				}
				att = att.Next;
			}
			if (att != null)
			{
				att.Val = val;
			}
			else
			{
				att = new AttVal(null, null, (int) '"', name, val);
				att.Dict = AttributeTable.DefaultAttributeTable.FindAttribute(att);
				if (Adaptee.Attributes == null)
				{
					Adaptee.Attributes = att;
				}
				else
				{
					att.Next = Adaptee.Attributes;
					Adaptee.Attributes = att;
				}
			}
		}
		
		public virtual void RemoveAttribute(string name)
		{
			if (Adaptee == null)
			{
				return;
			}
			
			AttVal att = Adaptee.Attributes;
			AttVal pre = null;
			while (att != null)
			{
				if (att.Attribute.Equals(name))
				{
					break;
				}
				pre = att;
				att = att.Next;
			}
			if (att != null)
			{
				if (pre == null)
				{
					Adaptee.Attributes = att.Next;
				}
				else
				{
					pre.Next = att.Next;
				}
			}
		}

		public virtual IAttr GetAttributeNode(string name)
		{
			if (Adaptee == null)
			{
				return null;
			}
			
			AttVal att = Adaptee.Attributes;
			while (att != null)
			{
				if (att.Attribute.Equals(name))
					break;
				att = att.Next;
			}
			if (att != null)
			{
				return att.Adapter;
			}
			else
			{
				return null;
			}
		}
		
		public virtual IAttr SetAttributeNode(IAttr newAttr)
		{
			if (newAttr == null)
			{
				return null;
			}

			if (!(newAttr is DomAttrImpl))
			{
				throw new DomException(DomException.WrongDocument, "newAttr not instanceof DomAttrImpl");
			}
			
			DomAttrImpl newatt = (DomAttrImpl) newAttr;
			string name = newatt.AttValAdaptee.Attribute;
			IAttr result = null;
			
			AttVal att = Adaptee.Attributes;
			while (att != null)
			{
				if (att.Attribute.Equals(name))
				{
					break;
				}
				att = att.Next;
			}
			if (att != null)
			{
				result = att.Adapter;
				att.Adapter = newAttr;
			}
			else
			{
				if (Adaptee.Attributes == null)
				{
					Adaptee.Attributes = newatt.AttValAdaptee;
				}
				else
				{
					newatt.AttValAdaptee.Next = Adaptee.Attributes;
					Adaptee.Attributes = newatt.AttValAdaptee;
				}
			}
			return result;
		}

		public virtual IAttr RemoveAttributeNode(IAttr oldAttr)
		{
			if (oldAttr == null)
			{
				return null;
			}
			
			IAttr result = null;
			AttVal att = Adaptee.Attributes;
			AttVal pre = null;
			while (att != null)
			{
				if (att.Adapter == oldAttr)
				{
					break;
				}
				pre = att;
				att = att.Next;
			}
			if (att != null)
			{
				if (pre == null)
				{
					Adaptee.Attributes = att.Next;
				}
				else
				{
					pre.Next = att.Next;
				}
				result = oldAttr;
			}
			else
			{
				throw new DomException(DomException.NotFound, "oldAttr not found");
			}
			return result;
		}

		public virtual INodeList GetElementsByTagName(string name)
		{
			return new DomNodeListByTagNameImpl(Adaptee, name);
		}

		public override void Normalize()
		{
			// NOT SUPPORTED
		}

		public virtual string GetAttributeNS(string namespaceURI, string localName)
		{
			return null;
		}

		/// <summary> DOM2 - not implemented. </summary>
		public virtual void SetAttributeNS(string namespaceURI, string qualifiedName, string val)
		{
		}
		
		/// <summary> DOM2 - not implemented. </summary>
		public virtual void RemoveAttributeNS(string namespaceURI, string localName)
		{
		}
		
		/// <summary> DOM2 - not implemented. </summary>
		public virtual IAttr GetAttributeNodeNS(string namespaceURI, string localName)
		{
			return null;
		}
		
		/// <summary> DOM2 - not implemented. </summary>
		public virtual IAttr SetAttributeNodeNS(IAttr newAttr)
		{
			return null;
		}

		/// <summary> DOM2 - not implemented. </summary>
		public virtual INodeList GetElementsByTagNameNS(string namespaceURI, string localName)
		{
			return null;
		}
		
		/// <summary> DOM2 - not implemented. </summary>
		public virtual bool HasAttribute(string name)
		{
			return false;
		}
		
		/// <summary> DOM2 - not implemented. </summary>
		public virtual bool HasAttributeNS(string namespaceURI, string localName)
		{
			return false;
		}
	}
}