using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct MusicPlayerWantsVolume_t
	{
		internal const int CallbackId = 4011;

		internal float NewVolume;

		internal static MusicPlayerWantsVolume_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (MusicPlayerWantsVolume_t)Marshal.PtrToStructure(p, typeof(MusicPlayerWantsVolume_t));
			}
			return (MusicPlayerWantsVolume_t.PackSmall)Marshal.PtrToStructure(p, typeof(MusicPlayerWantsVolume_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return MusicPlayerWantsVolume_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return MusicPlayerWantsVolume_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			MusicPlayerWantsVolume_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			MusicPlayerWantsVolume_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			MusicPlayerWantsVolume_t musicPlayerWantsVolumeT = MusicPlayerWantsVolume_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<MusicPlayerWantsVolume_t>(musicPlayerWantsVolumeT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<MusicPlayerWantsVolume_t>(musicPlayerWantsVolumeT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			MusicPlayerWantsVolume_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(MusicPlayerWantsVolume_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(MusicPlayerWantsVolume_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(MusicPlayerWantsVolume_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(MusicPlayerWantsVolume_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(MusicPlayerWantsVolume_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(MusicPlayerWantsVolume_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(MusicPlayerWantsVolume_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(MusicPlayerWantsVolume_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(MusicPlayerWantsVolume_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(MusicPlayerWantsVolume_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(MusicPlayerWantsVolume_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(MusicPlayerWantsVolume_t.OnGetSize)
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
				CallbackId = 4011
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 4011);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(MusicPlayerWantsVolume_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(MusicPlayerWantsVolume_t));
		}

		internal struct PackSmall
		{
			internal float NewVolume;

			public static implicit operator MusicPlayerWantsVolume_t(MusicPlayerWantsVolume_t.PackSmall d)
			{
				return new MusicPlayerWantsVolume_t()
				{
					NewVolume = d.NewVolume
				};
			}
		}
	}
}