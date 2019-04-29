using System;
using UnityEngine;

[Serializable]
public class ItemAmountRanged : ItemAmount
{
	public float maxAmount = -1f;

	public ItemAmountRanged(ItemDefinition item = null, float amt = 0f, float max = -1f) : base(item, amt)
	{
		this.maxAmount = max;
	}

	public override float GetAmount()
	{
		if (this.maxAmount <= 0f || this.maxAmount <= this.amount)
		{
			return this.amount;
		}
		return UnityEngine.Random.Range(this.amount, this.maxAmount);
	}

	public override void OnAfterDeserialize()
	{
		base.OnAfterDeserialize();
	}
}