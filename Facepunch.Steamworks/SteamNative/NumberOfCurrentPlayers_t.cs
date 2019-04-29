using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct NumberOfCurrentPlayers_t
	{
		internal const int CallbackId = 1107;

		internal byte Success;

		internal int CPlayers;

		internal static CallResult<NumberOfCurrentPlayers_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<NumberOfCurrentPlayers_t, bool> CallbackFunction)
		{
			return new CallResult<NumberOfCurrentPlayers_t>(steamworks, call, CallbackFunction, new CallResult<NumberOfCurrentPlayers_t>.ConvertFromPointer(NumberOfCurrentPlayers_t.FromPointer), NumberOfCurrentPlayers_t.StructSize(), 1107);
		}

		internal static NumberOfCurrentPlayers_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (NumberOfCurrentPlayers_t)Marshal.PtrToStructure(p, typeof(NumberOfCurrentPlayers_t));
			}
			return (NumberOfCurrentPlayers_t.PackSmall)Marshal.PtrToStructure(p, typeof(NumberOfCurrentPlayers_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return NumberOfCurrentPlayers_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return NumberOfCurrentPlayers_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			NumberOfCurrentPlayers_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			NumberOfCurrentPlayers_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			NumberOfCurrentPlayers_t numberOfCurrentPlayersT = NumberOfCurrentPlayers_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<NumberOfCurrentPlayers_t>(numberOfCurrentPlayersT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<NumberOfCurrentPlayers_t>(numberOfCurrentPlayersT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			NumberOfCurrentPlayers_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(NumberOfCurrentPlayers_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(NumberOfCurrentPlayers_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(NumberOfCurrentPlayers_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(NumberOfCurrentPlayers_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(NumberOfCurrentPlayers_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(NumberOfCurrentPlayers_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(NumberOfCurrentPlayers_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(NumberOfCurrentPlayers_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(NumberOfCurrentPlayers_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(NumberOfCurrentPlayers_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(NumberOfCurrentPlayers_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(NumberOfCurrentPlayers_t.OnGetSize)
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
				CallbackId = 1107
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 1107);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(NumberOfCurrentPlayers_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(NumberOfCurrentPlayers_t));
		}

		internal struct PackSmall
		{
			internal byte Success;

			internal int CPlayers;

			public static implicit operator NumberOfCurrentPlayers_t(NumberOfCurrentPlayers_t.PackSmall d)
			{
				NumberOfCurrentPlayers_t numberOfCurrentPlayersT = new NumberOfCurrentPlayers_t()
				{
					Success = d.Success,
					CPlayers = d.CPlayers
				};
				return numberOfCurrentPlayersT;
			}
		}
	}
}