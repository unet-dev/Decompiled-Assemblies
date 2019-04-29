using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct AddUGCDependencyResult_t
	{
		internal const int CallbackId = 3412;

		internal SteamNative.Result Result;

		internal ulong PublishedFileId;

		internal ulong ChildPublishedFileId;

		internal static CallResult<AddUGCDependencyResult_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<AddUGCDependencyResult_t, bool> CallbackFunction)
		{
			return new CallResult<AddUGCDependencyResult_t>(steamworks, call, CallbackFunction, new CallResult<AddUGCDependencyResult_t>.ConvertFromPointer(AddUGCDependencyResult_t.FromPointer), AddUGCDependencyResult_t.StructSize(), 3412);
		}

		internal static AddUGCDependencyResult_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (AddUGCDependencyResult_t)Marshal.PtrToStructure(p, typeof(AddUGCDependencyResult_t));
			}
			return (AddUGCDependencyResult_t.PackSmall)Marshal.PtrToStructure(p, typeof(AddUGCDependencyResult_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return AddUGCDependencyResult_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return AddUGCDependencyResult_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			AddUGCDependencyResult_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			AddUGCDependencyResult_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			AddUGCDependencyResult_t addUGCDependencyResultT = AddUGCDependencyResult_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<AddUGCDependencyResult_t>(addUGCDependencyResultT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<AddUGCDependencyResult_t>(addUGCDependencyResultT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			AddUGCDependencyResult_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(AddUGCDependencyResult_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(AddUGCDependencyResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(AddUGCDependencyResult_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(AddUGCDependencyResult_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(AddUGCDependencyResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(AddUGCDependencyResult_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(AddUGCDependencyResult_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(AddUGCDependencyResult_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(AddUGCDependencyResult_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(AddUGCDependencyResult_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(AddUGCDependencyResult_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(AddUGCDependencyResult_t.OnGetSize)
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
				CallbackId = 3412
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 3412);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(AddUGCDependencyResult_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(AddUGCDependencyResult_t));
		}

		internal struct PackSmall
		{
			internal SteamNative.Result Result;

			internal ulong PublishedFileId;

			internal ulong ChildPublishedFileId;

			public static implicit operator AddUGCDependencyResult_t(AddUGCDependencyResult_t.PackSmall d)
			{
				AddUGCDependencyResult_t addUGCDependencyResultT = new AddUGCDependencyResult_t()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId,
					ChildPublishedFileId = d.ChildPublishedFileId
				};
				return addUGCDependencyResultT;
			}
		}
	}
}