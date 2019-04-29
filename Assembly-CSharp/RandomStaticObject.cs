using System;
using UnityEngine;

public class RandomStaticObject : MonoBehaviour
{
	public uint Seed;

	public float Probability = 0.5f;

	public GameObject[] Candidates;

	public RandomStaticObject()
	{
	}

	protected void Start()
	{
		uint num = base.transform.position.Seed(World.Seed + this.Seed);
		if (SeedRandom.Value(ref num) > this.Probability)
		{
			for (int i = 0; i < (int)this.Candidates.Length; i++)
			{
				GameManager.Destroy(this.Candidates[i], 0f);
			}
			GameManager.Destroy(this, 0f);
			return;
		}
		int num1 = SeedRandom.Range(num, 0, base.transform.childCount);
		for (int j = 0; j < (int)this.Candidates.Length; j++)
		{
			GameObject candidates = this.Candidates[j];
			if (j != num1)
			{
				GameManager.Destroy(candidates, 0f);
			}
			else
			{
				candidates.SetActive(true);
			}
		}
		GameManager.Destroy(this, 0f);
	}
}