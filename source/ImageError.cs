using System;
using System.Collections.Generic;
using System.Text;

namespace ImaggaBatchUploader
{
	public class ImageError
	{
		/// <summary>
		/// Name of the tagged file
		/// </summary>
		[CsvHelper.Configuration.Attributes.Index(0)]
		[CsvHelper.Configuration.Attributes.Name("filename")]
		public string Filename { get; set; }
		/// <summary>
		/// HTTP return code (if any)
		/// </summary>
		[CsvHelper.Configuration.Attributes.Index(1)]
		[CsvHelper.Configuration.Attributes.Name("http_code")]
		public string HttpCode { get; set; }
		/// <summary>
		/// Error description provided by service or determined by http code
		/// </summary>
		[CsvHelper.Configuration.Attributes.Index(2)]
		[CsvHelper.Configuration.Attributes.Name("error_description")]
		public string ErrorDescription { get; set; }
	}
}
