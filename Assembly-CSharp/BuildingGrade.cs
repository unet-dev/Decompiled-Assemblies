using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Rust/Building Grade")]
public class BuildingGrade : ScriptableObject
{
	public BuildingGrade.Enum type;

	public float baseHealth;

	public List<ItemAmount> baseCost;

	public PhysicMaterial physicMaterial;

	public ProtectionProperties damageProtecton;

	public BaseEntity.Menu.Option upgradeMenu;

	public BuildingGrade()
	{
	}

	public enum Enum
	{
		None = -1,
		Twigs = 0,
		Wood = 1,
		Stone = 2,
		Metal = 3,
		TopTier = 4,
		Count = 5
	}
}