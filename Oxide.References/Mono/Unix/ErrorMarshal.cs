using Mono.Unix.Native;
using System;
using System.Text;

namespace Mono.Unix
{
	internal class ErrorMarshal
	{
		internal readonly static ErrorMarshal.ErrorTranslator Translate;

		static ErrorMarshal()
		{
			try
			{
				ErrorMarshal.Translate = new ErrorMarshal.ErrorTranslator(ErrorMarshal.strerror_r);
				ErrorMarshal.Translate(34);
			}
			catch (EntryPointNotFoundException entryPointNotFoundException)
			{
				ErrorMarshal.Translate = new ErrorMarshal.ErrorTranslator(ErrorMarshal.strerror);
			}
		}

		public ErrorMarshal()
		{
		}

		private static string strerror(Errno errno)
		{
			return Stdlib.strerror(errno);
		}

		private static string strerror_r(Errno errno)
		{
			StringBuilder stringBuilder = new StringBuilder(16);
			int num = 0;
			do
			{
				StringBuilder capacity = stringBuilder;
				capacity.Capacity = capacity.Capacity * 2;
				num = Syscall.strerror_r(errno, stringBuilder);
			}
			while (num == -1 && Stdlib.GetLastError() == Errno.ERANGE);
			if (num != -1)
			{
				return stringBuilder.ToString();
			}
			return string.Concat("** Unknown error code: ", (int)errno, "**");
		}

		internal delegate string ErrorTranslator(Errno errno);
	}
}