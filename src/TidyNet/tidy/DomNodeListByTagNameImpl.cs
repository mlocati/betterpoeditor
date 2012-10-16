using System;
using TidyNet.Dom;

namespace TidyNet
{	
	/// <summary>
	/// DomNodeListByTagNameImpl
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
	/// <remarks> <p>The items in the <code>NodeList</code> are accessible via an integral 
	/// index, starting from 0. 
	/// </remarks>
	internal class DomNodeListByTagNameImpl : INodeList
	{
		protected internal DomNodeListByTagNameImpl(Node first, string tagName)
		{
			_first = first;
			_tagName = tagName;
		}
		
		virtual public int Length
		{
			get
			{
				_currIndex = 0;
				_maxIndex = Int32.MaxValue;
				PreTraverse(_first);
				return _currIndex;
			}
		}
		
		public virtual INode Item(int index)
		{
			_currIndex = 0;
			_maxIndex = index;
			PreTraverse(_first);
			
			if (_currIndex > _maxIndex && _currNode != null)
			{
				return _currNode.Adapter;
			}
			else
			{
				return null;
			}
		}
		
		protected internal virtual void PreTraverse(Node node)
		{
			if (node == null)
			{
				return;
			}
			
			if (node.Type == Node.StartTag || node.Type == Node.StartEndTag)
			{
				if (_currIndex <= _maxIndex && (_tagName.Equals("*") || _tagName.Equals(node.Element)))
				{
					_currIndex += 1;
					_currNode = node;
				}
			}
			if (_currIndex > _maxIndex)
			{
				return;
			}
			
			node = node.Content;
			while (node != null)
			{
				PreTraverse(node);
				node = node.Next;
			}
		}

		private Node _first = null;
		private string _tagName = "*";
		private int _currIndex = 0;
		private int _maxIndex = 0;
		private Node _currNode = null;		
	}
}