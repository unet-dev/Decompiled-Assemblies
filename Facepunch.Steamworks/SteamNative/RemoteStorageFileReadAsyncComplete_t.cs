using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct RemoteStorageFileReadAsyncComplete_t
	{
		internal const int CallbackId = 1332;

		internal ulong FileReadAsync;

		internal SteamNative.Result Result;

		internal uint Offset;

		internal uint Read;

		internal static CallResult<RemoteStorageFileReadAsyncComplete_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<RemoteStorageFileReadAsyncComplete_t, bool> CallbackFunction)
		{
			return new CallResult<RemoteStorageFileReadAsyncComplete_t>(steamworks, call, CallbackFunction, new CallResult<RemoteStorageFileReadAsyncComplete_t>.ConvertFromPointer(RemoteStorageFileReadAsyncComplete_t.FromPointer), RemoteStorageFileReadAsyncComplete_t.StructSize(), 1332);
		}

		internal static RemoteStorageFileReadAsyncComplete_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (RemoteStorageFileReadAsyncComplete_t)Marshal.PtrToStructure(p, typeof(RemoteStorageFileReadAsyncComplete_t));
			}
			return (RemoteStorageFileReadAsyncComplete_t.PackSmall)Marshal.PtrToStructure(p, typeof(RemoteStorageFileReadAsyncComplete_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return RemoteStorageFileReadAsyncComplete_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return RemoteStorageFileReadAsyncComplete_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			RemoteStorageFileReadAsyncComplete_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			RemoteStorageFileReadAsyncComplete_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			RemoteStorageFileReadAsyncComplete_t remoteStorageFileReadAsyncCompleteT = RemoteStorageFileReadAsyncComplete_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<RemoteStorageFileReadAsyncComplete_t>(remoteStorageFileReadAsyncCompleteT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<RemoteStorageFileReadAsyncComplete_t>(remoteStorageFileReadAsyncCompleteT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			RemoteStorageFileReadAsyncComplete_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(RemoteStorageFileReadAsyncComplete_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(RemoteStorageFileReadAsyncComplete_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(RemoteStorageFileReadAsyncComplete_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(RemoteStorageFileReadAsyncComplete_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(RemoteStorageFileReadAsyncComplete_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(RemoteStorageFileReadAsyncComplete_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(RemoteStorageFileReadAsyncComplete_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(RemoteStorageFileReadAsyncComplete_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(RemoteStorageFileReadAsyncComplete_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(RemoteStorageFileReadAsyncComplete_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(RemoteStorageFileReadAsyncComplete_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(RemoteStorageFileReadAsyncComplete_t.OnGetSize)
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
				CallbackId = 1332
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 1332);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(RemoteStorageFileReadAsyncComplete_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(RemoteStorageFileReadAsyncComplete_t));
		}

		internal struct PackSmall
		{
			internal ulong FileReadAsync;

			internal SteamNative.Result Result;

			internal uint Offset;

			internal uint Read;

			public static implicit operator RemoteStorageFileReadAsyncComplete_t(RemoteStorageFileReadAsyncComplete_t.PackSmall d)
			{
				RemoteStorageFileReadAsyncComplete_t remoteStorageFileReadAsyncCompleteT = new RemoteStorageFileReadAsyncComplete_t()
				{
					FileReadAsync = d.FileReadAsync,
					Result = d.Result,
					Offset = d.Offset,
					Read = d.Read
				};
				return remoteStorageFileReadAsyncCompleteT;
			}
		}
	}
}