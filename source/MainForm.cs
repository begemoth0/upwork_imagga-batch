using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace ImaggaBatchUploader
{
	public partial class MainForm : Form
	{
		private static Logger logger;
		private MainFormController logic;
		private Settings settings;

		public MainForm()
		{
			InitializeComponent();
			var configuration = LogManager.Configuration;
			var tbTarget = new TextBoxTarget(tbLog)
			{
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
				tbSelectedFolder.Text = "                         <press this button to select folder -->";
				lblTotalImagesCount.Text = "";
				lblUnrecognizedFilesCount.Text = "";
				btnStartStop.Text = "Start tagging";
				btnStartStop.Enabled = false;
				tsLblStatus.Text = "Select folder to start...";
			}
			else
			{
				toolStripProgressBar1.Maximum = logic.ImagesList.Count;
				tbSelectedFolder.Text = logic.SelectedDirectory;
				lblTotalImagesCount.Text = logic.ImagesList.Count.ToString();
				lblUnrecognizedFilesCount.Text = logic.UnrecognizedFilesCount.ToString();
				btnStartStop.Enabled = true;
				llProcessedCount.Text = logic.Tags.Count.ToString();
				llProcessedCount.Tag = logic.TagsCsvPath;
				llProcessedCount.Enabled = logic.Tags.Count > 0;
				llErrorsCount.Text = logic.Errors.Count.ToString();
				llErrorsCount.Tag = logic.ErrorsCsvPath;
				llErrorsCount.Enabled = logic.Errors.Count > 0;
				BindProgressCounters(logic.Tags, logic.Errors);

				if (logic.IsTaggingInProcess)
				{
					toolStripProgressBar1.Visible = true;
					btnStartStop.Text = "Stop";
					tsLblStatus.Text = "Tagging...";
				}
				else
				{
					// winforms progress bar is animating while visible so it's better to hide it while not tagging
					toolStripProgressBar1.Visible = false;
					btnStartStop.Text = "Start";
					tsLblStatus.Text = "Press start to begin";
				}
			}
		}

		private void btnSelectFolder_Click(object sender, EventArgs e)
		{
			var res = fbDlg.ShowDialog();
			if (res == DialogResult.OK)
			{
				if (!logic.SelectDirectory(fbDlg.SelectedPath, settings.ImageExtensions))
					MessageBox.Show(logic.LastError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				BindState();
			}
		}

		private void frmMain_Load(object sender, EventArgs e)
		{
			settings = SettingsController.LoadSettings();
			logic = new MainFormController(logger);
		}


		private void BindProgressCounters(List<ImageTag> tags, List<ImageError> errors)
		{
			var procesedImages = tags.Select(a => a.Filename).Distinct().Count();
			var errorImages = errors.Select(a => a.Filename).Distinct().Count();
			llProcessedCount.Text = procesedImages.ToString();
			llErrorsCount.Text = errorImages.ToString();
			toolStripProgressBar1.Value = procesedImages + errorImages;
		}
		private void BatchUpdatedCallback()
		{
			this.Invoke(new Action(() =>
			{
				BindProgressCounters(logic.Tags, logic.Errors);
			}));
		}

		private bool stoppedManually = false;
		private void btnStartStop_Click(object sender, EventArgs e)
		{
			if (!logic.IsTaggingInProcess)
			{
				var task = logic.StartTagging(BatchUpdatedCallback, BatchFinishedCallback, settings);
				BindState();
				if (task != null)
				{
					task.Start();
				}
				else
				{
					MessageBox.Show(logic.LastError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			else
			{
				stoppedManually = true;
				logic.StopTagging();
			}
		}

		private void BatchFinishedCallback(bool success)
		{
			this.Invoke(new Action(() =>
			{
				if (!stoppedManually)
				{
					if (success)
					{
						if (logic.Errors.Count > 0)
							MessageBox.Show("Batch finished with errors.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						else
							MessageBox.Show("Batch finished successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
					else
						MessageBox.Show(logic.LastError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

				}
				else
				{
					stoppedManually = false;
				}
				BindState();
			}));
		}

		private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
		{
			logic.StopTagging();
		}

		private void btnSettings_Click(object sender, EventArgs e)
		{
			var frmSettings = new SettingsForm(settings);
			
			if (frmSettings.ShowDialog() == DialogResult.OK)
			{
				this.settings = frmSettings.SettingsObject;
			}
		}

		private void OpenFileFromTag(string filename)
		{
			var p = new Process();
			p.StartInfo.FileName = filename;
			p.StartInfo.UseShellExecute = true;
			p.Start();
		}
		private void llErrorsCount_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			OpenFileFromTag((string)llErrorsCount.Tag);
		}

		private void llProcessedCount_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			OpenFileFromTag((string)llProcessedCount.Tag);
		}
	}
}
