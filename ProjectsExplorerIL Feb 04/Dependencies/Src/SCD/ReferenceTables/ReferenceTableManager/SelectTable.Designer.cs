namespace ReferenceTableManager
{
	partial class SelectTable
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
			this.comboBoxSelect = new System.Windows.Forms.ComboBox();
			this.labelSelect = new System.Windows.Forms.Label();
			this.buttonBack = new System.Windows.Forms.Button();
			this.buttonNext = new System.Windows.Forms.Button();
			this.buttonOk = new System.Windows.Forms.Button();
			this.labelId = new System.Windows.Forms.Label();
			this.textBoxId = new System.Windows.Forms.TextBox();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// comboBoxSelect
			// 
			this.comboBoxSelect.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.comboBoxSelect.FormattingEnabled = true;
			this.comboBoxSelect.Location = new System.Drawing.Point(357, 156);
			this.comboBoxSelect.Name = "comboBoxSelect";
			this.comboBoxSelect.Size = new System.Drawing.Size(312, 28);
			this.comboBoxSelect.TabIndex = 1;
			// 
			// labelSelect
			// 
			this.labelSelect.AutoSize = true;
			this.labelSelect.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.labelSelect.Location = new System.Drawing.Point(90, 159);
			this.labelSelect.Name = "labelSelect";
			this.labelSelect.Size = new System.Drawing.Size(97, 20);
			this.labelSelect.TabIndex = 2;
			this.labelSelect.Text = "Select table:";
			// 
			// buttonBack
			// 
			this.buttonBack.AutoSize = true;
			this.buttonBack.BackColor = System.Drawing.Color.SlateGray;
			this.buttonBack.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.buttonBack.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.buttonBack.Location = new System.Drawing.Point(278, 391);
			this.buttonBack.Name = "buttonBack";
			this.buttonBack.Size = new System.Drawing.Size(90, 30);
			this.buttonBack.TabIndex = 5;
			this.buttonBack.Text = "<< &Back";
			this.buttonBack.UseVisualStyleBackColor = false;
			// 
			// buttonNext
			// 
			this.buttonNext.AutoSize = true;
			this.buttonNext.BackColor = System.Drawing.Color.SlateGray;
			this.buttonNext.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.buttonNext.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.buttonNext.Location = new System.Drawing.Point(424, 391);
			this.buttonNext.Name = "buttonNext";
			this.buttonNext.Size = new System.Drawing.Size(90, 30);
			this.buttonNext.TabIndex = 4;
			this.buttonNext.Text = "&Next >>";
			this.buttonNext.UseVisualStyleBackColor = false;
			this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
			// 
			// buttonOk
			// 
			this.buttonOk.AutoSize = true;
			this.buttonOk.BackColor = System.Drawing.Color.SlateGray;
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonOk.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.buttonOk.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.buttonOk.Location = new System.Drawing.Point(533, 391);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(90, 30);
			this.buttonOk.TabIndex = 6;
			this.buttonOk.Text = "&Ok";
			this.buttonOk.UseVisualStyleBackColor = false;
			this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
			// 
			// labelId
			// 
			this.labelId.AutoSize = true;
			this.labelId.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.labelId.Location = new System.Drawing.Point(90, 221);
			this.labelId.Name = "labelId";
			this.labelId.Size = new System.Drawing.Size(110, 20);
			this.labelId.TabIndex = 7;
			this.labelId.Text = "Input table ID:";
			// 
			// textBoxId
			// 
			this.textBoxId.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.textBoxId.Location = new System.Drawing.Point(357, 218);
			this.textBoxId.MaxLength = 5;
			this.textBoxId.Name = "textBoxId";
			this.textBoxId.Size = new System.Drawing.Size(238, 26);
			this.textBoxId.TabIndex = 13;
			// 
			// buttonCancel
			// 
			this.buttonCancel.AutoSize = true;
			this.buttonCancel.BackColor = System.Drawing.Color.SlateGray;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.buttonCancel.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.buttonCancel.Location = new System.Drawing.Point(640, 391);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(90, 30);
			this.buttonCancel.TabIndex = 14;
			this.buttonCancel.Text = "&Cancel";
			this.buttonCancel.UseVisualStyleBackColor = false;
			// 
			// SelectTable
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(200)))), ((int)(((byte)(210)))));
			this.CancelButton = this.buttonBack;
			this.ClientSize = new System.Drawing.Size(792, 473);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.textBoxId);
			this.Controls.Add(this.labelId);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.buttonBack);
			this.Controls.Add(this.buttonNext);
			this.Controls.Add(this.labelSelect);
			this.Controls.Add(this.comboBoxSelect);
			this.Name = "SelectTable";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Select Table";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox comboBoxSelect;
		private System.Windows.Forms.Label labelSelect;
		internal System.Windows.Forms.Button buttonBack;
		internal System.Windows.Forms.Button buttonNext;
		internal System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.Label labelId;
		private System.Windows.Forms.TextBox textBoxId;
		internal System.Windows.Forms.Button buttonCancel;
	}
}