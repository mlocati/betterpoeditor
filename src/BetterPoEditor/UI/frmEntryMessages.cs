using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BePoE.UI
{
	public partial class frmEntryMessages : Form
	{
		public class MessageDetail
		{
			public readonly string SinglelineText;
			public readonly string MultilineText;
			public readonly int? SelectionStart;
			public readonly int? SelectionLength;
			internal MessageDetail(Utils.Tidy.Result tidy)
				: this(tidy.ToString(false), tidy.ToString(true), tidy.SelectionStart, tidy.SelectionLength)
			{ }
			internal MessageDetail(Utils.SpellChecker.Result spellError)
				: this(spellError.ToString(false), spellError.ToString(true), spellError.SelectionStart, spellError.SelectionLength)
			{
			}
			private MessageDetail(string singlelineText, string multilineText, int? selectionStart, int? selectionLength)
			{
				this.SinglelineText = (singlelineText == null) ? "" : singlelineText;
				this.MultilineText = (multilineText == null) ? "" : multilineText;
				if (selectionStart.HasValue && selectionLength.HasValue)
				{
					this.SelectionStart = selectionStart.Value;
					this.SelectionLength = selectionLength.Value;
				}
				else
				{
					this.SelectionStart = null;
					this.SelectionLength = null;
				}
			}
			public override string ToString()
			{
				return this.SinglelineText;
			}
		}
		public interface IEntryMessageResultsConnection
		{
			void HideWin();
			void WinUnloaded();
		}
		private IEntryMessageResultsConnection _connector;
		private TextBox _textBox;
		public frmEntryMessages(IEntryMessageResultsConnection connector)
		{
			InitializeComponent();
			Utils.Gfx.SetFormIcon(this);
			this._connector = connector;
			this._textBox = null;
		}
		internal frmEntryMessages(IEntryMessageResultsConnection connector, TextBox textBox, MessageDetail[] messages, bool showDetailsPanel)
			: this(connector)
		{
			this.Setup(textBox, messages, showDetailsPanel);
		}
		internal void Setup(TextBox textBox, MessageDetail[] messages, bool showDetailsPanel)
		{
			this.lbxItems.Items.Clear();
			this.scPanels.Panel2Collapsed = !showDetailsPanel;
			if ((textBox == null) || (messages == null) || (messages.Length == 0))
			{
				this._textBox = null;
			}
			else
			{
				this._textBox = textBox;
				this.lbxItems.Items.AddRange(messages);
			}
			this.UpdateSelected();
		}

		private void lbxItems_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.UpdateSelected();
		}
		private MessageDetail Current
		{
			get
			{ return this.lbxItems.SelectedItem as MessageDetail; }
		}
		private void UpdateSelected()
		{
			if (this._textBox != null)
			{
				this._textBox.SelectionStart = 0;
				this._textBox.SelectionLength = 0;
			}

			if (this.Current == null)
			{
				this.tbxItem.Text = "";
			}
			else
			{
				this.tbxItem.Text = this.Current.MultilineText;
				if (this.Current.SelectionStart.HasValue)
				{
					this._textBox.SelectionStart = this.Current.SelectionStart.Value;
					this._textBox.SelectionLength = this.Current.SelectionLength.Value;
					this._textBox.ScrollToCaret();
				}
			}
		}

		private void frmTidyMessages_FormClosing(object sender, FormClosingEventArgs e)
		{
			switch (e.CloseReason)
			{
				case CloseReason.FormOwnerClosing:
					this._connector.WinUnloaded();
					break;
				default:
					e.Cancel = true;
					this._connector.HideWin();
					break;
			}
		}
	}
}