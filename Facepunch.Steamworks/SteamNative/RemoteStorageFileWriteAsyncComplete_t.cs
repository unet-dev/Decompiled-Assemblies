using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct RemoteStorageFileWriteAsyncComplete_t
	{
		internal const int CallbackId = 1331;

		internal SteamNative.Result Result;

		internal static CallResult<RemoteStorageFileWriteAsyncComplete_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<RemoteStorageFileWriteAsyncComplete_t, bool> CallbackFunction)
		{
			return new CallResult<RemoteStorageFileWriteAsyncComplete_t>(steamworks, call, CallbackFunction, new CallResult<RemoteStorageFileWriteAsyncComplete_t>.ConvertFromPointer(RemoteStorageFileWriteAsyncComplete_t.FromPointer), RemoteStorageFileWriteAsyncComplete_t.StructSize(), 1331);
		}

		internal static RemoteStorageFileWriteAsyncComplete_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (RemoteStorageFileWriteAsyncComplete_t)Marshal.PtrToStructure(p, typeof(RemoteStorageFileWriteAsyncComplete_t));
			}
			return (RemoteStorageFileWriteAsyncComplete_t.PackSmall)Marshal.PtrToStructure(p, typeof(RemoteStorageFileWriteAsyncComplete_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return RemoteStorageFileWriteAsyncComplete_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return RemoteStorageFileWriteAsyncComplete_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			RemoteStorageFileWriteAsyncComplete_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			RemoteStorageFileWriteAsyncComplete_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			RemoteStorageFileWriteAsyncComplete_t remoteStorageFileWriteAsyncCompleteT = RemoteStorageFileWriteAsyncComplete_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<RemoteStorageFileWriteAsyncComplete_t>(remoteStorageFileWriteAsyncCompleteT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<RemoteStorageFileWriteAsyncComplete_t>(remoteStorageFileWriteAsyncCompleteT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			RemoteStorageFileWriteAsyncComplete_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(RemoteStorageFileWriteAsyncComplete_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(RemoteStorageFileWriteAsyncComplete_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(RemoteStorageFileWriteAsyncComplete_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(RemoteStorageFileWriteAsyncComplete_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(RemoteStorageFileWriteAsyncComplete_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(RemoteStorageFileWriteAsyncComplete_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(RemoteStorageFileWriteAsyncComplete_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(RemoteStorageFileWriteAsyncComplete_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(RemoteStorageFileWriteAsyncComplete_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(RemoteStorageFileWriteAsyncComplete_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(RemoteStorageFileWriteAsyncComplete_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(RemoteStorageFileWriteAsyncComplete_t.OnGetSize)
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
				CallbackId = 1331
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 1331);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(RemoteStorageFileWriteAsyncComplete_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(RemoteStorageFileWriteAsyncComplete_t));
		}

		internal struct PackSmall
		{
			internal SteamNative.Result Result;

			public static implicit operator RemoteStorageFileWriteAsyncComplete_t(RemoteStorageFileWriteAsyncComplete_t.PackSmall d)
			{
				return new RemoteStorageFileWriteAsyncComplete_t()
				{
					Result = d.Result
				};
			}
		}
	}
}