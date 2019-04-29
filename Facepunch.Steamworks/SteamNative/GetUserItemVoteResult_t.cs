using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct GetUserItemVoteResult_t
	{
		internal const int CallbackId = 3409;

		internal ulong PublishedFileId;

		internal SteamNative.Result Result;

		internal bool VotedUp;

		internal bool VotedDown;

		internal bool VoteSkipped;

		internal static CallResult<GetUserItemVoteResult_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<GetUserItemVoteResult_t, bool> CallbackFunction)
		{
			return new CallResult<GetUserItemVoteResult_t>(steamworks, call, CallbackFunction, new CallResult<GetUserItemVoteResult_t>.ConvertFromPointer(GetUserItemVoteResult_t.FromPointer), GetUserItemVoteResult_t.StructSize(), 3409);
		}

		internal static GetUserItemVoteResult_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (GetUserItemVoteResult_t)Marshal.PtrToStructure(p, typeof(GetUserItemVoteResult_t));
			}
			return (GetUserItemVoteResult_t.PackSmall)Marshal.PtrToStructure(p, typeof(GetUserItemVoteResult_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return GetUserItemVoteResult_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return GetUserItemVoteResult_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			GetUserItemVoteResult_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			GetUserItemVoteResult_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			GetUserItemVoteResult_t getUserItemVoteResultT = GetUserItemVoteResult_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<GetUserItemVoteResult_t>(getUserItemVoteResultT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<GetUserItemVoteResult_t>(getUserItemVoteResultT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			GetUserItemVoteResult_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(GetUserItemVoteResult_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(GetUserItemVoteResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(GetUserItemVoteResult_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(GetUserItemVoteResult_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(GetUserItemVoteResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(GetUserItemVoteResult_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(GetUserItemVoteResult_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(GetUserItemVoteResult_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(GetUserItemVoteResult_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(GetUserItemVoteResult_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(GetUserItemVoteResult_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(GetUserItemVoteResult_t.OnGetSize)
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
				CallbackId = 3409
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 3409);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(GetUserItemVoteResult_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(GetUserItemVoteResult_t));
		}

		internal struct PackSmall
		{
			internal ulong PublishedFileId;

			internal SteamNative.Result Result;

			internal bool VotedUp;

			internal bool VotedDown;

			internal bool VoteSkipped;

			public static implicit operator GetUserItemVoteResult_t(GetUserItemVoteResult_t.PackSmall d)
			{
				GetUserItemVoteResult_t getUserItemVoteResultT = new GetUserItemVoteResult_t()
				{
					PublishedFileId = d.PublishedFileId,
					Result = d.Result,
					VotedUp = d.VotedUp,
					VotedDown = d.VotedDown,
					VoteSkipped = d.VoteSkipped
				};
				return getUserItemVoteResultT;
			}
		}
	}
}