using Newtonsoft.Json.Shims;
using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Newtonsoft.Json.Serialization
{
	[Preserve]
	public class DiagnosticsTraceWriter : ITraceWriter
	{
		public TraceLevel LevelFilter
		{
			get
			{
				return JustDecompileGenerated_get_LevelFilter();
			}
			set
			{
				JustDecompileGenerated_set_LevelFilter(value);
			}
		}

		private TraceLevel JustDecompileGenerated_LevelFilter_k__BackingField;

		public TraceLevel JustDecompileGenerated_get_LevelFilter()
		{
			return this.JustDecompileGenerated_LevelFilter_k__BackingField;
		}

		public void JustDecompileGenerated_set_LevelFilter(TraceLevel value)
		{
			this.JustDecompileGenerated_LevelFilter_k__BackingField = value;
		}

		public DiagnosticsTraceWriter()
		{
		}

		private TraceEventType GetTraceEventType(TraceLevel level)
		{
			switch (level)
			{
				case TraceLevel.Error:
				{
					return TraceEventType.Error;
				}
				case TraceLevel.Warning:
				{
					return TraceEventType.Warning;
				}
				case TraceLevel.Info:
				{
					return TraceEventType.Information;
				}
				case TraceLevel.Verbose:
				{
					return TraceEventType.Verbose;
				}
			}
			throw new ArgumentOutOfRangeException("level");
		}

		public void Trace(TraceLevel level, string message, Exception ex)
		{
			if (level == TraceLevel.Off)
			{
				return;
			}
			TraceEventCache traceEventCache = new TraceEventCache();
			TraceEventType traceEventType = this.GetTraceEventType(level);
			foreach (TraceListener listener in Trace.Listeners)
			{
				if (listener.IsThreadSafe)
				{
					listener.TraceEvent(traceEventCache, "Newtonsoft.Json", traceEventType, 0, message);
				}
				else
				{
					TraceListener traceListener = listener;
					Monitor.Enter(traceListener);
					try
					{
						listener.TraceEvent(traceEventCache, "Newtonsoft.Json", traceEventType, 0, message);
					}
					finally
					{
						Monitor.Exit(traceListener);
					}
				}
				if (!Trace.AutoFlush)
				{
					continue;
				}
				listener.Flush();
			}
		}
	}
}