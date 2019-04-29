using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct RemoteStorageEnumerateUserSubscribedFilesResult_t
	{
		internal const int CallbackId = 1314;

		internal SteamNative.Result Result;

		internal int ResultsReturned;

		internal int TotalResultCount;

		internal ulong[] GPublishedFileId;

		internal uint[] GRTimeSubscribed;

		internal static CallResult<RemoteStorageEnumerateUserSubscribedFilesResult_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<RemoteStorageEnumerateUserSubscribedFilesResult_t, bool> CallbackFunction)
		{
			return new CallResult<RemoteStorageEnumerateUserSubscribedFilesResult_t>(steamworks, call, CallbackFunction, new CallResult<RemoteStorageEnumerateUserSubscribedFilesResult_t>.ConvertFromPointer(RemoteStorageEnumerateUserSubscribedFilesResult_t.FromPointer), RemoteStorageEnumerateUserSubscribedFilesResult_t.StructSize(), 1314);
		}

		internal static RemoteStorageEnumerateUserSubscribedFilesResult_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (RemoteStorageEnumerateUserSubscribedFilesResult_t)Marshal.PtrToStructure(p, typeof(RemoteStorageEnumerateUserSubscribedFilesResult_t));
			}
			return (RemoteStorageEnumerateUserSubscribedFilesResult_t.PackSmall)Marshal.PtrToStructure(p, typeof(RemoteStorageEnumerateUserSubscribedFilesResult_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return RemoteStorageEnumerateUserSubscribedFilesResult_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return RemoteStorageEnumerateUserSubscribedFilesResult_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			RemoteStorageEnumerateUserSubscribedFilesResult_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			RemoteStorageEnumerateUserSubscribedFilesResult_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			RemoteStorageEnumerateUserSubscribedFilesResult_t remoteStorageEnumerateUserSubscribedFilesResultT = RemoteStorageEnumerateUserSubscribedFilesResult_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<RemoteStorageEnumerateUserSubscribedFilesResult_t>(remoteStorageEnumerateUserSubscribedFilesResultT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<RemoteStorageEnumerateUserSubscribedFilesResult_t>(remoteStorageEnumerateUserSubscribedFilesResultT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			RemoteStorageEnumerateUserSubscribedFilesResult_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(RemoteStorageEnumerateUserSubscribedFilesResult_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(RemoteStorageEnumerateUserSubscribedFilesResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(RemoteStorageEnumerateUserSubscribedFilesResult_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(RemoteStorageEnumerateUserSubscribedFilesResult_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(RemoteStorageEnumerateUserSubscribedFilesResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(RemoteStorageEnumerateUserSubscribedFilesResult_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(RemoteStorageEnumerateUserSubscribedFilesResult_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(RemoteStorageEnumerateUserSubscribedFilesResult_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(RemoteStorageEnumerateUserSubscribedFilesResult_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(RemoteStorageEnumerateUserSubscribedFilesResult_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(RemoteStorageEnumerateUserSubscribedFilesResult_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(RemoteStorageEnumerateUserSubscribedFilesResult_t.OnGetSize)
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
				CallbackId = 1314
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 1314);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(RemoteStorageEnumerateUserSubscribedFilesResult_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(RemoteStorageEnumerateUserSubscribedFilesResult_t));
		}

		internal struct PackSmall
		{
			internal SteamNative.Result Result;

			internal int ResultsReturned;

			internal int TotalResultCount;

			internal ulong[] GPublishedFileId;

			internal uint[] GRTimeSubscribed;

			public static implicit operator RemoteStorageEnumerateUserSubscribedFilesResult_t(RemoteStorageEnumerateUserSubscribedFilesResult_t.PackSmall d)
			{
				RemoteStorageEnumerateUserSubscribedFilesResult_t remoteStorageEnumerateUserSubscribedFilesResultT = new RemoteStorageEnumerateUserSubscribedFilesResult_t()
				{
					Result = d.Result,
					ResultsReturned = d.ResultsReturned,
					TotalResultCount = d.TotalResultCount,
					GPublishedFileId = d.GPublishedFileId,
					GRTimeSubscribed = d.GRTimeSubscribed
				};
				return remoteStorageEnumerateUserSubscribedFilesResultT;
			}
		}
	}
}