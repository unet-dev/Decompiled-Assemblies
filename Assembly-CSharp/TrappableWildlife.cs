using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Rust/TrappableWildlife")]
[Serializable]
public class TrappableWildlife : ScriptableObject
{
	public GameObjectRef worldObject;

	public ItemDefinition inventoryObject;

	public int minToCatch;

	public int maxToCatch;

	public List<TrappableWildlife.BaitType> baitTypes;

	public int caloriesForInterest = 20;

	public float successRate = 1f;

	public float xpScale = 1f;

	public TrappableWildlife()
	{
	}

	[Serializable]
	public class BaitType
	{
		public float successRate;

		public ItemDefinition bait;

		public int minForInterest;

		public int maxToConsume;

		public BaitType()
		{
		}
	}
}