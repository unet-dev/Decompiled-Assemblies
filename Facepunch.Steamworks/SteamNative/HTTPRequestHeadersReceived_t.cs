using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct HTTPRequestHeadersReceived_t
	{
		internal const int CallbackId = 2102;

		internal uint Request;

		internal ulong ContextValue;

		internal static HTTPRequestHeadersReceived_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (HTTPRequestHeadersReceived_t)Marshal.PtrToStructure(p, typeof(HTTPRequestHeadersReceived_t));
			}
			return (HTTPRequestHeadersReceived_t.PackSmall)Marshal.PtrToStructure(p, typeof(HTTPRequestHeadersReceived_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return HTTPRequestHeadersReceived_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return HTTPRequestHeadersReceived_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			HTTPRequestHeadersReceived_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			HTTPRequestHeadersReceived_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			HTTPRequestHeadersReceived_t hTTPRequestHeadersReceivedT = HTTPRequestHeadersReceived_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<HTTPRequestHeadersReceived_t>(hTTPRequestHeadersReceivedT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<HTTPRequestHeadersReceived_t>(hTTPRequestHeadersReceivedT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			HTTPRequestHeadersReceived_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(HTTPRequestHeadersReceived_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(HTTPRequestHeadersReceived_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(HTTPRequestHeadersReceived_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(HTTPRequestHeadersReceived_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(HTTPRequestHeadersReceived_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(HTTPRequestHeadersReceived_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(HTTPRequestHeadersReceived_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(HTTPRequestHeadersReceived_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(HTTPRequestHeadersReceived_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(HTTPRequestHeadersReceived_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(HTTPRequestHeadersReceived_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(HTTPRequestHeadersReceived_t.OnGetSize)
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
				CallbackId = 2102
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 2102);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(HTTPRequestHeadersReceived_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(HTTPRequestHeadersReceived_t));
		}

		internal struct PackSmall
		{
			internal uint Request;

			internal ulong ContextValue;

			public static implicit operator HTTPRequestHeadersReceived_t(HTTPRequestHeadersReceived_t.PackSmall d)
			{
				HTTPRequestHeadersReceived_t hTTPRequestHeadersReceivedT = new HTTPRequestHeadersReceived_t()
				{
					Request = d.Request,
					ContextValue = d.ContextValue
				};
				return hTTPRequestHeadersReceivedT;
			}
		}
	}
}