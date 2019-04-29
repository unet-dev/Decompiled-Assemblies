using Newtonsoft.Json.Shims;
using System;

namespace Newtonsoft.Json.Utilities
{
	[Preserve]
	internal struct DateTimeParser
	{
		public int Year;

		public int Month;

		public int Day;

		public int Hour;

		public int Minute;

		public int Second;

		public int Fraction;

		public int ZoneHour;

		public int ZoneMinute;

		public ParserTimeZone Zone;

		private char[] _text;

		private int _end;

		private readonly static int[] Power10;

		private readonly static int Lzyyyy;

		private readonly static int Lzyyyy_;

		private readonly static int Lzyyyy_MM;

		private readonly static int Lzyyyy_MM_;

		private readonly static int Lzyyyy_MM_dd;

		private readonly static int Lzyyyy_MM_ddT;

		private readonly static int LzHH;

		private readonly static int LzHH_;

		private readonly static int LzHH_mm;

		private readonly static int LzHH_mm_;

		private readonly static int LzHH_mm_ss;

		private readonly static int Lz_;

		private readonly static int Lz_zz;

		private const short MaxFractionDigits = 7;

		static DateTimeParser()
		{
			DateTimeParser.Power10 = new int[] { -1, 10, 100, 1000, 10000, 100000, 1000000 };
			DateTimeParser.Lzyyyy = "yyyy".Length;
			DateTimeParser.Lzyyyy_ = "yyyy-".Length;
			DateTimeParser.Lzyyyy_MM = "yyyy-MM".Length;
			DateTimeParser.Lzyyyy_MM_ = "yyyy-MM-".Length;
			DateTimeParser.Lzyyyy_MM_dd = "yyyy-MM-dd".Length;
			DateTimeParser.Lzyyyy_MM_ddT = "yyyy-MM-ddT".Length;
			DateTimeParser.LzHH = "HH".Length;
			DateTimeParser.LzHH_ = "HH:".Length;
			DateTimeParser.LzHH_mm = "HH:mm".Length;
			DateTimeParser.LzHH_mm_ = "HH:mm:".Length;
			DateTimeParser.LzHH_mm_ss = "HH:mm:ss".Length;
			DateTimeParser.Lz_ = "-".Length;
			DateTimeParser.Lz_zz = "-zz".Length;
		}

		public bool Parse(char[] text, int startIndex, int length)
		{
			this._text = text;
			this._end = startIndex + length;
			if (this.ParseDate(startIndex) && this.ParseChar(DateTimeParser.Lzyyyy_MM_dd + startIndex, 'T') && this.ParseTimeAndZoneAndWhitespace(DateTimeParser.Lzyyyy_MM_ddT + startIndex))
			{
				return true;
			}
			return false;
		}

		private bool Parse2Digit(int start, out int num)
		{
			if (start + 1 < this._end)
			{
				int num1 = this._text[start] - 48;
				int num2 = this._text[start + 1] - 48;
				if (0 <= num1 && num1 < 10 && 0 <= num2 && num2 < 10)
				{
					num = num1 * 10 + num2;
					return true;
				}
			}
			num = 0;
			return false;
		}

		private bool Parse4Digit(int start, out int num)
		{
			if (start + 3 < this._end)
			{
				int num1 = this._text[start] - 48;
				int num2 = this._text[start + 1] - 48;
				int num3 = this._text[start + 2] - 48;
				int num4 = this._text[start + 3] - 48;
				if (0 <= num1 && num1 < 10 && 0 <= num2 && num2 < 10 && 0 <= num3 && num3 < 10 && 0 <= num4 && num4 < 10)
				{
					num = ((num1 * 10 + num2) * 10 + num3) * 10 + num4;
					return true;
				}
			}
			num = 0;
			return false;
		}

		private bool ParseChar(int start, char ch)
		{
			if (start >= this._end)
			{
				return false;
			}
			return this._text[start] == ch;
		}

		private bool ParseDate(int start)
		{
			if (!this.Parse4Digit(start, out this.Year) || 1 > this.Year || !this.ParseChar(start + DateTimeParser.Lzyyyy, '-') || !this.Parse2Digit(start + DateTimeParser.Lzyyyy_, out this.Month) || 1 > this.Month || this.Month > 12 || !this.ParseChar(start + DateTimeParser.Lzyyyy_MM, '-') || !this.Parse2Digit(start + DateTimeParser.Lzyyyy_MM_, out this.Day) || 1 > this.Day)
			{
				return false;
			}
			return this.Day <= DateTime.DaysInMonth(this.Year, this.Month);
		}

		private bool ParseTime(ref int start)
		{
			if (!this.Parse2Digit(start, out this.Hour) || this.Hour > 24 || !this.ParseChar(start + DateTimeParser.LzHH, ':') || !this.Parse2Digit(start + DateTimeParser.LzHH_, out this.Minute) || this.Minute >= 60 || !this.ParseChar(start + DateTimeParser.LzHH_mm, ':') || !this.Parse2Digit(start + DateTimeParser.LzHH_mm_, out this.Second) || this.Second >= 60 || this.Hour == 24 && (this.Minute != 0 || this.Second != 0))
			{
				return false;
			}
			start += DateTimeParser.LzHH_mm_ss;
			if (this.ParseChar(start, '.'))
			{
				this.Fraction = 0;
				int num = 0;
				while (true)
				{
					int num1 = start + 1;
					start = num1;
					if (num1 >= this._end || num >= 7)
					{
						break;
					}
					int num2 = this._text[start] - 48;
					if (num2 < 0 || num2 > 9)
					{
						break;
					}
					this.Fraction = this.Fraction * 10 + num2;
					num++;
				}
				if (num < 7)
				{
					if (num == 0)
					{
						return false;
					}
					this.Fraction *= DateTimeParser.Power10[7 - num];
				}
				if (this.Hour == 24 && this.Fraction != 0)
				{
					return false;
				}
			}
			return true;
		}

		private bool ParseTimeAndZoneAndWhitespace(int start)
		{
			if (!this.ParseTime(ref start))
			{
				return false;
			}
			return this.ParseZone(start);
		}

		private bool ParseZone(int start)
		{
			if (start < this._end)
			{
				char chr = this._text[start];
				if (chr == 'Z' || chr == 'z')
				{
					this.Zone = ParserTimeZone.Utc;
					start++;
				}
				else
				{
					if (start + 2 < this._end && this.Parse2Digit(start + DateTimeParser.Lz_, out this.ZoneHour) && this.ZoneHour <= 99)
					{
						if (chr == '+')
						{
							this.Zone = ParserTimeZone.LocalEastOfUtc;
							start += DateTimeParser.Lz_zz;
						}
						else if (chr == '-')
						{
							this.Zone = ParserTimeZone.LocalWestOfUtc;
							start += DateTimeParser.Lz_zz;
						}
					}
					if (start < this._end)
					{
						if (this.ParseChar(start, ':'))
						{
							start++;
							if (start + 1 < this._end && this.Parse2Digit(start, out this.ZoneMinute) && this.ZoneMinute <= 99)
							{
								start += 2;
							}
						}
						else if (start + 1 < this._end && this.Parse2Digit(start, out this.ZoneMinute) && this.ZoneMinute <= 99)
						{
							start += 2;
						}
					}
				}
			}
			return start == this._end;
		}
	}
}