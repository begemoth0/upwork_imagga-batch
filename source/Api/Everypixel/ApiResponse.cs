using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImageBatchUploader.Api.Everypixel
{
	public class ApiResponse
	{
		[JsonProperty("status")]
		public string Status { get; set; }
		/// <summary>
		/// Deserialized properties with no matching class member
		/// </summary>
		[JsonExtensionData]
		public IDictionary<string, JToken> ExtraData;
	}
}
