using System;
using System.Collections.Generic;
using System.Text;

namespace ImaggaBatchUploader
{
	/// <summary>
	/// Settings for application
	/// </summary>
	public class Settings : ICloneable
	{
		public string ApiKey { get; set; }
		public string ApiSecret { get; set; }
		public string ApiEndpoint { get; set; }
		public string[] ImageExtensions { get; set; }
		public int TaggingThreshold { get; set; }

		public Settings Clone()
		{
			return (Settings)((ICloneable)this).Clone();
		}
		object ICloneable.Clone()
		{
			return MemberwiseClone();
		}
	}
}
