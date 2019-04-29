using Facepunch;
using System;
using UnityEngine;

public class UIPrefab : MonoBehaviour
{
	public GameObject prefabSource;

	internal GameObject createdGameObject;

	public UIPrefab()
	{
	}

	private void Awake()
	{
		if (this.prefabSource == null)
		{
			return;
		}
		if (this.createdGameObject != null)
		{
			return;
		}
		this.createdGameObject = Instantiate.GameObject(this.prefabSource, null);
		this.createdGameObject.name = this.prefabSource.name;
		this.createdGameObject.transform.SetParent(base.transform, false);
		this.createdGameObject.Identity();
	}

	public void SetVisible(bool visible)
	{
		if (this.createdGameObject == null)
		{
			return;
		}
		if (this.createdGameObject.activeSelf == visible)
		{
			return;
		}
		this.createdGameObject.SetActive(visible);
	}
}