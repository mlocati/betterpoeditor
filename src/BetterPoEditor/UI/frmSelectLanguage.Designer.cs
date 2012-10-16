namespace BePoE.UI
{
	partial class frmSelectLanguage
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
			this.tvLangs = new System.Windows.Forms.TreeView();
			this.btnKO = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// tvLangs
			// 
			this.tvLangs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
							| System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.tvLangs.HideSelection = false;
			this.tvLangs.Location = new System.Drawing.Point(12, 12);
			this.tvLangs.Name = "tvLangs";
			this.tvLangs.Size = new System.Drawing.Size(268, 218);
			this.tvLangs.TabIndex = 0;
			// 
			// btnKO
			// 
			this.btnKO.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnKO.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnKO.Location = new System.Drawing.Point(124, 238);
			this.btnKO.Name = "btnKO";
			this.btnKO.Size = new System.Drawing.Size(75, 23);
			this.btnKO.TabIndex = 1;
			this.btnKO.Text = "Cancel";
			this.btnKO.UseVisualStyleBackColor = true;
			this.btnKO.Click += new System.EventHandler(this.btnKO_Click);
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.Location = new System.Drawing.Point(205, 238);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 2;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// frmSelectLanguage
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnKO;
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnKO);
			this.Controls.Add(this.tvLangs);
			this.Name = "frmSelectLanguage";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Select language";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TreeView tvLangs;
		private System.Windows.Forms.Button btnKO;
		private System.Windows.Forms.Button btnOK;
	}
}