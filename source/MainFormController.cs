using CsvHelper;
using CsvHelper.Configuration;
using ImaggaBatchUploader.Rest;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImaggaBatchUploader
{
	/// <summary>
	/// Class that represents UX logic and controls batch workflow
	/// </summary>
	public class MainFormController
	{
		private NLog.Logger logger;
		private const string TagsFilename = "tags.csv";
		private const string ErrorsFilename = "errors.csv";
		private CancellationTokenSource cancellationTokenSource;
		/// <summary>
		/// Full path to CSV file with tags
		/// </summary>
		public string TagsCsvPath { get; private set; }
		/// <summary>
		/// Full path to CSV file with errors
		/// </summary>
		public string ErrorsCsvPath { get; private set; }
		/// <summary>
		/// Error which occured during last method call
		/// </summary>
		public string LastError { get; private set; }
		/// <summary>
		/// List of images in current directory
		/// </summary>
		public List<string> ImagesList { get; private set; }
		public int UnrecognizedFilesCount { get; private set; }
		/// <summary>
		/// Currently selected directory
		/// </summary>
		public string SelectedDirectory { get; private set; }
		/// <summary>
		/// Tags for images from current session or restored state.
		/// </summary>
		public List<ImageTag> Tags { get; set; }
		public List<ImageError> Errors { get; private set; }
		public MainFormController(NLog.Logger logger)
		{
			this.logger = logger;
			logger.Info("Program started.");
		}

		/// <summary>
		/// Indicates whether tagging has been started and not finished nor cancelled
		/// </summary>
		public bool IsTaggingInProcess
		{
			get
			{
				return cancellationTokenSource != null;
			}
		}

		/// <summary>
		/// Scan selected directory for images and restore state.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public bool SelectDirectory(string path, string[] allowedExtensions)
		{
			List<string> images = null;
			int unrecognized;
			try
			{
				var files = Directory.EnumerateFiles(path);
				var extSet = allowedExtensions.ToHashSet();
				images = files.Where(a => extSet.Contains(Path.GetExtension(a))).ToList();
				var fc = files.Count();
				unrecognized = fc - images.Count;
				logger.Info($"Selected folder: {path}. Images extensions: '{string.Join(' ', allowedExtensions)}'. Total files: {fc}, images: {images.Count}.");
			}
			catch (Exception ex)
			{
				logger.Error(ex.Message);
				LastError = "Can't open selected folder. See details in intermediate output.";
				return false;
			}
			var targetDir = Directory.GetParent(path).FullName;
			var folderName = Path.GetFileName(path);
			TagsCsvPath = Path.Combine(targetDir, $"tags-{folderName}.csv");
			ErrorsCsvPath = Path.Combine(targetDir, $"errors-{folderName}.csv");
			var tags = new List<ImageTag>();
			if (File.Exists(TagsCsvPath))
			{
				try
				{
					var imageSet = images.Select(a => Path.GetFileName(a)).ToHashSet();
					using (var csvReader = new CsvReader(File.OpenText(TagsCsvPath), GetCsvConfiguration()))
					{
						var records = csvReader.GetRecords<ImageTag>().ToList();
						// discard state for non-existing files (we don't want state labels to be confusing)
						tags = records.Where(a => imageSet.Contains(a.Filename)).ToList();
						logger.Info($"Loaded saved state file '{TagsCsvPath}'.");
						var discarded = records.Count() - tags.Count;
						if (discarded > 0)
							logger.Info($"Discarded {discarded} records due to missing files.");
					}

				}
				catch (Exception ex)
				{
					logger.Error($"Can't open {TagsCsvPath}: ${ex.Message})");
					LastError = "Can't read saved state file. See intermediate output for details.";
					return false;
				}
			}

			// apply new state consistently
			ImagesList = images;
			SelectedDirectory = path;
			UnrecognizedFilesCount = unrecognized;
			Tags = tags;
			Errors = new List<ImageError>();
			return true;
		}


		/// <summary>
		/// Helper method to ensure consistent CSV configuratiion
		/// </summary>
		/// <param name="sw"></param>
		/// <returns></returns>
		private CsvConfiguration GetCsvConfiguration()
		{
			var config = new CsvConfiguration(CultureInfo.InvariantCulture)
			{
				Delimiter = ";",
				LeaveOpen = false
			};
			return config;
		}


		/// <summary>
		/// Start asynchronous tagging process. Reserves two output files in the folder above selected one.
		/// </summary>
		/// <param name="updateCallback">Delegate to call when another image in batch is processed.</param>
		/// <param name="finishedCallback">Delegate to call when task finishes. Parameter is success state.</param>
		/// <returns>Task instance if tagging is ready to start, null otherwise (check LastError then) </returns>
		public Task StartTagging(Action updateCallback, Action<bool> finishedCallback, Settings settings)
		{
			ApiClient api;
			try
			{
				logger.Debug($"Testing API client. Endpoint: {settings.ApiEndpoint}, API Key: {settings.ApiKey}");
				api = new ApiClient(settings.ApiEndpoint, settings.ApiKey, settings.ApiSecret);
			}
			catch (Exception ex)
			{
				logger.Error($"ApiCLient constructor failed! {ex.Message}");
				LastError = $"Can't instantiate API client. See intermediate output for details.";
				return null;
			}
			CsvWriter tagsCsv = null, errorCsv = null;
			// helper for cleanup consistency and code neatness
			Task cleanup(string errorMessage)
			{
				logger.Error(errorMessage);
				if (tagsCsv != null)
					tagsCsv.Dispose();
				if (errorCsv != null)
					errorCsv.Dispose();
				return null;
			}

			// lock output files for writing
			try
			{
				tagsCsv = new CsvWriter(new StreamWriter(TagsCsvPath, false, new UTF8Encoding(true)), GetCsvConfiguration());
				errorCsv = new CsvWriter(new StreamWriter(ErrorsCsvPath, false, new UTF8Encoding(true)), GetCsvConfiguration());
			}
			catch (Exception ex)
			{
				LastError = $"Could not open output file for writing. See immediate output for details.";
				return cleanup(ex.Message);
			}
			// overwrite existing file with restored data to ensure consistency
			tagsCsv.WriteRecords(Tags);
			// 
			CancellationTokenSource cts = new CancellationTokenSource();
			cancellationTokenSource = cts;
			var task = new Task(() => BatchProcessingAction(tagsCsv, errorCsv, updateCallback, finishedCallback, api, cts.Token), cts.Token);
			return task;
		}

		private void BatchProcessingAction(CsvWriter tagsCsv, CsvWriter errorsCsv,
			Action updateCallback, Action<bool> finishedCallback,
			ApiClient api,
			CancellationToken ct)
		{
			void cleanup(bool result)
			{
				// critical section to prevent race condition
				lock (this)
				{
					cancellationTokenSource = null;
				}
				// close all CSV writers
				tagsCsv.Dispose();
				errorsCsv.Dispose();
				if (Errors.Count == 0)
					File.Delete(ErrorsCsvPath);
				finishedCallback(result);
			}
			try
			{
				var usage = api.Usage();
				var monthlyLimit = usage.ExtraData["result"].Value<int>("monthly_limit");
				var monthlyRequests = usage.ExtraData["result"].Value<int>("monthly_processed");
				logger.Info($"Imagga API available. Monthly quota used: {monthlyRequests}/{monthlyLimit}.");
				var requestsLeft = monthlyLimit - monthlyRequests;
				if (requestsLeft < ImagesList.Count)
				{
					LastError = $"Insufficient API calls quota. Images to tag: {ImagesList.Count}, requests left: {requestsLeft}. Reduce number of images in the batch or upgrade API usage restrictions.";
					cleanup(false);
					return;
				}
			}
			catch (Exception ex)
			{
				LastError = "Can't start batch processing. See immediate output for details.";
				logger.Error(ex.Message);
				cleanup(false);
				return;
			}
			var processedImages = Tags.Select(a => a.Filename).ToHashSet();
			errorsCsv.WriteHeader<ImageError>();
			errorsCsv.NextRecord();
			int taggedSuccessfully = 0;
			int taggedWithErrors = 0;
			foreach (var imagePath in ImagesList)
			{
				var fname = Path.GetFileName(imagePath);
				if (processedImages.Contains(fname))
					continue;
				try
				{
					FileInfo fi = new FileInfo(imagePath);
					logger.Debug($"Tagging '{fname}', file size: {GetBytesReadable(fi.Length)}");
					var response = api.TagsByImagePath(imagePath);
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
							Tags.Add(tag);
							tagsCsv.WriteRecord(tag);
							tagsCsv.NextRecord();
						}

					}
					logger.Info($"Tagged OK: '{fname}', {response.Result.Tags.Count()} tags received");
					taggedSuccessfully += 1;
				}
				catch (Exception ex)
				{
					var aex = ex as ApiException;
					var error = new ImageError()
					{
						Filename = fname,
						HttpCode = aex != null ? ((int)aex.HttpCode).ToString() : null,
						ErrorDescription = ex.Message
					};
					Errors.Add(error);
					logger.Warn($"Tagging error: '{fname}', {error.ErrorDescription}");
					errorsCsv.WriteRecord(error);
					errorsCsv.NextRecord();
					taggedWithErrors += 1;
				}
				if (cancellationTokenSource.Token.WaitHandle.WaitOne(0))
					break;
				updateCallback();
			}
			// mock: waiting for manual cancel
			if (cancellationTokenSource.Token.WaitHandle.WaitOne(0))
				logger.Info($"Batch cancelled. Images successfully tagged in this session: {taggedSuccessfully}, with errors: {taggedWithErrors}.");
			else
				logger.Info($"Batch finished. Images successfully tagged in this session: {taggedSuccessfully}, with errors: {taggedWithErrors}.");
			cleanup(true);

		}

		public void StopTagging()
		{
			lock (this)
			{
				if (cancellationTokenSource != null)
					cancellationTokenSource.Cancel();
			}
		}

		// Returns the human-readable file size for an arbitrary, 64-bit file size 
		// The default format is "0.### XB", e.g. "4.2 KB" or "1.434 GB"
		// attributed to https://www.somacon.com/p576.php
		private string GetBytesReadable(long i)
		{
			// Get absolute value
			long absolute_i = (i < 0 ? -i : i);
			// Determine the suffix and readable value
			string suffix;
			double readable;
			if (absolute_i >= 0x1000000000000000) // Exabyte
			{
				suffix = "EB";
				readable = (i >> 50);
			}
			else if (absolute_i >= 0x4000000000000) // Petabyte
			{
				suffix = "PB";
				readable = (i >> 40);
			}
			else if (absolute_i >= 0x10000000000) // Terabyte
			{
				suffix = "TB";
				readable = (i >> 30);
			}
			else if (absolute_i >= 0x40000000) // Gigabyte
			{
				suffix = "GB";
				readable = (i >> 20);
			}
			else if (absolute_i >= 0x100000) // Megabyte
			{
				suffix = "MB";
				readable = (i >> 10);
			}
			else if (absolute_i >= 0x400) // Kilobyte
			{
				suffix = "KB";
				readable = i;
			}
			else
			{
				return i.ToString("0 B"); // Byte
			}
			// Divide by 1024 to get fractional value
			readable = (readable / 1024);
			// Return formatted number with suffix
			return readable.ToString("0.# ") + suffix;
		}
	}
}
