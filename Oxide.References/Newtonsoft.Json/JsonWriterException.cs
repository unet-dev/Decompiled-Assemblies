using Newtonsoft.Json.Shims;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Newtonsoft.Json
{
	[Preserve]
	[Serializable]
	public class JsonWriterException : JsonException
	{
		public string Path
		{
			get;
			private set;
		}

		public JsonWriterException()
		{
		}

		public JsonWriterException(string message) : base(message)
		{
		}

		public JsonWriterException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public JsonWriterException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		internal JsonWriterException(string message, Exception innerException, string path) : base(message, innerException)
		{
			this.Path = path;
		}

		internal static JsonWriterException Create(JsonWriter writer, string message, Exception ex)
		{
			return JsonWriterException.Create(writer.ContainerPath, message, ex);
		}

		internal static JsonWriterException Create(string path, string message, Exception ex)
		{
			message = JsonPosition.FormatMessage(null, path, message);
			return new JsonWriterException(message, ex, path);
		}
	}
}