using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct HTTPRequestDataReceived_t
	{
		internal const int CallbackId = 2103;

		internal uint Request;

		internal ulong ContextValue;

		internal uint COffset;

		internal uint CBytesReceived;

		internal static HTTPRequestDataReceived_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (HTTPRequestDataReceived_t)Marshal.PtrToStructure(p, typeof(HTTPRequestDataReceived_t));
			}
			return (HTTPRequestDataReceived_t.PackSmall)Marshal.PtrToStructure(p, typeof(HTTPRequestDataReceived_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return HTTPRequestDataReceived_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return HTTPRequestDataReceived_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			HTTPRequestDataReceived_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			HTTPRequestDataReceived_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			HTTPRequestDataReceived_t hTTPRequestDataReceivedT = HTTPRequestDataReceived_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<HTTPRequestDataReceived_t>(hTTPRequestDataReceivedT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<HTTPRequestDataReceived_t>(hTTPRequestDataReceivedT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			HTTPRequestDataReceived_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(HTTPRequestDataReceived_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(HTTPRequestDataReceived_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(HTTPRequestDataReceived_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(HTTPRequestDataReceived_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(HTTPRequestDataReceived_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(HTTPRequestDataReceived_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(HTTPRequestDataReceived_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(HTTPRequestDataReceived_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(HTTPRequestDataReceived_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(HTTPRequestDataReceived_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(HTTPRequestDataReceived_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(HTTPRequestDataReceived_t.OnGetSize)
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
				CallbackId = 2103
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 2103);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(HTTPRequestDataReceived_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(HTTPRequestDataReceived_t));
		}

		internal struct PackSmall
		{
			internal uint Request;

			internal ulong ContextValue;

			internal uint COffset;

			internal uint CBytesReceived;

			public static implicit operator HTTPRequestDataReceived_t(HTTPRequestDataReceived_t.PackSmall d)
			{
				HTTPRequestDataReceived_t hTTPRequestDataReceivedT = new HTTPRequestDataReceived_t()
				{
					Request = d.Request,
					ContextValue = d.ContextValue,
					COffset = d.COffset,
					CBytesReceived = d.CBytesReceived
				};
				return hTTPRequestDataReceivedT;
			}
		}
	}
}