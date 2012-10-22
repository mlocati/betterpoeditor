using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace BePoE.UI
{
	public partial class frmInfo : Form
	{
		public frmInfo(ICollection<frmMain.AcceletatorKeyInfoAttribute> keys)
		{
			InitializeComponent();
			Utils.Gfx.SetFormIcon(this);
			this.Text = string.Format("About {0}", Application.ProductName);
			this.lblProduct.Text = Application.ProductName;
			this.lblVersion.Text = Program.VERSION;
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Key", typeof(string)));
			dt.Columns.Add(new DataColumn("Meaning", typeof(string)));
			foreach (frmMain.AcceletatorKeyInfoAttribute a in keys)
			{
				dt.Rows.Add(new string[] { (a.Modifiers == Keys.None) ? a.Key.ToString() : string.Format("{0} + {1}", a.Modifiers, a.Key), a.Description });
			}
			this.dgvKeys.DataSource = dt;
			this.pgInfo.SelectedObject = new AboutInfo();
		}
		private class AboutInfo
		{
			[DisplayName("CLR version")]
			public string CLRVersion
			{
				get
				{
					return Environment.Version.ToString(4);
				}
			}
			[DisplayName("Machine name")]
			public string MachineName
			{
				get
				{
					return Environment.MachineName;
				}
			}
			[DisplayName("Platform")]
			public string OSVersion_Platform
			{
				get
				{
					return Environment.OSVersion.Platform.ToString();
				}
			}
			[DisplayName("OS Version")]
			public string OSVersion_VersionString
			{
				get
				{
					return Environment.OSVersion.VersionString;
				}
			}
			[DisplayName("Service pack")]
			public string OSVersion_ServicePack
			{
				get
				{
					return Environment.OSVersion.ServicePack;
				}
			}
			[DisplayName("Number of processors")]
			public string ProcessorCount
			{
				get
				{
					return Environment.ProcessorCount.ToString("N0");
				}
			}
			[DisplayName("OS is case sensitive?")]
			public string OSCase
			{
				get
				{ return Vars.OsIsCaseSensitive ? "yes" : "no"; }
			}
			[DisplayName("User's domain")]
			public string UserDomainName
			{
				get
				{
					return Environment.UserDomainName;
				}
			}
			[DisplayName("User name")]
			public string UserName
			{
				get
				{
					return Environment.UserName;
				}
			}
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void lnkWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				ProcessStartInfo psi = new ProcessStartInfo();
				psi.ErrorDialog = false;
				psi.FileName = this.lnkWebsite.Text.Contains("://") ? this.lnkWebsite.Text : string.Format("http://{0}", this.lnkWebsite.Text);
				psi.UseShellExecute = true;
				psi.Verb = "open";
				psi.WindowStyle = ProcessWindowStyle.Normal;
				using (Process process = new Process())
				{
					process.StartInfo = psi;
					process.Start();
				}
			}
			catch (Exception x)
			{
				MessageBox.Show(x.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}
	}
}