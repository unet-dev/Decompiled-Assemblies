using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemModConsumable : MonoBehaviour
{
	public int amountToConsume = 1;

	public float conditionFractionToLose;

	public string achievementWhenEaten;

	public List<ItemModConsumable.ConsumableEffect> effects = new List<ItemModConsumable.ConsumableEffect>();

	public ItemModConsumable()
	{
	}

	public float GetIfType(MetabolismAttribute.Type typeToPick)
	{
		for (int i = 0; i < this.effects.Count; i++)
		{
			if (this.effects[i].type == typeToPick)
			{
				return this.effects[i].amount;
			}
		}
		return 0f;
	}

	[Serializable]
	public class ConsumableEffect
	{
		public MetabolismAttribute.Type type;

		public float amount;

		public float time;

		public float onlyIfHealthLessThan;

		public ConsumableEffect()
		{
		}
	}
}