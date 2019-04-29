using Newtonsoft.Json;
using Newtonsoft.Json.Shims;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Newtonsoft.Json.Utilities
{
	[Preserve]
	internal static class DateTimeUtils
	{
		internal readonly static long InitialJavaScriptDateTicks;

		private const string IsoDateFormat = "yyyy-MM-ddTHH:mm:ss.FFFFFFFK";

		private const int DaysPer100Years = 36524;

		private const int DaysPer400Years = 146097;

		private const int DaysPer4Years = 1461;

		private const int DaysPerYear = 365;

		private const long TicksPerDay = 864000000000L;

		private readonly static int[] DaysToMonth365;

		private readonly static int[] DaysToMonth366;

		static DateTimeUtils()
		{
			DateTimeUtils.InitialJavaScriptDateTicks = 621355968000000000L;
			DateTimeUtils.DaysToMonth365 = new int[] { 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365 };
			DateTimeUtils.DaysToMonth366 = new int[] { 0, 31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335, 366 };
		}

		internal static long ConvertDateTimeToJavaScriptTicks(DateTime dateTime, TimeSpan offset)
		{
			return DateTimeUtils.UniversialTicksToJavaScriptTicks(DateTimeUtils.ToUniversalTicks(dateTime, offset));
		}

		internal static long ConvertDateTimeToJavaScriptTicks(DateTime dateTime)
		{
			return DateTimeUtils.ConvertDateTimeToJavaScriptTicks(dateTime, true);
		}

		internal static long ConvertDateTimeToJavaScriptTicks(DateTime dateTime, bool convertToUtc)
		{
			return DateTimeUtils.UniversialTicksToJavaScriptTicks((convertToUtc ? DateTimeUtils.ToUniversalTicks(dateTime) : dateTime.Ticks));
		}

		internal static DateTime ConvertJavaScriptTicksToDateTime(long javaScriptTicks)
		{
			return new DateTime(javaScriptTicks * (long)10000 + DateTimeUtils.InitialJavaScriptDateTicks, DateTimeKind.Utc);
		}

		private static void CopyIntToCharArray(char[] chars, int start, int value, int digits)
		{
			while (true)
			{
				int num = digits;
				digits = num - 1;
				if (num == 0)
				{
					break;
				}
				chars[start + digits] = (char)(value % 10 + 48);
				value /= 10;
			}
		}

		private static DateTime CreateDateTime(DateTimeParser dateTimeParser)
		{
			bool flag;
			if (dateTimeParser.Hour != 24)
			{
				flag = false;
			}
			else
			{
				flag = true;
				dateTimeParser.Hour = 0;
			}
			DateTime dateTime = new DateTime(dateTimeParser.Year, dateTimeParser.Month, dateTimeParser.Day, dateTimeParser.Hour, dateTimeParser.Minute, dateTimeParser.Second);
			dateTime = dateTime.AddTicks((long)dateTimeParser.Fraction);
			if (flag)
			{
				dateTime = dateTime.AddDays(1);
			}
			return dateTime;
		}

		internal static DateTime EnsureDateTime(DateTime value, DateTimeZoneHandling timeZone)
		{
			switch (timeZone)
			{
				case DateTimeZoneHandling.Local:
				{
					value = DateTimeUtils.SwitchToLocalTime(value);
					return value;
				}
				case DateTimeZoneHandling.Utc:
				{
					value = DateTimeUtils.SwitchToUtcTime(value);
					return value;
				}
				case DateTimeZoneHandling.Unspecified:
				{
					value = new DateTime(value.Ticks, DateTimeKind.Unspecified);
					return value;
				}
				case DateTimeZoneHandling.RoundtripKind:
				{
					return value;
				}
			}
			throw new ArgumentException("Invalid date time handling value.");
		}

		private static void GetDateValues(DateTime td, out int year, out int month, out int day)
		{
			bool flag;
			int ticks = (int)(td.Ticks / 864000000000L);
			int num = ticks / 146097;
			ticks = ticks - num * 146097;
			int num1 = ticks / 36524;
			if (num1 == 4)
			{
				num1 = 3;
			}
			ticks = ticks - num1 * 36524;
			int num2 = ticks / 1461;
			ticks = ticks - num2 * 1461;
			int num3 = ticks / 365;
			if (num3 == 4)
			{
				num3 = 3;
			}
			year = num * 400 + num1 * 100 + num2 * 4 + num3 + 1;
			ticks = ticks - num3 * 365;
			if (num3 != 3)
			{
				flag = false;
			}
			else
			{
				flag = (num2 != 24 ? true : num1 == 3);
			}
			int[] numArray = (flag ? DateTimeUtils.DaysToMonth366 : DateTimeUtils.DaysToMonth365);
			int num4 = ticks >> 6;
			while (ticks >= numArray[num4])
			{
				num4++;
			}
			month = num4;
			day = ticks - numArray[num4 - 1] + 1;
		}

		public static TimeSpan GetUtcOffset(this DateTime d)
		{
			return TimeZone.CurrentTimeZone.GetUtcOffset(d);
		}

		private static DateTime SwitchToLocalTime(DateTime value)
		{
			switch (value.Kind)
			{
				case DateTimeKind.Unspecified:
				{
					return new DateTime(value.Ticks, DateTimeKind.Local);
				}
				case DateTimeKind.Utc:
				{
					return value.ToLocalTime();
				}
				case DateTimeKind.Local:
				{
					return value;
				}
			}
			return value;
		}

		private static DateTime SwitchToUtcTime(DateTime value)
		{
			switch (value.Kind)
			{
				case DateTimeKind.Unspecified:
				{
					return new DateTime(value.Ticks, DateTimeKind.Utc);
				}
				case DateTimeKind.Utc:
				{
					return value;
				}
				case DateTimeKind.Local:
				{
					return value.ToUniversalTime();
				}
			}
			return value;
		}

		public static XmlDateTimeSerializationMode ToSerializationMode(DateTimeKind kind)
		{
			switch (kind)
			{
				case DateTimeKind.Unspecified:
				{
					return XmlDateTimeSerializationMode.Unspecified;
				}
				case DateTimeKind.Utc:
				{
					return XmlDateTimeSerializationMode.Utc;
				}
				case DateTimeKind.Local:
				{
					return XmlDateTimeSerializationMode.Local;
				}
			}
			throw MiscellaneousUtils.CreateArgumentOutOfRangeException("kind", kind, "Unexpected DateTimeKind value.");
		}

		private static long ToUniversalTicks(DateTime dateTime)
		{
			if (dateTime.Kind == DateTimeKind.Utc)
			{
				return dateTime.Ticks;
			}
			return DateTimeUtils.ToUniversalTicks(dateTime, dateTime.GetUtcOffset());
		}

		private static long ToUniversalTicks(DateTime dateTime, TimeSpan offset)
		{
			if (dateTime.Kind == DateTimeKind.Utc || dateTime == DateTime.MaxValue || dateTime == DateTime.MinValue)
			{
				return dateTime.Ticks;
			}
			long ticks = dateTime.Ticks - offset.Ticks;
			if (ticks > 3155378975999999999L)
			{
				return 3155378975999999999L;
			}
			if (ticks >= (long)0)
			{
				return ticks;
			}
			return (long)0;
		}

		internal static bool TryParseDateTime(StringReference s, DateTimeZoneHandling dateTimeZoneHandling, string dateFormatString, CultureInfo culture, out DateTime dt)
		{
			if (s.Length > 0)
			{
				int startIndex = s.StartIndex;
				if (s[startIndex] == '/')
				{
					if (s.Length >= 9 && s.StartsWith("/Date(") && s.EndsWith(")/") && DateTimeUtils.TryParseDateTimeMicrosoft(s, dateTimeZoneHandling, out dt))
					{
						return true;
					}
				}
				else if (s.Length >= 19 && s.Length <= 40 && char.IsDigit(s[startIndex]) && s[startIndex + 10] == 'T' && DateTimeUtils.TryParseDateTimeIso(s, dateTimeZoneHandling, out dt))
				{
					return true;
				}
				if (!string.IsNullOrEmpty(dateFormatString) && DateTimeUtils.TryParseDateTimeExact(s.ToString(), dateTimeZoneHandling, dateFormatString, culture, out dt))
				{
					return true;
				}
			}
			dt = new DateTime();
			return false;
		}

		internal static bool TryParseDateTime(string s, DateTimeZoneHandling dateTimeZoneHandling, string dateFormatString, CultureInfo culture, out DateTime dt)
		{
			if (s.Length > 0)
			{
				if (s[0] == '/')
				{
					if (s.Length >= 9 && s.StartsWith("/Date(", StringComparison.Ordinal) && s.EndsWith(")/", StringComparison.Ordinal) && DateTimeUtils.TryParseDateTimeMicrosoft(new StringReference(s.ToCharArray(), 0, s.Length), dateTimeZoneHandling, out dt))
					{
						return true;
					}
				}
				else if (s.Length >= 19 && s.Length <= 40 && char.IsDigit(s[0]) && s[10] == 'T' && DateTime.TryParseExact(s, "yyyy-MM-ddTHH:mm:ss.FFFFFFFK", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dt))
				{
					dt = DateTimeUtils.EnsureDateTime(dt, dateTimeZoneHandling);
					return true;
				}
				if (!string.IsNullOrEmpty(dateFormatString) && DateTimeUtils.TryParseDateTimeExact(s, dateTimeZoneHandling, dateFormatString, culture, out dt))
				{
					return true;
				}
			}
			dt = new DateTime();
			return false;
		}

		private static bool TryParseDateTimeExact(string text, DateTimeZoneHandling dateTimeZoneHandling, string dateFormatString, CultureInfo culture, out DateTime dt)
		{
			DateTime dateTime;
			if (!DateTime.TryParseExact(text, dateFormatString, culture, DateTimeStyles.RoundtripKind, out dateTime))
			{
				dt = new DateTime();
				return false;
			}
			dateTime = DateTimeUtils.EnsureDateTime(dateTime, dateTimeZoneHandling);
			dt = dateTime;
			return true;
		}

		internal static bool TryParseDateTimeIso(StringReference text, DateTimeZoneHandling dateTimeZoneHandling, out DateTime dt)
		{
			long ticks;
			TimeSpan utcOffset;
			DateTimeParser dateTimeParser = new DateTimeParser();
			if (!dateTimeParser.Parse(text.Chars, text.StartIndex, text.Length))
			{
				dt = new DateTime();
				return false;
			}
			DateTime dateTime = DateTimeUtils.CreateDateTime(dateTimeParser);
			switch (dateTimeParser.Zone)
			{
				case ParserTimeZone.Utc:
				{
					dateTime = new DateTime(dateTime.Ticks, DateTimeKind.Utc);
					break;
				}
				case ParserTimeZone.LocalWestOfUtc:
				{
					TimeSpan timeSpan = new TimeSpan(dateTimeParser.ZoneHour, dateTimeParser.ZoneMinute, 0);
					ticks = dateTime.Ticks + timeSpan.Ticks;
					if (ticks > DateTime.MaxValue.Ticks)
					{
						utcOffset = dateTime.GetUtcOffset();
						ticks += utcOffset.Ticks;
						if (ticks > DateTime.MaxValue.Ticks)
						{
							ticks = DateTime.MaxValue.Ticks;
						}
						dateTime = new DateTime(ticks, DateTimeKind.Local);
						break;
					}
					else
					{
						dateTime = (new DateTime(ticks, DateTimeKind.Utc)).ToLocalTime();
						break;
					}
				}
				case ParserTimeZone.LocalEastOfUtc:
				{
					TimeSpan timeSpan1 = new TimeSpan(dateTimeParser.ZoneHour, dateTimeParser.ZoneMinute, 0);
					ticks = dateTime.Ticks - timeSpan1.Ticks;
					if (ticks < DateTime.MinValue.Ticks)
					{
						utcOffset = dateTime.GetUtcOffset();
						ticks += utcOffset.Ticks;
						if (ticks < DateTime.MinValue.Ticks)
						{
							ticks = DateTime.MinValue.Ticks;
						}
						dateTime = new DateTime(ticks, DateTimeKind.Local);
						break;
					}
					else
					{
						dateTime = (new DateTime(ticks, DateTimeKind.Utc)).ToLocalTime();
						break;
					}
				}
			}
			dt = DateTimeUtils.EnsureDateTime(dateTime, dateTimeZoneHandling);
			return true;
		}

		private static bool TryParseDateTimeMicrosoft(StringReference text, DateTimeZoneHandling dateTimeZoneHandling, out DateTime dt)
		{
			long num;
			TimeSpan timeSpan;
			DateTimeKind dateTimeKind;
			if (!DateTimeUtils.TryParseMicrosoftDate(text, out num, out timeSpan, out dateTimeKind))
			{
				dt = new DateTime();
				return false;
			}
			DateTime dateTime = DateTimeUtils.ConvertJavaScriptTicksToDateTime(num);
			if (dateTimeKind == DateTimeKind.Unspecified)
			{
				dt = DateTime.SpecifyKind(dateTime.ToLocalTime(), DateTimeKind.Unspecified);
			}
			else if (dateTimeKind == DateTimeKind.Local)
			{
				dt = dateTime.ToLocalTime();
			}
			else
			{
				dt = dateTime;
			}
			dt = DateTimeUtils.EnsureDateTime(dt, dateTimeZoneHandling);
			return true;
		}

		internal static bool TryParseDateTimeOffset(StringReference s, string dateFormatString, CultureInfo culture, out DateTimeOffset dt)
		{
			if (s.Length > 0)
			{
				int startIndex = s.StartIndex;
				if (s[startIndex] == '/')
				{
					if (s.Length >= 9 && s.StartsWith("/Date(") && s.EndsWith(")/") && DateTimeUtils.TryParseDateTimeOffsetMicrosoft(s, out dt))
					{
						return true;
					}
				}
				else if (s.Length >= 19 && s.Length <= 40 && char.IsDigit(s[startIndex]) && s[startIndex + 10] == 'T' && DateTimeUtils.TryParseDateTimeOffsetIso(s, out dt))
				{
					return true;
				}
				if (!string.IsNullOrEmpty(dateFormatString) && DateTimeUtils.TryParseDateTimeOffsetExact(s.ToString(), dateFormatString, culture, out dt))
				{
					return true;
				}
			}
			dt = new DateTimeOffset();
			return false;
		}

		internal static bool TryParseDateTimeOffset(string s, string dateFormatString, CultureInfo culture, out DateTimeOffset dt)
		{
			if (s.Length > 0)
			{
				if (s[0] == '/')
				{
					if (s.Length >= 9 && s.StartsWith("/Date(", StringComparison.Ordinal) && s.EndsWith(")/", StringComparison.Ordinal) && DateTimeUtils.TryParseDateTimeOffsetMicrosoft(new StringReference(s.ToCharArray(), 0, s.Length), out dt))
					{
						return true;
					}
				}
				else if (s.Length >= 19 && s.Length <= 40 && char.IsDigit(s[0]) && s[10] == 'T' && DateTimeOffset.TryParseExact(s, "yyyy-MM-ddTHH:mm:ss.FFFFFFFK", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dt) && DateTimeUtils.TryParseDateTimeOffsetIso(new StringReference(s.ToCharArray(), 0, s.Length), out dt))
				{
					return true;
				}
				if (!string.IsNullOrEmpty(dateFormatString) && DateTimeUtils.TryParseDateTimeOffsetExact(s, dateFormatString, culture, out dt))
				{
					return true;
				}
			}
			dt = new DateTimeOffset();
			return false;
		}

		private static bool TryParseDateTimeOffsetExact(string text, string dateFormatString, CultureInfo culture, out DateTimeOffset dt)
		{
			DateTimeOffset dateTimeOffset;
			if (!DateTimeOffset.TryParseExact(text, dateFormatString, culture, DateTimeStyles.RoundtripKind, out dateTimeOffset))
			{
				dt = new DateTimeOffset();
				return false;
			}
			dt = dateTimeOffset;
			return true;
		}

		internal static bool TryParseDateTimeOffsetIso(StringReference text, out DateTimeOffset dt)
		{
			TimeSpan timeSpan;
			DateTimeParser dateTimeParser = new DateTimeParser();
			if (!dateTimeParser.Parse(text.Chars, text.StartIndex, text.Length))
			{
				dt = new DateTimeOffset();
				return false;
			}
			DateTime dateTime = DateTimeUtils.CreateDateTime(dateTimeParser);
			switch (dateTimeParser.Zone)
			{
				case ParserTimeZone.Utc:
				{
					timeSpan = new TimeSpan((long)0);
					break;
				}
				case ParserTimeZone.LocalWestOfUtc:
				{
					timeSpan = new TimeSpan(-dateTimeParser.ZoneHour, -dateTimeParser.ZoneMinute, 0);
					break;
				}
				case ParserTimeZone.LocalEastOfUtc:
				{
					timeSpan = new TimeSpan(dateTimeParser.ZoneHour, dateTimeParser.ZoneMinute, 0);
					break;
				}
				default:
				{
					timeSpan = TimeZoneInfo.Local.GetUtcOffset(dateTime);
					break;
				}
			}
			long ticks = dateTime.Ticks - timeSpan.Ticks;
			if (ticks < (long)0 || ticks > 3155378975999999999L)
			{
				dt = new DateTimeOffset();
				return false;
			}
			dt = new DateTimeOffset(dateTime, timeSpan);
			return true;
		}

		private static bool TryParseDateTimeOffsetMicrosoft(StringReference text, out DateTimeOffset dt)
		{
			long num;
			TimeSpan timeSpan;
			DateTimeKind dateTimeKind;
			DateTime dateTime;
			if (!DateTimeUtils.TryParseMicrosoftDate(text, out num, out timeSpan, out dateTimeKind))
			{
				dateTime = new DateTime();
				dt = dateTime;
				return false;
			}
			dateTime = DateTimeUtils.ConvertJavaScriptTicksToDateTime(num).Add(timeSpan);
			dt = new DateTimeOffset(dateTime.Ticks, timeSpan);
			return true;
		}

		private static bool TryParseMicrosoftDate(StringReference text, out long ticks, out TimeSpan offset, out DateTimeKind kind)
		{
			kind = DateTimeKind.Utc;
			int length = text.IndexOf('+', 7, text.Length - 8);
			if (length == -1)
			{
				length = text.IndexOf('-', 7, text.Length - 8);
			}
			if (length == -1)
			{
				offset = TimeSpan.Zero;
				length = text.Length - 2;
			}
			else
			{
				kind = DateTimeKind.Local;
				if (!DateTimeUtils.TryReadOffset(text, length + text.StartIndex, out offset))
				{
					ticks = (long)0;
					return false;
				}
			}
			return ConvertUtils.Int64TryParse(text.Chars, 6 + text.StartIndex, length - 6, out ticks) == ParseResult.Success;
		}

		private static bool TryReadOffset(StringReference offsetText, int startIndex, out TimeSpan offset)
		{
			int num;
			bool item = offsetText[startIndex] == '-';
			if (ConvertUtils.Int32TryParse(offsetText.Chars, startIndex + 1, 2, out num) != ParseResult.Success)
			{
				offset = new TimeSpan();
				return false;
			}
			int num1 = 0;
			if (offsetText.Length - startIndex > 5 && ConvertUtils.Int32TryParse(offsetText.Chars, startIndex + 3, 2, out num1) != ParseResult.Success)
			{
				offset = new TimeSpan();
				return false;
			}
			offset = TimeSpan.FromHours((double)num) + TimeSpan.FromMinutes((double)num1);
			if (item)
			{
				offset = offset.Negate();
			}
			return true;
		}

		private static long UniversialTicksToJavaScriptTicks(long universialTicks)
		{
			return (universialTicks - DateTimeUtils.InitialJavaScriptDateTicks) / (long)10000;
		}

		internal static int WriteDateTimeOffset(char[] chars, int start, TimeSpan offset, DateFormatHandling format)
		{
			char[] chrArray = chars;
			int num = start;
			start = num + 1;
			chrArray[num] = (offset.Ticks >= (long)0 ? '+' : '-');
			int num1 = Math.Abs(offset.Hours);
			DateTimeUtils.CopyIntToCharArray(chars, start, num1, 2);
			start += 2;
			if (format == DateFormatHandling.IsoDateFormat)
			{
				int num2 = start;
				start = num2 + 1;
				chars[num2] = ':';
			}
			int num3 = Math.Abs(offset.Minutes);
			DateTimeUtils.CopyIntToCharArray(chars, start, num3, 2);
			start += 2;
			return start;
		}

		internal static void WriteDateTimeOffsetString(TextWriter writer, DateTimeOffset value, DateFormatHandling format, string formatString, CultureInfo culture)
		{
			if (!string.IsNullOrEmpty(formatString))
			{
				writer.Write(value.ToString(formatString, culture));
				return;
			}
			char[] chrArray = new char[64];
			int num = DateTimeUtils.WriteDateTimeString(chrArray, 0, (format == DateFormatHandling.IsoDateFormat ? value.DateTime : value.UtcDateTime), new TimeSpan?(value.Offset), DateTimeKind.Local, format);
			writer.Write(chrArray, 0, num);
		}

		internal static void WriteDateTimeString(TextWriter writer, DateTime value, DateFormatHandling format, string formatString, CultureInfo culture)
		{
			if (!string.IsNullOrEmpty(formatString))
			{
				writer.Write(value.ToString(formatString, culture));
				return;
			}
			char[] chrArray = new char[64];
			TimeSpan? nullable = null;
			int num = DateTimeUtils.WriteDateTimeString(chrArray, 0, value, nullable, value.Kind, format);
			writer.Write(chrArray, 0, num);
		}

		internal static int WriteDateTimeString(char[] chars, int start, DateTime value, TimeSpan? offset, DateTimeKind kind, DateFormatHandling format)
		{
			TimeSpan? nullable;
			int length = start;
			if (format != DateFormatHandling.MicrosoftDateFormat)
			{
				length = DateTimeUtils.WriteDefaultIsoDate(chars, length, value);
				if (kind == DateTimeKind.Utc)
				{
					int num = length;
					length = num + 1;
					chars[num] = 'Z';
				}
				else if (kind == DateTimeKind.Local)
				{
					char[] chrArray = chars;
					int num1 = length;
					nullable = offset;
					length = DateTimeUtils.WriteDateTimeOffset(chrArray, num1, (nullable.HasValue ? nullable.GetValueOrDefault() : value.GetUtcOffset()), format);
				}
			}
			else
			{
				nullable = offset;
				TimeSpan timeSpan = (nullable.HasValue ? nullable.GetValueOrDefault() : value.GetUtcOffset());
				long javaScriptTicks = DateTimeUtils.ConvertDateTimeToJavaScriptTicks(value, timeSpan);
				"\\/Date(".CopyTo(0, chars, length, 7);
				length += 7;
				string str = javaScriptTicks.ToString(CultureInfo.InvariantCulture);
				str.CopyTo(0, chars, length, str.Length);
				length += str.Length;
				if (kind != DateTimeKind.Unspecified)
				{
					if (kind == DateTimeKind.Local)
					{
						length = DateTimeUtils.WriteDateTimeOffset(chars, length, timeSpan, format);
					}
				}
				else if (value != DateTime.MaxValue && value != DateTime.MinValue)
				{
					length = DateTimeUtils.WriteDateTimeOffset(chars, length, timeSpan, format);
				}
				")\\/".CopyTo(0, chars, length, 3);
				length += 3;
			}
			return length;
		}

		internal static int WriteDefaultIsoDate(char[] chars, int start, DateTime dt)
		{
			int num;
			int num1;
			int num2;
			int num3 = 19;
			DateTimeUtils.GetDateValues(dt, out num, out num1, out num2);
			DateTimeUtils.CopyIntToCharArray(chars, start, num, 4);
			chars[start + 4] = '-';
			DateTimeUtils.CopyIntToCharArray(chars, start + 5, num1, 2);
			chars[start + 7] = '-';
			DateTimeUtils.CopyIntToCharArray(chars, start + 8, num2, 2);
			chars[start + 10] = 'T';
			DateTimeUtils.CopyIntToCharArray(chars, start + 11, dt.Hour, 2);
			chars[start + 13] = ':';
			DateTimeUtils.CopyIntToCharArray(chars, start + 14, dt.Minute, 2);
			chars[start + 16] = ':';
			DateTimeUtils.CopyIntToCharArray(chars, start + 17, dt.Second, 2);
			int ticks = (int)(dt.Ticks % (long)10000000);
			if (ticks != 0)
			{
				int num4 = 7;
				while (ticks % 10 == 0)
				{
					num4--;
					ticks /= 10;
				}
				chars[start + 19] = '.';
				DateTimeUtils.CopyIntToCharArray(chars, start + 20, ticks, num4);
				num3 = num3 + num4 + 1;
			}
			return start + num3;
		}
	}
}