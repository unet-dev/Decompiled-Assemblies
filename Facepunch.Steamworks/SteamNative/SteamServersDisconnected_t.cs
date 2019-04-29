using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct SteamServersDisconnected_t
	{
		internal const int CallbackId = 103;

		internal SteamNative.Result Result;

		internal static SteamServersDisconnected_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (SteamServersDisconnected_t)Marshal.PtrToStructure(p, typeof(SteamServersDisconnected_t));
			}
			return (SteamServersDisconnected_t.PackSmall)Marshal.PtrToStructure(p, typeof(SteamServersDisconnected_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return SteamServersDisconnected_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return SteamServersDisconnected_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			SteamServersDisconnected_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			SteamServersDisconnected_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			SteamServersDisconnected_t steamServersDisconnectedT = SteamServersDisconnected_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<SteamServersDisconnected_t>(steamServersDisconnectedT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<SteamServersDisconnected_t>(steamServersDisconnectedT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			SteamServersDisconnected_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(SteamServersDisconnected_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(SteamServersDisconnected_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(SteamServersDisconnected_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(SteamServersDisconnected_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(SteamServersDisconnected_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(SteamServersDisconnected_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(SteamServersDisconnected_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(SteamServersDisconnected_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(SteamServersDisconnected_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(SteamServersDisconnected_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(SteamServersDisconnected_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(SteamServersDisconnected_t.OnGetSize)
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
				CallbackId = 103
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 103);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(SteamServersDisconnected_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(SteamServersDisconnected_t));
		}

		internal struct PackSmall
		{
			internal SteamNative.Result Result;

			public static implicit operator SteamServersDisconnected_t(SteamServersDisconnected_t.PackSmall d)
			{
				return new SteamServersDisconnected_t()
				{
					Result = d.Result
				};
			}
		}
	}
}