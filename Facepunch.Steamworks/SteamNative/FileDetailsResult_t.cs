using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct FileDetailsResult_t
	{
		internal const int CallbackId = 1023;

		internal SteamNative.Result Result;

		internal ulong FileSize;

		internal char FileSHA;

		internal uint Flags;

		internal static CallResult<FileDetailsResult_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<FileDetailsResult_t, bool> CallbackFunction)
		{
			return new CallResult<FileDetailsResult_t>(steamworks, call, CallbackFunction, new CallResult<FileDetailsResult_t>.ConvertFromPointer(FileDetailsResult_t.FromPointer), FileDetailsResult_t.StructSize(), 1023);
		}

		internal static FileDetailsResult_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (FileDetailsResult_t)Marshal.PtrToStructure(p, typeof(FileDetailsResult_t));
			}
			return (FileDetailsResult_t.PackSmall)Marshal.PtrToStructure(p, typeof(FileDetailsResult_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return FileDetailsResult_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return FileDetailsResult_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			FileDetailsResult_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			FileDetailsResult_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			FileDetailsResult_t fileDetailsResultT = FileDetailsResult_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<FileDetailsResult_t>(fileDetailsResultT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<FileDetailsResult_t>(fileDetailsResultT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			FileDetailsResult_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(FileDetailsResult_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(FileDetailsResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(FileDetailsResult_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(FileDetailsResult_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(FileDetailsResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(FileDetailsResult_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(FileDetailsResult_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(FileDetailsResult_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(FileDetailsResult_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(FileDetailsResult_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(FileDetailsResult_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(FileDetailsResult_t.OnGetSize)
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
				CallbackId = 1023
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 1023);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(FileDetailsResult_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(FileDetailsResult_t));
		}

		internal struct PackSmall
		{
			internal SteamNative.Result Result;

			internal ulong FileSize;

			internal char FileSHA;

			internal uint Flags;

			public static implicit operator FileDetailsResult_t(FileDetailsResult_t.PackSmall d)
			{
				FileDetailsResult_t fileDetailsResultT = new FileDetailsResult_t()
				{
					Result = d.Result,
					FileSize = d.FileSize,
					FileSHA = d.FileSHA,
					Flags = d.Flags
				};
				return fileDetailsResultT;
			}
		}
	}
}