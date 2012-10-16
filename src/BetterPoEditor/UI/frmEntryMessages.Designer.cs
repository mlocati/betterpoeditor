namespace BePoE.UI
{
	partial class frmEntryMessages
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
			this.scPanels = new System.Windows.Forms.SplitContainer();
			this.lbxItems = new System.Windows.Forms.ListBox();
			this.tbxItem = new System.Windows.Forms.TextBox();
			this.scPanels.Panel1.SuspendLayout();
			this.scPanels.Panel2.SuspendLayout();
			this.scPanels.SuspendLayout();
			this.SuspendLayout();
			// 
			// scPanels
			// 
			this.scPanels.Dock = System.Windows.Forms.DockStyle.Fill;
			this.scPanels.Location = new System.Drawing.Point(0, 0);
			this.scPanels.Name = "scPanels";
			this.scPanels.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// scPanels.Panel1
			// 
			this.scPanels.Panel1.Controls.Add(this.lbxItems);
			// 
			// scPanels.Panel2
			// 
			this.scPanels.Panel2.Controls.Add(this.tbxItem);
			this.scPanels.Size = new System.Drawing.Size(292, 273);
			this.scPanels.SplitterDistance = 96;
			this.scPanels.TabIndex = 0;
			// 
			// lbxItems
			// 
			this.lbxItems.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lbxItems.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbxItems.FormattingEnabled = true;
			this.lbxItems.Location = new System.Drawing.Point(0, 0);
			this.lbxItems.Name = "lbxItems";
			this.lbxItems.Size = new System.Drawing.Size(292, 93);
			this.lbxItems.TabIndex = 0;
			this.lbxItems.SelectedIndexChanged += new System.EventHandler(this.lbxItems_SelectedIndexChanged);
			// 
			// tbxItem
			// 
			this.tbxItem.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbxItem.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbxItem.Location = new System.Drawing.Point(0, 0);
			this.tbxItem.Multiline = true;
			this.tbxItem.Name = "tbxItem";
			this.tbxItem.ReadOnly = true;
			this.tbxItem.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.tbxItem.Size = new System.Drawing.Size(292, 173);
			this.tbxItem.TabIndex = 0;
			// 
			// frmEntryMessages
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Controls.Add(this.scPanels);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmEntryMessages";
			this.Text = "Entry messages";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmTidyMessages_FormClosing);
			this.scPanels.Panel1.ResumeLayout(false);
			this.scPanels.Panel2.ResumeLayout(false);
			this.scPanels.Panel2.PerformLayout();
			this.scPanels.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer scPanels;
		private System.Windows.Forms.ListBox lbxItems;
		private System.Windows.Forms.TextBox tbxItem;
	}
}