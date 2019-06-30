using Steamworks.Data;
using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	internal class ISteamMatchmakingServers : SteamInterface
	{
		private ISteamMatchmakingServers.FRequestInternetServerList _RequestInternetServerList;

		private ISteamMatchmakingServers.FRequestLANServerList _RequestLANServerList;

		private ISteamMatchmakingServers.FRequestFriendsServerList _RequestFriendsServerList;

		private ISteamMatchmakingServers.FRequestFavoritesServerList _RequestFavoritesServerList;

		private ISteamMatchmakingServers.FRequestHistoryServerList _RequestHistoryServerList;

		private ISteamMatchmakingServers.FRequestSpectatorServerList _RequestSpectatorServerList;

		private ISteamMatchmakingServers.FReleaseRequest _ReleaseRequest;

		private ISteamMatchmakingServers.FGetServerDetails _GetServerDetails;

		private ISteamMatchmakingServers.FCancelQuery _CancelQuery;

		private ISteamMatchmakingServers.FRefreshQuery _RefreshQuery;

		private ISteamMatchmakingServers.FIsRefreshing _IsRefreshing;

		private ISteamMatchmakingServers.FGetServerCount _GetServerCount;

		private ISteamMatchmakingServers.FRefreshServer _RefreshServer;

		private ISteamMatchmakingServers.FPingServer _PingServer;

		private ISteamMatchmakingServers.FPlayerDetails _PlayerDetails;

		private ISteamMatchmakingServers.FServerRules _ServerRules;

		private ISteamMatchmakingServers.FCancelServerQuery _CancelServerQuery;

		public override string InterfaceName
		{
			get
			{
				return "SteamMatchMakingServers002";
			}
		}

		public ISteamMatchmakingServers()
		{
		}

		internal void CancelQuery(HServerListRequest hRequest)
		{
			this._CancelQuery(this.Self, hRequest);
		}

		internal void CancelServerQuery(HServerQuery hServerQuery)
		{
			this._CancelServerQuery(this.Self, hServerQuery);
		}

		internal int GetServerCount(HServerListRequest hRequest)
		{
			return this._GetServerCount(this.Self, hRequest);
		}

		internal gameserveritem_t GetServerDetails(HServerListRequest hRequest, int iServer)
		{
			gameserveritem_t gameserveritemT = gameserveritem_t.Fill(this._GetServerDetails(this.Self, hRequest, iServer));
			return gameserveritemT;
		}

		public override void InitInternals()
		{
			this._RequestInternetServerList = Marshal.GetDelegateForFunctionPointer<ISteamMatchmakingServers.FRequestInternetServerList>(Marshal.ReadIntPtr(this.VTable, 0));
			this._RequestLANServerList = Marshal.GetDelegateForFunctionPointer<ISteamMatchmakingServers.FRequestLANServerList>(Marshal.ReadIntPtr(this.VTable, 8));
			this._RequestFriendsServerList = Marshal.GetDelegateForFunctionPointer<ISteamMatchmakingServers.FRequestFriendsServerList>(Marshal.ReadIntPtr(this.VTable, 16));
			this._RequestFavoritesServerList = Marshal.GetDelegateForFunctionPointer<ISteamMatchmakingServers.FRequestFavoritesServerList>(Marshal.ReadIntPtr(this.VTable, 24));
			this._RequestHistoryServerList = Marshal.GetDelegateForFunctionPointer<ISteamMatchmakingServers.FRequestHistoryServerList>(Marshal.ReadIntPtr(this.VTable, 32));
			this._RequestSpectatorServerList = Marshal.GetDelegateForFunctionPointer<ISteamMatchmakingServers.FRequestSpectatorServerList>(Marshal.ReadIntPtr(this.VTable, 40));
			this._ReleaseRequest = Marshal.GetDelegateForFunctionPointer<ISteamMatchmakingServers.FReleaseRequest>(Marshal.ReadIntPtr(this.VTable, 48));
			this._GetServerDetails = Marshal.GetDelegateForFunctionPointer<ISteamMatchmakingServers.FGetServerDetails>(Marshal.ReadIntPtr(this.VTable, 56));
			this._CancelQuery = Marshal.GetDelegateForFunctionPointer<ISteamMatchmakingServers.FCancelQuery>(Marshal.ReadIntPtr(this.VTable, 64));
			this._RefreshQuery = Marshal.GetDelegateForFunctionPointer<ISteamMatchmakingServers.FRefreshQuery>(Marshal.ReadIntPtr(this.VTable, 72));
			this._IsRefreshing = Marshal.GetDelegateForFunctionPointer<ISteamMatchmakingServers.FIsRefreshing>(Marshal.ReadIntPtr(this.VTable, 80));
			this._GetServerCount = Marshal.GetDelegateForFunctionPointer<ISteamMatchmakingServers.FGetServerCount>(Marshal.ReadIntPtr(this.VTable, 88));
			this._RefreshServer = Marshal.GetDelegateForFunctionPointer<ISteamMatchmakingServers.FRefreshServer>(Marshal.ReadIntPtr(this.VTable, 96));
			this._PingServer = Marshal.GetDelegateForFunctionPointer<ISteamMatchmakingServers.FPingServer>(Marshal.ReadIntPtr(this.VTable, 104));
			this._PlayerDetails = Marshal.GetDelegateForFunctionPointer<ISteamMatchmakingServers.FPlayerDetails>(Marshal.ReadIntPtr(this.VTable, 112));
			this._ServerRules = Marshal.GetDelegateForFunctionPointer<ISteamMatchmakingServers.FServerRules>(Marshal.ReadIntPtr(this.VTable, 120));
			this._CancelServerQuery = Marshal.GetDelegateForFunctionPointer<ISteamMatchmakingServers.FCancelServerQuery>(Marshal.ReadIntPtr(this.VTable, 128));
		}

		internal bool IsRefreshing(HServerListRequest hRequest)
		{
			return this._IsRefreshing(this.Self, hRequest);
		}

		internal HServerQuery PingServer(uint unIP, ushort usPort, IntPtr pRequestServersResponse)
		{
			return this._PingServer(this.Self, unIP, usPort, pRequestServersResponse);
		}

		internal HServerQuery PlayerDetails(uint unIP, ushort usPort, IntPtr pRequestServersResponse)
		{
			return this._PlayerDetails(this.Self, unIP, usPort, pRequestServersResponse);
		}

		internal void RefreshQuery(HServerListRequest hRequest)
		{
			this._RefreshQuery(this.Self, hRequest);
		}

		internal void RefreshServer(HServerListRequest hRequest, int iServer)
		{
			this._RefreshServer(this.Self, hRequest, iServer);
		}

		internal void ReleaseRequest(HServerListRequest hServerListRequest)
		{
			this._ReleaseRequest(this.Self, hServerListRequest);
		}

		internal HServerListRequest RequestFavoritesServerList(AppId iApp, [In][Out] ref MatchMakingKeyValuePair_t[] ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
		{
			HServerListRequest self = this._RequestFavoritesServerList(this.Self, iApp, ref ppchFilters, nFilters, pRequestServersResponse);
			return self;
		}

		internal HServerListRequest RequestFriendsServerList(AppId iApp, [In][Out] ref MatchMakingKeyValuePair_t[] ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
		{
			HServerListRequest self = this._RequestFriendsServerList(this.Self, iApp, ref ppchFilters, nFilters, pRequestServersResponse);
			return self;
		}

		internal HServerListRequest RequestHistoryServerList(AppId iApp, [In][Out] ref MatchMakingKeyValuePair_t[] ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
		{
			HServerListRequest self = this._RequestHistoryServerList(this.Self, iApp, ref ppchFilters, nFilters, pRequestServersResponse);
			return self;
		}

		internal HServerListRequest RequestInternetServerList(AppId iApp, [In][Out] ref MatchMakingKeyValuePair_t[] ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
		{
			HServerListRequest self = this._RequestInternetServerList(this.Self, iApp, ref ppchFilters, nFilters, pRequestServersResponse);
			return self;
		}

		internal HServerListRequest RequestLANServerList(AppId iApp, IntPtr pRequestServersResponse)
		{
			return this._RequestLANServerList(this.Self, iApp, pRequestServersResponse);
		}

		internal HServerListRequest RequestSpectatorServerList(AppId iApp, [In][Out] ref MatchMakingKeyValuePair_t[] ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
		{
			HServerListRequest self = this._RequestSpectatorServerList(this.Self, iApp, ref ppchFilters, nFilters, pRequestServersResponse);
			return self;
		}

		internal HServerQuery ServerRules(uint unIP, ushort usPort, IntPtr pRequestServersResponse)
		{
			return this._ServerRules(this.Self, unIP, usPort, pRequestServersResponse);
		}

		internal override void Shutdown()
		{
			base.Shutdown();
			this._RequestInternetServerList = null;
			this._RequestLANServerList = null;
			this._RequestFriendsServerList = null;
			this._RequestFavoritesServerList = null;
			this._RequestHistoryServerList = null;
			this._RequestSpectatorServerList = null;
			this._ReleaseRequest = null;
			this._GetServerDetails = null;
			this._CancelQuery = null;
			this._RefreshQuery = null;
			this._IsRefreshing = null;
			this._GetServerCount = null;
			this._RefreshServer = null;
			this._PingServer = null;
			this._PlayerDetails = null;
			this._ServerRules = null;
			this._CancelServerQuery = null;
		}

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FCancelQuery(IntPtr self, HServerListRequest hRequest);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FCancelServerQuery(IntPtr self, HServerQuery hServerQuery);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetServerCount(IntPtr self, HServerListRequest hRequest);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate IntPtr FGetServerDetails(IntPtr self, HServerListRequest hRequest, int iServer);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FIsRefreshing(IntPtr self, HServerListRequest hRequest);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate HServerQuery FPingServer(IntPtr self, uint unIP, ushort usPort, IntPtr pRequestServersResponse);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate HServerQuery FPlayerDetails(IntPtr self, uint unIP, ushort usPort, IntPtr pRequestServersResponse);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FRefreshQuery(IntPtr self, HServerListRequest hRequest);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FRefreshServer(IntPtr self, HServerListRequest hRequest, int iServer);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FReleaseRequest(IntPtr self, HServerListRequest hServerListRequest);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate HServerListRequest FRequestFavoritesServerList(IntPtr self, AppId iApp, [In][Out] ref MatchMakingKeyValuePair_t[] ppchFilters, uint nFilters, IntPtr pRequestServersResponse);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate HServerListRequest FRequestFriendsServerList(IntPtr self, AppId iApp, [In][Out] ref MatchMakingKeyValuePair_t[] ppchFilters, uint nFilters, IntPtr pRequestServersResponse);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate HServerListRequest FRequestHistoryServerList(IntPtr self, AppId iApp, [In][Out] ref MatchMakingKeyValuePair_t[] ppchFilters, uint nFilters, IntPtr pRequestServersResponse);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate HServerListRequest FRequestInternetServerList(IntPtr self, AppId iApp, [In][Out] ref MatchMakingKeyValuePair_t[] ppchFilters, uint nFilters, IntPtr pRequestServersResponse);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate HServerListRequest FRequestLANServerList(IntPtr self, AppId iApp, IntPtr pRequestServersResponse);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate HServerListRequest FRequestSpectatorServerList(IntPtr self, AppId iApp, [In][Out] ref MatchMakingKeyValuePair_t[] ppchFilters, uint nFilters, IntPtr pRequestServersResponse);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate HServerQuery FServerRules(IntPtr self, uint unIP, ushort usPort, IntPtr pRequestServersResponse);
	}
}