using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Oxide.Core.Logging
{
	public sealed class RotatingFileLogger : ThreadedLogger
	{
		private StreamWriter writer;

		public string Directory
		{
			get;
			set;
		}

		public RotatingFileLogger()
		{
		}

		protected override void BeginBatchProcess()
		{
			this.writer = new StreamWriter(new FileStream(this.GetLogFilename(DateTime.Now), FileMode.Append, FileAccess.Write));
		}

		protected override void FinishBatchProcess()
		{
			this.writer.Close();
			this.writer.Dispose();
			this.writer = null;
		}

		private string GetLogFilename(DateTime date)
		{
			return Path.Combine(this.Directory, string.Format("oxide_{0:yyyy-MM-dd}.txt", date));
		}

		protected override void ProcessMessage(Logger.LogMessage message)
		{
			this.writer.WriteLine(message.LogfileMessage);
		}
	}
}