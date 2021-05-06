using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImageBatchUploader.Api.Everypixel
{
	public class KeywordsMethodResponse : ApiResponse
	{
		public class KeywordItem
		{
			[JsonProperty("keyword")]
			public string Keyword { get; set; }
			[JsonProperty("score")]
			public double Score { get; set; }
		}
		[JsonProperty("keywords")]
		public KeywordItem[] Keywords { get; set; }
	}
}
