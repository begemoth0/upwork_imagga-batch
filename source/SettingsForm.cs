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
		/// <summary>
		/// Settings suboject where we store 
		/// </summary>
		private Settings.EndpointSettings stashed;
		private Settings.EndpointSettings displayed;

		public SettingsForm(Settings original, Settings overrided)
		{
			InitializeComponent();
			this.SettingsObject = SettingsController.Merge(original, overrided);
			// set default values
			if (SettingsObject.DefaultApi == null)
				SettingsObject.DefaultApi = Api.ApiType.Imagga;
			if (SettingsObject.Imagga == null)
				SettingsObject.Imagga = new Settings.EndpointSettings();
			if (SettingsObject.Everypixel == null)
				SettingsObject.Everypixel = new Settings.EndpointSettings();
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

		private void BindEndpointSettings(Settings.EndpointSettings epSettings)
		{
			displayed = epSettings;
			tbKey.Text = epSettings.ApiKey;
			tbSecret.Text = epSettings.ApiSecret;
			tbEndpoint.Text = epSettings.ApiEndpoint;
		}
		private Settings.EndpointSettings CollectEndpointSettings()
		{
			var s = displayed.Clone();
			s.ApiKey = tbKey.Text.Trim();
			s.ApiSecret = tbSecret.Text.Trim();
			s.ApiEndpoint = tbEndpoint.Text.Trim();
			return s;
		}
		private void SettingsForm_Load(object sender, EventArgs e)
		{
			switch (SettingsObject.DefaultApi)
			{
				case Api.ApiType.Everypixel:
					rbtnEverypixel.Checked = true;
					stashed = SettingsObject.Imagga;
					BindEndpointSettings(SettingsObject.Everypixel);
					break;
				case Api.ApiType.Imagga:
					rbtnImagga.Checked = true;
					stashed = SettingsObject.Everypixel;
					BindEndpointSettings(SettingsObject.Imagga);
					break;
			}
			if (stashed == null)
				stashed = new Settings.EndpointSettings();
			tbExtensions.Text = string.Join(' ', SettingsObject.ImageExtensions);
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			try
			{
				// collect settings object
				Settings sobj = SettingsObject.Clone();
				if (rbtnImagga.Checked)
				{
					sobj.DefaultApi = Api.ApiType.Imagga;
					sobj.Imagga = CollectEndpointSettings();
					sobj.Everypixel = stashed;
				}
				if (rbtnEverypixel.Checked)
				{
					sobj.DefaultApi = Api.ApiType.Everypixel;
					sobj.Everypixel = CollectEndpointSettings();
					sobj.Imagga = stashed;
				}
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

		private void rbtnChecked(object sender, EventArgs e)
		{
			// 'form is loading' condition
			if (stashed != null)
			{
				// change places
				var toDisplay = stashed;
				stashed = CollectEndpointSettings();
				BindEndpointSettings(toDisplay);
			}
		}
	}
}
