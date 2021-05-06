using System;
using System.Collections.Generic;
using System.Text;

namespace ImageBatchUploader
{
	/// <summary>
	/// Settings for application
	/// </summary>
	public class Settings : ICloneable
	{
		/// <summary>
		/// Settings which differ by API
		/// </summary>
		public class EndpointSettings : ICloneable
		{
			public string ApiKey { get; set; }
			public string ApiSecret { get; set; }
			public string ApiEndpoint { get; set; }
			public int TaggingThreshold { get; set; }

			public EndpointSettings Clone()
			{
				return (EndpointSettings)((ICloneable)this).Clone();
			}
			object ICloneable.Clone()
			{
				return MemberwiseClone();
			}
		}
		/// <summary>
		/// Currently selected API to use
		/// </summary>
		public Api.ApiType? DefaultApi { get; set; }
		/// <summary>
		/// Files with which extensions are considered images
		/// </summary>
		public string[] ImageExtensions { get; set; }
		/// <summary>
		/// Endpoint settings for Imagga.com API
		/// </summary>
		public EndpointSettings Imagga { get; set; }
		/// <summary>
		/// Endpoint settings for 
		/// </summary>
		public EndpointSettings Everypixel { get; set; }

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
