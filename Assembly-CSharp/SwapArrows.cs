using Rust;
using System;
using UnityEngine;

public class SwapArrows : MonoBehaviour, IClientComponent
{
	public GameObject[] arrowModels;

	[NonSerialized]
	private string curAmmoType = "";

	public SwapArrows()
	{
	}

	private void Cleanup()
	{
		this.HideAllArrowHeads();
		this.curAmmoType = "";
	}

	public void HideAllArrowHeads()
	{
		GameObject[] gameObjectArray = this.arrowModels;
		for (int i = 0; i < (int)gameObjectArray.Length; i++)
		{
			gameObjectArray[i].SetActive(false);
		}
	}

	public void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		this.Cleanup();
	}

	public void OnEnable()
	{
		this.Cleanup();
	}

	public void SelectArrowType(int iType)
	{
		this.HideAllArrowHeads();
		if (iType < (int)this.arrowModels.Length)
		{
			this.arrowModels[iType].SetActive(true);
		}
	}

	public void UpdateAmmoType(ItemDefinition ammoType)
	{
		if (this.curAmmoType == ammoType.shortname)
		{
			return;
		}
		this.curAmmoType = ammoType.shortname;
		string str = this.curAmmoType;
		if (str != "ammo_arrow")
		{
			if (str == "arrow.bone")
			{
				this.SelectArrowType(0);
				return;
			}
			if (str == "arrow.fire")
			{
				this.SelectArrowType(1);
				return;
			}
			if (str == "arrow.hv")
			{
				this.SelectArrowType(2);
				return;
			}
			if (str == "ammo_arrow_poison")
			{
				this.SelectArrowType(3);
				return;
			}
			if (str == "ammo_arrow_stone")
			{
				this.SelectArrowType(4);
				return;
			}
		}
		this.HideAllArrowHeads();
	}

	public enum ArrowType
	{
		One,
		Two,
		Three,
		Four
	}
}