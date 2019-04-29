using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Rust/Protection Properties")]
public class ProtectionProperties : ScriptableObject
{
	[TextArea]
	public string comments;

	[Range(0f, 100f)]
	public float density = 1f;

	[ArrayIndexIsEnumRanged(enumType=typeof(DamageType), min=-4f, max=1f)]
	public float[] amounts = new float[22];

	public ProtectionProperties()
	{
	}

	public void Add(float amount)
	{
		for (int i = 0; i < (int)this.amounts.Length; i++)
		{
			this.amounts[i] += amount;
		}
	}

	public void Add(DamageType index, float amount)
	{
		this.amounts[(int)index] += amount;
	}

	public void Add(ProtectionProperties other, float scale)
	{
		for (int i = 0; i < Mathf.Min((int)other.amounts.Length, (int)this.amounts.Length); i++)
		{
			ref float singlePointer = ref this.amounts[i];
			singlePointer = singlePointer + other.amounts[i] * scale;
		}
	}

	public void Add(List<Item> items, HitArea area = -1)
	{
		for (int i = 0; i < items.Count; i++)
		{
			Item item = items[i];
			ItemModWearable component = item.info.GetComponent<ItemModWearable>();
			if (!(component == null) && component.ProtectsArea(area))
			{
				component.CollectProtection(item, this);
			}
		}
	}

	public void Clear()
	{
		for (int i = 0; i < (int)this.amounts.Length; i++)
		{
			this.amounts[i] = 0f;
		}
	}

	public float Get(DamageType damageType)
	{
		return this.amounts[(int)damageType];
	}

	public void Multiply(float multiplier)
	{
		for (int i = 0; i < (int)this.amounts.Length; i++)
		{
			this.amounts[i] *= multiplier;
		}
	}

	public void Multiply(DamageType index, float multiplier)
	{
		this.amounts[(int)index] *= multiplier;
	}

	public void OnValidate()
	{
		if ((int)this.amounts.Length < 22)
		{
			float[] singleArray = new float[22];
			for (int i = 0; i < (int)singleArray.Length; i++)
			{
				if (i < (int)this.amounts.Length)
				{
					singleArray[i] = this.amounts[i];
				}
				else if (i == 21)
				{
					singleArray[i] = this.amounts[9];
				}
			}
			this.amounts = singleArray;
		}
	}

	public void Scale(DamageTypeList damageList, float ProtectionAmount = 1f)
	{
		for (int i = 0; i < (int)this.amounts.Length; i++)
		{
			if (this.amounts[i] != 0f)
			{
				damageList.Scale((DamageType)i, 1f - this.amounts[i] * ProtectionAmount);
			}
		}
	}
}