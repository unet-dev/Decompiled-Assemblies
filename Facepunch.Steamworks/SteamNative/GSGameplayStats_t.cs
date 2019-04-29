using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct GSGameplayStats_t
	{
		internal const int CallbackId = 207;

		internal SteamNative.Result Result;

		internal int Rank;

		internal uint TotalConnects;

		internal uint TotalMinutesPlayed;

		internal static GSGameplayStats_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (GSGameplayStats_t)Marshal.PtrToStructure(p, typeof(GSGameplayStats_t));
			}
			return (GSGameplayStats_t.PackSmall)Marshal.PtrToStructure(p, typeof(GSGameplayStats_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return GSGameplayStats_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return GSGameplayStats_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			GSGameplayStats_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			GSGameplayStats_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			GSGameplayStats_t gSGameplayStatsT = GSGameplayStats_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<GSGameplayStats_t>(gSGameplayStatsT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<GSGameplayStats_t>(gSGameplayStatsT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			GSGameplayStats_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(GSGameplayStats_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(GSGameplayStats_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(GSGameplayStats_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(GSGameplayStats_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(GSGameplayStats_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(GSGameplayStats_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(GSGameplayStats_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(GSGameplayStats_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(GSGameplayStats_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(GSGameplayStats_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(GSGameplayStats_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(GSGameplayStats_t.OnGetSize)
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
				CallbackId = 207
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 207);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(GSGameplayStats_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(GSGameplayStats_t));
		}

		internal struct PackSmall
		{
			internal SteamNative.Result Result;

			internal int Rank;

			internal uint TotalConnects;

			internal uint TotalMinutesPlayed;

			public static implicit operator GSGameplayStats_t(GSGameplayStats_t.PackSmall d)
			{
				GSGameplayStats_t gSGameplayStatsT = new GSGameplayStats_t()
				{
					Result = d.Result,
					Rank = d.Rank,
					TotalConnects = d.TotalConnects,
					TotalMinutesPlayed = d.TotalMinutesPlayed
				};
				return gSGameplayStatsT;
			}
		}
	}
}