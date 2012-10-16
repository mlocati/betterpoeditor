using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BePoE.UI
{
	public partial class frmSelectCodePage : Form
	{
		private int _selectedCodePage;
		public int SelectedCodePage
		{
			get
			{ return this._selectedCodePage; }
		}
		private class CodePageWrapper
		{
			public readonly int CodePage;
			public readonly string Name;
			public CodePageWrapper(KeyValuePair<int, string> availableCodePage)
			{
				this.CodePage = availableCodePage.Key;
				this.Name = availableCodePage.Value;
			}
			public override string ToString()
			{
				return this.Name;
			}
		}

		private byte[] _dataBytes;
		private int[] _ansiCharPositions;
		public frmSelectCodePage(Dictionary<int, string> availableCodePages, int defaultCodePage, byte[] dataBytes, int[] ansiCharPositions)
		{
			InitializeComponent();
			Utils.Gfx.SetFormIcon(this);
			this._selectedCodePage = 0;
			this._dataBytes = dataBytes;
			this._ansiCharPositions = ansiCharPositions;
			this.cbxCodePage.Items.Clear();
			CodePageWrapper selectedCP = null;
			foreach (KeyValuePair<int, string> availableCodePage in availableCodePages)
			{
				CodePageWrapper cpw = new CodePageWrapper(availableCodePage);
				if ((selectedCP == null) || (cpw.CodePage == defaultCodePage))
					selectedCP = cpw;
				this.cbxCodePage.Items.Add(cpw);
			}
			this.cbxCodePage.SelectedItem = selectedCP;
		}

		private void cbxCodePage_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.lbxText.Items.Clear();
			CodePageWrapper cpw = this.cbxCodePage.SelectedItem as CodePageWrapper;
			if (cpw != null)
			{
				foreach (int ansiCharPosition in this._ansiCharPositions)
				{
					int p0 = Math.Max(0, ansiCharPosition - 10);
					int p1 = Math.Min(this._dataBytes.Length - 1, ansiCharPosition + 10);
					StringBuilder sb = new StringBuilder();
					if (p0 > 0)
						sb.Append("...");
					sb.Append(Encoding.GetEncoding(cpw.CodePage).GetString(this._dataBytes, p0, p1 - p0 + 1));
					if(p1<(this._dataBytes.Length - 1))
						sb.Append("...");
					this.lbxText.Items.Add(sb.ToString());
				}
			}
			this.btnOk.Enabled = (cpw != null);
		}

		private void btnKo_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
			CodePageWrapper cpw = this.cbxCodePage.SelectedItem as CodePageWrapper;
			if (cpw == null)
				return;
			this._selectedCodePage = cpw.CodePage;
			this.DialogResult = DialogResult.OK;
			this.Close();
		}
	}
}