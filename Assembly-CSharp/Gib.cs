using System;
using UnityEngine;

public class Gib : MonoBehaviour
{
	public static int gibCount;

	static Gib()
	{
	}

	public Gib()
	{
	}

	public static string GetEffect(PhysicMaterial physicMaterial)
	{
		string nameLower = physicMaterial.GetNameLower();
		if (nameLower == "wood")
		{
			return "assets/bundled/prefabs/fx/building/wood_gib.prefab";
		}
		if (nameLower == "concrete")
		{
			return "assets/bundled/prefabs/fx/building/stone_gib.prefab";
		}
		if (nameLower == "metal")
		{
			return "assets/bundled/prefabs/fx/building/metal_sheet_gib.prefab";
		}
		if (nameLower == "rock")
		{
			return "assets/bundled/prefabs/fx/building/stone_gib.prefab";
		}
		if (nameLower == "flesh")
		{
			return "assets/bundled/prefabs/fx/building/wood_gib.prefab";
		}
		return "assets/bundled/prefabs/fx/building/wood_gib.prefab";
	}
}