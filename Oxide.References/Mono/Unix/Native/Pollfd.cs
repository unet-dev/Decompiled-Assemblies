using System;

namespace Mono.Unix.Native
{
	[Map("struct pollfd")]
	public struct Pollfd : IEquatable<Pollfd>
	{
		public int fd;

		[CLSCompliant(false)]
		public PollEvents events;

		[CLSCompliant(false)]
		public PollEvents revents;

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != this.GetType())
			{
				return false;
			}
			Pollfd pollfd = (Pollfd)obj;
			return (pollfd.events != this.events ? false : pollfd.revents == this.revents);
		}

		public bool Equals(Pollfd value)
		{
			return (value.events != this.events ? false : value.revents == this.revents);
		}

		public override int GetHashCode()
		{
			return this.events.GetHashCode() ^ this.revents.GetHashCode();
		}

		public static bool operator ==(Pollfd lhs, Pollfd rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Pollfd lhs, Pollfd rhs)
		{
			return !lhs.Equals(rhs);
		}
	}
}