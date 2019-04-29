using System;
using UnityEngine;

[Serializable]
public struct Vector3i : IEquatable<Vector3i>
{
	public readonly static Vector3i zero;

	public readonly static Vector3i one;

	public readonly static Vector3i forward;

	public readonly static Vector3i back;

	public readonly static Vector3i up;

	public readonly static Vector3i down;

	public readonly static Vector3i right;

	public readonly static Vector3i left;

	public int x;

	public int y;

	public int z;

	static Vector3i()
	{
		Vector3i.zero = new Vector3i(0, 0, 0);
		Vector3i.one = new Vector3i(1, 1, 1);
		Vector3i.forward = new Vector3i(0, 0, 1);
		Vector3i.back = new Vector3i(0, 0, -1);
		Vector3i.up = new Vector3i(0, 1, 0);
		Vector3i.down = new Vector3i(0, -1, 0);
		Vector3i.right = new Vector3i(1, 0, 0);
		Vector3i.left = new Vector3i(-1, 0, 0);
	}

	public Vector3i(int x, int y, int z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public bool Equals(Vector3i o)
	{
		if (this.x != o.x || this.y != o.y)
		{
			return false;
		}
		return this.z == o.z;
	}

	public override bool Equals(object o)
	{
		if (o == null || !(o is Vector3i))
		{
			return false;
		}
		return this.Equals((Vector3i)o);
	}

	public override int GetHashCode()
	{
		return this.x.GetHashCode() ^ this.y.GetHashCode() ^ this.z.GetHashCode();
	}

	public static Vector3i operator +(Vector3i a, Vector3i b)
	{
		return new Vector3i(a.x + b.x, a.y + b.y, a.z + b.z);
	}

	public static Vector3i operator /(Vector3i v, int divisor)
	{
		return new Vector3i(v.x / divisor, v.y / divisor, v.z / divisor);
	}

	public static Vector3 operator /(Vector3i v, float divisor)
	{
		return new Vector3((float)v.x / divisor, (float)v.y / divisor, (float)v.z / divisor);
	}

	public static bool operator ==(Vector3i a, Vector3i b)
	{
		return a.Equals(b);
	}

	public static explicit operator Vector3i(Vector3 other)
	{
		return new Vector3i((int)other.x, (int)other.y, (int)other.z);
	}

	public static implicit operator Vector3(Vector3i other)
	{
		return new Vector3((float)other.x, (float)other.y, (float)other.z);
	}

	public static bool operator !=(Vector3i a, Vector3i b)
	{
		return !a.Equals(b);
	}

	public static Vector3i operator <<(Vector3i v, int shift)
	{
		return new Vector3i(v.x << (shift & 31), v.y << (shift & 31), v.z << (shift & 31));
	}

	public static Vector3i operator %(Vector3i v, int mod)
	{
		int num = v.x % mod;
		int num1 = v.y % mod;
		return new Vector3i(num, num1, v.z % mod);
	}

	public static Vector3i operator *(Vector3i v, int multiplier)
	{
		return new Vector3i(v.x * multiplier, v.y * multiplier, v.z * multiplier);
	}

	public static Vector3 operator *(Vector3i v, float multiplier)
	{
		return new Vector3((float)v.x * multiplier, (float)v.y * multiplier, (float)v.z * multiplier);
	}

	public static Vector3i operator >>(Vector3i v, int shift)
	{
		return new Vector3i(v.x >> (shift & 31), v.y >> (shift & 31), v.z >> (shift & 31));
	}

	public static Vector3i operator -(Vector3i a, Vector3i b)
	{
		return new Vector3i(a.x - b.x, a.y - b.y, a.z - b.z);
	}

	public static Vector3i operator -(Vector3i v)
	{
		return new Vector3i(-v.x, -v.y, -v.z);
	}

	public static Vector3i operator +(Vector3i v)
	{
		return new Vector3i(v.x, v.y, v.z);
	}

	public override string ToString()
	{
		return string.Format("[{0},{1},{2}]", this.x, this.y, this.z);
	}
}