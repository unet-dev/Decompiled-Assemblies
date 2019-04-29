using System;
using System.Collections.Generic;

namespace Oxide.Core.Logging
{
	public sealed class CompoundLogger : Logger
	{
		private readonly HashSet<Logger> subloggers;

		private readonly List<Logger.LogMessage> messagecache;

		private bool usecache;

		public CompoundLogger() : base(true)
		{
			this.subloggers = new HashSet<Logger>();
			this.messagecache = new List<Logger.LogMessage>();
			this.usecache = true;
		}

		public void AddLogger(Logger logger)
		{
			this.subloggers.Add(logger);
			foreach (Logger.LogMessage logMessage in this.messagecache)
			{
				logger.Write(logMessage);
			}
		}

		public void DisableCache()
		{
			this.usecache = false;
			this.messagecache.Clear();
		}

		public void RemoveLogger(Logger logger)
		{
			logger.OnRemoved();
			this.subloggers.Remove(logger);
		}

		public void Shutdown()
		{
			foreach (Logger sublogger in this.subloggers)
			{
				sublogger.OnRemoved();
			}
			this.subloggers.Clear();
		}

		public override void Write(LogType type, string format, params object[] args)
		{
			foreach (Logger sublogger in this.subloggers)
			{
				sublogger.Write(type, format, args);
			}
			if (this.usecache)
			{
				this.messagecache.Add(base.CreateLogMessage(type, format, args));
			}
		}
	}
}