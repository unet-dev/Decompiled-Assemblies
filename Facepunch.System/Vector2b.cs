using System;

[Serializable]
public struct Vector2b : IEquatable<Vector2b>
{
	public readonly static Vector2b alltrue;

	public readonly static Vector2b allfalse;

	public bool x;

	public bool y;

	static Vector2b()
	{
		Vector2b.alltrue = new Vector2b(true, true);
		Vector2b.allfalse = new Vector2b(false, false);
	}

	public Vector2b(bool x, bool y)
	{
		this.x = x;
		this.y = y;
	}

	public bool Equals(Vector2b o)
	{
		if (this.x != o.x)
		{
			return false;
		}
		return this.y == o.y;
	}

	public override bool Equals(object o)
	{
		if (o == null || !(o is Vector2b))
		{
			return false;
		}
		return this.Equals((Vector2b)o);
	}

	public override int GetHashCode()
	{
		return this.x.GetHashCode() ^ this.y.GetHashCode();
	}

	public static bool operator ==(Vector2b a, Vector2b b)
	{
		return a.Equals(b);
	}

	public static bool operator !=(Vector2b a, Vector2b b)
	{
		return !a.Equals(b);
	}

	public override string ToString()
	{
		return string.Format("[{0},{1}]", this.x, this.y);
	}
}