using System;
using TidyNet.Dom;

namespace TidyNet
{
	/// <summary>
	/// DomCharacterDataImpl
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
	/// <version>1.7, 1999/12/06 Tidy Release 30 Nov 1999</version>
	/// <version>1.8, 2000/01/22 Tidy Release 13 Jan 2000</version>
	/// <version>1.9, 2000/06/03 Tidy Release 30 Apr 2000</version>
	/// <version>1.10, 2000/07/22 Tidy Release 8 Jul 2000</version>
	/// <version>1.11, 2000/08/16 Tidy Release 4 Aug 2000</version>
	internal class DomCharacterDataImpl : DomNodeImpl, ICharacterData
	{
		protected internal DomCharacterDataImpl(Node Adaptee) : base(Adaptee)
		{
		}
		
		virtual public string Data
		{
			get
			{
				return NodeValue;
			}
			set
			{
				// NOT SUPPORTED
				throw new DomException(DomException.NoModificationAllowed, "Not supported");
			}
		}

		virtual public int Length
		{
			get
			{
				int len = 0;
				if (Adaptee.Textarray != null && Adaptee.Start < Adaptee.End)
				{
					len = Adaptee.End - Adaptee.Start;
				}
				return len;
			}
		}

		public virtual string SubstringData(int offset, int count)
		{
			int len;
			string val = null;
			if (count < 0)
			{
				throw new DomException(DomException.IndexSize, "Invalid length");
			}
			if (Adaptee.Textarray != null && Adaptee.Start < Adaptee.End)
			{
				if (Adaptee.Start + offset >= Adaptee.End)
				{
					throw new DomException(DomException.IndexSize, "Invalid offset");
				}
				len = count;
				if (Adaptee.Start + offset + len - 1 >= Adaptee.End)
					len = Adaptee.End - Adaptee.Start - offset;
				
				val = Lexer.GetString(Adaptee.Textarray, Adaptee.Start + offset, len);
			}
			return val;
		}
		
		public virtual void AppendData(string arg)
		{
			// NOT SUPPORTED
			throw new DomException(DomException.NoModificationAllowed, "Not supported");
		}
		
		public virtual void InsertData(int offset, string arg)
		{
			// NOT SUPPORTED
			throw new DomException(DomException.NoModificationAllowed, "Not supported");
		}
		
		public virtual void DeleteData(int offset, int count)
		{
			// NOT SUPPORTED
			throw new DomException(DomException.NoModificationAllowed, "Not supported");
		}
		
		public virtual void ReplaceData(int offset, int count, string arg)
		{
			// NOT SUPPORTED
			throw new DomException(DomException.NoModificationAllowed, "Not supported");
		}
	}
}