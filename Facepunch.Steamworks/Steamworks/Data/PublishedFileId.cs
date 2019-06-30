using System;

namespace Steamworks.Data
{
	public struct PublishedFileId : IEquatable<PublishedFileId>, IComparable<PublishedFileId>
	{
		public ulong Value;

		public int CompareTo(PublishedFileId other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override bool Equals(object p)
		{
			return this.Equals((PublishedFileId)p);
		}

		public bool Equals(PublishedFileId p)
		{
			return p.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(PublishedFileId a, PublishedFileId b)
		{
			return a.Equals(b);
		}

		public static implicit operator PublishedFileId(ulong value)
		{
			return new PublishedFileId()
			{
				Value = value
			};
		}

		public static implicit operator UInt64(PublishedFileId value)
		{
			return value.Value;
		}

		public static bool operator !=(PublishedFileId a, PublishedFileId b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}