using System;
using UnityEngine;

[Serializable]
public class ResourceRef<T>
where T : UnityEngine.Object
{
	public string guid;

	private UnityEngine.Object _cachedObject;

	public bool isValid
	{
		get
		{
			return !string.IsNullOrEmpty(this.guid);
		}
	}

	public uint resourceID
	{
		get
		{
			return StringPool.Get(this.resourcePath);
		}
	}

	public string resourcePath
	{
		get
		{
			return GameManifest.GUIDToPath(this.guid);
		}
	}

	public ResourceRef()
	{
	}

	public T Get()
	{
		if (this._cachedObject == null)
		{
			this._cachedObject = GameManifest.GUIDToObject(this.guid);
		}
		return (T)(this._cachedObject as T);
	}
}