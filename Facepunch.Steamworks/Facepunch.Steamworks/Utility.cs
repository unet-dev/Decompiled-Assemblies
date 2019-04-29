using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Facepunch.Steamworks
{
	public static class Utility
	{
		internal static string FormatPrice(string currency, ulong price)
		{
			return Utility.FormatPrice(currency, (double)((float)price) / 100);
		}

		public static string FormatPrice(string currency, double price)
		{
			string str = price.ToString("0.00");
			if (currency == "AED")
			{
				return string.Concat(str, "د.إ");
			}
			if (currency == "ARS")
			{
				return string.Concat("$", str, " ARS");
			}
			if (currency == "AUD")
			{
				return string.Concat("$", str, " AUD");
			}
			if (currency == "BRL")
			{
				return string.Concat("R$ ", str);
			}
			if (currency == "CAD")
			{
				return string.Concat("$", str, " CAD");
			}
			if (currency == "CHF")
			{
				return string.Concat("Fr. ", str);
			}
			if (currency == "CLP")
			{
				return string.Concat("$", str, " CLP");
			}
			if (currency == "CNY")
			{
				return string.Concat(str, "元");
			}
			if (currency == "COP")
			{
				return string.Concat("COL$ ", str);
			}
			if (currency == "CRC")
			{
				return string.Concat("₡", str);
			}
			if (currency == "EUR")
			{
				return string.Concat("€", str);
			}
			if (currency == "GBP")
			{
				return string.Concat("£", str);
			}
			if (currency == "HKD")
			{
				return string.Concat("HK$ ", str);
			}
			if (currency == "ILS")
			{
				return string.Concat("₪", str);
			}
			if (currency == "IDR")
			{
				return string.Concat("Rp", str);
			}
			if (currency == "INR")
			{
				return string.Concat("₹", str);
			}
			if (currency == "JPY")
			{
				return string.Concat("¥", str);
			}
			if (currency == "KRW")
			{
				return string.Concat("₩", str);
			}
			if (currency == "KWD")
			{
				return string.Concat("KD ", str);
			}
			if (currency == "KZT")
			{
				return string.Concat(str, "₸");
			}
			if (currency == "MXN")
			{
				return string.Concat("Mex$ ", str);
			}
			if (currency == "MYR")
			{
				return string.Concat("RM ", str);
			}
			if (currency == "NOK")
			{
				return string.Concat(str, " kr");
			}
			if (currency == "NZD")
			{
				return string.Concat("$", str, " NZD");
			}
			if (currency == "PEN")
			{
				return string.Concat("S/. ", str);
			}
			if (currency == "PHP")
			{
				return string.Concat("₱", str);
			}
			if (currency == "PLN")
			{
				return string.Concat("zł ", str);
			}
			if (currency == "QAR")
			{
				return string.Concat("QR ", str);
			}
			if (currency == "RUB")
			{
				return string.Concat("₽", str);
			}
			if (currency == "SAR")
			{
				return string.Concat("SR ", str);
			}
			if (currency == "SGD")
			{
				return string.Concat("S$ ", str);
			}
			if (currency == "THB")
			{
				return string.Concat("฿", str);
			}
			if (currency == "TRY")
			{
				return string.Concat("₺", str);
			}
			if (currency == "TWD")
			{
				return string.Concat("NT$ ", str);
			}
			if (currency == "UAH")
			{
				return string.Concat("₴", str);
			}
			if (currency == "USD")
			{
				return string.Concat("$", str);
			}
			if (currency == "UYU")
			{
				return string.Concat("$U ", str);
			}
			if (currency == "VND")
			{
				return string.Concat("₫", str);
			}
			if (currency == "ZAR")
			{
				return string.Concat("R ", str);
			}
			return string.Concat(str, " ", currency);
		}

		public static IPAddress Int32ToIp(uint ipAddress)
		{
			return new IPAddress((long)Utility.Swap(ipAddress));
		}

		public static uint IpToInt32(this IPAddress ipAddress)
		{
			return Utility.Swap((uint)ipAddress.Address);
		}

		public static string ReadNullTerminatedUTF8String(this BinaryReader br, byte[] buffer = null)
		{
			if (buffer == null)
			{
				buffer = new byte[1024];
			}
			int num = 0;
			while (true)
			{
				byte num1 = br.ReadByte();
				byte num2 = num1;
				if (num1 == 0 || num >= (int)buffer.Length)
				{
					break;
				}
				buffer[num] = num2;
				num++;
			}
			return Encoding.UTF8.GetString(buffer, 0, num);
		}

		internal static uint Swap(uint x)
		{
			return ((x & 255) << 24) + ((x & 65280) << 8) + ((x & 16711680) >> 8) + ((x & -16777216) >> 24);
		}

		public static IEnumerable<T> UnionSelect<T>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, T, T> selector)
		where T : IEquatable<T>
		{
			T t;
			Dictionary<T, T> ts = new Dictionary<T, T>();
			foreach (T t1 in first)
			{
				ts[t1] = t1;
			}
			foreach (T t2 in second)
			{
				if (!ts.TryGetValue(t2, out t))
				{
					continue;
				}
				ts.Remove(t2);
				yield return selector(t, t2);
			}
		}

		internal static class Epoch
		{
			private readonly static DateTime epoch;

			public static int Current
			{
				get
				{
					return (int)DateTime.UtcNow.Subtract(Utility.Epoch.epoch).TotalSeconds;
				}
			}

			static Epoch()
			{
				Utility.Epoch.epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			}

			public static uint FromDateTime(DateTime dt)
			{
				return (uint)dt.Subtract(Utility.Epoch.epoch).TotalSeconds;
			}

			public static DateTime ToDateTime(decimal unixTime)
			{
				return Utility.Epoch.epoch.AddSeconds((double)((long)unixTime));
			}
		}
	}
}