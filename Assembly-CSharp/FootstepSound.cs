using System;
using UnityEngine;

public class FootstepSound : MonoBehaviour, IClientComponent
{
	public SoundDefinition lightSound;

	public SoundDefinition medSound;

	public SoundDefinition hardSound;

	private const float panAmount = 0.05f;

	public FootstepSound()
	{
	}

	public enum Hardness
	{
		Light = 1,
		Medium = 2,
		Hard = 3
	}
}