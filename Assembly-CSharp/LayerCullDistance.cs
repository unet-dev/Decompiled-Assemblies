using System;
using UnityEngine;

public class LayerCullDistance : MonoBehaviour
{
	public string Layer = "Default";

	public float Distance = 1000f;

	public LayerCullDistance()
	{
	}

	protected void OnEnable()
	{
		Camera component = base.GetComponent<Camera>();
		float[] singleArray = component.layerCullDistances;
		singleArray[LayerMask.NameToLayer(this.Layer)] = this.Distance;
		component.layerCullDistances = singleArray;
	}
}