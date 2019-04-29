using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct IPCFailure_t
	{
		internal const int CallbackId = 117;

		internal byte FailureType;

		internal static IPCFailure_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (IPCFailure_t)Marshal.PtrToStructure(p, typeof(IPCFailure_t));
			}
			return (IPCFailure_t.PackSmall)Marshal.PtrToStructure(p, typeof(IPCFailure_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return IPCFailure_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return IPCFailure_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			IPCFailure_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			IPCFailure_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			IPCFailure_t pCFailureT = IPCFailure_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<IPCFailure_t>(pCFailureT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<IPCFailure_t>(pCFailureT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			IPCFailure_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(IPCFailure_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(IPCFailure_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(IPCFailure_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(IPCFailure_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(IPCFailure_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(IPCFailure_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(IPCFailure_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(IPCFailure_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(IPCFailure_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(IPCFailure_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(IPCFailure_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(IPCFailure_t.OnGetSize)
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
				CallbackId = 117
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 117);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(IPCFailure_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(IPCFailure_t));
		}

		internal struct PackSmall
		{
			internal byte FailureType;

			public static implicit operator IPCFailure_t(IPCFailure_t.PackSmall d)
			{
				return new IPCFailure_t()
				{
					FailureType = d.FailureType
				};
			}
		}
	}
}