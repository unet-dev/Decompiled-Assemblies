using System;

namespace Mono.Unix.Native
{
	[Map("struct timezone")]
	public struct Timezone : IEquatable<Timezone>
	{
		public int tz_minuteswest;

		private int tz_dsttime;

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != this.GetType())
			{
				return false;
			}
			return ((Timezone)obj).tz_minuteswest == this.tz_minuteswest;
		}

		public bool Equals(Timezone value)
		{
			return value.tz_minuteswest == this.tz_minuteswest;
		}

		public override int GetHashCode()
		{
			return this.tz_minuteswest.GetHashCode();
		}

		public static bool operator ==(Timezone lhs, Timezone rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Timezone lhs, Timezone rhs)
		{
			return !lhs.Equals(rhs);
		}
	}
}