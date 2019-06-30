using System;
using System.Runtime.CompilerServices;

namespace Facepunch.Extend
{
	public static class NumberExtensions
	{
		public static T Clamp<T>(this T input, T min, T max)
		where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
		{
			if (input.CompareTo(min) < 0)
			{
				return min;
			}
			if (input.CompareTo(max) > 0)
			{
				return max;
			}
			return input;
		}

		public static string FormatBytes<T>(this T input, bool shortFormat = false)
		where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
		{
			string str;
			ulong num = (ulong)Convert.ChangeType(input, typeof(ulong));
			double num1 = (double)((float)num);
			if (num >= 1152921504606846976L)
			{
				str = "eb";
				num1 = (double)((float)(num >> 50));
			}
			else if (num >= 1125899906842624L)
			{
				str = "pb";
				num1 = (double)((float)(num >> 40));
			}
			else if (num >= 1099511627776L)
			{
				str = "tb";
				num1 = (double)((float)(num >> 30));
			}
			else if (num >= (long)1073741824)
			{
				str = "gb";
				num1 = (double)((float)(num >> 20));
			}
			else if (num < (long)1048576)
			{
				if (num < (long)1024)
				{
					return num.ToString("0b");
				}
				str = "kb";
				num1 = (double)((float)num);
			}
			else
			{
				str = "mb";
				num1 = (double)((float)(num >> 10));
			}
			num1 /= 1024;
			return string.Concat(num1.ToString((shortFormat ? "0" : "0.00")), str);
		}

		public static string FormatNumberShort(this ulong i)
		{
			return ((long)i).FormatNumberShort();
		}

		public static string FormatNumberShort(this long num)
		{
			if (num >= (long)100000)
			{
				return string.Concat((num / (long)1000).FormatNumberShort(), "K");
			}
			if (num < (long)10000)
			{
				return num.ToString("#,0");
			}
			double num1 = (double)num / 1000;
			return string.Concat(num1.ToString("0.#"), "K");
		}

		public static string FormatSeconds(this ulong i)
		{
			return ((long)i).FormatSeconds();
		}

		public static string FormatSeconds(this long s)
		{
			double num = Math.Floor((double)((float)s / 60f));
			double num1 = Math.Floor((double)((float)num / 60f));
			double num2 = Math.Floor((double)((float)num1 / 24f));
			double num3 = Math.Floor((double)((float)num2 / 7f));
			if (s < (long)60)
			{
				return string.Format("{0}s", s);
			}
			if (num < 60)
			{
				return string.Format("{1}m{0}s", new object[] { s % (long)60, num, num1, num2, num3 });
			}
			if (num1 < 48)
			{
				return string.Format("{2}h{1}m{0}s", new object[] { s % (long)60, num % 60, num1, num2, num3 });
			}
			if (num2 < 7)
			{
				return string.Format("{3}d{2}h{1}m{0}s", new object[] { s % (long)60, num % 60, num1 % 24, num2 % 7, num3 });
			}
			return string.Format("{4}w{3}d{2}h{1}m{0}s", new object[] { s % (long)60, num % 60, num1 % 24, num2 % 7, num3 });
		}

		public static string FormatSecondsLong(this ulong i)
		{
			return ((long)i).FormatSecondsLong();
		}

		public static string FormatSecondsLong(this long s)
		{
			double num = Math.Floor((double)((float)s / 60f));
			double num1 = Math.Floor((double)((float)num / 60f));
			double num2 = Math.Floor((double)((float)num1 / 24f));
			double num3 = Math.Floor((double)((float)num2 / 7f));
			if (s < (long)60)
			{
				return string.Format("{0} seconds", s);
			}
			if (num < 60)
			{
				return string.Format("{1} minutes", new object[] { s % (long)60, num, num1, num2, num3 });
			}
			if (num1 < 48)
			{
				return string.Format("{2} hours", new object[] { s % (long)60, num % 60, num1, num2, num3 });
			}
			if (num2 >= 2)
			{
				return string.Format("{3} days", new object[] { s % (long)60, num % 60, num1 % 24, num2, num3 });
			}
			return string.Format("{3} days, {2} hours", new object[] { s % (long)60, num % 60, num1 % 24, num2 % 7, num3 });
		}
	}
}