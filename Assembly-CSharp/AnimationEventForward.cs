using System;
using UnityEngine;

public class AnimationEventForward : MonoBehaviour
{
	public GameObject targetObject;

	public AnimationEventForward()
	{
	}

	public void Event(string type)
	{
		this.targetObject.SendMessage(type);
	}
}