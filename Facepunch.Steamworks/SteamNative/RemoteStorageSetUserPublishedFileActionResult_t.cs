using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct RemoteStorageSetUserPublishedFileActionResult_t
	{
		internal const int CallbackId = 1327;

		internal SteamNative.Result Result;

		internal ulong PublishedFileId;

		internal WorkshopFileAction Action;

		internal static CallResult<RemoteStorageSetUserPublishedFileActionResult_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<RemoteStorageSetUserPublishedFileActionResult_t, bool> CallbackFunction)
		{
			return new CallResult<RemoteStorageSetUserPublishedFileActionResult_t>(steamworks, call, CallbackFunction, new CallResult<RemoteStorageSetUserPublishedFileActionResult_t>.ConvertFromPointer(RemoteStorageSetUserPublishedFileActionResult_t.FromPointer), RemoteStorageSetUserPublishedFileActionResult_t.StructSize(), 1327);
		}

		internal static RemoteStorageSetUserPublishedFileActionResult_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (RemoteStorageSetUserPublishedFileActionResult_t)Marshal.PtrToStructure(p, typeof(RemoteStorageSetUserPublishedFileActionResult_t));
			}
			return (RemoteStorageSetUserPublishedFileActionResult_t.PackSmall)Marshal.PtrToStructure(p, typeof(RemoteStorageSetUserPublishedFileActionResult_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return RemoteStorageSetUserPublishedFileActionResult_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return RemoteStorageSetUserPublishedFileActionResult_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			RemoteStorageSetUserPublishedFileActionResult_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			RemoteStorageSetUserPublishedFileActionResult_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			RemoteStorageSetUserPublishedFileActionResult_t remoteStorageSetUserPublishedFileActionResultT = RemoteStorageSetUserPublishedFileActionResult_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<RemoteStorageSetUserPublishedFileActionResult_t>(remoteStorageSetUserPublishedFileActionResultT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<RemoteStorageSetUserPublishedFileActionResult_t>(remoteStorageSetUserPublishedFileActionResultT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			RemoteStorageSetUserPublishedFileActionResult_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(RemoteStorageSetUserPublishedFileActionResult_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(RemoteStorageSetUserPublishedFileActionResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(RemoteStorageSetUserPublishedFileActionResult_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(RemoteStorageSetUserPublishedFileActionResult_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(RemoteStorageSetUserPublishedFileActionResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(RemoteStorageSetUserPublishedFileActionResult_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(RemoteStorageSetUserPublishedFileActionResult_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(RemoteStorageSetUserPublishedFileActionResult_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(RemoteStorageSetUserPublishedFileActionResult_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(RemoteStorageSetUserPublishedFileActionResult_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(RemoteStorageSetUserPublishedFileActionResult_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(RemoteStorageSetUserPublishedFileActionResult_t.OnGetSize)
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
				CallbackId = 1327
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 1327);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(RemoteStorageSetUserPublishedFileActionResult_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(RemoteStorageSetUserPublishedFileActionResult_t));
		}

		internal struct PackSmall
		{
			internal SteamNative.Result Result;

			internal ulong PublishedFileId;

			internal WorkshopFileAction Action;

			public static implicit operator RemoteStorageSetUserPublishedFileActionResult_t(RemoteStorageSetUserPublishedFileActionResult_t.PackSmall d)
			{
				RemoteStorageSetUserPublishedFileActionResult_t remoteStorageSetUserPublishedFileActionResultT = new RemoteStorageSetUserPublishedFileActionResult_t()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId,
					Action = d.Action
				};
				return remoteStorageSetUserPublishedFileActionResultT;
			}
		}
	}
}