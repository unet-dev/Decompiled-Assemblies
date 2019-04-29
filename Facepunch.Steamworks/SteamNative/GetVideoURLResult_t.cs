using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct GetVideoURLResult_t
	{
		internal const int CallbackId = 4611;

		internal SteamNative.Result Result;

		internal uint VideoAppID;

		internal string URL;

		internal static GetVideoURLResult_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (GetVideoURLResult_t)Marshal.PtrToStructure(p, typeof(GetVideoURLResult_t));
			}
			return (GetVideoURLResult_t.PackSmall)Marshal.PtrToStructure(p, typeof(GetVideoURLResult_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return GetVideoURLResult_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return GetVideoURLResult_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			GetVideoURLResult_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			GetVideoURLResult_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			GetVideoURLResult_t getVideoURLResultT = GetVideoURLResult_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<GetVideoURLResult_t>(getVideoURLResultT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<GetVideoURLResult_t>(getVideoURLResultT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			GetVideoURLResult_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(GetVideoURLResult_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(GetVideoURLResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(GetVideoURLResult_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(GetVideoURLResult_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(GetVideoURLResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(GetVideoURLResult_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(GetVideoURLResult_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(GetVideoURLResult_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(GetVideoURLResult_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(GetVideoURLResult_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(GetVideoURLResult_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(GetVideoURLResult_t.OnGetSize)
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
				CallbackId = 4611
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 4611);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(GetVideoURLResult_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(GetVideoURLResult_t));
		}

		internal struct PackSmall
		{
			internal SteamNative.Result Result;

			internal uint VideoAppID;

			internal string URL;

			public static implicit operator GetVideoURLResult_t(GetVideoURLResult_t.PackSmall d)
			{
				GetVideoURLResult_t getVideoURLResultT = new GetVideoURLResult_t()
				{
					Result = d.Result,
					VideoAppID = d.VideoAppID,
					URL = d.URL
				};
				return getVideoURLResultT;
			}
		}
	}
}