using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct GSClientApprove_t
	{
		internal const int CallbackId = 201;

		internal ulong SteamID;

		internal ulong OwnerSteamID;

		internal static GSClientApprove_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (GSClientApprove_t)Marshal.PtrToStructure(p, typeof(GSClientApprove_t));
			}
			return (GSClientApprove_t.PackSmall)Marshal.PtrToStructure(p, typeof(GSClientApprove_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return GSClientApprove_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return GSClientApprove_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			GSClientApprove_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			GSClientApprove_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			GSClientApprove_t gSClientApproveT = GSClientApprove_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<GSClientApprove_t>(gSClientApproveT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<GSClientApprove_t>(gSClientApproveT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			GSClientApprove_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(GSClientApprove_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(GSClientApprove_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(GSClientApprove_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(GSClientApprove_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(GSClientApprove_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(GSClientApprove_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(GSClientApprove_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(GSClientApprove_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(GSClientApprove_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(GSClientApprove_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(GSClientApprove_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(GSClientApprove_t.OnGetSize)
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
				CallbackId = 201
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 201);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(GSClientApprove_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(GSClientApprove_t));
		}

		internal struct PackSmall
		{
			internal ulong SteamID;

			internal ulong OwnerSteamID;

			public static implicit operator GSClientApprove_t(GSClientApprove_t.PackSmall d)
			{
				GSClientApprove_t gSClientApproveT = new GSClientApprove_t()
				{
					SteamID = d.SteamID,
					OwnerSteamID = d.OwnerSteamID
				};
				return gSClientApproveT;
			}
		}
	}
}