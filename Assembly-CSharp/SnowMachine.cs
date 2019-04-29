using System;
using UnityEngine;

public class SnowMachine : FogMachine
{
	public AdaptMeshToTerrain snowMesh;

	public TriggerTemperature tempTrigger;

	public SnowMachine()
	{
	}

	public override void EnableFogField()
	{
		base.EnableFogField();
		this.tempTrigger.gameObject.SetActive(true);
	}

	public override void FinishFogging()
	{
		base.FinishFogging();
		this.tempTrigger.gameObject.SetActive(false);
	}

	public override bool MotionModeEnabled()
	{
		return false;
	}
}