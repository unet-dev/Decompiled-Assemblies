using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct SteamInventoryFullUpdate_t
	{
		internal const int CallbackId = 4701;

		internal int Handle;

		internal static SteamInventoryFullUpdate_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (SteamInventoryFullUpdate_t)Marshal.PtrToStructure(p, typeof(SteamInventoryFullUpdate_t));
			}
			return (SteamInventoryFullUpdate_t.PackSmall)Marshal.PtrToStructure(p, typeof(SteamInventoryFullUpdate_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return SteamInventoryFullUpdate_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return SteamInventoryFullUpdate_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			SteamInventoryFullUpdate_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			SteamInventoryFullUpdate_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			SteamInventoryFullUpdate_t steamInventoryFullUpdateT = SteamInventoryFullUpdate_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<SteamInventoryFullUpdate_t>(steamInventoryFullUpdateT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<SteamInventoryFullUpdate_t>(steamInventoryFullUpdateT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			SteamInventoryFullUpdate_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(SteamInventoryFullUpdate_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(SteamInventoryFullUpdate_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(SteamInventoryFullUpdate_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(SteamInventoryFullUpdate_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(SteamInventoryFullUpdate_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(SteamInventoryFullUpdate_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(SteamInventoryFullUpdate_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(SteamInventoryFullUpdate_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(SteamInventoryFullUpdate_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(SteamInventoryFullUpdate_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(SteamInventoryFullUpdate_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(SteamInventoryFullUpdate_t.OnGetSize)
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
				CallbackId = 4701
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 4701);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(SteamInventoryFullUpdate_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(SteamInventoryFullUpdate_t));
		}

		internal struct PackSmall
		{
			internal int Handle;

			public static implicit operator SteamInventoryFullUpdate_t(SteamInventoryFullUpdate_t.PackSmall d)
			{
				return new SteamInventoryFullUpdate_t()
				{
					Handle = d.Handle
				};
			}
		}
	}
}