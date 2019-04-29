using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct AssociateWithClanResult_t
	{
		internal const int CallbackId = 210;

		internal SteamNative.Result Result;

		internal static CallResult<AssociateWithClanResult_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<AssociateWithClanResult_t, bool> CallbackFunction)
		{
			return new CallResult<AssociateWithClanResult_t>(steamworks, call, CallbackFunction, new CallResult<AssociateWithClanResult_t>.ConvertFromPointer(AssociateWithClanResult_t.FromPointer), AssociateWithClanResult_t.StructSize(), 210);
		}

		internal static AssociateWithClanResult_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (AssociateWithClanResult_t)Marshal.PtrToStructure(p, typeof(AssociateWithClanResult_t));
			}
			return (AssociateWithClanResult_t.PackSmall)Marshal.PtrToStructure(p, typeof(AssociateWithClanResult_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return AssociateWithClanResult_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return AssociateWithClanResult_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			AssociateWithClanResult_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			AssociateWithClanResult_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			AssociateWithClanResult_t associateWithClanResultT = AssociateWithClanResult_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<AssociateWithClanResult_t>(associateWithClanResultT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<AssociateWithClanResult_t>(associateWithClanResultT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			AssociateWithClanResult_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(AssociateWithClanResult_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(AssociateWithClanResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(AssociateWithClanResult_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(AssociateWithClanResult_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(AssociateWithClanResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(AssociateWithClanResult_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(AssociateWithClanResult_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(AssociateWithClanResult_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(AssociateWithClanResult_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(AssociateWithClanResult_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(AssociateWithClanResult_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(AssociateWithClanResult_t.OnGetSize)
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
				CallbackId = 210
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 210);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(AssociateWithClanResult_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(AssociateWithClanResult_t));
		}

		internal struct PackSmall
		{
			internal SteamNative.Result Result;

			public static implicit operator AssociateWithClanResult_t(AssociateWithClanResult_t.PackSmall d)
			{
				return new AssociateWithClanResult_t()
				{
					Result = d.Result
				};
			}
		}
	}
}