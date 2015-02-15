namespace ReferenceTableWriter
{
	partial class ManageReferenceTables
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.groupBoxTable = new System.Windows.Forms.GroupBox();
			this.listViewTable = new System.Windows.Forms.ListView();
			this.contextMenuStripAdd = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.buttonExit = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.groupBoxTable.SuspendLayout();
			this.contextMenuStripAdd.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.groupBoxTable);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.buttonExit);
			this.splitContainer1.Size = new System.Drawing.Size(1779, 582);
			this.splitContainer1.SplitterDistance = 528;
			this.splitContainer1.TabIndex = 0;
			// 
			// groupBoxTable
			// 
			this.groupBoxTable.BackColor = System.Drawing.Color.Beige;
			this.groupBoxTable.Controls.Add(this.listViewTable);
			this.groupBoxTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBoxTable.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.groupBoxTable.ForeColor = System.Drawing.SystemColors.ControlText;
			this.groupBoxTable.Location = new System.Drawing.Point(0, 0);
			this.groupBoxTable.Name = "groupBoxTable";
			this.groupBoxTable.Size = new System.Drawing.Size(1779, 528);
			this.groupBoxTable.TabIndex = 2;
			this.groupBoxTable.TabStop = false;
			this.groupBoxTable.Text = "Table";
			// 
			// listViewTable
			// 
			this.listViewTable.Alignment = System.Windows.Forms.ListViewAlignment.Left;
			this.listViewTable.AllowDrop = true;
			this.listViewTable.BackColor = System.Drawing.Color.MintCream;
			this.listViewTable.ContextMenuStrip = this.contextMenuStripAdd;
			this.listViewTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewTable.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.listViewTable.FullRowSelect = true;
			this.listViewTable.Location = new System.Drawing.Point(3, 22);
			this.listViewTable.Name = "listViewTable";
			this.listViewTable.OwnerDraw = true;
			this.listViewTable.ShowItemToolTips = true;
			this.listViewTable.Size = new System.Drawing.Size(1773, 503);
			this.listViewTable.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.listViewTable.TabIndex = 2;
			this.listViewTable.UseCompatibleStateImageBehavior = false;
			this.listViewTable.View = System.Windows.Forms.View.Details;
			this.listViewTable.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewTable_ColumnClick);
			this.listViewTable.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.listViewTable_DrawColumnHeader);
			this.listViewTable.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.listViewTable_DrawItem);
			this.listViewTable.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.listViewTable_DrawSubItem);
			this.listViewTable.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewTable_MouseDoubleClick);
			// 
			// contextMenuStripAdd
			// 
			this.contextMenuStripAdd.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addNewToolStripMenuItem});
			this.contextMenuStripAdd.Name = "contextMenuStripAdd";
			this.contextMenuStripAdd.Size = new System.Drawing.Size(117, 26);
			// 
			// addNewToolStripMenuItem
			// 
			this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
			this.addNewToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
			this.addNewToolStripMenuItem.Text = "&Add new";
			this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
			// 
			// buttonExit
			// 
			this.buttonExit.AutoSize = true;
			this.buttonExit.BackColor = System.Drawing.SystemColors.Info;
			this.buttonExit.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonExit.Location = new System.Drawing.Point(408, 10);
			this.buttonExit.Name = "buttonExit";
			this.buttonExit.Size = new System.Drawing.Size(75, 30);
			this.buttonExit.TabIndex = 4;
			this.buttonExit.Text = "&Exit";
			this.buttonExit.UseVisualStyleBackColor = false;
			// 
			// ManageReferenceTables
			// 
			this.AcceptButton = this.buttonExit;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.LightSteelBlue;
			this.ClientSize = new System.Drawing.Size(1779, 582);
			this.Controls.Add(this.splitContainer1);
			this.Name = "ManageReferenceTables";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Manage Reference Table";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.groupBoxTable.ResumeLayout(false);
			this.contextMenuStripAdd.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.Button buttonExit;
		private System.Windows.Forms.GroupBox groupBoxTable;
		private System.Windows.Forms.ListView listViewTable;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripAdd;
		private System.Windows.Forms.ToolStripMenuItem addNewToolStripMenuItem;

	}
}