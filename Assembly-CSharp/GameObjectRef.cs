using Facepunch;
using System;
using UnityEngine;

[Serializable]
public class GameObjectRef : ResourceRef<GameObject>
{
	public GameObjectRef()
	{
	}

	public GameObject Instantiate(Transform parent = null)
	{
		return Instantiate.GameObject(base.Get(), parent);
	}
}