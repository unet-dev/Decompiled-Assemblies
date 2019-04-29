using System;
using System.Collections.Generic;
using UnityEngine;

public class UIBlackoutOverlay : MonoBehaviour
{
	public CanvasGroup @group;

	public static Dictionary<UIBlackoutOverlay.blackoutType, UIBlackoutOverlay> instances;

	public UIBlackoutOverlay.blackoutType overlayType = UIBlackoutOverlay.blackoutType.NONE;

	public UIBlackoutOverlay()
	{
	}

	public enum blackoutType
	{
		FULLBLACK = 0,
		BINOCULAR = 1,
		SCOPE = 2,
		HELMETSLIT = 3,
		SNORKELGOGGLE = 4,
		NONE = 64
	}
}