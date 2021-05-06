using System;
using System.Collections.Generic;
using System.Text;

namespace ImageBatchUploader.Api
{
	class ApiException : Exception
	{
		/// <summary>
		/// Http code of response with exception if received 
		/// </summary>
		public System.Net.HttpStatusCode HttpCode { get; private set; }
		public ApiException(string message, System.Net.HttpStatusCode responseCode, Exception innerException) : base(message, innerException)
		{
			HttpCode = responseCode;
		}
	}
}
