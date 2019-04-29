using System;
using UnityEngine;

public class LineRendererActivate : MonoBehaviour, IClientComponent
{
	public LineRendererActivate()
	{
	}

	private void OnEnable()
	{
		base.GetComponent<LineRenderer>().enabled = true;
	}
}