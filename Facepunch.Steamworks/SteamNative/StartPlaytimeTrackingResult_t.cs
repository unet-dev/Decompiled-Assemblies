using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct StartPlaytimeTrackingResult_t
	{
		internal const int CallbackId = 3410;

		internal SteamNative.Result Result;

		internal static CallResult<StartPlaytimeTrackingResult_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<StartPlaytimeTrackingResult_t, bool> CallbackFunction)
		{
			return new CallResult<StartPlaytimeTrackingResult_t>(steamworks, call, CallbackFunction, new CallResult<StartPlaytimeTrackingResult_t>.ConvertFromPointer(StartPlaytimeTrackingResult_t.FromPointer), StartPlaytimeTrackingResult_t.StructSize(), 3410);
		}

		internal static StartPlaytimeTrackingResult_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (StartPlaytimeTrackingResult_t)Marshal.PtrToStructure(p, typeof(StartPlaytimeTrackingResult_t));
			}
			return (StartPlaytimeTrackingResult_t.PackSmall)Marshal.PtrToStructure(p, typeof(StartPlaytimeTrackingResult_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return StartPlaytimeTrackingResult_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return StartPlaytimeTrackingResult_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			StartPlaytimeTrackingResult_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			StartPlaytimeTrackingResult_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			StartPlaytimeTrackingResult_t startPlaytimeTrackingResultT = StartPlaytimeTrackingResult_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<StartPlaytimeTrackingResult_t>(startPlaytimeTrackingResultT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<StartPlaytimeTrackingResult_t>(startPlaytimeTrackingResultT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			StartPlaytimeTrackingResult_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(StartPlaytimeTrackingResult_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(StartPlaytimeTrackingResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(StartPlaytimeTrackingResult_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(StartPlaytimeTrackingResult_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(StartPlaytimeTrackingResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(StartPlaytimeTrackingResult_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(StartPlaytimeTrackingResult_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(StartPlaytimeTrackingResult_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(StartPlaytimeTrackingResult_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(StartPlaytimeTrackingResult_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(StartPlaytimeTrackingResult_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(StartPlaytimeTrackingResult_t.OnGetSize)
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
				CallbackId = 3410
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 3410);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(StartPlaytimeTrackingResult_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(StartPlaytimeTrackingResult_t));
		}

		internal struct PackSmall
		{
			internal SteamNative.Result Result;

			public static implicit operator StartPlaytimeTrackingResult_t(StartPlaytimeTrackingResult_t.PackSmall d)
			{
				return new StartPlaytimeTrackingResult_t()
				{
					Result = d.Result
				};
			}
		}
	}
}