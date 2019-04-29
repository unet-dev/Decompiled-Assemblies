using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct RemoteStorageGetPublishedItemVoteDetailsResult_t
	{
		internal const int CallbackId = 1320;

		internal SteamNative.Result Result;

		internal ulong PublishedFileId;

		internal int VotesFor;

		internal int VotesAgainst;

		internal int Reports;

		internal float FScore;

		internal static CallResult<RemoteStorageGetPublishedItemVoteDetailsResult_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<RemoteStorageGetPublishedItemVoteDetailsResult_t, bool> CallbackFunction)
		{
			return new CallResult<RemoteStorageGetPublishedItemVoteDetailsResult_t>(steamworks, call, CallbackFunction, new CallResult<RemoteStorageGetPublishedItemVoteDetailsResult_t>.ConvertFromPointer(RemoteStorageGetPublishedItemVoteDetailsResult_t.FromPointer), RemoteStorageGetPublishedItemVoteDetailsResult_t.StructSize(), 1320);
		}

		internal static RemoteStorageGetPublishedItemVoteDetailsResult_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (RemoteStorageGetPublishedItemVoteDetailsResult_t)Marshal.PtrToStructure(p, typeof(RemoteStorageGetPublishedItemVoteDetailsResult_t));
			}
			return (RemoteStorageGetPublishedItemVoteDetailsResult_t.PackSmall)Marshal.PtrToStructure(p, typeof(RemoteStorageGetPublishedItemVoteDetailsResult_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return RemoteStorageGetPublishedItemVoteDetailsResult_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return RemoteStorageGetPublishedItemVoteDetailsResult_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			RemoteStorageGetPublishedItemVoteDetailsResult_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			RemoteStorageGetPublishedItemVoteDetailsResult_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			RemoteStorageGetPublishedItemVoteDetailsResult_t remoteStorageGetPublishedItemVoteDetailsResultT = RemoteStorageGetPublishedItemVoteDetailsResult_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<RemoteStorageGetPublishedItemVoteDetailsResult_t>(remoteStorageGetPublishedItemVoteDetailsResultT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<RemoteStorageGetPublishedItemVoteDetailsResult_t>(remoteStorageGetPublishedItemVoteDetailsResultT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			RemoteStorageGetPublishedItemVoteDetailsResult_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(RemoteStorageGetPublishedItemVoteDetailsResult_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(RemoteStorageGetPublishedItemVoteDetailsResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(RemoteStorageGetPublishedItemVoteDetailsResult_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(RemoteStorageGetPublishedItemVoteDetailsResult_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(RemoteStorageGetPublishedItemVoteDetailsResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(RemoteStorageGetPublishedItemVoteDetailsResult_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(RemoteStorageGetPublishedItemVoteDetailsResult_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(RemoteStorageGetPublishedItemVoteDetailsResult_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(RemoteStorageGetPublishedItemVoteDetailsResult_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(RemoteStorageGetPublishedItemVoteDetailsResult_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(RemoteStorageGetPublishedItemVoteDetailsResult_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(RemoteStorageGetPublishedItemVoteDetailsResult_t.OnGetSize)
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
				CallbackId = 1320
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 1320);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(RemoteStorageGetPublishedItemVoteDetailsResult_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(RemoteStorageGetPublishedItemVoteDetailsResult_t));
		}

		internal struct PackSmall
		{
			internal SteamNative.Result Result;

			internal ulong PublishedFileId;

			internal int VotesFor;

			internal int VotesAgainst;

			internal int Reports;

			internal float FScore;

			public static implicit operator RemoteStorageGetPublishedItemVoteDetailsResult_t(RemoteStorageGetPublishedItemVoteDetailsResult_t.PackSmall d)
			{
				RemoteStorageGetPublishedItemVoteDetailsResult_t remoteStorageGetPublishedItemVoteDetailsResultT = new RemoteStorageGetPublishedItemVoteDetailsResult_t()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId,
					VotesFor = d.VotesFor,
					VotesAgainst = d.VotesAgainst,
					Reports = d.Reports,
					FScore = d.FScore
				};
				return remoteStorageGetPublishedItemVoteDetailsResultT;
			}
		}
	}
}