using System;
using UnityEngine;

public abstract class TerrainModifier : PrefabAttribute
{
	public float Opacity = 1f;

	public float Radius;

	public float Fade;

	protected TerrainModifier()
	{
	}

	public void Apply(Vector3 pos, float scale)
	{
		float opacity = this.Opacity;
		float single = scale * this.Radius;
		this.Apply(pos, opacity, single, scale * this.Fade);
	}

	protected abstract void Apply(Vector3 position, float opacity, float radius, float fade);

	protected override Type GetIndexedType()
	{
		return typeof(TerrainModifier);
	}

	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0.5f, 0.5f, 0.5f, this.Opacity);
		GizmosUtil.DrawWireCircleY(base.transform.position, base.transform.lossyScale.y * this.Radius);
	}
}