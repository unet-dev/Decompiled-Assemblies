using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct SubmitItemUpdateResult_t
	{
		internal const int CallbackId = 3404;

		internal SteamNative.Result Result;

		internal bool UserNeedsToAcceptWorkshopLegalAgreement;

		internal ulong PublishedFileId;

		internal static CallResult<SubmitItemUpdateResult_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<SubmitItemUpdateResult_t, bool> CallbackFunction)
		{
			return new CallResult<SubmitItemUpdateResult_t>(steamworks, call, CallbackFunction, new CallResult<SubmitItemUpdateResult_t>.ConvertFromPointer(SubmitItemUpdateResult_t.FromPointer), SubmitItemUpdateResult_t.StructSize(), 3404);
		}

		internal static SubmitItemUpdateResult_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (SubmitItemUpdateResult_t)Marshal.PtrToStructure(p, typeof(SubmitItemUpdateResult_t));
			}
			return (SubmitItemUpdateResult_t.PackSmall)Marshal.PtrToStructure(p, typeof(SubmitItemUpdateResult_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return SubmitItemUpdateResult_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return SubmitItemUpdateResult_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			SubmitItemUpdateResult_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			SubmitItemUpdateResult_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			SubmitItemUpdateResult_t submitItemUpdateResultT = SubmitItemUpdateResult_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<SubmitItemUpdateResult_t>(submitItemUpdateResultT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<SubmitItemUpdateResult_t>(submitItemUpdateResultT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			SubmitItemUpdateResult_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(SubmitItemUpdateResult_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(SubmitItemUpdateResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(SubmitItemUpdateResult_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(SubmitItemUpdateResult_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(SubmitItemUpdateResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(SubmitItemUpdateResult_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(SubmitItemUpdateResult_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(SubmitItemUpdateResult_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(SubmitItemUpdateResult_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(SubmitItemUpdateResult_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(SubmitItemUpdateResult_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(SubmitItemUpdateResult_t.OnGetSize)
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
				CallbackId = 3404
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 3404);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(SubmitItemUpdateResult_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(SubmitItemUpdateResult_t));
		}

		internal struct PackSmall
		{
			internal SteamNative.Result Result;

			internal bool UserNeedsToAcceptWorkshopLegalAgreement;

			internal ulong PublishedFileId;

			public static implicit operator SubmitItemUpdateResult_t(SubmitItemUpdateResult_t.PackSmall d)
			{
				SubmitItemUpdateResult_t submitItemUpdateResultT = new SubmitItemUpdateResult_t()
				{
					Result = d.Result,
					UserNeedsToAcceptWorkshopLegalAgreement = d.UserNeedsToAcceptWorkshopLegalAgreement,
					PublishedFileId = d.PublishedFileId
				};
				return submitItemUpdateResultT;
			}
		}
	}
}