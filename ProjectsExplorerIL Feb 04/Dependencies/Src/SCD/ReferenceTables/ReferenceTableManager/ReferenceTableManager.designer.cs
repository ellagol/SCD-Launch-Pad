namespace ReferenceTableManager
{
	partial class ReferenceTableManager
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
			this.textBoxUser = new System.Windows.Forms.TextBox();
			this.labelUser = new System.Windows.Forms.Label();
			this.richTextBoxConnectionString = new System.Windows.Forms.RichTextBox();
			this.labelConnectionString = new System.Windows.Forms.Label();
			this.buttonSelectTable = new System.Windows.Forms.Button();
			this.comboBoxSelectOption = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// textBoxUser
			// 
			this.textBoxUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.textBoxUser.Location = new System.Drawing.Point(238, 173);
			this.textBoxUser.Name = "textBoxUser";
			this.textBoxUser.Size = new System.Drawing.Size(238, 26);
			this.textBoxUser.TabIndex = 12;
			this.textBoxUser.Text = "miriamr";
			// 
			// labelUser
			// 
			this.labelUser.AutoSize = true;
			this.labelUser.Location = new System.Drawing.Point(138, 181);
			this.labelUser.Name = "labelUser";
			this.labelUser.Size = new System.Drawing.Size(32, 13);
			this.labelUser.TabIndex = 11;
			this.labelUser.Text = "User:";
			// 
			// richTextBoxConnectionString
			// 
			this.richTextBoxConnectionString.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.richTextBoxConnectionString.Location = new System.Drawing.Point(238, 68);
			this.richTextBoxConnectionString.Name = "richTextBoxConnectionString";
			this.richTextBoxConnectionString.Size = new System.Drawing.Size(461, 70);
			this.richTextBoxConnectionString.TabIndex = 10;
			this.richTextBoxConnectionString.Text = "Data Source=Hermes;Initial Catalog=GenPR_Test;Persist Security Info=True;User ID=" +
    "GenPR_Test_User;Password=GenPR_Test_User";
			// 
			// labelConnectionString
			// 
			this.labelConnectionString.AutoSize = true;
			this.labelConnectionString.Location = new System.Drawing.Point(138, 76);
			this.labelConnectionString.Name = "labelConnectionString";
			this.labelConnectionString.Size = new System.Drawing.Size(94, 13);
			this.labelConnectionString.TabIndex = 9;
			this.labelConnectionString.Text = "Connection String:";
			// 
			// buttonSelectTable
			// 
			this.buttonSelectTable.AutoSize = true;
			this.buttonSelectTable.BackColor = System.Drawing.Color.SlateGray;
			this.buttonSelectTable.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.buttonSelectTable.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.buttonSelectTable.Location = new System.Drawing.Point(684, 299);
			this.buttonSelectTable.Name = "buttonSelectTable";
			this.buttonSelectTable.Size = new System.Drawing.Size(110, 30);
			this.buttonSelectTable.TabIndex = 13;
			this.buttonSelectTable.Text = "Go";
			this.buttonSelectTable.UseVisualStyleBackColor = false;
			this.buttonSelectTable.Click += new System.EventHandler(this.buttonSelectTable_Click);
			// 
			// comboBoxSelectOption
			// 
			this.comboBoxSelectOption.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.comboBoxSelectOption.FormattingEnabled = true;
			this.comboBoxSelectOption.Items.AddRange(new object[] {
            "Edit Reference Table data",
            "Edit Reference Table metadata",
            "Create Reference Table metadata",
            "Update Reference Table name in metadata for existing table"});
			this.comboBoxSelectOption.Location = new System.Drawing.Point(42, 299);
			this.comboBoxSelectOption.Name = "comboBoxSelectOption";
			this.comboBoxSelectOption.Size = new System.Drawing.Size(584, 28);
			this.comboBoxSelectOption.TabIndex = 14;
			// 
			// ReferenceTableManager
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(200)))), ((int)(((byte)(210)))));
			this.ClientSize = new System.Drawing.Size(836, 381);
			this.Controls.Add(this.comboBoxSelectOption);
			this.Controls.Add(this.buttonSelectTable);
			this.Controls.Add(this.textBoxUser);
			this.Controls.Add(this.labelUser);
			this.Controls.Add(this.richTextBoxConnectionString);
			this.Controls.Add(this.labelConnectionString);
			this.Name = "ReferenceTableManager";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ReferenceTableManager";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBoxUser;
		private System.Windows.Forms.Label labelUser;
		private System.Windows.Forms.RichTextBox richTextBoxConnectionString;
		private System.Windows.Forms.Label labelConnectionString;
		private System.Windows.Forms.Button buttonSelectTable;
		private System.Windows.Forms.ComboBox comboBoxSelectOption;
	}
}

