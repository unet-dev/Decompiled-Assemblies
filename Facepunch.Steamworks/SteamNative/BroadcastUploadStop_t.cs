using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct BroadcastUploadStop_t
	{
		internal const int CallbackId = 4605;

		internal BroadcastUploadResult Result;

		internal static BroadcastUploadStop_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (BroadcastUploadStop_t)Marshal.PtrToStructure(p, typeof(BroadcastUploadStop_t));
			}
			return (BroadcastUploadStop_t.PackSmall)Marshal.PtrToStructure(p, typeof(BroadcastUploadStop_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return BroadcastUploadStop_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return BroadcastUploadStop_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			BroadcastUploadStop_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			BroadcastUploadStop_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			BroadcastUploadStop_t broadcastUploadStopT = BroadcastUploadStop_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<BroadcastUploadStop_t>(broadcastUploadStopT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<BroadcastUploadStop_t>(broadcastUploadStopT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			BroadcastUploadStop_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(BroadcastUploadStop_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(BroadcastUploadStop_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(BroadcastUploadStop_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(BroadcastUploadStop_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(BroadcastUploadStop_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(BroadcastUploadStop_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(BroadcastUploadStop_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(BroadcastUploadStop_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(BroadcastUploadStop_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(BroadcastUploadStop_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(BroadcastUploadStop_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(BroadcastUploadStop_t.OnGetSize)
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
				CallbackId = 4605
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 4605);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(BroadcastUploadStop_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(BroadcastUploadStop_t));
		}

		internal struct PackSmall
		{
			internal BroadcastUploadResult Result;

			public static implicit operator BroadcastUploadStop_t(BroadcastUploadStop_t.PackSmall d)
			{
				return new BroadcastUploadStop_t()
				{
					Result = d.Result
				};
			}
		}
	}
}