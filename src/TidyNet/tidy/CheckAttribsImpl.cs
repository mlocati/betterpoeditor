using System;
using TidyNet.Dom;

namespace TidyNet
{
	/// <summary>
	/// Check HTML attributes implementation
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
	internal class CheckAttribsImpl
	{
		public static ICheckAttribs CheckHtml
		{
			get
			{
				return _checkHtml;
			}
		}

		public static ICheckAttribs CheckScript
		{
			get
			{
				return _checkScript;
			}
		}

		public static ICheckAttribs CheckTable
		{
			get
			{
				return _checkTable;
			}
		}

		public static ICheckAttribs CheckCaption
		{
			get
			{
				return _checkCaption;
			}
		}

		public static ICheckAttribs CheckImg
		{
			get
			{
				return _checkImg;
			}
		}

		public static ICheckAttribs CheckArea
		{
			get
			{
				return _checkArea;
			}
		}
		
		public static ICheckAttribs CheckAnchor
		{
			get
			{
				return _checkAnchor;
			}
		}
		
		public static ICheckAttribs CheckMap
		{
			get
			{
				return _checkMap;
			}
		}
		
		public static ICheckAttribs CheckStyle
		{
			get
			{
				return _checkStyle;
			}
		}
		
		public static ICheckAttribs CheckTableCell
		{
			get
			{
				return _checkTableCell;
			}
		}

		public static ICheckAttribs CheckLink
		{
			get
			{
				return _checkLink;
			}
		}
		
		public static ICheckAttribs CheckHr
		{
			get
			{
				return _checkHr;
			}
		}

		private static ICheckAttribs _checkHtml = new HtmlCheckAttribs();
		private static ICheckAttribs _checkScript = new ScriptCheckAttribs();
		private static ICheckAttribs _checkTable = new TableCheckAttribs();
		private static ICheckAttribs _checkCaption = new CaptionCheckTableCheckAttribs();
		private static ICheckAttribs _checkImg = new ImgCheckTableCheckAttribs();
		private static ICheckAttribs _checkArea = new AreaCheckTableCheckAttribs();
		private static ICheckAttribs _checkAnchor = new AnchorCheckTableCheckAttribs();
		private static ICheckAttribs _checkMap = new MapCheckTableCheckAttribs();
		private static ICheckAttribs _checkStyle = new StyleCheckTableCheckAttribs();
		private static ICheckAttribs _checkTableCell = new TableCellCheckTableCheckAttribs();
		private static ICheckAttribs _checkLink = new LinkCheckTableCheckAttribs();
		private static ICheckAttribs _checkHr = new HrCheckTableCheckAttribs();
	}
}