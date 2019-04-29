using System;
using UnityEngine;

public class RandomDestroy : MonoBehaviour
{
	public uint Seed;

	public float Probability = 0.5f;

	public RandomDestroy()
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
		GameManager.Destroy(base.gameObject, 0f);
	}
}