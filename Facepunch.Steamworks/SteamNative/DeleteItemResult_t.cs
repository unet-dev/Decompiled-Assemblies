using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct DeleteItemResult_t
	{
		internal const int CallbackId = 3417;

		internal SteamNative.Result Result;

		internal ulong PublishedFileId;

		internal static CallResult<DeleteItemResult_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<DeleteItemResult_t, bool> CallbackFunction)
		{
			return new CallResult<DeleteItemResult_t>(steamworks, call, CallbackFunction, new CallResult<DeleteItemResult_t>.ConvertFromPointer(DeleteItemResult_t.FromPointer), DeleteItemResult_t.StructSize(), 3417);
		}

		internal static DeleteItemResult_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (DeleteItemResult_t)Marshal.PtrToStructure(p, typeof(DeleteItemResult_t));
			}
			return (DeleteItemResult_t.PackSmall)Marshal.PtrToStructure(p, typeof(DeleteItemResult_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return DeleteItemResult_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return DeleteItemResult_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			DeleteItemResult_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			DeleteItemResult_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			DeleteItemResult_t deleteItemResultT = DeleteItemResult_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<DeleteItemResult_t>(deleteItemResultT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<DeleteItemResult_t>(deleteItemResultT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			DeleteItemResult_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(DeleteItemResult_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(DeleteItemResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(DeleteItemResult_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(DeleteItemResult_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(DeleteItemResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(DeleteItemResult_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(DeleteItemResult_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(DeleteItemResult_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(DeleteItemResult_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(DeleteItemResult_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(DeleteItemResult_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(DeleteItemResult_t.OnGetSize)
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
				CallbackId = 3417
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 3417);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(DeleteItemResult_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(DeleteItemResult_t));
		}

		internal struct PackSmall
		{
			internal SteamNative.Result Result;

			internal ulong PublishedFileId;

			public static implicit operator DeleteItemResult_t(DeleteItemResult_t.PackSmall d)
			{
				DeleteItemResult_t deleteItemResultT = new DeleteItemResult_t()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId
				};
				return deleteItemResultT;
			}
		}
	}
}