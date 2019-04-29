using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct UserStatsReceived_t
	{
		internal const int CallbackId = 1101;

		internal ulong GameID;

		internal SteamNative.Result Result;

		internal ulong SteamIDUser;

		internal static CallResult<UserStatsReceived_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<UserStatsReceived_t, bool> CallbackFunction)
		{
			return new CallResult<UserStatsReceived_t>(steamworks, call, CallbackFunction, new CallResult<UserStatsReceived_t>.ConvertFromPointer(UserStatsReceived_t.FromPointer), UserStatsReceived_t.StructSize(), 1101);
		}

		internal static UserStatsReceived_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (UserStatsReceived_t)Marshal.PtrToStructure(p, typeof(UserStatsReceived_t));
			}
			return (UserStatsReceived_t.PackSmall)Marshal.PtrToStructure(p, typeof(UserStatsReceived_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return UserStatsReceived_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return UserStatsReceived_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			UserStatsReceived_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			UserStatsReceived_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			UserStatsReceived_t userStatsReceivedT = UserStatsReceived_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<UserStatsReceived_t>(userStatsReceivedT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<UserStatsReceived_t>(userStatsReceivedT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			UserStatsReceived_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(UserStatsReceived_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(UserStatsReceived_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(UserStatsReceived_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(UserStatsReceived_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(UserStatsReceived_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(UserStatsReceived_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(UserStatsReceived_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(UserStatsReceived_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(UserStatsReceived_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(UserStatsReceived_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(UserStatsReceived_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(UserStatsReceived_t.OnGetSize)
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
				CallbackId = 1101
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 1101);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(UserStatsReceived_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(UserStatsReceived_t));
		}

		internal struct PackSmall
		{
			internal ulong GameID;

			internal SteamNative.Result Result;

			internal ulong SteamIDUser;

			public static implicit operator UserStatsReceived_t(UserStatsReceived_t.PackSmall d)
			{
				UserStatsReceived_t userStatsReceivedT = new UserStatsReceived_t()
				{
					GameID = d.GameID,
					Result = d.Result,
					SteamIDUser = d.SteamIDUser
				};
				return userStatsReceivedT;
			}
		}
	}
}