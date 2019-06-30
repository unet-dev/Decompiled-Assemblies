using Steamworks;
using Steamworks.ServerList;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	public struct ServerInfo : IEquatable<ServerInfo>
	{
		private string[] _tags;

		internal const uint k_unFavoriteFlagNone = 0;

		internal const uint k_unFavoriteFlagFavorite = 1;

		internal const uint k_unFavoriteFlagHistory = 2;

		public IPAddress Address
		{
			get;
			set;
		}

		public uint AddressRaw
		{
			get;
			set;
		}

		public uint AppId
		{
			get;
			set;
		}

		public int BotPlayers
		{
			get;
			set;
		}

		public int ConnectionPort
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public string GameDir
		{
			get;
			set;
		}

		private static ISteamMatchmakingServers Internal
		{
			get
			{
				return Base.Internal;
			}
		}

		public uint LastTimePlayed
		{
			get;
			set;
		}

		public string Map
		{
			get;
			set;
		}

		public int MaxPlayers
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public bool Passworded
		{
			get;
			set;
		}

		public int Ping
		{
			get;
			set;
		}

		public int Players
		{
			get;
			set;
		}

		public int QueryPort
		{
			get;
			set;
		}

		public bool Secure
		{
			get;
			set;
		}

		public ulong SteamId
		{
			get;
			set;
		}

		public string[] Tags
		{
			get
			{
				if (this._tags == null)
				{
					if (!String.IsNullOrEmpty(this.TagString))
					{
						this._tags = this.TagString.Split(new Char[] { ',' });
					}
				}
				return this._tags;
			}
		}

		public string TagString
		{
			get;
			set;
		}

		public int Version
		{
			get;
			set;
		}

		public ServerInfo(uint ip, ushort cport, ushort qport, uint timeplayed)
		{
			this = new ServerInfo()
			{
				AddressRaw = ip,
				Address = Utility.Int32ToIp(ip),
				ConnectionPort = cport,
				QueryPort = qport,
				LastTimePlayed = timeplayed
			};
		}

		public void AddToFavourites()
		{
			SteamMatchmaking.Internal.AddFavoriteGame(SteamClient.AppId, this.AddressRaw, (ushort)this.ConnectionPort, (ushort)this.QueryPort, 1, (uint)Epoch.Current);
		}

		public void AddToHistory()
		{
			SteamMatchmaking.Internal.AddFavoriteGame(SteamClient.AppId, this.AddressRaw, (ushort)this.ConnectionPort, (ushort)this.QueryPort, 2, (uint)Epoch.Current);
		}

		public bool Equals(ServerInfo other)
		{
			return this.GetHashCode() == other.GetHashCode();
		}

		internal static ServerInfo From(gameserveritem_t item)
		{
			ServerInfo serverInfo = new ServerInfo()
			{
				AddressRaw = item.NetAdr.IP,
				Address = Utility.Int32ToIp(item.NetAdr.IP),
				ConnectionPort = item.NetAdr.ConnectionPort,
				QueryPort = item.NetAdr.QueryPort,
				Name = item.ServerName,
				Ping = item.Ping,
				GameDir = item.GameDir,
				Map = item.Map,
				Description = item.GameDescription,
				AppId = item.AppID,
				Players = item.Players,
				MaxPlayers = item.MaxPlayers,
				BotPlayers = item.BotPlayers,
				Passworded = item.Password,
				Secure = item.Secure,
				LastTimePlayed = item.TimeLastPlayed,
				Version = item.ServerVersion,
				TagString = item.GameTags,
				SteamId = item.SteamID
			};
			return serverInfo;
		}

		public override int GetHashCode()
		{
			int hashCode = this.Address.GetHashCode() + this.SteamId.GetHashCode();
			int connectionPort = this.ConnectionPort;
			int num = hashCode + connectionPort.GetHashCode();
			connectionPort = this.QueryPort;
			return num + connectionPort.GetHashCode();
		}

		public async Task<Dictionary<string, string>> QueryRulesAsync()
		{
			return await SourceServerQuery.GetRules(this);
		}

		public void RemoveFromFavourites()
		{
			SteamMatchmaking.Internal.RemoveFavoriteGame(SteamClient.AppId, this.AddressRaw, (ushort)this.ConnectionPort, (ushort)this.QueryPort, 1);
		}

		public void RemoveFromHistory()
		{
			SteamMatchmaking.Internal.RemoveFavoriteGame(SteamClient.AppId, this.AddressRaw, (ushort)this.ConnectionPort, (ushort)this.QueryPort, 2);
		}
	}
}