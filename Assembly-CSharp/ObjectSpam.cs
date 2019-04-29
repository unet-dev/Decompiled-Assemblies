using System;
using UnityEngine;

public class ObjectSpam : MonoBehaviour
{
	public GameObject source;

	public int amount = 1000;

	public float radius;

	public ObjectSpam()
	{
	}

	private void Start()
	{
		for (int i = 0; i < this.amount; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.source);
			gameObject.transform.position = base.transform.position + Vector3Ex.Range(-this.radius, this.radius);
			gameObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
		}
	}
}