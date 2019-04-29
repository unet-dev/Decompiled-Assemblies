using System;

namespace Oxide.Core.Libraries
{
	public class Time : Library
	{
		private readonly static DateTime Epoch;

		public override bool IsGlobal
		{
			get
			{
				return false;
			}
		}

		static Time()
		{
			Time.Epoch = new DateTime(1970, 1, 1);
		}

		public Time()
		{
		}

		[LibraryFunction("GetCurrentTime")]
		public DateTime GetCurrentTime()
		{
			return DateTime.UtcNow;
		}

		[LibraryFunction("GetDateTimeFromUnix")]
		public DateTime GetDateTimeFromUnix(uint timestamp)
		{
			return Time.Epoch.AddSeconds((double)((float)timestamp));
		}

		[LibraryFunction("GetUnixFromDateTime")]
		public uint GetUnixFromDateTime(DateTime time)
		{
			return (uint)time.Subtract(Time.Epoch).TotalSeconds;
		}

		[LibraryFunction("GetUnixTimestamp")]
		public uint GetUnixTimestamp()
		{
			return (uint)DateTime.UtcNow.Subtract(Time.Epoch).TotalSeconds;
		}
	}
}