using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct GSStatsReceived_t
	{
		internal const int CallbackId = 1800;

		internal SteamNative.Result Result;

		internal ulong SteamIDUser;

		internal static CallResult<GSStatsReceived_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<GSStatsReceived_t, bool> CallbackFunction)
		{
			return new CallResult<GSStatsReceived_t>(steamworks, call, CallbackFunction, new CallResult<GSStatsReceived_t>.ConvertFromPointer(GSStatsReceived_t.FromPointer), GSStatsReceived_t.StructSize(), 1800);
		}

		internal static GSStatsReceived_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (GSStatsReceived_t)Marshal.PtrToStructure(p, typeof(GSStatsReceived_t));
			}
			return (GSStatsReceived_t.PackSmall)Marshal.PtrToStructure(p, typeof(GSStatsReceived_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return GSStatsReceived_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return GSStatsReceived_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			GSStatsReceived_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			GSStatsReceived_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			GSStatsReceived_t gSStatsReceivedT = GSStatsReceived_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<GSStatsReceived_t>(gSStatsReceivedT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<GSStatsReceived_t>(gSStatsReceivedT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			GSStatsReceived_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(GSStatsReceived_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(GSStatsReceived_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(GSStatsReceived_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(GSStatsReceived_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(GSStatsReceived_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(GSStatsReceived_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(GSStatsReceived_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(GSStatsReceived_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(GSStatsReceived_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(GSStatsReceived_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(GSStatsReceived_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(GSStatsReceived_t.OnGetSize)
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
				CallbackId = 1800
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 1800);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(GSStatsReceived_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(GSStatsReceived_t));
		}

		internal struct PackSmall
		{
			internal SteamNative.Result Result;

			internal ulong SteamIDUser;

			public static implicit operator GSStatsReceived_t(GSStatsReceived_t.PackSmall d)
			{
				GSStatsReceived_t gSStatsReceivedT = new GSStatsReceived_t()
				{
					Result = d.Result,
					SteamIDUser = d.SteamIDUser
				};
				return gSStatsReceivedT;
			}
		}
	}
}