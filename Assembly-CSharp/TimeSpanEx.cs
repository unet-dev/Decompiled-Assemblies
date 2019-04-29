using System;
using System.Runtime.CompilerServices;

public static class TimeSpanEx
{
	public static string ToShortString(this TimeSpan timeSpan)
	{
		return string.Format("{0:00}:{1:00}:{2:00}", (int)timeSpan.TotalHours, timeSpan.Minutes, timeSpan.Seconds);
	}
}