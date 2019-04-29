using System;

namespace Oxide.Core.Libraries.Covalence
{
	public class GenericPosition
	{
		public float X;

		public float Y;

		public float Z;

		public GenericPosition()
		{
		}

		public GenericPosition(float x, float y, float z)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is GenericPosition))
			{
				return false;
			}
			GenericPosition genericPosition = (GenericPosition)obj;
			if (!this.X.Equals(genericPosition.X) || !this.Y.Equals(genericPosition.Y))
			{
				return false;
			}
			return this.Z.Equals(genericPosition.Z);
		}

		public override int GetHashCode()
		{
			return this.X.GetHashCode() ^ this.Y.GetHashCode() << 2 ^ this.Z.GetHashCode() >> 2;
		}

		public static GenericPosition operator +(GenericPosition a, GenericPosition b)
		{
			return new GenericPosition(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		}

		public static GenericPosition operator /(GenericPosition a, float div)
		{
			return new GenericPosition(a.X / div, a.Y / div, a.Z / div);
		}

		public static bool operator ==(GenericPosition a, GenericPosition b)
		{
			if ((object)a == (object)b)
			{
				return true;
			}
			if (a == null || b == null)
			{
				return false;
			}
			if (!a.X.Equals(b.X) || !a.Y.Equals(b.Y))
			{
				return false;
			}
			return a.Z.Equals(b.Z);
		}

		public static bool operator !=(GenericPosition a, GenericPosition b)
		{
			return !(a == b);
		}

		public static GenericPosition operator *(float mult, GenericPosition a)
		{
			return new GenericPosition(a.X * mult, a.Y * mult, a.Z * mult);
		}

		public static GenericPosition operator *(GenericPosition a, float mult)
		{
			return new GenericPosition(a.X * mult, a.Y * mult, a.Z * mult);
		}

		public static GenericPosition operator -(GenericPosition a, GenericPosition b)
		{
			return new GenericPosition(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		}

		public override string ToString()
		{
			return string.Format("({0}, {1}, {2})", this.X, this.Y, this.Z);
		}
	}
}