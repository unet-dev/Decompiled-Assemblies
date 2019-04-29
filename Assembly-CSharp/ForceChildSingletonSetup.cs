using System;
using UnityEngine;

public class ForceChildSingletonSetup : MonoBehaviour
{
	public ForceChildSingletonSetup()
	{
	}

	[ComponentHelp("Any child objects of this object that contain SingletonComponents will be registered - even if they're not enabled")]
	private void Awake()
	{
		SingletonComponent[] componentsInChildren = base.GetComponentsInChildren<SingletonComponent>(true);
		for (int i = 0; i < (int)componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Setup();
		}
	}
}