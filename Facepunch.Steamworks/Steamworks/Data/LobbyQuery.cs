using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	public struct LobbyQuery
	{
		internal LobbyDistanceFilter? distance;

		internal int? slotsAvailable;

		internal int? maxResults;

		private void ApplyFilters()
		{
			if (this.distance.HasValue)
			{
				SteamMatchmaking.Internal.AddRequestLobbyListDistanceFilter(this.distance.Value);
			}
			if (this.slotsAvailable.HasValue)
			{
				SteamMatchmaking.Internal.AddRequestLobbyListFilterSlotsAvailable(this.slotsAvailable.Value);
			}
			if (this.maxResults.HasValue)
			{
				SteamMatchmaking.Internal.AddRequestLobbyListResultCountFilter(this.maxResults.Value);
			}
		}

		public LobbyQuery FilterDistanceClose()
		{
			this.distance = new LobbyDistanceFilter?(LobbyDistanceFilter.Close);
			return this;
		}

		public LobbyQuery FilterDistanceFar()
		{
			this.distance = new LobbyDistanceFilter?(LobbyDistanceFilter.Far);
			return this;
		}

		public LobbyQuery FilterDistanceWorldwide()
		{
			this.distance = new LobbyDistanceFilter?(LobbyDistanceFilter.Worldwide);
			return this;
		}

		public async Task<Lobby[]> RequestAsync()
		{
			Lobby[] lobbyArray;
			bool flag;
			this.ApplyFilters();
			LobbyMatchList_t? nullable = await SteamMatchmaking.Internal.RequestLobbyList();
			LobbyMatchList_t? nullable1 = nullable;
			nullable = null;
			flag = (!nullable1.HasValue ? true : nullable1.Value.LobbiesMatching == 0);
			if (!flag)
			{
				Lobby[] lobbyArray1 = new Lobby[nullable1.Value.LobbiesMatching];
				for (int i = 0; (long)i < (ulong)nullable1.Value.LobbiesMatching; i++)
				{
					Lobby[] lobbyArray2 = lobbyArray1;
					int num = i;
					Lobby lobby = new Lobby()
					{
						Id = SteamMatchmaking.Internal.GetLobbyByIndex(i)
					};
					lobbyArray2[num] = lobby;
				}
				lobbyArray = lobbyArray1;
			}
			else
			{
				lobbyArray = null;
			}
			return lobbyArray;
		}

		public LobbyQuery WithMaxResults(int max)
		{
			this.maxResults = new int?(max);
			return this;
		}

		public LobbyQuery WithSlotsAvailable(int minSlots)
		{
			this.slotsAvailable = new int?(minSlots);
			return this;
		}
	}
}