using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct ClanOfficerListResponse_t
	{
		internal const int CallbackId = 335;

		internal ulong SteamIDClan;

		internal int COfficers;

		internal byte Success;

		internal static CallResult<ClanOfficerListResponse_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<ClanOfficerListResponse_t, bool> CallbackFunction)
		{
			return new CallResult<ClanOfficerListResponse_t>(steamworks, call, CallbackFunction, new CallResult<ClanOfficerListResponse_t>.ConvertFromPointer(ClanOfficerListResponse_t.FromPointer), ClanOfficerListResponse_t.StructSize(), 335);
		}

		internal static ClanOfficerListResponse_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (ClanOfficerListResponse_t)Marshal.PtrToStructure(p, typeof(ClanOfficerListResponse_t));
			}
			return (ClanOfficerListResponse_t.PackSmall)Marshal.PtrToStructure(p, typeof(ClanOfficerListResponse_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return ClanOfficerListResponse_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return ClanOfficerListResponse_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			ClanOfficerListResponse_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			ClanOfficerListResponse_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			ClanOfficerListResponse_t clanOfficerListResponseT = ClanOfficerListResponse_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<ClanOfficerListResponse_t>(clanOfficerListResponseT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<ClanOfficerListResponse_t>(clanOfficerListResponseT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			ClanOfficerListResponse_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(ClanOfficerListResponse_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(ClanOfficerListResponse_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(ClanOfficerListResponse_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(ClanOfficerListResponse_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(ClanOfficerListResponse_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(ClanOfficerListResponse_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(ClanOfficerListResponse_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(ClanOfficerListResponse_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(ClanOfficerListResponse_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(ClanOfficerListResponse_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(ClanOfficerListResponse_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(ClanOfficerListResponse_t.OnGetSize)
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
				CallbackId = 335
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 335);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(ClanOfficerListResponse_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(ClanOfficerListResponse_t));
		}

		internal struct PackSmall
		{
			internal ulong SteamIDClan;

			internal int COfficers;

			internal byte Success;

			public static implicit operator ClanOfficerListResponse_t(ClanOfficerListResponse_t.PackSmall d)
			{
				ClanOfficerListResponse_t clanOfficerListResponseT = new ClanOfficerListResponse_t()
				{
					SteamIDClan = d.SteamIDClan,
					COfficers = d.COfficers,
					Success = d.Success
				};
				return clanOfficerListResponseT;
			}
		}
	}
}