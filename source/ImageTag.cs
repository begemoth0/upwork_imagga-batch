using System;
using System.Collections.Generic;
using System.Text;

namespace ImageBatchUploader
{
	/// <summary>
	/// Class that represents image tag for single language
	/// </summary>
	public class ImageTag
	{
		/// <summary>
		/// Name of the tagged file
		/// </summary>
		[CsvHelper.Configuration.Attributes.Index(0)]
		[CsvHelper.Configuration.Attributes.Name("filename")]
		public string Filename { get; set; }
		/// <summary>
		/// Tag value in the specified language
		/// </summary>
		[CsvHelper.Configuration.Attributes.Index(1)]
		[CsvHelper.Configuration.Attributes.Name("tag")] 
		public string Value { get; set; }
		/// <summary>
		/// Confidence level for the specified tag
		/// </summary>
		[CsvHelper.Configuration.Attributes.Index(2)]
		[CsvHelper.Configuration.Attributes.Name("confidence")]
		public double Confidence { get; set; }
		/// <summary>
		/// Tag language 
		/// </summary>
		[CsvHelper.Configuration.Attributes.Index(0)]
		[CsvHelper.Configuration.Attributes.Name("language")]
		public string Language { get; set; }
	}
}
