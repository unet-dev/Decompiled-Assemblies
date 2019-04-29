using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct GSPolicyResponse_t
	{
		internal const int CallbackId = 115;

		internal byte Secure;

		internal static GSPolicyResponse_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (GSPolicyResponse_t)Marshal.PtrToStructure(p, typeof(GSPolicyResponse_t));
			}
			return (GSPolicyResponse_t.PackSmall)Marshal.PtrToStructure(p, typeof(GSPolicyResponse_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return GSPolicyResponse_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return GSPolicyResponse_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			GSPolicyResponse_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			GSPolicyResponse_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			GSPolicyResponse_t gSPolicyResponseT = GSPolicyResponse_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<GSPolicyResponse_t>(gSPolicyResponseT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<GSPolicyResponse_t>(gSPolicyResponseT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			GSPolicyResponse_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(GSPolicyResponse_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(GSPolicyResponse_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(GSPolicyResponse_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(GSPolicyResponse_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(GSPolicyResponse_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(GSPolicyResponse_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(GSPolicyResponse_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(GSPolicyResponse_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(GSPolicyResponse_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(GSPolicyResponse_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(GSPolicyResponse_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(GSPolicyResponse_t.OnGetSize)
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
				CallbackId = 115
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 115);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(GSPolicyResponse_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(GSPolicyResponse_t));
		}

		internal struct PackSmall
		{
			internal byte Secure;

			public static implicit operator GSPolicyResponse_t(GSPolicyResponse_t.PackSmall d)
			{
				return new GSPolicyResponse_t()
				{
					Secure = d.Secure
				};
			}
		}
	}
}