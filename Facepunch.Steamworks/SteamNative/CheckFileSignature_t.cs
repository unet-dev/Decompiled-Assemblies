using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct CheckFileSignature_t
	{
		internal const int CallbackId = 705;

		internal SteamNative.CheckFileSignature CheckFileSignature;

		internal static CallResult<CheckFileSignature_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<CheckFileSignature_t, bool> CallbackFunction)
		{
			return new CallResult<CheckFileSignature_t>(steamworks, call, CallbackFunction, new CallResult<CheckFileSignature_t>.ConvertFromPointer(CheckFileSignature_t.FromPointer), CheckFileSignature_t.StructSize(), 705);
		}

		internal static CheckFileSignature_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (CheckFileSignature_t)Marshal.PtrToStructure(p, typeof(CheckFileSignature_t));
			}
			return (CheckFileSignature_t.PackSmall)Marshal.PtrToStructure(p, typeof(CheckFileSignature_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return CheckFileSignature_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return CheckFileSignature_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			CheckFileSignature_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			CheckFileSignature_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			CheckFileSignature_t checkFileSignatureT = CheckFileSignature_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<CheckFileSignature_t>(checkFileSignatureT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<CheckFileSignature_t>(checkFileSignatureT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			CheckFileSignature_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(CheckFileSignature_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(CheckFileSignature_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(CheckFileSignature_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(CheckFileSignature_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(CheckFileSignature_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(CheckFileSignature_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(CheckFileSignature_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(CheckFileSignature_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(CheckFileSignature_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(CheckFileSignature_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(CheckFileSignature_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(CheckFileSignature_t.OnGetSize)
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
				CallbackId = 705
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 705);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(CheckFileSignature_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(CheckFileSignature_t));
		}

		internal struct PackSmall
		{
			internal SteamNative.CheckFileSignature CheckFileSignature;

			public static implicit operator CheckFileSignature_t(CheckFileSignature_t.PackSmall d)
			{
				return new CheckFileSignature_t()
				{
					CheckFileSignature = d.CheckFileSignature
				};
			}
		}
	}
}