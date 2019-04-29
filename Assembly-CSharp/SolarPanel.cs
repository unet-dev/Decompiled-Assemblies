using System;
using UnityEngine;

public class SolarPanel : IOEntity
{
	public Transform sunSampler;

	private const int tickrateSeconds = 60;

	public int maximalPowerOutput = 10;

	public float dot_minimum = 0.1f;

	public float dot_maximum = 0.6f;

	public SolarPanel()
	{
	}

	public override int ConsumptionAmount()
	{
		return 0;
	}

	public override bool IsRootEntity()
	{
		return true;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRandomized(new Action(this.SunUpdate), 1f, 5f, 2f);
	}

	public void SunUpdate()
	{
		int num = this.currentEnergy;
		if (!TOD_Sky.Instance.IsNight)
		{
			Vector3 sun = TOD_Sky.Instance.Components.Sun.transform.position - this.sunSampler.transform.position;
			Vector3 vector3 = sun.normalized;
			float single = Vector3.Dot(this.sunSampler.transform.forward, vector3);
			float single1 = Mathf.InverseLerp(this.dot_minimum, this.dot_maximum, single);
			if (single1 > 0f && !base.IsVisible(this.sunSampler.transform.position + (vector3 * 100f), 101f))
			{
				single1 = 0f;
			}
			num = Mathf.FloorToInt((float)this.maximalPowerOutput * single1 * base.healthFraction);
		}
		else
		{
			num = 0;
		}
		bool flag = this.currentEnergy != num;
		this.currentEnergy = num;
		if (flag)
		{
			this.MarkDirty();
		}
	}
}