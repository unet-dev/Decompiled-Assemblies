using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct ValidateAuthTicketResponse_t
	{
		internal const int CallbackId = 143;

		internal ulong SteamID;

		internal SteamNative.AuthSessionResponse AuthSessionResponse;

		internal ulong OwnerSteamID;

		internal static ValidateAuthTicketResponse_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (ValidateAuthTicketResponse_t)Marshal.PtrToStructure(p, typeof(ValidateAuthTicketResponse_t));
			}
			return (ValidateAuthTicketResponse_t.PackSmall)Marshal.PtrToStructure(p, typeof(ValidateAuthTicketResponse_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return ValidateAuthTicketResponse_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return ValidateAuthTicketResponse_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			ValidateAuthTicketResponse_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			ValidateAuthTicketResponse_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			ValidateAuthTicketResponse_t validateAuthTicketResponseT = ValidateAuthTicketResponse_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<ValidateAuthTicketResponse_t>(validateAuthTicketResponseT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<ValidateAuthTicketResponse_t>(validateAuthTicketResponseT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			ValidateAuthTicketResponse_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(ValidateAuthTicketResponse_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(ValidateAuthTicketResponse_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(ValidateAuthTicketResponse_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(ValidateAuthTicketResponse_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(ValidateAuthTicketResponse_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(ValidateAuthTicketResponse_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(ValidateAuthTicketResponse_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(ValidateAuthTicketResponse_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(ValidateAuthTicketResponse_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(ValidateAuthTicketResponse_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(ValidateAuthTicketResponse_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(ValidateAuthTicketResponse_t.OnGetSize)
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
				CallbackId = 143
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 143);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(ValidateAuthTicketResponse_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(ValidateAuthTicketResponse_t));
		}

		internal struct PackSmall
		{
			internal ulong SteamID;

			internal SteamNative.AuthSessionResponse AuthSessionResponse;

			internal ulong OwnerSteamID;

			public static implicit operator ValidateAuthTicketResponse_t(ValidateAuthTicketResponse_t.PackSmall d)
			{
				ValidateAuthTicketResponse_t validateAuthTicketResponseT = new ValidateAuthTicketResponse_t()
				{
					SteamID = d.SteamID,
					AuthSessionResponse = d.AuthSessionResponse,
					OwnerSteamID = d.OwnerSteamID
				};
				return validateAuthTicketResponseT;
			}
		}
	}
}