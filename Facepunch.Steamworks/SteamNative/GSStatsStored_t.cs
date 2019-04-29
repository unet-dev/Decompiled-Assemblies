using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct GSStatsStored_t
	{
		internal const int CallbackId = 1801;

		internal SteamNative.Result Result;

		internal ulong SteamIDUser;

		internal static CallResult<GSStatsStored_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<GSStatsStored_t, bool> CallbackFunction)
		{
			return new CallResult<GSStatsStored_t>(steamworks, call, CallbackFunction, new CallResult<GSStatsStored_t>.ConvertFromPointer(GSStatsStored_t.FromPointer), GSStatsStored_t.StructSize(), 1801);
		}

		internal static GSStatsStored_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (GSStatsStored_t)Marshal.PtrToStructure(p, typeof(GSStatsStored_t));
			}
			return (GSStatsStored_t.PackSmall)Marshal.PtrToStructure(p, typeof(GSStatsStored_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return GSStatsStored_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return GSStatsStored_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			GSStatsStored_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			GSStatsStored_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			GSStatsStored_t gSStatsStoredT = GSStatsStored_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<GSStatsStored_t>(gSStatsStoredT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<GSStatsStored_t>(gSStatsStoredT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			GSStatsStored_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(GSStatsStored_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(GSStatsStored_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(GSStatsStored_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(GSStatsStored_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(GSStatsStored_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(GSStatsStored_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(GSStatsStored_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(GSStatsStored_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(GSStatsStored_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(GSStatsStored_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(GSStatsStored_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(GSStatsStored_t.OnGetSize)
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
				CallbackId = 1801
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 1801);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(GSStatsStored_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(GSStatsStored_t));
		}

		internal struct PackSmall
		{
			internal SteamNative.Result Result;

			internal ulong SteamIDUser;

			public static implicit operator GSStatsStored_t(GSStatsStored_t.PackSmall d)
			{
				GSStatsStored_t gSStatsStoredT = new GSStatsStored_t()
				{
					Result = d.Result,
					SteamIDUser = d.SteamIDUser
				};
				return gSStatsStoredT;
			}
		}
	}
}