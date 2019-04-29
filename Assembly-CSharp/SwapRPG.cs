using System;
using UnityEngine;

public class SwapRPG : MonoBehaviour
{
	public GameObject[] rpgModels;

	[NonSerialized]
	private string curAmmoType = "";

	public SwapRPG()
	{
	}

	public void SelectRPGType(int iType)
	{
		GameObject[] gameObjectArray = this.rpgModels;
		for (int i = 0; i < (int)gameObjectArray.Length; i++)
		{
			gameObjectArray[i].SetActive(false);
		}
		this.rpgModels[iType].SetActive(true);
	}

	private void Start()
	{
	}

	public void UpdateAmmoType(ItemDefinition ammoType)
	{
		if (this.curAmmoType == ammoType.shortname)
		{
			return;
		}
		this.curAmmoType = ammoType.shortname;
		string str = this.curAmmoType;
		if (str != "ammo.rocket.basic")
		{
			if (str == "ammo.rocket.fire")
			{
				this.SelectRPGType(1);
				return;
			}
			if (str == "ammo.rocket.hv")
			{
				this.SelectRPGType(2);
				return;
			}
			if (str == "ammo.rocket.smoke")
			{
				this.SelectRPGType(3);
				return;
			}
		}
		this.SelectRPGType(0);
	}

	public enum RPGType
	{
		One,
		Two,
		Three,
		Four
	}
}