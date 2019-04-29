using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct AddAppDependencyResult_t
	{
		internal const int CallbackId = 3414;

		internal SteamNative.Result Result;

		internal ulong PublishedFileId;

		internal uint AppID;

		internal static CallResult<AddAppDependencyResult_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<AddAppDependencyResult_t, bool> CallbackFunction)
		{
			return new CallResult<AddAppDependencyResult_t>(steamworks, call, CallbackFunction, new CallResult<AddAppDependencyResult_t>.ConvertFromPointer(AddAppDependencyResult_t.FromPointer), AddAppDependencyResult_t.StructSize(), 3414);
		}

		internal static AddAppDependencyResult_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (AddAppDependencyResult_t)Marshal.PtrToStructure(p, typeof(AddAppDependencyResult_t));
			}
			return (AddAppDependencyResult_t.PackSmall)Marshal.PtrToStructure(p, typeof(AddAppDependencyResult_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return AddAppDependencyResult_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return AddAppDependencyResult_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			AddAppDependencyResult_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			AddAppDependencyResult_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			AddAppDependencyResult_t addAppDependencyResultT = AddAppDependencyResult_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<AddAppDependencyResult_t>(addAppDependencyResultT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<AddAppDependencyResult_t>(addAppDependencyResultT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			AddAppDependencyResult_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(AddAppDependencyResult_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(AddAppDependencyResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(AddAppDependencyResult_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(AddAppDependencyResult_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(AddAppDependencyResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(AddAppDependencyResult_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(AddAppDependencyResult_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(AddAppDependencyResult_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(AddAppDependencyResult_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(AddAppDependencyResult_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(AddAppDependencyResult_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(AddAppDependencyResult_t.OnGetSize)
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
				CallbackId = 3414
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 3414);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(AddAppDependencyResult_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(AddAppDependencyResult_t));
		}

		internal struct PackSmall
		{
			internal SteamNative.Result Result;

			internal ulong PublishedFileId;

			internal uint AppID;

			public static implicit operator AddAppDependencyResult_t(AddAppDependencyResult_t.PackSmall d)
			{
				AddAppDependencyResult_t addAppDependencyResultT = new AddAppDependencyResult_t()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId,
					AppID = d.AppID
				};
				return addAppDependencyResultT;
			}
		}
	}
}