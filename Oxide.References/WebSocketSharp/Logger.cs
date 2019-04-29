using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace WebSocketSharp
{
	public class Logger
	{
		private volatile string _file;

		private volatile WebSocketSharp.LogLevel _level;

		private Action<LogData, string> _output;

		private object _sync;

		public string File
		{
			get
			{
				return this._file;
			}
			set
			{
				object obj = this._sync;
				Monitor.Enter(obj);
				try
				{
					this._file = value;
					this.Warn(string.Format("The current path to the log file has been changed to {0}.", this._file));
				}
				finally
				{
					Monitor.Exit(obj);
				}
			}
		}

		public WebSocketSharp.LogLevel Level
		{
			get
			{
				return this._level;
			}
			set
			{
				object obj = this._sync;
				Monitor.Enter(obj);
				try
				{
					this._level = value;
					this.Warn(string.Format("The current logging level has been changed to {0}.", (WebSocketSharp.LogLevel)this._level));
				}
				finally
				{
					Monitor.Exit(obj);
				}
			}
		}

		public Action<LogData, string> Output
		{
			get
			{
				return this._output;
			}
			set
			{
				object obj = this._sync;
				Monitor.Enter(obj);
				try
				{
					this._output = value ?? new Action<LogData, string>(Logger.defaultOutput);
					this.Warn("The current output action has been changed.");
				}
				finally
				{
					Monitor.Exit(obj);
				}
			}
		}

		public Logger() : this(WebSocketSharp.LogLevel.Error, null, null)
		{
		}

		public Logger(WebSocketSharp.LogLevel level) : this(level, null, null)
		{
		}

		public Logger(WebSocketSharp.LogLevel level, string file, Action<LogData, string> output)
		{
			this._level = level;
			this._file = file;
			this._output = output ?? new Action<LogData, string>(Logger.defaultOutput);
			this._sync = new object();
		}

		public void Debug(string message)
		{
			if (this._level <= WebSocketSharp.LogLevel.Debug)
			{
				this.output(message, WebSocketSharp.LogLevel.Debug);
			}
		}

		private static void defaultOutput(LogData data, string path)
		{
			string str = data.ToString();
			Console.WriteLine(str);
			if ((path == null ? false : path.Length > 0))
			{
				Logger.writeToFile(str, path);
			}
		}

		public void Error(string message)
		{
			if (this._level <= WebSocketSharp.LogLevel.Error)
			{
				this.output(message, WebSocketSharp.LogLevel.Error);
			}
		}

		public void Fatal(string message)
		{
			this.output(message, WebSocketSharp.LogLevel.Fatal);
		}

		public void Info(string message)
		{
			if (this._level <= WebSocketSharp.LogLevel.Info)
			{
				this.output(message, WebSocketSharp.LogLevel.Info);
			}
		}

		private void output(string message, WebSocketSharp.LogLevel level)
		{
			object obj = this._sync;
			Monitor.Enter(obj);
			try
			{
				if (this._level <= level)
				{
					LogData logDatum = null;
					try
					{
						logDatum = new LogData(level, new StackFrame(2, true), message);
						this._output(logDatum, this._file);
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						logDatum = new LogData(WebSocketSharp.LogLevel.Fatal, new StackFrame(0, true), exception.Message);
						Console.WriteLine(logDatum.ToString());
					}
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}

		public void Trace(string message)
		{
			if (this._level <= WebSocketSharp.LogLevel.Trace)
			{
				this.output(message, WebSocketSharp.LogLevel.Trace);
			}
		}

		public void Warn(string message)
		{
			if (this._level <= WebSocketSharp.LogLevel.Warn)
			{
				this.output(message, WebSocketSharp.LogLevel.Warn);
			}
		}

		private static void writeToFile(string value, string path)
		{
			using (StreamWriter streamWriter = new StreamWriter(path, true))
			{
				using (TextWriter textWriter = TextWriter.Synchronized(streamWriter))
				{
					textWriter.WriteLine(value);
				}
			}
		}
	}
}