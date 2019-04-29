using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct GSReputation_t
	{
		internal const int CallbackId = 209;

		internal SteamNative.Result Result;

		internal uint ReputationScore;

		internal bool Banned;

		internal uint BannedIP;

		internal ushort BannedPort;

		internal ulong BannedGameID;

		internal uint BanExpires;

		internal static CallResult<GSReputation_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<GSReputation_t, bool> CallbackFunction)
		{
			return new CallResult<GSReputation_t>(steamworks, call, CallbackFunction, new CallResult<GSReputation_t>.ConvertFromPointer(GSReputation_t.FromPointer), GSReputation_t.StructSize(), 209);
		}

		internal static GSReputation_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (GSReputation_t)Marshal.PtrToStructure(p, typeof(GSReputation_t));
			}
			return (GSReputation_t.PackSmall)Marshal.PtrToStructure(p, typeof(GSReputation_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return GSReputation_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return GSReputation_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			GSReputation_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			GSReputation_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			GSReputation_t gSReputationT = GSReputation_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<GSReputation_t>(gSReputationT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<GSReputation_t>(gSReputationT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			GSReputation_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(GSReputation_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(GSReputation_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(GSReputation_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(GSReputation_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(GSReputation_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(GSReputation_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(GSReputation_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(GSReputation_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(GSReputation_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(GSReputation_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(GSReputation_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(GSReputation_t.OnGetSize)
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
				CallbackId = 209
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 209);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(GSReputation_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(GSReputation_t));
		}

		internal struct PackSmall
		{
			internal SteamNative.Result Result;

			internal uint ReputationScore;

			internal bool Banned;

			internal uint BannedIP;

			internal ushort BannedPort;

			internal ulong BannedGameID;

			internal uint BanExpires;

			public static implicit operator GSReputation_t(GSReputation_t.PackSmall d)
			{
				GSReputation_t gSReputationT = new GSReputation_t()
				{
					Result = d.Result,
					ReputationScore = d.ReputationScore,
					Banned = d.Banned,
					BannedIP = d.BannedIP,
					BannedPort = d.BannedPort,
					BannedGameID = d.BannedGameID,
					BanExpires = d.BanExpires
				};
				return gSReputationT;
			}
		}
	}
}