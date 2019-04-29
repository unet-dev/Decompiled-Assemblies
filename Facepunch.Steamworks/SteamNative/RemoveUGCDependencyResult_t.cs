using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct RemoveUGCDependencyResult_t
	{
		internal const int CallbackId = 3413;

		internal SteamNative.Result Result;

		internal ulong PublishedFileId;

		internal ulong ChildPublishedFileId;

		internal static CallResult<RemoveUGCDependencyResult_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<RemoveUGCDependencyResult_t, bool> CallbackFunction)
		{
			return new CallResult<RemoveUGCDependencyResult_t>(steamworks, call, CallbackFunction, new CallResult<RemoveUGCDependencyResult_t>.ConvertFromPointer(RemoveUGCDependencyResult_t.FromPointer), RemoveUGCDependencyResult_t.StructSize(), 3413);
		}

		internal static RemoveUGCDependencyResult_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (RemoveUGCDependencyResult_t)Marshal.PtrToStructure(p, typeof(RemoveUGCDependencyResult_t));
			}
			return (RemoveUGCDependencyResult_t.PackSmall)Marshal.PtrToStructure(p, typeof(RemoveUGCDependencyResult_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return RemoveUGCDependencyResult_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return RemoveUGCDependencyResult_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			RemoveUGCDependencyResult_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			RemoveUGCDependencyResult_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			RemoveUGCDependencyResult_t removeUGCDependencyResultT = RemoveUGCDependencyResult_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<RemoveUGCDependencyResult_t>(removeUGCDependencyResultT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<RemoveUGCDependencyResult_t>(removeUGCDependencyResultT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			RemoveUGCDependencyResult_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(RemoveUGCDependencyResult_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(RemoveUGCDependencyResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(RemoveUGCDependencyResult_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(RemoveUGCDependencyResult_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(RemoveUGCDependencyResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(RemoveUGCDependencyResult_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(RemoveUGCDependencyResult_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(RemoveUGCDependencyResult_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(RemoveUGCDependencyResult_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(RemoveUGCDependencyResult_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(RemoveUGCDependencyResult_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(RemoveUGCDependencyResult_t.OnGetSize)
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
				CallbackId = 3413
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 3413);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(RemoveUGCDependencyResult_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(RemoveUGCDependencyResult_t));
		}

		internal struct PackSmall
		{
			internal SteamNative.Result Result;

			internal ulong PublishedFileId;

			internal ulong ChildPublishedFileId;

			public static implicit operator RemoveUGCDependencyResult_t(RemoveUGCDependencyResult_t.PackSmall d)
			{
				RemoveUGCDependencyResult_t removeUGCDependencyResultT = new RemoveUGCDependencyResult_t()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId,
					ChildPublishedFileId = d.ChildPublishedFileId
				};
				return removeUGCDependencyResultT;
			}
		}
	}
}