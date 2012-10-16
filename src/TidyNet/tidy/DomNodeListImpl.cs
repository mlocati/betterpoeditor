using System;
using TidyNet.Dom;

namespace TidyNet
{	
	/// <summary>
	/// DomNodeListImpl
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
	/// </p>
	/// </remarks>
	internal class DomNodeListImpl : INodeList
	{
		protected internal DomNodeListImpl(Node parent)
		{
			_parent = parent;
		}
		
		virtual public int Length
		{
			get
			{
				int len = 0;
				Node node = _parent.Content;
				while (node != null)
				{
					len++;
					node = node.Next;
				}
				return len;
			}
		}
		
		public virtual INode Item(int index)
		{
			int i = 0;
			Node node = _parent.Content;
			while (node != null)
			{
				if (i >= index)
				{
					break;
				}
				i++;
				node = node.Next;
			}

			if (node != null)
			{
				return node.Adapter;
			}
			else
			{
				return null;
			}
		}
		
		private Node _parent = null;		
	}
}