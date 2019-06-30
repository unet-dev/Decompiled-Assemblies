using Steamworks;
using System;
using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	internal struct P2PSessionState_t
	{
		internal byte ConnectionActive;

		internal byte Connecting;

		internal byte P2PSessionError;

		internal byte UsingRelay;

		internal int BytesQueuedForSend;

		internal int PacketsQueuedForSend;

		internal uint RemoteIP;

		internal ushort RemotePort;

		internal static P2PSessionState_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (P2PSessionState_t)Marshal.PtrToStructure(p, typeof(P2PSessionState_t)) : (P2PSessionState_t.Pack8)Marshal.PtrToStructure(p, typeof(P2PSessionState_t.Pack8)));
		}

		public struct Pack8
		{
			internal byte ConnectionActive;

			internal byte Connecting;

			internal byte P2PSessionError;

			internal byte UsingRelay;

			internal int BytesQueuedForSend;

			internal int PacketsQueuedForSend;

			internal uint RemoteIP;

			internal ushort RemotePort;

			public static implicit operator P2PSessionState_t(P2PSessionState_t.Pack8 d)
			{
				P2PSessionState_t p2PSessionStateT = new P2PSessionState_t()
				{
					ConnectionActive = d.ConnectionActive,
					Connecting = d.Connecting,
					P2PSessionError = d.P2PSessionError,
					UsingRelay = d.UsingRelay,
					BytesQueuedForSend = d.BytesQueuedForSend,
					PacketsQueuedForSend = d.PacketsQueuedForSend,
					RemoteIP = d.RemoteIP,
					RemotePort = d.RemotePort
				};
				return p2PSessionStateT;
			}

			public static implicit operator Pack8(P2PSessionState_t d)
			{
				P2PSessionState_t.Pack8 pack8 = new P2PSessionState_t.Pack8()
				{
					ConnectionActive = d.ConnectionActive,
					Connecting = d.Connecting,
					P2PSessionError = d.P2PSessionError,
					UsingRelay = d.UsingRelay,
					BytesQueuedForSend = d.BytesQueuedForSend,
					PacketsQueuedForSend = d.PacketsQueuedForSend,
					RemoteIP = d.RemoteIP,
					RemotePort = d.RemotePort
				};
				return pack8;
			}
		}
	}
}