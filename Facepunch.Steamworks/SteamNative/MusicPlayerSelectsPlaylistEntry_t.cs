using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct MusicPlayerSelectsPlaylistEntry_t
	{
		internal const int CallbackId = 4013;

		internal int NID;

		internal static MusicPlayerSelectsPlaylistEntry_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (MusicPlayerSelectsPlaylistEntry_t)Marshal.PtrToStructure(p, typeof(MusicPlayerSelectsPlaylistEntry_t));
			}
			return (MusicPlayerSelectsPlaylistEntry_t.PackSmall)Marshal.PtrToStructure(p, typeof(MusicPlayerSelectsPlaylistEntry_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return MusicPlayerSelectsPlaylistEntry_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return MusicPlayerSelectsPlaylistEntry_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			MusicPlayerSelectsPlaylistEntry_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			MusicPlayerSelectsPlaylistEntry_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			MusicPlayerSelectsPlaylistEntry_t musicPlayerSelectsPlaylistEntryT = MusicPlayerSelectsPlaylistEntry_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<MusicPlayerSelectsPlaylistEntry_t>(musicPlayerSelectsPlaylistEntryT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<MusicPlayerSelectsPlaylistEntry_t>(musicPlayerSelectsPlaylistEntryT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			MusicPlayerSelectsPlaylistEntry_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(MusicPlayerSelectsPlaylistEntry_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(MusicPlayerSelectsPlaylistEntry_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(MusicPlayerSelectsPlaylistEntry_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(MusicPlayerSelectsPlaylistEntry_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(MusicPlayerSelectsPlaylistEntry_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(MusicPlayerSelectsPlaylistEntry_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(MusicPlayerSelectsPlaylistEntry_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(MusicPlayerSelectsPlaylistEntry_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(MusicPlayerSelectsPlaylistEntry_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(MusicPlayerSelectsPlaylistEntry_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(MusicPlayerSelectsPlaylistEntry_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(MusicPlayerSelectsPlaylistEntry_t.OnGetSize)
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
				CallbackId = 4013
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 4013);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(MusicPlayerSelectsPlaylistEntry_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(MusicPlayerSelectsPlaylistEntry_t));
		}

		internal struct PackSmall
		{
			internal int NID;

			public static implicit operator MusicPlayerSelectsPlaylistEntry_t(MusicPlayerSelectsPlaylistEntry_t.PackSmall d)
			{
				return new MusicPlayerSelectsPlaylistEntry_t()
				{
					NID = d.NID
				};
			}
		}
	}
}