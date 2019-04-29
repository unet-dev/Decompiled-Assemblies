using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct RemoteStoragePublishedFileSubscribed_t
	{
		internal const int CallbackId = 1321;

		internal ulong PublishedFileId;

		internal uint AppID;

		internal static RemoteStoragePublishedFileSubscribed_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (RemoteStoragePublishedFileSubscribed_t)Marshal.PtrToStructure(p, typeof(RemoteStoragePublishedFileSubscribed_t));
			}
			return (RemoteStoragePublishedFileSubscribed_t.PackSmall)Marshal.PtrToStructure(p, typeof(RemoteStoragePublishedFileSubscribed_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return RemoteStoragePublishedFileSubscribed_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return RemoteStoragePublishedFileSubscribed_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			RemoteStoragePublishedFileSubscribed_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			RemoteStoragePublishedFileSubscribed_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			RemoteStoragePublishedFileSubscribed_t remoteStoragePublishedFileSubscribedT = RemoteStoragePublishedFileSubscribed_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<RemoteStoragePublishedFileSubscribed_t>(remoteStoragePublishedFileSubscribedT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<RemoteStoragePublishedFileSubscribed_t>(remoteStoragePublishedFileSubscribedT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			RemoteStoragePublishedFileSubscribed_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(RemoteStoragePublishedFileSubscribed_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(RemoteStoragePublishedFileSubscribed_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(RemoteStoragePublishedFileSubscribed_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(RemoteStoragePublishedFileSubscribed_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(RemoteStoragePublishedFileSubscribed_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(RemoteStoragePublishedFileSubscribed_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(RemoteStoragePublishedFileSubscribed_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(RemoteStoragePublishedFileSubscribed_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(RemoteStoragePublishedFileSubscribed_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(RemoteStoragePublishedFileSubscribed_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(RemoteStoragePublishedFileSubscribed_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(RemoteStoragePublishedFileSubscribed_t.OnGetSize)
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
				CallbackId = 1321
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 1321);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(RemoteStoragePublishedFileSubscribed_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(RemoteStoragePublishedFileSubscribed_t));
		}

		internal struct PackSmall
		{
			internal ulong PublishedFileId;

			internal uint AppID;

			public static implicit operator RemoteStoragePublishedFileSubscribed_t(RemoteStoragePublishedFileSubscribed_t.PackSmall d)
			{
				RemoteStoragePublishedFileSubscribed_t remoteStoragePublishedFileSubscribedT = new RemoteStoragePublishedFileSubscribed_t()
				{
					PublishedFileId = d.PublishedFileId,
					AppID = d.AppID
				};
				return remoteStoragePublishedFileSubscribedT;
			}
		}
	}
}