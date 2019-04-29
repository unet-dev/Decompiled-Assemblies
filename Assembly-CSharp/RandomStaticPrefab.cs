using System;
using UnityEngine;

public class RandomStaticPrefab : MonoBehaviour
{
	public uint Seed;

	public float Probability = 0.5f;

	public string ResourceFolder = string.Empty;

	public RandomStaticPrefab()
	{
	}

	protected void Start()
	{
		uint num = base.transform.position.Seed(World.Seed + this.Seed);
		if (SeedRandom.Value(ref num) > this.Probability)
		{
			GameManager.Destroy(this, 0f);
			return;
		}
		Prefab.LoadRandom(string.Concat("assets/bundled/prefabs/autospawn/", this.ResourceFolder), ref num, null, null, true).Spawn(base.transform);
		GameManager.Destroy(this, 0f);
	}
}