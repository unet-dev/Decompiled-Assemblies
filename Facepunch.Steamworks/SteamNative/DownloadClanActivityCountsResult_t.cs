using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct DownloadClanActivityCountsResult_t
	{
		internal const int CallbackId = 341;

		internal bool Success;

		internal static DownloadClanActivityCountsResult_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (DownloadClanActivityCountsResult_t)Marshal.PtrToStructure(p, typeof(DownloadClanActivityCountsResult_t));
			}
			return (DownloadClanActivityCountsResult_t.PackSmall)Marshal.PtrToStructure(p, typeof(DownloadClanActivityCountsResult_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return DownloadClanActivityCountsResult_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return DownloadClanActivityCountsResult_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			DownloadClanActivityCountsResult_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			DownloadClanActivityCountsResult_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			DownloadClanActivityCountsResult_t downloadClanActivityCountsResultT = DownloadClanActivityCountsResult_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<DownloadClanActivityCountsResult_t>(downloadClanActivityCountsResultT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<DownloadClanActivityCountsResult_t>(downloadClanActivityCountsResultT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			DownloadClanActivityCountsResult_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(DownloadClanActivityCountsResult_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(DownloadClanActivityCountsResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(DownloadClanActivityCountsResult_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(DownloadClanActivityCountsResult_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(DownloadClanActivityCountsResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(DownloadClanActivityCountsResult_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(DownloadClanActivityCountsResult_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(DownloadClanActivityCountsResult_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(DownloadClanActivityCountsResult_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(DownloadClanActivityCountsResult_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(DownloadClanActivityCountsResult_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(DownloadClanActivityCountsResult_t.OnGetSize)
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
				CallbackId = 341
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 341);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(DownloadClanActivityCountsResult_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(DownloadClanActivityCountsResult_t));
		}

		internal struct PackSmall
		{
			internal bool Success;

			public static implicit operator DownloadClanActivityCountsResult_t(DownloadClanActivityCountsResult_t.PackSmall d)
			{
				return new DownloadClanActivityCountsResult_t()
				{
					Success = d.Success
				};
			}
		}
	}
}