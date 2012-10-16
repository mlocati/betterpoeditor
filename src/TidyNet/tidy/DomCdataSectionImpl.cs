using System;
using TidyNet.Dom;

namespace TidyNet
{
	/// <summary>
	/// DomCdataSectionImpl
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
	/// <author>Gary L Peskin &lt;garyp@firstech.com&gt;</author>
	/// <version>1.11, 2000/08/16 Tidy Release 4 Aug 2000</version>
	internal class DomCdataSectionImpl : DomTextImpl, ICdataSection
	{
		override public string NodeName
		{
			get
			{
				return "#cdata-section";
			}
		}

		override public NodeType NodeType
		{
			get
			{
				return NodeType.CDATA_SECTION_NODE;
			}
		}
		
		protected internal DomCdataSectionImpl(Node adaptee):base(adaptee)
		{
		}
	}
}