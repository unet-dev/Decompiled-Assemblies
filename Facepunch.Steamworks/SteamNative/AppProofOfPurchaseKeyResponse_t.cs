using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct AppProofOfPurchaseKeyResponse_t
	{
		internal const int CallbackId = 1021;

		internal SteamNative.Result Result;

		internal uint AppID;

		internal uint CchKeyLength;

		internal string Key;

		internal static AppProofOfPurchaseKeyResponse_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (AppProofOfPurchaseKeyResponse_t)Marshal.PtrToStructure(p, typeof(AppProofOfPurchaseKeyResponse_t));
			}
			return (AppProofOfPurchaseKeyResponse_t.PackSmall)Marshal.PtrToStructure(p, typeof(AppProofOfPurchaseKeyResponse_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return AppProofOfPurchaseKeyResponse_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return AppProofOfPurchaseKeyResponse_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			AppProofOfPurchaseKeyResponse_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			AppProofOfPurchaseKeyResponse_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			AppProofOfPurchaseKeyResponse_t appProofOfPurchaseKeyResponseT = AppProofOfPurchaseKeyResponse_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<AppProofOfPurchaseKeyResponse_t>(appProofOfPurchaseKeyResponseT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<AppProofOfPurchaseKeyResponse_t>(appProofOfPurchaseKeyResponseT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			AppProofOfPurchaseKeyResponse_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(AppProofOfPurchaseKeyResponse_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(AppProofOfPurchaseKeyResponse_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(AppProofOfPurchaseKeyResponse_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(AppProofOfPurchaseKeyResponse_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(AppProofOfPurchaseKeyResponse_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(AppProofOfPurchaseKeyResponse_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(AppProofOfPurchaseKeyResponse_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(AppProofOfPurchaseKeyResponse_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(AppProofOfPurchaseKeyResponse_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(AppProofOfPurchaseKeyResponse_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(AppProofOfPurchaseKeyResponse_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(AppProofOfPurchaseKeyResponse_t.OnGetSize)
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
				CallbackId = 1021
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 1021);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(AppProofOfPurchaseKeyResponse_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(AppProofOfPurchaseKeyResponse_t));
		}

		internal struct PackSmall
		{
			internal SteamNative.Result Result;

			internal uint AppID;

			internal uint CchKeyLength;

			internal string Key;

			public static implicit operator AppProofOfPurchaseKeyResponse_t(AppProofOfPurchaseKeyResponse_t.PackSmall d)
			{
				AppProofOfPurchaseKeyResponse_t appProofOfPurchaseKeyResponseT = new AppProofOfPurchaseKeyResponse_t()
				{
					Result = d.Result,
					AppID = d.AppID,
					CchKeyLength = d.CchKeyLength,
					Key = d.Key
				};
				return appProofOfPurchaseKeyResponseT;
			}
		}
	}
}