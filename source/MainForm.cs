using NLog;
using NLog.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ImaggaBatchUploader
{
	public partial class frmMain : Form
	{
		private static Logger logger;
		private MainFormController logic;
		public frmMain()
		{
			InitializeComponent();
			var configuration = LogManager.Configuration;
			var tbTarget = new FormControlTarget()
			{
				FormName = this.Name,
				ControlName = this.tbLog.Name,
				Layout = "[${time}]${level:uppercase=true:padding=6}| ${message}\r\n"
			};
			configuration.AddRule(LogLevel.Debug, LogLevel.Fatal, tbTarget);
			LogManager.Configuration = configuration;
			logger = LogManager.GetLogger("Main");
		}


		/// <summary>
		/// Bind control elements to current controller state
		/// </summary>
		private void BindState()
		{
			// initial state
			if (logic.ImagesList == null)
			{
				tbSelectedFolder.Text = "<select folder>";
				lblTotalImagesCount.Text = "";
				lblUnrecognizedFilesCount.Text = "";
				btnStartStop.Text = "Start tagging";
				btnStartStop.Enabled = false;
				tsLblStatus.Text = "Select folder to start...";
			}
			else
			{
				tbSelectedFolder.Text = logic.SelectedDirectory;
				lblTotalImagesCount.Text = logic.ImagesList.Count.ToString();
				lblUnrecognizedFilesCount.Text = logic.UnrecognizedFilesCount.ToString();
				btnStartStop.Text = "Start";
				btnStartStop.Enabled = true;
				tsLblStatus.Text = "Press start to begin tagging";
			}
		}

		private void btnSelectFolder_Click(object sender, EventArgs e)
		{
			var res = fbDlg.ShowDialog();
			if (res == DialogResult.OK)
			{
				if (!logic.SelectDirectory(fbDlg.SelectedPath))
					MessageBox.Show(logic.LastError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				BindState();
			}
		}

		private void frmMain_Load(object sender, EventArgs e)
		{
			logic = new MainFormController(logger);
		}
	}
}
