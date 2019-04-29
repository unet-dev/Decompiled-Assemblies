using System;
using UnityEngine;

public class ExplosionsShaderColorGradient : MonoBehaviour
{
	public string ShaderProperty = "_TintColor";

	public int MaterialID;

	public Gradient Color = new Gradient();

	public float TimeMultiplier = 1f;

	private bool canUpdate;

	private Material matInstance;

	private int propertyID;

	private float startTime;

	private UnityEngine.Color oldColor;

	public ExplosionsShaderColorGradient()
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
		this.oldColor = this.matInstance.GetColor(this.propertyID);
	}

	private void Update()
	{
		float single = Time.time - this.startTime;
		if (this.canUpdate)
		{
			UnityEngine.Color color = this.Color.Evaluate(single / this.TimeMultiplier);
			this.matInstance.SetColor(this.propertyID, color * this.oldColor);
		}
		if (single >= this.TimeMultiplier)
		{
			this.canUpdate = false;
		}
	}
}