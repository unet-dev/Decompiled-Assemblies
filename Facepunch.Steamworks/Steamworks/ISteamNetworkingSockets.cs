using Steamworks.Data;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Steamworks
{
	internal class ISteamNetworkingSockets : SteamInterface
	{
		private ISteamNetworkingSockets.FCreateListenSocketIP _CreateListenSocketIP;

		private ISteamNetworkingSockets.FConnectByIPAddress _ConnectByIPAddress;

		private ISteamNetworkingSockets.FCreateListenSocketP2P _CreateListenSocketP2P;

		private ISteamNetworkingSockets.FConnectP2P _ConnectP2P;

		private ISteamNetworkingSockets.FAcceptConnection _AcceptConnection;

		private ISteamNetworkingSockets.FCloseConnection _CloseConnection;

		private ISteamNetworkingSockets.FCloseListenSocket _CloseListenSocket;

		private ISteamNetworkingSockets.FSetConnectionUserData _SetConnectionUserData;

		private ISteamNetworkingSockets.FGetConnectionUserData _GetConnectionUserData;

		private ISteamNetworkingSockets.FSetConnectionName _SetConnectionName;

		private ISteamNetworkingSockets.FGetConnectionName _GetConnectionName;

		private ISteamNetworkingSockets.FSendMessageToConnection _SendMessageToConnection;

		private ISteamNetworkingSockets.FFlushMessagesOnConnection _FlushMessagesOnConnection;

		private ISteamNetworkingSockets.FReceiveMessagesOnConnection _ReceiveMessagesOnConnection;

		private ISteamNetworkingSockets.FReceiveMessagesOnListenSocket _ReceiveMessagesOnListenSocket;

		private ISteamNetworkingSockets.FGetConnectionInfo _GetConnectionInfo;

		private ISteamNetworkingSockets.FGetQuickConnectionStatus _GetQuickConnectionStatus;

		private ISteamNetworkingSockets.FGetDetailedConnectionStatus _GetDetailedConnectionStatus;

		private ISteamNetworkingSockets.FGetListenSocketAddress _GetListenSocketAddress;

		private ISteamNetworkingSockets.FCreateSocketPair _CreateSocketPair;

		private ISteamNetworkingSockets.FGetIdentity _GetIdentity;

		private ISteamNetworkingSockets.FReceivedRelayAuthTicket _ReceivedRelayAuthTicket;

		private ISteamNetworkingSockets.FFindRelayAuthTicketForServer _FindRelayAuthTicketForServer;

		private ISteamNetworkingSockets.FConnectToHostedDedicatedServer _ConnectToHostedDedicatedServer;

		private ISteamNetworkingSockets.FGetHostedDedicatedServerPort _GetHostedDedicatedServerPort;

		private ISteamNetworkingSockets.FGetHostedDedicatedServerPOPID _GetHostedDedicatedServerPOPID;

		private ISteamNetworkingSockets.FGetHostedDedicatedServerAddress _GetHostedDedicatedServerAddress;

		private ISteamNetworkingSockets.FCreateHostedDedicatedServerListenSocket _CreateHostedDedicatedServerListenSocket;

		private ISteamNetworkingSockets.FRunCallbacks _RunCallbacks;

		public override string InterfaceName
		{
			get
			{
				return "SteamNetworkingSockets002";
			}
		}

		public ISteamNetworkingSockets()
		{
		}

		internal Result AcceptConnection(Connection hConn)
		{
			return this._AcceptConnection(this.Self, hConn);
		}

		internal bool CloseConnection(Connection hPeer, int nReason, string pszDebug, bool bEnableLinger)
		{
			bool self = this._CloseConnection(this.Self, hPeer, nReason, pszDebug, bEnableLinger);
			return self;
		}

		internal bool CloseListenSocket(Socket hSocket)
		{
			return this._CloseListenSocket(this.Self, hSocket);
		}

		internal Connection ConnectByIPAddress(ref NetAddress address)
		{
			return this._ConnectByIPAddress(this.Self, ref address);
		}

		internal Connection ConnectP2P(ref NetIdentity identityRemote, int nVirtualPort)
		{
			return this._ConnectP2P(this.Self, ref identityRemote, nVirtualPort);
		}

		internal Connection ConnectToHostedDedicatedServer(ref NetIdentity identityTarget, int nVirtualPort)
		{
			return this._ConnectToHostedDedicatedServer(this.Self, ref identityTarget, nVirtualPort);
		}

		internal Socket CreateHostedDedicatedServerListenSocket(int nVirtualPort)
		{
			return this._CreateHostedDedicatedServerListenSocket(this.Self, nVirtualPort);
		}

		internal Socket CreateListenSocketIP(ref NetAddress localAddress)
		{
			return this._CreateListenSocketIP(this.Self, ref localAddress);
		}

		internal Socket CreateListenSocketP2P(int nVirtualPort)
		{
			return this._CreateListenSocketP2P(this.Self, nVirtualPort);
		}

		internal bool CreateSocketPair([In][Out] Connection[] pOutConnection1, [In][Out] Connection[] pOutConnection2, bool bUseNetworkLoopback, ref NetIdentity pIdentity1, ref NetIdentity pIdentity2)
		{
			bool self = this._CreateSocketPair(this.Self, pOutConnection1, pOutConnection2, bUseNetworkLoopback, ref pIdentity1, ref pIdentity2);
			return self;
		}

		internal int FindRelayAuthTicketForServer(ref NetIdentity identityGameServer, int nVirtualPort, [In][Out] SteamDatagramRelayAuthTicket[] pOutParsedTicket)
		{
			return this._FindRelayAuthTicketForServer(this.Self, ref identityGameServer, nVirtualPort, pOutParsedTicket);
		}

		internal Result FlushMessagesOnConnection(Connection hConn)
		{
			return this._FlushMessagesOnConnection(this.Self, hConn);
		}

		internal bool GetConnectionInfo(Connection hConn, ref ConnectionInfo pInfo)
		{
			return this._GetConnectionInfo(this.Self, hConn, ref pInfo);
		}

		internal bool GetConnectionName(Connection hPeer, StringBuilder pszName, int nMaxLen)
		{
			return this._GetConnectionName(this.Self, hPeer, pszName, nMaxLen);
		}

		internal long GetConnectionUserData(Connection hPeer)
		{
			return this._GetConnectionUserData(this.Self, hPeer);
		}

		internal int GetDetailedConnectionStatus(Connection hConn, StringBuilder pszBuf, int cbBuf)
		{
			return this._GetDetailedConnectionStatus(this.Self, hConn, pszBuf, cbBuf);
		}

		internal bool GetHostedDedicatedServerAddress(ref SteamDatagramHostedAddress pRouting)
		{
			return this._GetHostedDedicatedServerAddress(this.Self, ref pRouting);
		}

		internal SteamNetworkingPOPID GetHostedDedicatedServerPOPID()
		{
			return this._GetHostedDedicatedServerPOPID(this.Self);
		}

		internal ushort GetHostedDedicatedServerPort()
		{
			return this._GetHostedDedicatedServerPort(this.Self);
		}

		internal bool GetIdentity(ref NetIdentity pIdentity)
		{
			return this._GetIdentity(this.Self, ref pIdentity);
		}

		internal bool GetListenSocketAddress(Socket hSocket, ref NetAddress address)
		{
			return this._GetListenSocketAddress(this.Self, hSocket, ref address);
		}

		internal bool GetQuickConnectionStatus(Connection hConn, ref SteamNetworkingQuickConnectionStatus pStats)
		{
			return this._GetQuickConnectionStatus(this.Self, hConn, ref pStats);
		}

		public override void InitInternals()
		{
			this._CreateListenSocketIP = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingSockets.FCreateListenSocketIP>(Marshal.ReadIntPtr(this.VTable, 0));
			this._ConnectByIPAddress = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingSockets.FConnectByIPAddress>(Marshal.ReadIntPtr(this.VTable, 8));
			this._CreateListenSocketP2P = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingSockets.FCreateListenSocketP2P>(Marshal.ReadIntPtr(this.VTable, 16));
			this._ConnectP2P = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingSockets.FConnectP2P>(Marshal.ReadIntPtr(this.VTable, 24));
			this._AcceptConnection = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingSockets.FAcceptConnection>(Marshal.ReadIntPtr(this.VTable, 32));
			this._CloseConnection = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingSockets.FCloseConnection>(Marshal.ReadIntPtr(this.VTable, 40));
			this._CloseListenSocket = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingSockets.FCloseListenSocket>(Marshal.ReadIntPtr(this.VTable, 48));
			this._SetConnectionUserData = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingSockets.FSetConnectionUserData>(Marshal.ReadIntPtr(this.VTable, 56));
			this._GetConnectionUserData = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingSockets.FGetConnectionUserData>(Marshal.ReadIntPtr(this.VTable, 64));
			this._SetConnectionName = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingSockets.FSetConnectionName>(Marshal.ReadIntPtr(this.VTable, 72));
			this._GetConnectionName = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingSockets.FGetConnectionName>(Marshal.ReadIntPtr(this.VTable, 80));
			this._SendMessageToConnection = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingSockets.FSendMessageToConnection>(Marshal.ReadIntPtr(this.VTable, 88));
			this._FlushMessagesOnConnection = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingSockets.FFlushMessagesOnConnection>(Marshal.ReadIntPtr(this.VTable, 96));
			this._ReceiveMessagesOnConnection = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingSockets.FReceiveMessagesOnConnection>(Marshal.ReadIntPtr(this.VTable, 104));
			this._ReceiveMessagesOnListenSocket = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingSockets.FReceiveMessagesOnListenSocket>(Marshal.ReadIntPtr(this.VTable, 112));
			this._GetConnectionInfo = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingSockets.FGetConnectionInfo>(Marshal.ReadIntPtr(this.VTable, 120));
			this._GetQuickConnectionStatus = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingSockets.FGetQuickConnectionStatus>(Marshal.ReadIntPtr(this.VTable, 128));
			this._GetDetailedConnectionStatus = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingSockets.FGetDetailedConnectionStatus>(Marshal.ReadIntPtr(this.VTable, 136));
			this._GetListenSocketAddress = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingSockets.FGetListenSocketAddress>(Marshal.ReadIntPtr(this.VTable, 144));
			this._CreateSocketPair = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingSockets.FCreateSocketPair>(Marshal.ReadIntPtr(this.VTable, 152));
			this._GetIdentity = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingSockets.FGetIdentity>(Marshal.ReadIntPtr(this.VTable, 160));
			this._ReceivedRelayAuthTicket = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingSockets.FReceivedRelayAuthTicket>(Marshal.ReadIntPtr(this.VTable, 168));
			this._FindRelayAuthTicketForServer = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingSockets.FFindRelayAuthTicketForServer>(Marshal.ReadIntPtr(this.VTable, 176));
			this._ConnectToHostedDedicatedServer = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingSockets.FConnectToHostedDedicatedServer>(Marshal.ReadIntPtr(this.VTable, 184));
			this._GetHostedDedicatedServerPort = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingSockets.FGetHostedDedicatedServerPort>(Marshal.ReadIntPtr(this.VTable, 192));
			this._GetHostedDedicatedServerPOPID = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingSockets.FGetHostedDedicatedServerPOPID>(Marshal.ReadIntPtr(this.VTable, 200));
			this._GetHostedDedicatedServerAddress = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingSockets.FGetHostedDedicatedServerAddress>(Marshal.ReadIntPtr(this.VTable, 208));
			this._CreateHostedDedicatedServerListenSocket = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingSockets.FCreateHostedDedicatedServerListenSocket>(Marshal.ReadIntPtr(this.VTable, 216));
			this._RunCallbacks = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingSockets.FRunCallbacks>(Marshal.ReadIntPtr(this.VTable, 224));
		}

		internal bool ReceivedRelayAuthTicket(IntPtr pvTicket, int cbTicket, [In][Out] SteamDatagramRelayAuthTicket[] pOutParsedTicket)
		{
			return this._ReceivedRelayAuthTicket(this.Self, pvTicket, cbTicket, pOutParsedTicket);
		}

		internal int ReceiveMessagesOnConnection(Connection hConn, IntPtr ppOutMessages, int nMaxMessages)
		{
			return this._ReceiveMessagesOnConnection(this.Self, hConn, ppOutMessages, nMaxMessages);
		}

		internal int ReceiveMessagesOnListenSocket(Socket hSocket, IntPtr ppOutMessages, int nMaxMessages)
		{
			return this._ReceiveMessagesOnListenSocket(this.Self, hSocket, ppOutMessages, nMaxMessages);
		}

		internal void RunCallbacks(IntPtr pCallbacks)
		{
			this._RunCallbacks(this.Self, pCallbacks);
		}

		internal Result SendMessageToConnection(Connection hConn, IntPtr pData, uint cbData, int nSendFlags)
		{
			Result self = this._SendMessageToConnection(this.Self, hConn, pData, cbData, nSendFlags);
			return self;
		}

		internal void SetConnectionName(Connection hPeer, string pszName)
		{
			this._SetConnectionName(this.Self, hPeer, pszName);
		}

		internal bool SetConnectionUserData(Connection hPeer, long nUserData)
		{
			return this._SetConnectionUserData(this.Self, hPeer, nUserData);
		}

		internal override void Shutdown()
		{
			base.Shutdown();
			this._CreateListenSocketIP = null;
			this._ConnectByIPAddress = null;
			this._CreateListenSocketP2P = null;
			this._ConnectP2P = null;
			this._AcceptConnection = null;
			this._CloseConnection = null;
			this._CloseListenSocket = null;
			this._SetConnectionUserData = null;
			this._GetConnectionUserData = null;
			this._SetConnectionName = null;
			this._GetConnectionName = null;
			this._SendMessageToConnection = null;
			this._FlushMessagesOnConnection = null;
			this._ReceiveMessagesOnConnection = null;
			this._ReceiveMessagesOnListenSocket = null;
			this._GetConnectionInfo = null;
			this._GetQuickConnectionStatus = null;
			this._GetDetailedConnectionStatus = null;
			this._GetListenSocketAddress = null;
			this._CreateSocketPair = null;
			this._GetIdentity = null;
			this._ReceivedRelayAuthTicket = null;
			this._FindRelayAuthTicketForServer = null;
			this._ConnectToHostedDedicatedServer = null;
			this._GetHostedDedicatedServerPort = null;
			this._GetHostedDedicatedServerPOPID = null;
			this._GetHostedDedicatedServerAddress = null;
			this._CreateHostedDedicatedServerListenSocket = null;
			this._RunCallbacks = null;
		}

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate Result FAcceptConnection(IntPtr self, Connection hConn);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FCloseConnection(IntPtr self, Connection hPeer, int nReason, string pszDebug, bool bEnableLinger);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FCloseListenSocket(IntPtr self, Socket hSocket);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate Connection FConnectByIPAddress(IntPtr self, ref NetAddress address);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate Connection FConnectP2P(IntPtr self, ref NetIdentity identityRemote, int nVirtualPort);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate Connection FConnectToHostedDedicatedServer(IntPtr self, ref NetIdentity identityTarget, int nVirtualPort);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate Socket FCreateHostedDedicatedServerListenSocket(IntPtr self, int nVirtualPort);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate Socket FCreateListenSocketIP(IntPtr self, ref NetAddress localAddress);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate Socket FCreateListenSocketP2P(IntPtr self, int nVirtualPort);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FCreateSocketPair(IntPtr self, [In][Out] Connection[] pOutConnection1, [In][Out] Connection[] pOutConnection2, bool bUseNetworkLoopback, ref NetIdentity pIdentity1, ref NetIdentity pIdentity2);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FFindRelayAuthTicketForServer(IntPtr self, ref NetIdentity identityGameServer, int nVirtualPort, [In][Out] SteamDatagramRelayAuthTicket[] pOutParsedTicket);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate Result FFlushMessagesOnConnection(IntPtr self, Connection hConn);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetConnectionInfo(IntPtr self, Connection hConn, ref ConnectionInfo pInfo);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetConnectionName(IntPtr self, Connection hPeer, StringBuilder pszName, int nMaxLen);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate long FGetConnectionUserData(IntPtr self, Connection hPeer);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetDetailedConnectionStatus(IntPtr self, Connection hConn, StringBuilder pszBuf, int cbBuf);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetHostedDedicatedServerAddress(IntPtr self, ref SteamDatagramHostedAddress pRouting);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamNetworkingPOPID FGetHostedDedicatedServerPOPID(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate ushort FGetHostedDedicatedServerPort(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetIdentity(IntPtr self, ref NetIdentity pIdentity);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetListenSocketAddress(IntPtr self, Socket hSocket, ref NetAddress address);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetQuickConnectionStatus(IntPtr self, Connection hConn, ref SteamNetworkingQuickConnectionStatus pStats);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FReceivedRelayAuthTicket(IntPtr self, IntPtr pvTicket, int cbTicket, [In][Out] SteamDatagramRelayAuthTicket[] pOutParsedTicket);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FReceiveMessagesOnConnection(IntPtr self, Connection hConn, IntPtr ppOutMessages, int nMaxMessages);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FReceiveMessagesOnListenSocket(IntPtr self, Socket hSocket, IntPtr ppOutMessages, int nMaxMessages);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FRunCallbacks(IntPtr self, IntPtr pCallbacks);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate Result FSendMessageToConnection(IntPtr self, Connection hConn, IntPtr pData, uint cbData, int nSendFlags);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FSetConnectionName(IntPtr self, Connection hPeer, string pszName);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetConnectionUserData(IntPtr self, Connection hPeer, long nUserData);
	}
}