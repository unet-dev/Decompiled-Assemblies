using Oxide.Core;
using Oxide.Core.Libraries.Covalence;
using ProtoBuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Oxide.Game.Rust.Libraries.Covalence
{
	public class RustPlayerManager : IPlayerManager
	{
		private IDictionary<string, RustPlayerManager.PlayerRecord> playerData;

		private IDictionary<string, RustPlayer> allPlayers;

		private IDictionary<string, RustPlayer> connectedPlayers;

		public IEnumerable<IPlayer> All
		{
			get
			{
				return this.allPlayers.Values.Cast<IPlayer>();
			}
		}

		public IEnumerable<IPlayer> Connected
		{
			get
			{
				return this.connectedPlayers.Values.Cast<IPlayer>();
			}
		}

		public IEnumerable<IPlayer> Sleeping
		{
			get
			{
				return 
					from p in BasePlayer.sleepingPlayerList
					select p.IPlayer;
			}
		}

		public RustPlayerManager()
		{
		}

		public IPlayer FindPlayer(string partialNameOrId)
		{
			IPlayer[] array = this.FindPlayers(partialNameOrId).ToArray<IPlayer>();
			if ((int)array.Length != 1)
			{
				return null;
			}
			return array[0];
		}

		public IPlayer FindPlayerById(string id)
		{
			RustPlayer rustPlayer;
			if (!this.allPlayers.TryGetValue(id, out rustPlayer))
			{
				return null;
			}
			return rustPlayer;
		}

		public IPlayer FindPlayerByObj(object obj)
		{
			return this.connectedPlayers.Values.FirstOrDefault<RustPlayer>((RustPlayer p) => (object)p.Object == (object)obj);
		}

		public IEnumerable<IPlayer> FindPlayers(string partialNameOrId)
		{
			RustPlayerManager rustPlayerManager = null;
			foreach (RustPlayer value in rustPlayerManager.allPlayers.Values)
			{
				if ((value.Name == null || value.Name.IndexOf(partialNameOrId, StringComparison.OrdinalIgnoreCase) < 0) && !(value.Id == partialNameOrId))
				{
					continue;
				}
				yield return value;
			}
		}

		internal void Initialize()
		{
			Utility.DatafileToProto<Dictionary<string, RustPlayerManager.PlayerRecord>>("oxide.covalence", true);
			this.playerData = ProtoStorage.Load<Dictionary<string, RustPlayerManager.PlayerRecord>>(new String[] { "oxide.covalence" }) ?? new Dictionary<string, RustPlayerManager.PlayerRecord>();
			this.allPlayers = new Dictionary<string, RustPlayer>();
			this.connectedPlayers = new Dictionary<string, RustPlayer>();
			foreach (KeyValuePair<string, RustPlayerManager.PlayerRecord> playerDatum in this.playerData)
			{
				this.allPlayers.Add(playerDatum.Key, new RustPlayer(playerDatum.Value.Id, playerDatum.Value.Name));
			}
		}

		internal void PlayerConnected(BasePlayer player)
		{
			this.allPlayers[player.UserIDString] = new RustPlayer(player);
			this.connectedPlayers[player.UserIDString] = new RustPlayer(player);
		}

		internal void PlayerDisconnected(BasePlayer player)
		{
			this.connectedPlayers.Remove(player.UserIDString);
		}

		internal void PlayerJoin(ulong userId, string name)
		{
			RustPlayerManager.PlayerRecord playerRecord;
			string str = userId.ToString();
			if (this.playerData.TryGetValue(str, out playerRecord))
			{
				playerRecord.Name = name;
				this.playerData[str] = playerRecord;
				this.allPlayers.Remove(str);
				this.allPlayers.Add(str, new RustPlayer(userId, name));
				return;
			}
			RustPlayerManager.PlayerRecord playerRecord1 = new RustPlayerManager.PlayerRecord()
			{
				Id = userId,
				Name = name
			};
			playerRecord = playerRecord1;
			this.playerData.Add(str, playerRecord);
			this.allPlayers.Add(str, new RustPlayer(userId, name));
		}

		internal void SavePlayerData()
		{
			ProtoStorage.Save<IDictionary<string, RustPlayerManager.PlayerRecord>>(this.playerData, new String[] { "oxide.covalence" });
		}

		[ProtoContract(ImplicitFields=ImplicitFields.AllFields)]
		private struct PlayerRecord
		{
			public string Name;

			public ulong Id;
		}
	}
}