using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct GetAppDependenciesResult_t
	{
		internal const int CallbackId = 3416;

		internal SteamNative.Result Result;

		internal ulong PublishedFileId;

		internal AppId_t[] GAppIDs;

		internal uint NumAppDependencies;

		internal uint TotalNumAppDependencies;

		internal static CallResult<GetAppDependenciesResult_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<GetAppDependenciesResult_t, bool> CallbackFunction)
		{
			return new CallResult<GetAppDependenciesResult_t>(steamworks, call, CallbackFunction, new CallResult<GetAppDependenciesResult_t>.ConvertFromPointer(GetAppDependenciesResult_t.FromPointer), GetAppDependenciesResult_t.StructSize(), 3416);
		}

		internal static GetAppDependenciesResult_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (GetAppDependenciesResult_t)Marshal.PtrToStructure(p, typeof(GetAppDependenciesResult_t));
			}
			return (GetAppDependenciesResult_t.PackSmall)Marshal.PtrToStructure(p, typeof(GetAppDependenciesResult_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return GetAppDependenciesResult_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return GetAppDependenciesResult_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			GetAppDependenciesResult_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			GetAppDependenciesResult_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			GetAppDependenciesResult_t getAppDependenciesResultT = GetAppDependenciesResult_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<GetAppDependenciesResult_t>(getAppDependenciesResultT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<GetAppDependenciesResult_t>(getAppDependenciesResultT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			GetAppDependenciesResult_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(GetAppDependenciesResult_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(GetAppDependenciesResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(GetAppDependenciesResult_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(GetAppDependenciesResult_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(GetAppDependenciesResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(GetAppDependenciesResult_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(GetAppDependenciesResult_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(GetAppDependenciesResult_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(GetAppDependenciesResult_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(GetAppDependenciesResult_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(GetAppDependenciesResult_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(GetAppDependenciesResult_t.OnGetSize)
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
				CallbackId = 3416
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 3416);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(GetAppDependenciesResult_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(GetAppDependenciesResult_t));
		}

		internal struct PackSmall
		{
			internal SteamNative.Result Result;

			internal ulong PublishedFileId;

			internal AppId_t[] GAppIDs;

			internal uint NumAppDependencies;

			internal uint TotalNumAppDependencies;

			public static implicit operator GetAppDependenciesResult_t(GetAppDependenciesResult_t.PackSmall d)
			{
				GetAppDependenciesResult_t getAppDependenciesResultT = new GetAppDependenciesResult_t()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId,
					GAppIDs = d.GAppIDs,
					NumAppDependencies = d.NumAppDependencies,
					TotalNumAppDependencies = d.TotalNumAppDependencies
				};
				return getAppDependenciesResultT;
			}
		}
	}
}