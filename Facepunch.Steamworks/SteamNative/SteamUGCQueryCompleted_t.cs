using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct SteamUGCQueryCompleted_t
	{
		internal const int CallbackId = 3401;

		internal ulong Handle;

		internal SteamNative.Result Result;

		internal uint NumResultsReturned;

		internal uint TotalMatchingResults;

		internal bool CachedData;

		internal static CallResult<SteamUGCQueryCompleted_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<SteamUGCQueryCompleted_t, bool> CallbackFunction)
		{
			return new CallResult<SteamUGCQueryCompleted_t>(steamworks, call, CallbackFunction, new CallResult<SteamUGCQueryCompleted_t>.ConvertFromPointer(SteamUGCQueryCompleted_t.FromPointer), SteamUGCQueryCompleted_t.StructSize(), 3401);
		}

		internal static SteamUGCQueryCompleted_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (SteamUGCQueryCompleted_t)Marshal.PtrToStructure(p, typeof(SteamUGCQueryCompleted_t));
			}
			return (SteamUGCQueryCompleted_t.PackSmall)Marshal.PtrToStructure(p, typeof(SteamUGCQueryCompleted_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return SteamUGCQueryCompleted_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return SteamUGCQueryCompleted_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			SteamUGCQueryCompleted_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			SteamUGCQueryCompleted_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			SteamUGCQueryCompleted_t steamUGCQueryCompletedT = SteamUGCQueryCompleted_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<SteamUGCQueryCompleted_t>(steamUGCQueryCompletedT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<SteamUGCQueryCompleted_t>(steamUGCQueryCompletedT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			SteamUGCQueryCompleted_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(SteamUGCQueryCompleted_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(SteamUGCQueryCompleted_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(SteamUGCQueryCompleted_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(SteamUGCQueryCompleted_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(SteamUGCQueryCompleted_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(SteamUGCQueryCompleted_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(SteamUGCQueryCompleted_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(SteamUGCQueryCompleted_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(SteamUGCQueryCompleted_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(SteamUGCQueryCompleted_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(SteamUGCQueryCompleted_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(SteamUGCQueryCompleted_t.OnGetSize)
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
				CallbackId = 3401
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 3401);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(SteamUGCQueryCompleted_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(SteamUGCQueryCompleted_t));
		}

		internal struct PackSmall
		{
			internal ulong Handle;

			internal SteamNative.Result Result;

			internal uint NumResultsReturned;

			internal uint TotalMatchingResults;

			internal bool CachedData;

			public static implicit operator SteamUGCQueryCompleted_t(SteamUGCQueryCompleted_t.PackSmall d)
			{
				SteamUGCQueryCompleted_t steamUGCQueryCompletedT = new SteamUGCQueryCompleted_t()
				{
					Handle = d.Handle,
					Result = d.Result,
					NumResultsReturned = d.NumResultsReturned,
					TotalMatchingResults = d.TotalMatchingResults,
					CachedData = d.CachedData
				};
				return steamUGCQueryCompletedT;
			}
		}
	}
}