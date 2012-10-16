using System;

namespace TidyNet
{
	/// <summary>
	/// Linked list of class names and styles
	/// 
	/// (c) 1998-2000 (W3C) MIT, INRIA, Keio University
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
	internal class Style
	{		
		public Style(string tag, string tagClass, string properties, Style next)
		{
			_tag = tag;
			_tagClass = tagClass;
			_properties = properties;
			_next = next;
		}
		
		public Style(string tag, string tagClass, string properties) : this(tag, tagClass, properties, null)
		{
		}
		
		public Style() : this(null, null, null, null)
		{
		}
		
		public string Tag
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

		public string TagClass
		{
			get
			{
				return _tagClass;
			}
			set
			{
				_tagClass = value;
			}
		}

		public string Properties
		{
			get
			{
				return _properties;
			}
			set
			{
				_properties = value;
			}
		}

		public Style Next
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

		private string _tag;
		private string _tagClass;
		private string _properties;
		private Style _next;
	}
}