using System;
using UnityEngine;

public class ScaleBySpeed : MonoBehaviour
{
	public float minScale = 0.001f;

	public float maxScale = 1f;

	public float minSpeed;

	public float maxSpeed = 1f;

	public MonoBehaviour component;

	public bool toggleComponent = true;

	public bool onlyWhenSubmerged;

	public float submergedThickness = 0.33f;

	private Vector3 prevPosition = Vector3.zero;

	public ScaleBySpeed()
	{
	}

	private void Start()
	{
		this.prevPosition = base.transform.position;
	}

	private void Update()
	{
		Vector3 vector3 = base.transform.position;
		float single = (vector3 - this.prevPosition).sqrMagnitude;
		float single1 = this.minScale;
		bool height = WaterSystem.GetHeight(vector3) > vector3.y - this.submergedThickness;
		if (single > 0.0001f)
		{
			single = Mathf.Sqrt(single);
			float single2 = Mathf.Clamp(single, this.minSpeed, this.maxSpeed) / (this.maxSpeed - this.minSpeed);
			single1 = Mathf.Lerp(this.minScale, this.maxScale, Mathf.Clamp01(single2));
			if (this.component != null && this.toggleComponent)
			{
				this.component.enabled = height;
			}
		}
		else if (this.component != null && this.toggleComponent)
		{
			this.component.enabled = false;
		}
		base.transform.localScale = new Vector3(single1, single1, single1);
		this.prevPosition = vector3;
	}
}