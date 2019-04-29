using System;
using UnityEngine;

public class ApplyTerrainModifiers : MonoBehaviour
{
	public ApplyTerrainModifiers()
	{
	}

	protected void Awake()
	{
		BaseEntity component = base.GetComponent<BaseEntity>();
		TerrainModifier[] terrainModifierArray = null;
		if (component.isServer)
		{
			terrainModifierArray = PrefabAttribute.server.FindAll<TerrainModifier>(component.prefabID);
		}
		base.transform.ApplyTerrainModifiers(terrainModifierArray);
		GameManager.Destroy(this, 0f);
	}
}