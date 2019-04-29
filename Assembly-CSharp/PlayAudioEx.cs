using System;
using UnityEngine;

public class PlayAudioEx : MonoBehaviour
{
	public float delay;

	public PlayAudioEx()
	{
	}

	private void OnEnable()
	{
		AudioSource component = base.GetComponent<AudioSource>();
		if (component)
		{
			component.PlayDelayed(this.delay);
		}
	}

	private void Start()
	{
	}
}