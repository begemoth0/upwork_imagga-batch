
namespace ImaggaBatchUploader
{
	partial class frmMain
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
			this.fbDlg = new System.Windows.Forms.FolderBrowserDialog();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.tsLblStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
			this.gbFolderInfo = new System.Windows.Forms.GroupBox();
			this.btnStartStop = new System.Windows.Forms.Button();
			this.llErrorsCount = new System.Windows.Forms.LinkLabel();
			this.llProcessedCount = new System.Windows.Forms.LinkLabel();
			this.lblErrors = new System.Windows.Forms.Label();
			this.lblProcessed = new System.Windows.Forms.Label();
			this.lblUnrecognizedFilesCount = new System.Windows.Forms.Label();
			this.lblTotalImagesCount = new System.Windows.Forms.Label();
			this.lblUnrecognizedLabel = new System.Windows.Forms.Label();
			this.lblTotalImagesLabel = new System.Windows.Forms.Label();
			this.tbSelectedFolder = new System.Windows.Forms.TextBox();
			this.btnSelectFolder = new System.Windows.Forms.Button();
			this.btnSettings = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.tbLog = new System.Windows.Forms.TextBox();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.statusStrip1.SuspendLayout();
			this.gbFolderInfo.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// fbDlg
			// 
			this.fbDlg.ShowNewFolderButton = false;
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsLblStatus,
            this.toolStripProgressBar1});
			this.statusStrip1.Location = new System.Drawing.Point(0, 363);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(500, 22);
			this.statusStrip1.TabIndex = 0;
			this.statusStrip1.Text = "Helo World";
			// 
			// tsLblStatus
			// 
			this.tsLblStatus.Name = "tsLblStatus";
			this.tsLblStatus.Size = new System.Drawing.Size(118, 17);
			this.tsLblStatus.Text = "Select folder to start..";
			// 
			// toolStripProgressBar1
			// 
			this.toolStripProgressBar1.Name = "toolStripProgressBar1";
			this.toolStripProgressBar1.Size = new System.Drawing.Size(350, 16);
			// 
			// gbFolderInfo
			// 
			this.gbFolderInfo.Controls.Add(this.btnStartStop);
			this.gbFolderInfo.Controls.Add(this.llErrorsCount);
			this.gbFolderInfo.Controls.Add(this.llProcessedCount);
			this.gbFolderInfo.Controls.Add(this.lblErrors);
			this.gbFolderInfo.Controls.Add(this.lblProcessed);
			this.gbFolderInfo.Controls.Add(this.lblUnrecognizedFilesCount);
			this.gbFolderInfo.Controls.Add(this.lblTotalImagesCount);
			this.gbFolderInfo.Controls.Add(this.lblUnrecognizedLabel);
			this.gbFolderInfo.Controls.Add(this.lblTotalImagesLabel);
			this.gbFolderInfo.Location = new System.Drawing.Point(12, 41);
			this.gbFolderInfo.Name = "gbFolderInfo";
			this.gbFolderInfo.Size = new System.Drawing.Size(475, 74);
			this.gbFolderInfo.TabIndex = 2;
			this.gbFolderInfo.TabStop = false;
			this.gbFolderInfo.Text = "Folder info";
			// 
			// btnStartStop
			// 
			this.btnStartStop.Enabled = false;
			this.btnStartStop.Location = new System.Drawing.Point(341, 15);
			this.btnStartStop.Name = "btnStartStop";
			this.btnStartStop.Size = new System.Drawing.Size(128, 50);
			this.btnStartStop.TabIndex = 8;
			this.btnStartStop.Text = "Start tagging";
			this.toolTip1.SetToolTip(this.btnStartStop, "Select folder to start tagging.");
			this.btnStartStop.UseVisualStyleBackColor = true;
			// 
			// llErrorsCount
			// 
			this.llErrorsCount.AutoSize = true;
			this.llErrorsCount.Enabled = false;
			this.llErrorsCount.Location = new System.Drawing.Point(281, 50);
			this.llErrorsCount.Name = "llErrorsCount";
			this.llErrorsCount.Size = new System.Drawing.Size(0, 15);
			this.llErrorsCount.TabIndex = 7;
			// 
			// llProcessedCount
			// 
			this.llProcessedCount.AutoSize = true;
			this.llProcessedCount.Enabled = false;
			this.llProcessedCount.Location = new System.Drawing.Point(281, 23);
			this.llProcessedCount.Name = "llProcessedCount";
			this.llProcessedCount.Size = new System.Drawing.Size(0, 15);
			this.llProcessedCount.TabIndex = 6;
			// 
			// lblErrors
			// 
			this.lblErrors.AutoSize = true;
			this.lblErrors.Location = new System.Drawing.Point(167, 50);
			this.lblErrors.Name = "lblErrors";
			this.lblErrors.Size = new System.Drawing.Size(107, 15);
			this.lblErrors.TabIndex = 5;
			this.lblErrors.Text = "Images with errors:";
			// 
			// lblProcessed
			// 
			this.lblProcessed.AutoSize = true;
			this.lblProcessed.Location = new System.Drawing.Point(170, 23);
			this.lblProcessed.Name = "lblProcessed";
			this.lblProcessed.Size = new System.Drawing.Size(104, 15);
			this.lblProcessed.TabIndex = 4;
			this.lblProcessed.Text = "Processed images:";
			// 
			// lblUnrecognizedFilesCount
			// 
			this.lblUnrecognizedFilesCount.AutoSize = true;
			this.lblUnrecognizedFilesCount.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
			this.lblUnrecognizedFilesCount.Location = new System.Drawing.Point(121, 50);
			this.lblUnrecognizedFilesCount.Name = "lblUnrecognizedFilesCount";
			this.lblUnrecognizedFilesCount.Size = new System.Drawing.Size(0, 15);
			this.lblUnrecognizedFilesCount.TabIndex = 3;
			// 
			// lblTotalImagesCount
			// 
			this.lblTotalImagesCount.AutoSize = true;
			this.lblTotalImagesCount.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
			this.lblTotalImagesCount.Location = new System.Drawing.Point(121, 23);
			this.lblTotalImagesCount.Name = "lblTotalImagesCount";
			this.lblTotalImagesCount.Size = new System.Drawing.Size(0, 15);
			this.lblTotalImagesCount.TabIndex = 2;
			// 
			// lblUnrecognizedLabel
			// 
			this.lblUnrecognizedLabel.AutoSize = true;
			this.lblUnrecognizedLabel.Location = new System.Drawing.Point(7, 50);
			this.lblUnrecognizedLabel.Name = "lblUnrecognizedLabel";
			this.lblUnrecognizedLabel.Size = new System.Drawing.Size(107, 15);
			this.lblUnrecognizedLabel.TabIndex = 1;
			this.lblUnrecognizedLabel.Text = "Unrecognized files:";
			// 
			// lblTotalImagesLabel
			// 
			this.lblTotalImagesLabel.AutoSize = true;
			this.lblTotalImagesLabel.Location = new System.Drawing.Point(7, 23);
			this.lblTotalImagesLabel.Name = "lblTotalImagesLabel";
			this.lblTotalImagesLabel.Size = new System.Drawing.Size(76, 15);
			this.lblTotalImagesLabel.TabIndex = 0;
			this.lblTotalImagesLabel.Text = "Total images:";
			// 
			// tbSelectedFolder
			// 
			this.tbSelectedFolder.Location = new System.Drawing.Point(12, 12);
			this.tbSelectedFolder.Name = "tbSelectedFolder";
			this.tbSelectedFolder.ReadOnly = true;
			this.tbSelectedFolder.Size = new System.Drawing.Size(361, 23);
			this.tbSelectedFolder.TabIndex = 4;
			this.tbSelectedFolder.Text = "<select folder>";
			// 
			// btnSelectFolder
			// 
			this.btnSelectFolder.Location = new System.Drawing.Point(379, 12);
			this.btnSelectFolder.Name = "btnSelectFolder";
			this.btnSelectFolder.Size = new System.Drawing.Size(27, 23);
			this.btnSelectFolder.TabIndex = 5;
			this.btnSelectFolder.Text = "...";
			this.btnSelectFolder.UseVisualStyleBackColor = true;
			this.btnSelectFolder.Click += new System.EventHandler(this.btnSelectFolder_Click);
			// 
			// btnSettings
			// 
			this.btnSettings.Location = new System.Drawing.Point(432, 11);
			this.btnSettings.Name = "btnSettings";
			this.btnSettings.Size = new System.Drawing.Size(55, 23);
			this.btnSettings.TabIndex = 6;
			this.btnSettings.Text = "settings";
			this.btnSettings.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.tbLog);
			this.groupBox1.Location = new System.Drawing.Point(12, 122);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(475, 238);
			this.groupBox1.TabIndex = 7;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Intermediate output";
			// 
			// tbLog
			// 
			this.tbLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbLog.BackColor = System.Drawing.SystemColors.Window;
			this.tbLog.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.tbLog.Location = new System.Drawing.Point(7, 23);
			this.tbLog.Multiline = true;
			this.tbLog.Name = "tbLog";
			this.tbLog.ReadOnly = true;
			this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.tbLog.Size = new System.Drawing.Size(462, 209);
			this.tbLog.TabIndex = 0;
			// 
			// frmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(500, 385);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.btnSettings);
			this.Controls.Add(this.btnSelectFolder);
			this.Controls.Add(this.tbSelectedFolder);
			this.Controls.Add(this.gbFolderInfo);
			this.Controls.Add(this.statusStrip1);
			this.MaximizeBox = false;
			this.Name = "frmMain";
			this.Text = "Imagga.com batch tagger";
			this.Load += new System.EventHandler(this.frmMain_Load);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.gbFolderInfo.ResumeLayout(false);
			this.gbFolderInfo.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.FolderBrowserDialog fbDlg;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel tsLblStatus;
		private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
		private System.Windows.Forms.GroupBox gbFolderInfo;
		private System.Windows.Forms.TextBox tbSelectedFolder;
		private System.Windows.Forms.Button btnSelectFolder;
		private System.Windows.Forms.Button btnSettings;
		private System.Windows.Forms.Label lblErrors;
		private System.Windows.Forms.Label lblProcessed;
		private System.Windows.Forms.Label lblUnrecognizedFilesCount;
		private System.Windows.Forms.Label lblTotalImagesCount;
		private System.Windows.Forms.Label lblUnrecognizedLabel;
		private System.Windows.Forms.Label lblTotalImagesLabel;
		private System.Windows.Forms.LinkLabel llProcessedCount;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox tbLog;
		private System.Windows.Forms.LinkLabel llErrorsCount;
		private System.Windows.Forms.Button btnStartStop;
		private System.Windows.Forms.ToolTip toolTip1;
	}
}