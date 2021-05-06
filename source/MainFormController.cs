using CsvHelper;
using CsvHelper.Configuration;
using ImageBatchUploader.Api;
using ImageBatchUploader.Api.Imagga;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImageBatchUploader
{
	/// <summary>
	/// Class that represents UX logic and controls batch workflow
	/// </summary>
	public class MainFormController
	{
		private NLog.Logger logger;
		private CancellationTokenSource cancellationTokenSource;
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
		/// Tags for images from current session or restored state.
		/// </summary>
		public List<ImageTag> Tags { get; set; }
		public List<ImageError> Errors { get; private set; }
		/// <summary>
		/// Settings that have been loaded on program start.
		/// </summary>
		public Settings SettingsOriginal { get; private set; }
		/// <summary>
		/// Settings that have been loaded from images directory
		/// </summary>
		public Settings SettingsOverride { get; private set; }
		/// <summary>
		/// Current OR last finished tagging task. Has null value if no tagging has been started so far.
		/// </summary>
		public Task<bool> TaggingTask { get; private set; }
		/// <summary>
		/// Abstraction layer which hides various API implementation/handling details
		/// </summary>
		public ApiAbstractor Api { get; private set; }
		/// <summary>
		/// Flag which is turned ON if we encounter quota exceeded errors during image processing.
		/// </summary>
		public bool QuotaExceeded { get; private set; }
		public MainFormController(NLog.Logger logger)
		{
			this.logger = logger;
			SettingsOriginal = SettingsController.LoadSettings();
			logger.Info("Program started.");
		}

		/// <summary>
		/// Indicates whether tagging has been started and not finished nor cancelled
		/// </summary>
		public bool IsTaggingInProcess
		{
			get
			{
				return TaggingTask != null && !TaggingTask.IsCompleted;
			}
		}

		/// <summary>
		/// Scan selected directory for images and restore state.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public bool SelectDirectory(string path)
		{
			var images = new List<string>();
			ApiAbstractor api = null;
			Settings effective, overrided = null;
			try
			{
				overrided = SettingsController.TryLoadSettingsOveride(path);
				if (overrided != null)
					logger.Debug($"Using overrided settings from '{path}'.");
				effective = SettingsController.Merge(SettingsOriginal, overrided);
			}
			catch (Exception ex)
			{
				logger.Error(ex.Message);
				LastError = "Can't read settings override file. See details in intermediate output.";
				return false;
			}
			try
			{
				api = new ApiAbstractor(logger, effective, path);
			}
			catch (Exception ex)
			{
				logger.Error(ex.Message);
				LastError = "Can't instantiate API abstractor. See details in intermediate output.";
				return false;
			}
			int unrecognized;
			try
			{
				var files = Directory.EnumerateFiles(path);
				var extSet = effective.ImageExtensions.Select(a => a.ToLower()).ToHashSet();
				var unrecognizedFiles = new List<string>();
				foreach (var f in files)
				{
					if (extSet.Contains(Path.GetExtension(f).ToLower()))
						images.Add(f);
					else
						unrecognizedFiles.Add(f);
				}
				var unrecognizedExtensions = unrecognizedFiles
					.Select(a => Path.GetExtension(a).ToLower())
					.GroupBy(a => a)
					.ToDictionary(a => a.Key, g => g.Count())
					.OrderByDescending(a => a.Value)
					.Select(a => $"{a.Key}({a.Value})");
				unrecognized = unrecognizedFiles.Count();
				logger.Info($"Selected folder: {path}. Images extensions: '{string.Join(' ', effective.ImageExtensions)}'. Total files: {files.Count()}, images: {images.Count}. Unrecognized extensions: {string.Join(", ", unrecognizedExtensions)}.");
			}
			catch (Exception ex)
			{
				logger.Error(ex.Message);
				LastError = "Can't open selected folder. See details in intermediate output.";
				return false;
			}
			var tags = new List<ImageTag>();
			try
			{
				var records = api.LoadSavedState();
				//tags
				var imageSet = images.Select(a => Path.GetFileName(a)).ToHashSet();
				// discard state for non-existing files (we don't want state labels to be confusing)
				tags = records.Where(a => imageSet.Contains(a.Filename)).ToList();
				logger.Info($"Loaded saved state file. {records.Count} records, {tags.Count} for existing images, {records.Count() - tags.Count} discarded.");

			}
			catch (Exception ex)
			{
				logger.Error($"Can't open saved state file: ${ex.Message}. Please fix errors or delete saved file.");
				LastError = "Can't read saved state file. See intermediate output for details.";
				return false;
			}
			// apply new state consistently
			ImagesList = images;
			Api = api;
			UnrecognizedFilesCount = unrecognized;
			Tags = tags;
			Errors = new List<ImageError>();
			SettingsOverride = overrided;
			QuotaExceeded = false;
			return true;
		}



		/// <summary>
		/// Creates task fort asynchronous tagging process. 
		/// </summary>
		/// <param name="updateCallback">Delegate to call when another image in batch is processed.</param>
		public void InitializeTaggingTask(Action updateCallback)
		{
			var settings = SettingsController.Merge(SettingsOriginal, SettingsOverride);
			cancellationTokenSource =  new CancellationTokenSource();
			TaggingTask = new Task<bool>(() => BatchProcessingAction(settings, updateCallback, cancellationTokenSource.Token), cancellationTokenSource.Token);
		}

		private bool BatchProcessingAction(Settings settings, Action updateCallback, CancellationToken ct)
		{
			QuotaExceeded = false;
			bool cleanup(bool result)
			{
				cancellationTokenSource = null;
				Api.ReleaseOutputFiles();
				return result;
			}
			// initialize API client
			try
			{
				Api.InstantiateApiClient();
			}
			catch (Exception ex)
			{
				LastError = $"Can't instantiate API client. See intermediate output for details.";
				logger.Error($"ApiClient constructor failed! {ex.Message}");
				return cleanup(false);
			}
			// perform usage quota check
			var processedImages = Tags.Select(a => a.Filename).ToHashSet();
			var imageQueue = ImagesList.Where(a => !processedImages.Contains(Path.GetFileName(a))).ToList();
			try
			{
				var quotaTask = Api.CheckApiQuotas(ct);
				Task.WaitAny(quotaTask);
				if (ct.IsCancellationRequested)
				{
					logger.Debug($"Cancelled while requesting usage statistics.");
					return cleanup(true);
				}
				if (quotaTask.Result.HasValue)
				{
					var quota = quotaTask.Result.Value;
					// helper value to test quota exceeded cornner cases
					var debugQuotaCorrection = 0;
					var requestsLeft = quota.limit - quota.used + debugQuotaCorrection;
					logger.Info($"Imagga API available. Monthly quota used: {quota.used}/{quota.limit}. Tags left: {requestsLeft}, required: {imageQueue.Count}.");
					if (requestsLeft <= 0 && imageQueue.Count > 0)
						throw new Exception($"Monthly API calls quota exceeded. Use another credentials or upgrade API usage restrictions");
					if (requestsLeft < imageQueue.Count)
						logger.Warn("Insufficient API usage quota to process whole batch. Batch will be processed partially.");
				}
			}
			catch (Exception ex)
			{
				LastError = "Can't start batch processing. See intermediate output for details.";
				logger.Error(ex.Message);
				return cleanup(false);
			}
			// lock output files for writing
			try
			{
				Api.LockOutputFiles(Tags);
				// discard old errors and move on
				Errors.Clear();
				// update form after we cleared out errors
				updateCallback();
			}
			catch (Exception ex)
			{
				LastError = $"Could not open output file for writing. See immediate output for details.";
				logger.Error(ex.Message);
				return cleanup(false);
			}
			// main tagging loop
			int taggedSuccessfully = 0;
			int taggedWithErrors = 0;
			foreach (var imagePath in imageQueue)
			{
				var fname = Path.GetFileName(imagePath);
				try
				{
					FileInfo fi = new FileInfo(imagePath);
					logger.Debug($"Tagging '{fname}', file size: {GetBytesReadable(fi.Length)}");
					var tagAsyncTask = Api.TagSingleImage(fname, ct);
					// wait without throwing exception on cancelling 
					Task.WaitAny(tagAsyncTask);
					if (ct.IsCancellationRequested)
						return cleanup(true);
					logger.Info($"Tagged OK: '{fname}', {tagAsyncTask.Result.Count()} tags received.");
					Api.WriteTagToCsv(tagAsyncTask.Result);
					Tags.AddRange(tagAsyncTask.Result);
					taggedSuccessfully += 1;

				}
				catch (Exception ex)
				{
					var error = Api.ExceptionToError(ex, fname);
					// no need to process further
					if (error.QuotaExceeded)
					{
						QuotaExceeded = true;
						break;
					}
					Errors.Add(error);
					logger.Warn($"Tagging error: '{fname}', {error.ErrorDescription}");
					Api.WriteErrorToCsv(error);
					taggedWithErrors += 1;
				}
				updateCallback();
			}
			logger.Info($"Batch {(ct.IsCancellationRequested ? "cancelled" : "finished")}. Images successfully tagged in this session: {taggedSuccessfully}, with errors: {taggedWithErrors}.");
			return cleanup(true);
		}

		public void StopTagging()
		{
			if (cancellationTokenSource != null)
				cancellationTokenSource.Cancel();
		}

		public void UpdateSettings(Settings settings)
		{
			SettingsOriginal = settings;
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
