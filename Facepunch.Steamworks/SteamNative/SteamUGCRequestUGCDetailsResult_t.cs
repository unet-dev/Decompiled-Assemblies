using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct SteamUGCRequestUGCDetailsResult_t
	{
		internal const int CallbackId = 3402;

		internal SteamUGCDetails_t Details;

		internal bool CachedData;

		internal static SteamUGCRequestUGCDetailsResult_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (SteamUGCRequestUGCDetailsResult_t)Marshal.PtrToStructure(p, typeof(SteamUGCRequestUGCDetailsResult_t));
			}
			return (SteamUGCRequestUGCDetailsResult_t.PackSmall)Marshal.PtrToStructure(p, typeof(SteamUGCRequestUGCDetailsResult_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return SteamUGCRequestUGCDetailsResult_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return SteamUGCRequestUGCDetailsResult_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			SteamUGCRequestUGCDetailsResult_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			SteamUGCRequestUGCDetailsResult_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			SteamUGCRequestUGCDetailsResult_t steamUGCRequestUGCDetailsResultT = SteamUGCRequestUGCDetailsResult_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<SteamUGCRequestUGCDetailsResult_t>(steamUGCRequestUGCDetailsResultT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<SteamUGCRequestUGCDetailsResult_t>(steamUGCRequestUGCDetailsResultT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			SteamUGCRequestUGCDetailsResult_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(SteamUGCRequestUGCDetailsResult_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(SteamUGCRequestUGCDetailsResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(SteamUGCRequestUGCDetailsResult_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(SteamUGCRequestUGCDetailsResult_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(SteamUGCRequestUGCDetailsResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(SteamUGCRequestUGCDetailsResult_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(SteamUGCRequestUGCDetailsResult_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(SteamUGCRequestUGCDetailsResult_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(SteamUGCRequestUGCDetailsResult_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(SteamUGCRequestUGCDetailsResult_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(SteamUGCRequestUGCDetailsResult_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(SteamUGCRequestUGCDetailsResult_t.OnGetSize)
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
				CallbackId = 3402
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 3402);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(SteamUGCRequestUGCDetailsResult_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(SteamUGCRequestUGCDetailsResult_t));
		}

		internal struct PackSmall
		{
			internal SteamUGCDetails_t Details;

			internal bool CachedData;

			public static implicit operator SteamUGCRequestUGCDetailsResult_t(SteamUGCRequestUGCDetailsResult_t.PackSmall d)
			{
				SteamUGCRequestUGCDetailsResult_t steamUGCRequestUGCDetailsResultT = new SteamUGCRequestUGCDetailsResult_t()
				{
					Details = d.Details,
					CachedData = d.CachedData
				};
				return steamUGCRequestUGCDetailsResultT;
			}
		}
	}
}