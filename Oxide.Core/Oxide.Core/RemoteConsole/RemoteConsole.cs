using Oxide.Core;
using Oxide.Core.Configuration;
using Oxide.Core.Libraries.Covalence;
using System;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using WebSocketSharp;
using WebSocketSharp.Net.WebSockets;
using WebSocketSharp.Server;

namespace Oxide.Core.RemoteConsole
{
	public class RemoteConsole
	{
		private readonly Covalence covalence = Interface.Oxide.GetLibrary<Covalence>(null);

		private readonly OxideConfig.OxideRcon config = Interface.Oxide.Config.Rcon;

		private Oxide.Core.RemoteConsole.RemoteConsole.RconListener listener;

		private WebSocketServer server;

		public RemoteConsole()
		{
		}

		public void Initalize()
		{
			int? nullable;
			int? nullable1;
			int? nullable2;
			if (!this.config.Enabled || this.listener != null || this.server != null)
			{
				return;
			}
			if (string.IsNullOrEmpty(this.config.Password))
			{
				Interface.Oxide.LogWarning("[Rcon] Remote console password is not set, disabling", Array.Empty<object>());
				return;
			}
			try
			{
				this.server = new WebSocketServer(this.config.Port)
				{
					WaitTime = TimeSpan.FromSeconds(5),
					ReuseAddress = true
				};
				this.server.AddWebSocketService<Oxide.Core.RemoteConsole.RemoteConsole.RconListener>(string.Concat("/", this.config.Password), () => {
					Oxide.Core.RemoteConsole.RemoteConsole.RconListener rconListener = new Oxide.Core.RemoteConsole.RemoteConsole.RconListener(this);
					Oxide.Core.RemoteConsole.RemoteConsole.RconListener rconListener1 = rconListener;
					this.listener = rconListener;
					return rconListener1;
				});
				this.server.Start();
				Interface.Oxide.LogInfo(string.Format("[Rcon] Server started successfully on port {0}", this.server.Port), Array.Empty<object>());
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				OxideMod oxide = Interface.Oxide;
				WebSocketServer webSocketServer = this.server;
				if (webSocketServer != null)
				{
					nullable1 = new int?(webSocketServer.Port);
				}
				else
				{
					nullable = null;
					nullable1 = nullable;
				}
				oxide.LogException(string.Format("[Rcon] Failed to start server on port {0}", nullable1), exception);
				WebSocketServer webSocketServer1 = this.server;
				if (webSocketServer1 != null)
				{
					nullable2 = new int?(webSocketServer1.Port);
				}
				else
				{
					nullable = null;
					nullable2 = nullable;
				}
				RemoteLogger.Exception(string.Format("Failed to start RCON server on port {0}", nullable2), exception);
			}
		}

		private void OnMessage(MessageEventArgs e, WebSocketContext connection)
		{
			if (this.covalence == null)
			{
				Interface.Oxide.LogError("[Rcon] Failed to process command, Covalence is null", Array.Empty<object>());
				return;
			}
			RemoteMessage message = RemoteMessage.GetMessage(e.Data);
			if (message == null)
			{
				Interface.Oxide.LogError("[Rcon] Failed to process command, RemoteMessage is null", Array.Empty<object>());
				return;
			}
			if (string.IsNullOrEmpty(message.Message))
			{
				Interface.Oxide.LogError("[Rcon] Failed to process command, RemoteMessage.Text is not set", Array.Empty<object>());
				return;
			}
			string[] strArrays = CommandLine.Split(message.Message);
			string lower = strArrays[0].ToLower();
			string[] array = strArrays.Skip<string>(1).ToArray<string>();
			if (Interface.CallHook("OnRconCommand", connection.UserEndPoint, lower, array) != null)
			{
				return;
			}
			this.covalence.Server.Command(lower, array);
		}

		public void SendMessage(RemoteMessage message)
		{
			if (message != null && this.server != null && this.server.IsListening && this.listener != null)
			{
				this.listener.SendMessage(message);
			}
		}

		public void SendMessage(string message, int identifier)
		{
			if (!string.IsNullOrEmpty(message) && this.server != null && this.server.IsListening && this.listener != null)
			{
				this.listener.SendMessage(RemoteMessage.CreateMessage(message, identifier, "Generic", ""));
			}
		}

		public void SendMessage(WebSocketContext connection, string message, int identifier)
		{
			if (!string.IsNullOrEmpty(message) && this.server != null && this.server.IsListening && this.listener != null && connection != null)
			{
				WebSocket webSocket = connection.WebSocket;
				if (webSocket == null)
				{
					return;
				}
				webSocket.Send(RemoteMessage.CreateMessage(message, identifier, "Generic", "").ToJSON());
			}
		}

		public void Shutdown(string reason = "Server shutting down", CloseStatusCode code = 1000)
		{
			if (this.server != null)
			{
				this.server.Stop(code, reason);
				this.server = null;
				this.listener = null;
				Interface.Oxide.LogInfo(string.Format("[Rcon] Service has stopped: {0} ({1})", reason, code), Array.Empty<object>());
			}
		}

		public class RconListener : WebSocketBehavior
		{
			private readonly Oxide.Core.RemoteConsole.RemoteConsole Parent;

			private IPAddress Address;

			public RconListener(Oxide.Core.RemoteConsole.RemoteConsole parent)
			{
				base.IgnoreExtensions = true;
				this.Parent = parent;
			}

			protected override void OnClose(CloseEventArgs e)
			{
				string str = (string.IsNullOrEmpty(e.Reason) ? "Unknown" : e.Reason);
				Interface.Oxide.LogInfo(string.Format("[Rcon] Connection from {0} closed: {1} ({2})", this.Address, str, e.Code), Array.Empty<object>());
			}

			protected override void OnError(ErrorEventArgs e)
			{
				Interface.Oxide.LogException(e.Message, e.Exception);
			}

			protected override void OnMessage(MessageEventArgs e)
			{
				Oxide.Core.RemoteConsole.RemoteConsole parent = this.Parent;
				if (parent == null)
				{
					return;
				}
				parent.OnMessage(e, base.Context);
			}

			protected override void OnOpen()
			{
				this.Address = base.Context.UserEndPoint.Address;
				Interface.Oxide.LogInfo(string.Format("[Rcon] New connection from {0}", this.Address), Array.Empty<object>());
			}

			public void SendMessage(RemoteMessage message)
			{
				base.Sessions.Broadcast(message.ToJSON());
			}
		}

		private struct RconPlayer
		{
			private string Address
			{
				get;
			}

			private int ConnectedSeconds
			{
				get;
			}

			private float CurrentLevel
			{
				get;
			}

			private string DisplayName
			{
				get;
			}

			private float Health
			{
				get;
			}

			private string OwnerSteamID
			{
				get;
			}

			private int Ping
			{
				get;
			}

			private string SteamID
			{
				get;
			}

			private float UnspentXp
			{
				get;
			}

			private float VoiationLevel
			{
				get;
			}

			public RconPlayer(IPlayer player)
			{
				this.SteamID = player.Id;
				this.OwnerSteamID = "0";
				this.DisplayName = player.Name;
				this.Address = player.Address;
				this.Ping = player.Ping;
				this.ConnectedSeconds = 0;
				this.VoiationLevel = 0f;
				this.CurrentLevel = 0f;
				this.UnspentXp = 0f;
				this.Health = player.Health;
			}
		}
	}
}