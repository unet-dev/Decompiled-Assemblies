using System;
using UnityEngine;

public class ExplosionDemoReactivator : MonoBehaviour
{
	public float TimeDelayToReactivate = 3f;

	public ExplosionDemoReactivator()
	{
	}

	private void Reactivate()
	{
		Transform[] componentsInChildren = base.GetComponentsInChildren<Transform>();
		for (int i = 0; i < (int)componentsInChildren.Length; i++)
		{
			Transform transforms = componentsInChildren[i];
			transforms.gameObject.SetActive(false);
			transforms.gameObject.SetActive(true);
		}
	}

	private void Start()
	{
		base.InvokeRepeating("Reactivate", 0f, this.TimeDelayToReactivate);
	}
}