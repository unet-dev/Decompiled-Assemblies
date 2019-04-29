using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct SteamAPICallCompleted_t
	{
		internal const int CallbackId = 703;

		internal ulong AsyncCall;

		internal int Callback;

		internal uint ParamCount;

		internal static SteamAPICallCompleted_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (SteamAPICallCompleted_t)Marshal.PtrToStructure(p, typeof(SteamAPICallCompleted_t));
			}
			return (SteamAPICallCompleted_t.PackSmall)Marshal.PtrToStructure(p, typeof(SteamAPICallCompleted_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return SteamAPICallCompleted_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return SteamAPICallCompleted_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			SteamAPICallCompleted_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			SteamAPICallCompleted_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			SteamAPICallCompleted_t steamAPICallCompletedT = SteamAPICallCompleted_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<SteamAPICallCompleted_t>(steamAPICallCompletedT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<SteamAPICallCompleted_t>(steamAPICallCompletedT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			SteamAPICallCompleted_t.OnResultWithInfo(param, failure, call);
		}

		internal static void Register(BaseSteamworks steamworks)
		{
			CallbackHandle callbackHandle = new CallbackHandle(steamworks);
			if (Config.UseThisCall)
			{
				if (!Platform.IsWindows)
				{
					callbackHandle.vTablePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SteamNative.Callback.VTableThis)));
					SteamNative.Callback.VTableThis vTableThi = new SteamNative.Callback.VTableThis()
					{
						ResultA = new SteamNative.Callback.VTableThis.ResultD(SteamAPICallCompleted_t.OnResultThis),
						ResultB = new SteamNative.Callback.VTableThis.ResultWithInfoD(SteamAPICallCompleted_t.OnResultWithInfoThis),
						GetSize = new SteamNative.Callback.VTableThis.GetSizeD(SteamAPICallCompleted_t.OnGetSizeThis)
					};
					callbackHandle.FuncA = GCHandle.Alloc(vTableThi.ResultA);
					callbackHandle.FuncB = GCHandle.Alloc(vTableThi.ResultB);
					callbackHandle.FuncC = GCHandle.Alloc(vTableThi.GetSize);
					Marshal.StructureToPtr(vTableThi, callbackHandle.vTablePtr, false);
				}
				else
				{
					callbackHandle.vTablePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SteamNative.Callback.VTableWinThis)));
					SteamNative.Callback.VTableWinThis vTableWinThi = new SteamNative.Callback.VTableWinThis()
					{
						ResultA = new SteamNative.Callback.VTableWinThis.ResultD(SteamAPICallCompleted_t.OnResultThis),
						ResultB = new SteamNative.Callback.VTableWinThis.ResultWithInfoD(SteamAPICallCompleted_t.OnResultWithInfoThis),
						GetSize = new SteamNative.Callback.VTableWinThis.GetSizeD(SteamAPICallCompleted_t.OnGetSizeThis)
					};
					callbackHandle.FuncA = GCHandle.Alloc(vTableWinThi.ResultA);
					callbackHandle.FuncB = GCHandle.Alloc(vTableWinThi.ResultB);
					callbackHandle.FuncC = GCHandle.Alloc(vTableWinThi.GetSize);
					Marshal.StructureToPtr(vTableWinThi, callbackHandle.vTablePtr, false);
				}
			}
			else if (!Platform.IsWindows)
			{
				callbackHandle.vTablePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SteamNative.Callback.VTable)));
				SteamNative.Callback.VTable vTable = new SteamNative.Callback.VTable()
				{
					ResultA = new SteamNative.Callback.VTable.ResultD(SteamAPICallCompleted_t.OnResult),
					ResultB = new SteamNative.Callback.VTable.ResultWithInfoD(SteamAPICallCompleted_t.OnResultWithInfo),
					GetSize = new SteamNative.Callback.VTable.GetSizeD(SteamAPICallCompleted_t.OnGetSize)
				};
				callbackHandle.FuncA = GCHandle.Alloc(vTable.ResultA);
				callbackHandle.FuncB = GCHandle.Alloc(vTable.ResultB);
				callbackHandle.FuncC = GCHandle.Alloc(vTable.GetSize);
				Marshal.StructureToPtr(vTable, callbackHandle.vTablePtr, false);
			}
			else
			{
				callbackHandle.vTablePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SteamNative.Callback.VTableWin)));
				SteamNative.Callback.VTableWin vTableWin = new SteamNative.Callback.VTableWin()
				{
					ResultA = new SteamNative.Callback.VTableWin.ResultD(SteamAPICallCompleted_t.OnResult),
					ResultB = new SteamNative.Callback.VTableWin.ResultWithInfoD(SteamAPICallCompleted_t.OnResultWithInfo),
					GetSize = new SteamNative.Callback.VTableWin.GetSizeD(SteamAPICallCompleted_t.OnGetSize)
				};
				callbackHandle.FuncA = GCHandle.Alloc(vTableWin.ResultA);
				callbackHandle.FuncB = GCHandle.Alloc(vTableWin.ResultB);
				callbackHandle.FuncC = GCHandle.Alloc(vTableWin.GetSize);
				Marshal.StructureToPtr(vTableWin, callbackHandle.vTablePtr, false);
			}
			SteamNative.Callback callback = new SteamNative.Callback()
			{
				vTablePtr = callbackHandle.vTablePtr,
				CallbackFlags = (byte)((steamworks.IsGameServer ? 2 : 0)),
				CallbackId = 703
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 703);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(SteamAPICallCompleted_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(SteamAPICallCompleted_t));
		}

		internal struct PackSmall
		{
			internal ulong AsyncCall;

			internal int Callback;

			internal uint ParamCount;

			public static implicit operator SteamAPICallCompleted_t(SteamAPICallCompleted_t.PackSmall d)
			{
				SteamAPICallCompleted_t steamAPICallCompletedT = new SteamAPICallCompleted_t()
				{
					AsyncCall = d.AsyncCall,
					Callback = d.Callback,
					ParamCount = d.ParamCount
				};
				return steamAPICallCompletedT;
			}
		}
	}
}