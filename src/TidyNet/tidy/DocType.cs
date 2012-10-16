using System;

namespace TidyNet
{
	/// <summary>
	/// DOCTYPE enumeration.
	/// 
	/// (c) 1998-2000 (W3C) MIT, INRIA, Keio University
	/// See Tidy.cs for the copyright notice.
	/// Derived from <a href="http://www.w3.org/People/Raggett/tidy">
	/// HTML Tidy Release 4 Aug 2000</a>
	/// 
	/// </summary>
	/// <author>Seth Yates &lt;seth_yates@hotmail.com&gt; (translation to C#)</author>
	public enum DocType
	{
		/// <summary>
		/// Omit / omitted
		/// </summary>
		Omit = 0,

		/// <summary>
		/// Automatic
		/// </summary>
		Auto = 1,

		/// <summary>
		/// Strict
		/// </summary>
		Strict = 2,

		/// <summary>
		/// Loose
		/// </summary>
		Loose = 3,

		/// <summary>
		/// User-defined
		/// </summary>
		User = 4
	}
}
