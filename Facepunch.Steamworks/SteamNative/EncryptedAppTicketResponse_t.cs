using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct EncryptedAppTicketResponse_t
	{
		internal const int CallbackId = 154;

		internal SteamNative.Result Result;

		internal static CallResult<EncryptedAppTicketResponse_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<EncryptedAppTicketResponse_t, bool> CallbackFunction)
		{
			return new CallResult<EncryptedAppTicketResponse_t>(steamworks, call, CallbackFunction, new CallResult<EncryptedAppTicketResponse_t>.ConvertFromPointer(EncryptedAppTicketResponse_t.FromPointer), EncryptedAppTicketResponse_t.StructSize(), 154);
		}

		internal static EncryptedAppTicketResponse_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (EncryptedAppTicketResponse_t)Marshal.PtrToStructure(p, typeof(EncryptedAppTicketResponse_t));
			}
			return (EncryptedAppTicketResponse_t.PackSmall)Marshal.PtrToStructure(p, typeof(EncryptedAppTicketResponse_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return EncryptedAppTicketResponse_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return EncryptedAppTicketResponse_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			EncryptedAppTicketResponse_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			EncryptedAppTicketResponse_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			EncryptedAppTicketResponse_t encryptedAppTicketResponseT = EncryptedAppTicketResponse_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<EncryptedAppTicketResponse_t>(encryptedAppTicketResponseT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<EncryptedAppTicketResponse_t>(encryptedAppTicketResponseT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			EncryptedAppTicketResponse_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(EncryptedAppTicketResponse_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(EncryptedAppTicketResponse_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(EncryptedAppTicketResponse_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(EncryptedAppTicketResponse_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(EncryptedAppTicketResponse_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(EncryptedAppTicketResponse_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(EncryptedAppTicketResponse_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(EncryptedAppTicketResponse_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(EncryptedAppTicketResponse_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(EncryptedAppTicketResponse_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(EncryptedAppTicketResponse_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(EncryptedAppTicketResponse_t.OnGetSize)
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
				CallbackId = 154
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 154);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(EncryptedAppTicketResponse_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(EncryptedAppTicketResponse_t));
		}

		internal struct PackSmall
		{
			internal SteamNative.Result Result;

			public static implicit operator EncryptedAppTicketResponse_t(EncryptedAppTicketResponse_t.PackSmall d)
			{
				return new EncryptedAppTicketResponse_t()
				{
					Result = d.Result
				};
			}
		}
	}
}