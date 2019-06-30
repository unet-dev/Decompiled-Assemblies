using Steamworks;
using System;
using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	internal struct CallbackMsg_t
	{
		internal int SteamUser;

		internal int Callback;

		internal IntPtr ParamPtr;

		internal int ParamCount;

		internal static CallbackMsg_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (CallbackMsg_t)Marshal.PtrToStructure(p, typeof(CallbackMsg_t)) : (CallbackMsg_t.Pack8)Marshal.PtrToStructure(p, typeof(CallbackMsg_t.Pack8)));
		}

		public struct Pack8
		{
			internal int SteamUser;

			internal int Callback;

			internal IntPtr ParamPtr;

			internal int ParamCount;

			public static implicit operator CallbackMsg_t(CallbackMsg_t.Pack8 d)
			{
				CallbackMsg_t callbackMsgT = new CallbackMsg_t()
				{
					SteamUser = d.SteamUser,
					Callback = d.Callback,
					ParamPtr = d.ParamPtr,
					ParamCount = d.ParamCount
				};
				return callbackMsgT;
			}

			public static implicit operator Pack8(CallbackMsg_t d)
			{
				CallbackMsg_t.Pack8 pack8 = new CallbackMsg_t.Pack8()
				{
					SteamUser = d.SteamUser,
					Callback = d.Callback,
					ParamPtr = d.ParamPtr,
					ParamCount = d.ParamCount
				};
				return pack8;
			}
		}
	}
}