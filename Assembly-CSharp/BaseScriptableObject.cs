using System;
using UnityEngine;

public class BaseScriptableObject : ScriptableObject
{
	[HideInInspector]
	public uint FilenameStringId;

	public BaseScriptableObject()
	{
	}

	public override bool Equals(object o)
	{
		if (o == null || !(o is BaseScriptableObject))
		{
			return false;
		}
		return o as BaseScriptableObject == this;
	}

	public override int GetHashCode()
	{
		return (int)this.FilenameStringId;
	}

	public string LookupFileName()
	{
		return StringPool.Get(this.FilenameStringId);
	}

	public static bool operator ==(BaseScriptableObject a, BaseScriptableObject b)
	{
		if ((object)a == (object)b)
		{
			return true;
		}
		if (a == null || b == null)
		{
			return false;
		}
		return a.FilenameStringId == b.FilenameStringId;
	}

	public static bool operator !=(BaseScriptableObject a, BaseScriptableObject b)
	{
		return !(a == b);
	}
}