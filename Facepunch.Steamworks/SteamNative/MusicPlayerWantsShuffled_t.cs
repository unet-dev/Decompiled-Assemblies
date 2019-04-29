using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct MusicPlayerWantsShuffled_t
	{
		internal const int CallbackId = 4109;

		internal bool Shuffled;

		internal static MusicPlayerWantsShuffled_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (MusicPlayerWantsShuffled_t)Marshal.PtrToStructure(p, typeof(MusicPlayerWantsShuffled_t));
			}
			return (MusicPlayerWantsShuffled_t.PackSmall)Marshal.PtrToStructure(p, typeof(MusicPlayerWantsShuffled_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return MusicPlayerWantsShuffled_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return MusicPlayerWantsShuffled_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			MusicPlayerWantsShuffled_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			MusicPlayerWantsShuffled_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			MusicPlayerWantsShuffled_t musicPlayerWantsShuffledT = MusicPlayerWantsShuffled_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<MusicPlayerWantsShuffled_t>(musicPlayerWantsShuffledT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<MusicPlayerWantsShuffled_t>(musicPlayerWantsShuffledT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			MusicPlayerWantsShuffled_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(MusicPlayerWantsShuffled_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(MusicPlayerWantsShuffled_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(MusicPlayerWantsShuffled_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(MusicPlayerWantsShuffled_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(MusicPlayerWantsShuffled_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(MusicPlayerWantsShuffled_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(MusicPlayerWantsShuffled_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(MusicPlayerWantsShuffled_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(MusicPlayerWantsShuffled_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(MusicPlayerWantsShuffled_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(MusicPlayerWantsShuffled_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(MusicPlayerWantsShuffled_t.OnGetSize)
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
				CallbackId = 4109
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 4109);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(MusicPlayerWantsShuffled_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(MusicPlayerWantsShuffled_t));
		}

		internal struct PackSmall
		{
			internal bool Shuffled;

			public static implicit operator MusicPlayerWantsShuffled_t(MusicPlayerWantsShuffled_t.PackSmall d)
			{
				return new MusicPlayerWantsShuffled_t()
				{
					Shuffled = d.Shuffled
				};
			}
		}
	}
}