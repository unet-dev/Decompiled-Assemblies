using Newtonsoft.Json.Shims;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Newtonsoft.Json
{
	[Preserve]
	[Serializable]
	public class JsonReaderException : JsonException
	{
		public int LineNumber
		{
			get;
			private set;
		}

		public int LinePosition
		{
			get;
			private set;
		}

		public string Path
		{
			get;
			private set;
		}

		public JsonReaderException()
		{
		}

		public JsonReaderException(string message) : base(message)
		{
		}

		public JsonReaderException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public JsonReaderException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		internal JsonReaderException(string message, Exception innerException, string path, int lineNumber, int linePosition) : base(message, innerException)
		{
			this.Path = path;
			this.LineNumber = lineNumber;
			this.LinePosition = linePosition;
		}

		internal static JsonReaderException Create(JsonReader reader, string message)
		{
			return JsonReaderException.Create(reader, message, null);
		}

		internal static JsonReaderException Create(JsonReader reader, string message, Exception ex)
		{
			return JsonReaderException.Create(reader as IJsonLineInfo, reader.Path, message, ex);
		}

		internal static JsonReaderException Create(IJsonLineInfo lineInfo, string path, string message, Exception ex)
		{
			int lineNumber;
			int linePosition;
			message = JsonPosition.FormatMessage(lineInfo, path, message);
			if (lineInfo == null || !lineInfo.HasLineInfo())
			{
				lineNumber = 0;
				linePosition = 0;
			}
			else
			{
				lineNumber = lineInfo.LineNumber;
				linePosition = lineInfo.LinePosition;
			}
			return new JsonReaderException(message, ex, path, lineNumber, linePosition);
		}
	}
}