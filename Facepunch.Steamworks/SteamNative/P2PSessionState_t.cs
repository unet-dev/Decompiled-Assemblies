using System;
using System.Runtime.InteropServices;

namespace SteamNative
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

		internal static P2PSessionState_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (P2PSessionState_t)Marshal.PtrToStructure(p, typeof(P2PSessionState_t));
			}
			return (P2PSessionState_t.PackSmall)Marshal.PtrToStructure(p, typeof(P2PSessionState_t.PackSmall));
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(P2PSessionState_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(P2PSessionState_t));
		}

		internal struct PackSmall
		{
			internal byte ConnectionActive;

			internal byte Connecting;

			internal byte P2PSessionError;

			internal byte UsingRelay;

			internal int BytesQueuedForSend;

			internal int PacketsQueuedForSend;

			internal uint RemoteIP;

			internal ushort RemotePort;

			public static implicit operator P2PSessionState_t(P2PSessionState_t.PackSmall d)
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
		}
	}
}