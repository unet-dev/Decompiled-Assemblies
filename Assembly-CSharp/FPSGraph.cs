using ConVar;
using System;
using UnityEngine;

public class FPSGraph : Graph
{
	public FPSGraph()
	{
	}

	protected override Color GetColor(float value)
	{
		if (value < 10f)
		{
			return Color.red;
		}
		if (value >= 30f)
		{
			return Color.green;
		}
		return Color.yellow;
	}

	protected override float GetValue()
	{
		return 1f / UnityEngine.Time.deltaTime;
	}

	protected void OnEnable()
	{
		this.Refresh();
	}

	public void Refresh()
	{
		base.enabled = FPS.graph > 0;
		int num = Mathf.Clamp(FPS.graph, 0, Screen.width);
		int num1 = num;
		this.Resolution = num;
		this.Area.width = (float)num1;
	}
}