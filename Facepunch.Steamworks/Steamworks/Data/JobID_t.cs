using System;

namespace Steamworks.Data
{
	internal struct JobID_t : IEquatable<JobID_t>, IComparable<JobID_t>
	{
		public ulong Value;

		public int CompareTo(JobID_t other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((JobID_t)p);
		}

		public bool Equals(JobID_t p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(JobID_t a, JobID_t b)
		{
			return a.Equals(b);
		}

		public static implicit operator JobID_t(ulong value)
		{
			return new JobID_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(JobID_t value)
		{
			return value.Value;
		}

		public static bool operator !=(JobID_t a, JobID_t b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}