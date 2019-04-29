using System;
using UnityEngine;

public class ScaleTransform : ScaleRenderer
{
	private Vector3 initialScale;

	public ScaleTransform()
	{
	}

	public override void GatherInitialValues()
	{
		this.initialScale = this.myRenderer.transform.localScale;
		base.GatherInitialValues();
	}

	public override void SetScale_Internal(float scale)
	{
		base.SetScale_Internal(scale);
		this.myRenderer.transform.localScale = this.initialScale * scale;
	}
}