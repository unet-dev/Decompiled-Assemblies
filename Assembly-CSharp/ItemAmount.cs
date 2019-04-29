using System;
using UnityEngine;

[Serializable]
public class ItemAmount : ISerializationCallbackReceiver
{
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition itemDef;

	public float amount;

	[NonSerialized]
	public float startAmount;

	public int itemid
	{
		get
		{
			if (this.itemDef == null)
			{
				return 0;
			}
			return this.itemDef.itemid;
		}
	}

	public ItemAmount(ItemDefinition item = null, float amt = 0f)
	{
		this.itemDef = item;
		this.amount = amt;
		this.startAmount = this.amount;
	}

	public virtual float GetAmount()
	{
		return this.amount;
	}

	public virtual void OnAfterDeserialize()
	{
		this.startAmount = this.amount;
	}

	public virtual void OnBeforeSerialize()
	{
	}
}