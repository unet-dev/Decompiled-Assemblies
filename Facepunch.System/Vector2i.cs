using System;
using UnityEngine;

[Serializable]
public struct Vector2i : IEquatable<Vector2i>
{
	public readonly static Vector2i zero;

	public readonly static Vector2i one;

	public readonly static Vector2i left;

	public readonly static Vector2i right;

	public readonly static Vector2i forward;

	public readonly static Vector2i back;

	public int x;

	public int y;

	static Vector2i()
	{
		Vector2i.zero = new Vector2i(0, 0);
		Vector2i.one = new Vector2i(1, 1);
		Vector2i.left = new Vector2i(-1, 0);
		Vector2i.right = new Vector2i(1, 0);
		Vector2i.forward = new Vector2i(0, 1);
		Vector2i.back = new Vector2i(0, -1);
	}

	public Vector2i(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public bool Equals(Vector2i o)
	{
		if (this.x != o.x)
		{
			return false;
		}
		return this.y == o.y;
	}

	public override bool Equals(object o)
	{
		if (o == null || !(o is Vector2i))
		{
			return false;
		}
		return this.Equals((Vector2i)o);
	}

	public override int GetHashCode()
	{
		return this.x.GetHashCode() ^ this.y.GetHashCode();
	}

	public static Vector2i operator +(Vector2i a, Vector2i b)
	{
		return new Vector2i(a.x + b.x, a.y + b.y);
	}

	public static Vector2i operator /(Vector2i v, int divisor)
	{
		return new Vector2i(Mathf.RoundToInt((float)(v.x / divisor)), Mathf.RoundToInt((float)(v.y / divisor)));
	}

	public static Vector2 operator /(Vector2i v, float divisor)
	{
		return new Vector2((float)v.x / divisor, (float)v.y / divisor);
	}

	public static bool operator ==(Vector2i a, Vector2i b)
	{
		return a.Equals(b);
	}

	public static explicit operator Vector2i(Vector2 other)
	{
		return new Vector2i((int)other.x, (int)other.y);
	}

	public static implicit operator Vector2(Vector2i other)
	{
		return new Vector2((float)other.x, (float)other.y);
	}

	public static bool operator !=(Vector2i a, Vector2i b)
	{
		return !a.Equals(b);
	}

	public static Vector2i operator <<(Vector2i v, int shift)
	{
		return new Vector2i(v.x << (shift & 31), v.y << (shift & 31));
	}

	public static Vector2i operator %(Vector2i v, int mod)
	{
		return new Vector2i(v.x % mod, v.y % mod);
	}

	public static Vector2i operator *(Vector2i v, int multiplier)
	{
		return new Vector2i(Mathf.RoundToInt((float)(v.x * multiplier)), Mathf.RoundToInt((float)(v.y * multiplier)));
	}

	public static Vector2 operator *(Vector2i v, float multiplier)
	{
		return new Vector2((float)v.x * multiplier, (float)v.y * multiplier);
	}

	public static Vector2i operator >>(Vector2i v, int shift)
	{
		return new Vector2i(v.x >> (shift & 31), v.y >> (shift & 31));
	}

	public static Vector2i operator -(Vector2i a, Vector2i b)
	{
		return new Vector2i(a.x - b.x, a.y - b.y);
	}

	public static Vector2i operator -(Vector2i v)
	{
		return new Vector2i(-v.x, -v.y);
	}

	public static Vector2i operator +(Vector2i v)
	{
		return new Vector2i(v.x, v.y);
	}

	public override string ToString()
	{
		return string.Format("[{0},{1}]", this.x, this.y);
	}
}