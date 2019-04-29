using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct GCMessageFailed_t
	{
		internal const int CallbackId = 1702;

		internal static GCMessageFailed_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (GCMessageFailed_t)Marshal.PtrToStructure(p, typeof(GCMessageFailed_t));
			}
			return (GCMessageFailed_t.PackSmall)Marshal.PtrToStructure(p, typeof(GCMessageFailed_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return GCMessageFailed_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return GCMessageFailed_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			GCMessageFailed_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			GCMessageFailed_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			GCMessageFailed_t gCMessageFailedT = GCMessageFailed_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<GCMessageFailed_t>(gCMessageFailedT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<GCMessageFailed_t>(gCMessageFailedT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			GCMessageFailed_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(GCMessageFailed_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(GCMessageFailed_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(GCMessageFailed_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(GCMessageFailed_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(GCMessageFailed_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(GCMessageFailed_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(GCMessageFailed_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(GCMessageFailed_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(GCMessageFailed_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(GCMessageFailed_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(GCMessageFailed_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(GCMessageFailed_t.OnGetSize)
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
				CallbackId = 1702
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 1702);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(GCMessageFailed_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(GCMessageFailed_t));
		}

		internal struct PackSmall
		{
			public static implicit operator GCMessageFailed_t(GCMessageFailed_t.PackSmall d)
			{
				return new GCMessageFailed_t();
			}
		}
	}
}