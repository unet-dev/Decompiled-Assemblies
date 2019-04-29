using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct GCMessageAvailable_t
	{
		internal const int CallbackId = 1701;

		internal uint MessageSize;

		internal static GCMessageAvailable_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (GCMessageAvailable_t)Marshal.PtrToStructure(p, typeof(GCMessageAvailable_t));
			}
			return (GCMessageAvailable_t.PackSmall)Marshal.PtrToStructure(p, typeof(GCMessageAvailable_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return GCMessageAvailable_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return GCMessageAvailable_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			GCMessageAvailable_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			GCMessageAvailable_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			GCMessageAvailable_t gCMessageAvailableT = GCMessageAvailable_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<GCMessageAvailable_t>(gCMessageAvailableT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<GCMessageAvailable_t>(gCMessageAvailableT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			GCMessageAvailable_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(GCMessageAvailable_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(GCMessageAvailable_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(GCMessageAvailable_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(GCMessageAvailable_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(GCMessageAvailable_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(GCMessageAvailable_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(GCMessageAvailable_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(GCMessageAvailable_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(GCMessageAvailable_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(GCMessageAvailable_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(GCMessageAvailable_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(GCMessageAvailable_t.OnGetSize)
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
				CallbackId = 1701
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 1701);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(GCMessageAvailable_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(GCMessageAvailable_t));
		}

		internal struct PackSmall
		{
			internal uint MessageSize;

			public static implicit operator GCMessageAvailable_t(GCMessageAvailable_t.PackSmall d)
			{
				return new GCMessageAvailable_t()
				{
					MessageSize = d.MessageSize
				};
			}
		}
	}
}