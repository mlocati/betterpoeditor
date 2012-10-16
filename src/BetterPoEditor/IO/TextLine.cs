using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace BePoE.IO
{
	public class TextLine
	{
		private string _value;
		public string Value
		{
			get
			{ return this._value; }
			set
			{ this._value = (value == null) ? "" : value; }
		}
		public TextLine(string value)
		{
			this.Value = value;
		}
		public override string ToString()
		{
			return this._value;
		}
	}
}
