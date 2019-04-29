using Newtonsoft.Json.Shims;
using System;
using System.Globalization;

namespace Newtonsoft.Json.Utilities
{
	[Preserve]
	internal static class MiscellaneousUtils
	{
		public static int ByteArrayCompare(byte[] a1, byte[] a2)
		{
			int length = (int)a1.Length;
			int num = length.CompareTo((int)a2.Length);
			if (num != 0)
			{
				return num;
			}
			for (int i = 0; i < (int)a1.Length; i++)
			{
				int num1 = a1[i].CompareTo(a2[i]);
				if (num1 != 0)
				{
					return num1;
				}
			}
			return 0;
		}

		public static ArgumentOutOfRangeException CreateArgumentOutOfRangeException(string paramName, object actualValue, string message)
		{
			string str = string.Concat(message, Environment.NewLine, "Actual value was {0}.".FormatWith(CultureInfo.InvariantCulture, actualValue));
			return new ArgumentOutOfRangeException(paramName, str);
		}

		internal static string FormatValueForPrint(object value)
		{
			if (value == null)
			{
				return "{null}";
			}
			if (!(value is string))
			{
				return value.ToString();
			}
			return string.Concat("\"", value, "\"");
		}

		public static string GetLocalName(string qualifiedName)
		{
			string str;
			string str1;
			MiscellaneousUtils.GetQualifiedNameParts(qualifiedName, out str, out str1);
			return str1;
		}

		public static string GetPrefix(string qualifiedName)
		{
			string str;
			string str1;
			MiscellaneousUtils.GetQualifiedNameParts(qualifiedName, out str, out str1);
			return str;
		}

		public static void GetQualifiedNameParts(string qualifiedName, out string prefix, out string localName)
		{
			int num = qualifiedName.IndexOf(':');
			if (num == -1 || num == 0 || qualifiedName.Length - 1 == num)
			{
				prefix = null;
				localName = qualifiedName;
				return;
			}
			prefix = qualifiedName.Substring(0, num);
			localName = qualifiedName.Substring(num + 1);
		}

		public static string ToString(object value)
		{
			if (value == null)
			{
				return "{null}";
			}
			if (!(value is string))
			{
				return value.ToString();
			}
			return string.Concat("\"", value.ToString(), "\"");
		}

		public static bool ValueEquals(object objA, object objB)
		{
			if (objA == null && objB == null)
			{
				return true;
			}
			if (objA != null && objB == null)
			{
				return false;
			}
			if (objA == null && objB != null)
			{
				return false;
			}
			if (objA.GetType() == objB.GetType())
			{
				return objA.Equals(objB);
			}
			if (ConvertUtils.IsInteger(objA) && ConvertUtils.IsInteger(objB))
			{
				decimal num = Convert.ToDecimal(objA, CultureInfo.CurrentCulture);
				return num.Equals(Convert.ToDecimal(objB, CultureInfo.CurrentCulture));
			}
			if (!(objA is double) && !(objA is float) && !(objA is decimal) || !(objB is double) && !(objB is float) && !(objB is decimal))
			{
				return false;
			}
			return MathUtils.ApproxEquals(Convert.ToDouble(objA, CultureInfo.CurrentCulture), Convert.ToDouble(objB, CultureInfo.CurrentCulture));
		}
	}
}