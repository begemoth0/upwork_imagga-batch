using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImaggaBatchUploader.Rest
{
	/// <summary>
	/// Response to /tags method, according to https://docs.imagga.com/?csharp#tags
	/// </summary>
	public class TagsMethodResponse : ApiResponse
	{
		public class TagItem
		{
			/// <summary>
			/// A number representing a percentage from 0 to 100 where 100 means that the API is absolutely sure this tag must be relevant and confidence < 30 means that there is a higher chance that the tag might not be such;
			/// </summary>
			[JsonProperty("confidence")]
			public double Confidence { get; set; }
			/// <summary>
			/// Key: language. Value: tag name.
			/// The tag itself which could be an object, concept, color, etc. describing something from the photo scene;
			/// </summary>
			[JsonProperty("tag")]
			public Dictionary<string, string> Tag { get; set; }
		}
	}
}
