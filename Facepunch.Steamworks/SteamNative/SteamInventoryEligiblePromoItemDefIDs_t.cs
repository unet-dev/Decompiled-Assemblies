using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct SteamInventoryEligiblePromoItemDefIDs_t
	{
		internal const int CallbackId = 4703;

		internal SteamNative.Result Result;

		internal ulong SteamID;

		internal int UmEligiblePromoItemDefs;

		internal bool CachedData;

		internal static CallResult<SteamInventoryEligiblePromoItemDefIDs_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<SteamInventoryEligiblePromoItemDefIDs_t, bool> CallbackFunction)
		{
			return new CallResult<SteamInventoryEligiblePromoItemDefIDs_t>(steamworks, call, CallbackFunction, new CallResult<SteamInventoryEligiblePromoItemDefIDs_t>.ConvertFromPointer(SteamInventoryEligiblePromoItemDefIDs_t.FromPointer), SteamInventoryEligiblePromoItemDefIDs_t.StructSize(), 4703);
		}

		internal static SteamInventoryEligiblePromoItemDefIDs_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (SteamInventoryEligiblePromoItemDefIDs_t)Marshal.PtrToStructure(p, typeof(SteamInventoryEligiblePromoItemDefIDs_t));
			}
			return (SteamInventoryEligiblePromoItemDefIDs_t.PackSmall)Marshal.PtrToStructure(p, typeof(SteamInventoryEligiblePromoItemDefIDs_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return SteamInventoryEligiblePromoItemDefIDs_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return SteamInventoryEligiblePromoItemDefIDs_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			SteamInventoryEligiblePromoItemDefIDs_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			SteamInventoryEligiblePromoItemDefIDs_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			SteamInventoryEligiblePromoItemDefIDs_t steamInventoryEligiblePromoItemDefIDsT = SteamInventoryEligiblePromoItemDefIDs_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<SteamInventoryEligiblePromoItemDefIDs_t>(steamInventoryEligiblePromoItemDefIDsT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<SteamInventoryEligiblePromoItemDefIDs_t>(steamInventoryEligiblePromoItemDefIDsT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			SteamInventoryEligiblePromoItemDefIDs_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(SteamInventoryEligiblePromoItemDefIDs_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(SteamInventoryEligiblePromoItemDefIDs_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(SteamInventoryEligiblePromoItemDefIDs_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(SteamInventoryEligiblePromoItemDefIDs_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(SteamInventoryEligiblePromoItemDefIDs_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(SteamInventoryEligiblePromoItemDefIDs_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(SteamInventoryEligiblePromoItemDefIDs_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(SteamInventoryEligiblePromoItemDefIDs_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(SteamInventoryEligiblePromoItemDefIDs_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(SteamInventoryEligiblePromoItemDefIDs_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(SteamInventoryEligiblePromoItemDefIDs_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(SteamInventoryEligiblePromoItemDefIDs_t.OnGetSize)
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
				CallbackId = 4703
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 4703);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(SteamInventoryEligiblePromoItemDefIDs_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(SteamInventoryEligiblePromoItemDefIDs_t));
		}

		internal struct PackSmall
		{
			internal SteamNative.Result Result;

			internal ulong SteamID;

			internal int UmEligiblePromoItemDefs;

			internal bool CachedData;

			public static implicit operator SteamInventoryEligiblePromoItemDefIDs_t(SteamInventoryEligiblePromoItemDefIDs_t.PackSmall d)
			{
				SteamInventoryEligiblePromoItemDefIDs_t steamInventoryEligiblePromoItemDefIDsT = new SteamInventoryEligiblePromoItemDefIDs_t()
				{
					Result = d.Result,
					SteamID = d.SteamID,
					UmEligiblePromoItemDefs = d.UmEligiblePromoItemDefs,
					CachedData = d.CachedData
				};
				return steamInventoryEligiblePromoItemDefIDsT;
			}
		}
	}
}