using System;

namespace TidyNet
{
	/// <summary>
	/// Content Model enum.
	/// 
	/// (c) 1998-2000 (W3C) MIT, INRIA, Keio University
	/// See Tidy.cs for the copyright notice.
	/// Derived from <a href="http://www.w3.org/People/Raggett/tidy">
	/// HTML Tidy Release 4 Aug 2000</a>
	/// 
	/// </summary>
	/// <author>Seth Yates &lt;seth_yates@hotmail.com&gt; (translation to C#)</author>
	internal sealed class ContentModel
	{
		/* content model shortcut encoding */
		public const int Unknown = 0;
		public const int Empty = (1 << 0);
		public const int Html = (1 << 1);
		public const int Head = (1 << 2);
		public const int Block = (1 << 3);
		public const int Inline = (1 << 4);
		public const int List = (1 << 5);
		public const int Deflist = (1 << 6);
		public const int Table = (1 << 7);
		public const int Rowgrp = (1 << 8);
		public const int Row = (1 << 9);
		public const int Field = (1 << 10);
		public const int Object = (1 << 11);
		public const int Param = (1 << 12);
		public const int Frames = (1 << 13);
		public const int Heading = (1 << 14);
		public const int Opt = (1 << 15);
		public const int Img = (1 << 16);
		public const int Mixed = (1 << 17);
		public const int NoIndent = (1 << 18);
		public const int Obsolete = (1 << 19);
		public const int New = (1 << 20);
		public const int OmitSt = (1 << 21);
	}
}
