using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ImaggaBatchUploader
{
	/// <summary>
	/// Class that represents UX logic and controls batch workflow
	/// </summary>
	public class MainFormController
	{
		private NLog.Logger logger;
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
		public MainFormController(NLog.Logger logger)
		{
			this.logger = logger;
			logger.Info("Program started.");
		}

		public bool SelectDirectory(string path)
		{
			List<string> images = null;
			int unrecognized;
			try
			{
				var files = Directory.EnumerateFiles(path);
				var allowedExtensions = new[] { ".jpg", ".jpeg" }.ToHashSet();
				images = files.Where(a => allowedExtensions.Contains(Path.GetExtension(a))).ToList();
				var fc = files.Count();
				unrecognized =  fc - images.Count;
				logger.Info("Selected folder: {0}. Total files: {1}, images: {2}", path, fc, images.Count);
			}
			catch (Exception ex)
			{
				logger.Error(ex.Message);
				LastError = "Can't open selected folder. See details in intermediate output.";
				return false;
			}
			// apply new state consistently
			ImagesList = images;
			SelectedDirectory = path;
			UnrecognizedFilesCount = unrecognized;
			return true;
		}
	}
}
