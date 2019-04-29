using System;
using UnityEngine;

[Serializable]
public class ItemAmountRandom
{
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition itemDef;

	public AnimationCurve amount = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(1f, 1f) });

	public ItemAmountRandom()
	{
	}

	public int RandomAmount()
	{
		return Mathf.RoundToInt(this.amount.Evaluate(UnityEngine.Random.Range(0f, 1f)));
	}
}