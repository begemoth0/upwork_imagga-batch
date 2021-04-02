﻿using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

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
		public ApiClient(string baseUrl, string apiKey, string apiSecret, int timeout = 60)
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

		/// <summary>
		/// Call tag method https://docs.imagga.com/#tags passing image file name as parameter
		/// </summary>
		/// <param name="imagePath">Local absolute path to image file</param>
		/// <returns></returns>
		public TagsMethodResponse TagsByImagePath(string imagePath)
		{
			var request = CreateRequest("/tags", Method.POST);
			request.AddFile("image", imagePath);
			var response = client.Execute(request);

			if (response.StatusCode == System.Net.HttpStatusCode.OK)
			{
				var result = JsonConvert.DeserializeObject<TagsMethodResponse>(response.Content);
				return result;
			}
			else
				throw new WebException(response.Content, response.ErrorException);
		}
	}
}
