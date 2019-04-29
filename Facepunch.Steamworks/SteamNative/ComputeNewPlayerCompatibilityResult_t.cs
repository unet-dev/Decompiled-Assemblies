using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct ComputeNewPlayerCompatibilityResult_t
	{
		internal const int CallbackId = 211;

		internal SteamNative.Result Result;

		internal int CPlayersThatDontLikeCandidate;

		internal int CPlayersThatCandidateDoesntLike;

		internal int CClanPlayersThatDontLikeCandidate;

		internal ulong SteamIDCandidate;

		internal static CallResult<ComputeNewPlayerCompatibilityResult_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<ComputeNewPlayerCompatibilityResult_t, bool> CallbackFunction)
		{
			return new CallResult<ComputeNewPlayerCompatibilityResult_t>(steamworks, call, CallbackFunction, new CallResult<ComputeNewPlayerCompatibilityResult_t>.ConvertFromPointer(ComputeNewPlayerCompatibilityResult_t.FromPointer), ComputeNewPlayerCompatibilityResult_t.StructSize(), 211);
		}

		internal static ComputeNewPlayerCompatibilityResult_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (ComputeNewPlayerCompatibilityResult_t)Marshal.PtrToStructure(p, typeof(ComputeNewPlayerCompatibilityResult_t));
			}
			return (ComputeNewPlayerCompatibilityResult_t.PackSmall)Marshal.PtrToStructure(p, typeof(ComputeNewPlayerCompatibilityResult_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return ComputeNewPlayerCompatibilityResult_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return ComputeNewPlayerCompatibilityResult_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			ComputeNewPlayerCompatibilityResult_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			ComputeNewPlayerCompatibilityResult_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			ComputeNewPlayerCompatibilityResult_t computeNewPlayerCompatibilityResultT = ComputeNewPlayerCompatibilityResult_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<ComputeNewPlayerCompatibilityResult_t>(computeNewPlayerCompatibilityResultT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<ComputeNewPlayerCompatibilityResult_t>(computeNewPlayerCompatibilityResultT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			ComputeNewPlayerCompatibilityResult_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(ComputeNewPlayerCompatibilityResult_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(ComputeNewPlayerCompatibilityResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(ComputeNewPlayerCompatibilityResult_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(ComputeNewPlayerCompatibilityResult_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(ComputeNewPlayerCompatibilityResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(ComputeNewPlayerCompatibilityResult_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(ComputeNewPlayerCompatibilityResult_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(ComputeNewPlayerCompatibilityResult_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(ComputeNewPlayerCompatibilityResult_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(ComputeNewPlayerCompatibilityResult_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(ComputeNewPlayerCompatibilityResult_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(ComputeNewPlayerCompatibilityResult_t.OnGetSize)
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
				CallbackId = 211
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 211);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(ComputeNewPlayerCompatibilityResult_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(ComputeNewPlayerCompatibilityResult_t));
		}

		internal struct PackSmall
		{
			internal SteamNative.Result Result;

			internal int CPlayersThatDontLikeCandidate;

			internal int CPlayersThatCandidateDoesntLike;

			internal int CClanPlayersThatDontLikeCandidate;

			internal ulong SteamIDCandidate;

			public static implicit operator ComputeNewPlayerCompatibilityResult_t(ComputeNewPlayerCompatibilityResult_t.PackSmall d)
			{
				ComputeNewPlayerCompatibilityResult_t computeNewPlayerCompatibilityResultT = new ComputeNewPlayerCompatibilityResult_t()
				{
					Result = d.Result,
					CPlayersThatDontLikeCandidate = d.CPlayersThatDontLikeCandidate,
					CPlayersThatCandidateDoesntLike = d.CPlayersThatCandidateDoesntLike,
					CClanPlayersThatDontLikeCandidate = d.CClanPlayersThatDontLikeCandidate,
					SteamIDCandidate = d.SteamIDCandidate
				};
				return computeNewPlayerCompatibilityResultT;
			}
		}
	}
}