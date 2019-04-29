using Oxide.Core;
using Oxide.Core.Configuration;
using Oxide.Core.RemoteConsole;
using Oxide.Core.ServerConsole;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Oxide.Core.Logging
{
	public abstract class Logger
	{
		protected Queue<Logger.LogMessage> MessageQueue;

		private bool processImediately;

		protected Logger(bool processImediately)
		{
			this.processImediately = processImediately;
			if (!processImediately)
			{
				this.MessageQueue = new Queue<Logger.LogMessage>();
			}
		}

		protected Logger.LogMessage CreateLogMessage(LogType type, string format, object[] args)
		{
			Logger.LogMessage logMessage = new Logger.LogMessage()
			{
				Type = type
			};
			DateTime now = DateTime.Now;
			logMessage.ConsoleMessage = string.Format("[Oxide] {0} [{1}] {2}", now.ToShortTimeString(), type, format);
			now = DateTime.Now;
			logMessage.LogfileMessage = string.Format("{0} [{1}] {2}", now.ToShortTimeString(), type, format);
			Logger.LogMessage logMessage1 = logMessage;
			if (Interface.Oxide.Config.Console.MinimalistMode)
			{
				logMessage1.ConsoleMessage = format;
			}
			if (args.Length != 0)
			{
				logMessage1.ConsoleMessage = string.Format(logMessage1.ConsoleMessage, args);
				logMessage1.LogfileMessage = string.Format(logMessage1.LogfileMessage, args);
			}
			return logMessage1;
		}

		public virtual void HandleMessage(string message, string stackTrace, LogType logType)
		{
			ConsoleColor consoleColor;
			string str;
			if (message.ToLower().Contains("[chat]"))
			{
				logType = LogType.Chat;
			}
			switch (logType)
			{
				case LogType.Chat:
				{
					consoleColor = ConsoleColor.Green;
					str = "Chat";
					break;
				}
				case LogType.Error:
				{
					consoleColor = ConsoleColor.Red;
					str = "Error";
					break;
				}
				case LogType.Info:
				{
					consoleColor = ConsoleColor.Gray;
					str = "Generic";
					break;
				}
				case LogType.Warning:
				{
					consoleColor = ConsoleColor.Yellow;
					str = "Warning";
					break;
				}
				default:
				{
					goto case LogType.Info;
				}
			}
			Interface.Oxide.ServerConsole.AddMessage(message, consoleColor);
			Interface.Oxide.RemoteConsole.SendMessage(new RemoteMessage()
			{
				Message = message,
				Identifier = -1,
				Type = str,
				Stacktrace = stackTrace
			});
		}

		public virtual void OnRemoved()
		{
		}

		protected virtual void ProcessMessage(Logger.LogMessage message)
		{
		}

		public virtual void Write(LogType type, string format, params object[] args)
		{
			this.Write(this.CreateLogMessage(type, format, args));
		}

		internal virtual void Write(Logger.LogMessage message)
		{
			if (this.processImediately)
			{
				this.ProcessMessage(message);
				return;
			}
			this.MessageQueue.Enqueue(message);
		}

		public virtual void WriteException(string message, Exception ex)
		{
			string str = ExceptionHandler.FormatException(ex);
			if (str != null)
			{
				this.Write(LogType.Error, string.Concat(message, Environment.NewLine, str), Array.Empty<object>());
				return;
			}
			Exception exception = ex;
			while (ex.InnerException != null)
			{
				ex = ex.InnerException;
			}
			if (exception.GetType() != ex.GetType())
			{
				this.Write(LogType.Error, "ExType: {0}", new object[] { exception.GetType().Name });
			}
			this.Write(LogType.Error, string.Concat(new string[] { message, " (", ex.GetType().Name, ": ", ex.Message, ")\n", ex.StackTrace }), Array.Empty<object>());
		}

		public struct LogMessage
		{
			public LogType Type;

			public string ConsoleMessage;

			public string LogfileMessage;
		}
	}
}