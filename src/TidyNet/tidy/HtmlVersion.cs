using System;

namespace TidyNet
{
	/// <summary>
	/// Version of HTML.
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
	/// <remarks>
	/// If the document uses just HTML 2.0 tags and attributes described it as HTML 2.0
	/// Similarly for HTML 3.2 and the 3 flavors of HTML 4.0. If there are proprietary
	/// tags and attributes then describe it as HTML Proprietary. If it includes the
	/// xml-lang or xmlns attributes but is otherwise HTML 2.0, 3.2 or 4.0 then describe
	/// it as one of the flavors of Voyager (strict, loose or frameset).
	/// </remarks>
	[Flags]
	public enum HtmlVersion
	{
		Unknown = 0,
		Html20 = 1,
		Html32 = 2,
		Html40Strict = 4,
		Html40Loose = 8,
		Frames = 16,
		Xml = 32,
		Netscape = 64,
		Microsoft = 128,
		Sun = 256,
		Malformed = 512,
		All = Html20 | Html32 | Html40Strict | Html40Loose | Frames,
		Html40 = Html40Strict | Html40Loose | Frames,
		Loose = Html32 | Html40Loose | Frames,
		Iframes = Html40Loose | Frames,
		From32 = Html40Strict | Loose,
		Proprietary = Netscape | Microsoft | Sun,
		Everything = All | Proprietary
		
	}
}
