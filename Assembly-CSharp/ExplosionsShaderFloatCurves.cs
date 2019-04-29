using System;
using UnityEngine;

public class ExplosionsShaderFloatCurves : MonoBehaviour
{
	public string ShaderProperty = "_BumpAmt";

	public int MaterialID;

	public AnimationCurve FloatPropertyCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	public float GraphTimeMultiplier = 1f;

	public float GraphScaleMultiplier = 1f;

	private bool canUpdate;

	private Material matInstance;

	private int propertyID;

	private float startTime;

	public ExplosionsShaderFloatCurves()
	{
	}

	private void OnEnable()
	{
		this.startTime = Time.time;
		this.canUpdate = true;
	}

	private void Start()
	{
		Material[] component = base.GetComponent<Renderer>().materials;
		if (this.MaterialID >= (int)component.Length)
		{
			Debug.Log("ShaderColorGradient: Material ID more than shader materials count.");
		}
		this.matInstance = component[this.MaterialID];
		if (!this.matInstance.HasProperty(this.ShaderProperty))
		{
			Debug.Log(string.Concat("ShaderColorGradient: Shader not have \"", this.ShaderProperty, "\" property"));
		}
		this.propertyID = Shader.PropertyToID(this.ShaderProperty);
	}

	private void Update()
	{
		float single = Time.time - this.startTime;
		if (this.canUpdate)
		{
			float single1 = this.FloatPropertyCurve.Evaluate(single / this.GraphTimeMultiplier) * this.GraphScaleMultiplier;
			this.matInstance.SetFloat(this.propertyID, single1);
		}
		if (single >= this.GraphTimeMultiplier)
		{
			this.canUpdate = false;
		}
	}
}