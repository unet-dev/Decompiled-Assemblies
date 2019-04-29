using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct GlobalAchievementPercentagesReady_t
	{
		internal const int CallbackId = 1110;

		internal ulong GameID;

		internal SteamNative.Result Result;

		internal static CallResult<GlobalAchievementPercentagesReady_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<GlobalAchievementPercentagesReady_t, bool> CallbackFunction)
		{
			return new CallResult<GlobalAchievementPercentagesReady_t>(steamworks, call, CallbackFunction, new CallResult<GlobalAchievementPercentagesReady_t>.ConvertFromPointer(GlobalAchievementPercentagesReady_t.FromPointer), GlobalAchievementPercentagesReady_t.StructSize(), 1110);
		}

		internal static GlobalAchievementPercentagesReady_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (GlobalAchievementPercentagesReady_t)Marshal.PtrToStructure(p, typeof(GlobalAchievementPercentagesReady_t));
			}
			return (GlobalAchievementPercentagesReady_t.PackSmall)Marshal.PtrToStructure(p, typeof(GlobalAchievementPercentagesReady_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return GlobalAchievementPercentagesReady_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return GlobalAchievementPercentagesReady_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			GlobalAchievementPercentagesReady_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			GlobalAchievementPercentagesReady_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			GlobalAchievementPercentagesReady_t globalAchievementPercentagesReadyT = GlobalAchievementPercentagesReady_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<GlobalAchievementPercentagesReady_t>(globalAchievementPercentagesReadyT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<GlobalAchievementPercentagesReady_t>(globalAchievementPercentagesReadyT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			GlobalAchievementPercentagesReady_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(GlobalAchievementPercentagesReady_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(GlobalAchievementPercentagesReady_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(GlobalAchievementPercentagesReady_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(GlobalAchievementPercentagesReady_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(GlobalAchievementPercentagesReady_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(GlobalAchievementPercentagesReady_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(GlobalAchievementPercentagesReady_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(GlobalAchievementPercentagesReady_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(GlobalAchievementPercentagesReady_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(GlobalAchievementPercentagesReady_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(GlobalAchievementPercentagesReady_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(GlobalAchievementPercentagesReady_t.OnGetSize)
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
				CallbackId = 1110
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 1110);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(GlobalAchievementPercentagesReady_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(GlobalAchievementPercentagesReady_t));
		}

		internal struct PackSmall
		{
			internal ulong GameID;

			internal SteamNative.Result Result;

			public static implicit operator GlobalAchievementPercentagesReady_t(GlobalAchievementPercentagesReady_t.PackSmall d)
			{
				GlobalAchievementPercentagesReady_t globalAchievementPercentagesReadyT = new GlobalAchievementPercentagesReady_t()
				{
					GameID = d.GameID,
					Result = d.Result
				};
				return globalAchievementPercentagesReadyT;
			}
		}
	}
}