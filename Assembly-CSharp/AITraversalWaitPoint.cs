using System;
using UnityEngine;

public class AITraversalWaitPoint : MonoBehaviour
{
	public float nextFreeTime;

	public AITraversalWaitPoint()
	{
	}

	public bool Occupied()
	{
		return Time.time > this.nextFreeTime;
	}

	public void Occupy(float dur = 1f)
	{
		this.nextFreeTime = Time.time + dur;
	}
}