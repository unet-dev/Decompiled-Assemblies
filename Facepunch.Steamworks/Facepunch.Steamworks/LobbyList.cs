using Facepunch.Steamworks.Interop;
using SteamNative;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Facepunch.Steamworks
{
	public class LobbyList : IDisposable
	{
		internal Client client;

		internal List<ulong> requests;

		public Action OnLobbiesUpdated;

		public bool Finished
		{
			get;
			private set;
		}

		public List<LobbyList.Lobby> Lobbies
		{
			get;
			private set;
		}

		internal LobbyList(Client client)
		{
			this.client = client;
			this.Lobbies = new List<LobbyList.Lobby>();
			this.requests = new List<ulong>();
		}

		private void checkFinished()
		{
			if (this.Lobbies.Count == this.requests.Count)
			{
				this.Finished = true;
				return;
			}
			this.Finished = false;
		}

		public void Dispose()
		{
			this.client = null;
		}

		private void OnLobbyDataUpdated(LobbyDataUpdate_t callback)
		{
			if (callback.Success == 1)
			{
				LobbyList.Lobby lobby = this.Lobbies.Find((LobbyList.Lobby x) => x.LobbyID == callback.SteamIDLobby);
				if (lobby == null)
				{
					this.Lobbies.Add(lobby);
					this.checkFinished();
				}
				if (this.OnLobbiesUpdated != null)
				{
					this.OnLobbiesUpdated();
				}
			}
		}

		private void OnLobbyList(LobbyMatchList_t callback, bool error)
		{
			if (error)
			{
				return;
			}
			uint lobbiesMatching = callback.LobbiesMatching;
			for (int i = 0; (long)i < (ulong)lobbiesMatching; i++)
			{
				ulong lobbyByIndex = this.client.native.matchmaking.GetLobbyByIndex(i);
				this.requests.Add(lobbyByIndex);
				LobbyList.Lobby lobby = LobbyList.Lobby.FromSteam(this.client, lobbyByIndex);
				if (lobby.Name == "")
				{
					this.client.native.matchmaking.RequestLobbyData(lobbyByIndex);
					this.client.RegisterCallback<LobbyDataUpdate_t>(new Action<LobbyDataUpdate_t>(this.OnLobbyDataUpdated));
				}
				else
				{
					this.Lobbies.Add(lobby);
					this.checkFinished();
				}
			}
			this.checkFinished();
			if (this.OnLobbiesUpdated != null)
			{
				this.OnLobbiesUpdated();
			}
		}

		public void Refresh(LobbyList.Filter filter = null)
		{
			this.Lobbies.Clear();
			this.requests.Clear();
			this.Finished = false;
			if (filter == null)
			{
				filter = new LobbyList.Filter();
				filter.StringFilters.Add("appid", this.client.AppId.ToString());
				this.client.native.matchmaking.RequestLobbyList(new Action<LobbyMatchList_t, bool>(this.OnLobbyList));
				return;
			}
			this.client.native.matchmaking.AddRequestLobbyListDistanceFilter((LobbyDistanceFilter)filter.DistanceFilter);
			if (filter.SlotsAvailable.HasValue)
			{
				this.client.native.matchmaking.AddRequestLobbyListFilterSlotsAvailable(filter.SlotsAvailable.Value);
			}
			if (filter.MaxResults.HasValue)
			{
				this.client.native.matchmaking.AddRequestLobbyListResultCountFilter(filter.MaxResults.Value);
			}
			foreach (KeyValuePair<string, string> stringFilter in filter.StringFilters)
			{
				this.client.native.matchmaking.AddRequestLobbyListStringFilter(stringFilter.Key, stringFilter.Value, LobbyComparison.Equal);
			}
			foreach (KeyValuePair<string, int> nearFilter in filter.NearFilters)
			{
				this.client.native.matchmaking.AddRequestLobbyListNearValueFilter(nearFilter.Key, nearFilter.Value);
			}
			this.client.native.matchmaking.RequestLobbyList(new Action<LobbyMatchList_t, bool>(this.OnLobbyList));
		}

		public class Filter
		{
			public Dictionary<string, string> StringFilters;

			public Dictionary<string, int> NearFilters;

			public LobbyList.Filter.Distance DistanceFilter;

			public int? MaxResults
			{
				get;
				set;
			}

			public int? SlotsAvailable
			{
				get;
				set;
			}

			public Filter()
			{
			}

			public enum Comparison
			{
				EqualToOrLessThan = -2,
				LessThan = -1,
				Equal = 0,
				GreaterThan = 1,
				EqualToOrGreaterThan = 2,
				NotEqual = 3
			}

			public enum Distance
			{
				Close,
				Default,
				Far,
				Worldwide
			}
		}

		public class Lobby
		{
			private Dictionary<string, string> lobbyData;

			internal Client Client;

			public ulong LobbyID
			{
				get;
				private set;
			}

			public string LobbyType
			{
				get;
				private set;
			}

			public int MemberLimit
			{
				get;
				private set;
			}

			public string Name
			{
				get;
				private set;
			}

			public int NumMembers
			{
				get;
				private set;
			}

			public ulong Owner
			{
				get;
				private set;
			}

			public Lobby()
			{
			}

			internal static LobbyList.Lobby FromSteam(Client client, ulong lobby)
			{
				string str;
				string str1;
				Dictionary<string, string> strs = new Dictionary<string, string>();
				int lobbyDataCount = client.native.matchmaking.GetLobbyDataCount(lobby);
				for (int i = 0; i < lobbyDataCount; i++)
				{
					if (client.native.matchmaking.GetLobbyDataByIndex(lobby, i, out str, out str1))
					{
						strs.Add(str, str1);
					}
				}
				return new LobbyList.Lobby()
				{
					Client = client,
					LobbyID = lobby,
					Name = client.native.matchmaking.GetLobbyData(lobby, "name"),
					LobbyType = client.native.matchmaking.GetLobbyData(lobby, "lobbytype"),
					MemberLimit = client.native.matchmaking.GetLobbyMemberLimit(lobby),
					Owner = client.native.matchmaking.GetLobbyOwner(lobby),
					NumMembers = client.native.matchmaking.GetNumLobbyMembers(lobby),
					lobbyData = strs
				};
			}

			public Dictionary<string, string> GetAllData()
			{
				Dictionary<string, string> strs = new Dictionary<string, string>();
				foreach (KeyValuePair<string, string> lobbyDatum in this.lobbyData)
				{
					strs.Add(lobbyDatum.Key, lobbyDatum.Value);
				}
				return strs;
			}

			public string GetData(string k)
			{
				string str;
				if (this.lobbyData.TryGetValue(k, out str))
				{
					return str;
				}
				return string.Empty;
			}
		}
	}
}