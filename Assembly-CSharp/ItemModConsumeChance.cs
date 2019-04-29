using System;
using UnityEngine;

public class ItemModConsumeChance : ItemModConsume
{
	public float chanceForSecondaryConsume = 0.5f;

	public GameObjectRef secondaryConsumeEffect;

	public ItemModConsumable secondaryConsumable;

	public ItemModConsumeChance()
	{
	}

	private bool GetChance()
	{
		UnityEngine.Random.State state = UnityEngine.Random.state;
		UnityEngine.Random.InitState(Time.frameCount);
		UnityEngine.Random.state = state;
		return UnityEngine.Random.Range(0f, 1f) <= this.chanceForSecondaryConsume;
	}

	public override ItemModConsumable GetConsumable()
	{
		if (this.GetChance())
		{
			return this.secondaryConsumable;
		}
		return base.GetConsumable();
	}

	public override GameObjectRef GetConsumeEffect()
	{
		if (this.GetChance())
		{
			return this.secondaryConsumeEffect;
		}
		return base.GetConsumeEffect();
	}
}