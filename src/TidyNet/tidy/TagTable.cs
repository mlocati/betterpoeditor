using System;
using System.Collections;

namespace TidyNet
{
	/// <summary>
	/// Tag dictionary node hash table
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
	internal class TagTable
	{
		public TagTable()
		{
			for (int i = 0; i < _tags.Length; i++)
			{
				Add(_tags[i]);
			}
			TagHtml = Lookup("html");
			TagHead = Lookup("head");
			TagBody = Lookup("body");
			TagFrameset = Lookup("frameset");
			TagFrame = Lookup("frame");
			TagNoframes = Lookup("noframes");
			TagMeta = Lookup("meta");
			TagTitle = Lookup("title");
			TagBase = Lookup("base");
			TagHr = Lookup("hr");
			TagPre = Lookup("pre");
			TagListing = Lookup("listing");
			TagH1 = Lookup("h1");
			TagH2 = Lookup("h2");
			TagP = Lookup("p");
			TagUl = Lookup("ul");
			TagOl = Lookup("ol");
			TagDir = Lookup("dir");
			TagLi = Lookup("li");
			TagDt = Lookup("dt");
			TagDd = Lookup("dd");
			TagDl = Lookup("dl");
			TagTd = Lookup("td");
			TagTh = Lookup("th");
			TagTr = Lookup("tr");
			TagCol = Lookup("col");
			TagBr = Lookup("br");
			TagA = Lookup("a");
			TagLink = Lookup("link");
			TagB = Lookup("b");
			TagI = Lookup("i");
			TagStrong = Lookup("strong");
			TagEm = Lookup("em");
			TagBig = Lookup("big");
			TagSmall = Lookup("small");
			TagParam = Lookup("param");
			TagOption = Lookup("option");
			TagOptgroup = Lookup("optgroup");
			TagImg = Lookup("img");
			TagMap = Lookup("map");
			TagArea = Lookup("area");
			TagNobr = Lookup("nobr");
			TagWbr = Lookup("wbr");
			TagFont = Lookup("font");
			TagSpacer = Lookup("spacer");
			TagLayer = Lookup("layer");
			TagCenter = Lookup("center");
			TagStyle = Lookup("style");
			TagScript = Lookup("script");
			TagNoscript = Lookup("noscript");
			tagTable = Lookup("table");
			TagCaption = Lookup("caption");
			TagForm = Lookup("form");
			TagTextarea = Lookup("textarea");
			TagBlockquote = Lookup("blockquote");
			TagApplet = Lookup("applet");
			TagObject = Lookup("object");
			TagDiv = Lookup("div");
			TagSpan = Lookup("span");
		}

		public TidyOptions Options
		{
			get
			{
				return _options;
			}
			set
			{
				_options = value;
			}
		}

		public Dict Lookup(string name)
		{
			return (Dict)_tagHashtable[name];
		}
		
		public Dict Add(Dict dict)
		{
			Dict d = (Dict)_tagHashtable[dict.Name];
			if (d != null)
			{
				d.Versions = dict.Versions;
				d.Model |= dict.Model;
				d.Parser = dict.Parser;
				d.CheckAttribs = dict.CheckAttribs;
				return d;
			}
			else
			{
				_tagHashtable[dict.Name] = dict;
				return dict;
			}
		}

		/* public method for finding tag by name */
		public bool FindTag(Node node)
		{
			Dict np;
			
			if (Options != null && Options.XmlTags)
			{
				node.Tag = XmlTags;
				return true;
			}
			
			if (node.Element != null)
			{
				np = Lookup(node.Element);
				if (np != null)
				{
					node.Tag = np;
					return true;
				}
			}
			
			return false;
		}
		
		public IParser FindParser(Node node)
		{
			Dict np;
			
			if (node.Element != null)
			{
				np = Lookup(node.Element);
				if (np != null)
				{
					return np.Parser;
				}
			}
			
			return null;
		}

		public void DefineInlineTag(string name)
		{
			Add(new Dict(name, HtmlVersion.Proprietary, (ContentModel.Inline | ContentModel.NoIndent | ContentModel.New), ParserImpl.ParseBlock, null));
		}
		
		public void DefineBlockTag(string name)
		{
			Add(new Dict(name, HtmlVersion.Proprietary, (ContentModel.Block | ContentModel.NoIndent | ContentModel.New), ParserImpl.ParseBlock, null));
		}
		
		public void defineEmptyTag(string name)
		{
			Add(new Dict(name, HtmlVersion.Proprietary, (ContentModel.Empty | ContentModel.NoIndent | ContentModel.New), ParserImpl.ParseBlock, null));
		}
		
		public void DefinePreTag(string name)
		{
			Add(new Dict(name, HtmlVersion.Proprietary, (ContentModel.Block | ContentModel.NoIndent | ContentModel.New), ParserImpl.ParsePre, null));
		}

		/* create dummy entry for all xml tags */
		public Dict XmlTags = new Dict(null, HtmlVersion.All, ContentModel.Block, null, null);
		public Dict TagHtml = null;
		public Dict TagHead = null;
		public Dict TagBody = null;
		public Dict TagFrameset = null;
		public Dict TagFrame = null;
		public Dict TagNoframes = null;
		public Dict TagMeta = null;
		public Dict TagTitle = null;
		public Dict TagBase = null;
		public Dict TagHr = null;
		public Dict TagPre = null;
		public Dict TagListing = null;
		public Dict TagH1 = null;
		public Dict TagH2 = null;
		public Dict TagP = null;
		public Dict TagUl = null;
		public Dict TagOl = null;
		public Dict TagDir = null;
		public Dict TagLi = null;
		public Dict TagDt = null;
		public Dict TagDd = null;
		public Dict TagDl = null;
		public Dict TagTd = null;
		public Dict TagTh = null;
		public Dict TagTr = null;
		public Dict TagCol = null;
		public Dict TagBr = null;
		public Dict TagA = null;
		public Dict TagLink = null;
		public Dict TagB = null;
		public Dict TagI = null;
		public Dict TagStrong = null;
		public Dict TagEm = null;
		public Dict TagBig = null;
		public Dict TagSmall = null;
		public Dict TagParam = null;
		public Dict TagOption = null;
		public Dict TagOptgroup = null;
		public Dict TagImg = null;
		public Dict TagMap = null;
		public Dict TagArea = null;
		public Dict TagNobr = null;
		public Dict TagWbr = null;
		public Dict TagFont = null;
		public Dict TagSpacer = null;
		public Dict TagLayer = null;
		public Dict TagCenter = null;
		public Dict TagStyle = null;
		public Dict TagScript = null;
		public Dict TagNoscript = null;
		public Dict tagTable = null;
		public Dict TagCaption = null;
		public Dict TagForm = null;
		public Dict TagTextarea = null;
		public Dict TagBlockquote = null;
		public Dict TagApplet = null;
		public Dict TagObject = null;
		public Dict TagDiv = null;
		public Dict TagSpan = null;

		private TidyOptions _options = null;
		private Hashtable _tagHashtable = new Hashtable();
		private static Dict[] _tags = new Dict[]
			{
				new Dict("html", HtmlVersion.All | HtmlVersion.Frames, ContentModel.Html | ContentModel.Opt | ContentModel.OmitSt, ParserImpl.ParseHTML, CheckAttribsImpl.CheckHtml), 
				new Dict("head", HtmlVersion.All | HtmlVersion.Frames, ContentModel.Html | ContentModel.Opt | ContentModel.OmitSt, ParserImpl.ParseHead, null), 
				new Dict("title", HtmlVersion.All | HtmlVersion.Frames, ContentModel.Head, ParserImpl.ParseTitle, null), 
				new Dict("base", HtmlVersion.All | HtmlVersion.Frames, ContentModel.Head | ContentModel.Empty, null, null), 
				new Dict("link", HtmlVersion.All | HtmlVersion.Frames, ContentModel.Head | ContentModel.Empty, null, CheckAttribsImpl.CheckLink), 
				new Dict("meta", HtmlVersion.All | HtmlVersion.Frames, ContentModel.Head | ContentModel.Empty, null, null), 
				new Dict("style", HtmlVersion.From32 | HtmlVersion.Frames, ContentModel.Head, ParserImpl.ParseScript, CheckAttribsImpl.CheckStyle), 
				new Dict("script", HtmlVersion.From32 | HtmlVersion.Frames, ContentModel.Head | ContentModel.Mixed | ContentModel.Block | ContentModel.Inline, ParserImpl.ParseScript, CheckAttribsImpl.CheckScript), 
				new Dict("server", HtmlVersion.Netscape, ContentModel.Head | ContentModel.Mixed | ContentModel.Block | ContentModel.Inline, ParserImpl.ParseScript, null), 
				new Dict("body", HtmlVersion.All, ContentModel.Html | ContentModel.Opt | ContentModel.OmitSt, ParserImpl.ParseBody, null), 
				new Dict("frameset", HtmlVersion.Frames, ContentModel.Html | ContentModel.Frames, ParserImpl.ParseFrameSet, null), 
				new Dict("p", HtmlVersion.All, ContentModel.Block | ContentModel.Opt, ParserImpl.ParseInline, null), 
				new Dict("h1", HtmlVersion.All, ContentModel.Block | ContentModel.Heading, ParserImpl.ParseInline, null), 
				new Dict("h2", HtmlVersion.All, ContentModel.Block | ContentModel.Heading, ParserImpl.ParseInline, null), 
				new Dict("h3", HtmlVersion.All, ContentModel.Block | ContentModel.Heading, ParserImpl.ParseInline, null), 
				new Dict("h4", HtmlVersion.All, ContentModel.Block | ContentModel.Heading, ParserImpl.ParseInline, null), 
				new Dict("h5", HtmlVersion.All, ContentModel.Block | ContentModel.Heading, ParserImpl.ParseInline, null), 
				new Dict("h6", HtmlVersion.All, ContentModel.Block | ContentModel.Heading, ParserImpl.ParseInline, null), 
				new Dict("ul", HtmlVersion.All, ContentModel.Block, ParserImpl.ParseList, null), 
				new Dict("ol", HtmlVersion.All, ContentModel.Block, ParserImpl.ParseList, null), 
				new Dict("dl", HtmlVersion.All, ContentModel.Block, ParserImpl.ParseDefList, null), 
				new Dict("dir", HtmlVersion.Loose, ContentModel.Block | ContentModel.Obsolete, ParserImpl.ParseList, null), 
				new Dict("menu", HtmlVersion.Loose, ContentModel.Block | ContentModel.Obsolete, ParserImpl.ParseList, null), 
				new Dict("pre", HtmlVersion.All, ContentModel.Block, ParserImpl.ParsePre, null), 
				new Dict("listing", HtmlVersion.All, ContentModel.Block | ContentModel.Obsolete, ParserImpl.ParsePre, null), 
				new Dict("xmp", HtmlVersion.All, ContentModel.Block | ContentModel.Obsolete, ParserImpl.ParsePre, null), 
				new Dict("plaintext", HtmlVersion.All, ContentModel.Block | ContentModel.Obsolete, ParserImpl.ParsePre, null), 
				new Dict("address", HtmlVersion.All, ContentModel.Block, ParserImpl.ParseBlock, null), 
				new Dict("blockquote", HtmlVersion.All, ContentModel.Block, ParserImpl.ParseBlock, null), 
				new Dict("form", HtmlVersion.All, ContentModel.Block, ParserImpl.ParseBlock, null), 
				new Dict("isindex", HtmlVersion.Loose, ContentModel.Block | ContentModel.Empty, null, null), 
				new Dict("fieldset", HtmlVersion.Html40, ContentModel.Block, ParserImpl.ParseBlock, null), 
				new Dict("table", HtmlVersion.From32, ContentModel.Block, ParserImpl.ParseTableTag, CheckAttribsImpl.CheckTable), 
				new Dict("hr", HtmlVersion.All, ContentModel.Block | ContentModel.Empty, null, CheckAttribsImpl.CheckHr), 
				new Dict("div", HtmlVersion.From32, ContentModel.Block, ParserImpl.ParseBlock, null), 
				new Dict("multicol", HtmlVersion.Netscape, ContentModel.Block, ParserImpl.ParseBlock, null), 
				new Dict("nosave", HtmlVersion.Netscape, ContentModel.Block, ParserImpl.ParseBlock, null), 
				new Dict("layer", HtmlVersion.Netscape, ContentModel.Block, ParserImpl.ParseBlock, null), 
				new Dict("ilayer", HtmlVersion.Netscape, ContentModel.Inline, ParserImpl.ParseInline, null), 
				new Dict("nolayer", HtmlVersion.Netscape, ContentModel.Block | ContentModel.Inline | ContentModel.Mixed, ParserImpl.ParseBlock, null), 
				new Dict("align", HtmlVersion.Netscape, ContentModel.Block, ParserImpl.ParseBlock, null), 
				new Dict("center", HtmlVersion.Loose, ContentModel.Block, ParserImpl.ParseBlock, null), 
				new Dict("ins", HtmlVersion.Html40, ContentModel.Inline | ContentModel.Block | ContentModel.Mixed, ParserImpl.ParseInline, null), 
				new Dict("del", HtmlVersion.Html40, ContentModel.Inline | ContentModel.Block | ContentModel.Mixed, ParserImpl.ParseInline, null), 
				new Dict("li", HtmlVersion.All, ContentModel.List | ContentModel.Opt | ContentModel.NoIndent, ParserImpl.ParseBlock, null), 
				new Dict("dt", HtmlVersion.All, ContentModel.Deflist | ContentModel.Opt | ContentModel.NoIndent, ParserImpl.ParseInline, null), 
				new Dict("dd", HtmlVersion.All, ContentModel.Deflist | ContentModel.Opt | ContentModel.NoIndent, ParserImpl.ParseBlock, null), 
				new Dict("caption", HtmlVersion.From32, ContentModel.Table, ParserImpl.ParseInline, CheckAttribsImpl.CheckCaption), 
				new Dict("colgroup", HtmlVersion.Html40, ContentModel.Table | ContentModel.Opt, ParserImpl.ParseColGroup, null), 
				new Dict("col", HtmlVersion.Html40, ContentModel.Table | ContentModel.Empty, null, null), 
				new Dict("thead", HtmlVersion.Html40, ContentModel.Table | ContentModel.Rowgrp | ContentModel.Opt, ParserImpl.ParseRowGroup, null), 
				new Dict("tfoot", HtmlVersion.Html40, ContentModel.Table | ContentModel.Rowgrp | ContentModel.Opt, ParserImpl.ParseRowGroup, null), 
				new Dict("tbody", HtmlVersion.Html40, ContentModel.Table | ContentModel.Rowgrp | ContentModel.Opt, ParserImpl.ParseRowGroup, null), 
				new Dict("tr", HtmlVersion.From32, ContentModel.Table | ContentModel.Opt, ParserImpl.ParseRow, null), 
				new Dict("td", HtmlVersion.From32, ContentModel.Row | ContentModel.Opt | ContentModel.NoIndent, ParserImpl.ParseBlock, CheckAttribsImpl.CheckTableCell), 
				new Dict("th", HtmlVersion.From32, ContentModel.Row | ContentModel.Opt | ContentModel.NoIndent, ParserImpl.ParseBlock, CheckAttribsImpl.CheckTableCell), 
				new Dict("q", HtmlVersion.Html40, ContentModel.Inline, ParserImpl.ParseInline, null), 
				new Dict("a", HtmlVersion.All, ContentModel.Inline, ParserImpl.ParseInline, CheckAttribsImpl.CheckAnchor), 
				new Dict("br", HtmlVersion.All, ContentModel.Inline | ContentModel.Empty, null, null), 
				new Dict("img", HtmlVersion.All, ContentModel.Inline | ContentModel.Img | ContentModel.Empty, null, CheckAttribsImpl.CheckImg), 
				new Dict("object", HtmlVersion.Html40, ContentModel.Object | ContentModel.Head | ContentModel.Img | ContentModel.Inline | ContentModel.Param, ParserImpl.ParseBlock, null), 
				new Dict("applet", HtmlVersion.Loose, ContentModel.Object | ContentModel.Img | ContentModel.Inline | ContentModel.Param, ParserImpl.ParseBlock, null), 
				new Dict("servlet", HtmlVersion.Sun, ContentModel.Object | ContentModel.Img | ContentModel.Inline | ContentModel.Param, ParserImpl.ParseBlock, null), 
				new Dict("param", HtmlVersion.From32, ContentModel.Inline | ContentModel.Empty, null, null), 
				new Dict("embed", HtmlVersion.Netscape, ContentModel.Inline | ContentModel.Img | ContentModel.Empty, null, null), 
				new Dict("noembed", HtmlVersion.Netscape, ContentModel.Inline, ParserImpl.ParseInline, null), 
				new Dict("iframe", HtmlVersion.Html40Loose, ContentModel.Inline, ParserImpl.ParseBlock, null), 
				new Dict("frame", HtmlVersion.Frames, ContentModel.Frames | ContentModel.Empty, null, null), 
				new Dict("noframes", HtmlVersion.Iframes, ContentModel.Block | ContentModel.Frames, ParserImpl.ParseNoFrames, null), 
				new Dict("noscript", HtmlVersion.Frames | HtmlVersion.Html40, ContentModel.Block | ContentModel.Inline | ContentModel.Mixed, ParserImpl.ParseBlock, null), 
				new Dict("b", HtmlVersion.All, ContentModel.Inline, ParserImpl.ParseInline, null), 
				new Dict("i", HtmlVersion.All, ContentModel.Inline, ParserImpl.ParseInline, null), 
				new Dict("u", HtmlVersion.Loose, ContentModel.Inline, ParserImpl.ParseInline, null), 
				new Dict("tt", HtmlVersion.All, ContentModel.Inline, ParserImpl.ParseInline, null), 
				new Dict("s", HtmlVersion.Loose, ContentModel.Inline, ParserImpl.ParseInline, null), 
				new Dict("strike", HtmlVersion.Loose, ContentModel.Inline, ParserImpl.ParseInline, null), 
				new Dict("big", HtmlVersion.From32, ContentModel.Inline, ParserImpl.ParseInline, null), 
				new Dict("small", HtmlVersion.From32, ContentModel.Inline, ParserImpl.ParseInline, null), 
				new Dict("sub", HtmlVersion.From32, ContentModel.Inline, ParserImpl.ParseInline, null), 
				new Dict("sup", HtmlVersion.From32, ContentModel.Inline, ParserImpl.ParseInline, null), 
				new Dict("em", HtmlVersion.All, ContentModel.Inline, ParserImpl.ParseInline, null), 
				new Dict("strong", HtmlVersion.All, ContentModel.Inline, ParserImpl.ParseInline, null), 
				new Dict("dfn", HtmlVersion.All, ContentModel.Inline, ParserImpl.ParseInline, null), 
				new Dict("code", HtmlVersion.All, ContentModel.Inline, ParserImpl.ParseInline, null), 
				new Dict("samp", HtmlVersion.All, ContentModel.Inline, ParserImpl.ParseInline, null), 
				new Dict("kbd", HtmlVersion.All, ContentModel.Inline, ParserImpl.ParseInline, null), 
				new Dict("var", HtmlVersion.All, ContentModel.Inline, ParserImpl.ParseInline, null), 
				new Dict("cite", HtmlVersion.All, ContentModel.Inline, ParserImpl.ParseInline, null), 
				new Dict("abbr", HtmlVersion.Html40, ContentModel.Inline, ParserImpl.ParseInline, null), 
				new Dict("acronym", HtmlVersion.Html40, ContentModel.Inline, ParserImpl.ParseInline, null), 
				new Dict("span", HtmlVersion.From32, ContentModel.Inline, ParserImpl.ParseInline, null), 
				new Dict("blink", HtmlVersion.Proprietary, ContentModel.Inline, ParserImpl.ParseInline, null), 
				new Dict("nobr", HtmlVersion.Proprietary, ContentModel.Inline, ParserImpl.ParseInline, null), 
				new Dict("wbr", HtmlVersion.Proprietary, ContentModel.Inline | ContentModel.Empty, null, null), 
				new Dict("marquee", HtmlVersion.Microsoft, ContentModel.Inline | ContentModel.Opt, ParserImpl.ParseInline, null), 
				new Dict("bgsound", HtmlVersion.Microsoft, ContentModel.Head | ContentModel.Empty, null, null), 
				new Dict("comment", HtmlVersion.Microsoft, ContentModel.Inline, ParserImpl.ParseInline, null), 
				new Dict("spacer", HtmlVersion.Netscape, ContentModel.Inline | ContentModel.Empty, null, null), 
				new Dict("keygen", HtmlVersion.Netscape, ContentModel.Inline | ContentModel.Empty, null, null), 
				new Dict("nolayer", HtmlVersion.Netscape, ContentModel.Block | ContentModel.Inline | ContentModel.Mixed, ParserImpl.ParseBlock, null), 
				new Dict("ilayer", HtmlVersion.Netscape, ContentModel.Inline, ParserImpl.ParseInline, null), 
				new Dict("map", HtmlVersion.From32, ContentModel.Inline, ParserImpl.ParseBlock, CheckAttribsImpl.CheckMap), 
				new Dict("area", HtmlVersion.All, ContentModel.Block | ContentModel.Empty, null, CheckAttribsImpl.CheckArea), 
				new Dict("input", HtmlVersion.All, ContentModel.Inline | ContentModel.Img | ContentModel.Empty, null, null), 
				new Dict("select", HtmlVersion.All, ContentModel.Inline | ContentModel.Field, ParserImpl.ParseSelect, null), 
				new Dict("option", HtmlVersion.All, ContentModel.Field | ContentModel.Opt, ParserImpl.ParseText, null), 
				new Dict("optgroup", HtmlVersion.Html40, ContentModel.Field | ContentModel.Opt, ParserImpl.ParseOptGroup, null), 
				new Dict("textarea", HtmlVersion.All, ContentModel.Inline | ContentModel.Field, ParserImpl.ParseText, null), 
				new Dict("label", HtmlVersion.Html40, ContentModel.Inline, ParserImpl.ParseInline, null), 
				new Dict("legend", HtmlVersion.Html40, ContentModel.Inline, ParserImpl.ParseInline, null), 
				new Dict("button", HtmlVersion.Html40, ContentModel.Inline, ParserImpl.ParseInline, null), 
				new Dict("basefont", HtmlVersion.Loose, ContentModel.Inline | ContentModel.Empty, null, null), 
				new Dict("font", HtmlVersion.Loose, ContentModel.Inline, ParserImpl.ParseInline, null), 
				new Dict("bdo", HtmlVersion.Html40, ContentModel.Inline, ParserImpl.ParseInline, null)
			};
	}
}