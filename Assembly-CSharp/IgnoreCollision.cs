using System;
using UnityEngine;

public class IgnoreCollision : MonoBehaviour
{
	public Collider collider;

	public IgnoreCollision()
	{
	}

	protected void OnTriggerEnter(Collider other)
	{
		Debug.Log(string.Concat("IgnoreCollision: ", this.collider.gameObject.name, " + ", other.gameObject.name));
		Physics.IgnoreCollision(other, this.collider, true);
	}
}