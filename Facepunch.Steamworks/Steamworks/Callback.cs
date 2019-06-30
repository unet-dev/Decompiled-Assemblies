using Steamworks.Data;
using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	internal class Callback
	{
		public IntPtr vTablePtr;

		public byte CallbackFlags;

		public int CallbackId;

		public Callback()
		{
		}

		[MonoPInvokeCallback]
		internal static void RunStub(IntPtr self, IntPtr param, bool failure, SteamAPICall_t call)
		{
			throw new Exception("Something changed in the Steam API and now CCallbackBack is calling the CallResult function [Run( void *pvParam, bool bIOFailure, SteamAPICall_t hSteamAPICall )]");
		}

		[MonoPInvokeCallback]
		internal static int SizeStub(IntPtr self)
		{
			throw new Exception("Something changed in the Steam API and now CCallbackBack is calling the GetSize function [GetCallbackSizeBytes()]");
		}

		internal enum Flags : byte
		{
			Registered = 1,
			GameServer = 2
		}

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		public delegate int GetCallbackSizeBytes(IntPtr thisptr);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		public delegate void Run(IntPtr thisptr, IntPtr pvParam);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		public delegate void RunCall(IntPtr thisptr, IntPtr pvParam, bool bIOFailure, SteamAPICall_t hSteamAPICall);
	}
}