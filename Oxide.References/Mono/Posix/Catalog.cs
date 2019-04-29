using System;
using System.Runtime.InteropServices;

namespace Mono.Posix
{
	[Obsolete("Use Mono.Unix.Catalog")]
	public class Catalog
	{
		public Catalog()
		{
		}

		[DllImport("intl", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern IntPtr bind_textdomain_codeset(IntPtr domainname, IntPtr codeset);

		[DllImport("intl", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern IntPtr bindtextdomain(IntPtr domainname, IntPtr dirname);

		public static string GetPluralString(string s, string p, int n)
		{
			IntPtr hGlobalAuto = Marshal.StringToHGlobalAuto(s);
			IntPtr intPtr = Marshal.StringToHGlobalAuto(p);
			string stringAnsi = Marshal.PtrToStringAnsi(Catalog.ngettext(hGlobalAuto, intPtr, n));
			Marshal.FreeHGlobal(hGlobalAuto);
			Marshal.FreeHGlobal(intPtr);
			return stringAnsi;
		}

		public static string GetString(string s)
		{
			IntPtr hGlobalAuto = Marshal.StringToHGlobalAuto(s);
			string stringAuto = Marshal.PtrToStringAuto(Catalog.gettext(hGlobalAuto));
			Marshal.FreeHGlobal(hGlobalAuto);
			return stringAuto;
		}

		[DllImport("intl", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern IntPtr gettext(IntPtr instring);

		public static void Init(string package, string localedir)
		{
			IntPtr hGlobalAuto = Marshal.StringToHGlobalAuto(package);
			IntPtr intPtr = Marshal.StringToHGlobalAuto(localedir);
			IntPtr hGlobalAuto1 = Marshal.StringToHGlobalAuto("UTF-8");
			Catalog.bindtextdomain(hGlobalAuto, intPtr);
			Catalog.bind_textdomain_codeset(hGlobalAuto, hGlobalAuto1);
			Catalog.textdomain(hGlobalAuto);
			Marshal.FreeHGlobal(hGlobalAuto);
			Marshal.FreeHGlobal(intPtr);
			Marshal.FreeHGlobal(hGlobalAuto1);
		}

		[DllImport("intl", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern IntPtr ngettext(IntPtr singular, IntPtr plural, int n);

		[DllImport("intl", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern IntPtr textdomain(IntPtr domainname);
	}
}