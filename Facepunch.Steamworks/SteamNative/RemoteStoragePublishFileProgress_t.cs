using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct RemoteStoragePublishFileProgress_t
	{
		internal const int CallbackId = 1329;

		internal double DPercentFile;

		internal bool Preview;

		internal static CallResult<RemoteStoragePublishFileProgress_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<RemoteStoragePublishFileProgress_t, bool> CallbackFunction)
		{
			return new CallResult<RemoteStoragePublishFileProgress_t>(steamworks, call, CallbackFunction, new CallResult<RemoteStoragePublishFileProgress_t>.ConvertFromPointer(RemoteStoragePublishFileProgress_t.FromPointer), RemoteStoragePublishFileProgress_t.StructSize(), 1329);
		}

		internal static RemoteStoragePublishFileProgress_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (RemoteStoragePublishFileProgress_t)Marshal.PtrToStructure(p, typeof(RemoteStoragePublishFileProgress_t));
			}
			return (RemoteStoragePublishFileProgress_t.PackSmall)Marshal.PtrToStructure(p, typeof(RemoteStoragePublishFileProgress_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return RemoteStoragePublishFileProgress_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return RemoteStoragePublishFileProgress_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			RemoteStoragePublishFileProgress_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			RemoteStoragePublishFileProgress_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			RemoteStoragePublishFileProgress_t remoteStoragePublishFileProgressT = RemoteStoragePublishFileProgress_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<RemoteStoragePublishFileProgress_t>(remoteStoragePublishFileProgressT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<RemoteStoragePublishFileProgress_t>(remoteStoragePublishFileProgressT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			RemoteStoragePublishFileProgress_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(RemoteStoragePublishFileProgress_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(RemoteStoragePublishFileProgress_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(RemoteStoragePublishFileProgress_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(RemoteStoragePublishFileProgress_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(RemoteStoragePublishFileProgress_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(RemoteStoragePublishFileProgress_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(RemoteStoragePublishFileProgress_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(RemoteStoragePublishFileProgress_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(RemoteStoragePublishFileProgress_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(RemoteStoragePublishFileProgress_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(RemoteStoragePublishFileProgress_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(RemoteStoragePublishFileProgress_t.OnGetSize)
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
				CallbackId = 1329
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 1329);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(RemoteStoragePublishFileProgress_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(RemoteStoragePublishFileProgress_t));
		}

		internal struct PackSmall
		{
			internal double DPercentFile;

			internal bool Preview;

			public static implicit operator RemoteStoragePublishFileProgress_t(RemoteStoragePublishFileProgress_t.PackSmall d)
			{
				RemoteStoragePublishFileProgress_t remoteStoragePublishFileProgressT = new RemoteStoragePublishFileProgress_t()
				{
					DPercentFile = d.DPercentFile,
					Preview = d.Preview
				};
				return remoteStoragePublishFileProgressT;
			}
		}
	}
}