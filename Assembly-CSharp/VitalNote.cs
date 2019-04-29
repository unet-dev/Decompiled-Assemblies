using System;
using UnityEngine;
using UnityEngine.UI;

public class VitalNote : MonoBehaviour, IClientComponent
{
	public VitalNote.Vital VitalType;

	public FloatConditions showIf;

	public Text valueText;

	public Animator animator;

	public VitalNote()
	{
	}

	public enum Vital
	{
		Comfort,
		Radiation,
		Poison,
		Cold,
		Bleeding,
		Hot,
		Drowning,
		Wet,
		Hygiene,
		Starving,
		Dehydration
	}
}