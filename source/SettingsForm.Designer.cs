
namespace ImaggaBatchUploader
{
	partial class SettingsForm
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
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnSave = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.tbKey = new System.Windows.Forms.TextBox();
			this.tbSecret = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.tbEndpoint = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.tbExtensions = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.lblOverrideNotification = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(271, 190);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 0;
			this.btnCancel.TabStop = false;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// btnSave
			// 
			this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnSave.Location = new System.Drawing.Point(190, 190);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(75, 23);
			this.btnSave.TabIndex = 1;
			this.btnSave.Text = "Save";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(49, 15);
			this.label1.TabIndex = 2;
			this.label1.Text = "API key:";
			// 
			// tbKey
			// 
			this.tbKey.Location = new System.Drawing.Point(121, 13);
			this.tbKey.Name = "tbKey";
			this.tbKey.Size = new System.Drawing.Size(225, 23);
			this.tbKey.TabIndex = 3;
			// 
			// tbSecret
			// 
			this.tbSecret.Location = new System.Drawing.Point(121, 42);
			this.tbSecret.Name = "tbSecret";
			this.tbSecret.Size = new System.Drawing.Size(225, 23);
			this.tbSecret.TabIndex = 4;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 45);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(63, 15);
			this.label2.TabIndex = 5;
			this.label2.Text = "API Secret:";
			// 
			// tbEndpoint
			// 
			this.tbEndpoint.Location = new System.Drawing.Point(121, 89);
			this.tbEndpoint.Name = "tbEndpoint";
			this.tbEndpoint.Size = new System.Drawing.Size(225, 23);
			this.tbEndpoint.TabIndex = 6;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(13, 92);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(79, 15);
			this.label3.TabIndex = 7;
			this.label3.Text = "API Endpoint:";
			// 
			// tbExtensions
			// 
			this.tbExtensions.Location = new System.Drawing.Point(121, 118);
			this.tbExtensions.Name = "tbExtensions";
			this.tbExtensions.Size = new System.Drawing.Size(225, 23);
			this.tbExtensions.TabIndex = 8;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(13, 121);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(102, 15);
			this.label4.TabIndex = 9;
			this.label4.Text = "Image extensions:";
			// 
			// lblOverrideNotification
			// 
			this.lblOverrideNotification.AutoSize = true;
			this.lblOverrideNotification.Location = new System.Drawing.Point(102, 159);
			this.lblOverrideNotification.Name = "lblOverrideNotification";
			this.lblOverrideNotification.Size = new System.Drawing.Size(241, 15);
			this.lblOverrideNotification.TabIndex = 10;
			this.lblOverrideNotification.Text = "Using overrided settings.  Editing is disabled.";
			// 
			// SettingsForm
			// 
			this.AcceptButton = this.btnSave;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(355, 218);
			this.ControlBox = false;
			this.Controls.Add(this.lblOverrideNotification);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.tbExtensions);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.tbEndpoint);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.tbSecret);
			this.Controls.Add(this.tbKey);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.btnCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SettingsForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Settings";
			this.Load += new System.EventHandler(this.SettingsForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		internal System.Windows.Forms.TextBox tbKey;
		internal System.Windows.Forms.TextBox tbSecret;
		internal System.Windows.Forms.TextBox tbEndpoint;
		internal System.Windows.Forms.TextBox tbExtensions;
		private System.Windows.Forms.Label lblOverrideNotification;
	}
}