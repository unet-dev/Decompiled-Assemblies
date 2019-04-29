using System;
using UnityEngine;
using UnityEngine.UI;

public class LookatHealth : MonoBehaviour
{
	public static bool Enabled;

	public GameObject container;

	public Text textHealth;

	public Text textStability;

	public Image healthBar;

	public Image healthBarBG;

	public Color barBGColorNormal;

	public Color barBGColorUnstable;

	static LookatHealth()
	{
		LookatHealth.Enabled = true;
	}

	public LookatHealth()
	{
	}
}