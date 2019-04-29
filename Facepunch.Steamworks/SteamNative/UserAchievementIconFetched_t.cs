using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct UserAchievementIconFetched_t
	{
		internal const int CallbackId = 1109;

		internal ulong GameID;

		internal string AchievementName;

		internal bool Achieved;

		internal int IconHandle;

		internal static UserAchievementIconFetched_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (UserAchievementIconFetched_t)Marshal.PtrToStructure(p, typeof(UserAchievementIconFetched_t));
			}
			return (UserAchievementIconFetched_t.PackSmall)Marshal.PtrToStructure(p, typeof(UserAchievementIconFetched_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return UserAchievementIconFetched_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return UserAchievementIconFetched_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			UserAchievementIconFetched_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			UserAchievementIconFetched_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			UserAchievementIconFetched_t userAchievementIconFetchedT = UserAchievementIconFetched_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<UserAchievementIconFetched_t>(userAchievementIconFetchedT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<UserAchievementIconFetched_t>(userAchievementIconFetchedT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			UserAchievementIconFetched_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(UserAchievementIconFetched_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(UserAchievementIconFetched_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(UserAchievementIconFetched_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(UserAchievementIconFetched_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(UserAchievementIconFetched_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(UserAchievementIconFetched_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(UserAchievementIconFetched_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(UserAchievementIconFetched_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(UserAchievementIconFetched_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(UserAchievementIconFetched_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(UserAchievementIconFetched_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(UserAchievementIconFetched_t.OnGetSize)
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
				CallbackId = 1109
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 1109);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(UserAchievementIconFetched_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(UserAchievementIconFetched_t));
		}

		internal struct PackSmall
		{
			internal ulong GameID;

			internal string AchievementName;

			internal bool Achieved;

			internal int IconHandle;

			public static implicit operator UserAchievementIconFetched_t(UserAchievementIconFetched_t.PackSmall d)
			{
				UserAchievementIconFetched_t userAchievementIconFetchedT = new UserAchievementIconFetched_t()
				{
					GameID = d.GameID,
					AchievementName = d.AchievementName,
					Achieved = d.Achieved,
					IconHandle = d.IconHandle
				};
				return userAchievementIconFetchedT;
			}
		}
	}
}