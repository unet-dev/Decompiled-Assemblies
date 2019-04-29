using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Facepunch.Extend
{
	public static class Base36Extensions
	{
		private const string CharList = "0123456789abcdefghijklmnopqrstuvwxyz";

		private static char[] CharArray;

		static Base36Extensions()
		{
			Base36Extensions.CharArray = "0123456789abcdefghijklmnopqrstuvwxyz".ToCharArray();
		}

		public static long FromBase36(this string input)
		{
			IEnumerable<char> chrs = input.ToLower().Reverse<char>();
			long num = (long)0;
			int num1 = 0;
			foreach (char chr in chrs)
			{
				num = num + (long)"0123456789abcdefghijklmnopqrstuvwxyz".IndexOf(chr) * (long)Math.Pow(36, (double)num1);
				num1++;
			}
			return num;
		}

		public static string ToBase36<T>(this T i)
		where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
		{
			long num = (long)Convert.ToDecimal(i);
			if (num < (long)0)
			{
				throw new ArgumentOutOfRangeException("input", (object)num, "input cannot be negative");
			}
			Stack<char> chrs = new Stack<char>();
			while (num != 0)
			{
				chrs.Push(Base36Extensions.CharArray[checked((IntPtr)(num % (long)36))]);
				num /= (long)36;
			}
			return new string(chrs.ToArray());
		}
	}
}