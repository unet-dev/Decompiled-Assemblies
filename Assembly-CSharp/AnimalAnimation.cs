using System;
using UnityEngine;

public class AnimalAnimation : MonoBehaviour, IClientComponent
{
	public BaseNpc Target;

	public UnityEngine.Animator Animator;

	public MaterialEffect FootstepEffects;

	public Transform[] Feet;

	public SoundDefinition saddleMovementSoundDef;

	[ReadOnly]
	public string BaseFolder;

	public AnimalAnimation()
	{
	}
}