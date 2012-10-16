using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace BePoE.UI
{
	public partial class frmOptions : Form
	{
		public delegate void NewOptionsApplier();
		private NewOptionsApplier _newOptionsApplier;
		private Dictionary<ColorEnviroPlace, Label> _colorsLabels;
		public frmOptions(NewOptionsApplier newOptionsApplier)
		{
			InitializeComponent();
			Utils.Gfx.SetFormIcon(this);
			this._newOptionsApplier = newOptionsApplier;
			this.chkCompileOnSave.Checked = Program.Vars.CompileOnSave;
			this.nudMaxSearchResults.Value = Convert.ToDecimal(Program.Vars.MaxSearchResults);
			this.btnApply.Enabled = (this._newOptionsApplier != null);
			this.tbxViewerPath.Text = Program.Vars.ViewerPath;
			this.tbxViewerParams.Text = Program.Vars.ViewerParameters;
			this.tbxMOCompilerPath.Text = Program.Vars.MOCompilerPath;
			this.tbxMOCompilerParams.Text = Program.Vars.MOCompilerParameters;
			this._colorsLabels = new Dictionary<ColorEnviroPlace, Label>();
			foreach (ColorEnviro enviro in Enum.GetValues(typeof(ColorEnviro)))
			{
				foreach (ColorPlace place in Enum.GetValues(typeof(ColorPlace)))
				{
					ColorEnviroPlace key = ColorEnviroPlace.Get(enviro, place);
					Label label = new Label();
					label.Cursor = Cursors.Hand;
					label.Dock = DockStyle.Fill;
					label.Margin = new Padding(1);
					label.Tag = key;
					this.tlpColors.Controls.Add(label, 1 + (int)place, 1 + (int)enviro);
					this._colorsLabels.Add(ColorEnviroPlace.Get(enviro, place), label);
					label.Click += this.ColorLabels_Click;

				}
			}
			this.SetColors(Program.Vars.Colors);
			FontSet(this.lnkFontSource, Program.Vars.SourceFont);
			FontSet(this.lnkFontTranslation, Program.Vars.TranslatedFont);
		}
		private static void FontSet(LinkLabel lnk, Font font)
		{
			lnk.Font = font;
			lnk.Text = string.Format("{0} {1}pt", font.Name, font.SizeInPoints);
		}

		private void btnApply_Click(object sender, EventArgs e)
		{
			if (!this.Save())
				return;
			this._newOptionsApplier();
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
			if (!this.Save())
				return;
			this.DialogResult = DialogResult.OK;
			this.Close();
		}
		private bool Save()
		{
			string viewerName = this.tbxViewerPath.Text.Trim();
			if (viewerName.Length == 0)
			{
				if (MessageBox.Show(string.Format("The viewer path is not specified.{0}{0}Proceed anyway?", Environment.NewLine), Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
				{
					FocalizeControl(this.tbxViewerPath);
					return false;
				}
			}
			else
			{
				string sErr;
				try
				{
					sErr = File.Exists(viewerName) ? null : string.Format("The file{0}{1}{0}does not exist.", Environment.NewLine, viewerName);
				}
				catch (Exception x)
				{
					sErr = x.Message;
				}
				if (sErr != null)
				{
					MessageBox.Show(sErr, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					FocalizeControl(this.tbxViewerPath);
					return false;
				}
			}
			string viewerParams = this.tbxViewerParams.Text.Trim();
			if (viewerParams.Length == 0)
				viewerParams = "%file%";
			if (!viewerParams.Contains("%file%"))
			{
				MessageBox.Show("The viewer parameters don't contain the %file% placeholder.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				FocalizeControl(this.tbxViewerParams);
				return false;
			}

			string moCompilerName = this.tbxMOCompilerPath.Text.Trim();
			if (moCompilerName.Length == 0)
			{
				if (MessageBox.Show(string.Format("The mo compiler path is not specified.{0}{0}Proceed anyway?", Environment.NewLine), Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
				{
					FocalizeControl(this.tbxMOCompilerPath);
					return false;
				}
			}
			else
			{
				string sErr;
				try
				{
					sErr = File.Exists(moCompilerName) ? null : string.Format("The file{0}{1}{0}does not exist.", Environment.NewLine, moCompilerName);
				}
				catch (Exception x)
				{
					sErr = x.Message;
				}
				if (sErr != null)
				{
					MessageBox.Show(sErr, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					FocalizeControl(this.tbxMOCompilerPath);
					return false;
				}
			}
			string moCompilerParams = this.tbxMOCompilerParams.Text.Trim();
			if (moCompilerParams.Length == 0)
				moCompilerParams = "\"%source%\" --output-file=\"%destination%\"";
			if (!moCompilerParams.Contains("%source%"))
			{
				MessageBox.Show("The mo compiler parameters don't contain the %source% placeholder.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				FocalizeControl(this.tbxMOCompilerParams);
				return false;
			}
			if (!moCompilerParams.Contains("%destination%"))
			{
				MessageBox.Show("The mo compiler parameters don't contain the %destination% placeholder.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				FocalizeControl(this.tbxMOCompilerParams);
				return false;
			}

			Program.Vars.CompileOnSave = this.chkCompileOnSave.Checked;
			Program.Vars.MaxSearchResults = Convert.ToInt32(Math.Round(this.nudMaxSearchResults.Value));
			Program.Vars.ViewerPath = viewerName;
			Program.Vars.ViewerParameters = viewerParams;
			Program.Vars.MOCompilerPath = moCompilerName;
			Program.Vars.MOCompilerParameters = moCompilerParams;
			foreach (KeyValuePair<ColorEnviroPlace, Label> k in this._colorsLabels)
			{
				Program.Vars.Colors[k.Key] = k.Value.BackColor;
			}
			Program.Vars.SourceFont = this.lnkFontSource.Font;
			Program.Vars.TranslatedFont = this.lnkFontTranslation.Font;
			Program.Vars.Save();
			return true;
		}

		private void btnKo_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void lnkDefaultColors_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			this.SetColors(Vars.DefaultColors);
		}
		private void SetColors(Dictionary<ColorEnviroPlace, Color> colors)
		{
			foreach (KeyValuePair<ColorEnviroPlace, Label> k in this._colorsLabels)
				k.Value.BackColor = colors[k.Key];
		}

		private void ColorLabels_Click(object sender, EventArgs e)
		{
			Label label = (Label)sender;
			this.cdPick.Color = label.BackColor;
			if (this.cdPick.ShowDialog(this) == DialogResult.OK)
			{
				label.BackColor = this.cdPick.Color;
			}
		}

		private void lnkFontSource_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			this.fdPick.Font = this.lnkFontSource.Font;
			if (this.fdPick.ShowDialog(this) == DialogResult.OK)
			{
				FontSet(this.lnkFontSource, this.fdPick.Font);
			}
		}

		private void lnkFontTranslation_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			this.fdPick.Font = this.lnkFontTranslation.Font;
			if (this.fdPick.ShowDialog(this) == DialogResult.OK)
			{
				FontSet(this.lnkFontTranslation, this.fdPick.Font);
			}
		}

		private static void FocalizeControl(Control ctrl)
		{
			Control prn = ctrl;
			while (prn != null)
			{
				if (prn is TabPage)
				{
					((TabControl)prn.Parent).SelectedTab = (TabPage)prn;
				}
				else if (prn is Form)
					break;
				prn = prn.Parent;
			}
			if (ctrl.Enabled)
				ctrl.Focus();
		}

		private static string InitFilePicker(Form form, OpenFileDialog ofd, string path, string defaultInitialFolder)
		{
			bool setUp = false;
			if (!string.IsNullOrEmpty(path))
			{
				try
				{
					FileInfo fi = new FileInfo(path.Trim());
					if (fi.Exists)
					{
						ofd.InitialDirectory = fi.Directory.FullName;
						ofd.FileName = fi.FullName;
						setUp = true;
					}
					if (!setUp)
					{
						DirectoryInfo di = new DirectoryInfo(path.Trim());
						while ((di != null) && (!setUp))
						{
							if (di.Exists)
							{
								ofd.FileName = "";
								ofd.InitialDirectory = di.FullName;
								setUp = true;
							}
							else
								di = di.Parent;
						}
					}
				}
				catch
				{ }
			}
			if (!setUp)
			{
				ofd.FileName = "";
				ofd.InitialDirectory = defaultInitialFolder;
			}
			try
			{
				if (ofd.ShowDialog(form) == DialogResult.OK)
					return ofd.FileName;
				else
					return null;
			}
			catch (Exception x)
			{
				MessageBox.Show(x.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return null;
			}
		}


		private void btnViewerPath_Click(object sender, EventArgs e)
		{
			string fn = InitFilePicker(this, this.ofdViewer, this.tbxViewerPath.Text, Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
			if (fn != null)
				this.tbxViewerPath.Text = fn;
		}
		private void btnMOCompilerPath_Click(object sender, EventArgs e)
		{
			string fn = InitFilePicker(this, this.ofdViewer, this.tbxMOCompilerPath.Text, new FileInfo(Application.ExecutablePath).Directory.FullName);
			if (fn != null)
				this.tbxMOCompilerPath.Text = fn;
		}
	}
}