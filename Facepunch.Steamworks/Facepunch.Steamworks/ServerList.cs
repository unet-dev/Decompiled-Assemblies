using Facepunch.Steamworks.Interop;
using SteamNative;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Facepunch.Steamworks
{
	public class ServerList : IDisposable
	{
		internal Client client;

		private HashSet<ulong> FavouriteHash = new HashSet<ulong>();

		private HashSet<ulong> HistoryHash = new HashSet<ulong>();

		internal ServerList(Client client)
		{
			this.client = client;
			this.UpdateFavouriteList();
		}

		public ServerList.Request Custom(IEnumerable<string> serverList)
		{
			ServerList.Request request = new ServerList.Request(this.client)
			{
				ServerList = serverList
			};
			request.StartCustomQuery();
			return request;
		}

		public void Dispose()
		{
			this.client = null;
		}

		public ServerList.Request Favourites(ServerList.Filter filter = null)
		{
			if (filter == null)
			{
				filter = new ServerList.Filter()
				{
					{ "appid", this.client.AppId.ToString() }
				};
			}
			filter.Start();
			ServerList.Request request = new ServerList.Request(this.client)
			{
				Filter = filter
			};
			request.AddRequest(this.client.native.servers.RequestFavoritesServerList(this.client.AppId, filter.NativeArray, (uint)filter.Count, IntPtr.Zero));
			filter.Free();
			return request;
		}

		public ServerList.Request Friends(ServerList.Filter filter = null)
		{
			if (filter == null)
			{
				filter = new ServerList.Filter()
				{
					{ "appid", this.client.AppId.ToString() }
				};
			}
			filter.Start();
			ServerList.Request request = new ServerList.Request(this.client)
			{
				Filter = filter
			};
			request.AddRequest(this.client.native.servers.RequestFriendsServerList(this.client.AppId, filter.NativeArray, (uint)filter.Count, IntPtr.Zero));
			filter.Free();
			return request;
		}

		public ServerList.Request History(ServerList.Filter filter = null)
		{
			if (filter == null)
			{
				filter = new ServerList.Filter()
				{
					{ "appid", this.client.AppId.ToString() }
				};
			}
			filter.Start();
			ServerList.Request request = new ServerList.Request(this.client)
			{
				Filter = filter
			};
			request.AddRequest(this.client.native.servers.RequestHistoryServerList(this.client.AppId, filter.NativeArray, (uint)filter.Count, IntPtr.Zero));
			filter.Free();
			return request;
		}

		public ServerList.Request Internet(ServerList.Filter filter = null)
		{
			if (filter == null)
			{
				filter = new ServerList.Filter()
				{
					{ "appid", this.client.AppId.ToString() }
				};
			}
			filter.Start();
			ServerList.Request request = new ServerList.Request(this.client)
			{
				Filter = filter
			};
			request.AddRequest(this.client.native.servers.RequestInternetServerList(this.client.AppId, filter.NativeArray, (uint)filter.Count, IntPtr.Zero));
			filter.Free();
			return request;
		}

		internal bool IsFavourite(ServerList.Server server)
		{
			ulong num = (ulong)server.Address.IpToInt32();
			num = num << 32 | (ulong)server.ConnectionPort;
			return this.FavouriteHash.Contains(num);
		}

		internal bool IsHistory(ServerList.Server server)
		{
			ulong num = (ulong)server.Address.IpToInt32();
			num = num << 32 | (ulong)server.ConnectionPort;
			return this.HistoryHash.Contains(num);
		}

		public ServerList.Request Local(ServerList.Filter filter = null)
		{
			if (filter == null)
			{
				filter = new ServerList.Filter()
				{
					{ "appid", this.client.AppId.ToString() }
				};
			}
			filter.Start();
			ServerList.Request request = new ServerList.Request(this.client)
			{
				Filter = filter
			};
			request.AddRequest(this.client.native.servers.RequestLANServerList(this.client.AppId, IntPtr.Zero));
			filter.Free();
			return request;
		}

		internal void UpdateFavouriteList()
		{
			uint num;
			ushort num1;
			ushort num2;
			uint num3;
			uint num4;
			this.FavouriteHash.Clear();
			this.HistoryHash.Clear();
			for (int i = 0; i < this.client.native.matchmaking.GetFavoriteGameCount(); i++)
			{
				AppId_t appIdT = 0;
				this.client.native.matchmaking.GetFavoriteGame(i, ref appIdT, out num, out num1, out num2, out num4, out num3);
				ulong num5 = (ulong)num;
				num5 <<= 32;
				num5 |= (ulong)num1;
				if ((num4 & 1) == 1)
				{
					this.FavouriteHash.Add(num5);
				}
				if ((num4 & 1) == 1)
				{
					this.HistoryHash.Add(num5);
				}
			}
		}

		public class Filter : List<KeyValuePair<string, string>>
		{
			internal IntPtr NativeArray;

			private IntPtr m_pArrayEntries;

			private int AppId;

			public Filter()
			{
			}

			public void Add(string k, string v)
			{
				base.Add(new KeyValuePair<string, string>(k, v));
			}

			internal void Free()
			{
				if (this.m_pArrayEntries != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(this.m_pArrayEntries);
				}
				if (this.NativeArray != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(this.NativeArray);
				}
			}

			internal void Start()
			{
				MatchMakingKeyValuePair_t[] array = this.Select<KeyValuePair<string, string>, MatchMakingKeyValuePair_t>((KeyValuePair<string, string> x) => {
					if (x.Key == "appid")
					{
						this.AppId = int.Parse(x.Value);
					}
					return new MatchMakingKeyValuePair_t()
					{
						Key = x.Key,
						Value = x.Value
					};
				}).ToArray<MatchMakingKeyValuePair_t>();
				int num = Marshal.SizeOf(typeof(MatchMakingKeyValuePair_t));
				this.NativeArray = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)) * (int)array.Length);
				this.m_pArrayEntries = Marshal.AllocHGlobal(num * (int)array.Length);
				for (int i = 0; i < (int)array.Length; i++)
				{
					Marshal.StructureToPtr(array[i], new IntPtr(this.m_pArrayEntries.ToInt64() + (long)(i * num)), false);
				}
				Marshal.WriteIntPtr(this.NativeArray, this.m_pArrayEntries);
			}

			internal bool Test(gameserveritem_t info)
			{
				if (this.AppId != 0 && (long)this.AppId != (ulong)info.AppID)
				{
					return false;
				}
				return true;
			}
		}

		private struct MatchPair
		{
			public string key;

			public string @value;
		}

		public class Request : IDisposable
		{
			internal Client client;

			internal List<ServerList.Request.SubRequest> Requests;

			public Action OnUpdate;

			public Action<ServerList.Server> OnServerResponded;

			public Action OnFinished;

			public List<ServerList.Server> Responded;

			public List<ServerList.Server> Unresponsive;

			public bool Finished;

			internal ServerList.Filter Filter
			{
				get;
				set;
			}

			internal IEnumerable<string> ServerList
			{
				get;
				set;
			}

			internal Request(Client c)
			{
				this.client = c;
				this.client.OnUpdate += new Action(this.Update);
			}

			internal void AddRequest(IntPtr id)
			{
				this.Requests.Add(new ServerList.Request.SubRequest()
				{
					Request = id
				});
			}

			public void Dispose()
			{
				if (this.client.IsValid)
				{
					this.client.OnUpdate -= new Action(this.Update);
				}
				foreach (ServerList.Request.SubRequest request in this.Requests)
				{
					if (!this.client.IsValid)
					{
						continue;
					}
					this.client.native.servers.CancelQuery(request.Request);
				}
				this.Requests.Clear();
			}

			~Request()
			{
				this.Dispose();
			}

			private void OnServer(gameserveritem_t info)
			{
				if (!info.HadSuccessfulResponse)
				{
					this.Unresponsive.Add(ServerList.Server.FromSteam(this.client, info));
					return;
				}
				if (this.Filter != null && !this.Filter.Test(info))
				{
					return;
				}
				ServerList.Server server = ServerList.Server.FromSteam(this.client, info);
				this.Responded.Add(server);
				Action<ServerList.Server> onServerResponded = this.OnServerResponded;
				if (onServerResponded == null)
				{
					return;
				}
				onServerResponded(server);
			}

			internal void StartCustomQuery()
			{
				if (this.ServerList == null)
				{
					return;
				}
				int num = 16;
				int num1 = 0;
				while (true)
				{
					IEnumerable<string> strs = this.ServerList.Skip<string>(num1).Take<string>(num);
					if (strs.Count<string>() == 0)
					{
						break;
					}
					num1 += strs.Count<string>();
					ServerList.Filter filter = new ServerList.Filter();
					int num2 = strs.Count<string>();
					filter.Add("or", num2.ToString());
					foreach (string str in strs)
					{
						filter.Add("gameaddr", str);
					}
					filter.Start();
					HServerListRequest hServerListRequest = this.client.native.servers.RequestInternetServerList(this.client.AppId, filter.NativeArray, (uint)filter.Count, IntPtr.Zero);
					filter.Free();
					this.AddRequest(hServerListRequest);
				}
				this.ServerList = null;
			}

			private void Update()
			{
				if (this.Requests.Count == 0)
				{
					return;
				}
				for (int i = 0; i < this.Requests.Count<ServerList.Request.SubRequest>(); i++)
				{
					if (this.Requests[i].Update(this.client.native.servers, new Action<gameserveritem_t>(this.OnServer), this.OnUpdate))
					{
						this.Requests.RemoveAt(i);
						i--;
					}
				}
				if (this.Requests.Count == 0)
				{
					this.Finished = true;
					this.client.OnUpdate -= new Action(this.Update);
					Action onFinished = this.OnFinished;
					if (onFinished == null)
					{
						return;
					}
					onFinished();
				}
			}

			internal class SubRequest
			{
				internal IntPtr Request;

				internal int Pointer;

				internal List<int> WatchList;

				internal Stopwatch Timer;

				public SubRequest()
				{
				}

				internal bool Update(SteamMatchmakingServers servers, Action<gameserveritem_t> OnServer, Action OnUpdate)
				{
					if (this.Request == IntPtr.Zero)
					{
						return true;
					}
					if (this.Timer.Elapsed.TotalSeconds < 0.5)
					{
						return false;
					}
					this.Timer.Reset();
					this.Timer.Start();
					bool flag = false;
					int serverCount = servers.GetServerCount(this.Request);
					if (serverCount != this.Pointer)
					{
						for (int i = this.Pointer; i < serverCount; i++)
						{
							this.WatchList.Add(i);
						}
					}
					this.Pointer = serverCount;
					this.WatchList.RemoveAll((int x) => {
						gameserveritem_t serverDetails = servers.GetServerDetails(this.Request, x);
						if (!serverDetails.HadSuccessfulResponse)
						{
							return false;
						}
						OnServer(serverDetails);
						flag = true;
						return true;
					});
					if (!servers.IsRefreshing(this.Request))
					{
						this.WatchList.RemoveAll((int x) => {
							gameserveritem_t serverDetails = servers.GetServerDetails(this.Request, x);
							OnServer(serverDetails);
							return true;
						});
						servers.CancelQuery(this.Request);
						this.Request = IntPtr.Zero;
						flag = true;
					}
					if (flag && OnUpdate != null)
					{
						OnUpdate();
					}
					return this.Request == IntPtr.Zero;
				}
			}
		}

		public class Server
		{
			internal Client Client;

			public Action<bool> OnReceivedRules;

			public Dictionary<string, string> Rules;

			internal SourceServerQuery RulesRequest;

			internal const uint k_unFavoriteFlagNone = 0;

			internal const uint k_unFavoriteFlagFavorite = 1;

			internal const uint k_unFavoriteFlagHistory = 2;

			public IPAddress Address
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

			public bool Favourite
			{
				get
				{
					return this.Client.ServerList.IsFavourite(this);
				}
			}

			public string GameDir
			{
				get;
				set;
			}

			public bool HasRules
			{
				get
				{
					if (this.Rules == null)
					{
						return false;
					}
					return this.Rules.Count > 0;
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
				get;
				set;
			}

			public int Version
			{
				get;
				set;
			}

			public Server()
			{
			}

			public void AddToFavourites()
			{
				this.Client.native.matchmaking.AddFavoriteGame(this.AppId, this.Address.IpToInt32(), (ushort)this.ConnectionPort, (ushort)this.QueryPort, 1, (uint)Utility.Epoch.Current);
				this.Client.ServerList.UpdateFavouriteList();
			}

			public void AddToHistory()
			{
				this.Client.native.matchmaking.AddFavoriteGame(this.AppId, this.Address.IpToInt32(), (ushort)this.ConnectionPort, (ushort)this.QueryPort, 2, (uint)Utility.Epoch.Current);
				this.Client.ServerList.UpdateFavouriteList();
			}

			public void FetchRules()
			{
				if (this.RulesRequest != null)
				{
					return;
				}
				this.Rules = null;
				this.RulesRequest = new SourceServerQuery(this, this.Address, this.QueryPort);
			}

			internal static ServerList.Server FromSteam(Client client, gameserveritem_t item)
			{
				string[] strArrays;
				ServerList.Server server = new ServerList.Server()
				{
					Client = client,
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
					Version = item.ServerVersion
				};
				ServerList.Server server1 = server;
				if (item.GameTags == null)
				{
					strArrays = null;
				}
				else
				{
					strArrays = item.GameTags.Split(new char[] { ',' });
				}
				server1.Tags = strArrays;
				server.SteamId = item.SteamID;
				return server;
			}

			internal void OnServerRulesReceiveFinished(Dictionary<string, string> rules, bool Success)
			{
				this.RulesRequest = null;
				if (Success)
				{
					this.Rules = rules;
				}
				if (this.OnReceivedRules != null)
				{
					this.OnReceivedRules(Success);
				}
			}

			public void RemoveFromFavourites()
			{
				this.Client.native.matchmaking.RemoveFavoriteGame(this.AppId, this.Address.IpToInt32(), (ushort)this.ConnectionPort, (ushort)this.QueryPort, 1);
				this.Client.ServerList.UpdateFavouriteList();
			}

			public void RemoveFromHistory()
			{
				this.Client.native.matchmaking.RemoveFavoriteGame(this.AppId, this.Address.IpToInt32(), (ushort)this.ConnectionPort, (ushort)this.QueryPort, 2);
				this.Client.ServerList.UpdateFavouriteList();
			}
		}
	}
}