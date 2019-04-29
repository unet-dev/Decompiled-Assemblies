using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct UserAchievementStored_t
	{
		internal const int CallbackId = 1103;

		internal ulong GameID;

		internal bool GroupAchievement;

		internal string AchievementName;

		internal uint CurProgress;

		internal uint MaxProgress;

		internal static UserAchievementStored_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (UserAchievementStored_t)Marshal.PtrToStructure(p, typeof(UserAchievementStored_t));
			}
			return (UserAchievementStored_t.PackSmall)Marshal.PtrToStructure(p, typeof(UserAchievementStored_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return UserAchievementStored_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return UserAchievementStored_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			UserAchievementStored_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			UserAchievementStored_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			UserAchievementStored_t userAchievementStoredT = UserAchievementStored_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<UserAchievementStored_t>(userAchievementStoredT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<UserAchievementStored_t>(userAchievementStoredT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			UserAchievementStored_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(UserAchievementStored_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(UserAchievementStored_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(UserAchievementStored_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(UserAchievementStored_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(UserAchievementStored_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(UserAchievementStored_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(UserAchievementStored_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(UserAchievementStored_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(UserAchievementStored_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(UserAchievementStored_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(UserAchievementStored_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(UserAchievementStored_t.OnGetSize)
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
				CallbackId = 1103
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 1103);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(UserAchievementStored_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(UserAchievementStored_t));
		}

		internal struct PackSmall
		{
			internal ulong GameID;

			internal bool GroupAchievement;

			internal string AchievementName;

			internal uint CurProgress;

			internal uint MaxProgress;

			public static implicit operator UserAchievementStored_t(UserAchievementStored_t.PackSmall d)
			{
				UserAchievementStored_t userAchievementStoredT = new UserAchievementStored_t()
				{
					GameID = d.GameID,
					GroupAchievement = d.GroupAchievement,
					AchievementName = d.AchievementName,
					CurProgress = d.CurProgress,
					MaxProgress = d.MaxProgress
				};
				return userAchievementStoredT;
			}
		}
	}
}