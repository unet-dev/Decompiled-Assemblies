using Mono.Unix.Native;
using System;

namespace Mono.Unix
{
	public struct UnixPipes : IEquatable<UnixPipes>
	{
		public UnixStream Reading;

		public UnixStream Writing;

		public UnixPipes(UnixStream reading, UnixStream writing)
		{
			this.Reading = reading;
			this.Writing = writing;
		}

		public static UnixPipes CreatePipes()
		{
			int num;
			int num1;
			UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.pipe(out num, out num1));
			return new UnixPipes(new UnixStream(num), new UnixStream(num1));
		}

		public override bool Equals(object value)
		{
			if (value == null || value.GetType() != this.GetType())
			{
				return false;
			}
			UnixPipes unixPipe = (UnixPipes)value;
			return (this.Reading.Handle != unixPipe.Reading.Handle ? false : this.Writing.Handle == unixPipe.Writing.Handle);
		}

		public bool Equals(UnixPipes value)
		{
			return (this.Reading.Handle != value.Reading.Handle ? false : this.Writing.Handle == value.Writing.Handle);
		}

		public override int GetHashCode()
		{
			return this.Reading.Handle.GetHashCode() ^ this.Writing.Handle.GetHashCode();
		}

		public static bool operator ==(UnixPipes lhs, UnixPipes rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(UnixPipes lhs, UnixPipes rhs)
		{
			return !lhs.Equals(rhs);
		}
	}
}