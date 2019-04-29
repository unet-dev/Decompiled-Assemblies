using Mono.Unix.Native;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Mono.Unix
{
	public sealed class UnixMarshal
	{
		private UnixMarshal()
		{
		}

		public static IntPtr AllocHeap(long size)
		{
			if (size < (long)0)
			{
				throw new ArgumentOutOfRangeException("size", "< 0");
			}
			return Stdlib.malloc((ulong)size);
		}

		private static int CountStrings(IntPtr stringArray)
		{
			int num = 0;
			while (Marshal.ReadIntPtr(stringArray, num * IntPtr.Size) != IntPtr.Zero)
			{
				num++;
			}
			return num;
		}

		internal static Exception CreateExceptionForError(Errno errno)
		{
			string errorDescription = UnixMarshal.GetErrorDescription(errno);
			UnixIOException unixIOException = new UnixIOException(errno);
			Errno errno1 = errno;
			switch (errno1)
			{
				case Errno.EPERM:
				{
					return new InvalidOperationException(errorDescription, unixIOException);
				}
				case Errno.ENOENT:
				{
					return new FileNotFoundException(errorDescription, unixIOException);
				}
				case Errno.EIO:
				case Errno.ENXIO:
				{
					return new IOException(errorDescription, unixIOException);
				}
				case Errno.ENOEXEC:
				{
					return new InvalidProgramException(errorDescription, unixIOException);
				}
				case Errno.EBADF:
				case Errno.EINVAL:
				{
					return new ArgumentException(errorDescription, unixIOException);
				}
				case Errno.EACCES:
				case Errno.EISDIR:
				{
					return new UnauthorizedAccessException(errorDescription, unixIOException);
				}
				case Errno.EFAULT:
				{
					return new NullReferenceException(errorDescription, unixIOException);
				}
				case Errno.ENOTDIR:
				{
					return new DirectoryNotFoundException(errorDescription, unixIOException);
				}
				default:
				{
					switch (errno1)
					{
						case Errno.ENOSPC:
						case Errno.ESPIPE:
						case Errno.EROFS:
						case Errno.ENOTEMPTY:
						{
							return new IOException(errorDescription, unixIOException);
						}
						case Errno.ERANGE:
						{
							return new ArgumentOutOfRangeException(errorDescription);
						}
						case Errno.ENAMETOOLONG:
						{
							return new PathTooLongException(errorDescription, unixIOException);
						}
						default:
						{
							if (errno1 == Errno.EOVERFLOW)
							{
								return new OverflowException(errorDescription, unixIOException);
							}
							if (errno1 == Errno.EOPNOTSUPP)
							{
								return new InvalidOperationException(errorDescription, unixIOException);
							}
							return unixIOException;
						}
					}
					break;
				}
			}
		}

		internal static Exception CreateExceptionForLastError()
		{
			return UnixMarshal.CreateExceptionForError(Stdlib.GetLastError());
		}

		internal static string EscapeFormatString(string message, char[] permitted)
		{
			if (message == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder(message.Length);
			for (int i = 0; i < message.Length; i++)
			{
				char chr = message[i];
				stringBuilder.Append(chr);
				if (chr == '%' && i + 1 < message.Length)
				{
					char chr1 = message[i + 1];
					if (chr1 == '%' || UnixMarshal.IsCharPresent(permitted, chr1))
					{
						stringBuilder.Append(chr1);
					}
					else
					{
						stringBuilder.Append('%').Append(chr1);
					}
					i++;
				}
				else if (chr == '%')
				{
					stringBuilder.Append('%');
				}
			}
			return stringBuilder.ToString();
		}

		public static void FreeHeap(IntPtr ptr)
		{
			Stdlib.free(ptr);
		}

		[CLSCompliant(false)]
		public static string GetErrorDescription(Errno errno)
		{
			return ErrorMarshal.Translate(errno);
		}

		private static int GetInt16BufferLength(IntPtr p)
		{
			int num = 0;
			while (Marshal.ReadInt16(p, num * 2) != 0)
			{
				num = checked(num + 1);
			}
			return checked(num * 2);
		}

		private static int GetInt32BufferLength(IntPtr p)
		{
			int num = 0;
			while (Marshal.ReadInt32(p, num * 4) != 0)
			{
				num = checked(num + 1);
			}
			return checked(num * 4);
		}

		private static int GetRandomBufferLength(IntPtr p, int nullLength)
		{
			int num;
			int num1;
			int num2;
			switch (nullLength)
			{
				case 1:
				{
					return (void*)(checked((int)Stdlib.strlen(p)));
				}
				case 2:
				{
					return UnixMarshal.GetInt16BufferLength(p);
				}
				case 3:
				{
					num = 0;
					num1 = 0;
					do
					{
						num2 = num;
						num = num2 + 1;
						if (Marshal.ReadByte(p, num2) != 0)
						{
							num1 = 0;
						}
						else
						{
							num1++;
						}
					}
					while (num1 != nullLength);
					return num;
				}
				case 4:
				{
					return UnixMarshal.GetInt32BufferLength(p);
				}
				default:
				{
					num = 0;
					num1 = 0;
					do
					{
						num2 = num;
						num = num2 + 1;
						if (Marshal.ReadByte(p, num2) != 0)
						{
							num1 = 0;
						}
						else
						{
							num1++;
						}
					}
					while (num1 != nullLength);
					return num;
				}
			}
		}

		private static int GetStringByteLength(IntPtr p, Encoding encoding)
		{
			Type type = encoding.GetType();
			int num = -1;
			if (typeof(UTF8Encoding).IsAssignableFrom(type) || typeof(UTF7Encoding).IsAssignableFrom(type) || typeof(UnixEncoding).IsAssignableFrom(type) || typeof(ASCIIEncoding).IsAssignableFrom(type))
			{
				num = (void*)(checked((int)Stdlib.strlen(p)));
			}
			else
			{
				num = (!typeof(UnicodeEncoding).IsAssignableFrom(type) ? UnixMarshal.GetRandomBufferLength(p, encoding.GetMaxByteCount(1)) : UnixMarshal.GetInt16BufferLength(p));
			}
			if (num == -1)
			{
				throw new NotSupportedException("Unable to determine native string buffer length");
			}
			return num;
		}

		private static bool IsCharPresent(char[] array, char c)
		{
			if (array == null)
			{
				return false;
			}
			for (int i = 0; i < (int)array.Length; i++)
			{
				if (array[i] == c)
				{
					return true;
				}
			}
			return false;
		}

		public static string PtrToString(IntPtr p)
		{
			if (p == IntPtr.Zero)
			{
				return null;
			}
			return UnixMarshal.PtrToString(p, UnixEncoding.Instance);
		}

		public static string PtrToString(IntPtr p, Encoding encoding)
		{
			if (p == IntPtr.Zero)
			{
				return null;
			}
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			int stringByteLength = UnixMarshal.GetStringByteLength(p, encoding);
			string str = new string((void*)p, 0, stringByteLength, encoding);
			stringByteLength = str.Length;
			while (stringByteLength > 0 && str[stringByteLength - 1] == 0)
			{
				stringByteLength--;
			}
			if (stringByteLength == str.Length)
			{
				return str;
			}
			return str.Substring(0, stringByteLength);
		}

		public static string[] PtrToStringArray(IntPtr stringArray)
		{
			return UnixMarshal.PtrToStringArray(stringArray, UnixEncoding.Instance);
		}

		public static string[] PtrToStringArray(IntPtr stringArray, Encoding encoding)
		{
			if (stringArray == IntPtr.Zero)
			{
				return new string[0];
			}
			return UnixMarshal.PtrToStringArray(UnixMarshal.CountStrings(stringArray), stringArray, encoding);
		}

		public static string[] PtrToStringArray(int count, IntPtr stringArray)
		{
			return UnixMarshal.PtrToStringArray(count, stringArray, UnixEncoding.Instance);
		}

		public static string[] PtrToStringArray(int count, IntPtr stringArray, Encoding encoding)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "< 0");
			}
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			if (stringArray == IntPtr.Zero)
			{
				return new string[count];
			}
			string[] str = new string[count];
			for (int i = 0; i < count; i++)
			{
				IntPtr intPtr = Marshal.ReadIntPtr(stringArray, i * IntPtr.Size);
				str[i] = UnixMarshal.PtrToString(intPtr, encoding);
			}
			return str;
		}

		public static string PtrToStringUnix(IntPtr p)
		{
			if (p == IntPtr.Zero)
			{
				return null;
			}
			int num = (void*)(checked((int)Stdlib.strlen(p)));
			return new string((void*)p, 0, num, UnixEncoding.Instance);
		}

		public static IntPtr ReAllocHeap(IntPtr ptr, long size)
		{
			if (size < (long)0)
			{
				throw new ArgumentOutOfRangeException("size", "< 0");
			}
			return Stdlib.realloc(ptr, (ulong)size);
		}

		public static bool ShouldRetrySyscall(int r)
		{
			if (r == -1 && Stdlib.GetLastError() == Errno.EINTR)
			{
				return true;
			}
			return false;
		}

		[CLSCompliant(false)]
		public static bool ShouldRetrySyscall(int r, out Errno errno)
		{
			errno = (Errno)0;
			if (r == -1)
			{
				Errno lastError = Stdlib.GetLastError();
				Errno errno1 = lastError;
				errno = lastError;
				if (errno1 == Errno.EINTR)
				{
					return true;
				}
			}
			return false;
		}

		public static IntPtr StringToHeap(string s)
		{
			return UnixMarshal.StringToHeap(s, UnixEncoding.Instance);
		}

		public static IntPtr StringToHeap(string s, Encoding encoding)
		{
			return UnixMarshal.StringToHeap(s, 0, s.Length, encoding);
		}

		public static IntPtr StringToHeap(string s, int index, int count)
		{
			return UnixMarshal.StringToHeap(s, index, count, UnixEncoding.Instance);
		}

		public static IntPtr StringToHeap(string s, int index, int count, Encoding encoding)
		{
			if (s == null)
			{
				return IntPtr.Zero;
			}
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			int maxByteCount = encoding.GetMaxByteCount(1);
			char[] charArray = s.ToCharArray(index, count);
			byte[] numArray = new byte[encoding.GetByteCount(charArray) + maxByteCount];
			if (encoding.GetBytes(charArray, 0, (int)charArray.Length, numArray, 0) != (int)numArray.Length - maxByteCount)
			{
				throw new NotSupportedException("encoding.GetBytes() doesn't equal encoding.GetByteCount()!");
			}
			IntPtr intPtr = UnixMarshal.AllocHeap((long)((int)numArray.Length));
			if (intPtr == IntPtr.Zero)
			{
				throw new UnixIOException(Errno.ENOMEM);
			}
			bool flag = false;
			try
			{
				Marshal.Copy(numArray, 0, intPtr, (int)numArray.Length);
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					UnixMarshal.FreeHeap(intPtr);
				}
			}
			return intPtr;
		}

		[CLSCompliant(false)]
		public static void ThrowExceptionForError(Errno errno)
		{
			throw UnixMarshal.CreateExceptionForError(errno);
		}

		[CLSCompliant(false)]
		public static void ThrowExceptionForErrorIf(int retval, Errno errno)
		{
			if (retval == -1)
			{
				UnixMarshal.ThrowExceptionForError(errno);
			}
		}

		public static void ThrowExceptionForLastError()
		{
			throw UnixMarshal.CreateExceptionForLastError();
		}

		public static void ThrowExceptionForLastErrorIf(int retval)
		{
			if (retval == -1)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
		}
	}
}