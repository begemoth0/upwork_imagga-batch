using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using Newtonsoft.Json;

namespace ImageBatchUploader
{
	public partial class SettingsForm : Form
	{
		public Settings SettingsObject { get; private set; }
		public SettingsForm(Settings original, Settings overrided)
		{
			InitializeComponent();
			this.SettingsObject = SettingsController.Merge(original, overrided);
			// deny editing for override mode
			if (overrided != null)
			{
				btnSave.Enabled = false;
				lblOverrideNotification.Visible = true;
			}
			else
			{
				btnSave.Enabled = true;
				lblOverrideNotification.Visible = false;
			}
		}

		private void SettingsForm_Load(object sender, EventArgs e)
		{
			tbKey.Text = SettingsObject.Imagga.ApiKey;
			tbSecret.Text = SettingsObject.Imagga.ApiSecret;
			tbExtensions.Text = string.Join(' ', SettingsObject.ImageExtensions);
			tbEndpoint.Text = SettingsObject.Imagga.ApiEndpoint;
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			try
			{
				// collect settings object
				Settings sobj = SettingsObject.Clone();
				sobj.Imagga.ApiKey = tbKey.Text.Trim();
				sobj.Imagga.ApiSecret = tbSecret.Text.Trim();
				sobj.Imagga.ApiEndpoint = tbEndpoint.Text.Trim();
				sobj.ImageExtensions = tbExtensions.Text
					.Split(' ', StringSplitOptions.RemoveEmptyEntries)
					.Where(a => a.IndexOfAny(Path.GetInvalidFileNameChars()) < 0)
					.ToArray();
				// try to save data
				SettingsController.SaveSettings(sobj);
				// replace settings object if successfull
				SettingsObject = sobj;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Can't save settings file: {ex.Message}.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				DialogResult = DialogResult.None;
			}
		}
	}
}
