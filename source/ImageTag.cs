using System;
using System.Collections.Generic;
using System.Text;

namespace ImaggaBatchUploader
{
	/// <summary>
	/// Class that represents image tag for single language
	/// </summary>
	public class ImageTag
	{
		/// <summary>
		/// Name of the tagged file
		/// </summary>
		public string Filename { get; set; }
		/// <summary>
		/// Tag value in the specified language
		/// </summary>
		public string Value { get; set; }
		/// <summary>
		/// Confidence level for the specified tag
		/// </summary>
		public double Confidence { get; set; }
		/// <summary>
		/// Tag language 
		/// </summary>
		public string Language { get; set; }
	}
}
