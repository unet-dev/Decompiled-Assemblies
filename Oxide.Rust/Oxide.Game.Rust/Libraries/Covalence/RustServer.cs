using ConVar;
using Facepunch;
using Oxide.Core;
using Oxide.Core.Libraries.Covalence;
using Oxide.Game.Rust.Libraries;
using Rust;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;

namespace Oxide.Game.Rust.Libraries.Covalence
{
	public class RustServer : IServer
	{
		internal readonly Oxide.Game.Rust.Libraries.Server Server = new Oxide.Game.Rust.Libraries.Server();

		private static IPAddress address;

		private static IPAddress localAddress;

		public IPAddress Address
		{
			get
			{
				IPAddress any;
				try
				{
					if (RustServer.address == null)
					{
						if (Oxide.Core.Utility.ValidateIPv4(ConVar.Server.ip) && !Oxide.Core.Utility.IsLocalIP(ConVar.Server.ip))
						{
							IPAddress.TryParse(ConVar.Server.ip, out RustServer.address);
							Interface.Oxide.LogInfo(String.Format("IP address from command-line: {0}", RustServer.address), Array.Empty<object>());
						}
						else if (!SteamServer.IsValid || SteamServer.PublicIp == null)
						{
							IPAddress.TryParse((new WebClient()).DownloadString("http://api.ipify.org"), out RustServer.address);
							Interface.Oxide.LogInfo(String.Format("IP address from external API: {0}", RustServer.address), Array.Empty<object>());
						}
						else
						{
							RustServer.address = SteamServer.PublicIp;
							Interface.Oxide.LogInfo(String.Format("IP address from Steam query: {0}", RustServer.address), Array.Empty<object>());
						}
					}
					any = RustServer.address;
				}
				catch (Exception exception)
				{
					RemoteLogger.Exception("Couldn't get server's public IP address", exception);
					any = IPAddress.Any;
				}
				return any;
			}
		}

		public CultureInfo Language
		{
			get
			{
				return CultureInfo.InstalledUICulture;
			}
		}

		public IPAddress LocalAddress
		{
			get
			{
				IPAddress any;
				try
				{
					IPAddress localIP = RustServer.localAddress;
					if (localIP == null)
					{
						localIP = Oxide.Core.Utility.GetLocalIP();
						RustServer.localAddress = localIP;
					}
					any = localIP;
				}
				catch (Exception exception)
				{
					RemoteLogger.Exception("Couldn't get server's local IP address", exception);
					any = IPAddress.Any;
				}
				return any;
			}
		}

		public int MaxPlayers
		{
			get
			{
				return ConVar.Server.maxplayers;
			}
			set
			{
				ConVar.Server.maxplayers = value;
			}
		}

		public string Name
		{
			get
			{
				return ConVar.Server.hostname;
			}
			set
			{
				ConVar.Server.hostname = value;
			}
		}

		public int Players
		{
			get
			{
				return BasePlayer.activePlayerList.Count;
			}
		}

		public ushort Port
		{
			get
			{
				return (ushort)ConVar.Server.port;
			}
		}

		public string Protocol
		{
			get
			{
				return Rust.Protocol.printable;
			}
		}

		public Oxide.Core.Libraries.Covalence.SaveInfo SaveInfo { get; } = Oxide.Core.Libraries.Covalence.SaveInfo.Create(World.SaveFileName);

		public DateTime Time
		{
			get
			{
				return TOD_Sky.Instance.Cycle.DateTime;
			}
			set
			{
				TOD_Sky.Instance.Cycle.DateTime = value;
			}
		}

		public string Version
		{
			get
			{
				return BuildInfo.Current.Build.Number;
			}
		}

		public RustServer()
		{
		}

		public void Ban(string id, string reason, TimeSpan duration = null)
		{
			if (!this.IsBanned(id))
			{
				ServerUsers.Set(UInt64.Parse(id), ServerUsers.UserGroup.Banned, this.Name, reason);
				ServerUsers.Save();
			}
		}

		public TimeSpan BanTimeRemaining(string id)
		{
			if (!this.IsBanned(id))
			{
				return TimeSpan.Zero;
			}
			return TimeSpan.MaxValue;
		}

		public void Broadcast(string message, string prefix, params object[] args)
		{
			this.Server.Broadcast(message, prefix, (ulong)0, args);
		}

		public void Broadcast(string message)
		{
			this.Broadcast(message, null, Array.Empty<object>());
		}

		public void Command(string command, params object[] args)
		{
			this.Server.Command(command, args);
		}

		public bool IsBanned(string id)
		{
			return ServerUsers.Is(UInt64.Parse(id), ServerUsers.UserGroup.Banned);
		}

		public void Save()
		{
			ConVar.Server.save(null);
			File.WriteAllText(String.Concat(ConVar.Server.GetServerFolder("cfg"), "/serverauto.cfg"), ConsoleSystem.SaveToConfigString(true));
			ServerUsers.Save();
		}

		public void Unban(string id)
		{
			if (this.IsBanned(id))
			{
				ServerUsers.Remove(UInt64.Parse(id));
				ServerUsers.Save();
			}
		}
	}
}