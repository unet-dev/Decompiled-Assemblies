using Oxide.Core.Libraries;
using Oxide.Core.Libraries.Covalence;
using System;

namespace Oxide.Game.Rust.Libraries
{
	public class Server : Library
	{
		public Server()
		{
		}

		public void Broadcast(string message, string prefix, ulong userId = 0L, params object[] args)
		{
			if (!string.IsNullOrEmpty(message))
			{
				message = (args.Length != 0 ? string.Format(Formatter.ToUnity(message), args) : Formatter.ToUnity(message));
				string str = (prefix != null ? string.Concat(prefix, ": ", message) : message);
				ConsoleNetwork.BroadcastToAllClients("chat.add", new object[] { userId, str, 1 });
			}
		}

		public void Broadcast(string message, ulong userId = 0L)
		{
			this.Broadcast(message, null, userId, Array.Empty<object>());
		}

		public void Command(string command, params object[] args)
		{
			ConsoleSystem.Run(ConsoleSystem.Option.Server, command, args);
		}
	}
}