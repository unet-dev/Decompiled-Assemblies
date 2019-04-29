using System;
using UnityEngine;

public class TimedRemoval : MonoBehaviour
{
	public UnityEngine.Object objectToDestroy;

	public float removeDelay = 1f;

	public TimedRemoval()
	{
	}

	private void OnEnable()
	{
		UnityEngine.Object.Destroy(this.objectToDestroy, this.removeDelay);
	}
}