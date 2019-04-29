using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct LobbyChatUpdate_t
	{
		internal const int CallbackId = 506;

		internal ulong SteamIDLobby;

		internal ulong SteamIDUserChanged;

		internal ulong SteamIDMakingChange;

		internal uint GfChatMemberStateChange;

		internal static LobbyChatUpdate_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (LobbyChatUpdate_t)Marshal.PtrToStructure(p, typeof(LobbyChatUpdate_t));
			}
			return (LobbyChatUpdate_t.PackSmall)Marshal.PtrToStructure(p, typeof(LobbyChatUpdate_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return LobbyChatUpdate_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return LobbyChatUpdate_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			LobbyChatUpdate_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			LobbyChatUpdate_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			LobbyChatUpdate_t lobbyChatUpdateT = LobbyChatUpdate_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<LobbyChatUpdate_t>(lobbyChatUpdateT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<LobbyChatUpdate_t>(lobbyChatUpdateT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			LobbyChatUpdate_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(LobbyChatUpdate_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(LobbyChatUpdate_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(LobbyChatUpdate_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(LobbyChatUpdate_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(LobbyChatUpdate_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(LobbyChatUpdate_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(LobbyChatUpdate_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(LobbyChatUpdate_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(LobbyChatUpdate_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(LobbyChatUpdate_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(LobbyChatUpdate_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(LobbyChatUpdate_t.OnGetSize)
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
				CallbackId = 506
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 506);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(LobbyChatUpdate_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(LobbyChatUpdate_t));
		}

		internal struct PackSmall
		{
			internal ulong SteamIDLobby;

			internal ulong SteamIDUserChanged;

			internal ulong SteamIDMakingChange;

			internal uint GfChatMemberStateChange;

			public static implicit operator LobbyChatUpdate_t(LobbyChatUpdate_t.PackSmall d)
			{
				LobbyChatUpdate_t lobbyChatUpdateT = new LobbyChatUpdate_t()
				{
					SteamIDLobby = d.SteamIDLobby,
					SteamIDUserChanged = d.SteamIDUserChanged,
					SteamIDMakingChange = d.SteamIDMakingChange,
					GfChatMemberStateChange = d.GfChatMemberStateChange
				};
				return lobbyChatUpdateT;
			}
		}
	}
}