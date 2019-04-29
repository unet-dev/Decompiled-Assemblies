using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProceduralObject : MonoBehaviour
{
	protected ProceduralObject()
	{
	}

	protected void Awake()
	{
		if (SingletonComponent<WorldSetup>.Instance == null)
		{
			return;
		}
		if (SingletonComponent<WorldSetup>.Instance.ProceduralObjects == null)
		{
			Debug.LogError("WorldSetup.Instance.ProceduralObjects is null.", this);
			return;
		}
		SingletonComponent<WorldSetup>.Instance.ProceduralObjects.Add(this);
	}

	public abstract void Process();
}