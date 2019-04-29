using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemModConsumable : MonoBehaviour
{
	public int amountToConsume = 1;

	public float conditionFractionToLose;

	public List<ItemModConsumable.ConsumableEffect> effects = new List<ItemModConsumable.ConsumableEffect>();

	public ItemModConsumable()
	{
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