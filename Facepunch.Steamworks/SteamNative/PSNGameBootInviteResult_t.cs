using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct PSNGameBootInviteResult_t
	{
		internal const int CallbackId = 515;

		internal bool GameBootInviteExists;

		internal ulong SteamIDLobby;

		internal static PSNGameBootInviteResult_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (PSNGameBootInviteResult_t)Marshal.PtrToStructure(p, typeof(PSNGameBootInviteResult_t));
			}
			return (PSNGameBootInviteResult_t.PackSmall)Marshal.PtrToStructure(p, typeof(PSNGameBootInviteResult_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return PSNGameBootInviteResult_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return PSNGameBootInviteResult_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			PSNGameBootInviteResult_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			PSNGameBootInviteResult_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			PSNGameBootInviteResult_t pSNGameBootInviteResultT = PSNGameBootInviteResult_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<PSNGameBootInviteResult_t>(pSNGameBootInviteResultT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<PSNGameBootInviteResult_t>(pSNGameBootInviteResultT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			PSNGameBootInviteResult_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(PSNGameBootInviteResult_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(PSNGameBootInviteResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(PSNGameBootInviteResult_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(PSNGameBootInviteResult_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(PSNGameBootInviteResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(PSNGameBootInviteResult_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(PSNGameBootInviteResult_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(PSNGameBootInviteResult_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(PSNGameBootInviteResult_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(PSNGameBootInviteResult_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(PSNGameBootInviteResult_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(PSNGameBootInviteResult_t.OnGetSize)
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
				CallbackId = 515
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 515);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(PSNGameBootInviteResult_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(PSNGameBootInviteResult_t));
		}

		internal struct PackSmall
		{
			internal bool GameBootInviteExists;

			internal ulong SteamIDLobby;

			public static implicit operator PSNGameBootInviteResult_t(PSNGameBootInviteResult_t.PackSmall d)
			{
				PSNGameBootInviteResult_t pSNGameBootInviteResultT = new PSNGameBootInviteResult_t()
				{
					GameBootInviteExists = d.GameBootInviteExists,
					SteamIDLobby = d.SteamIDLobby
				};
				return pSNGameBootInviteResultT;
			}
		}
	}
}