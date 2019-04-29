using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct SetUserItemVoteResult_t
	{
		internal const int CallbackId = 3408;

		internal ulong PublishedFileId;

		internal SteamNative.Result Result;

		internal bool VoteUp;

		internal static CallResult<SetUserItemVoteResult_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<SetUserItemVoteResult_t, bool> CallbackFunction)
		{
			return new CallResult<SetUserItemVoteResult_t>(steamworks, call, CallbackFunction, new CallResult<SetUserItemVoteResult_t>.ConvertFromPointer(SetUserItemVoteResult_t.FromPointer), SetUserItemVoteResult_t.StructSize(), 3408);
		}

		internal static SetUserItemVoteResult_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (SetUserItemVoteResult_t)Marshal.PtrToStructure(p, typeof(SetUserItemVoteResult_t));
			}
			return (SetUserItemVoteResult_t.PackSmall)Marshal.PtrToStructure(p, typeof(SetUserItemVoteResult_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return SetUserItemVoteResult_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return SetUserItemVoteResult_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			SetUserItemVoteResult_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			SetUserItemVoteResult_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			SetUserItemVoteResult_t setUserItemVoteResultT = SetUserItemVoteResult_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<SetUserItemVoteResult_t>(setUserItemVoteResultT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<SetUserItemVoteResult_t>(setUserItemVoteResultT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			SetUserItemVoteResult_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(SetUserItemVoteResult_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(SetUserItemVoteResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(SetUserItemVoteResult_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(SetUserItemVoteResult_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(SetUserItemVoteResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(SetUserItemVoteResult_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(SetUserItemVoteResult_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(SetUserItemVoteResult_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(SetUserItemVoteResult_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(SetUserItemVoteResult_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(SetUserItemVoteResult_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(SetUserItemVoteResult_t.OnGetSize)
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
				CallbackId = 3408
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 3408);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(SetUserItemVoteResult_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(SetUserItemVoteResult_t));
		}

		internal struct PackSmall
		{
			internal ulong PublishedFileId;

			internal SteamNative.Result Result;

			internal bool VoteUp;

			public static implicit operator SetUserItemVoteResult_t(SetUserItemVoteResult_t.PackSmall d)
			{
				SetUserItemVoteResult_t setUserItemVoteResultT = new SetUserItemVoteResult_t()
				{
					PublishedFileId = d.PublishedFileId,
					Result = d.Result,
					VoteUp = d.VoteUp
				};
				return setUserItemVoteResultT;
			}
		}
	}
}