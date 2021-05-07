using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImageBatchUploader.Api.Everypixel
{
	public class ApiClient
	{
		private RestClient client;
		private string authHeader;
		//private Logger
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="baseUrl">Base API URL (eg https://api.everypixel.com/) </param>
		/// <param name="clientID">API user account name</param>
		/// <param name="secret">API user secret</param>
		/// <param name="timeout">API calls timeout in seconds</param>
		public ApiClient(string baseUrl, string clientID, string secret, int timeout = 120000)
		{
			authHeader = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientID}:{secret}"));
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
			// throw exception if needed
			if (response.ErrorException != null)
				throw response.ErrorException;
			// try to extract JSON from response body
			if (!string.IsNullOrEmpty(response.Content))
			{
				T responseData = JsonConvert.DeserializeObject<T>(response.Content);
				var genericResponse = responseData as ApiResponse;
				if (genericResponse.Status == "error")
					throw new ApiException(genericResponse.ExtraData["message"].ToString(), response.StatusCode, null);
				return responseData;
			}
			throw new ApiException(response.StatusCode.ToString(), response.StatusCode, response.ErrorException);
		}
		public async Task<KeywordsMethodResponse> Keywords(string imagePath, int threshold, CancellationToken token)
		{
			var request = CreateRequest("v1/keywords", Method.POST);
			request.AddFile("data", imagePath);
			if (threshold > 0)
				request.AddParameter("threshold", threshold);
			return await ExecuteRequest<KeywordsMethodResponse>(request, token);
		}
	}
}
