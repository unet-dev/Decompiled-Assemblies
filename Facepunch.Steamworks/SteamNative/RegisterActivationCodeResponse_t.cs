using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct RegisterActivationCodeResponse_t
	{
		internal const int CallbackId = 1008;

		internal RegisterActivationCodeResult Result;

		internal uint PackageRegistered;

		internal static RegisterActivationCodeResponse_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (RegisterActivationCodeResponse_t)Marshal.PtrToStructure(p, typeof(RegisterActivationCodeResponse_t));
			}
			return (RegisterActivationCodeResponse_t.PackSmall)Marshal.PtrToStructure(p, typeof(RegisterActivationCodeResponse_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return RegisterActivationCodeResponse_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return RegisterActivationCodeResponse_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			RegisterActivationCodeResponse_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			RegisterActivationCodeResponse_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			RegisterActivationCodeResponse_t registerActivationCodeResponseT = RegisterActivationCodeResponse_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<RegisterActivationCodeResponse_t>(registerActivationCodeResponseT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<RegisterActivationCodeResponse_t>(registerActivationCodeResponseT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			RegisterActivationCodeResponse_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(RegisterActivationCodeResponse_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(RegisterActivationCodeResponse_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(RegisterActivationCodeResponse_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(RegisterActivationCodeResponse_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(RegisterActivationCodeResponse_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(RegisterActivationCodeResponse_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(RegisterActivationCodeResponse_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(RegisterActivationCodeResponse_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(RegisterActivationCodeResponse_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(RegisterActivationCodeResponse_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(RegisterActivationCodeResponse_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(RegisterActivationCodeResponse_t.OnGetSize)
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
				CallbackId = 1008
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 1008);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(RegisterActivationCodeResponse_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(RegisterActivationCodeResponse_t));
		}

		internal struct PackSmall
		{
			internal RegisterActivationCodeResult Result;

			internal uint PackageRegistered;

			public static implicit operator RegisterActivationCodeResponse_t(RegisterActivationCodeResponse_t.PackSmall d)
			{
				RegisterActivationCodeResponse_t registerActivationCodeResponseT = new RegisterActivationCodeResponse_t()
				{
					Result = d.Result,
					PackageRegistered = d.PackageRegistered
				};
				return registerActivationCodeResponseT;
			}
		}
	}
}