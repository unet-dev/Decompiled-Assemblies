using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct MusicPlayerWantsPlayingRepeatStatus_t
	{
		internal const int CallbackId = 4114;

		internal int PlayingRepeatStatus;

		internal static MusicPlayerWantsPlayingRepeatStatus_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (MusicPlayerWantsPlayingRepeatStatus_t)Marshal.PtrToStructure(p, typeof(MusicPlayerWantsPlayingRepeatStatus_t));
			}
			return (MusicPlayerWantsPlayingRepeatStatus_t.PackSmall)Marshal.PtrToStructure(p, typeof(MusicPlayerWantsPlayingRepeatStatus_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return MusicPlayerWantsPlayingRepeatStatus_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return MusicPlayerWantsPlayingRepeatStatus_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			MusicPlayerWantsPlayingRepeatStatus_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			MusicPlayerWantsPlayingRepeatStatus_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			MusicPlayerWantsPlayingRepeatStatus_t musicPlayerWantsPlayingRepeatStatusT = MusicPlayerWantsPlayingRepeatStatus_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<MusicPlayerWantsPlayingRepeatStatus_t>(musicPlayerWantsPlayingRepeatStatusT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<MusicPlayerWantsPlayingRepeatStatus_t>(musicPlayerWantsPlayingRepeatStatusT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			MusicPlayerWantsPlayingRepeatStatus_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(MusicPlayerWantsPlayingRepeatStatus_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(MusicPlayerWantsPlayingRepeatStatus_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(MusicPlayerWantsPlayingRepeatStatus_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(MusicPlayerWantsPlayingRepeatStatus_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(MusicPlayerWantsPlayingRepeatStatus_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(MusicPlayerWantsPlayingRepeatStatus_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(MusicPlayerWantsPlayingRepeatStatus_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(MusicPlayerWantsPlayingRepeatStatus_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(MusicPlayerWantsPlayingRepeatStatus_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(MusicPlayerWantsPlayingRepeatStatus_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(MusicPlayerWantsPlayingRepeatStatus_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(MusicPlayerWantsPlayingRepeatStatus_t.OnGetSize)
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
				CallbackId = 4114
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 4114);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(MusicPlayerWantsPlayingRepeatStatus_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(MusicPlayerWantsPlayingRepeatStatus_t));
		}

		internal struct PackSmall
		{
			internal int PlayingRepeatStatus;

			public static implicit operator MusicPlayerWantsPlayingRepeatStatus_t(MusicPlayerWantsPlayingRepeatStatus_t.PackSmall d)
			{
				return new MusicPlayerWantsPlayingRepeatStatus_t()
				{
					PlayingRepeatStatus = d.PlayingRepeatStatus
				};
			}
		}
	}
}