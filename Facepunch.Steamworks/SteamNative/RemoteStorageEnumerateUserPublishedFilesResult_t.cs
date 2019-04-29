using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct RemoteStorageEnumerateUserPublishedFilesResult_t
	{
		internal const int CallbackId = 1312;

		internal SteamNative.Result Result;

		internal int ResultsReturned;

		internal int TotalResultCount;

		internal ulong[] GPublishedFileId;

		internal static CallResult<RemoteStorageEnumerateUserPublishedFilesResult_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<RemoteStorageEnumerateUserPublishedFilesResult_t, bool> CallbackFunction)
		{
			return new CallResult<RemoteStorageEnumerateUserPublishedFilesResult_t>(steamworks, call, CallbackFunction, new CallResult<RemoteStorageEnumerateUserPublishedFilesResult_t>.ConvertFromPointer(RemoteStorageEnumerateUserPublishedFilesResult_t.FromPointer), RemoteStorageEnumerateUserPublishedFilesResult_t.StructSize(), 1312);
		}

		internal static RemoteStorageEnumerateUserPublishedFilesResult_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (RemoteStorageEnumerateUserPublishedFilesResult_t)Marshal.PtrToStructure(p, typeof(RemoteStorageEnumerateUserPublishedFilesResult_t));
			}
			return (RemoteStorageEnumerateUserPublishedFilesResult_t.PackSmall)Marshal.PtrToStructure(p, typeof(RemoteStorageEnumerateUserPublishedFilesResult_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return RemoteStorageEnumerateUserPublishedFilesResult_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return RemoteStorageEnumerateUserPublishedFilesResult_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			RemoteStorageEnumerateUserPublishedFilesResult_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			RemoteStorageEnumerateUserPublishedFilesResult_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			RemoteStorageEnumerateUserPublishedFilesResult_t remoteStorageEnumerateUserPublishedFilesResultT = RemoteStorageEnumerateUserPublishedFilesResult_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<RemoteStorageEnumerateUserPublishedFilesResult_t>(remoteStorageEnumerateUserPublishedFilesResultT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<RemoteStorageEnumerateUserPublishedFilesResult_t>(remoteStorageEnumerateUserPublishedFilesResultT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			RemoteStorageEnumerateUserPublishedFilesResult_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(RemoteStorageEnumerateUserPublishedFilesResult_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(RemoteStorageEnumerateUserPublishedFilesResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(RemoteStorageEnumerateUserPublishedFilesResult_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(RemoteStorageEnumerateUserPublishedFilesResult_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(RemoteStorageEnumerateUserPublishedFilesResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(RemoteStorageEnumerateUserPublishedFilesResult_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(RemoteStorageEnumerateUserPublishedFilesResult_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(RemoteStorageEnumerateUserPublishedFilesResult_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(RemoteStorageEnumerateUserPublishedFilesResult_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(RemoteStorageEnumerateUserPublishedFilesResult_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(RemoteStorageEnumerateUserPublishedFilesResult_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(RemoteStorageEnumerateUserPublishedFilesResult_t.OnGetSize)
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
				CallbackId = 1312
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 1312);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(RemoteStorageEnumerateUserPublishedFilesResult_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(RemoteStorageEnumerateUserPublishedFilesResult_t));
		}

		internal struct PackSmall
		{
			internal SteamNative.Result Result;

			internal int ResultsReturned;

			internal int TotalResultCount;

			internal ulong[] GPublishedFileId;

			public static implicit operator RemoteStorageEnumerateUserPublishedFilesResult_t(RemoteStorageEnumerateUserPublishedFilesResult_t.PackSmall d)
			{
				RemoteStorageEnumerateUserPublishedFilesResult_t remoteStorageEnumerateUserPublishedFilesResultT = new RemoteStorageEnumerateUserPublishedFilesResult_t()
				{
					Result = d.Result,
					ResultsReturned = d.ResultsReturned,
					TotalResultCount = d.TotalResultCount,
					GPublishedFileId = d.GPublishedFileId
				};
				return remoteStorageEnumerateUserPublishedFilesResultT;
			}
		}
	}
}