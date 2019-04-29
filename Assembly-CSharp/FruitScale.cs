using System;
using UnityEngine;

public class FruitScale : MonoBehaviour, IClientComponent
{
	public FruitScale()
	{
	}

	public void SetProgress(float progress)
	{
		base.transform.localScale = Vector3.one * progress;
	}
}