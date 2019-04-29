using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct MicroTxnAuthorizationResponse_t
	{
		internal const int CallbackId = 152;

		internal uint AppID;

		internal ulong OrderID;

		internal byte Authorized;

		internal static MicroTxnAuthorizationResponse_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (MicroTxnAuthorizationResponse_t)Marshal.PtrToStructure(p, typeof(MicroTxnAuthorizationResponse_t));
			}
			return (MicroTxnAuthorizationResponse_t.PackSmall)Marshal.PtrToStructure(p, typeof(MicroTxnAuthorizationResponse_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return MicroTxnAuthorizationResponse_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return MicroTxnAuthorizationResponse_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			MicroTxnAuthorizationResponse_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			MicroTxnAuthorizationResponse_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			MicroTxnAuthorizationResponse_t microTxnAuthorizationResponseT = MicroTxnAuthorizationResponse_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<MicroTxnAuthorizationResponse_t>(microTxnAuthorizationResponseT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<MicroTxnAuthorizationResponse_t>(microTxnAuthorizationResponseT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			MicroTxnAuthorizationResponse_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(MicroTxnAuthorizationResponse_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(MicroTxnAuthorizationResponse_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(MicroTxnAuthorizationResponse_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(MicroTxnAuthorizationResponse_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(MicroTxnAuthorizationResponse_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(MicroTxnAuthorizationResponse_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(MicroTxnAuthorizationResponse_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(MicroTxnAuthorizationResponse_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(MicroTxnAuthorizationResponse_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(MicroTxnAuthorizationResponse_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(MicroTxnAuthorizationResponse_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(MicroTxnAuthorizationResponse_t.OnGetSize)
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
				CallbackId = 152
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 152);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(MicroTxnAuthorizationResponse_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(MicroTxnAuthorizationResponse_t));
		}

		internal struct PackSmall
		{
			internal uint AppID;

			internal ulong OrderID;

			internal byte Authorized;

			public static implicit operator MicroTxnAuthorizationResponse_t(MicroTxnAuthorizationResponse_t.PackSmall d)
			{
				MicroTxnAuthorizationResponse_t microTxnAuthorizationResponseT = new MicroTxnAuthorizationResponse_t()
				{
					AppID = d.AppID,
					OrderID = d.OrderID,
					Authorized = d.Authorized
				};
				return microTxnAuthorizationResponseT;
			}
		}
	}
}