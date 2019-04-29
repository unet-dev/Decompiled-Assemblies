using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct GSStatsUnloaded_t
	{
		internal const int CallbackId = 1108;

		internal ulong SteamIDUser;

		internal static GSStatsUnloaded_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (GSStatsUnloaded_t)Marshal.PtrToStructure(p, typeof(GSStatsUnloaded_t));
			}
			return (GSStatsUnloaded_t.PackSmall)Marshal.PtrToStructure(p, typeof(GSStatsUnloaded_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return GSStatsUnloaded_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return GSStatsUnloaded_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			GSStatsUnloaded_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			GSStatsUnloaded_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			GSStatsUnloaded_t gSStatsUnloadedT = GSStatsUnloaded_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<GSStatsUnloaded_t>(gSStatsUnloadedT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<GSStatsUnloaded_t>(gSStatsUnloadedT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			GSStatsUnloaded_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(GSStatsUnloaded_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(GSStatsUnloaded_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(GSStatsUnloaded_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(GSStatsUnloaded_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(GSStatsUnloaded_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(GSStatsUnloaded_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(GSStatsUnloaded_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(GSStatsUnloaded_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(GSStatsUnloaded_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(GSStatsUnloaded_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(GSStatsUnloaded_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(GSStatsUnloaded_t.OnGetSize)
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
				CallbackId = 1108
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 1108);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(GSStatsUnloaded_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(GSStatsUnloaded_t));
		}

		internal struct PackSmall
		{
			internal ulong SteamIDUser;

			public static implicit operator GSStatsUnloaded_t(GSStatsUnloaded_t.PackSmall d)
			{
				return new GSStatsUnloaded_t()
				{
					SteamIDUser = d.SteamIDUser
				};
			}
		}
	}
}