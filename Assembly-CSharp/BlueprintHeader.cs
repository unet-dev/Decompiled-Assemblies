using System;
using UnityEngine;
using UnityEngine.UI;

public class BlueprintHeader : MonoBehaviour
{
	public Text categoryName;

	public Text unlockCount;

	public BlueprintHeader()
	{
	}

	public void Setup(ItemCategory name, int unlocked, int total)
	{
		this.categoryName.text = name.ToString().ToUpper();
		this.unlockCount.text = string.Format("UNLOCKED {0}/{1}", unlocked, total);
	}
}