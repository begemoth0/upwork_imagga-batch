using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ImaggaBatchUploader
{
	public static class SettingsController
	{
		private const string settingsFilename = "settings.json";
		/// <summary>
		/// Load settings from default storage
		/// </summary>
		/// <returns></returns>
		public static Settings LoadSettings()
		{
			return LoadSettingsRaw(settingsFilename);
		}

		/// <summary>
		/// Save settings to default storage
		/// </summary>
		/// <param name="settings"></param>
		/// <returns></returns>
		public static void SaveSettings(Settings settings)
		{
			using (var jw = new JsonTextWriter(new StreamWriter(settingsFilename)))
			{
				var js = new JsonSerializer();
				js.Formatting = Formatting.Indented;
				js.Serialize(jw, settings);
			}
		}
		/// <summary>
		/// Load settings override file from the specified folder
		/// </summary>
		/// <returns>Settings object or null if settings file not exists</returns>
		public static Settings TryLoadSettingsOveride(string directory)
		{
			var path = Path.Combine(directory, settingsFilename);
			if (File.Exists(settingsFilename))
				return LoadSettingsRaw(path);
			else
				return null;
		}

		private static Settings LoadSettingsRaw(string path)
		{
			var settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(path));
			return settings;
		}
	}
}
