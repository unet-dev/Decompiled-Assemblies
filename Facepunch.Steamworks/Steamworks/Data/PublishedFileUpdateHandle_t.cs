using System;

namespace Steamworks.Data
{
	internal struct PublishedFileUpdateHandle_t : IEquatable<PublishedFileUpdateHandle_t>, IComparable<PublishedFileUpdateHandle_t>
	{
		public ulong Value;

		public int CompareTo(PublishedFileUpdateHandle_t other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((PublishedFileUpdateHandle_t)p);
		}

		public bool Equals(PublishedFileUpdateHandle_t p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(PublishedFileUpdateHandle_t a, PublishedFileUpdateHandle_t b)
		{
			return a.Equals(b);
		}

		public static implicit operator PublishedFileUpdateHandle_t(ulong value)
		{
			return new PublishedFileUpdateHandle_t()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(PublishedFileUpdateHandle_t value)
		{
			return value.Value;
		}

		public static bool operator !=(PublishedFileUpdateHandle_t a, PublishedFileUpdateHandle_t b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}