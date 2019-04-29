using System;

[Serializable]
public struct Vector3b : IEquatable<Vector3b>
{
	public readonly static Vector3b alltrue;

	public readonly static Vector3b allfalse;

	public bool x;

	public bool y;

	public bool z;

	static Vector3b()
	{
		Vector3b.alltrue = new Vector3b(true, true, true);
		Vector3b.allfalse = new Vector3b(false, false, false);
	}

	public Vector3b(bool x, bool y, bool z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public bool Equals(Vector3b o)
	{
		if (this.x != o.x || this.y != o.y)
		{
			return false;
		}
		return this.z == o.z;
	}

	public override bool Equals(object o)
	{
		if (o == null || !(o is Vector3b))
		{
			return false;
		}
		return this.Equals((Vector3b)o);
	}

	public override int GetHashCode()
	{
		return this.x.GetHashCode() ^ this.y.GetHashCode() ^ this.z.GetHashCode();
	}

	public static bool operator ==(Vector3b a, Vector3b b)
	{
		return a.Equals(b);
	}

	public static bool operator !=(Vector3b a, Vector3b b)
	{
		return !a.Equals(b);
	}

	public override string ToString()
	{
		return string.Format("[{0},{1},{2}]", this.x, this.y, this.z);
	}
}