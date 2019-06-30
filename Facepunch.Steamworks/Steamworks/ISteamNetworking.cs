using Steamworks.Data;
using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	internal class ISteamNetworking : SteamInterface
	{
		private ISteamNetworking.FSendP2PPacket _SendP2PPacket;

		private ISteamNetworking.FIsP2PPacketAvailable _IsP2PPacketAvailable;

		private ISteamNetworking.FReadP2PPacket _ReadP2PPacket;

		private ISteamNetworking.FAcceptP2PSessionWithUser _AcceptP2PSessionWithUser;

		private ISteamNetworking.FCloseP2PSessionWithUser _CloseP2PSessionWithUser;

		private ISteamNetworking.FCloseP2PChannelWithUser _CloseP2PChannelWithUser;

		private ISteamNetworking.FGetP2PSessionState _GetP2PSessionState;

		private ISteamNetworking.FGetP2PSessionState_Windows _GetP2PSessionState_Windows;

		private ISteamNetworking.FAllowP2PPacketRelay _AllowP2PPacketRelay;

		private ISteamNetworking.FCreateListenSocket _CreateListenSocket;

		private ISteamNetworking.FCreateP2PConnectionSocket _CreateP2PConnectionSocket;

		private ISteamNetworking.FCreateConnectionSocket _CreateConnectionSocket;

		private ISteamNetworking.FDestroySocket _DestroySocket;

		private ISteamNetworking.FDestroyListenSocket _DestroyListenSocket;

		private ISteamNetworking.FSendDataOnSocket _SendDataOnSocket;

		private ISteamNetworking.FIsDataAvailableOnSocket _IsDataAvailableOnSocket;

		private ISteamNetworking.FRetrieveDataFromSocket _RetrieveDataFromSocket;

		private ISteamNetworking.FIsDataAvailable _IsDataAvailable;

		private ISteamNetworking.FRetrieveData _RetrieveData;

		private ISteamNetworking.FGetSocketInfo _GetSocketInfo;

		private ISteamNetworking.FGetListenSocketInfo _GetListenSocketInfo;

		private ISteamNetworking.FGetSocketConnectionType _GetSocketConnectionType;

		private ISteamNetworking.FGetMaxPacketSize _GetMaxPacketSize;

		public override string InterfaceName
		{
			get
			{
				return "SteamNetworking005";
			}
		}

		public ISteamNetworking()
		{
		}

		internal bool AcceptP2PSessionWithUser(SteamId steamIDRemote)
		{
			return this._AcceptP2PSessionWithUser(this.Self, steamIDRemote);
		}

		internal bool AllowP2PPacketRelay(bool bAllow)
		{
			return this._AllowP2PPacketRelay(this.Self, bAllow);
		}

		internal bool CloseP2PChannelWithUser(SteamId steamIDRemote, int nChannel)
		{
			return this._CloseP2PChannelWithUser(this.Self, steamIDRemote, nChannel);
		}

		internal bool CloseP2PSessionWithUser(SteamId steamIDRemote)
		{
			return this._CloseP2PSessionWithUser(this.Self, steamIDRemote);
		}

		internal SNetSocket_t CreateConnectionSocket(uint nIP, ushort nPort, int nTimeoutSec)
		{
			return this._CreateConnectionSocket(this.Self, nIP, nPort, nTimeoutSec);
		}

		internal SNetListenSocket_t CreateListenSocket(int nVirtualP2PPort, uint nIP, ushort nPort, bool bAllowUseOfPacketRelay)
		{
			SNetListenSocket_t self = this._CreateListenSocket(this.Self, nVirtualP2PPort, nIP, nPort, bAllowUseOfPacketRelay);
			return self;
		}

		internal SNetSocket_t CreateP2PConnectionSocket(SteamId steamIDTarget, int nVirtualPort, int nTimeoutSec, bool bAllowUseOfPacketRelay)
		{
			SNetSocket_t self = this._CreateP2PConnectionSocket(this.Self, steamIDTarget, nVirtualPort, nTimeoutSec, bAllowUseOfPacketRelay);
			return self;
		}

		internal bool DestroyListenSocket(SNetListenSocket_t hSocket, bool bNotifyRemoteEnd)
		{
			return this._DestroyListenSocket(this.Self, hSocket, bNotifyRemoteEnd);
		}

		internal bool DestroySocket(SNetSocket_t hSocket, bool bNotifyRemoteEnd)
		{
			return this._DestroySocket(this.Self, hSocket, bNotifyRemoteEnd);
		}

		internal bool GetListenSocketInfo(SNetListenSocket_t hListenSocket, ref uint pnIP, ref ushort pnPort)
		{
			return this._GetListenSocketInfo(this.Self, hListenSocket, ref pnIP, ref pnPort);
		}

		internal int GetMaxPacketSize(SNetSocket_t hSocket)
		{
			return this._GetMaxPacketSize(this.Self, hSocket);
		}

		internal bool GetP2PSessionState(SteamId steamIDRemote, ref P2PSessionState_t pConnectionState)
		{
			bool self;
			if (Config.Os != OsType.Windows)
			{
				self = this._GetP2PSessionState(this.Self, steamIDRemote, ref pConnectionState);
			}
			else
			{
				P2PSessionState_t.Pack8 pack8 = pConnectionState;
				bool _GetP2PSessionStateWindows = this._GetP2PSessionState_Windows(this.Self, steamIDRemote, ref pack8);
				pConnectionState = pack8;
				self = _GetP2PSessionStateWindows;
			}
			return self;
		}

		internal SNetSocketConnectionType GetSocketConnectionType(SNetSocket_t hSocket)
		{
			return this._GetSocketConnectionType(this.Self, hSocket);
		}

		internal bool GetSocketInfo(SNetSocket_t hSocket, ref SteamId pSteamIDRemote, ref int peSocketStatus, ref uint punIPRemote, ref ushort punPortRemote)
		{
			bool self = this._GetSocketInfo(this.Self, hSocket, ref pSteamIDRemote, ref peSocketStatus, ref punIPRemote, ref punPortRemote);
			return self;
		}

		public override void InitInternals()
		{
			this._SendP2PPacket = Marshal.GetDelegateForFunctionPointer<ISteamNetworking.FSendP2PPacket>(Marshal.ReadIntPtr(this.VTable, 0));
			this._IsP2PPacketAvailable = Marshal.GetDelegateForFunctionPointer<ISteamNetworking.FIsP2PPacketAvailable>(Marshal.ReadIntPtr(this.VTable, 8));
			this._ReadP2PPacket = Marshal.GetDelegateForFunctionPointer<ISteamNetworking.FReadP2PPacket>(Marshal.ReadIntPtr(this.VTable, 16));
			this._AcceptP2PSessionWithUser = Marshal.GetDelegateForFunctionPointer<ISteamNetworking.FAcceptP2PSessionWithUser>(Marshal.ReadIntPtr(this.VTable, 24));
			this._CloseP2PSessionWithUser = Marshal.GetDelegateForFunctionPointer<ISteamNetworking.FCloseP2PSessionWithUser>(Marshal.ReadIntPtr(this.VTable, 32));
			this._CloseP2PChannelWithUser = Marshal.GetDelegateForFunctionPointer<ISteamNetworking.FCloseP2PChannelWithUser>(Marshal.ReadIntPtr(this.VTable, 40));
			this._GetP2PSessionState = Marshal.GetDelegateForFunctionPointer<ISteamNetworking.FGetP2PSessionState>(Marshal.ReadIntPtr(this.VTable, 48));
			this._GetP2PSessionState_Windows = Marshal.GetDelegateForFunctionPointer<ISteamNetworking.FGetP2PSessionState_Windows>(Marshal.ReadIntPtr(this.VTable, 48));
			this._AllowP2PPacketRelay = Marshal.GetDelegateForFunctionPointer<ISteamNetworking.FAllowP2PPacketRelay>(Marshal.ReadIntPtr(this.VTable, 56));
			this._CreateListenSocket = Marshal.GetDelegateForFunctionPointer<ISteamNetworking.FCreateListenSocket>(Marshal.ReadIntPtr(this.VTable, 64));
			this._CreateP2PConnectionSocket = Marshal.GetDelegateForFunctionPointer<ISteamNetworking.FCreateP2PConnectionSocket>(Marshal.ReadIntPtr(this.VTable, 72));
			this._CreateConnectionSocket = Marshal.GetDelegateForFunctionPointer<ISteamNetworking.FCreateConnectionSocket>(Marshal.ReadIntPtr(this.VTable, 80));
			this._DestroySocket = Marshal.GetDelegateForFunctionPointer<ISteamNetworking.FDestroySocket>(Marshal.ReadIntPtr(this.VTable, 88));
			this._DestroyListenSocket = Marshal.GetDelegateForFunctionPointer<ISteamNetworking.FDestroyListenSocket>(Marshal.ReadIntPtr(this.VTable, 96));
			this._SendDataOnSocket = Marshal.GetDelegateForFunctionPointer<ISteamNetworking.FSendDataOnSocket>(Marshal.ReadIntPtr(this.VTable, 104));
			this._IsDataAvailableOnSocket = Marshal.GetDelegateForFunctionPointer<ISteamNetworking.FIsDataAvailableOnSocket>(Marshal.ReadIntPtr(this.VTable, 112));
			this._RetrieveDataFromSocket = Marshal.GetDelegateForFunctionPointer<ISteamNetworking.FRetrieveDataFromSocket>(Marshal.ReadIntPtr(this.VTable, 120));
			this._IsDataAvailable = Marshal.GetDelegateForFunctionPointer<ISteamNetworking.FIsDataAvailable>(Marshal.ReadIntPtr(this.VTable, 128));
			this._RetrieveData = Marshal.GetDelegateForFunctionPointer<ISteamNetworking.FRetrieveData>(Marshal.ReadIntPtr(this.VTable, 136));
			this._GetSocketInfo = Marshal.GetDelegateForFunctionPointer<ISteamNetworking.FGetSocketInfo>(Marshal.ReadIntPtr(this.VTable, 144));
			this._GetListenSocketInfo = Marshal.GetDelegateForFunctionPointer<ISteamNetworking.FGetListenSocketInfo>(Marshal.ReadIntPtr(this.VTable, 152));
			this._GetSocketConnectionType = Marshal.GetDelegateForFunctionPointer<ISteamNetworking.FGetSocketConnectionType>(Marshal.ReadIntPtr(this.VTable, 160));
			this._GetMaxPacketSize = Marshal.GetDelegateForFunctionPointer<ISteamNetworking.FGetMaxPacketSize>(Marshal.ReadIntPtr(this.VTable, 168));
		}

		internal bool IsDataAvailable(SNetListenSocket_t hListenSocket, ref uint pcubMsgSize, ref SNetSocket_t phSocket)
		{
			return this._IsDataAvailable(this.Self, hListenSocket, ref pcubMsgSize, ref phSocket);
		}

		internal bool IsDataAvailableOnSocket(SNetSocket_t hSocket, ref uint pcubMsgSize)
		{
			return this._IsDataAvailableOnSocket(this.Self, hSocket, ref pcubMsgSize);
		}

		internal bool IsP2PPacketAvailable(ref uint pcubMsgSize, int nChannel)
		{
			return this._IsP2PPacketAvailable(this.Self, ref pcubMsgSize, nChannel);
		}

		internal bool ReadP2PPacket(IntPtr pubDest, uint cubDest, ref uint pcubMsgSize, ref SteamId psteamIDRemote, int nChannel)
		{
			bool self = this._ReadP2PPacket(this.Self, pubDest, cubDest, ref pcubMsgSize, ref psteamIDRemote, nChannel);
			return self;
		}

		internal bool RetrieveData(SNetListenSocket_t hListenSocket, [In][Out] IntPtr[] pubDest, uint cubDest, ref uint pcubMsgSize, ref SNetSocket_t phSocket)
		{
			bool self = this._RetrieveData(this.Self, hListenSocket, pubDest, cubDest, ref pcubMsgSize, ref phSocket);
			return self;
		}

		internal bool RetrieveDataFromSocket(SNetSocket_t hSocket, [In][Out] IntPtr[] pubDest, uint cubDest, ref uint pcubMsgSize)
		{
			bool self = this._RetrieveDataFromSocket(this.Self, hSocket, pubDest, cubDest, ref pcubMsgSize);
			return self;
		}

		internal bool SendDataOnSocket(SNetSocket_t hSocket, [In][Out] IntPtr[] pubData, uint cubData, bool bReliable)
		{
			bool self = this._SendDataOnSocket(this.Self, hSocket, pubData, cubData, bReliable);
			return self;
		}

		internal bool SendP2PPacket(SteamId steamIDRemote, IntPtr pubData, uint cubData, P2PSend eP2PSendType, int nChannel)
		{
			bool self = this._SendP2PPacket(this.Self, steamIDRemote, pubData, cubData, eP2PSendType, nChannel);
			return self;
		}

		internal override void Shutdown()
		{
			base.Shutdown();
			this._SendP2PPacket = null;
			this._IsP2PPacketAvailable = null;
			this._ReadP2PPacket = null;
			this._AcceptP2PSessionWithUser = null;
			this._CloseP2PSessionWithUser = null;
			this._CloseP2PChannelWithUser = null;
			this._GetP2PSessionState = null;
			this._GetP2PSessionState_Windows = null;
			this._AllowP2PPacketRelay = null;
			this._CreateListenSocket = null;
			this._CreateP2PConnectionSocket = null;
			this._CreateConnectionSocket = null;
			this._DestroySocket = null;
			this._DestroyListenSocket = null;
			this._SendDataOnSocket = null;
			this._IsDataAvailableOnSocket = null;
			this._RetrieveDataFromSocket = null;
			this._IsDataAvailable = null;
			this._RetrieveData = null;
			this._GetSocketInfo = null;
			this._GetListenSocketInfo = null;
			this._GetSocketConnectionType = null;
			this._GetMaxPacketSize = null;
		}

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FAcceptP2PSessionWithUser(IntPtr self, SteamId steamIDRemote);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FAllowP2PPacketRelay(IntPtr self, bool bAllow);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FCloseP2PChannelWithUser(IntPtr self, SteamId steamIDRemote, int nChannel);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FCloseP2PSessionWithUser(IntPtr self, SteamId steamIDRemote);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SNetSocket_t FCreateConnectionSocket(IntPtr self, uint nIP, ushort nPort, int nTimeoutSec);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SNetListenSocket_t FCreateListenSocket(IntPtr self, int nVirtualP2PPort, uint nIP, ushort nPort, bool bAllowUseOfPacketRelay);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SNetSocket_t FCreateP2PConnectionSocket(IntPtr self, SteamId steamIDTarget, int nVirtualPort, int nTimeoutSec, bool bAllowUseOfPacketRelay);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FDestroyListenSocket(IntPtr self, SNetListenSocket_t hSocket, bool bNotifyRemoteEnd);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FDestroySocket(IntPtr self, SNetSocket_t hSocket, bool bNotifyRemoteEnd);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetListenSocketInfo(IntPtr self, SNetListenSocket_t hListenSocket, ref uint pnIP, ref ushort pnPort);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetMaxPacketSize(IntPtr self, SNetSocket_t hSocket);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetP2PSessionState(IntPtr self, SteamId steamIDRemote, ref P2PSessionState_t pConnectionState);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetP2PSessionState_Windows(IntPtr self, SteamId steamIDRemote, ref P2PSessionState_t.Pack8 pConnectionState);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SNetSocketConnectionType FGetSocketConnectionType(IntPtr self, SNetSocket_t hSocket);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetSocketInfo(IntPtr self, SNetSocket_t hSocket, ref SteamId pSteamIDRemote, ref int peSocketStatus, ref uint punIPRemote, ref ushort punPortRemote);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FIsDataAvailable(IntPtr self, SNetListenSocket_t hListenSocket, ref uint pcubMsgSize, ref SNetSocket_t phSocket);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FIsDataAvailableOnSocket(IntPtr self, SNetSocket_t hSocket, ref uint pcubMsgSize);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FIsP2PPacketAvailable(IntPtr self, ref uint pcubMsgSize, int nChannel);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FReadP2PPacket(IntPtr self, IntPtr pubDest, uint cubDest, ref uint pcubMsgSize, ref SteamId psteamIDRemote, int nChannel);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FRetrieveData(IntPtr self, SNetListenSocket_t hListenSocket, [In][Out] IntPtr[] pubDest, uint cubDest, ref uint pcubMsgSize, ref SNetSocket_t phSocket);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FRetrieveDataFromSocket(IntPtr self, SNetSocket_t hSocket, [In][Out] IntPtr[] pubDest, uint cubDest, ref uint pcubMsgSize);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSendDataOnSocket(IntPtr self, SNetSocket_t hSocket, [In][Out] IntPtr[] pubData, uint cubData, bool bReliable);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSendP2PPacket(IntPtr self, SteamId steamIDRemote, IntPtr pubData, uint cubData, P2PSend eP2PSendType, int nChannel);
	}
}