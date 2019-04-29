using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct CallbackMsg_t
	{
		internal int SteamUser;

		internal int Callback;

		internal IntPtr ParamPtr;

		internal int ParamCount;

		internal static CallbackMsg_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (CallbackMsg_t)Marshal.PtrToStructure(p, typeof(CallbackMsg_t));
			}
			return (CallbackMsg_t.PackSmall)Marshal.PtrToStructure(p, typeof(CallbackMsg_t.PackSmall));
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(CallbackMsg_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(CallbackMsg_t));
		}

		internal struct PackSmall
		{
			internal int SteamUser;

			internal int Callback;

			internal IntPtr ParamPtr;

			internal int ParamCount;

			public static implicit operator CallbackMsg_t(CallbackMsg_t.PackSmall d)
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
		}
	}
}