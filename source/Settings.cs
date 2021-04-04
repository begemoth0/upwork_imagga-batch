using System;
using System.Collections.Generic;
using System.Text;

namespace ImaggaBatchUploader
{
	/// <summary>
	/// Settings for application
	/// </summary>
	public class Settings
	{
		public string ApiKey { get; set; }
		public string ApiSecret { get; set; }
		public string ApiEndpoint { get; set; }
		public string[] ImageExtensions { get; set; }
	}
}
