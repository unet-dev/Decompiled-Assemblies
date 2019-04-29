using System;
using UnityEngine;

public class SoundVoiceLimiter : MonoBehaviour, IClientComponent
{
	public int maxSimultaneousSounds = 5;

	public SoundVoiceLimiter()
	{
	}
}