using Mono.Unix.Native;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Mono.Unix
{
	[Serializable]
	public class UnixIOException : IOException
	{
		private int errno;

		public Errno ErrorCode
		{
			get
			{
				return NativeConvert.ToErrno(this.errno);
			}
		}

		public int NativeErrorCode
		{
			get
			{
				return this.errno;
			}
		}

		public UnixIOException() : this(Marshal.GetLastWin32Error())
		{
		}

		public UnixIOException(int errno) : base(UnixIOException.GetMessage(NativeConvert.ToErrno(errno)))
		{
			this.errno = errno;
		}

		public UnixIOException(int errno, Exception inner) : base(UnixIOException.GetMessage(NativeConvert.ToErrno(errno)), inner)
		{
			this.errno = errno;
		}

		public UnixIOException(Errno errno) : base(UnixIOException.GetMessage(errno))
		{
			this.errno = NativeConvert.FromErrno(errno);
		}

		public UnixIOException(Errno errno, Exception inner) : base(UnixIOException.GetMessage(errno), inner)
		{
			this.errno = NativeConvert.FromErrno(errno);
		}

		public UnixIOException(string message) : base(message)
		{
			this.errno = 0;
		}

		public UnixIOException(string message, Exception inner) : base(message, inner)
		{
			this.errno = 0;
		}

		protected UnixIOException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		private static string GetMessage(Errno errno)
		{
			return string.Format("{0} [{1}].", UnixMarshal.GetErrorDescription(errno), errno);
		}
	}
}