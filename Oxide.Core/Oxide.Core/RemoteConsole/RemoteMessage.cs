using Newtonsoft.Json;
using Oxide.Core;
using System;

namespace Oxide.Core.RemoteConsole
{
	[Serializable]
	public class RemoteMessage
	{
		public string Message;

		public int Identifier;

		public string Type;

		public string Stacktrace;

		public RemoteMessage()
		{
		}

		public static RemoteMessage CreateMessage(string message, int identifier = -1, string type = "Generic", string trace = "")
		{
			return new RemoteMessage()
			{
				Message = message,
				Identifier = identifier,
				Type = type,
				Stacktrace = trace
			};
		}

		public static RemoteMessage GetMessage(string text)
		{
			RemoteMessage remoteMessage;
			try
			{
				remoteMessage = JsonConvert.DeserializeObject<RemoteMessage>(text);
			}
			catch (JsonReaderException jsonReaderException)
			{
				Interface.Oxide.LogError("[Rcon] Failed to parse message, incorrect format", Array.Empty<object>());
				remoteMessage = null;
			}
			return remoteMessage;
		}

		internal string ToJSON()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
	}
}