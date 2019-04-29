using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct ItemInstalled_t
	{
		internal const int CallbackId = 3405;

		internal uint AppID;

		internal ulong PublishedFileId;

		internal static ItemInstalled_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (ItemInstalled_t)Marshal.PtrToStructure(p, typeof(ItemInstalled_t));
			}
			return (ItemInstalled_t.PackSmall)Marshal.PtrToStructure(p, typeof(ItemInstalled_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return ItemInstalled_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return ItemInstalled_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			ItemInstalled_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			ItemInstalled_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			ItemInstalled_t itemInstalledT = ItemInstalled_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<ItemInstalled_t>(itemInstalledT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<ItemInstalled_t>(itemInstalledT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			ItemInstalled_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(ItemInstalled_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(ItemInstalled_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(ItemInstalled_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(ItemInstalled_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(ItemInstalled_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(ItemInstalled_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(ItemInstalled_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(ItemInstalled_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(ItemInstalled_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(ItemInstalled_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(ItemInstalled_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(ItemInstalled_t.OnGetSize)
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
				CallbackId = 3405
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 3405);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(ItemInstalled_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(ItemInstalled_t));
		}

		internal struct PackSmall
		{
			internal uint AppID;

			internal ulong PublishedFileId;

			public static implicit operator ItemInstalled_t(ItemInstalled_t.PackSmall d)
			{
				ItemInstalled_t itemInstalledT = new ItemInstalled_t()
				{
					AppID = d.AppID,
					PublishedFileId = d.PublishedFileId
				};
				return itemInstalledT;
			}
		}
	}
}