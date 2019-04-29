using System;
using UnityEngine;
using UnityEngine.UI;

public class LookatTooltip : MonoBehaviour
{
	public static bool Enabled;

	public Animator tooltipAnimator;

	public BaseEntity currentlyLookingAt;

	public Text textLabel;

	public Image icon;

	static LookatTooltip()
	{
		LookatTooltip.Enabled = true;
	}

	public LookatTooltip()
	{
	}
}