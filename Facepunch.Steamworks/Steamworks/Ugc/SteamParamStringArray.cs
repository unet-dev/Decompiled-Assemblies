using Steamworks.Data;
using System;
using System.Runtime.InteropServices;

namespace Steamworks.Ugc
{
	internal struct SteamParamStringArray : IDisposable
	{
		public SteamParamStringArray_t Value;

		private IntPtr[] NativeStrings;

		private IntPtr NativeArray;

		public void Dispose()
		{
			IntPtr[] nativeStrings = this.NativeStrings;
			for (int i = 0; i < (int)nativeStrings.Length; i++)
			{
				Marshal.FreeHGlobal(nativeStrings[i]);
			}
			Marshal.FreeHGlobal(this.NativeArray);
		}

		public static SteamParamStringArray From(string[] array)
		{
			SteamParamStringArray hGlobalAnsi = new SteamParamStringArray()
			{
				NativeStrings = new IntPtr[(int)array.Length]
			};
			for (int i = 0; i < (int)hGlobalAnsi.NativeStrings.Length; i++)
			{
				hGlobalAnsi.NativeStrings[i] = Marshal.StringToHGlobalAnsi(array[i]);
			}
			int num = Marshal.SizeOf(typeof(IntPtr)) * (int)hGlobalAnsi.NativeStrings.Length;
			hGlobalAnsi.NativeArray = Marshal.AllocHGlobal(num);
			Marshal.Copy(hGlobalAnsi.NativeStrings, 0, hGlobalAnsi.NativeArray, (int)hGlobalAnsi.NativeStrings.Length);
			SteamParamStringArray_t steamParamStringArrayT = new SteamParamStringArray_t()
			{
				Strings = hGlobalAnsi.NativeArray,
				NumStrings = (int)array.Length
			};
			hGlobalAnsi.Value = steamParamStringArrayT;
			return hGlobalAnsi;
		}
	}
}