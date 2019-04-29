using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct UserStatsStored_t
	{
		internal const int CallbackId = 1102;

		internal ulong GameID;

		internal SteamNative.Result Result;

		internal static UserStatsStored_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (UserStatsStored_t)Marshal.PtrToStructure(p, typeof(UserStatsStored_t));
			}
			return (UserStatsStored_t.PackSmall)Marshal.PtrToStructure(p, typeof(UserStatsStored_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return UserStatsStored_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return UserStatsStored_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			UserStatsStored_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			UserStatsStored_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			UserStatsStored_t userStatsStoredT = UserStatsStored_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<UserStatsStored_t>(userStatsStoredT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<UserStatsStored_t>(userStatsStoredT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			UserStatsStored_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(UserStatsStored_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(UserStatsStored_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(UserStatsStored_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(UserStatsStored_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(UserStatsStored_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(UserStatsStored_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(UserStatsStored_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(UserStatsStored_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(UserStatsStored_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(UserStatsStored_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(UserStatsStored_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(UserStatsStored_t.OnGetSize)
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
				CallbackId = 1102
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 1102);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(UserStatsStored_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(UserStatsStored_t));
		}

		internal struct PackSmall
		{
			internal ulong GameID;

			internal SteamNative.Result Result;

			public static implicit operator UserStatsStored_t(UserStatsStored_t.PackSmall d)
			{
				UserStatsStored_t userStatsStoredT = new UserStatsStored_t()
				{
					GameID = d.GameID,
					Result = d.Result
				};
				return userStatsStoredT;
			}
		}
	}
}