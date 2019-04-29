using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct GSClientAchievementStatus_t
	{
		internal const int CallbackId = 206;

		internal ulong SteamID;

		internal string PchAchievement;

		internal bool Unlocked;

		internal static GSClientAchievementStatus_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (GSClientAchievementStatus_t)Marshal.PtrToStructure(p, typeof(GSClientAchievementStatus_t));
			}
			return (GSClientAchievementStatus_t.PackSmall)Marshal.PtrToStructure(p, typeof(GSClientAchievementStatus_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return GSClientAchievementStatus_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return GSClientAchievementStatus_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			GSClientAchievementStatus_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			GSClientAchievementStatus_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			GSClientAchievementStatus_t gSClientAchievementStatusT = GSClientAchievementStatus_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<GSClientAchievementStatus_t>(gSClientAchievementStatusT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<GSClientAchievementStatus_t>(gSClientAchievementStatusT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			GSClientAchievementStatus_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(GSClientAchievementStatus_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(GSClientAchievementStatus_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(GSClientAchievementStatus_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(GSClientAchievementStatus_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(GSClientAchievementStatus_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(GSClientAchievementStatus_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(GSClientAchievementStatus_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(GSClientAchievementStatus_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(GSClientAchievementStatus_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(GSClientAchievementStatus_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(GSClientAchievementStatus_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(GSClientAchievementStatus_t.OnGetSize)
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
				CallbackId = 206
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 206);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(GSClientAchievementStatus_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(GSClientAchievementStatus_t));
		}

		internal struct PackSmall
		{
			internal ulong SteamID;

			internal string PchAchievement;

			internal bool Unlocked;

			public static implicit operator GSClientAchievementStatus_t(GSClientAchievementStatus_t.PackSmall d)
			{
				GSClientAchievementStatus_t gSClientAchievementStatusT = new GSClientAchievementStatus_t()
				{
					SteamID = d.SteamID,
					PchAchievement = d.PchAchievement,
					Unlocked = d.Unlocked
				};
				return gSClientAchievementStatusT;
			}
		}
	}
}