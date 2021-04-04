using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImaggaBatchUploader
{
	static class Program
	{
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			var config = new NLog.Config.LoggingConfiguration();
			// Targets where to log to: File and Console
			var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
			// Rules for mapping loggers to targets            
			config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, logconsole);
			// Apply config           
			NLog.LogManager.Configuration = config;

			Application.SetHighDpiMode(HighDpiMode.SystemAware);
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
	}
}
