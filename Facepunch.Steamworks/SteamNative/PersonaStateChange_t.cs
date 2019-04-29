using Facepunch.Steamworks;
using Facepunch.Steamworks.Interop;
using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct PersonaStateChange_t
	{
		internal const int CallbackId = 304;

		internal ulong SteamID;

		internal int ChangeFlags;

		internal static PersonaStateChange_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (PersonaStateChange_t)Marshal.PtrToStructure(p, typeof(PersonaStateChange_t));
			}
			return (PersonaStateChange_t.PackSmall)Marshal.PtrToStructure(p, typeof(PersonaStateChange_t.PackSmall));
		}

		[MonoPInvokeCallback]
		internal static int OnGetSize()
		{
			return PersonaStateChange_t.StructSize();
		}

		[MonoPInvokeCallback]
		internal static int OnGetSizeThis(IntPtr self)
		{
			return PersonaStateChange_t.OnGetSize();
		}

		[MonoPInvokeCallback]
		internal static void OnResult(IntPtr param)
		{
			PersonaStateChange_t.OnResultWithInfo(param, false, (long)0);
		}

		[MonoPInvokeCallback]
		internal static void OnResultThis(IntPtr self, IntPtr param)
		{
			PersonaStateChange_t.OnResult(param);
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfo(IntPtr param, bool failure, SteamAPICall_t call)
		{
			if (failure)
			{
				return;
			}
			PersonaStateChange_t personaStateChangeT = PersonaStateChange_t.FromPointer(param);
			if (Client.Instance != null)
			{
				Client.Instance.OnCallback<PersonaStateChange_t>(personaStateChangeT);
			}
			if (Server.Instance != null)
			{
				Server.Instance.OnCallback<PersonaStateChange_t>(personaStateChangeT);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnResultWithInfoThis(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			PersonaStateChange_t.OnResultWithInfo(param, failure, call);
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
						ResultA = new Callback.VTableThis.ResultD(PersonaStateChange_t.OnResultThis),
						ResultB = new Callback.VTableThis.ResultWithInfoD(PersonaStateChange_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableThis.GetSizeD(PersonaStateChange_t.OnGetSizeThis)
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
						ResultA = new Callback.VTableWinThis.ResultD(PersonaStateChange_t.OnResultThis),
						ResultB = new Callback.VTableWinThis.ResultWithInfoD(PersonaStateChange_t.OnResultWithInfoThis),
						GetSize = new Callback.VTableWinThis.GetSizeD(PersonaStateChange_t.OnGetSizeThis)
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
					ResultA = new Callback.VTable.ResultD(PersonaStateChange_t.OnResult),
					ResultB = new Callback.VTable.ResultWithInfoD(PersonaStateChange_t.OnResultWithInfo),
					GetSize = new Callback.VTable.GetSizeD(PersonaStateChange_t.OnGetSize)
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
					ResultA = new Callback.VTableWin.ResultD(PersonaStateChange_t.OnResult),
					ResultB = new Callback.VTableWin.ResultWithInfoD(PersonaStateChange_t.OnResultWithInfo),
					GetSize = new Callback.VTableWin.GetSizeD(PersonaStateChange_t.OnGetSize)
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
				CallbackId = 304
			};
			callbackHandle.PinnedCallback = GCHandle.Alloc(callback, GCHandleType.Pinned);
			steamworks.native.api.SteamAPI_RegisterCallback(callbackHandle.PinnedCallback.AddrOfPinnedObject(), 304);
			steamworks.RegisterCallbackHandle(callbackHandle);
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(PersonaStateChange_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(PersonaStateChange_t));
		}

		internal struct PackSmall
		{
			internal ulong SteamID;

			internal int ChangeFlags;

			public static implicit operator PersonaStateChange_t(PersonaStateChange_t.PackSmall d)
			{
				PersonaStateChange_t personaStateChangeT = new PersonaStateChange_t()
				{
					SteamID = d.SteamID,
					ChangeFlags = d.ChangeFlags
				};
				return personaStateChangeT;
			}
		}
	}
}