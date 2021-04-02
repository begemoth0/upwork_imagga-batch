using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImaggaBatchUploader.Rest
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
	}
}
