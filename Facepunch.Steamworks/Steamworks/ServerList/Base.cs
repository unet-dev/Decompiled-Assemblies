using Steamworks;
using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Steamworks.ServerList
{
	public abstract class Base : IDisposable
	{
		private static ISteamMatchmakingServers _internal;

		public List<ServerInfo> Responsive = new List<ServerInfo>();

		public List<ServerInfo> Unresponsive = new List<ServerInfo>();

		internal HServerListRequest request;

		internal List<MatchMakingKeyValuePair_t> filters = new List<MatchMakingKeyValuePair_t>();

		internal List<int> watchList = new List<int>();

		internal int LastCount = 0;

		public Steamworks.AppId AppId
		{
			get;
			set;
		}

		internal int Count
		{
			get
			{
				return Base.Internal.GetServerCount(this.request);
			}
		}

		internal static ISteamMatchmakingServers Internal
		{
			get
			{
				if (Base._internal == null)
				{
					Base._internal = new ISteamMatchmakingServers();
					Base._internal.Init();
				}
				return Base._internal;
			}
		}

		internal bool IsRefreshing
		{
			get
			{
				return (this.request.Value == IntPtr.Zero ? false : Base.Internal.IsRefreshing(this.request));
			}
		}

		public Base()
		{
			this.AppId = SteamClient.AppId;
		}

		public void AddFilter(string key, string value)
		{
			List<MatchMakingKeyValuePair_t> matchMakingKeyValuePairTs = this.filters;
			MatchMakingKeyValuePair_t matchMakingKeyValuePairT = new MatchMakingKeyValuePair_t()
			{
				Key = key,
				Value = value
			};
			matchMakingKeyValuePairTs.Add(matchMakingKeyValuePairT);
		}

		public virtual void Cancel()
		{
			Base.Internal.CancelQuery(this.request);
		}

		public void Dispose()
		{
			this.ReleaseQuery();
		}

		internal virtual MatchMakingKeyValuePair_t[] GetFilters()
		{
			return this.filters.ToArray();
		}

		internal void InvokeChanges()
		{
			Action action = this.OnChanges;
			if (action != null)
			{
				action();
			}
			else
			{
			}
		}

		internal abstract void LaunchQuery();

		private void MovePendingToUnresponsive()
		{
			this.watchList.RemoveAll((int x) => {
				gameserveritem_t serverDetails = Base.Internal.GetServerDetails(this.request, x);
				this.OnServer(ServerInfo.From(serverDetails), serverDetails.HadSuccessfulResponse);
				return true;
			});
		}

		private void OnServer(ServerInfo serverInfo, bool responded)
		{
			if (!responded)
			{
				this.Unresponsive.Add(serverInfo);
			}
			else
			{
				this.Responsive.Add(serverInfo);
				Action<ServerInfo> action = this.OnResponsiveServer;
				if (action != null)
				{
					action(serverInfo);
				}
				else
				{
				}
			}
		}

		private void ReleaseQuery()
		{
			if (this.request.Value != IntPtr.Zero)
			{
				this.Cancel();
				Base.Internal.ReleaseRequest(this.request);
				this.request = IntPtr.Zero;
			}
		}

		private void Reset()
		{
			this.ReleaseQuery();
			this.LastCount = 0;
			this.watchList.Clear();
		}

		public virtual async Task<bool> RunQueryAsync(float timeoutSeconds = 10f)
		{
			bool flag;
			bool flag1;
			Stopwatch stopwatch = Stopwatch.StartNew();
			this.Reset();
			this.LaunchQuery();
			HServerListRequest hServerListRequest = this.request;
			while (this.IsRefreshing)
			{
				await Task.Delay(33);
				flag1 = (this.request.Value == IntPtr.Zero ? true : hServerListRequest.Value != this.request.Value);
				if (flag1)
				{
					flag = false;
					return flag;
				}
				else if (SteamClient.IsValid)
				{
					int count = this.Responsive.Count;
					this.UpdatePending();
					this.UpdateResponsive();
					if (count != this.Responsive.Count)
					{
						this.InvokeChanges();
					}
					if (stopwatch.Elapsed.TotalSeconds > (double)timeoutSeconds)
					{
						break;
					}
				}
				else
				{
					flag = false;
					return flag;
				}
			}
			this.MovePendingToUnresponsive();
			this.InvokeChanges();
			flag = true;
			return flag;
		}

		internal static void Shutdown()
		{
			Base._internal = null;
		}

		private void UpdatePending()
		{
			int count = this.Count;
			if (count != this.LastCount)
			{
				for (int i = this.LastCount; i < count; i++)
				{
					this.watchList.Add(i);
				}
				this.LastCount = count;
			}
		}

		public void UpdateResponsive()
		{
			this.watchList.RemoveAll((int x) => {
				bool flag;
				gameserveritem_t serverDetails = Base.Internal.GetServerDetails(this.request, x);
				if (!serverDetails.HadSuccessfulResponse)
				{
					flag = false;
				}
				else
				{
					this.OnServer(ServerInfo.From(serverDetails), serverDetails.HadSuccessfulResponse);
					flag = true;
				}
				return flag;
			});
		}

		public event Action OnChanges;

		public event Action<ServerInfo> OnResponsiveServer;
	}
}