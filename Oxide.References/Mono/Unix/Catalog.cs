using Mono.Unix.Native;
using System;
using System.Runtime.InteropServices;

namespace Mono.Unix
{
	public class Catalog
	{
		private Catalog()
		{
		}

		[DllImport("intl", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern IntPtr bind_textdomain_codeset(IntPtr domainname, IntPtr codeset);

		[DllImport("intl", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern IntPtr bindtextdomain(IntPtr domainname, IntPtr dirname);

		public static string GetPluralString(string s, string p, int n)
		{
			IntPtr intPtr;
			IntPtr intPtr1;
			IntPtr intPtr2;
			string str;
			Catalog.MarshalStrings(s, out intPtr, p, out intPtr1, null, out intPtr2);
			try
			{
				IntPtr intPtr3 = Catalog.ngettext(intPtr, intPtr1, n);
				if (intPtr3 != intPtr)
				{
					str = (intPtr3 != intPtr1 ? UnixMarshal.PtrToStringUnix(intPtr3) : p);
				}
				else
				{
					str = s;
				}
			}
			finally
			{
				UnixMarshal.FreeHeap(intPtr);
				UnixMarshal.FreeHeap(intPtr1);
			}
			return str;
		}

		public static string GetString(string s)
		{
			string str;
			IntPtr heap = UnixMarshal.StringToHeap(s);
			try
			{
				IntPtr intPtr = Catalog.gettext(heap);
				str = (intPtr == heap ? s : UnixMarshal.PtrToStringUnix(intPtr));
			}
			finally
			{
				UnixMarshal.FreeHeap(heap);
			}
			return str;
		}

		[DllImport("intl", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern IntPtr gettext(IntPtr instring);

		public static void Init(string package, string localedir)
		{
			IntPtr intPtr;
			IntPtr intPtr1;
			IntPtr intPtr2;
			Catalog.MarshalStrings(package, out intPtr, localedir, out intPtr1, "UTF-8", out intPtr2);
			try
			{
				if (Catalog.bindtextdomain(intPtr, intPtr1) == IntPtr.Zero)
				{
					throw new UnixIOException(Errno.ENOMEM);
				}
				if (Catalog.bind_textdomain_codeset(intPtr, intPtr2) == IntPtr.Zero)
				{
					throw new UnixIOException(Errno.ENOMEM);
				}
				if (Catalog.textdomain(intPtr) == IntPtr.Zero)
				{
					throw new UnixIOException(Errno.ENOMEM);
				}
			}
			finally
			{
				UnixMarshal.FreeHeap(intPtr);
				UnixMarshal.FreeHeap(intPtr1);
				UnixMarshal.FreeHeap(intPtr2);
			}
		}

		private static void MarshalStrings(string s1, out IntPtr p1, string s2, out IntPtr p2, string s3, out IntPtr p3)
		{
			IntPtr zero = IntPtr.Zero;
			IntPtr intPtr = zero;
			p3 = zero;
			IntPtr intPtr1 = intPtr;
			intPtr = intPtr1;
			p2 = intPtr1;
			p1 = intPtr;
			bool flag = true;
			try
			{
				p1 = UnixMarshal.StringToHeap(s1);
				p2 = UnixMarshal.StringToHeap(s2);
				if (s3 != null)
				{
					p3 = UnixMarshal.StringToHeap(s3);
				}
				flag = false;
			}
			finally
			{
				if (flag)
				{
					UnixMarshal.FreeHeap((void*)p1);
					UnixMarshal.FreeHeap((void*)p2);
					UnixMarshal.FreeHeap((void*)p3);
				}
			}
		}

		[DllImport("intl", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern IntPtr ngettext(IntPtr singular, IntPtr plural, int n);

		[DllImport("intl", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern IntPtr textdomain(IntPtr domainname);
	}
}