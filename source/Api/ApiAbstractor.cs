using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Globalization;

using ImageBatchUploader;

using CsvHelper;
using CsvHelper.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImageBatchUploader.Api
{
	/// <summary>
	/// Stateful class that abstracts implementation details for different APIs.
	/// One instance should be instantiated once tagging starts and disposed
	/// when tagging is over.
	/// 
	/// Also this class provides some static methods which behavior depends on
	/// used API.
	/// </summary>
	public class ApiAbstractor : IDisposable
	{
		private CsvWriter tagsCsv = null, errorsCsv = null;
		private Settings settings;
		public string SelectedDirectory { get; private set; }
		public string TagsCsvPath { get; private set; }
		public string ErrorsCsvPath { get; private set; }
		private bool deleteErrorsOnCleanup = true;
		private NLog.Logger logger;

		private Imagga.ApiClient imaggaApi;
		private Everypixel.ApiClient everypixelApi;
		void IDisposable.Dispose()
		{
			ReleaseOutputFiles();
		}

		public ApiAbstractor(NLog.Logger logger, Settings settings, string folderPath)
		{
			// save stateful variables
			this.logger = logger;
			this.settings = settings;
			this.SelectedDirectory = folderPath;
			// compose output files paths
			var folderName = Path.GetFileName(folderPath);
			var targetDir = Directory.GetParent(folderPath).FullName;
			if (settings.DefaultApi == null)
				throw new Exception("Default API setting can't be null");
			string apiTypeString = settings.DefaultApi.ToString().ToLower();
			this.ErrorsCsvPath = Path.Combine(targetDir, $"errors-{folderName}.{apiTypeString}.csv");
			this.TagsCsvPath = Path.Combine(targetDir, $"tags-{folderName}.{apiTypeString}.csv");
		}
		/// <summary>
		/// List files that have already been processed
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="directory"></param>
		/// <returns></returns>
		public List<ImageTag> LoadSavedState()
		{
			if (File.Exists(TagsCsvPath))
			{
				logger.Debug($"Loading saved state file '{TagsCsvPath}'.");
				using (var csvReader = new CsvReader(File.OpenText(TagsCsvPath), GetCsvConfiguration()))
				{
					var records = csvReader.GetRecords<ImageTag>().ToList();
					return records;
				}
			}
			return new List<ImageTag>();
		}

		public void InstantiateApiClient()
		{
			switch (settings.DefaultApi)
			{
				case ApiType.Imagga:
					var st = settings.Imagga;
					if (st == null)
						throw new Exception("Unable to find Imagga section in settings.");
					logger.Debug($"Instantiating API client. Type: {settings.DefaultApi}, API Key: {st.ApiKey}, endpoint: {st.ApiEndpoint}. Tagging threshold: {st.TaggingThreshold}");
					imaggaApi = new Imagga.ApiClient(st.ApiEndpoint, st.ApiKey, st.ApiSecret);
					break;
				case ApiType.Everypixel:
					var est = settings.Everypixel;
					if (est == null)
						throw new Exception("Unable to find Everypixel section in settings.");
					logger.Debug($"Instantiating API client. Type: {settings.DefaultApi}, API Key: {est.ApiKey}, endpoint: {est.ApiEndpoint}. Tagging threshold: {est.TaggingThreshold}");
					everypixelApi = new Everypixel.ApiClient(est.ApiEndpoint, est.ApiKey, est.ApiSecret);
					break;

			default:
					throw new Exception($"Unknown default api: {settings.DefaultApi}");
			}
		}
		/// <summary>
		/// Lock output files for writing. Rewrites tags file with contents of tags list.
		/// </summary>
		/// <param name="tags"></param>
		/// <param name="errors"></param>
		public void LockOutputFiles(List<ImageTag> tags)
		{
			tagsCsv = new CsvWriter(new StreamWriter(TagsCsvPath, false, new UTF8Encoding(true)), GetCsvConfiguration());
			// overwrite existing file with restored data to ensure consistency
			tagsCsv.WriteRecords(tags);
			tagsCsv.Flush();
			// initialize error output
			errorsCsv = new CsvWriter(new StreamWriter(ErrorsCsvPath, false, new UTF8Encoding(true)), GetCsvConfiguration());
			errorsCsv.WriteHeader<ImageError>();
			errorsCsv.NextRecord();
			errorsCsv.Flush();
		}

		/// <summary>
		/// Closes CSV writers and deletes errors CSV file if nothing has been written.
		/// </summary>
		public void ReleaseOutputFiles()
		{
			// close all CSV writers
			if (tagsCsv != null)
				tagsCsv.Dispose();
			if (errorsCsv != null)
			{
				errorsCsv.Dispose();
				// TODO: check for correct condition whether we need to delete errors file
				if (deleteErrorsOnCleanup)
					File.Delete(ErrorsCsvPath);
			}
		}

		/// <summary>
		/// Checks for API calls quota availability.
		/// </summary>
		/// <param name="ct"></param>
		/// <param name="quotaNeeded"></param>
		public async Task<(int limit, int used)?> CheckApiQuotas(CancellationToken ct)
		{
			if (imaggaApi != null)
			{
				var usage = await imaggaApi.Usage(ct);
				// TODO: check if await works a
				var monthlyLimit = usage.ExtraData["result"].Value<int>("monthly_limit");
				var monthlyRequests = usage.ExtraData["result"].Value<int>("monthly_processed");
				return (monthlyLimit, monthlyRequests);
			}
			return null;
		}

		public async Task<List<ImageTag>> TagSingleImage(string fname, CancellationToken ct)
		{
			var imagePath = Path.Combine(SelectedDirectory, fname);
			var result = new List<ImageTag>();
			if (imaggaApi != null)
			{
				var response = await imaggaApi.TagsByImagePath(imagePath, settings.Imagga.TaggingThreshold, ct);
				// wait without throwing exception on cancelling 
				foreach (var t in response.Result.Tags)
				{
					foreach (var tagLanguagePair in t.Tag)
					{
						var tag = new ImageTag()
						{
							Filename = fname,
							Confidence = Math.Round(t.Confidence),
							Language = tagLanguagePair.Key,
							Value = tagLanguagePair.Value
						};
						result.Add(tag);
					}
				}
			}
			if (everypixelApi != null)
			{
				var response = await everypixelApi.Keywords(imagePath, settings.Everypixel.TaggingThreshold / 100, ct);
				result = response.Keywords.Select(a => new ImageTag()
				{
					Filename = fname,
					Confidence = Math.Round(a.Score * 100),
					Value = a.Keyword
				}).ToList();
			}
			return result;
		}

		/// <summary>
		/// Converts API call exception to 
		/// </summary>
		/// <param name="ex"></param>
		/// <param name="fname"></param>
		/// <returns></returns>
		public ImageError ExceptionToError(Exception ex, string fname)
		{
			var error = new ImageError()
			{
				Filename = fname,
				ErrorDescription = ex.Message
			};
			// unwrap api exception if possible
			if (ex is AggregateException agg && agg.InnerException is ApiException aex)
			{
				error.ErrorDescription = aex.Message;
				var httpCode = (int)aex.HttpCode;
				error.HttpCode = httpCode.ToString();
				error.QuotaExceeded =
					// https://labs.everypixel.com/api/docs
					// 429 - you have exceeded available requests from your plan.
					(settings.DefaultApi == ApiType.Everypixel && httpCode == 429) ||
					// https://docs.imagga.com/#errors
					// Forbidden – You have reached one of your subscription limits therefore you are temporarily not allowed to perform this type of request.
					(settings.DefaultApi == ApiType.Imagga && httpCode == 403);
			};
			return error;
		}

		public void WriteTagToCsv(List<ImageTag> tags)
		{
			foreach (var tag in tags)
			{
				tagsCsv.WriteRecord(tag);
				tagsCsv.NextRecord();
			}
			tagsCsv.Flush();
		}

		public void WriteErrorToCsv(ImageError error)
		{
			deleteErrorsOnCleanup = false;
			errorsCsv.WriteRecord(error);
			errorsCsv.NextRecord();
			errorsCsv.Flush();
		}

		/// <summary>
		/// Helper method to ensure consistent CSV configuratiion
		/// </summary>
		/// <returns></returns>
		private static CsvConfiguration GetCsvConfiguration()
		{
			var config = new CsvConfiguration(CultureInfo.InvariantCulture)
			{
				Delimiter = ";",
				LeaveOpen = false
			};
			return config;
		}
	}
}
