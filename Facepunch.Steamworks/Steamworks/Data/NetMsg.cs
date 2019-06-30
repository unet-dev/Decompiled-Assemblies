using System;
using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	internal struct NetMsg
	{
		internal IntPtr DataPtr;

		internal int DataSize;

		internal Steamworks.Data.Connection Connection;

		internal NetIdentity Identity;

		internal long ConnectionUserData;

		internal long RecvTime;

		internal long MessageNumber;

		internal IntPtr FreeDataPtr;

		internal IntPtr ReleasePtr;

		internal int Channel;

		public void Release(IntPtr data)
		{
			Marshal.GetDelegateForFunctionPointer<NetMsg.ReleaseDelegate>(this.ReleasePtr)(data);
		}

		internal delegate void ReleaseDelegate(IntPtr msg);
	}
}