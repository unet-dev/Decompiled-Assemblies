using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct StoreAuthURLResponse_t
	{
		internal const int CallbackId = 165;

		internal string URL;

		internal static CallResult<StoreAuthURLResponse_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<StoreAuthURLResponse_t, bool> CallbackFunction)
		{
			return new CallResult<StoreAuthURLResponse_t>(steamworks, call, CallbackFunction, new CallResult<StoreAuthURLResponse_t>.ConvertFromPointer(StoreAuthURLResponse_t.FromPointer), StoreAuthURLResponse_t.StructSize(), 165);
		}

		internal static StoreAuthURLResponse_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (StoreAuthURLResponse_t)Marshal.PtrToStructure(p, typeof(StoreAuthURLResponse_t));
			}
			return (StoreAuthURLResponse_t.PackSmall)Marshal.PtrToStructure(p, typeof(StoreAuthURLResponse_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return StoreAuthURLResponse_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return StoreAuthURLResponse_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			StoreAuthURLResponse_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			StoreAuthURLResponse_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			StoreAuthURLResponse_t storeAuthURLResponseT = StoreAuthURLResponse_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<StoreAuthURLResponse_t>(storeAuthURLResponseT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<StoreAuthURLResponse_t>(storeAuthURLResponseT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			StoreAuthURLResponse_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(StoreAuthURLResponse_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(StoreAuthURLResponse_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(StoreAuthURLResponse_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(StoreAuthURLResponse_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(StoreAuthURLResponse_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(StoreAuthURLResponse_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(StoreAuthURLResponse_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(StoreAuthURLResponse_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(StoreAuthURLResponse_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(StoreAuthURLResponse_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(StoreAuthURLResponse_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(StoreAuthURLResponse_t.OnGetSize)
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
				CallbackId = 165
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 165);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(StoreAuthURLResponse_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(StoreAuthURLResponse_t));
		}

		internal struct PackSmall
		{
			internal string URL;

			public static implicit operator StoreAuthURLResponse_t(StoreAuthURLResponse_t.PackSmall d)
			{
				return new StoreAuthURLResponse_t()
				{
					URL = d.URL
				};
			}
		}
	}
}