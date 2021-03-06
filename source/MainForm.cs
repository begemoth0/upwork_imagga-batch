using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ImageBatchUploader
{
	public partial class MainForm : Form
	{
		private static Logger logger;
		private MainFormController logic;

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
				tbSelectedFolder.Text = logic.Api.SelectedDirectory;
				lblTotalImagesCount.Text = logic.ImagesList.Count.ToString();
				lblUnrecognizedFilesCount.Text = logic.UnrecognizedFilesCount.ToString();
				btnStartStop.Enabled = true;
				llProcessedCount.Text = logic.Tags.Count.ToString();
				llProcessedCount.Tag = logic.Api.TagsCsvPath;
				llErrorsCount.Text = logic.Errors.Count.ToString();
				llErrorsCount.Tag = logic.Api.ErrorsCsvPath;
				BindProgressCounters(logic.Tags, logic.Errors);
				btnSelectFolder.Enabled = !logic.IsTaggingInProcess;
				if (logic.IsTaggingInProcess)
				{
					toolStripProgressBar1.Visible = true;
					btnStartStop.Text = "Stop";
					tsLblStatus.Text = "Tagging...";
					btnSettings.Enabled = false;
				}
				else
				{
					btnSettings.Enabled = true;
					// winforms progress bar is animating while visible so it's better to hide it while not tagging
					toolStripProgressBar1.Visible = false;
					btnStartStop.Text = "Start";
					tsLblStatus.Text = "Press start to begin";
				}
			}
		}
		/// <summary>
		/// Perform directory selection logic with error handling
		/// </summary>
		/// <param name="path"></param>
		private void SelectFolder(string path)
		{
			if (!logic.SelectDirectory(path))
				MessageBox.Show(logic.LastError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			BindState();
		}
		private void btnSelectFolder_Click(object sender, EventArgs e)
		{
			var res = fbDlg.ShowDialog();
			if (res == DialogResult.OK)
				SelectFolder(fbDlg.SelectedPath);
		}

		private void frmMain_Load(object sender, EventArgs e)
		{
			logic = new MainFormController(logger);
		}


		private void BindProgressCounters(List<ImageTag> tags, List<ImageError> errors)
		{
			var procesedImages = tags.Select(a => a.Filename).Distinct().Count();
			var errorImages = errors.Select(a => a.Filename).Distinct().Count();
			llProcessedCount.Text = procesedImages.ToString();
			llErrorsCount.Text = errorImages.ToString();
			llErrorsCount.Enabled = logic.Errors.Count > 0;
			llProcessedCount.Enabled = logic.Tags.Count > 0;
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
		private bool isExiting = false;
		private async void btnStartStop_Click(object sender, EventArgs e)
		{
			if (!logic.IsTaggingInProcess)
			{
				logic.InitializeTaggingTask(BatchUpdatedCallback);
				BindState();
				logic.TaggingTask.Start();
				bool success = await logic.TaggingTask;
				if (isExiting)
					return;
				if (!stoppedManually)
				{
					if (success)
					{
						var errors = new List<string>();
						if (logic.QuotaExceeded)
							errors.Add("API quota exceeded. Only part of images have been processed.");
						if (logic.Errors.Count > 0)
							errors.Add("Some images processed with errors.");
						if (errors.Count > 0)
						{
							MessageBox.Show(string.Join(' ', errors), "Success (almost :)", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						}
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
			}
			else
			{
				stoppedManually = true;
				logic.StopTagging();
			}
		}

		private async void frmMain_FormClosing(object sender, FormClosingEventArgs e)
		{
			isExiting = true;
			logic.StopTagging();
			if (logic.IsTaggingInProcess)
			{
				e.Cancel = true;
				await logic.TaggingTask;
				Close();
			}
		}

		private void btnSettings_Click(object sender, EventArgs e)
		{
			var frmSettings = new SettingsForm(logic.SettingsOriginal, logic.SettingsOverride);
			if (frmSettings.ShowDialog() == DialogResult.OK)
			{
				logic.UpdateSettings(frmSettings.SettingsObject);
				var settingsString = SettingsController.SettingsToString(frmSettings.SettingsObject);
				// reload directory if anything is changed
				if (!string.IsNullOrEmpty(fbDlg.SelectedPath))
				{
					logger.Debug($"Settings saved. Reloading selected folder. {settingsString}");
					SelectFolder(fbDlg.SelectedPath);
				}
				else
					logger.Debug($"Settings saved. {settingsString}");
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

		private void MainForm_DragEnter(object sender, DragEventArgs e)
		{
			// we want only filedrops for directories
			if (!logic.IsTaggingInProcess
				&& e.Data.GetData(DataFormats.FileDrop) is string[] path
				&& path.Length == 1
				&& Directory.Exists(path[0]))
			{
				e.Effect = DragDropEffects.Link;
			}
			else
				e.Effect = DragDropEffects.None;
		}

		private void MainForm_DragDrop(object sender, DragEventArgs e)
		{
			var path = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
			logger.Debug($"Drag-and-dropped: {path}");
			SelectFolder(path);
		}
	}
}
