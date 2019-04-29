using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct GSClientDeny_t
	{
		internal const int CallbackId = 202;

		internal ulong SteamID;

		internal SteamNative.DenyReason DenyReason;

		internal string OptionalText;

		internal static GSClientDeny_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (GSClientDeny_t)Marshal.PtrToStructure(p, typeof(GSClientDeny_t));
			}
			return (GSClientDeny_t.PackSmall)Marshal.PtrToStructure(p, typeof(GSClientDeny_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return GSClientDeny_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return GSClientDeny_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			GSClientDeny_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			GSClientDeny_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			GSClientDeny_t gSClientDenyT = GSClientDeny_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<GSClientDeny_t>(gSClientDenyT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<GSClientDeny_t>(gSClientDenyT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			GSClientDeny_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(GSClientDeny_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(GSClientDeny_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(GSClientDeny_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(GSClientDeny_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(GSClientDeny_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(GSClientDeny_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(GSClientDeny_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(GSClientDeny_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(GSClientDeny_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(GSClientDeny_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(GSClientDeny_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(GSClientDeny_t.OnGetSize)
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
				CallbackId = 202
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 202);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(GSClientDeny_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(GSClientDeny_t));
		}

		internal struct PackSmall
		{
			internal ulong SteamID;

			internal SteamNative.DenyReason DenyReason;

			internal string OptionalText;

			public static implicit operator GSClientDeny_t(GSClientDeny_t.PackSmall d)
			{
				GSClientDeny_t gSClientDenyT = new GSClientDeny_t()
				{
					SteamID = d.SteamID,
					DenyReason = d.DenyReason,
					OptionalText = d.OptionalText
				};
				return gSClientDenyT;
			}
		}
	}
}