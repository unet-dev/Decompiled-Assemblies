using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct ClientGameServerDeny_t
	{
		internal const int CallbackId = 113;

		internal uint AppID;

		internal uint GameServerIP;

		internal ushort GameServerPort;

		internal ushort Secure;

		internal uint Reason;

		internal static ClientGameServerDeny_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (ClientGameServerDeny_t)Marshal.PtrToStructure(p, typeof(ClientGameServerDeny_t));
			}
			return (ClientGameServerDeny_t.PackSmall)Marshal.PtrToStructure(p, typeof(ClientGameServerDeny_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return ClientGameServerDeny_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return ClientGameServerDeny_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			ClientGameServerDeny_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			ClientGameServerDeny_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			ClientGameServerDeny_t clientGameServerDenyT = ClientGameServerDeny_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<ClientGameServerDeny_t>(clientGameServerDenyT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<ClientGameServerDeny_t>(clientGameServerDenyT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			ClientGameServerDeny_t.OnResultWithInfo(param, failure, call);
		}

		internal static void Register(BaseSteamworks steamworks)
		{
			CallbackHandle callbackHandle = new CallbackHandle(steamworks);
			if (Config.UseThisCall)
			{
				if (!Platform.IsWindows)
				{
					callbackHandle.vTablePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Callback.VTableThis)));
					Callback.VTableThis vTableThi = new Callback.VTableThis()
					{
						ResultA = new Callback.VTableThis.ResultD(ClientGameServerDeny_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(ClientGameServerDeny_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(ClientGameServerDeny_t.OnGetSizeThis)
					};
					callbackHandle.FuncA = GCHandle.Alloc(vTableThi.ResultA);
					callbackHandle.FuncB = GCHandle.Alloc(vTableThi.ResultB);
					callbackHandle.FuncC = GCHandle.Alloc(vTableThi.GetSize);
					Marshal.StructureToPtr(vTableThi, callbackHandle.vTablePtr, false);
				}
				else
				{
					callbackHandle.vTablePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Callback.VTableWinThis)));
					Callback.VTableWinThis vTableWinThi = new Callback.VTableWinThis()
					{
						ResultA = new Callback.VTableWinThis.ResultD(ClientGameServerDeny_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(ClientGameServerDeny_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(ClientGameServerDeny_t.OnGetSizeThis)
					};
					callbackHandle.FuncA = GCHandle.Alloc(vTableWinThi.ResultA);
					callbackHandle.FuncB = GCHandle.Alloc(vTableWinThi.ResultB);
					callbackHandle.FuncC = GCHandle.Alloc(vTableWinThi.GetSize);
					Marshal.StructureToPtr(vTableWinThi, callbackHandle.vTablePtr, false);
				}
			}
			else if (!Platform.IsWindows)
			{
				callbackHandle.vTablePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Callback.VTable)));
				Callback.VTable vTable = new Callback.VTable()
				{
					ResultA = new Callback.VTable.ResultD(ClientGameServerDeny_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(ClientGameServerDeny_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(ClientGameServerDeny_t.OnGetSize)
				};
				callbackHandle.FuncA = GCHandle.Alloc(vTable.ResultA);
				callbackHandle.FuncB = GCHandle.Alloc(vTable.ResultB);
				callbackHandle.FuncC = GCHandle.Alloc(vTable.GetSize);
				Marshal.StructureToPtr(vTable, callbackHandle.vTablePtr, false);
			}
			else
			{
				callbackHandle.vTablePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Callback.VTableWin)));
				Callback.VTableWin vTableWin = new Callback.VTableWin()
				{
					ResultA = new Callback.VTableWin.ResultD(ClientGameServerDeny_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(ClientGameServerDeny_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(ClientGameServerDeny_t.OnGetSize)
				};
				callbackHandle.FuncA = GCHandle.Alloc(vTableWin.ResultA);
				callbackHandle.FuncB = GCHandle.Alloc(vTableWin.ResultB);
				callbackHandle.FuncC = GCHandle.Alloc(vTableWin.GetSize);
				Marshal.StructureToPtr(vTableWin, callbackHandle.vTablePtr, false);
			}
			Callback callback = new Callback()
			{
				vTablePtr = callbackHandle.vTablePtr,
				CallbackFlags = (byte)((steamworks.IsGameServer ? 2 : 0)),
				CallbackId = 113
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 113);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(ClientGameServerDeny_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(ClientGameServerDeny_t));
		}

		internal struct PackSmall
		{
			internal uint AppID;

			internal uint GameServerIP;

			internal ushort GameServerPort;

			internal ushort Secure;

			internal uint Reason;

			public static implicit operator ClientGameServerDeny_t(ClientGameServerDeny_t.PackSmall d)
			{
				ClientGameServerDeny_t clientGameServerDenyT = new ClientGameServerDeny_t()
				{
					AppID = d.AppID,
					GameServerIP = d.GameServerIP,
					GameServerPort = d.GameServerPort,
					Secure = d.Secure,
					Reason = d.Reason
				};
				return clientGameServerDenyT;
			}
		}
	}
}