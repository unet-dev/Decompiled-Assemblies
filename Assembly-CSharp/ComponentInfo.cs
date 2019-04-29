using System;
using UnityEngine;

public abstract class ComponentInfo : MonoBehaviour
{
	protected ComponentInfo()
	{
	}

	public abstract void Reset();

	public abstract void Setup();
}