using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BePoE.UI
{
	public partial class frmSelectLanguage : Form
	{
		private CultureInfo _result;
		public CultureInfo Result
		{
			get
			{ return this._result; }
		}
		public frmSelectLanguage(CultureInfo suggestedLanguage)
		{
			InitializeComponent();
			Utils.Gfx.SetFormIcon(this);
			this._result = null;
			Dictionary<CultureInfo, List<CultureInfo>> list = new Dictionary<CultureInfo, List<CultureInfo>>();
			foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.AllCultures))
			{
				if ((ci.Parent != null) && (ci.Parent != CultureInfo.InvariantCulture))
				{
					if (!list.ContainsKey(ci.Parent))
						list.Add(ci.Parent, new List<CultureInfo>());
					if (!list[ci.Parent].Contains(ci))
						list[ci.Parent].Add(ci);
				}
				if (!list.ContainsKey(ci))
					list.Add(ci, new List<CultureInfo>());
			}
			this.AddNodes(list, null, null);
			if (suggestedLanguage == null)
				suggestedLanguage = CultureInfo.CurrentUICulture;
			TreeNode node = FindNode(this.tvLangs.Nodes, suggestedLanguage.Name);
			if (node != null)
			{
				this.tvLangs.SelectedNode = node;
				node.EnsureVisible();
			}
		}
		private static TreeNode FindNode(TreeNodeCollection nodes, string cultureCode)
		{
			foreach (TreeNode n in nodes)
			{
				if (((CultureInfo)n.Tag).Name == cultureCode)
					return n;
				TreeNode f = FindNode(n.Nodes, cultureCode);
				if (f != null)
					return f;
			}
			return null;
		}
		private static void InsertNode(TreeNodeCollection nodes, TreeNode newNode)
		{
			for (int i = 0; i < nodes.Count; i++)
				if (string.Compare(nodes[i].Text, newNode.Text, true) >= 0)
				{
					nodes.Insert(i, newNode);
					return;
				}
			nodes.Add(newNode);
		}
		private void AddNodes(Dictionary<CultureInfo, List<CultureInfo>> list, CultureInfo parentCulture, TreeNode parentNode)
		{
			if (parentCulture == null)
			{
				foreach (CultureInfo thisParentCulture in list.Keys)
				{
					if ((thisParentCulture.Parent != null) && (thisParentCulture.Parent != CultureInfo.InvariantCulture))
						continue;
					TreeNode thisParentNode = new TreeNode(thisParentCulture.DisplayName);
					thisParentNode.Tag = thisParentCulture;
					this.AddNodes(list, thisParentCulture, thisParentNode);
					InsertNode(this.tvLangs.Nodes, thisParentNode);
				}
			}
			else
			{
				if (list.ContainsKey(parentCulture))
				{
					foreach (CultureInfo childCulture in list[parentCulture])
					{
						TreeNode childNode = new TreeNode(childCulture.DisplayName);
						childNode.Tag = childCulture;
						InsertNode(parentNode.Nodes, childNode);
						this.AddNodes(list, childCulture, childNode);
					}
				}
			}
		}

		private void btnKO_Click(object sender, EventArgs e)
		{
			this._result = null;
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			if (this.tvLangs.SelectedNode == null)
			{
				this.tvLangs.Focus();
				return;
			}
			this._result = (CultureInfo)this.tvLangs.SelectedNode.Tag;
			this.DialogResult = DialogResult.OK;
			this.Close();
		}
	}
}