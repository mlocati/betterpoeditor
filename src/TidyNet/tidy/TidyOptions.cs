using System;
using System.Collections;

namespace TidyNet
{
	/// <summary>
	/// Tidy options.
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
	public class TidyOptions
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="configuration"></param>
		public TidyOptions()
		{
		}

		/// <summary>Default indentation</summary>
		public virtual int Spaces
		{
			get
			{
				return _spaces;
			}
			set
			{
				_spaces = value;
			}
		}

		/// <summary>Default wrap margin</summary>
		public virtual int WrapLen
		{
			get
			{
				return _wrapLen;
			}
			set
			{
				_wrapLen = value;
			}
		}

		/// <summary>Character Encoding</summary>
		public virtual CharEncoding CharEncoding
		{
			get
			{
				return _charEncoding;
			}
			set
			{
				_charEncoding = value;
			}
		}

		/// <summary>Tab size</summary>
		public virtual int TabSize
		{
			get
			{
				return _tabSize;
			}
			set
			{
				_tabSize = value;
			}
		}

		/// <summary>Indent content of appropriate tags</summary>
		public virtual bool IndentContent
		{
			get
			{
				return _indentContent;
			}
			set
			{
				_indentContent = value;
			}
		}

		/// <summary>Does text/block level content affect indentation</summary>
		public virtual bool SmartIndent
		{
			get
			{
				return _smartIndent;
			}
			set
			{
				_smartIndent = value;
			}
		}

		/// <summary>Suppress optional end tags</summary>
		public virtual bool HideEndTags
		{
			get
			{
				return _hideEndTags;
			}
			set
			{
				_hideEndTags = value;
			}
		}

		/// <summary>Treat input as XML</summary>
		public virtual bool XmlTags
		{
			get
			{
				return _xmlTags;
			}
			set
			{
				_xmlTags = value;
			}
		}

		/// <summary>Create output as XML</summary>
		public virtual bool XmlOut
		{
			get
			{
				return _xmlOut;
			}
			set
			{
				_xmlOut = value;
			}
		}

		/// <summary>Output XHTML</summary>
		public virtual bool Xhtml
		{
			get
			{
				return _xhtml;
			}
			set
			{
				_xhtml = value;
			}
		}

		/// <summary>Avoid mapping values > 127 to entities</summary>
		public virtual bool RawOut
		{
			get
			{
				return _rawOut;
			}
			set
			{
				_rawOut = value;
			}
		}

		/// <summary>Output tags in upper not lower case</summary>
		public virtual bool UpperCaseTags
		{
			get
			{
				return _upperCaseTags;
			}
			set
			{
				_upperCaseTags = value;
			}
		}

		/// <summary>Output attributes in upper not lower case</summary>
		public virtual bool UpperCaseAttrs
		{
			get
			{
				return _upperCaseAttrs;
			}
			set
			{
				_upperCaseAttrs = value;
			}
		}

		/// <summary>Remove presentational clutter</summary>
		public virtual bool MakeClean
		{
			get
			{
				return _makeClean;
			}
			set
			{
				_makeClean = value;
			}
		}

		/// <summary>O/p newline before &lt;br&gt; or not?</summary>
		public virtual bool BreakBeforeBR
		{
			get
			{
				return _breakBeforeBR;
			}
			set
			{
				_breakBeforeBR = value;
			}
		}

		/// <summary>Create slides on each h2 element</summary>
		public virtual bool BurstSlides
		{
			get
			{
				return _burstSlides;
			}
			set
			{
				_burstSlides = value;
			}
		}

		/// <summary>Use numeric entities</summary>
		public virtual bool NumEntities
		{
			get
			{
				return _numEntities;
			}
			set
			{
				_numEntities = value;
			}
		}

		/// <summary>Output " marks as &amp;quot;</summary>
		public virtual bool QuoteMarks
		{
			get
			{
				return _quoteMarks;
			}
			set
			{
				_quoteMarks = value;
			}
		}

		/// <summary>Output non-breaking space as entity</summary>
		public virtual bool QuoteNbsp
		{
			get
			{
				return _quoteNbsp;
			}
			set
			{
				_quoteNbsp = value;
			}
		}

		/// <summary>Output naked ampersand as &amp;</summary>
		public virtual bool QuoteAmpersand
		{
			get
			{
				return _quoteAmpersand;
			}
			set
			{
				_quoteAmpersand = value;
			}
		}

		/// <summary>Wrap within attribute values</summary>
		public virtual bool WrapAttVals
		{
			get
			{
				return _wrapAttVals;
			}
			set
			{
				_wrapAttVals = value;
			}
		}

		/// <summary>Wrap within JavaScript string literals</summary>
		public virtual bool WrapScriptlets
		{
			get
			{
				return _wrapScriptlets;
			}
			set
			{
				_wrapScriptlets = value;
			}
		}

		/// <summary>Wrap within &lt;![ ... ]&gt; section tags</summary>
		public virtual bool WrapSection
		{
			get
			{
				return _wrapSection;
			}
			set
			{
				_wrapSection = value;
			}
		}

		/// <summary>Default text for alt attribute</summary>
		public virtual string AltText
		{
			get
			{
				return _altText;
			}
			set
			{
				_altText = value;
			}
		}

		/// <summary>Style sheet for slides</summary>
		public virtual string Slidestyle
		{
			get
			{
				return _slideStyle;
			}
			set
			{
				_slideStyle = value;
			}
		}

		/// <summary>Add &lt;?xml?&gt; for XML docs</summary>
		public virtual bool XmlPi
		{
			get
			{
				return _xmlPi;
			}
			set
			{
				_xmlPi = value;
			}
		}

		/// <summary>Discard presentation tags</summary>
		public virtual bool DropFontTags
		{
			get
			{
				return _dropFontTags;
			}
			set
			{
				_dropFontTags = value;
			}
		}

		/// <summary>Discard empty p elements</summary>
		public virtual bool DropEmptyParas
		{
			get
			{
				return _dropEmptyParas;
			}
			set
			{
				_dropEmptyParas = value;
			}
		}

		/// <summary>Fix comments with adjacent hyphens</summary>
		public virtual bool FixComments
		{
			get
			{
				return _fixComments;
			}
			set
			{
				_fixComments = value;
			}
		}

		/// <summary>Wrap within ASP pseudo elements</summary>
		public virtual bool WrapAsp
		{
			get
			{
				return _wrapAsp;
			}
			set
			{
				_wrapAsp = value;
			}
		}

		/// <summary>Wrap within JSTE pseudo elements</summary>
		public virtual bool WrapJste
		{
			get
			{
				return _wrapJste;
			}
			set
			{
				_wrapJste = value;
			}
		}

		/// <summary>Wrap within PHP pseudo elements</summary>
		public virtual bool WrapPhp
		{
			get
			{
				return _wrapPhp;
			}
			set
			{
				_wrapPhp = value;
			}
		}

		/// <summary>Fix URLs by replacing \ with /</summary>
		public virtual bool FixBackslash
		{
			get
			{
				return _fixBackslash;
			}
			set
			{
				_fixBackslash = value;
			}
		}

		/// <summary>Newline+indent before each attribute</summary>
		public virtual bool IndentAttributes
		{
			get
			{
				return _indentAttributes;
			}
			set
			{
				_indentAttributes = value;
			}
		}

		/// <summary>Replace i by em and b by strong</summary>
		public virtual bool LogicalEmphasis
		{
			get
			{
				return _logicalEmphasis;
			}
			set
			{
				_logicalEmphasis = value;
			}
		}

		/// <summary>If set to true PIs must end with ?></summary>
		public virtual bool XmlPIs
		{
			get
			{
				return _xmlPIs;
			}
			set
			{
				_xmlPIs = value;
			}
		}

		/// <summary>If true text at body is wrapped in &lt;p&gt;'s</summary>
		public virtual bool EncloseText
		{
			get
			{
				return _encloseBodyText;
			}
			set
			{
				_encloseBodyText = value;
			}
		}

		/// <summary>If true text in blocks is wrapped in &lt;p&gt;'s</summary>
		public virtual bool EncloseBlockText
		{
			get
			{
				return _encloseBlockText;
			}
			set
			{
				_encloseBlockText = value;
			}
		}

		/// <summary>Draconian cleaning for Word2000</summary>
		public virtual bool Word2000
		{
			get
			{
				return _word2000;
			}
			set
			{
				_word2000 = value;
			}
		}

		/// <summary>Add meta element indicating tidied doc</summary>
		public virtual bool TidyMark
		{
			get
			{
				return _tidyMark;
			}
			set
			{
				_tidyMark = value;
			}
		}

		/// <summary>If set to yes adds xml:space attr as needed</summary>
		public virtual bool XmlSpace
		{
			get
			{
				return _xmlSpace;
			}
			set
			{
				_xmlSpace = value;
			}
		}

		/// <summary>If true attributes may use newlines</summary>
		public virtual bool LiteralAttribs
		{
			get
			{
				return _literalAttribs;
			}
			set
			{
				_literalAttribs = value;
			}
		}

		/// <summary>
		/// The DOCTYPE
		/// </summary>
		public virtual DocType DocType
		{
			get
			{
				return _docType;
			}
			set
			{
				_docType = value;
			}
		}

		/*
		doctype: omit | auto | strict | loose | <fpi>
		
		where the fpi is a string similar to
		
		"-//ACME//DTD HTML 3.14159//EN"
		*/
		/* protected internal */ 
		internal string ParseDocType(string s, string option)
		{
			s = s.Trim();
			
			/* "-//ACME//DTD HTML 3.14159//EN" or similar */
			
			if (s.StartsWith("\""))
			{
				DocType = TidyNet.DocType.User;
				return s;
			}
			
			/* read first word */
			string word = "";
			Tokenizer t = new Tokenizer(s, " \t\n\r,");
			if (t.HasMoreTokens())
			{
				word = t.NextToken();
			}
			
			if (String.Compare(word, "omit") == 0)
			{
				DocType = TidyNet.DocType.Omit;
			}
			else if (String.Compare(word, "strict") == 0)
			{
				DocType = TidyNet.DocType.Strict;
			}
			else if (String.Compare(word, "loose") == 0 || String.Compare(word, "transitional") == 0)
			{
				DocType = TidyNet.DocType.Loose;
			}
			else if (String.Compare(word, "auto") == 0)
			{
				DocType = TidyNet.DocType.Auto;
			}
			else
			{
				DocType = TidyNet.DocType.Auto;
				Report.BadArgument(option);
			}
			return null;
		}

		/// <summary> DocType - user specified doctype
		/// omit | auto | strict | loose | <i>fpi</i>
		/// where the <i>fpi</i> is a string similar to
		/// &quot;-//ACME//DTD HTML 3.14159//EN&quot;
		/// Note: for <i>fpi</i> include the double-quotes in the string.
		/// </summary>
		public virtual string DocTypeStr
		{
			get
			{
				string result = null;
				switch (_docType)
				{
				case TidyNet.DocType.Omit:
					result = "omit";
					break;
					
				case TidyNet.DocType.Auto:
					result = "auto";
					break;
					
				case TidyNet.DocType.Strict:
					result = "strict";
					break;
					
				case TidyNet.DocType.Loose:
					result = "loose";
					break;
					
				case TidyNet.DocType.User:
					result = _docTypeStr;
					break;
				}
				return result;
			}
			set
			{
				if (value != null)
				{
					_docTypeStr = ParseDocType(value, "doctype");
				}
			}
		}

		internal class Tokenizer
		{
			public Tokenizer(string source, string delimiters)
			{
				_elements = new ArrayList();
				_delimiters = delimiters;
				_elements.AddRange(source.Split(_delimiters.ToCharArray()));
				for (int index = 0; index < _elements.Count; index++)
				{
					if ((string)_elements[index] == "")
					{
						_elements.RemoveAt(index);
						index--;
					}
				}
				_source = source;
			}

			public bool HasMoreTokens()
			{
				return (_elements.Count > 0);			
			}

			public string NextToken()
			{			
				string result;
				if (_source.Length == 0)
				{
					throw new Exception();
				}
				else
				{
					_elements = new ArrayList();
					_elements.AddRange(_source.Split(_delimiters.ToCharArray()));
					for (int index = 0; index < _elements.Count; index++)
					{
						if ((string)_elements[index] == "")
						{
							_elements.RemoveAt(index);
							index--;
						}
					}
					result = (string) _elements[0];
					_elements.RemoveAt(0);				
					_source = _source.Remove(_source.IndexOf(result), result.Length);
					_source = _source.TrimStart(_delimiters.ToCharArray());
					return result;					
				}			
			}

			private ArrayList _elements;
			private string _source;
			private string _delimiters = " \t\n\r";
		}

		internal virtual TagTable tt
		{
			get
			{
				return _tt;
			}
			set
			{
				_tt = value;
			}
		}

		/* ensure that config is self consistent */
		internal virtual void Adjust()
		{
			if (EncloseBlockText)
			{
				EncloseText = true;
			}
			
			/* avoid the need to set IndentContent when SmartIndent is set */
			
			if (SmartIndent)
			{
				IndentContent = true;
			}
			
			/* disable wrapping */
			if (WrapLen == 0)
			{
				WrapLen = 0x7FFFFFFF;
			}
			
			/* Word 2000 needs o:p to be declared as inline */
			if (Word2000)
			{
				tt.DefineInlineTag("o:p");
			}
			
			/* XHTML is written in lower case */
			if (Xhtml)
			{
				XmlOut = true;
				UpperCaseTags = false;
				UpperCaseAttrs = false;
			}
			
			/* if XML in, then XML out */
			if (XmlTags)
			{
				XmlOut = true;
				XmlPIs = true;
			}
			
			/* XML requires end tags */
			if (XmlOut)
			{
				QuoteAmpersand = true;
				HideEndTags = false;
			}
		}
		
		private void ParseInlineTagNames(string s, string option)
		{
			Tokenizer t = new Tokenizer(s, " \t\n\r,");
			while (t.HasMoreTokens())
			{
				tt.DefineInlineTag(t.NextToken());
			}
		}
		
		private void ParseBlockTagNames(string s, string option)
		{
			Tokenizer t = new Tokenizer(s, " \t\n\r,");
			while (t.HasMoreTokens())
			{
				tt.DefineBlockTag(t.NextToken());
			}
		}
		
		private void ParseEmptyTagNames(string s, string option)
		{
			Tokenizer t = new Tokenizer(s, " \t\n\r,");
			while (t.HasMoreTokens())
			{
				tt.defineEmptyTag(t.NextToken());
			}
		}
		
		private void ParsePreTagNames(string s, string option)
		{
			Tokenizer t = new Tokenizer(s, " \t\n\r,");
			while (t.HasMoreTokens())
			{
				tt.DefinePreTag(t.NextToken());
			}
		}
		
		private int _spaces = 2; /* default indentation */
		private int _wrapLen = 68; /* default wrap margin */
		private CharEncoding _charEncoding = TidyNet.CharEncoding.ASCII;
		private int _tabSize = 4;
		
		private TidyNet.DocType _docType = TidyNet.DocType.Auto;
		private string _altText = null; /* default text for alt attribute */
		private string _slideStyle = null; /* style sheet for slides */
		private string _docTypeStr = null; /* user specified doctype */

		private bool _indentContent = false; /* indent content of appropriate tags */
		private bool _smartIndent = false; /* does text/block level content effect indentation */
		private bool _hideEndTags = false; /* suppress optional end tags */
		private bool _xmlTags = false; /* treat input as XML */
		private bool _xmlOut = false; /* create output as XML */
		private bool _xhtml = false; /* output extensible HTML */
		private bool _xmlPi = false; /* add <?xml?> for XML docs */
		private bool _rawOut = false; /* avoid mapping values > 127 to entities */
		private bool _upperCaseTags = false; /* output tags in upper not lower case */
		private bool _upperCaseAttrs = false; /* output attributes in upper not lower case */
		private bool _makeClean = false; /* remove presentational clutter */
		private bool _logicalEmphasis = false; /* replace i by em and b by strong */
		private bool _dropFontTags = false; /* discard presentation tags */
		private bool _dropEmptyParas = true; /* discard empty p elements */
		private bool _fixComments = true; /* fix comments with adjacent hyphens */
		private bool _breakBeforeBR = false; /* o/p newline before <br> or not? */
		private bool _burstSlides = false; /* create slides on each h2 element */
		private bool _numEntities = false; /* use numeric entities */
		private bool _quoteMarks = false; /* output " marks as &quot; */
		private bool _quoteNbsp = true; /* output non-breaking space as entity */
		private bool _quoteAmpersand = true; /* output naked ampersand as &amp; */
		private bool _wrapAttVals = false; /* wrap within attribute values */
		private bool _wrapScriptlets = false; /* wrap within JavaScript string literals */
		private bool _wrapSection = true; /* wrap within <![ ... ]> section tags */
		private bool _wrapAsp = true; /* wrap within ASP pseudo elements */
		private bool _wrapJste = true; /* wrap within JSTE pseudo elements */
		private bool _wrapPhp = true; /* wrap within PHP pseudo elements */
		private bool _fixBackslash = true; /* fix URLs by replacing \ with / */
		private bool _indentAttributes = false; /* newline+indent before each attribute */
		private bool _xmlPIs = false; /* if set to yes PIs must end with ?> */
		private bool _xmlSpace = false; /* if set to yes adds xml:space attr as needed */
		private bool _encloseBodyText = false; /* if yes text at body is wrapped in <p>'s */
		private bool _encloseBlockText = false; /* if yes text in blocks is wrapped in <p>'s */
		private bool _word2000 = false; /* draconian cleaning for Word2000 */
		private bool _tidyMark = true; /* add meta element indicating tidied doc */
		private bool _literalAttribs = false; /* if true attributes may use newlines */
		private TagTable _tt = new TagTable();
	}
}
