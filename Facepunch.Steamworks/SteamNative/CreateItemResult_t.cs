using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct CreateItemResult_t
	{
		internal const int CallbackId = 3403;

		internal SteamNative.Result Result;

		internal ulong PublishedFileId;

		internal bool UserNeedsToAcceptWorkshopLegalAgreement;

		internal static CallResult<CreateItemResult_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<CreateItemResult_t, bool> CallbackFunction)
		{
			return new CallResult<CreateItemResult_t>(steamworks, call, CallbackFunction, new CallResult<CreateItemResult_t>.ConvertFromPointer(CreateItemResult_t.FromPointer), CreateItemResult_t.StructSize(), 3403);
		}

		internal static CreateItemResult_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (CreateItemResult_t)Marshal.PtrToStructure(p, typeof(CreateItemResult_t));
			}
			return (CreateItemResult_t.PackSmall)Marshal.PtrToStructure(p, typeof(CreateItemResult_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return CreateItemResult_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return CreateItemResult_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			CreateItemResult_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			CreateItemResult_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			CreateItemResult_t createItemResultT = CreateItemResult_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<CreateItemResult_t>(createItemResultT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<CreateItemResult_t>(createItemResultT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			CreateItemResult_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(CreateItemResult_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(CreateItemResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(CreateItemResult_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(CreateItemResult_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(CreateItemResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(CreateItemResult_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(CreateItemResult_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(CreateItemResult_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(CreateItemResult_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(CreateItemResult_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(CreateItemResult_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(CreateItemResult_t.OnGetSize)
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
				CallbackId = 3403
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 3403);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(CreateItemResult_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(CreateItemResult_t));
		}

		internal struct PackSmall
		{
			internal SteamNative.Result Result;

			internal ulong PublishedFileId;

			internal bool UserNeedsToAcceptWorkshopLegalAgreement;

			public static implicit operator CreateItemResult_t(CreateItemResult_t.PackSmall d)
			{
				CreateItemResult_t createItemResultT = new CreateItemResult_t()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId,
					UserNeedsToAcceptWorkshopLegalAgreement = d.UserNeedsToAcceptWorkshopLegalAgreement
				};
				return createItemResultT;
			}
		}
	}
}