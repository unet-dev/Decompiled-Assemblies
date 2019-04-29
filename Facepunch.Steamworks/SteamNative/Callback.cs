using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal class Callback
	{
		public IntPtr vTablePtr;

		public byte CallbackFlags;

		public int CallbackId;

		public Callback()
		{
		}

		internal enum Flags : byte
		{
			Registered = 1,
			GameServer = 2
		}

		public class VTable
		{
			public Callback.VTable.ResultD ResultA;

			public Callback.VTable.ResultWithInfoD ResultB;

			public Callback.VTable.GetSizeD GetSize;

			public VTable()
			{
			}

			[UnmanagedFunctionPointer(CallingConvention.StdCall)]
			public delegate int GetSizeD();

			[UnmanagedFunctionPointer(CallingConvention.StdCall)]
			public delegate void ResultD(IntPtr pvParam);

			[UnmanagedFunctionPointer(CallingConvention.StdCall)]
			public delegate void ResultWithInfoD(IntPtr pvParam, bool bIOFailure, SteamAPICall_t hSteamAPICall);
		}

		public class VTableThis
		{
			public Callback.VTableThis.ResultD ResultA;

			public Callback.VTableThis.ResultWithInfoD ResultB;

			public Callback.VTableThis.GetSizeD GetSize;

			public VTableThis()
			{
			}

			[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
			public delegate int GetSizeD(IntPtr thisptr);

			[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
			public delegate void ResultD(IntPtr thisptr, IntPtr pvParam);

			[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
			public delegate void ResultWithInfoD(IntPtr thisptr, IntPtr pvParam, bool bIOFailure, SteamAPICall_t hSteamAPICall);
		}

		public class VTableWin
		{
			public Callback.VTableWin.ResultWithInfoD ResultB;

			public Callback.VTableWin.ResultD ResultA;

			public Callback.VTableWin.GetSizeD GetSize;

			public VTableWin()
			{
			}

			[UnmanagedFunctionPointer(CallingConvention.StdCall)]
			public delegate int GetSizeD();

			[UnmanagedFunctionPointer(CallingConvention.StdCall)]
			public delegate void ResultD(IntPtr pvParam);

			[UnmanagedFunctionPointer(CallingConvention.StdCall)]
			public delegate void ResultWithInfoD(IntPtr pvParam, bool bIOFailure, SteamAPICall_t hSteamAPICall);
		}

		public class VTableWinThis
		{
			public Callback.VTableWinThis.ResultWithInfoD ResultB;

			public Callback.VTableWinThis.ResultD ResultA;

			public Callback.VTableWinThis.GetSizeD GetSize;

			public VTableWinThis()
			{
			}

			[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
			public delegate int GetSizeD(IntPtr thisptr);

			[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
			public delegate void ResultD(IntPtr thisptr, IntPtr pvParam);

			[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
			public delegate void ResultWithInfoD(IntPtr thisptr, IntPtr pvParam, bool bIOFailure, SteamAPICall_t hSteamAPICall);
		}
	}
}