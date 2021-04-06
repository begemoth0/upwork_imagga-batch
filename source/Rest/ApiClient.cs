using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImaggaBatchUploader.Rest
{
	/// <summary>
	/// Imagga.com API client
	/// </summary>
	public class ApiClient
	{
		private RestClient client;
		private string authHeader;
		//private Logger
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="baseUrl">Base API URL (eg https://api.imagga.com/v2) </param>
		/// <param name="apiKey">API user account name</param>
		/// <param name="apiSecret">API user secret</param>
		/// <param name="timeout">API calls timeout in seconds</param>
		public ApiClient(string baseUrl, string apiKey, string apiSecret, int timeout = 120000)
		{
			authHeader = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{apiKey}:{apiSecret}"));
			client = new RestClient(baseUrl);
			client.Timeout = timeout;
		}

		/// <summary>
		/// Create request with authorization header
		/// </summary>
		private RestRequest CreateRequest(string resource, Method method)
		{
			var request = new RestRequest(resource, method);
			request.AddHeader("Authorization", authHeader);
			return request;
		}

		private async Task<T> ExecuteRequest<T>(RestRequest request, CancellationToken ct)
		{
			var response = await client.ExecuteAsync(request, ct);
			if (response.StatusCode == System.Net.HttpStatusCode.OK)
			{
				var result = JsonConvert.DeserializeObject<T>(response.Content);
				return result;
			}
			else
			{
				var msg = response.ErrorException != null ? response.ErrorException.Message : response.StatusDescription;
				try
				{
					// try to extract info from response body
					if (!string.IsNullOrEmpty(response.Content))
					{
						var error = JsonConvert.DeserializeObject<ApiResponse>(response.Content);
						if (error != null)
							msg = error.Status.Text;
					}
				}
				catch
				{ 
				}
				throw new ApiException(msg, response.StatusCode, response.ErrorException);
			}
		}

		/// <summary>
		/// Call tag method https://docs.imagga.com/#tags passing image file name as parameter
		/// </summary>
		/// <param name="imagePath">Local absolute path to image file</param>
		/// <param name="threshold">Thresholds the confidence of tags in the result to the number you set. Double value is expected. By default all tags with confidence above 7 are being returned and you cannot go lower than that. Default: 0</param>
		/// <returns></returns>
		public async Task<TagsMethodResponse> TagsByImagePath(string imagePath, int threshold, CancellationToken token)
		{
			var request = CreateRequest("v2/tags", Method.POST);
			request.AddFile("image", imagePath);
			if (threshold > 0)
				request.AddParameter("threshold", threshold);
			return await ExecuteRequest<TagsMethodResponse>(request, token);
		}

		public async Task<ApiResponse> Usage(CancellationToken token = default)
		{
			var request = CreateRequest("v2/usage", Method.GET);
			return await ExecuteRequest<ApiResponse>(request, token);
		}
	}
}
