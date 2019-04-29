using Mono.Unix;
using System;

namespace Mono.Unix.Native
{
	public struct RealTimeSignum : IEquatable<RealTimeSignum>
	{
		private int rt_offset;

		private readonly static int MaxOffset;

		public readonly static RealTimeSignum MinValue;

		public readonly static RealTimeSignum MaxValue;

		public int Offset
		{
			get
			{
				return this.rt_offset;
			}
		}

		static RealTimeSignum()
		{
			RealTimeSignum.MaxOffset = UnixSignal.GetSIGRTMAX() - UnixSignal.GetSIGRTMIN() - 1;
			RealTimeSignum.MinValue = new RealTimeSignum(0);
			RealTimeSignum.MaxValue = new RealTimeSignum(RealTimeSignum.MaxOffset);
		}

		public RealTimeSignum(int offset)
		{
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("Offset cannot be negative");
			}
			if (offset > RealTimeSignum.MaxOffset)
			{
				throw new ArgumentOutOfRangeException("Offset greater than maximum supported SIGRT");
			}
			this.rt_offset = offset;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != this.GetType())
			{
				return false;
			}
			return this.Equals((RealTimeSignum)obj);
		}

		public bool Equals(RealTimeSignum value)
		{
			return this.Offset == value.Offset;
		}

		public override int GetHashCode()
		{
			return this.rt_offset.GetHashCode();
		}

		public static bool operator ==(RealTimeSignum lhs, RealTimeSignum rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(RealTimeSignum lhs, RealTimeSignum rhs)
		{
			return !lhs.Equals(rhs);
		}
	}
}