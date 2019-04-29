using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct JoinClanChatRoomCompletionResult_t
	{
		internal const int CallbackId = 342;

		internal ulong SteamIDClanChat;

		internal SteamNative.ChatRoomEnterResponse ChatRoomEnterResponse;

		internal static CallResult<JoinClanChatRoomCompletionResult_t> CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<JoinClanChatRoomCompletionResult_t, bool> CallbackFunction)
		{
			return new CallResult<JoinClanChatRoomCompletionResult_t>(steamworks, call, CallbackFunction, new CallResult<JoinClanChatRoomCompletionResult_t>.ConvertFromPointer(JoinClanChatRoomCompletionResult_t.FromPointer), JoinClanChatRoomCompletionResult_t.StructSize(), 342);
		}

		internal static JoinClanChatRoomCompletionResult_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (JoinClanChatRoomCompletionResult_t)Marshal.PtrToStructure(p, typeof(JoinClanChatRoomCompletionResult_t));
			}
			return (JoinClanChatRoomCompletionResult_t.PackSmall)Marshal.PtrToStructure(p, typeof(JoinClanChatRoomCompletionResult_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return JoinClanChatRoomCompletionResult_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return JoinClanChatRoomCompletionResult_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			JoinClanChatRoomCompletionResult_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			JoinClanChatRoomCompletionResult_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			JoinClanChatRoomCompletionResult_t joinClanChatRoomCompletionResultT = JoinClanChatRoomCompletionResult_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<JoinClanChatRoomCompletionResult_t>(joinClanChatRoomCompletionResultT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<JoinClanChatRoomCompletionResult_t>(joinClanChatRoomCompletionResultT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			JoinClanChatRoomCompletionResult_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(JoinClanChatRoomCompletionResult_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(JoinClanChatRoomCompletionResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(JoinClanChatRoomCompletionResult_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(JoinClanChatRoomCompletionResult_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(JoinClanChatRoomCompletionResult_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(JoinClanChatRoomCompletionResult_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(JoinClanChatRoomCompletionResult_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(JoinClanChatRoomCompletionResult_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(JoinClanChatRoomCompletionResult_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(JoinClanChatRoomCompletionResult_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(JoinClanChatRoomCompletionResult_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(JoinClanChatRoomCompletionResult_t.OnGetSize)
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
				CallbackId = 342
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 342);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(JoinClanChatRoomCompletionResult_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(JoinClanChatRoomCompletionResult_t));
		}

		internal struct PackSmall
		{
			internal ulong SteamIDClanChat;

			internal SteamNative.ChatRoomEnterResponse ChatRoomEnterResponse;

			public static implicit operator JoinClanChatRoomCompletionResult_t(JoinClanChatRoomCompletionResult_t.PackSmall d)
			{
				JoinClanChatRoomCompletionResult_t joinClanChatRoomCompletionResultT = new JoinClanChatRoomCompletionResult_t()
				{
					SteamIDClanChat = d.SteamIDClanChat,
					ChatRoomEnterResponse = d.ChatRoomEnterResponse
				};
				return joinClanChatRoomCompletionResultT;
			}
		}
	}
}