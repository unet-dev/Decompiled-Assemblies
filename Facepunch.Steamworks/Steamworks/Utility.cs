using System;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;

namespace Steamworks
{
	public static class Utility
	{
		public static string FormatPrice(string currency, double price)
		{
			string str;
			string str1 = price.ToString("0.00");
			string str2 = currency;
			switch (str2)
			{
				case null:
				{
					str = String.Concat(str1, " ", currency);
					return str;
				}
				case "AED":
				{
					str = String.Concat(str1, "د.إ");
					break;
				}
				case "ARS":
				{
					str = String.Concat("$", str1, " ARS");
					break;
				}
				case "AUD":
				{
					str = String.Concat("A$", str1);
					break;
				}
				case "BRL":
				{
					str = String.Concat("R$", str1);
					break;
				}
				case "CAD":
				{
					str = String.Concat("C$", str1);
					break;
				}
				case "CHF":
				{
					str = String.Concat("Fr. ", str1);
					break;
				}
				case "CLP":
				{
					str = String.Concat("$", str1, " CLP");
					break;
				}
				case "CNY":
				{
					str = String.Concat(str1, "元");
					break;
				}
				case "COP":
				{
					str = String.Concat("COL$ ", str1);
					break;
				}
				case "CRC":
				{
					str = String.Concat("₡", str1);
					break;
				}
				case "EUR":
				{
					str = String.Concat("€", str1);
					break;
				}
				case "SEK":
				{
					str = String.Concat(str1, "kr");
					break;
				}
				case "GBP":
				{
					str = String.Concat("£", str1);
					break;
				}
				case "HKD":
				{
					str = String.Concat("HK$", str1);
					break;
				}
				case "ILS":
				{
					str = String.Concat("₪", str1);
					break;
				}
				case "IDR":
				{
					str = String.Concat("Rp", str1);
					break;
				}
				case "INR":
				{
					str = String.Concat("₹", str1);
					break;
				}
				case "JPY":
				{
					str = String.Concat("¥", str1);
					break;
				}
				case "KRW":
				{
					str = String.Concat("₩", str1);
					break;
				}
				case "KWD":
				{
					str = String.Concat("KD ", str1);
					break;
				}
				case "KZT":
				{
					str = String.Concat(str1, "₸");
					break;
				}
				case "MXN":
				{
					str = String.Concat("Mex$", str1);
					break;
				}
				case "MYR":
				{
					str = String.Concat("RM ", str1);
					break;
				}
				case "NOK":
				{
					str = String.Concat(str1, " kr");
					break;
				}
				case "NZD":
				{
					str = String.Concat("$", str1, " NZD");
					break;
				}
				case "PEN":
				{
					str = String.Concat("S/. ", str1);
					break;
				}
				case "PHP":
				{
					str = String.Concat("₱", str1);
					break;
				}
				case "PLN":
				{
					str = String.Concat(str1, "zł");
					break;
				}
				case "QAR":
				{
					str = String.Concat("QR ", str1);
					break;
				}
				case "RUB":
				{
					str = String.Concat(str1, "₽");
					break;
				}
				case "SAR":
				{
					str = String.Concat("SR ", str1);
					break;
				}
				case "SGD":
				{
					str = String.Concat("S$", str1);
					break;
				}
				case "THB":
				{
					str = String.Concat("฿", str1);
					break;
				}
				case "TRY":
				{
					str = String.Concat("₺", str1);
					break;
				}
				case "TWD":
				{
					str = String.Concat("NT$ ", str1);
					break;
				}
				case "UAH":
				{
					str = String.Concat("₴", str1);
					break;
				}
				case "USD":
				{
					str = String.Concat("$", str1);
					break;
				}
				case "UYU":
				{
					str = String.Concat("$U ", str1);
					break;
				}
				case "VND":
				{
					str = String.Concat("₫", str1);
					break;
				}
				default:
				{
					if (str2 != "ZAR")
					{
						str = String.Concat(str1, " ", currency);
						return str;
					}
					str = String.Concat("R ", str1);
					break;
				}
			}
			return str;
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
				buffer = new Byte[1024];
			}
			int num = 0;
			while (true)
			{
				byte num1 = br.ReadByte();
				byte num2 = num1;
				if ((num1 == 0 ? true : num >= (int)buffer.Length))
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
			uint num = ((x & 255) << 24) + ((x & 65280) << 8) + ((x & 16711680) >> 8) + ((x & -16777216) >> 24);
			return num;
		}
	}
}