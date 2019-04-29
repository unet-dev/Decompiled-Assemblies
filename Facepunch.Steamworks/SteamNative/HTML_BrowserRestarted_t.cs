using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct HTML_BrowserRestarted_t
	{
		internal const int CallbackId = 4527;

		internal uint UnBrowserHandle;

		internal uint UnOldBrowserHandle;

		internal static HTML_BrowserRestarted_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (HTML_BrowserRestarted_t)Marshal.PtrToStructure(p, typeof(HTML_BrowserRestarted_t));
			}
			return (HTML_BrowserRestarted_t.PackSmall)Marshal.PtrToStructure(p, typeof(HTML_BrowserRestarted_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return HTML_BrowserRestarted_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return HTML_BrowserRestarted_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			HTML_BrowserRestarted_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			HTML_BrowserRestarted_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			HTML_BrowserRestarted_t hTMLBrowserRestartedT = HTML_BrowserRestarted_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<HTML_BrowserRestarted_t>(hTMLBrowserRestartedT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<HTML_BrowserRestarted_t>(hTMLBrowserRestartedT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			HTML_BrowserRestarted_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(HTML_BrowserRestarted_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(HTML_BrowserRestarted_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(HTML_BrowserRestarted_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(HTML_BrowserRestarted_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(HTML_BrowserRestarted_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(HTML_BrowserRestarted_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(HTML_BrowserRestarted_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(HTML_BrowserRestarted_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(HTML_BrowserRestarted_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(HTML_BrowserRestarted_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(HTML_BrowserRestarted_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(HTML_BrowserRestarted_t.OnGetSize)
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
				CallbackId = 4527
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 4527);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(HTML_BrowserRestarted_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(HTML_BrowserRestarted_t));
		}

		internal struct PackSmall
		{
			internal uint UnBrowserHandle;

			internal uint UnOldBrowserHandle;

			public static implicit operator HTML_BrowserRestarted_t(HTML_BrowserRestarted_t.PackSmall d)
			{
				HTML_BrowserRestarted_t hTMLBrowserRestartedT = new HTML_BrowserRestarted_t()
				{
					UnBrowserHandle = d.UnBrowserHandle,
					UnOldBrowserHandle = d.UnOldBrowserHandle
				};
				return hTMLBrowserRestartedT;
			}
		}
	}
}