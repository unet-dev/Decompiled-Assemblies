using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct RemoteStorageEnumeratePublishedFilesByUserActionResult_t
	{
		internal const int CallbackId = 1328;

		internal SteamNative.Result Result;

		internal WorkshopFileAction Action;

		internal int ResultsReturned;

		internal int TotalResultCount;

		internal ulong[] GPublishedFileId;

		internal uint[] GRTimeUpdated;

		internal static CallResult<RemoteStorageEnumeratePublishedFilesByUserActionResult_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<RemoteStorageEnumeratePublishedFilesByUserActionResult_t, bool> CallbackFunction)
		{
			return new CallResult<RemoteStorageEnumeratePublishedFilesByUserActionResult_t>(steamworks, call, CallbackFunction, new CallResult<RemoteStorageEnumeratePublishedFilesByUserActionResult_t>.ConvertFromPointer(RemoteStorageEnumeratePublishedFilesByUserActionResult_t.FromPointer), RemoteStorageEnumeratePublishedFilesByUserActionResult_t.StructSize(), 1328);
		}

		internal static RemoteStorageEnumeratePublishedFilesByUserActionResult_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (RemoteStorageEnumeratePublishedFilesByUserActionResult_t)Marshal.PtrToStructure(p, typeof(RemoteStorageEnumeratePublishedFilesByUserActionResult_t));
			}
			return (RemoteStorageEnumeratePublishedFilesByUserActionResult_t.PackSmall)Marshal.PtrToStructure(p, typeof(RemoteStorageEnumeratePublishedFilesByUserActionResult_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return RemoteStorageEnumeratePublishedFilesByUserActionResult_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return RemoteStorageEnumeratePublishedFilesByUserActionResult_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			RemoteStorageEnumeratePublishedFilesByUserActionResult_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			RemoteStorageEnumeratePublishedFilesByUserActionResult_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			RemoteStorageEnumeratePublishedFilesByUserActionResult_t remoteStorageEnumeratePublishedFilesByUserActionResultT = RemoteStorageEnumeratePublishedFilesByUserActionResult_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<RemoteStorageEnumeratePublishedFilesByUserActionResult_t>(remoteStorageEnumeratePublishedFilesByUserActionResultT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<RemoteStorageEnumeratePublishedFilesByUserActionResult_t>(remoteStorageEnumeratePublishedFilesByUserActionResultT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			RemoteStorageEnumeratePublishedFilesByUserActionResult_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(RemoteStorageEnumeratePublishedFilesByUserActionResult_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(RemoteStorageEnumeratePublishedFilesByUserActionResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(RemoteStorageEnumeratePublishedFilesByUserActionResult_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(RemoteStorageEnumeratePublishedFilesByUserActionResult_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(RemoteStorageEnumeratePublishedFilesByUserActionResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(RemoteStorageEnumeratePublishedFilesByUserActionResult_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(RemoteStorageEnumeratePublishedFilesByUserActionResult_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(RemoteStorageEnumeratePublishedFilesByUserActionResult_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(RemoteStorageEnumeratePublishedFilesByUserActionResult_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(RemoteStorageEnumeratePublishedFilesByUserActionResult_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(RemoteStorageEnumeratePublishedFilesByUserActionResult_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(RemoteStorageEnumeratePublishedFilesByUserActionResult_t.OnGetSize)
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
				CallbackId = 1328
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 1328);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(RemoteStorageEnumeratePublishedFilesByUserActionResult_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(RemoteStorageEnumeratePublishedFilesByUserActionResult_t));
		}

		internal struct PackSmall
		{
			internal SteamNative.Result Result;

			internal WorkshopFileAction Action;

			internal int ResultsReturned;

			internal int TotalResultCount;

			internal ulong[] GPublishedFileId;

			internal uint[] GRTimeUpdated;

			public static implicit operator RemoteStorageEnumeratePublishedFilesByUserActionResult_t(RemoteStorageEnumeratePublishedFilesByUserActionResult_t.PackSmall d)
			{
				RemoteStorageEnumeratePublishedFilesByUserActionResult_t remoteStorageEnumeratePublishedFilesByUserActionResultT = new RemoteStorageEnumeratePublishedFilesByUserActionResult_t()
				{
					Result = d.Result,
					Action = d.Action,
					ResultsReturned = d.ResultsReturned,
					TotalResultCount = d.TotalResultCount,
					GPublishedFileId = d.GPublishedFileId,
					GRTimeUpdated = d.GRTimeUpdated
				};
				return remoteStorageEnumeratePublishedFilesByUserActionResultT;
			}
		}
	}
}