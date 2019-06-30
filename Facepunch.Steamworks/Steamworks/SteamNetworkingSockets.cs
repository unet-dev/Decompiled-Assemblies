using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Steamworks
{
	public static class SteamNetworkingSockets
	{
		private static ISteamNetworkingSockets _internal;

		private static Dictionary<uint, SocketInterface> SocketInterfaces;

		private static Dictionary<uint, ConnectionInterface> ConnectionInterfaces;

		internal static ISteamNetworkingSockets Internal
		{
			get
			{
				if (SteamNetworkingSockets._internal == null)
				{
					SteamNetworkingSockets._internal = new ISteamNetworkingSockets();
					SteamNetworkingSockets._internal.Init();
					SteamNetworkingSockets.SocketInterfaces = new Dictionary<uint, SocketInterface>();
					SteamNetworkingSockets.ConnectionInterfaces = new Dictionary<uint, ConnectionInterface>();
				}
				return SteamNetworkingSockets._internal;
			}
		}

		private static void ConnectionStatusChanged(SteamNetConnectionStatusChangedCallback_t data)
		{
			if (data.Nfo.listenSocket.Id == 0)
			{
				ConnectionInterface connectionInterface = SteamNetworkingSockets.GetConnectionInterface(data.Conn.Id);
				if (connectionInterface != null)
				{
					connectionInterface.OnConnectionChanged(data.Nfo);
				}
			}
			else
			{
				SocketInterface socketInterface = SteamNetworkingSockets.GetSocketInterface(data.Nfo.listenSocket.Id);
				if (socketInterface != null)
				{
					socketInterface.OnConnectionChanged(data.Conn, data.Nfo);
				}
			}
			Action<Connection, ConnectionInfo> action = SteamNetworkingSockets.OnConnectionStatusChanged;
			if (action != null)
			{
				action(data.Conn, data.Nfo);
			}
			else
			{
			}
		}

		public static T ConnectNormal<T>(NetAddress address)
		where T : ConnectionInterface, new()
		{
			T t = Activator.CreateInstance<T>();
			t.Connection = SteamNetworkingSockets.Internal.ConnectByIPAddress(ref address);
			SteamNetworkingSockets.SetConnectionInterface(t.Connection.Id, t);
			return t;
		}

		public static T ConnectRelay<T>(SteamId serverId, int virtualport = 0)
		where T : ConnectionInterface, new()
		{
			T t = Activator.CreateInstance<T>();
			NetIdentity netIdentity = serverId;
			t.Connection = SteamNetworkingSockets.Internal.ConnectP2P(ref netIdentity, virtualport);
			SteamNetworkingSockets.SetConnectionInterface(t.Connection.Id, t);
			return t;
		}

		public static T CreateNormalSocket<T>(NetAddress address)
		where T : SocketInterface, new()
		{
			T t = Activator.CreateInstance<T>();
			t.Socket = SteamNetworkingSockets.Internal.CreateListenSocketIP(ref address);
			SteamNetworkingSockets.SetSocketInterface(t.Socket.Id, t);
			return t;
		}

		public static T CreateRelaySocket<T>(int virtualport = 0)
		where T : SocketInterface, new()
		{
			T t = Activator.CreateInstance<T>();
			t.Socket = SteamNetworkingSockets.Internal.CreateListenSocketP2P(virtualport);
			SteamNetworkingSockets.SetSocketInterface(t.Socket.Id, t);
			return t;
		}

		internal static ConnectionInterface GetConnectionInterface(uint id)
		{
			ConnectionInterface connectionInterface;
			ConnectionInterface connectionInterface1;
			if (SteamNetworkingSockets.ConnectionInterfaces == null)
			{
				connectionInterface1 = null;
			}
			else if (id == 0)
			{
				connectionInterface1 = null;
			}
			else if (!SteamNetworkingSockets.ConnectionInterfaces.TryGetValue(id, out connectionInterface))
			{
				connectionInterface1 = null;
			}
			else
			{
				connectionInterface1 = connectionInterface;
			}
			return connectionInterface1;
		}

		internal static SocketInterface GetSocketInterface(uint id)
		{
			SocketInterface socketInterface;
			SocketInterface socketInterface1;
			if (SteamNetworkingSockets.SocketInterfaces != null)
			{
				if (id == 0)
				{
					throw new ArgumentException("Invalid Socket");
				}
				if (!SteamNetworkingSockets.SocketInterfaces.TryGetValue(id, out socketInterface))
				{
					socketInterface1 = null;
				}
				else
				{
					socketInterface1 = socketInterface;
				}
			}
			else
			{
				socketInterface1 = null;
			}
			return socketInterface1;
		}

		internal static void InstallEvents()
		{
			SteamNetConnectionStatusChangedCallback_t.Install((SteamNetConnectionStatusChangedCallback_t x) => SteamNetworkingSockets.ConnectionStatusChanged(x), false);
		}

		internal static void SetConnectionInterface(uint id, ConnectionInterface iface)
		{
			if (id == 0)
			{
				throw new ArgumentException("Invalid Connection");
			}
			SteamNetworkingSockets.ConnectionInterfaces[id] = iface;
		}

		internal static void SetSocketInterface(uint id, SocketInterface iface)
		{
			if (id == 0)
			{
				throw new ArgumentException("Invalid Socket");
			}
			Console.WriteLine(String.Format("Installing Socket For {0}", id));
			SteamNetworkingSockets.SocketInterfaces[id] = iface;
		}

		internal static void Shutdown()
		{
			SteamNetworkingSockets._internal = null;
			SteamNetworkingSockets.SocketInterfaces = null;
			SteamNetworkingSockets.ConnectionInterfaces = null;
		}

		public static event Action<Connection, ConnectionInfo> OnConnectionStatusChanged;
	}
}