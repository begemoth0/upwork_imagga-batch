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

namespace ImaggaBatchUploader
{
	public partial class SettingsForm : Form
	{
		public Settings SettingsObject { get; private set; }
		public SettingsForm(Settings settings)
		{
			InitializeComponent();
			this.SettingsObject = settings;
		}

		private void SettingsForm_Load(object sender, EventArgs e)
		{
			tbKey.Text = SettingsObject.ApiKey;
			tbSecret.Text = SettingsObject.ApiSecret;
			tbExtensions.Text = string.Join(' ', SettingsObject.ImageExtensions);
			tbEndpoint.Text = SettingsObject.ApiEndpoint;
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			try
			{
				// collect settings object
				Settings sobj = SettingsObject.Clone();
				sobj.ApiKey = tbKey.Text.Trim();
				sobj.ApiSecret = tbSecret.Text.Trim();
				sobj.ApiEndpoint = tbEndpoint.Text.Trim();
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
