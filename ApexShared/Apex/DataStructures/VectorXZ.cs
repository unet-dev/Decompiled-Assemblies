using System;

namespace Apex.DataStructures
{
	public struct VectorXZ
	{
		public int x;

		public int z;

		public VectorXZ(int x, int z)
		{
			this.x = x;
			this.z = z;
		}

		public override bool Equals(object other)
		{
			if (!(other is VectorXZ))
			{
				return false;
			}
			VectorXZ vectorXZ = (VectorXZ)other;
			if (this.x != vectorXZ.x)
			{
				return false;
			}
			return this.z == vectorXZ.z;
		}

		public override int GetHashCode()
		{
			return this.x.GetHashCode() ^ this.z.GetHashCode();
		}

		public static VectorXZ operator +(VectorXZ a, VectorXZ b)
		{
			return new VectorXZ(a.x + b.x, a.z + b.z);
		}

		public static bool operator ==(VectorXZ lhs, VectorXZ rhs)
		{
			if (lhs.x != rhs.x)
			{
				return false;
			}
			return lhs.z == rhs.z;
		}

		public static bool operator !=(VectorXZ lhs, VectorXZ rhs)
		{
			return !(lhs == rhs);
		}

		public static VectorXZ operator -(VectorXZ a, VectorXZ b)
		{
			return new VectorXZ(a.x - b.x, a.z - b.z);
		}
	}
}