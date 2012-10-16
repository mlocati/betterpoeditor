using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace BePoE.UI
{
	public partial class frmSearch : Form
	{
		private const string PLACE_SOURCE = "Source texts";
		private const string PLACE_TRANSLATED = "Translated texts";
		public enum SearchPlace
		{
			Source,
			SourceComment,
			Context,
			Reference,
			Translated,
			TranslatorComment,
		}
		private enum Statuses : int
		{
			Collapsed = 0,
			Expanded_Working = 1,
			Expanded_Idle = 2,
		}
		private Size? _collapsedMinSize = null;
		private Size? _collapsedMaxSize = null;
		private Size? _expandedMinSize = null;
		private Size? _expandedMaxSize = null;

		private Statuses _status = Statuses.Collapsed;
		private Statuses Status
		{
			get
			{ return this._status; }
			set
			{
				this.sslStop.Visible = this.sspSearch.Visible = (value == Statuses.Expanded_Working);
				if (value >= Statuses.Expanded_Working)
					this.gbxSearchResults.Visible = true;
				if (value >= Statuses.Expanded_Working)
					this.SetSize(true);
				else
					this.SetSize(false);
				if (value < Statuses.Expanded_Working)
					this.gbxSearchResults.Visible = false;
				if (value < Statuses.Expanded_Idle)
					this.scFoundItems.Panel2Collapsed = true;
				this._status = value;
			}
		}

		public interface ISearchConnection
		{
			void HideWin();
			void WinUnloaded();
			List<PO.Entry> GetCurrentItems();
			void ShowResult(SearchResult result, Form formToActivate);
		}
		private ISearchConnection _searchConnection;
		public frmSearch(ISearchConnection searchConnection)
		{
			InitializeComponent();
			Utils.Gfx.SetFormIcon(this);
			this._expandedMinSize = this.MinimumSize;
			this._expandedMaxSize = new Size(0, 0);
			this._collapsedMinSize = new Size(this.Width - this.ClientRectangle.Width + 2 * this.tcPages.Left + this.tcPages.Width, this.MinimumSize.Height);
			this._collapsedMaxSize = new Size(this._collapsedMinSize.Value.Width, int.MaxValue);
			this._searchConnection = searchConnection;
			this.Status = Statuses.Collapsed;
			this.cbxTidy_Where.Items.Clear();
			this.cbxTidy_Where.Items.Add(PLACE_SOURCE);
			this.cbxTidy_Where.Items.Add(PLACE_TRANSLATED);
			this.cbxTidy_Where.SelectedIndex = 1;

		}
		private int GetSize(int value, int min, int max)
		{
			if (min != 0)
				value = Math.Max(value, min);
			if (max != 0)
				value = Math.Min(value, max);
			return value;
		}
		private int? _lastExpandedWidth = null;
		private void SetSize(bool expanded)
		{
			if (this.Width > this._collapsedMaxSize.Value.Width)
				this._lastExpandedWidth = this.Width;
			int wannaWidth = (expanded && this._lastExpandedWidth.HasValue) ? this._lastExpandedWidth.Value : this.Width;
			Size min = (expanded ? this._expandedMinSize.Value : this._collapsedMinSize.Value);
			Size max = (expanded ? this._expandedMaxSize.Value : this._collapsedMaxSize.Value);
			this.MinimumSize = new Size(0, 0);
			this.MaximumSize = new Size(0, 0);
			this.Width = GetSize(wannaWidth, min.Width, max.Width);
			this.Height = GetSize(this.Height, min.Height, max.Height);
			this.MinimumSize = min;
			this.MaximumSize = max;
		}

		private void frmSearch_FormClosing(object sender, FormClosingEventArgs e)
		{
			switch (e.CloseReason)
			{
				case CloseReason.FormOwnerClosing:
					this._searchConnection.WinUnloaded();
					break;
				default:
					e.Cancel = true;
					this._searchConnection.HideWin();
					break;
			}
		}

		private void btnStd_Go_Click(object sender, EventArgs e)
		{
			WorgDataStandard dta = WorgDataStandard.Get(this);
			if (dta == null)
				return;
			this.StartSearch(dta);
		}

		private abstract class WorgData
		{
			public readonly List<PO.Entry> Entries;
			protected WorgData(List<PO.Entry> entries)
			{
				this.Entries = entries;
			}
			public abstract List<SearchResult> Search(int entityIndex);
		}
		private class WorgDataStandard : WorgData
		{
			public readonly string SearchTerm;
			public readonly bool CaseSensitive;
			public readonly bool WholeWords;
			public readonly bool RegEx;
			public readonly List<SearchPlace> SearchPlaces;
			private WorgDataStandard(List<PO.Entry> entries, string searchTerm, bool caseSensitive, bool wholeWords, bool regEx, List<SearchPlace> searchPlaces)
				: base(entries)
			{
				this.SearchTerm = searchTerm;
				this.CaseSensitive = caseSensitive;
				this.WholeWords = wholeWords;
				this.RegEx = regEx;
				this.SearchPlaces = searchPlaces;
			}
			public static WorgDataStandard Get(frmSearch frm)
			{
				List<PO.Entry> entries = frm._searchConnection.GetCurrentItems();
				if ((entries == null) || (entries.Count == 0))
				{
					MessageBox.Show("No search entries in the main form.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
					return null;
				}
				string searchTerm = frm.tbxStd_Term.Text;
				if (searchTerm.Length == 0)
				{
					MessageBox.Show("No search term specified.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
					frm.tcPages.SelectedTab = frm.tpStandard;
					frm.tbxStd_Term.Focus();
					return null;
				}
				List<SearchPlace> searchPlaces = new List<SearchPlace>();
				if (frm.chkStd_WSource.Checked)
					searchPlaces.Add(SearchPlace.Source);
				if (frm.chkStd_WSourceComment.Checked)
					searchPlaces.Add(SearchPlace.SourceComment);
				if (frm.chkStd_WContext.Checked)
					searchPlaces.Add(SearchPlace.Context);
				if (frm.chkStd_WReference.Checked)
					searchPlaces.Add(SearchPlace.Reference);
				if (frm.chkStd_WTranslated.Checked)
					searchPlaces.Add(SearchPlace.Translated);
				if (frm.chkStd_WTranslatorComment.Checked)
					searchPlaces.Add(SearchPlace.TranslatorComment);
				if (searchPlaces.Count == 0)
				{
					MessageBox.Show("No search scope specified.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
					frm.tcPages.SelectedTab = frm.tpStandard;
					frm.chkStd_WSource.Focus();
					return null;
				}
				return new WorgDataStandard(entries, searchTerm, frm.chkStd_Case.Checked, frm.chkStd_WholeWords.Checked, frm.chkStd_RegEx.Checked, searchPlaces);
			}
			private delegate List<SearchResultLocation> SearcherHandler(string text, int? index);
			private SearcherHandler _searcher = null;
			private static void MergeList(List<SearchResultLocation> total, List<SearchResultLocation> addMe)
			{
				if ((addMe != null) && (addMe.Count > 0))
					total.AddRange(addMe.ToArray());
			}
			public override List<SearchResult> Search(int entityIndex)
			{
				if (this._searcher == null)
				{
					if (this.RegEx)
						this._searcher = this.Searcher_RegEx;
					else if (this.WholeWords)
						this._searcher = this.Searcher_WholeWords;
					else
						this._searcher = this.Searcher_Anywhere;
				}
				PO.Entry entry = this.Entries[entityIndex];
				List<SearchResult> result = new List<SearchResult>();
				PO.DataEntry dataEntry = entry as PO.DataEntry;
				PO.SingleDataEntry dataEntry1 = dataEntry as PO.SingleDataEntry;
				PO.PluralDataEntry dataEntryN = dataEntry as PO.PluralDataEntry;
				PO.RemovedEntry removedEntry = entry as PO.RemovedEntry;
				PO.SingleRemovedEntry removedEntry1 = removedEntry as PO.SingleRemovedEntry;
				PO.PluralRemovedEntry removedEntryN = removedEntry as PO.PluralRemovedEntry;
				foreach (SearchPlace place in this.SearchPlaces)
				{
					List<SearchResultLocation> ris1;
					switch (place)
					{
						case SearchPlace.SourceComment:
							ris1 = this._searcher(entry.ExtractedComment, null);
							break;
						case SearchPlace.TranslatorComment:
							ris1 = this._searcher(entry.TranslatorComment, null);
							break;
						case SearchPlace.Context:
							if (dataEntry != null)
								ris1 = this._searcher(dataEntry.Context, null);
							else
								ris1 = this._searcher(removedEntry.RemovedContext, null);
							break;
						case SearchPlace.Reference:
							ris1 = new List<SearchResultLocation>();
							for (int i = 0; i < entry.Reference.Length; i++)
							{
								MergeList(ris1, this._searcher(entry.Reference[i], i));
							}
							break;
						case SearchPlace.Source:
							ris1 = new List<SearchResultLocation>();
							if (dataEntry != null)
							{
								MergeList(ris1, this._searcher(dataEntry.ID, (dataEntryN == null) ? (int?)null : 0));
								if (dataEntryN != null)
									MergeList(ris1, this._searcher(dataEntryN.IDPlural, 1));
							}
							else
							{
								MergeList(ris1, this._searcher(removedEntry.RemovedID, (removedEntryN == null) ? (int?)null : 0));
								if (removedEntryN != null)
									MergeList(ris1, this._searcher(removedEntryN.RemovedIDPlural, 1));
							}
							break;
						case SearchPlace.Translated:
							ris1 = new List<SearchResultLocation>();
							if (dataEntry1 != null)
								MergeList(ris1, this._searcher(dataEntry1.Translated, null));
							else if (dataEntryN != null)
							{
								for (int i = 0; i < dataEntryN.NumTranslated; i++)
									MergeList(ris1, this._searcher(dataEntryN.get_Translated(i), i));
							}
							else if (removedEntry1 != null)
								MergeList(ris1, this._searcher(removedEntry1.RemovedTranslated, null));
							else
							{
								for (int i = 0; i < removedEntryN.RemovedTranslated.Length; i++)
									MergeList(ris1, this._searcher(removedEntryN.RemovedTranslated[i], i));
							}
							break;
						default:
							throw new NotImplementedException();
					}
					foreach (SearchResultLocation location in ris1)
					{
						if (location == null)
							result.Add(new SearchResult(entry, place));
						else
							result.Add(new SearchResult(entry, place, location.Index, location.Start, location.Length));
					}
				}
				return result;
			}
			private List<SearchResultLocation> Searcher_Anywhere(string text, int? index)
			{
				List<SearchResultLocation> ris = new List<SearchResultLocation>();
				int pLast = -1;
				int length = this.SearchTerm.Length;
				for (; ; )
				{
					int p = text.IndexOf(this.SearchTerm, pLast + 1, this.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
					if (p < 0)
						return ris;
					ris.Add(new SearchResultLocation(index, p, length));
					pLast = p;
				}
			}
			private Regex _searcher_RegExRegex = null;
			private List<SearchResultLocation> Searcher_RegEx(string text, int? index)
			{
				if (this._searcher_RegExRegex == null)
				{
					RegexOptions options = RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Multiline;
					if (!this.CaseSensitive)
						options |= RegexOptions.IgnoreCase;
					this._searcher_RegExRegex = new Regex(this.WholeWords ? string.Format(@"(^|\b){0}($|\b)", this.SearchTerm) : this.SearchTerm, options);
				}
				List<SearchResultLocation> ris = new List<SearchResultLocation>();
				foreach (Match match in this._searcher_RegExRegex.Matches(text))
				{
					ris.Add(new SearchResultLocation(index, match.Index, match.Length));
				}
				return ris;
			}

			private Regex _searcher_WholeWordsRegex = null;
			private List<SearchResultLocation> Searcher_WholeWords(string text, int? index)
			{
				if (this._searcher_WholeWordsRegex == null)
				{
					RegexOptions options = RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Multiline;
					if (!this.CaseSensitive)
						options |= RegexOptions.IgnoreCase;
					this._searcher_WholeWordsRegex = new Regex(string.Format(@"(^|\b)(?<g>{0})($|\b)", Regex.Escape(this.SearchTerm)), options);
				}
				List<SearchResultLocation> ris = new List<SearchResultLocation>();
				int length = this.SearchTerm.Length;
				foreach (Match match in this._searcher_WholeWordsRegex.Matches(text))
				{
					ris.Add(new SearchResultLocation(index, match.Groups["g"].Index, length));
				}
				return ris;
			}
		}

		private class WorkDataTidy : WorgData
		{
			public readonly SearchPlace Place;
			public readonly bool ReportErrors;
			public readonly bool ReportWarnings;
			public readonly bool UnescapeStrings;
			public static WorkDataTidy Get(frmSearch frm)
			{
				List<PO.Entry> entries = frm._searchConnection.GetCurrentItems();
				if ((entries == null) || (entries.Count == 0))
				{
					MessageBox.Show("No search entries in the main form.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
					return null;
				}
				SearchPlace place;
				switch (frm.cbxTidy_Where.SelectedItem as string)
				{
					case PLACE_SOURCE:
						place = SearchPlace.Source;
						break;
					case PLACE_TRANSLATED:
						place = SearchPlace.Translated;
						break;
					default:
						MessageBox.Show("Please specify the search place.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
						frm.cbxTidy_Where.Focus();
						return null;
				}

				bool reportErrors = frm.chkTidy_Errs.Checked;
				bool reportWarnings = frm.chkTidy_Warns.Checked;
				if (!(reportErrors || reportWarnings))
				{
					MessageBox.Show("Please specify at least one message kind to report.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
					frm.chkTidy_Errs.Focus();
					return null;
				}
				return new WorkDataTidy(entries, place, reportErrors, reportWarnings, frm.chkTidy_Unescape.Checked);
			}
			private WorkDataTidy(List<PO.Entry> entries, SearchPlace place, bool reportErrors, bool reportWarnings, bool unescapeStrings)
				: base(entries)
			{
				this.Place = place;
				this.ReportErrors = reportErrors;
				this.ReportWarnings = reportWarnings;
				this.UnescapeStrings = unescapeStrings;
			}
			private static string UnescapeString(string text)
			{
				if (string.IsNullOrEmpty(text))
					return text;
				StringBuilder ris = new StringBuilder();
				int p0 = 0;
				for (; ; )
				{
					int p = text.IndexOf('\\', p0);
					if (p < 0)
					{
						if (p0 == 0)
							return text;
						else
							ris.Append(text.Substring(p0));
						break;
					}
					if (p > p0)
						ris.Append(text.Substring(p0, p - p0));
					if (p == (text.Length - 1))
						return text; //Escape char without escaped char
					switch (text[p + 1])
					{
						case '\\':
						case '"':
							ris.Append(text[p + 1]);
							break;
						case 'n':
						case 'r':
							ris.Append(Environment.NewLine);
							break;
						case 't':
							ris.Append('\t');
							break;
						default:
							return text; //Unknown escaped char: text[p + 1]
					}
					p0 = p + 2;
					if (p0 == (text.Length))
						break;
				}
				return ris.ToString();
			}
			public override List<SearchResult> Search(int entityIndex)
			{
				PO.Entry entry = this.Entries[entityIndex];
				List<SearchResult> result = new List<SearchResult>();
				PO.DataEntry dataEntry = entry as PO.DataEntry;
				PO.SingleDataEntry dataEntry1 = dataEntry as PO.SingleDataEntry;
				PO.PluralDataEntry dataEntryN = dataEntry as PO.PluralDataEntry;
				PO.RemovedEntry removedEntry = entry as PO.RemovedEntry;
				PO.SingleRemovedEntry removedEntry1 = removedEntry as PO.SingleRemovedEntry;
				PO.PluralRemovedEntry removedEntryN = removedEntry as PO.PluralRemovedEntry;
				int?[] indexes;
				string[] texts;
				switch (this.Place)
				{
					case SearchPlace.Source:
						if (dataEntry1 != null)
						{
							indexes = new int?[] { null };
							texts = new string[] { dataEntry1.ID };
						}
						else if (dataEntryN != null)
						{
							indexes = new int?[] { 0, 1 };
							texts = new string[] { dataEntryN.ID, dataEntryN.IDPlural };
						}
						else if (removedEntry1 != null)
						{
							indexes = new int?[] { null };
							texts = new string[] { removedEntry1.RemovedID };
						}
						else
						{
							indexes = new int?[] { 0, 1 };
							texts = new string[] { removedEntryN.RemovedID, removedEntryN.RemovedIDPlural };
						}
						break;
					case SearchPlace.Translated:
						if (dataEntry1 != null)
						{
							indexes = new int?[] { null };
							texts = new string[] { dataEntry1.Translated };
						}
						else if (dataEntryN != null)
						{
							indexes = new int?[dataEntryN.NumTranslated];
							texts = new string[dataEntryN.NumTranslated];
							for (int t = 0; t < indexes.Length; t++)
							{
								indexes[t] = t;
								texts[t] = dataEntryN.get_Translated(t);
							}
						}
						else if (removedEntry1 != null)
						{
							indexes = new int?[] { null };
							texts = new string[] { removedEntry1.RemovedTranslated };
						}
						else
						{
							indexes = new int?[removedEntryN.RemovedTranslated.Length];
							texts = new string[removedEntryN.RemovedTranslated.Length];
							for (int t = 0; t < indexes.Length; t++)
							{
								indexes[t] = t;
								texts[t] = removedEntryN.RemovedTranslated[t];
							}
						}
						break;
					default:
						throw new NotImplementedException();
				}
				for (int i = 0; i < indexes.Length; i++)
				{
					string text = this.UnescapeStrings ? UnescapeString(texts[i]) : texts[i];
					if (string.IsNullOrEmpty(text))
						continue;
					foreach (Utils.Tidy.Result ris1 in Utils.Tidy.Result.Analyze(text))
					{
						bool ok;
						switch (ris1.Level)
						{
							case Utils.Tidy.Result.Levels.Error:
								ok = this.ReportErrors;
								break;
							case Utils.Tidy.Result.Levels.Warning:
								ok = this.ReportWarnings;
								break;
							default:
								ok = false;
								break;
						}
						if (ok)
							result.Add(new SearchResultDescribed(entry, this.Place, indexes[i], ris1.SelectionStart, ris1.SelectionLength, ris1.ToString(true)));
					}
				}
				return result;
			}
		}

		private class WorkDataSame : WorgData
		{
			public readonly bool CaseSensitive;
			public static WorkDataSame Get(frmSearch frm)
			{
				List<PO.Entry> entries = frm._searchConnection.GetCurrentItems();
				if ((entries == null) || (entries.Count == 0))
				{
					MessageBox.Show("No search entries in the main form.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
					return null;
				}
				return new WorkDataSame(entries, frm.chkSame_Case.Checked);
			}
			private WorkDataSame(List<PO.Entry> entries, bool caseSensitive)
				: base(entries)
			{
				this.CaseSensitive = caseSensitive;
			}
			public override List<SearchResult> Search(int entityIndex)
			{
				List<SearchResult> result = new List<SearchResult>();
				PO.Entry entry = this.Entries[entityIndex];
				PO.SingleDataEntry dataEntry1 = entry as PO.SingleDataEntry;
				if (dataEntry1 != null)
				{
					if (dataEntry1.ID.Equals(dataEntry1.Translated, this.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase))
						result.Add(new SearchResult(entry, new SearchPlace[] { SearchPlace.Source, SearchPlace.Translated }, new int?[] { null, null }, new int?[] { 0, 0 }, new int?[] { dataEntry1.ID.Length, dataEntry1.Translated.Length }));
				}
				else
				{
					PO.PluralDataEntry dataEntryN = entry as PO.PluralDataEntry;
					if (dataEntryN != null)
					{
						for (int i = 0; i < 2; i++)
						{
							string id = (i == 0) ? dataEntryN.ID : dataEntryN.IDPlural;
							for (int j = 0; j < dataEntryN.NumTranslated; j++)
							{
								if (id.Equals(dataEntryN.get_Translated(j), this.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase))
									result.Add(new SearchResult(entry, new SearchPlace[] { SearchPlace.Source, SearchPlace.Translated }, new int?[] { i, j }, new int?[] { 0, 0 }, new int?[] { id.Length, dataEntryN.get_Translated(j).Length }));
							}
						}
					}
					else
					{
						PO.SingleRemovedEntry removedEntry1 = entry as PO.SingleRemovedEntry;
						if (removedEntry1 != null)
						{
							if (removedEntry1.RemovedID.Equals(removedEntry1.RemovedTranslated, this.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase))
								result.Add(new SearchResult(entry, new SearchPlace[] { SearchPlace.Source, SearchPlace.Translated }, new int?[] { null, null }, new int?[] { 0, 0 }, new int?[] { removedEntry1.RemovedID.Length, removedEntry1.RemovedTranslated.Length }));
						}
						else
						{
							PO.PluralRemovedEntry removedEntryN = entry as PO.PluralRemovedEntry;
							for (int i = 0; i < 2; i++)
							{
								string id = (i == 0) ? removedEntryN.RemovedID : removedEntryN.RemovedIDPlural;
								for (int j = 0; j < removedEntryN.RemovedTranslated.Length; j++)
								{
									if (id.Equals(removedEntryN.RemovedTranslated[j], this.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase))
										result.Add(new SearchResult(entry, new SearchPlace[] { SearchPlace.Source, SearchPlace.Translated }, new int?[] { i, j }, new int?[] { 0, 0 }, new int?[] { id.Length, removedEntryN.RemovedTranslated[j].Length }));
								}
							}
						}
					}
				}
				return result;
			}
		}

		private class WorkDataNumOccur : WorgData
		{
			public readonly string Term;
			public static WorkDataNumOccur Get(frmSearch frm)
			{
				List<PO.Entry> entries = frm._searchConnection.GetCurrentItems();
				if ((entries == null) || (entries.Count == 0))
				{
					MessageBox.Show("No search entries in the main form.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
					return null;
				}
				string term = frm.tbxNumOccur_Text.Text;
				if (term.Length == 0)
				{
					MessageBox.Show("Please specify what to look for.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
					frm.tbxNumOccur_Text.Focus();
					return null;
				}
				return new WorkDataNumOccur(entries, term);
			}
			private WorkDataNumOccur(List<PO.Entry> entries, string term)
				: base(entries)
			{
				this.Term = term;
			}
			private int GetNumOccurs(string text)
			{
				if (string.IsNullOrEmpty(text))
					return 0;
				int n = 0;
				for (int i = -1; (i = text.IndexOf(this.Term, i + 1, StringComparison.InvariantCulture)) >= 0; )
					n++;
				return n;
			}
			private bool HasDifferences(string text1, string text2)
			{
				return this.GetNumOccurs(text1) != this.GetNumOccurs(text2);
			}
			public override List<SearchResult> Search(int entityIndex)
			{
				List<SearchResult> result = new List<SearchResult>();
				PO.Entry entry = this.Entries[entityIndex];
				PO.SingleDataEntry dataEntry1 = entry as PO.SingleDataEntry;
				if (dataEntry1 != null)
				{
					if (this.HasDifferences(dataEntry1.ID, dataEntry1.Translated))
						result.Add(new SearchResult(entry, new SearchPlace[] { SearchPlace.Source, SearchPlace.Translated }, new int?[] { null, null }, new int?[] { 0, 0 }, new int?[] { dataEntry1.ID.Length, dataEntry1.Translated.Length }));
				}
				else
				{
					PO.PluralDataEntry dataEntryN = entry as PO.PluralDataEntry;
					if (dataEntryN != null)
					{
						for (int i = 0; i < 2; i++)
						{
							string id = (i == 0) ? dataEntryN.ID : dataEntryN.IDPlural;
							for (int j = 0; j < dataEntryN.NumTranslated; j++)
							{
								if (this.HasDifferences(id, dataEntryN.get_Translated(j)))
									result.Add(new SearchResult(entry, new SearchPlace[] { SearchPlace.Source, SearchPlace.Translated }, new int?[] { i, j }, new int?[] { 0, 0 }, new int?[] { id.Length, dataEntryN.get_Translated(j).Length }));
							}
						}
					}
					else
					{
						PO.SingleRemovedEntry removedEntry1 = entry as PO.SingleRemovedEntry;
						if (removedEntry1 != null)
						{
							if (this.HasDifferences(removedEntry1.RemovedID, removedEntry1.RemovedTranslated))
								result.Add(new SearchResult(entry, new SearchPlace[] { SearchPlace.Source, SearchPlace.Translated }, new int?[] { null, null }, new int?[] { 0, 0 }, new int?[] { removedEntry1.RemovedID.Length, removedEntry1.RemovedTranslated.Length }));
						}
						else
						{
							PO.PluralRemovedEntry removedEntryN = entry as PO.PluralRemovedEntry;
							for (int i = 0; i < 2; i++)
							{
								string id = (i == 0) ? removedEntryN.RemovedID : removedEntryN.RemovedIDPlural;
								for (int j = 0; j < removedEntryN.RemovedTranslated.Length; j++)
								{
									if (this.HasDifferences(id, removedEntryN.RemovedTranslated[j]))
										result.Add(new SearchResult(entry, new SearchPlace[] { SearchPlace.Source, SearchPlace.Translated }, new int?[] { i, j }, new int?[] { 0, 0 }, new int?[] { id.Length, removedEntryN.RemovedTranslated[j].Length }));
								}
							}
						}
					}
				}
				return result;
			}
		}

		private class WorkDataSpellCheck : WorgData, IDisposable
		{
			public readonly SearchPlace Place;
			private readonly bool IgnoreAllCapsWords;
			private readonly bool IgnoreHtml;
			private readonly bool IgnoreWordsWithDigits;
			public static WorkDataSpellCheck Get(frmSearch frm)
			{
				List<PO.Entry> entries = frm._searchConnection.GetCurrentItems();
				if ((entries == null) || (entries.Count == 0))
				{
					MessageBox.Show("No search entries in the main form.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
					return null;
				}
				return new WorkDataSpellCheck(entries, SearchPlace.Translated, frm.chkSpellCheck_IgnoreAllCapsWords.Checked, frm.chkSpellCheck_IgnoreHtml.Checked, frm.chkSpellCheck_IgnoreWordsWithDigits.Checked);
			}
			private WorkDataSpellCheck(List<PO.Entry> entries, SearchPlace place, bool ignoreAllCapsWords, bool ignoreHtml, bool ignoreWordsWithDigits)
				: base(entries)
			{
				this.Place = place;
				this.IgnoreAllCapsWords = ignoreAllCapsWords;
				this.IgnoreHtml = ignoreHtml;
				this.IgnoreWordsWithDigits = ignoreWordsWithDigits;
			}
			private Utils.SpellChecker.Worker _worker = null;
			public void Dispose()
			{
				if (this._worker != null)
				{
					try
					{ this._worker.Dispose(); }
					catch
					{ }
				}
			}
			public override List<SearchResult> Search(int entityIndex)
			{
				PO.Entry entry = this.Entries[entityIndex];
				List<SearchResult> result = new List<SearchResult>();
				PO.DataEntry dataEntry = entry as PO.DataEntry;
				PO.SingleDataEntry dataEntry1 = dataEntry as PO.SingleDataEntry;
				PO.PluralDataEntry dataEntryN = dataEntry as PO.PluralDataEntry;
				PO.RemovedEntry removedEntry = entry as PO.RemovedEntry;
				PO.SingleRemovedEntry removedEntry1 = removedEntry as PO.SingleRemovedEntry;
				PO.PluralRemovedEntry removedEntryN = removedEntry as PO.PluralRemovedEntry;
				int?[] indexes;
				string[] texts;
				switch (this.Place)
				{
					case SearchPlace.Source:
						if (dataEntry1 != null)
						{
							indexes = new int?[] { null };
							texts = new string[] { dataEntry1.ID };
						}
						else if (dataEntryN != null)
						{
							indexes = new int?[] { 0, 1 };
							texts = new string[] { dataEntryN.ID, dataEntryN.IDPlural };
						}
						else if (removedEntry1 != null)
						{
							indexes = new int?[] { null };
							texts = new string[] { removedEntry1.RemovedID };
						}
						else
						{
							indexes = new int?[] { 0, 1 };
							texts = new string[] { removedEntryN.RemovedID, removedEntryN.RemovedIDPlural };
						}
						break;
					case SearchPlace.Translated:
						if (dataEntry1 != null)
						{
							indexes = new int?[] { null };
							texts = new string[] { dataEntry1.Translated };
						}
						else if (dataEntryN != null)
						{
							indexes = new int?[dataEntryN.NumTranslated];
							texts = new string[dataEntryN.NumTranslated];
							for (int t = 0; t < indexes.Length; t++)
							{
								indexes[t] = t;
								texts[t] = dataEntryN.get_Translated(t);
							}
						}
						else if (removedEntry1 != null)
						{
							indexes = new int?[] { null };
							texts = new string[] { removedEntry1.RemovedTranslated };
						}
						else
						{
							indexes = new int?[removedEntryN.RemovedTranslated.Length];
							texts = new string[removedEntryN.RemovedTranslated.Length];
							for (int t = 0; t < indexes.Length; t++)
							{
								indexes[t] = t;
								texts[t] = removedEntryN.RemovedTranslated[t];
							}
						}
						break;
					default:
						throw new NotImplementedException();
				}
				for (int i = 0; i < indexes.Length; i++)
				{
					if (string.IsNullOrEmpty(texts[i]))
						continue;
					if (this._worker == null)
					{
						this._worker = Program.SpellChecker.GetWorker(entry.File.Language);
						this._worker.IgnoreAllCapsWords = this.IgnoreAllCapsWords;
						this._worker.IgnoreHtml = this.IgnoreHtml;
						this._worker.IgnoreWordsWithDigits = this.IgnoreWordsWithDigits;
					}
					foreach (Utils.SpellChecker.Result ris1 in this._worker.Analyze(texts[i]))
						result.Add(new SearchResultDescribed(entry, this.Place, indexes[i], ris1.SelectionStart, ris1.SelectionLength, ris1.ToString(true)));
				}
				return result;
			}
		}
		protected override void OnClosed(EventArgs e)
		{
			this.StopSearch(Statuses.Collapsed, true);
		}

		private class SearchResultLocation
		{
			public readonly int? Index;
			public readonly int? Start;
			public readonly int? Length;
			public SearchResultLocation(int? index, int? start, int? length)
			{
				this.Index = index;
				this.Start = start;
				this.Length = length;
			}
		}
		public class SearchResult
		{
			public readonly PO.Entry Entry;
			[ReadOnly(true)]
			public readonly List<SearchPlace> Places;
			[ReadOnly(true)]
			public readonly List<int?> Indexes;
			[ReadOnly(true)]
			public readonly List<int?> Starts;
			[ReadOnly(true)]
			public readonly List<int?> Lengths;
			private SearchResult(PO.Entry entry)
			{
				this.Entry = entry;
				this.Places = new List<SearchPlace>();
				this.Indexes = new List<int?>();
				this.Starts = new List<int?>();
				this.Lengths = new List<int?>();
			}
			public SearchResult(PO.Entry entry, SearchPlace place)
				: this(entry, place, null, null, null) { }
			public SearchResult(PO.Entry entry, SearchPlace place, int? index, int? start, int? length)
				: this(entry, new SearchPlace[] { place } , new int?[] { index } , new int?[] { start } , new int?[] { length })
			{
			}
			public SearchResult(PO.Entry entry, List<SearchPlace> places, List<int?> indexes, List<int?> starts, List<int?> lengths)
				: this(entry, places.ToArray(), indexes.ToArray(), starts.ToArray(), lengths.ToArray())
			{
			}
			public SearchResult(PO.Entry entry, SearchPlace[] places, int?[] indexes, int?[] starts, int?[] lengths)
				: this(entry)
			{
				this.Places.AddRange(places);
				this.Indexes.AddRange(indexes);
				this.Starts.AddRange(starts);
				this.Lengths.AddRange(lengths);
			}
			public override string ToString()
			{
				switch (this.Entry.Kind)
				{
					case PO.Entry.Kinds.Standard:
						return ((PO.DataEntry)this.Entry).ID;
					case PO.Entry.Kinds.Removed:
						return ((PO.RemovedEntry)this.Entry).RemovedID;
				}
				return base.ToString();
			}
		}
		public class SearchResultDescribed : SearchResult
		{
			public readonly string Description;
			public SearchResultDescribed(PO.Entry entry, SearchPlace place, string description)
				: this(entry, place, null, null, null, description) { }
			public SearchResultDescribed(PO.Entry entry, SearchPlace place, int? index, int? start, int? length, string description)
				: this(entry, new SearchPlace[] { place } , new int?[] { index } , new int?[] { start } , new int?[] { length } , description)
			{
			}
			public SearchResultDescribed(PO.Entry entry, List<SearchPlace> places, List<int?> indexes, List<int?> starts, List<int?> lengths, string description)
				: this(entry, places.ToArray(), indexes.ToArray(), starts.ToArray(), lengths.ToArray(), description)
			{
			}
			public SearchResultDescribed(PO.Entry entry, SearchPlace[] places, int?[] indexes, int?[] starts, int?[] lengths, string description)
				: base(entry, places, indexes, starts, lengths)
			{
				this.Description = (description == null) ? "" : description;
			}

		}
		private class ProgressData
		{
			public readonly int Position;
			public readonly List<SearchResult> NewlyFoundEntries;
			public ProgressData(int position, List<SearchResult> newlyFoundEntries)
			{
				this.Position = position;
				this.NewlyFoundEntries = newlyFoundEntries;
			}
		}

		private BackgroundWorker _currentWorker = null;
		private WorgData _currentWorkerData = null;
		private void StartSearch(WorgData dta)
		{
			this.StopSearch(Statuses.Expanded_Working, true);
			this.sslSearch.ToolTipText = "";
			this.sslSearch.BackColor = SystemColors.Control;
			this.sslSearch.ForeColor = SystemColors.WindowText;
			this.sslSearch.Text = "Searching...";
			this.sspSearch.Minimum = 0;
			this.sspSearch.Value = 0;
			this.sspSearch.Maximum = dta.Entries.Count;
			this.ssSarch.Visible = true;
			this.sslStop.Visible = true;
			this._currentWorkerData = dta;
			try
			{
				this._currentWorker = new BackgroundWorker();
				this._currentWorker.WorkerReportsProgress = true;
				this._currentWorker.WorkerSupportsCancellation = true;
				this._currentWorker.DoWork += this._currentWorker_DoWork;
				this._currentWorker.ProgressChanged += this._currentWorker_ProgressChanged;
				this._currentWorker.RunWorkerCompleted += this._currentWorker_RunWorkerCompleted;
				this._currentWorker.RunWorkerAsync(dta);
			}
			catch (Exception x)
			{
				this._currentWorker = null;
				this._currentWorker_RunWorkerCompleted(null, new RunWorkerCompletedEventArgs(null, x, false));
			}
		}

		public void ClearAllData()
		{
			this.StopSearch(Statuses.Collapsed, true);
		}
		private void StopSearch(Statuses statusToSet, bool clearSearchResults)
		{
			if (this._currentWorker != null)
			{
				this._currentWorker.CancelAsync();
				while (this._currentWorker != null)
					Thread.Sleep(10);
			}
			if (this.Status != statusToSet)
				this.Status = statusToSet;
			if (clearSearchResults)
			{
				if (this.lbxFound.Items.Count > 0)
					this.lbxFound.Items.Clear();
				if (!this.scFoundItems.Panel2Collapsed)
					this.scFoundItems.Panel2Collapsed = true;
			}
		}

		private void _currentWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
				Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			}
			catch
			{ }
			try
			{
				int totalFound = 0;
				WorgData data = (WorgData)e.Argument;
				BackgroundWorker bgw = (BackgroundWorker)sender;
				for (int index = 0; index < data.Entries.Count; index++)
				{
					List<SearchResult> ris1 = data.Search(index);
					if (ris1 != null)
					{
						totalFound += ris1.Count;
						if (totalFound > Program.Vars.MaxSearchResults)
						{
							e.Result = new Exception("Too many results");
							return;
						}
					}
					bgw.ReportProgress(0, new ProgressData(index, ris1));
					if (bgw.CancellationPending)
					{
						e.Cancel = true;
						return;
					}
				}
			}
			catch (Exception x)
			{
				e.Result = x;
			}
			finally
			{
				this._currentWorker = null;
			}
		}

		private void _currentWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			ProgressData p = (ProgressData)e.UserState;
			this.sspSearch.Value = p.Position;
			if ((p.NewlyFoundEntries != null) && (p.NewlyFoundEntries.Count > 0))
				this.lbxFound.Items.AddRange(p.NewlyFoundEntries.ToArray());
			this.sspSearch.ToolTipText = string.Format("{0:N0} / {1:N0}", p.Position, this.sspSearch.Maximum);
		}

		private void _currentWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (this._currentWorkerData != null)
			{
				IDisposable id = this._currentWorkerData as IDisposable;
				if (id != null)
					id.Dispose();
				this._currentWorkerData = null;
			}
			if (e.Cancelled)
				return;
			this.Status = Statuses.Expanded_Idle;
			Exception x;
			if (e.Error != null)
				x = e.Error;
			else
				x = e.Result as Exception;
			if (x != null)
			{
				this.sslSearch.BackColor = SystemColors.Highlight;
				this.sslSearch.ForeColor = SystemColors.HighlightText;
				this.sslSearch.Text = x.Message;
			}
			else
			{
				this.sslSearch.BackColor = SystemColors.Control;
				this.sslSearch.ForeColor = SystemColors.WindowText;
				this.sslSearch.Text = (this.lbxFound.Items.Count == 1) ? "1 item found." : string.Format("{0:N0} items found.", this.lbxFound.Items.Count);
			}
			this.sslSearch.ToolTipText = this.sslSearch.Text;
		}

		private void btnSame_Go_Click(object sender, EventArgs e)
		{
			WorkDataSame dta = WorkDataSame.Get(this);
			if (dta == null)
				return;
			this.StartSearch(dta);
		}

		private void lbxFound_SelectedIndexChanged(object sender, EventArgs e)
		{
			SearchResult result = this.lbxFound.SelectedItem as SearchResult;
			SearchResultDescribed resultD = result as SearchResultDescribed;
			if (result != null)
			{
				this.tbxFoundDetail.Text = (resultD == null) ? "" : resultD.Description;
				this.scFoundItems.Panel2Collapsed = (resultD == null);
				if (this._currentWorker == null)
					this.ssSarch.Visible = false;
				this._searchConnection.ShowResult(result, this);
			}
			else
			{
				this.scFoundItems.Panel2Collapsed = true;
				this.tbxFoundDetail.Text = "";
			}
		}

		private void frmSearch_Shown(object sender, EventArgs e)
		{
			if (this.tcPages.SelectedTab == this.tpStandard)
				this.tbxStd_Term.Focus();
			else if (this.tcPages.SelectedTab == this.tpX)
				this.btnSame_Go.Focus();
		}

		private void tbxStd_Term_Enter(object sender, EventArgs e)
		{
			this.AcceptButton = this.btnStd_Go;
		}

		private void tbxStd_Term_Leave(object sender, EventArgs e)
		{
			this.AcceptButton = null;
		}

		private void frmSearch_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Escape:
					if (e.Modifiers == Keys.None)
					{
						this._searchConnection.HideWin();
					}
					break;
			}
		}

		private void btnTidy_Go_Click(object sender, EventArgs e)
		{
			WorkDataTidy dta = WorkDataTidy.Get(this);
			if (dta == null)
				return;
			this.StartSearch(dta);
		}

		private void btnNumOccur_Go_Click(object sender, EventArgs e)
		{
			WorkDataNumOccur dta = WorkDataNumOccur.Get(this);
			if (dta == null)
				return;
			this.StartSearch(dta);
		}

		private void btnSpellCheck_Go_Click(object sender, EventArgs e)
		{
			WorkDataSpellCheck dta = WorkDataSpellCheck.Get(this);
			if (dta == null)
				return;
			this.StartSearch(dta);
		}

		private void sslStop_Click(object sender, EventArgs e)
		{
			this.StopSearch(Statuses.Expanded_Idle, false);
			this.sslSearch.Text = "Aborted.";
		}


	}
}