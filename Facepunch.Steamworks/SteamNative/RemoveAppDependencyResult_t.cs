using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct RemoveAppDependencyResult_t
	{
		internal const int CallbackId = 3415;

		internal SteamNative.Result Result;

		internal ulong PublishedFileId;

		internal uint AppID;

		internal static CallResult<RemoveAppDependencyResult_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<RemoveAppDependencyResult_t, bool> CallbackFunction)
		{
			return new CallResult<RemoveAppDependencyResult_t>(steamworks, call, CallbackFunction, new CallResult<RemoveAppDependencyResult_t>.ConvertFromPointer(RemoveAppDependencyResult_t.FromPointer), RemoveAppDependencyResult_t.StructSize(), 3415);
		}

		internal static RemoveAppDependencyResult_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (RemoveAppDependencyResult_t)Marshal.PtrToStructure(p, typeof(RemoveAppDependencyResult_t));
			}
			return (RemoveAppDependencyResult_t.PackSmall)Marshal.PtrToStructure(p, typeof(RemoveAppDependencyResult_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return RemoveAppDependencyResult_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return RemoveAppDependencyResult_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			RemoveAppDependencyResult_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			RemoveAppDependencyResult_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			RemoveAppDependencyResult_t removeAppDependencyResultT = RemoveAppDependencyResult_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<RemoveAppDependencyResult_t>(removeAppDependencyResultT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<RemoveAppDependencyResult_t>(removeAppDependencyResultT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			RemoveAppDependencyResult_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(RemoveAppDependencyResult_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(RemoveAppDependencyResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(RemoveAppDependencyResult_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(RemoveAppDependencyResult_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(RemoveAppDependencyResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(RemoveAppDependencyResult_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(RemoveAppDependencyResult_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(RemoveAppDependencyResult_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(RemoveAppDependencyResult_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(RemoveAppDependencyResult_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(RemoveAppDependencyResult_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(RemoveAppDependencyResult_t.OnGetSize)
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
				CallbackId = 3415
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 3415);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(RemoveAppDependencyResult_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(RemoveAppDependencyResult_t));
		}

		internal struct PackSmall
		{
			internal SteamNative.Result Result;

			internal ulong PublishedFileId;

			internal uint AppID;

			public static implicit operator RemoveAppDependencyResult_t(RemoveAppDependencyResult_t.PackSmall d)
			{
				RemoveAppDependencyResult_t removeAppDependencyResultT = new RemoveAppDependencyResult_t()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId,
					AppID = d.AppID
				};
				return removeAppDependencyResultT;
			}
		}
	}
}