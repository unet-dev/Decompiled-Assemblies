using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct RemoteStorageUpdateUserPublishedItemVoteResult_t
	{
		internal const int CallbackId = 1324;

		internal SteamNative.Result Result;

		internal ulong PublishedFileId;

		internal static CallResult<RemoteStorageUpdateUserPublishedItemVoteResult_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<RemoteStorageUpdateUserPublishedItemVoteResult_t, bool> CallbackFunction)
		{
			return new CallResult<RemoteStorageUpdateUserPublishedItemVoteResult_t>(steamworks, call, CallbackFunction, new CallResult<RemoteStorageUpdateUserPublishedItemVoteResult_t>.ConvertFromPointer(RemoteStorageUpdateUserPublishedItemVoteResult_t.FromPointer), RemoteStorageUpdateUserPublishedItemVoteResult_t.StructSize(), 1324);
		}

		internal static RemoteStorageUpdateUserPublishedItemVoteResult_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (RemoteStorageUpdateUserPublishedItemVoteResult_t)Marshal.PtrToStructure(p, typeof(RemoteStorageUpdateUserPublishedItemVoteResult_t));
			}
			return (RemoteStorageUpdateUserPublishedItemVoteResult_t.PackSmall)Marshal.PtrToStructure(p, typeof(RemoteStorageUpdateUserPublishedItemVoteResult_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return RemoteStorageUpdateUserPublishedItemVoteResult_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return RemoteStorageUpdateUserPublishedItemVoteResult_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			RemoteStorageUpdateUserPublishedItemVoteResult_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			RemoteStorageUpdateUserPublishedItemVoteResult_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			RemoteStorageUpdateUserPublishedItemVoteResult_t remoteStorageUpdateUserPublishedItemVoteResultT = RemoteStorageUpdateUserPublishedItemVoteResult_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<RemoteStorageUpdateUserPublishedItemVoteResult_t>(remoteStorageUpdateUserPublishedItemVoteResultT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<RemoteStorageUpdateUserPublishedItemVoteResult_t>(remoteStorageUpdateUserPublishedItemVoteResultT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			RemoteStorageUpdateUserPublishedItemVoteResult_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(RemoteStorageUpdateUserPublishedItemVoteResult_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(RemoteStorageUpdateUserPublishedItemVoteResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(RemoteStorageUpdateUserPublishedItemVoteResult_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(RemoteStorageUpdateUserPublishedItemVoteResult_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(RemoteStorageUpdateUserPublishedItemVoteResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(RemoteStorageUpdateUserPublishedItemVoteResult_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(RemoteStorageUpdateUserPublishedItemVoteResult_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(RemoteStorageUpdateUserPublishedItemVoteResult_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(RemoteStorageUpdateUserPublishedItemVoteResult_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(RemoteStorageUpdateUserPublishedItemVoteResult_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(RemoteStorageUpdateUserPublishedItemVoteResult_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(RemoteStorageUpdateUserPublishedItemVoteResult_t.OnGetSize)
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
				CallbackId = 1324
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 1324);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(RemoteStorageUpdateUserPublishedItemVoteResult_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(RemoteStorageUpdateUserPublishedItemVoteResult_t));
		}

		internal struct PackSmall
		{
			internal SteamNative.Result Result;

			internal ulong PublishedFileId;

			public static implicit operator RemoteStorageUpdateUserPublishedItemVoteResult_t(RemoteStorageUpdateUserPublishedItemVoteResult_t.PackSmall d)
			{
				RemoteStorageUpdateUserPublishedItemVoteResult_t remoteStorageUpdateUserPublishedItemVoteResultT = new RemoteStorageUpdateUserPublishedItemVoteResult_t()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId
				};
				return remoteStorageUpdateUserPublishedItemVoteResultT;
			}
		}
	}
}