using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct SetPersonaNameResponse_t
	{
		internal const int CallbackId = 347;

		internal bool Success;

		internal bool LocalSuccess;

		internal SteamNative.Result Result;

		internal static CallResult<SetPersonaNameResponse_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<SetPersonaNameResponse_t, bool> CallbackFunction)
		{
			return new CallResult<SetPersonaNameResponse_t>(steamworks, call, CallbackFunction, new CallResult<SetPersonaNameResponse_t>.ConvertFromPointer(SetPersonaNameResponse_t.FromPointer), SetPersonaNameResponse_t.StructSize(), 347);
		}

		internal static SetPersonaNameResponse_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (SetPersonaNameResponse_t)Marshal.PtrToStructure(p, typeof(SetPersonaNameResponse_t));
			}
			return (SetPersonaNameResponse_t.PackSmall)Marshal.PtrToStructure(p, typeof(SetPersonaNameResponse_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return SetPersonaNameResponse_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return SetPersonaNameResponse_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			SetPersonaNameResponse_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			SetPersonaNameResponse_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			SetPersonaNameResponse_t setPersonaNameResponseT = SetPersonaNameResponse_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<SetPersonaNameResponse_t>(setPersonaNameResponseT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<SetPersonaNameResponse_t>(setPersonaNameResponseT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			SetPersonaNameResponse_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(SetPersonaNameResponse_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(SetPersonaNameResponse_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(SetPersonaNameResponse_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(SetPersonaNameResponse_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(SetPersonaNameResponse_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(SetPersonaNameResponse_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(SetPersonaNameResponse_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(SetPersonaNameResponse_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(SetPersonaNameResponse_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(SetPersonaNameResponse_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(SetPersonaNameResponse_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(SetPersonaNameResponse_t.OnGetSize)
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
				CallbackId = 347
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 347);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(SetPersonaNameResponse_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(SetPersonaNameResponse_t));
		}

		internal struct PackSmall
		{
			internal bool Success;

			internal bool LocalSuccess;

			internal SteamNative.Result Result;

			public static implicit operator SetPersonaNameResponse_t(SetPersonaNameResponse_t.PackSmall d)
			{
				SetPersonaNameResponse_t setPersonaNameResponseT = new SetPersonaNameResponse_t()
				{
					Success = d.Success,
					LocalSuccess = d.LocalSuccess,
					Result = d.Result
				};
				return setPersonaNameResponseT;
			}
		}
	}
}