using NLog;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ImageBatchUploader
{
    /// <summary>
    /// NLog target for writing to TextBox
    /// </summary>
	internal class TextBoxTarget : TargetWithLayout
	{
        private TextBox target;
        public TextBoxTarget(TextBox target)
		{
            this.target = target;
		}
        /// <summary>
        /// Log message to control.
        /// </summary>
        /// <param name="logEvent">
        /// The logging event.
        /// </param>
        protected override void Write(LogEventInfo logEvent)
        {
            string logMessage = Layout.Render(logEvent);
            target.BeginInvoke(new Action(() => target.AppendText(logMessage)));
        }
    }
}
