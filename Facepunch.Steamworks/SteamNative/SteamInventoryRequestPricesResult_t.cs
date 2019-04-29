using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct SteamInventoryRequestPricesResult_t
	{
		internal const int CallbackId = 4705;

		internal SteamNative.Result Result;

		internal string Currency;

		internal static CallResult<SteamInventoryRequestPricesResult_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<SteamInventoryRequestPricesResult_t, bool> CallbackFunction)
		{
			return new CallResult<SteamInventoryRequestPricesResult_t>(steamworks, call, CallbackFunction, new CallResult<SteamInventoryRequestPricesResult_t>.ConvertFromPointer(SteamInventoryRequestPricesResult_t.FromPointer), SteamInventoryRequestPricesResult_t.StructSize(), 4705);
		}

		internal static SteamInventoryRequestPricesResult_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (SteamInventoryRequestPricesResult_t)Marshal.PtrToStructure(p, typeof(SteamInventoryRequestPricesResult_t));
			}
			return (SteamInventoryRequestPricesResult_t.PackSmall)Marshal.PtrToStructure(p, typeof(SteamInventoryRequestPricesResult_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return SteamInventoryRequestPricesResult_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return SteamInventoryRequestPricesResult_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			SteamInventoryRequestPricesResult_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			SteamInventoryRequestPricesResult_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			SteamInventoryRequestPricesResult_t steamInventoryRequestPricesResultT = SteamInventoryRequestPricesResult_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<SteamInventoryRequestPricesResult_t>(steamInventoryRequestPricesResultT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<SteamInventoryRequestPricesResult_t>(steamInventoryRequestPricesResultT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			SteamInventoryRequestPricesResult_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(SteamInventoryRequestPricesResult_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(SteamInventoryRequestPricesResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(SteamInventoryRequestPricesResult_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(SteamInventoryRequestPricesResult_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(SteamInventoryRequestPricesResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(SteamInventoryRequestPricesResult_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(SteamInventoryRequestPricesResult_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(SteamInventoryRequestPricesResult_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(SteamInventoryRequestPricesResult_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(SteamInventoryRequestPricesResult_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(SteamInventoryRequestPricesResult_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(SteamInventoryRequestPricesResult_t.OnGetSize)
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
				CallbackId = 4705
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 4705);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(SteamInventoryRequestPricesResult_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(SteamInventoryRequestPricesResult_t));
		}

		internal struct PackSmall
		{
			internal SteamNative.Result Result;

			internal string Currency;

			public static implicit operator SteamInventoryRequestPricesResult_t(SteamInventoryRequestPricesResult_t.PackSmall d)
			{
				SteamInventoryRequestPricesResult_t steamInventoryRequestPricesResultT = new SteamInventoryRequestPricesResult_t()
				{
					Result = d.Result,
					Currency = d.Currency
				};
				return steamInventoryRequestPricesResultT;
			}
		}
	}
}