using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImageBatchUploader.Api.Imagga
{
	/// <summary>
	/// Imagga API call result
	/// </summary>
	public class ApiResponse
	{
		public class ApiStatus
		{
			/// <summary>
			/// success / error depending on whether the request was processed successfully;
			/// </summary>
			[JsonProperty("text")]
			public string Text { get; set; }
			/// <summary>
			/// human-readable reason why the processing was unsuccessful;
			/// </summary>
			[JsonProperty("type")]
			public string Type { get; set; }
		}
		[JsonProperty("status")]
		public ApiStatus Status { get; set; }
		/// <summary>
		/// Deserialized properties with no matching class member
		/// </summary>
		[JsonExtensionData]
		public IDictionary<string, JToken> ExtraData;
	}
}
