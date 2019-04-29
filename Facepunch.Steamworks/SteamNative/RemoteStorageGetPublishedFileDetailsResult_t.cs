using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct RemoteStorageGetPublishedFileDetailsResult_t
	{
		internal const int CallbackId = 1318;

		internal SteamNative.Result Result;

		internal ulong PublishedFileId;

		internal uint CreatorAppID;

		internal uint ConsumerAppID;

		internal string Title;

		internal string Description;

		internal ulong File;

		internal ulong PreviewFile;

		internal ulong SteamIDOwner;

		internal uint TimeCreated;

		internal uint TimeUpdated;

		internal RemoteStoragePublishedFileVisibility Visibility;

		internal bool Banned;

		internal string Tags;

		internal bool TagsTruncated;

		internal string PchFileName;

		internal int FileSize;

		internal int PreviewFileSize;

		internal string URL;

		internal WorkshopFileType FileType;

		internal bool AcceptedForUse;

		internal static CallResult<RemoteStorageGetPublishedFileDetailsResult_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<RemoteStorageGetPublishedFileDetailsResult_t, bool> CallbackFunction)
		{
			return new CallResult<RemoteStorageGetPublishedFileDetailsResult_t>(steamworks, call, CallbackFunction, new CallResult<RemoteStorageGetPublishedFileDetailsResult_t>.ConvertFromPointer(RemoteStorageGetPublishedFileDetailsResult_t.FromPointer), RemoteStorageGetPublishedFileDetailsResult_t.StructSize(), 1318);
		}

		internal static RemoteStorageGetPublishedFileDetailsResult_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (RemoteStorageGetPublishedFileDetailsResult_t)Marshal.PtrToStructure(p, typeof(RemoteStorageGetPublishedFileDetailsResult_t));
			}
			return (RemoteStorageGetPublishedFileDetailsResult_t.PackSmall)Marshal.PtrToStructure(p, typeof(RemoteStorageGetPublishedFileDetailsResult_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return RemoteStorageGetPublishedFileDetailsResult_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return RemoteStorageGetPublishedFileDetailsResult_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			RemoteStorageGetPublishedFileDetailsResult_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			RemoteStorageGetPublishedFileDetailsResult_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			RemoteStorageGetPublishedFileDetailsResult_t remoteStorageGetPublishedFileDetailsResultT = RemoteStorageGetPublishedFileDetailsResult_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<RemoteStorageGetPublishedFileDetailsResult_t>(remoteStorageGetPublishedFileDetailsResultT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<RemoteStorageGetPublishedFileDetailsResult_t>(remoteStorageGetPublishedFileDetailsResultT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			RemoteStorageGetPublishedFileDetailsResult_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(RemoteStorageGetPublishedFileDetailsResult_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(RemoteStorageGetPublishedFileDetailsResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(RemoteStorageGetPublishedFileDetailsResult_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(RemoteStorageGetPublishedFileDetailsResult_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(RemoteStorageGetPublishedFileDetailsResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(RemoteStorageGetPublishedFileDetailsResult_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(RemoteStorageGetPublishedFileDetailsResult_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(RemoteStorageGetPublishedFileDetailsResult_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(RemoteStorageGetPublishedFileDetailsResult_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(RemoteStorageGetPublishedFileDetailsResult_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(RemoteStorageGetPublishedFileDetailsResult_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(RemoteStorageGetPublishedFileDetailsResult_t.OnGetSize)
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
				CallbackId = 1318
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 1318);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(RemoteStorageGetPublishedFileDetailsResult_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(RemoteStorageGetPublishedFileDetailsResult_t));
		}

		internal struct PackSmall
		{
			internal SteamNative.Result Result;

			internal ulong PublishedFileId;

			internal uint CreatorAppID;

			internal uint ConsumerAppID;

			internal string Title;

			internal string Description;

			internal ulong File;

			internal ulong PreviewFile;

			internal ulong SteamIDOwner;

			internal uint TimeCreated;

			internal uint TimeUpdated;

			internal RemoteStoragePublishedFileVisibility Visibility;

			internal bool Banned;

			internal string Tags;

			internal bool TagsTruncated;

			internal string PchFileName;

			internal int FileSize;

			internal int PreviewFileSize;

			internal string URL;

			internal WorkshopFileType FileType;

			internal bool AcceptedForUse;

			public static implicit operator RemoteStorageGetPublishedFileDetailsResult_t(RemoteStorageGetPublishedFileDetailsResult_t.PackSmall d)
			{
				RemoteStorageGetPublishedFileDetailsResult_t remoteStorageGetPublishedFileDetailsResultT = new RemoteStorageGetPublishedFileDetailsResult_t()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId,
					CreatorAppID = d.CreatorAppID,
					ConsumerAppID = d.ConsumerAppID,
					Title = d.Title,
					Description = d.Description,
					File = d.File,
					PreviewFile = d.PreviewFile,
					SteamIDOwner = d.SteamIDOwner,
					TimeCreated = d.TimeCreated,
					TimeUpdated = d.TimeUpdated,
					Visibility = d.Visibility,
					Banned = d.Banned,
					Tags = d.Tags,
					TagsTruncated = d.TagsTruncated,
					PchFileName = d.PchFileName,
					FileSize = d.FileSize,
					PreviewFileSize = d.PreviewFileSize,
					URL = d.URL,
					FileType = d.FileType,
					AcceptedForUse = d.AcceptedForUse
				};
				return remoteStorageGetPublishedFileDetailsResultT;
			}
		}
	}
}