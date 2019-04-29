using System;
using UnityEngine;

public class ApplyTerrainAnchors : MonoBehaviour
{
	public ApplyTerrainAnchors()
	{
	}

	protected void Awake()
	{
		BaseEntity component = base.GetComponent<BaseEntity>();
		TerrainAnchor[] terrainAnchorArray = null;
		if (component.isServer)
		{
			terrainAnchorArray = PrefabAttribute.server.FindAll<TerrainAnchor>(component.prefabID);
		}
		base.transform.ApplyTerrainAnchors(terrainAnchorArray);
		GameManager.Destroy(this, 0f);
	}
}