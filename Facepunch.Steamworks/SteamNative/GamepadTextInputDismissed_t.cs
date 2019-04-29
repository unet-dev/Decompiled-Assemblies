using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct GamepadTextInputDismissed_t
	{
		internal const int CallbackId = 714;

		internal bool Submitted;

		internal uint SubmittedText;

		internal static GamepadTextInputDismissed_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (GamepadTextInputDismissed_t)Marshal.PtrToStructure(p, typeof(GamepadTextInputDismissed_t));
			}
			return (GamepadTextInputDismissed_t.PackSmall)Marshal.PtrToStructure(p, typeof(GamepadTextInputDismissed_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return GamepadTextInputDismissed_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return GamepadTextInputDismissed_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			GamepadTextInputDismissed_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			GamepadTextInputDismissed_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			GamepadTextInputDismissed_t gamepadTextInputDismissedT = GamepadTextInputDismissed_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<GamepadTextInputDismissed_t>(gamepadTextInputDismissedT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<GamepadTextInputDismissed_t>(gamepadTextInputDismissedT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			GamepadTextInputDismissed_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(GamepadTextInputDismissed_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(GamepadTextInputDismissed_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(GamepadTextInputDismissed_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(GamepadTextInputDismissed_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(GamepadTextInputDismissed_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(GamepadTextInputDismissed_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(GamepadTextInputDismissed_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(GamepadTextInputDismissed_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(GamepadTextInputDismissed_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(GamepadTextInputDismissed_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(GamepadTextInputDismissed_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(GamepadTextInputDismissed_t.OnGetSize)
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
				CallbackId = 714
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 714);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(GamepadTextInputDismissed_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(GamepadTextInputDismissed_t));
		}

		internal struct PackSmall
		{
			internal bool Submitted;

			internal uint SubmittedText;

			public static implicit operator GamepadTextInputDismissed_t(GamepadTextInputDismissed_t.PackSmall d)
			{
				GamepadTextInputDismissed_t gamepadTextInputDismissedT = new GamepadTextInputDismissed_t()
				{
					Submitted = d.Submitted,
					SubmittedText = d.SubmittedText
				};
				return gamepadTextInputDismissedT;
			}
		}
	}
}