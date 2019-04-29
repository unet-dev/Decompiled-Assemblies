using System;
using System.Collections.Generic;
using UnityEngine;

public class WaterCatcher : LiquidContainer
{
	[Header("Water Catcher")]
	public ItemDefinition itemToCreate;

	public float maxItemToCreate = 10f;

	[Header("Outside Test")]
	public Vector3 rainTestPosition = new Vector3(0f, 1f, 0f);

	public float rainTestSize = 1f;

	private const float collectInterval = 60f;

	public WaterCatcher()
	{
	}

	private void AddResource(int iAmount)
	{
		this.inventory.AddItem(this.itemToCreate, iAmount);
		base.UpdateOnFlag();
	}

	private void CollectWater()
	{
		if (this.IsFull())
		{
			return;
		}
		float fog = 0.25f;
		fog = fog + Climate.GetFog(base.transform.position) * 2f;
		if (this.TestIsOutside())
		{
			fog += Climate.GetRain(base.transform.position);
			fog = fog + Climate.GetSnow(base.transform.position) * 0.5f;
		}
		this.AddResource(Mathf.CeilToInt(this.maxItemToCreate * fog));
	}

	private bool IsFull()
	{
		if (this.inventory.itemList.Count == 0)
		{
			return false;
		}
		if (this.inventory.itemList[0].amount < this.inventory.maxStackSize)
		{
			return false;
		}
		return true;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		this.AddResource(1);
		base.InvokeRandomized(new Action(this.CollectWater), 60f, 60f, 6f);
	}

	private bool TestIsOutside()
	{
		Matrix4x4 matrix4x4 = base.transform.localToWorldMatrix;
		return !Physics.SphereCast(new Ray(matrix4x4.MultiplyPoint3x4(this.rainTestPosition), Vector3.up), this.rainTestSize, 256f, 1101070337);
	}
}