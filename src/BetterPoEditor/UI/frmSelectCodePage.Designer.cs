namespace BePoE.UI
{
	partial class frmSelectCodePage
	{
		/// <summary>
		/// Variabile di progettazione necessaria.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Liberare le risorse in uso.
		/// </summary>
		/// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Codice generato da Progettazione Windows Form

		/// <summary>
		/// Metodo necessario per il supporto della finestra di progettazione. Non modificare
		/// il contenuto del metodo con l'editor di codice.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnOk = new System.Windows.Forms.Button();
			this.btnKo = new System.Windows.Forms.Button();
			this.lblIntro = new System.Windows.Forms.Label();
			this.lblCbo = new System.Windows.Forms.Label();
			this.cbxCodePage = new System.Windows.Forms.ComboBox();
			this.lbxText = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// btnOk
			// 
			this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOk.Enabled = false;
			this.btnOk.Location = new System.Drawing.Point(124, 238);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 4;
			this.btnOk.Text = "OK";
			this.btnOk.UseVisualStyleBackColor = true;
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// btnKo
			// 
			this.btnKo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnKo.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnKo.Location = new System.Drawing.Point(205, 238);
			this.btnKo.Name = "btnKo";
			this.btnKo.Size = new System.Drawing.Size(75, 23);
			this.btnKo.TabIndex = 5;
			this.btnKo.Text = "Cancel";
			this.btnKo.UseVisualStyleBackColor = true;
			this.btnKo.Click += new System.EventHandler(this.btnKo_Click);
			// 
			// lblIntro
			// 
			this.lblIntro.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.lblIntro.Location = new System.Drawing.Point(12, 9);
			this.lblIntro.Name = "lblIntro";
			this.lblIntro.Size = new System.Drawing.Size(268, 81);
			this.lblIntro.TabIndex = 0;
			this.lblIntro.Text = "The selected file contains characters that may have different meanings for differ" +
				 "ent Countries.\r\nPlease select a value from the drop-down menu below which render" +
				 "s the text in the right way.\r\n";
			this.lblIntro.UseMnemonic = false;
			// 
			// lblCbo
			// 
			this.lblCbo.AutoSize = true;
			this.lblCbo.Location = new System.Drawing.Point(12, 90);
			this.lblCbo.Name = "lblCbo";
			this.lblCbo.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
			this.lblCbo.Size = new System.Drawing.Size(56, 17);
			this.lblCbo.TabIndex = 1;
			this.lblCbo.Text = "Codepage";
			// 
			// cbxCodePage
			// 
			this.cbxCodePage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.cbxCodePage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxCodePage.FormattingEnabled = true;
			this.cbxCodePage.Location = new System.Drawing.Point(74, 88);
			this.cbxCodePage.Name = "cbxCodePage";
			this.cbxCodePage.Size = new System.Drawing.Size(206, 21);
			this.cbxCodePage.Sorted = true;
			this.cbxCodePage.TabIndex = 2;
			this.cbxCodePage.SelectedIndexChanged += new System.EventHandler(this.cbxCodePage_SelectedIndexChanged);
			// 
			// lbxText
			// 
			this.lbxText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
							| System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.lbxText.FormattingEnabled = true;
			this.lbxText.IntegralHeight = false;
			this.lbxText.Location = new System.Drawing.Point(12, 115);
			this.lbxText.Name = "lbxText";
			this.lbxText.Size = new System.Drawing.Size(268, 117);
			this.lbxText.TabIndex = 3;
			// 
			// frmSelectCodePage
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnKo;
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Controls.Add(this.lbxText);
			this.Controls.Add(this.cbxCodePage);
			this.Controls.Add(this.lblCbo);
			this.Controls.Add(this.lblIntro);
			this.Controls.Add(this.btnKo);
			this.Controls.Add(this.btnOk);
			this.Name = "frmSelectCodePage";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Code page";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnKo;
		private System.Windows.Forms.Label lblIntro;
		private System.Windows.Forms.Label lblCbo;
		private System.Windows.Forms.ComboBox cbxCodePage;
		private System.Windows.Forms.ListBox lbxText;
	}
}