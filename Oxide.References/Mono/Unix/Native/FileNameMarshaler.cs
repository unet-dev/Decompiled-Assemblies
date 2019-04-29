using Mono.Unix;
using System;
using System.Runtime.InteropServices;

namespace Mono.Unix.Native
{
	internal class FileNameMarshaler : ICustomMarshaler
	{
		private static FileNameMarshaler Instance;

		static FileNameMarshaler()
		{
			FileNameMarshaler.Instance = new FileNameMarshaler();
		}

		public FileNameMarshaler()
		{
		}

		public void CleanUpManagedData(object o)
		{
		}

		public void CleanUpNativeData(IntPtr pNativeData)
		{
			UnixMarshal.FreeHeap(pNativeData);
		}

		public static ICustomMarshaler GetInstance(string s)
		{
			return FileNameMarshaler.Instance;
		}

		public int GetNativeDataSize()
		{
			return IntPtr.Size;
		}

		public IntPtr MarshalManagedToNative(object obj)
		{
			string str = obj as string;
			if (str == null)
			{
				return IntPtr.Zero;
			}
			return UnixMarshal.StringToHeap(str, UnixEncoding.Instance);
		}

		public object MarshalNativeToManaged(IntPtr pNativeData)
		{
			return UnixMarshal.PtrToString(pNativeData, UnixEncoding.Instance);
		}
	}
}