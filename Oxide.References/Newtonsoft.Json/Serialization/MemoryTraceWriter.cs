using Newtonsoft.Json.Shims;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Newtonsoft.Json.Serialization
{
	[Preserve]
	public class MemoryTraceWriter : ITraceWriter
	{
		private readonly Queue<string> _traceMessages;

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

		public MemoryTraceWriter()
		{
			this.LevelFilter = TraceLevel.Verbose;
			this._traceMessages = new Queue<string>();
		}

		public IEnumerable<string> GetTraceMessages()
		{
			return this._traceMessages;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string _traceMessage in this._traceMessages)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.AppendLine();
				}
				stringBuilder.Append(_traceMessage);
			}
			return stringBuilder.ToString();
		}

		public void Trace(TraceLevel level, string message, Exception ex)
		{
			if (this._traceMessages.Count >= 1000)
			{
				this._traceMessages.Dequeue();
			}
			StringBuilder stringBuilder = new StringBuilder();
			DateTime now = DateTime.Now;
			stringBuilder.Append(now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff", CultureInfo.InvariantCulture));
			stringBuilder.Append(" ");
			stringBuilder.Append(level.ToString("g"));
			stringBuilder.Append(" ");
			stringBuilder.Append(message);
			this._traceMessages.Enqueue(stringBuilder.ToString());
		}
	}
}