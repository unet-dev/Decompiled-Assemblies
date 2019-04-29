using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct GSClientGroupStatus_t
	{
		internal const int CallbackId = 208;

		internal ulong SteamIDUser;

		internal ulong SteamIDGroup;

		internal bool Member;

		internal bool Officer;

		internal static GSClientGroupStatus_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (GSClientGroupStatus_t)Marshal.PtrToStructure(p, typeof(GSClientGroupStatus_t));
			}
			return (GSClientGroupStatus_t.PackSmall)Marshal.PtrToStructure(p, typeof(GSClientGroupStatus_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return GSClientGroupStatus_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return GSClientGroupStatus_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			GSClientGroupStatus_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			GSClientGroupStatus_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			GSClientGroupStatus_t gSClientGroupStatusT = GSClientGroupStatus_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<GSClientGroupStatus_t>(gSClientGroupStatusT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<GSClientGroupStatus_t>(gSClientGroupStatusT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			GSClientGroupStatus_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(GSClientGroupStatus_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(GSClientGroupStatus_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(GSClientGroupStatus_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(GSClientGroupStatus_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(GSClientGroupStatus_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(GSClientGroupStatus_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(GSClientGroupStatus_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(GSClientGroupStatus_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(GSClientGroupStatus_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(GSClientGroupStatus_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(GSClientGroupStatus_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(GSClientGroupStatus_t.OnGetSize)
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
				CallbackId = 208
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 208);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(GSClientGroupStatus_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(GSClientGroupStatus_t));
		}

		internal struct PackSmall
		{
			internal ulong SteamIDUser;

			internal ulong SteamIDGroup;

			internal bool Member;

			internal bool Officer;

			public static implicit operator GSClientGroupStatus_t(GSClientGroupStatus_t.PackSmall d)
			{
				GSClientGroupStatus_t gSClientGroupStatusT = new GSClientGroupStatus_t()
				{
					SteamIDUser = d.SteamIDUser,
					SteamIDGroup = d.SteamIDGroup,
					Member = d.Member,
					Officer = d.Officer
				};
				return gSClientGroupStatusT;
			}
		}
	}
}